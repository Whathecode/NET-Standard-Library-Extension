using Whathecode.System.Algorithm;
using Xunit;


namespace Whathecode.Tests.System.Algorithm
{
    public class SkipGateTest
    {
		[Fact]
		public void OpenOnThird()
		{
			SkipGate openOnThird = new SkipGate( 2 );
			for ( int i = 1; i < 5; ++i )
			{
				if ( openOnThird.TryEnter() )
				{
					Assert.True( i >= 3 );
				}
			}
		}

		[Fact]
		public void OpenEveryFive()
		{
			SkipGate openOnPluralFive = new SkipGate( 4, true );
			int timesOpened = 0;
			for ( int i = 1; i <= 20; ++i )
			{
				if ( openOnPluralFive.TryEnter() )
				{
					Assert.True( i % 5 == 0 );
					++timesOpened;
				}
			}

			Assert.Equal( 4, timesOpened );
		}
    }
}
