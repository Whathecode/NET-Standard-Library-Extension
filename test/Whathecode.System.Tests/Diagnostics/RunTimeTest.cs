using System;
using Whathecode.System.Diagnostics;
using Xunit;


namespace Whathecode.Tests.System.Diagnostics
{
    public class RunTimeTest
    {
		[Fact]
		public void FromTest()
		{
			Action test = () => { };

			// Single run.
			StatisticsStopwatch.Measurement oneRun = RunTime.From( test );
			Assert.True( oneRun.MeasurementCount == 1 );

			// Multiple runs.
			const int runs = 10;
			StatisticsStopwatch.Measurement multipleRuns = RunTime.From( "Multiple runs", test, runs );
			Assert.True( multipleRuns.MeasurementCount == runs );
		}
    }
}
