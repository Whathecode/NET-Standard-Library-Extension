using System;
using Whathecode.System.Collections.Generic;


namespace Whathecode.System
{
	public partial class AbstractInterval<T, TSize>
	{
		/// <summary>
		/// Enumerator which allows you to walk across values inside an interval.
		/// TODO: Having to pass the methods to operate on the generic type here is quite messy. Is there a better way around this?
		/// </summary>
		/// <typeparam name = "T">The type used to specify the interval, and used for the calculations.</typeparam>
		/// <typeparam name = "TSize">The type used to specify distances in between two values of <see cref="T" />.</typeparam>
		private class Enumerator : AbstractEnumerator<T>
		{
			readonly IInterval<T, TSize> _interval;
			readonly TSize _step;
			readonly Func<T, T, TSize> _subtract;
			readonly Func<T, TSize, T> _addSize;
			readonly Func<TSize, double> _convertSizeToDouble;
			readonly Func<double, TSize> _convertDoubleToSize;
			readonly Func<T, bool, T, bool, IInterval<T, TSize>> _createInstance;

			readonly bool _isAnchorSet;
			readonly T _anchor;


			/// <summary>
			/// Create a new enumerator which traverses across an interval in specified steps.
			/// </summary>
			/// <param name = "interval">The interval which to traverse.</param>
			/// <param name = "step">The steps to step forward each time.</param>
			public Enumerator(
				IInterval<T, TSize> interval, TSize step,
				Func<T, T, TSize> subtract,
				Func<T, TSize, T> addSize,
				Func<TSize, double> convertSizeToDouble,
				Func<double, TSize> convertDoubeToSize,
				Func<T, bool, T, bool, IInterval<T, TSize>> createInstance )
			{
				_interval = interval;
				_step = step;
				_subtract = subtract;
				_addSize = addSize;
				_convertSizeToDouble = convertSizeToDouble;
				_convertDoubleToSize = convertDoubeToSize;
				_createInstance = createInstance;
			}

			public Enumerator(
				IInterval<T, TSize> interval, TSize step,
				Func<T, T, TSize> subtract,
				Func<T, TSize, T> addSize,
				Func<TSize, double> convertSizeToDouble,
				Func<double, TSize> convertDoubeToSize,
				Func<T, bool, T, bool, IInterval<T, TSize>> createInstance,
				T anchorAt )
				: this( interval, step, subtract, addSize, convertSizeToDouble, convertDoubeToSize, createInstance )
			{
				_isAnchorSet = true;
				_anchor = anchorAt;
			}


			protected override T GetFirst()
			{
				IInterval<T, TSize> interval = _interval;

				// When anchor is set, start the interval at the next anchor position.
				if ( _isAnchorSet )
				{
					TSize anchorDiff = _subtract( _interval.Start, _anchor );
					double stepSize = _convertSizeToDouble( _step );
					double diff = Math.Abs( _convertSizeToDouble( anchorDiff ) ) % stepSize;
					if ( diff > 0 )
					{
						if ( _anchor.CompareTo( _interval.Start ) < 0 )
						{
							diff = stepSize - diff;
						}
						TSize addition = _convertDoubleToSize( diff );
						interval = _createInstance(
							_addSize( _interval.Start, addition ), true,
							_interval.End, _interval.IsEndIncluded );
					}
				}

				// When first value doesn't lie in interval, immediately step.
				return interval.IsStartIncluded ? interval.Start : _addSize( interval.Start, _step );
			}

			protected override T GetNext( int enumeratedAlready, T previous )
			{
				return _addSize( previous, _step );
			}

			protected override bool HasElements()
			{
				bool nextInInterval = _interval.LiesInInterval( _addSize( _interval.Start, _step ) );
				return _interval.IsStartIncluded || nextInInterval;
			}

			protected override bool HasMoreElements( int enumeratedAlready, T previous )
			{
				if ( _convertSizeToDouble( _step ) == 0 && enumeratedAlready == 1 )
				{
					return false;
				}

				return _interval.LiesInInterval( _addSize( previous, _step ) );
			}

			public override void Dispose()
			{
				// TODO: Nothing to do?
			}
		}
	}
}