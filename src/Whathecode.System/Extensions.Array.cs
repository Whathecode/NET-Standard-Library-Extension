namespace Whathecode.System
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns the interval of the indices.
		/// </summary>
		/// <typeparam name = "T">Type of the values in the array.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <returns>The interval of the indices of the array.</returns>
		public static IInterval<int> GetIndexInterval<T>( this T[] source )
		{
			return new IntInterval( 0, source.Length - 1 );
		}
	}
}
