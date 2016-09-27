using Whathecode.System.Arithmetic.Interpolation;
using Whathecode.System.Arithmetic.Interpolation.KeyPoint;
using Whathecode.System.Arithmetic.Interpolation.TypeProvider;
using Xunit;


namespace Whathecode.Tests.System.Arithmetic.Interpolation
{
    public class LinearInterpolationTest
    {
		[Fact]
		public void InterpolateTest()
		{
			var keyPoints = new AbsoluteKeyPointCollection<int, int>( new ValueTypeInterpolationProvider<int>(), 0 );
			keyPoints.Add( 0 );
			keyPoints.Add( 10 );
			var interpolate = new LinearInterpolation<int, int>( keyPoints );

			Assert.Equal( 5, interpolate.Interpolate( 0.5 ) );
		}
    }
}
