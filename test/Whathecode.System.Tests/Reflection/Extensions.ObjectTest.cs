using System;
using Whathecode.System.Reflection;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public partial class Extensions
	{
		public class ObjectTest
		{
			#region Common test members

			class MainClass
			{
				public SubClass Sub { get; private set; }

				public MainClass( string value )
				{
					Sub = new SubClass( value );
				}
			}


			class SubClass
			{
				public SubSubClass SubSub { get; private set; }

				public SubClass( string value )
				{
					SubSub = new SubSubClass( value );
				}
			}


			class SubSubClass
			{
				public string Value;

				public SubSubClass( string value )
				{
					Value = value;
				}
			}

			class InheritedClass : MainClass
			{
				public InheritedClass( string value )
					: base( value )
				{
				}
			}

			#endregion // Common test members


			[Fact]
			public void ReferenceOrBoxedValueEqualsTest()
			{
				// ints
				object one = 1;
				object alsoOne = 1;
				object notOne = 2;
				Assert.Equal( true, one.ReferenceOrBoxedValueEquals( alsoOne ) );
				Assert.Equal( false, one.ReferenceOrBoxedValueEquals( notOne ) );

				// strings
				object bleh = "bleh";
				object equalBleh = "bleh";
				object notBleh = "notbleh";
				Assert.Equal( true, bleh.ReferenceOrBoxedValueEquals( equalBleh ) );
				Assert.Equal( false, bleh.ReferenceOrBoxedValueEquals( notBleh ) );

				// Reference types.
				object dummy1 = new object();
				object dummy2 = new object();
				Assert.Equal( true, dummy1.ReferenceOrBoxedValueEquals( dummy1 ) );
				Assert.Equal( false, dummy1.ReferenceOrBoxedValueEquals( dummy2 ) );
			}

			[Fact]
			public void GetValueTest()
			{
				// Get value by path.
				const string testString = "test";
				MainClass test = new MainClass( testString );
				Assert.Equal( testString, test.GetValue( "Sub.SubSub.Value" ) );

				// Get value of non-existing path.
				Assert.Throws<ArgumentException>( () => test.GetValue( "NonExisting" ) );

				// Values from base types in inherited types.
				InheritedClass inherited = new InheritedClass( testString );
				Assert.Equal( testString, inherited.GetValue( "Sub.SubSub.Value" ) );
			}
		}
	}
}