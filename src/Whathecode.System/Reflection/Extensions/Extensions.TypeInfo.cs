using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Whathecode.System.Linq;


namespace Whathecode.System.Reflection.Extensions
{
	/// <summary>
	/// TODO: Some consistency is needed here when TypeInfo is used/returned and when Type.
	/// </summary>
	public static partial class Extensions
	{
		/// <summary>
		/// Returns a <see cref="TypeInfo" /> object based on the <see cref="TypeInfo" /> object
		/// with its type parameters replaced with the given type arguments.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "typeArguments">
		/// An array of types to be substituted for the type parameters of the given <see cref="TypeInfo" />.
		/// </param>
		/// <returns>
		/// A <see cref="TypeInfo" /> object that represents the constructed type formed by substituting
		/// the elements of <paramref name="typeArguments" /> for the type parameters of the current type definition.
		/// </returns>
		public static TypeInfo GetGenericTypeDefinition( this TypeInfo source, params Type[] typeArguments )
		{
			if ( !source.IsGenericType )
			{
				throw new ArgumentException( "The type is not a generic type.", nameof( source ) );
			}

			return source
				.GetGenericTypeDefinition()
				.MakeGenericType( typeArguments )
				.GetTypeInfo();
		}

		/// <summary>
		/// Determines whether a conversion from one type to another is possible.
		/// This uses .NET rules. E.g. short is not implicitly convertible to int, while this is possible in C#.
		/// TODO: Support constraints, custom implicit conversion operators? Unit tests for explicit converts.
		/// </summary>
		/// <param name = "fromType">The type to convert from.</param>
		/// <param name = "targetType">The type to convert to.</param>
		/// <param name = "castType">Specifies what types of casts should be considered.</param>
		/// <returns>true when a conversion to the target type is possible, false otherwise.</returns>
		public static bool CanConvertTo( this TypeInfo fromType, TypeInfo targetType, CastType castType = CastType.Implicit )
		{
			return CanConvertTo( fromType, targetType, castType, false );
		}

		static bool CanConvertTo( this TypeInfo fromType, TypeInfo targetType, CastType castType, bool switchVariance )
		{
			bool sameHierarchy = castType == CastType.SameHierarchy;

			Func<TypeInfo, TypeInfo, bool> covarianceCheck = sameHierarchy
				? (Func<TypeInfo, TypeInfo, bool>)IsInHierarchy
				: ( from, to ) => from == to || from.IsSubclassOf( to.AsType() );
			Func<TypeInfo, TypeInfo, bool> contravarianceCheck = sameHierarchy
				? (Func<TypeInfo, TypeInfo, bool>)IsInHierarchy
				: ( from, to ) => from == to || to.IsSubclassOf( from.AsType() );

			if ( switchVariance )
			{
				Variable.Swap( ref covarianceCheck, ref contravarianceCheck );
			}

			// Simple hierarchy check.
			if ( covarianceCheck( fromType, targetType ) )
			{
				return true;
			}

			// Interface check.
			if ( (targetType.IsInterface && fromType.ImplementsInterface( targetType ))
				|| (sameHierarchy && fromType.IsInterface && targetType.ImplementsInterface( fromType )) )
			{
				return true;
			}

			// Explicit value type conversions (including enums).
			if ( sameHierarchy && (fromType.IsValueType && targetType.IsValueType) )
			{
				return true;
			}

			// Recursively verify when it is a generic type.
			if ( targetType.IsGenericType )
			{
				TypeInfo genericDefinition = targetType.GetGenericTypeDefinition().GetTypeInfo();
				TypeInfo sourceGeneric = fromType.GetMatchingGenericType( genericDefinition );

				// Delegates never support casting in the 'opposite' direction than their varience type parameters dictate.
				CastType cast = fromType.IsDelegate() ? CastType.Implicit : castType;

				if ( sourceGeneric != null ) // Same generic types.
				{
					// Check whether parameters correspond, taking into account variance rules.
					return sourceGeneric.GetGenericArguments().Select( s => s.GetTypeInfo() ).Zip(
						targetType.GetGenericArguments().Select( t => t.GetTypeInfo() ), genericDefinition.GetGenericArguments().Select( g => g.GetTypeInfo() ),
						( from, to, generic )
							=> !(from.IsValueType || to.IsValueType)	// Variance applies only to reference types.
								? generic.GenericParameterAttributes.HasFlag( GenericParameterAttributes.Covariant )
									? CanConvertTo( from, to, cast, false )
									: generic.GenericParameterAttributes.HasFlag( GenericParameterAttributes.Contravariant )
										? CanConvertTo( from, to, cast, true )
										: false
								: false )
						.All( match => match );
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether one type is in the same inheritance hierarchy than another.
		/// </summary>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "type">The type the check whether it is in the same inheritance hierarchy.</param>
		/// <returns>true when both types are in the same inheritance hierarchy, false otherwise.</returns>
		public static bool IsInHierarchy( this TypeInfo source, TypeInfo type )
		{
			return source == type || source.IsSubclassOf( type.AsType() ) || type.IsSubclassOf( source.AsType() );
		}

		/// <summary>
		/// Get the first found matching generic type.
		/// The type parameters of the generic type are optional.
		/// E.g., Dictionary&lt;,&gt; or Dictionary&lt;string,&gt;
		/// When full (generic) type is known (e.g., Dictionary&lt;string,string&gt;),
		/// the "is" operator is most likely more performant, but this function will still work correctly.
		/// </summary>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "type">The type to check for.</param>
		/// <returns>The first found matching complete generic type, or null when no matching type found.</returns>
		public static TypeInfo GetMatchingGenericType( this TypeInfo source, TypeInfo type )
		{
			Type[] genericArguments = type.GetGenericArguments();
			Type rawType = type.IsGenericType ? type.GetGenericTypeDefinition() : type.AsType();

			// Used to compare type arguments and see whether they match.
			Func<Type[], bool> argumentsMatch
				= arguments => genericArguments
					.Zip( arguments, Tuple.Create )
					.All( t => t.Item1.IsGenericParameter // No type specified.
						|| t.Item1 == t.Item2 );

			TypeInfo matchingType = null;
			if ( type.IsInterface && !source.IsInterface )
			{
				// Traverse across all interfaces to find a matching interface.
				matchingType = (
					from t in source.GetInterfaces().Select( i => i.GetTypeInfo() )
					let rawInterface = t.IsGenericType ? t.GetGenericTypeDefinition() : t.AsType()
					where rawInterface == rawType && argumentsMatch( t.GetGenericArguments() )
					select t
					).FirstOrDefault();
			}
			else
			{
				// Traverse across the type, and all it's base types.
				Type baseType = source.AsType();
				while ( baseType != null && baseType != typeof( object ) )
				{
					TypeInfo info = baseType.GetTypeInfo();
					Type rawCurrent = info.IsGenericType ? baseType.GetGenericTypeDefinition() : baseType;
					if ( rawType == rawCurrent )
					{
						// Same raw generic type, compare type arguments.
						if ( argumentsMatch( info.GetGenericArguments() ) )
						{
							matchingType = info;
							break;
						}
					}
					baseType = info.BaseType;
				}
			}

			return matchingType;
		}

		/// <summary>
		/// Is the type of a given generic type or not.
		/// Also works for raw generic types. E.g. Dictionary&lt;,&gt;.
		/// When full (generic) type is known (e.g. Dictionary&lt;string,string&gt;),
		/// the "is" operator is most likely more performant, but this function will still work correctly.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "type">The type to check for.</param>
		/// <returns>True when the type is of the given type, false otherwise.</returns>
		public static bool IsOfGenericType( this TypeInfo source, TypeInfo type )
		{
			return GetMatchingGenericType( source, type ) != null;
		}

		/// <summary>
		/// Verify whether the type is an enum with the <see cref="FlagsAttribute" /> applied.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a flags enum, false otherwise.</returns>
		public static bool IsFlagsEnum( this TypeInfo source )
		{
			return source.IsEnum && source.GetAttributes<FlagsAttribute>().Length != 0;
		}

		/// <summary>
		/// Verify whether the type is a delegate.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a delegate, false otherwise.</returns>
		public static bool IsDelegate( this TypeInfo source )
		{
			return source.IsSubclassOf( typeof( Delegate ) );
		}

		/// <summary>
		/// Verify whether the type is a numeric type.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a numeric type, false otherwise.</returns>
		public static bool IsNumericType( this TypeInfo source )
		{
			// All primitive types except bool are numeric.
			if ( source.IsPrimitive )
			{				
				return source.AsType() != typeof( bool );
			}

			// Check whether all numeric operators are available.
			return Operator.NumericalOperators.All( source.HasOperator );
		}

		/// <summary>
		/// Verify whether the given type is static. Only classes can be static.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a static class, false otherwise.</returns>
		public static bool IsStatic( this TypeInfo source )
		{
			// Static classes are declared abstract and sealed at the IL level.
			return source.IsAbstract && source.IsSealed;
		}

		/// <summary>
		/// Verify whether the type supports a certain operator.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "operator">The operator to check for.</param>
		/// <returns>True when the type supports the operator, false otherwise.</returns>
		public static bool HasOperator( this TypeInfo source, Operator @operator )
		{
			var defaultValue = Expression.Default( source.AsType() );

			var binaryOperator = @operator as BinaryOperator;
			if ( binaryOperator != null )
			{
				try
				{
					binaryOperator.GetExpression()( defaultValue, defaultValue ); // Throws an exception if operator is not defined.
					return true;
				}
				catch
				{
					return false;
				}								
			}

			var unaryOperator = @operator as UnaryOperator;
			if ( unaryOperator != null )
			{
				try
				{
					unaryOperator.GetExpression()( defaultValue ); // Throws an exception if operator is not defined.
					return true;
				}
				catch
				{
					return false;
				}					
			}

			throw new NotSupportedException( String.Format( "Operator \"{0}\" isn't supported.", @operator ) );
		}

		/// <summary>
		/// Does the type implement a given interface or not.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "interfaceType">The interface type to check for.</param>
		/// <returns>True when the type implements the given interface, false otherwise.</returns>
		public static bool ImplementsInterface( this TypeInfo source, TypeInfo interfaceType )
		{
			if ( !interfaceType.IsInterface )
			{
				throw new ArgumentException( "The passed type is not an interface.", nameof( interfaceType ) );
			}

			return source.GetInterface( interfaceType.ToString() ) != null;
		}

		/// <summary>
		/// Create a default initialisation of this object type.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>The default initialisation for the given objectType.</returns>
		public static object CreateDefault( this TypeInfo source )
		{
			return (source.IsValueType ? Activator.CreateInstance( source.AsType() ) : null);
		}

		/// <summary>
		/// Return all members of a specific type in a certain type.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "type">The type to search for.</param>
		/// <returns>A list of all object members with the specific type.</returns>
		public static IEnumerable<MemberInfo> GetMembers( this TypeInfo source, TypeInfo type )
		{
			return
				from m in source.GetMembers( ReflectionHelper.FlattenedClassMembers )
				where m is FieldInfo || m is PropertyInfo || m is EventInfo
				where m.GetMemberType().GetTypeInfo().IsOfGenericType( type )
				select m;
		}

		/// <summary>
		/// Returns all members which have a specified attribute annotated to them.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "memberTypes">The type of members to search in.</param>
		/// <param name = "inherit">Specifies whether to search this member's inheritance chain to find the attributes.</param>
		/// <param name = "bindingFlags">
		/// A bitmask comprised of one or more <see cref="BindingFlags" /> that specify how the search is conducted.
		/// -or-
		/// Zero, to return null.
		/// </param>
		/// <typeparam name = "TAttribute">The type of the attributes to search for.</typeparam>
		/// <returns>A dictionary containing all members with their attached attributes.</returns>
		public static Dictionary<MemberInfo, TAttribute[]> GetAttributedMembers<TAttribute>(
			this TypeInfo source,
			MemberTypes memberTypes = MemberTypes.All,
			bool inherit = false,
			BindingFlags bindingFlags = ReflectionHelper.FlattenedClassMembers )
			where TAttribute : Attribute
		{
			return (
				from member in source.GetMembers( bindingFlags )
				where member.MemberType.HasFlag( memberTypes )
				let attributes = member.GetAttributes<TAttribute>( inherit )
				where attributes.Length > 0
				select new { Member = member, Attributes = attributes }
				).ToDictionary( g => g.Member, g => g.Attributes );
		}

		/// <summary>
		/// Searches for all methods defined in an interface and its inherited interfaces.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "bindingFlags">
		/// A bitmask comprised of one or more <see cref="BindingFlags" /> that specify how the search is conducted.
		/// -or-
		/// Zero, to return null.
		/// </param>
		/// <returns>A list of all found methods.</returns>
		public static IEnumerable<MethodInfo> GetFlattenedInterfaceMethods( this TypeInfo source, BindingFlags bindingFlags )
		{
			foreach ( var info in source.GetMethods( bindingFlags ) )
			{
				yield return info;
			}

			var flattened = source.GetInterfaces().SelectMany( interfaceType => GetFlattenedInterfaceMethods( interfaceType.GetTypeInfo(), bindingFlags ) );
			foreach ( var info in flattened )
			{
				yield return info;
			}
		}
	}
}