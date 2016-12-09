using System;
using System.Collections.Generic;


namespace Whathecode.System
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns the interval of the indices.
		/// </summary>
		/// <typeparam name = "T">Type of the values in the list.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <returns>The interval of the indices of the list.</returns>
		public static IInterval<int> GetIndexInterval<T>( this IList<T> source )
		{
			return new IntInterval( 0, source.Count - 1 );
		}

		/// <summary>
		/// Swaps two items in a list.
		/// </summary>
		/// <typeparam name = "T">Type of the values in the list.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "item1">The item to swap with <paramref name="item2" />.</param>
		/// <param name = "item2">The item to swap with <paramref name="item1" />.</param>
		public static void Swap<T>( this IList<T> source, T item1, T item2 )
		{
			source.Swap( source.IndexOf( item1 ), source.IndexOf( item2 ) );
		}

		/// <summary>
		/// Swaps two items in a list, specified by their indices.
		/// </summary>
		/// <typeparam name = "T">Type of the values in the list.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "index1">The index of the item to swap with the item at <paramref name="index2" />.</param>
		/// <param name = "index2">The index of the item to swap with the item at <paramref name="index1" />.</param>
		public static void Swap<T>( this IList<T> source, int index1, int index2 )
		{
			if ( source == null )
			{
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( index1 < 0 || index1 >= source.Count )
			{
				throw new ArgumentException( "Index out of range.", nameof( index1 ) );
			}
			if ( index2 < 0 || index2 >= source.Count )
			{
				throw new ArgumentException( "Index out of range.", nameof( index1 ) );
			}

			if ( index1 == index2 )
			{
				return;
			}
			
			T temp = source[ index1 ];
			source[ index1 ] = source[ index2 ];
			source[ index2 ] = temp;
		}
	}
}