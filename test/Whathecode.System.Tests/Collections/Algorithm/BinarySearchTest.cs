using Whathecode.System.Collections.Algorithm;
using Whathecode.System.Collections.Delegates;
using Whathecode.System.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Collections.Algorithm
{
	public class BinarySearchTest
	{
		[Fact]
		public void SearchTest()
		{
			var numbers = new [] { -10, -8, 0, 10, 500 };
			var indexerDelegates = new IndexerDelegates<int, int>( index => numbers[ index ], index => index );

			// Object found.
			BinarySearchResult<int> result = BinarySearch<int, int>.Search( 0, numbers.GetIndexInterval(), indexerDelegates );
			Assert.Equal( true, result.IsObjectFound );
			Assert.Equal( true, result.IsObjectInRange );
			Assert.Equal( 0, result.Found.Object );
			Assert.Null( result.NotFound );

			// Object found, border.
			result = BinarySearch<int, int>.Search( 500, numbers.GetIndexInterval(), indexerDelegates );
			Assert.Equal( true, result.IsObjectFound );
			Assert.Equal( true, result.IsObjectInRange );
			Assert.Equal( 500, result.Found.Object );
			Assert.Null( result.NotFound );

			// Object not found, but in range.
			result = BinarySearch<int, int>.Search( -9, numbers.GetIndexInterval(), indexerDelegates );
			Assert.Equal( false, result.IsObjectFound );
			Assert.Equal( true, result.IsObjectInRange );
			Assert.Equal( -10, result.NotFound.Smaller );
			Assert.Equal( -8, result.NotFound.Bigger );
			Assert.Null( result.Found );

			// Object not found, out of range, left.
			result = BinarySearch<int, int>.Search( -20, numbers.GetIndexInterval(), indexerDelegates );
			Assert.Equal( false, result.IsObjectFound );
			Assert.Equal( false, result.IsObjectInRange );
			Assert.Null( result.NotFound );
			Assert.Null( result.Found );

			// Object not found, out of range, right.
			result = BinarySearch<int, int>.Search( 600, numbers.GetIndexInterval(), indexerDelegates );
			Assert.Equal( false, result.IsObjectFound );
			Assert.Equal( false, result.IsObjectInRange );
			Assert.Null( result.NotFound );
			Assert.Null( result.Found );
		}
	}
}
