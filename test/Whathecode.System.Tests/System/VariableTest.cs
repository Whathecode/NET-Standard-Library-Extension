using Whathecode.System;
using Xunit;


namespace Whathecode.Tests.System
{
	public class VariableTest
	{
		[Fact]
		public void SwapTest()
		{
			// Value type.
			int a = 0;
			int b = 100;
			Variable.Swap( ref a, ref b );
			Assert.Equal( 100, a );
			Assert.Equal( 0, b );

			// Reference type.
			object object1 = new object();
			object object2 = new object();
			object oldObject1 = object1;
			object oldObject2 = object2;
			Variable.Swap( ref object1, ref object2 );
			Assert.Equal( oldObject2, object1 );
			Assert.Equal( oldObject1, object2 );
		}
	}
}
