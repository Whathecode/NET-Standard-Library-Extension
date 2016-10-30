using System;
using System.Reflection;
using Whathecode.System.Reflection;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public partial class Extensions
	{
		public class TypeInfoTest
		{
			interface IOne<T> { }
			class One<T> : IOne<T> { }
			class ExtendingOne<T> : One<T> { }
			interface ITwo<T, T2> { }
			class Two<T, T2> : ITwo<T, T2> { }
			interface IExtendingInterface<T> : IOne<T> { }
			class ImplementExtending<T> : IExtendingInterface<T> { }

			[Theory]
			[InlineData( typeof( One<int> ), typeof( One<> ), new[] { typeof( int ) } )]
			[InlineData( typeof( One<int> ), typeof( One<string> ), new[] { typeof( int ) } )]
			[InlineData( typeof( IOne<int> ), typeof( IOne<> ), new[] { typeof( int ) } )]
			[InlineData( typeof( IOne<int> ), typeof( IOne<string> ), new[] { typeof( int ) } )]
			[InlineData( typeof( Two<int, int> ), typeof( Two<string, object> ), new[] { typeof( int ), typeof( int ) } )]
			public void GetGenericTypeDefinitionTest( Type expected, Type source, params Type[] arguments )
			{
				Assert.Equal( expected.GetTypeInfo(), source.GetTypeInfo().GetGenericTypeDefinition( arguments ) );
			}

			[Fact]
			public void GetGenericTypeDefinitionTestWrongArguments()
			{
				// Source can't be null.
				TypeInfo nullInfo = null;
				Assert.Throws<ArgumentNullException>( () => nullInfo.GetGenericTypeDefinition( new[] { typeof( int ) } ) );

				// Source has to be generic.
				Assert.Throws<ArgumentException>( () => typeof( object ).GetTypeInfo().GetGenericTypeDefinition( new Type[] { } ) );

				// Type arguments can't be null.
				Assert.Throws<ArgumentNullException>( () => typeof( One<int> ).GetTypeInfo().GetGenericTypeDefinition( null ) );

				// Insufficient type parameters.
				Assert.Throws<ArgumentException>( () => typeof( Two<int, int> ).GetTypeInfo().GetGenericTypeDefinition( new[] { typeof( int ) } ) );
			}

			[Theory]
			[InlineData( typeof( One<int> ), typeof( One<int> ), typeof( One<int> ) )]
			[InlineData( typeof( One<int> ), typeof( ExtendingOne<int> ), typeof( One<int> ) )]
			[InlineData( typeof( One<int> ), typeof( One<int> ), typeof( One<> ) )]
			[InlineData( typeof( IOne<int> ), typeof( IOne<int> ), typeof( IOne<int> ) )]
			[InlineData( typeof( IOne<int> ), typeof( One<int> ), typeof( IOne<int> ) )]
			[InlineData( typeof( IOne<int> ), typeof( One<int> ), typeof( IOne<> ) )]
			[InlineData( typeof( Two<int, string> ), typeof( Two<int, string> ), typeof( Two<int, string> ) )]
			[InlineData( typeof( ITwo<int, string> ), typeof( Two<int, string> ), typeof( ITwo<int, string> ) )]
			[InlineData( typeof( Two<int, string> ), typeof( Two<int, string> ), typeof( Two<,> ) )]
			[InlineData( typeof( IExtendingInterface<int> ), typeof( ImplementExtending<int> ), typeof( IExtendingInterface<int> ) )]
			[InlineData( typeof( IOne<int> ), typeof( ImplementExtending<int> ), typeof( IOne<> ) )]
			[InlineData( typeof( Action<int> ), typeof( Action<int> ), typeof( Action<> ) )]
			[InlineData( null, typeof( One<int> ), typeof( Two<int, int> ) )]
			[InlineData( null, typeof( One<int> ), typeof( ExtendingOne<int> ) )]
			[InlineData( null, typeof( One<int> ), typeof( One<double> ) )]
			[InlineData( null, typeof( One<int> ), typeof( IOne<double> ) )]
			[InlineData( typeof( object ), typeof( object ), typeof( object ) )]
			[InlineData( typeof( object ), typeof( One<int> ), typeof( object ) )]
			[InlineData( typeof( int ), typeof( int ), typeof( int ) )]
			public void GetMatchingGenericTypeTest( Type expected, Type sourceType, Type findMatchingType )
			{
				TypeInfo sourceInfo = sourceType.GetTypeInfo();
				TypeInfo findInfo = findMatchingType.GetTypeInfo();
				TypeInfo expectedInfo = expected?.GetTypeInfo();
				Assert.Equal( expectedInfo, sourceInfo.GetMatchingGenericType( findInfo ) );
			}

			[Fact]
			public void GetMatchingGenericTypeTestNulls()
			{
				// Source can't be null.
				TypeInfo nullInfo = null;
				Assert.Throws<ArgumentNullException>( () => nullInfo.GetMatchingGenericType( typeof( One<int> ).GetTypeInfo() ) );

				// Type to look for can't be null.
				TypeInfo one = typeof( One<int> ).GetTypeInfo();
				Assert.Throws<ArgumentNullException>( () => one.GetMatchingGenericType( null ) );
			}

			[Theory] // No need for extensive testing. This relies on 'GetMatchingGenericTypeTest' which is tested thoroughly above.
			[InlineData( true, typeof( One<int> ), typeof( One<> ) )]
			[InlineData( false, typeof( One<int> ), typeof( Two<,> ) )]
			[InlineData( true, typeof( One<int> ), typeof( object ) )]
			public void IsOfGenericTypeTest( bool expected, Type sourceType, Type checkType )
			{
				TypeInfo sourceInfo = sourceType.GetTypeInfo();
				TypeInfo checkInfo = checkType.GetTypeInfo();
				Assert.Equal( expected, sourceInfo.IsOfGenericType( checkInfo ) );
			}

			[Fact]
			public void IsOfGenericTypeTestNulls()
			{
				// Source can't be null.
				TypeInfo nullInfo = null;
				Assert.Throws<ArgumentNullException>( () => nullInfo.IsOfGenericType( typeof( One<int> ).GetTypeInfo() ) );

				// Type to look for can't be null.
				TypeInfo one = typeof( One<int> ).GetTypeInfo();
				Assert.Throws<ArgumentNullException>( () => one.IsOfGenericType( null ) );
			}


			enum Enum { }
			[Flags]
			enum FlagsEnum { }

			[Fact]
			public void IsFlagsEnumTest()
			{
				Assert.True( typeof( FlagsEnum ).GetTypeInfo().IsFlagsEnum() );
				Assert.False( typeof( Enum ).GetTypeInfo().IsFlagsEnum() );
				Assert.False( typeof( object ).GetTypeInfo().IsFlagsEnum() );
			}


			delegate bool TestDelegate( int test );

			[Fact]
			public void IsDelegateTest()
			{
				Assert.True( typeof( Func<int> ).GetTypeInfo().IsDelegate() );
				Assert.True( typeof( Action ).GetTypeInfo().IsDelegate() );
				Assert.True( typeof( TestDelegate ).GetTypeInfo().IsDelegate() );

				Assert.False( typeof( TypeInfoTest ).GetTypeInfo().IsDelegate() );
			}


			static class Static { }
			class NonStatic { }
			interface IInterface { }
			delegate void Delegate();
			enum EnumType { }
			abstract class Abstract { }

			[Theory]
			[InlineData( true, typeof( Static ) )]
			[InlineData( false, typeof( NonStatic ) )]
			[InlineData( false, typeof( IInterface ) )]
			[InlineData( false, typeof( Delegate ) )]
			[InlineData( false, typeof( EnumType ) )]
			[InlineData( false, typeof( Abstract ) )]
			[InlineData( false, typeof( int ) )]
			public void IsStaticTest( bool expected, Type type )
			{
				Assert.Equal( expected, type.GetTypeInfo().IsStatic() );
			}

			[Theory]
			[InlineData( true, typeof( One<int> ), typeof( IOne<int> ) )]
			[InlineData( true, typeof( ExtendingOne<int> ), typeof( IOne<int> ) )]
			[InlineData( false, typeof( One<int> ), typeof( ITwo<int, int> ) )]
			[InlineData( true, typeof( IExtendingInterface<int> ), typeof( IOne<int> ) )]
			[InlineData( true, typeof( ImplementExtending<int> ), typeof( IOne<int> ) )]
			public void ImplementsInterfaceTest( bool expected, Type type, Type interfaceType )
			{
				Assert.Equal( expected, type.GetTypeInfo().ImplementsInterface( interfaceType.GetTypeInfo() ) );
			}

			[Fact]
			public void ImplementsInterfaceTestWrongArguments()
			{
				// Source can't be null.
				TypeInfo nullInfo = null;
				Assert.Throws<ArgumentNullException>( () => nullInfo.ImplementsInterface( typeof( IOne<int> ).GetTypeInfo() ) );

				// Type to look for can't be null.
				TypeInfo one = typeof( One<int> ).GetTypeInfo();
				Assert.Throws<ArgumentNullException>( () => one.ImplementsInterface( null ) );
			}

			[Theory]
			[InlineData( 0, typeof( int ) )]
			[InlineData( 0.0, typeof( double ) )]
			[InlineData( null, typeof( string ) )]
			[InlineData( null, typeof( object ) )]
			[InlineData( null, typeof( IOne<int> ) )]
			[InlineData( new Enum(), typeof( Enum ) )]
			public void CreateDefaultTest( object expected, Type type )
			{
				Assert.Equal( expected, type.GetTypeInfo().CreateDefault() );
			}
		}
	}
}