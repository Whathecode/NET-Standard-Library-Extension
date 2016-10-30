using System;
using System.Collections.Generic;
using System.Linq;
using Whathecode.System.Linq;
using Xunit;


namespace Whathecode.Tests.System.Linq
{
	public partial class Extensions
	{
		// ReSharper disable InconsistentNaming    
		public class IEnumerableTest
		// ReSharper restore InconsistentNaming
		{
			[Fact]
			public void CombinationsTest()
			{
				Func<IEnumerable<IEnumerable<int>>, IEnumerable<string>> toStrings =
					input => input.Select( c => c.OrderBy( k => k ).Aggregate( "", ( s, n ) => s + n ) ).ToList();
				List<int> shortList = new List<int> { 0, 1, 2 };

				// No repetition allowed.
				var combinations = toStrings( shortList.Combinations( 2 ) );
				Assert.True( combinations.ContainsOnly( "01", "02", "12" ) );

				// Repetition allowed.
				var repeatedCombinations = toStrings( shortList.Combinations( 2, true ) );
				Assert.True( repeatedCombinations.ContainsOnly( "00", "11", "22", "01", "02", "12" ) );
			}

			[Fact]
			public void CountOfTest()
			{
				List<int> list = new List<int> { 0, 1, 2, 3, 4 };
				Assert.False( list.CountOf( 0 ) );
				Assert.False( list.CountOf( 2 ) );
				Assert.True( list.CountOf( 5 ) );
				Assert.False( list.CountOf( 10 ) );
			}

			[Fact]
			public void ContainsAllTest()
			{
				List<int> list = new List<int> { 0, 1, 2, 3, 4 };
				Assert.True( list.ContainsAll( new[] { 0, 2, 4 } ) );
				Assert.True( list.ContainsAll( 0, 2, 4 ) );
				Assert.False( list.ContainsAll( new[] { 0, 1, 2, 5 } ) );
			}

			[Fact]
			public void ContainsOnlyTest()
			{
				List<int> list = new List<int> { 0, 1, 2, 3, 4 };
				Assert.True( list.ContainsOnly( new[] { 0, 1, 2, 3, 4 } ) );
				Assert.True( list.ContainsOnly( 0, 1, 2, 3, 4 ) );
				Assert.False( list.ContainsOnly( new[] { 0, 1, 2 } ) );
				Assert.False( list.ContainsOnly( new[] { 5, 6, 7 } ) );
			}
		}
	}
}