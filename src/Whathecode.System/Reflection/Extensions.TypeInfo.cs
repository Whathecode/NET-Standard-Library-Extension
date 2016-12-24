using System;
using System.Linq;
using System.Reflection;
using Whathecode.System.Linq;


namespace Whathecode.System.Reflection
{
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
			if ( source == null || typeArguments == null )
			{
				throw new ArgumentNullException( "" );
			}
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
		/// Get the first found matching generic type. The type parameters of the generic type are optional: e.g., Dictionary&lt;,&gt;.
		/// When full (generic or non-generic) type is known (e.g., Dictionary&lt;string,string&gt;),
		/// the "is" operator is most likely more performant, but this function will still work correctly.
		/// </summary>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "type">The type to check for.</param>
		/// <returns>The first found matching complete generic type, or null when no matching type found.</returns>
		public static TypeInfo GetMatchingGenericType( this TypeInfo source, TypeInfo type )
		{
			if ( source == null || type == null )
			{
				throw new ArgumentNullException( "All arguments should be non-null." );
			}

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
				while ( baseType != null )
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
		/// Is the type of a given generic type or not. Also works for raw generic types: e.g., Dictionary&lt;,&gt;.
		/// When full (generic or non-generic) type is known (e.g., Dictionary&lt;string,string&gt;),
		/// the "is" operator is most likely more performant, but this function will still work correctly.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "type">The type to check for.</param>
		/// <returns>True when the type is of the given type, false otherwise.</returns>
		public static bool IsOfGenericType( this TypeInfo source, TypeInfo type )
		{
			if ( source == null || type == null )
			{
				throw new ArgumentNullException( "All arguments should be non-null." );
			}

			return GetMatchingGenericType( source, type ) != null;
		}

		/// <summary>
		/// Verify whether the type is an enum with the <see cref="FlagsAttribute" /> applied.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a flags enum, false otherwise.</returns>
		public static bool IsFlagsEnum( this TypeInfo source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( nameof( source ) );
			}

			return source.IsEnum && source.GetCustomAttributes<FlagsAttribute>().Count() != 0;
		}

		/// <summary>
		/// Verify whether the type is a delegate.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a delegate, false otherwise.</returns>
		public static bool IsDelegate( this TypeInfo source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( nameof( source ) );
			}

			return source.IsSubclassOf( typeof( Delegate ) );
		}

		/// <summary>
		/// Verify whether the given type is static. Only classes can be static.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <returns>True when the given type is a static class, false otherwise.</returns>
		public static bool IsStatic( this TypeInfo source )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( nameof( source ) );
			}

			// Static classes are declared abstract and sealed at the IL level.
			return source.IsAbstract && source.IsSealed;
		}

		/// <summary>
		/// Does the type implement a given interface or not.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "interfaceType">The interface type to check for.</param>
		/// <returns>True when the type implements the given interface, false otherwise.</returns>
		public static bool ImplementsInterface( this TypeInfo source, TypeInfo interfaceType )
		{
			if ( source == null || interfaceType == null )
			{
				throw new ArgumentNullException( "All arguments should be non-null." );
			}
			if ( !interfaceType.IsInterface )
			{
				throw new ArgumentException( "The passed type is not an interface.", nameof( interfaceType ) );
			}

			return source.GetInterfaces().Contains( interfaceType.AsType() );
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
	}
}