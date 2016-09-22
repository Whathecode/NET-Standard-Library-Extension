using Whathecode.System.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Extensions
{
	/// <summary>
	/// 	TODO: Now that xUnit is used, perhaps a cleaner way of testing these different types can be set up by implementing a custom Theory data provider.
	/// </summary>
	public class ObjectExtensionsTest
	{
		class DummyObject
		{
			public int Value { get; set; }
		}

		class MainClass
		{
			public InnerClass Inner { get; set; }
		}

		class InnerClass
		{
			public DummyObject Dummy { get; set; }
		}


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
			object dummy1 = new DummyObject();
			object dummy2 = new DummyObject();
			Assert.Equal( true, dummy1.ReferenceOrBoxedValueEquals( dummy1 ) );
			Assert.Equal( false, dummy1.ReferenceOrBoxedValueEquals( dummy2 ) );
		}

		[Fact]
		public void IfNotNullTest()
		{
			// Nothing null.
			DummyObject dummy = new DummyObject();
			MainClass notNull = new MainClass
			{
				Inner = new InnerClass
				{
					Dummy = dummy
				}
			};
			DummyObject inner = notNull.IfNotNull( x => x.Inner ).IfNotNull( x => x.Dummy );
			Assert.Equal( dummy, inner );

			// Halfway null.
			MainClass halfwayNull = new MainClass
			{
				Inner = new InnerClass()
			};
			inner = halfwayNull.IfNotNull( x => x.Inner ).IfNotNull( x => x.Dummy );
			Assert.Null( inner );

			// All null.
			MainClass isNull = null;
			inner = isNull.IfNotNull( x => x.Inner ).IfNotNull( x => x.Dummy );
			Assert.Null( inner );

			// Value type.
			const int setValue = 10;
			MainClass value = new MainClass
			{
				Inner = new InnerClass
				{
					Dummy = new DummyObject
					{
						Value = setValue
					}
				}
			};
			int innerValue = value.IfNotNull( x => x.Inner ).IfNotNull( x => x.Dummy ).IfNotNull( x => x.Value );
			Assert.Equal( setValue, innerValue );
		}
	}
}