using System;
using System.Linq;
using System.Reflection;
using Whathecode.System;
using Whathecode.System.Collections.Generic;
using Whathecode.System.Reflection.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	/// <summary>
	/// TODO: Since these operations were moved to TypeInfo rather than Type, quite a bit of boilerplate 'ToTypeInfo()' method calls were added. Can this be cleaned up?
	/// </summary>
	public class TypeInfoExtensionsTest
	{
		#region Common test members

		class Simple {}

		interface IOne<T> {}
		class One<T> : IOne<T> {}
		class ExtendingOne<T> : One<T> {}

		interface ICovariantOne<out T> {}
		interface IContravariantOne<in T> {}
		class CovariantOne<T> : ICovariantOne<T> {}
		class ContravariantOne<T> : IContravariantOne<T> {}

		delegate bool TestDelegate( int test );

		// Types.
		readonly TypeInfo _int = typeof( int ).GetTypeInfo();
		readonly TypeInfo _short = typeof( short ).GetTypeInfo();
		readonly TypeInfo _string = typeof( string ).GetTypeInfo();
		readonly TypeInfo _object = typeof( object ).GetTypeInfo();
		readonly TypeInfo _simple = typeof( Simple ).GetTypeInfo();
		readonly TypeInfo _comparable = typeof( IComparable ).GetTypeInfo();

		/// <summary>
		/// List which groups the type from which to which to convert,
		/// together with the expected outcome for an implicit and explicit conversion. (success/failure)
		/// </summary>
		class CanConvert : TupleList<TypeInfo, TypeInfo, bool, bool>
		{
			public void Add( TypeInfo from, TypeInfo to, bool expectedResult )
			{
				// By default, assume explicit conversions within the same hierarchy are always possible.
				Add( from, to, expectedResult, true );
			}

			public void Test()
			{
				foreach ( var test in this )
				{
					// Implicit test.
					Assert.Equal( test.Item3, test.Item1.CanConvertTo( test.Item2 ) );

					// Test whether explicit casts to to type in same hierarchy is possible.
					Assert.Equal( test.Item4, test.Item1.CanConvertTo( test.Item2, CastType.SameHierarchy ) );
				}
			}
		}

		#endregion // Common test members


		[Fact]
		public void CanConvertToTest()
		{
			new CanConvert
			{
				// Non generic.
				{ _int, _int, true },		// No change.
				{ _string, _string, true },
				{ _object, _object, true },
				{ _simple, _simple, true },
				{ _int, _object, true },	// object <-> value
				{ _object, _int, false },
				{ _string, _object, true },	// string <-> object
				{ _object, _string, false },
				{ _simple, _object, true },	// object <-> object
				{ _object, _simple, false },
				{ _int, _short, false },	// value <-> value (by .NET rules, not C#)
				{ _short, _int, false },

				// Interface.
				{ _comparable, _comparable, true },	// No change.
				{ _int, _comparable, true },		// value <-> interface
				{ _comparable, _int, false },
				{ _comparable, _object, true },		// object <-> interface
				{ _object, _comparable, false }
			}.Test();

			// Interface variant type parameters.
			Func<TypeInfo, TypeInfo, TypeInfo> makeGeneric = ( g, t ) => g.MakeGenericType( t.AsType() ).GetTypeInfo();
			VarianceCheck( typeof( ICovariantOne<> ), makeGeneric, true );
			VarianceCheck( typeof( IContravariantOne<> ), makeGeneric, false );

			// Delegate variant type parameter.
			VarianceCheck( typeof( Func<> ), makeGeneric, true );
			VarianceCheck( typeof( Action<> ), makeGeneric, false );

			// Multiple variant type parameters.
			TypeInfo simpleObject = typeof( Func<Simple, object> ).GetTypeInfo();
			TypeInfo objectSimple = typeof( Func<object, Simple> ).GetTypeInfo();
			TypeInfo simpleSimple = typeof( Func<Simple, Simple> ).GetTypeInfo();
			TypeInfo objectObject = typeof( Func<object, object> ).GetTypeInfo();
			Assert.True( simpleObject.CanConvertTo( simpleObject ) );
			Assert.False( simpleObject.CanConvertTo( objectSimple ) );
			Assert.True( objectSimple.CanConvertTo( simpleObject ) );
			Assert.False( simpleObject.CanConvertTo( simpleSimple ) );
			Assert.True( objectSimple.CanConvertTo( objectObject ) );

			// TODO: Multiple inheritance for interfaces.

			// Recursive variant type parameters.
			Func<TypeInfo, TypeInfo, TypeInfo> makeInnerGeneric = ( g, t ) 
				=> g.GetGenericTypeDefinition( g.GetGenericArguments()[ 0 ].GetGenericTypeDefinition().MakeGenericType( t.AsType() ) );
			VarianceCheck( typeof( Func<Func<object>> ), makeInnerGeneric, true );
			VarianceCheck( typeof( Action<Action<object>> ), makeInnerGeneric, false );
			VarianceCheck( typeof( ICovariantOne<ICovariantOne<object>> ), makeInnerGeneric, true );
			VarianceCheck( typeof( IContravariantOne<IContravariantOne<object>> ), makeInnerGeneric, false );

			// Mixed recursive covariant/contravariant type parameters.
			VarianceCheck( typeof( Func<Action<object>> ), makeInnerGeneric, false );
			VarianceCheck( typeof( Action<Func<object>> ), makeInnerGeneric, true );
		}

		/// <summary>
		/// Checks the variance rules for generic types.
		/// For interfaces, only considers single implementing interface.
		/// </summary>
		/// <param name = "genericType">The generic type to check.</param>
		/// <param name = "makeGeneric">Function which can convert generic type into a specific type.</param>
		/// <param name = "covariant">true when to check for covariance, false for contravariance.</param>
		void VarianceCheck( Type genericType, Func<TypeInfo, TypeInfo, TypeInfo> makeGeneric, bool covariant )
		{
			TypeInfo info = genericType.GetTypeInfo();
			TypeInfo genericSimple = makeGeneric( info, _simple );
			TypeInfo genericObject = makeGeneric( info, _object );
			TypeInfo genericValue = makeGeneric( info, _int );

			bool isDelegate = info.IsDelegate();

			new CanConvert
			{
				// No change.
				{ genericObject, genericObject, true },		
		
				// generic type <-> object
				{ genericObject, _object, true },
				{ _object, genericObject, false },

				// No variance for value type parameters.
				// Converting from a generic type with a value parameter to one with a reference type parameters is only possible
				// when it is an interface type, and a certain type implements both interfaces. (e.g. ICovariance<int> -> ICovariance<object>)
				{ genericValue, genericObject, false, false },
				{ genericObject, genericValue, false, false },

				// Covariance/contraviariance between reference type parameters.
				// Only generic interface types can explicitly convert in the 'opposite' direction of their variance. Delegates can't!				
				{ genericSimple, genericObject,
					covariant,
					covariant ? true : !isDelegate },
				{ genericObject, genericSimple,
					!covariant,
					!covariant ? true : !isDelegate }
			}.Test();
		}

		[Fact]
		public void GetMatchingGenericTypeTest()
		{
			TypeInfo baseType = typeof( One<int> ).GetTypeInfo();
			TypeInfo interfaceType = typeof( IOne<int> ).GetTypeInfo();
			TypeInfo incompleteInterfaceType = typeof( IOne<> ).GetTypeInfo();
			TypeInfo incompleteBaseType = typeof( One<> ).GetTypeInfo();
			TypeInfo extendingType = typeof( ExtendingOne<int> ).GetTypeInfo();
			
			// Base types.
			Assert.Equal( baseType, baseType.GetMatchingGenericType( baseType ) );
			Assert.Equal( baseType, extendingType.GetMatchingGenericType( baseType ) );
			Assert.Equal( baseType, extendingType.GetMatchingGenericType( incompleteBaseType ) );

			// Interfaces.
			Assert.Equal( interfaceType, interfaceType.GetMatchingGenericType( interfaceType ) );
			Assert.Equal( interfaceType, baseType.GetMatchingGenericType( interfaceType ) );
			Assert.Equal( interfaceType, baseType.GetMatchingGenericType( incompleteInterfaceType ) );
		}

		[Fact]
		public void IsDelegateTest()
		{
			Assert.True( typeof( Func<int> ).GetTypeInfo().IsDelegate() );
			Assert.True( typeof( Action ).GetTypeInfo().IsDelegate() );
			Assert.True( typeof( TestDelegate ).GetTypeInfo().IsDelegate() );

			Assert.False( typeof( TypeInfoExtensionsTest ).GetTypeInfo().IsDelegate() );
		}
	}
}