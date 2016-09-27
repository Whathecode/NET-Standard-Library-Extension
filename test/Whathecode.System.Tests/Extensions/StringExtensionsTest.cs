using Whathecode.System.Algorithm;
using Whathecode.System.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Extensions
{
	public class StringExtensionsTest
	{
		[Fact]
		public void SplitAtTest()
		{
			// Exclude split.
			const string toSplit = "Split.Here";
			int splitPosition = toSplit.IndexOf( '.' );
			string[] split = toSplit.SplitAt( splitPosition );
			Assert.Equal( "Split", split[ 0 ] );
			Assert.Equal( "Here", split[ 1 ] );

			// Include both.
			split = toSplit.SplitAt( splitPosition, SplitOption.Both );
			Assert.Equal( "Split.", split[ 0 ] );
			Assert.Equal( ".Here", split[ 1 ] );

			// Include left.
			split = toSplit.SplitAt( splitPosition, SplitOption.Left );
			Assert.Equal( "Split.", split[ 0 ] );
			Assert.Equal( "Here", split[ 1 ] );

			// Include right.
			split = toSplit.SplitAt( splitPosition, SplitOption.Right );
			Assert.Equal( "Split", split[ 0 ] );
			Assert.Equal( ".Here", split[ 1 ] );
		}
	}
}