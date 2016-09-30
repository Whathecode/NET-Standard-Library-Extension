using System;
using Whathecode.System.Reflection.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public class ObjectExtensionsTest
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
			public string Value { get; private set; }

			public SubSubClass( string value )
			{
				Value = value;
			}
		}

		#endregion // Common test members


		[Fact]
		public void GetValueTest()
		{
			// Get value by path.
			const string testString = "test";
			MainClass test = new MainClass( testString );
			Assert.Equal( testString, test.GetValue( "Sub.SubSub.Value" ) );

			// Get value of non-existing path.
			Assert.Throws<ArgumentException>( () => test.GetValue( "NonExisting" ) );
		}
	}
}