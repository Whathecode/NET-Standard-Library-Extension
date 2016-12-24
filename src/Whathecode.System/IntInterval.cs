namespace Whathecode.System
{
	public class IntInterval : AbstractInterval<int>
	{
		protected override bool IsIntegralType => true;

		public IntInterval( int start, int end )
			: this( start, true, end, true ) { }

		public IntInterval( int start, bool isStartIncluded, int end, bool isEndIncluded )
			: base( start, isStartIncluded, end, isEndIncluded ) { }

		protected override IInterval<int, int> CreateInstance( int start, bool isStartIncluded, int end, bool isEndIncluded )
		{
			return new IntInterval( start, isStartIncluded, end, isEndIncluded );
		}

		protected override IInterval<int> ReduceGenerics( IInterval<int, int> interval )
		{
			return new IntInterval( interval.Start, interval.IsStartIncluded, interval.End, interval.IsEndIncluded );
		}


		protected override int Convert( double value )
		{
			return (int)value;
		}

		protected override double Convert( int value )
		{
			return value;
		}

		protected override int Subtract( int value1, int value2 )
		{
			return value1 - value2;
		}

		protected override int AddSize( int value, int size )
		{
			return value + size;
		}
	}
}
