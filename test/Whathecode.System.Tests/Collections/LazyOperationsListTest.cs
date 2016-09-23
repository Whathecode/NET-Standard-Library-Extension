using System;
using System.Collections.Generic;
using Whathecode.System.Arithmetic.Range;
using Whathecode.System.Collections;
using Xunit;


namespace Whathecode.Tests.System.Collections
{
	/// <summary>
	/// Unit tests for <see cref = "LazyOperationsList{TObject}" />.
	/// </summary>
	public class LazyOperationsListTest
	{
		#region Common Test Members

		// List and starting data.
		LazyOperationsList<int> _list = new LazyOperationsList<int>();
		List<int> _originalData = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		// Ranges on which operations will be applied.
		List<Interval<int>> _ranges = new List<Interval<int>>
		{
			new Interval<int>( 0, 1 ),
			new Interval<int>( 3, 4 ),
			new Interval<int>( 1, 3 ), // Overlaps two.
			new Interval<int>( 7, 8 ),
			new Interval<int>( 8, 9 ), // Overlap right.
			new Interval<int>( 6, 7 ), // Overlap left.
			new Interval<int>( 5, 5 ), // Bordering.
			new Interval<int>( 1, 9 )  // Lots of intersections.
		};

		// Operation which is done for every range.
		Func<int, int> _operation = i => i + 1;


		public LazyOperationsListTest()
		{
			// Add data to list.
			foreach ( var d in _originalData )
			{
				_list.Add( d );
			}
		}

		void PerformAllOperations()
		{
			foreach ( var range in _ranges )
			{
				// Perform on original data.
				for ( int i = range.Start; i <= range.End; ++i )
				{
					_originalData[ i ] = _operation( _originalData[ i ] );
				}

				// Add to list.
				_list.AddOperation( _operation, range );
			}
		}

		void VerifyData()
		{
			for ( int i = 0; i < _originalData.Count; ++i )
			{
				Assert.Equal( _originalData[ i ], _list[ i ] );
			}
		}

		#endregion  // Common Test Members


		[Fact]
		public void FlushPendingOperationsTest()
		{
			PerformAllOperations();

			_list.FlushPendingOperations();

			VerifyData();
		}

		/// <summary>
		/// Test adding operations, and compare values with expected values after retrieving.
		/// </summary>
		[Fact]
		public void AddOperationTest()
		{
			PerformAllOperations();

			VerifyData();
			VerifyData(); // Check second time to verify correct multiple access.
		}

		[Fact]
		public void RemoveTest()
		{
			PerformAllOperations();

			Random rand = new Random();

			while ( _originalData.Count > 0 )
			{
				// Remove.
				int removeIndex = rand.Next( _originalData.Count );
				_originalData.RemoveAt( removeIndex );
				_list.RemoveAt( removeIndex );

				// Verify.
				for ( int i = 0; i < _originalData.Count; ++i )
				{
					Assert.Equal( _originalData[ i ], _list[ i ] );
				}
			}
		}
	}
}