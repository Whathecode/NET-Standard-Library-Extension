using Whathecode.System;
using Xunit;


namespace Whathecode.Tests.System
{
	public partial class Extensions
	{
		/// <summary>
		/// 	TODO: Now that xUnit is used, perhaps a cleaner way of testing these different types can be set up by implementing a custom Theory data provider.
		/// </summary>
		public class ObjectTest
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
}