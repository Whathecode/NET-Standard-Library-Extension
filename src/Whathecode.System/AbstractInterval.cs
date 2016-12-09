using System;
using System.Collections.Generic;
using System.Linq;
using Whathecode.System.Algorithm;


namespace Whathecode.System
{
	/// <summary>
	/// Abstract class specifying an interval from a value, to a value. Borders may be included or excluded. This type is immutable.
	/// TODO: The 'ToString' and 'Parse' methods were removed since they relied on TypeConverter, which is not present in the .NET Core Library. How to support this?
	/// </summary>
	/// <remarks>
	/// This is a wrapper class which simply redirect calls to a more generic base type.
	/// </remarks>
	/// <typeparam name = "T">The type used to specify the interval, and used for the calculations.</typeparam>
	public abstract class AbstractInterval<T> : AbstractInterval<T, T>, IInterval<T>
		where T : IComparable<T>
	{
		/// <summary>
		/// Create a new interval with a specified start and end.
		/// </summary>
		/// <param name = "start">The start of the interval.</param>
		/// <param name = "isStartIncluded">Is the value at the start of the interval included in the interval.</param>
		/// <param name = "end">The end of the interval.</param>
		/// <param name = "isEndIncluded">Is the value at the end of the interval included in the interval.</param>
		protected AbstractInterval( T start, bool isStartIncluded, T end, bool isEndIncluded )
			: base( start, isStartIncluded, end, isEndIncluded ) { }


		protected override T SubtractSize( T value, T size )
		{
			return Subtract( value, size );
		}

		protected override T SubtractSizes( T value1, T value2 )
		{
			return Subtract( value1, value2 );
		}

		/// <summary>
		/// Limit a given range to this range.
		/// When part of the given range lies outside of this range, it isn't included in the resulting range.
		/// </summary>
		/// <param name = "range">The range to limit to this range.</param>
		/// <returns>The given range, which excludes all parts lying outside of this range.</returns>
		public IInterval<T> Clamp( IInterval<T> range )
		{
			return ReduceGenerics( base.Clamp( range ) );
		}

		/// <summary>
		/// Split the interval into two intervals at the given point, or nearest valid point.
		/// </summary>
		/// <param name = "atPoint">The point where to split.</param>
		/// <param name = "option">Option which specifies in which intervals the split point ends up.</param>
		/// <param name = "before">The interval in which to store the part before the point, if any, null otherwise.</param>
		/// <param name = "after">The interval in which to store the part after the point, if any, null otherwise.</param>
		public void Split( T atPoint, SplitOption option, out IInterval<T> before, out IInterval<T> after )
		{
			IInterval<T, T> beforeInner;
			IInterval<T, T> afterInner;
			Split( atPoint, option, out beforeInner, out afterInner );
			before = ReduceGenerics( beforeInner );
			after = ReduceGenerics( afterInner );
		}

		/// <summary>
		/// Subtract a given interval from the current interval.
		/// </summary>
		/// <param name = "subtract">The interval to subtract from this interval.</param>
		/// <returns>The resulting intervals after subtraction.</returns>
		public List<IInterval<T>> Subtract( IInterval<T> subtract )
		{
			List<IInterval<T, T>> result = base.Subtract( subtract );
			return result.Select( r => ReduceGenerics( r ) ).Cast<IInterval<T>>().ToList();
		}

		/// <summary>
		/// Returns the intersection of this interval with another.
		/// </summary>
		/// <param name = "interval">The interval to get the intersection for.</param>
		/// <returns>The intersection of this interval with the given other. Null when no intersection.</returns>
		public IInterval<T> Intersection( IInterval<T> interval )
		{
			return ReduceGenerics( base.Intersection( interval ) );
		}

		/// <summary>
		/// Returns an expanded interval of the current interval up to the given value (and including).
		/// When the value lies within the interval the returned interval is the same.
		/// </summary>
		/// <param name = "value">The value up to which to expand the interval.</param>
		public new IInterval<T> ExpandTo( T value )
		{
			return ReduceGenerics( base.ExpandTo( value ) );
		}

		/// <summary>
		/// Returns an expanded interval of the current interval up to the given value.
		/// When the value lies within the interval the returned interval is the same.
		/// </summary>
		/// <param name = "value">The value up to which to expand the interval.</param>
		/// <param name = "include">Include the value to which is expanded in the interval.</param>
		public new IInterval<T> ExpandTo( T value, bool include )
		{
			return ReduceGenerics( base.ExpandTo( value, include ) );
		}

		/// <summary>
		/// Returns an interval offsetted from the current interval by a specified amount.
		/// </summary>
		/// <param name="amount">How much to move the interval.</param>
		public new IInterval<T> Move( T amount )
		{
			return ReduceGenerics( base.Move( amount ) );
		}

		/// <summary>
		/// Returns a scaled version of the current interval.
		/// </summary>
		/// <param name="scale">
		/// Percentage to scale the interval up or down.
		/// Smaller than 1.0 to scale down, larger to scale up.
		/// </param>
		/// <param name="aroundPercentage">The percentage inside the interval around which to scale.</param>
		public new IInterval<T> Scale( double scale, double aroundPercentage = 0.5 )
		{
			return ReduceGenerics( base.Scale( scale, aroundPercentage ) );
		}

		/// <summary>
		/// Returns a reversed version of the current interval, swapping the start position with the end position.
		/// </summary>
		public new IInterval<T> Reverse()
		{
			return ReduceGenerics( base.Reverse() );
		}

		public new object Clone()
		{
			return ReduceGenerics( CreateInstance( Start, IsStartIncluded, End, IsEndIncluded ) );
		}


		/// <summary>
		/// Create a less generic interval from a more generic base type.
		/// </summary>
		/// <param name = "interval">The more generic base type.</param>
		protected abstract IInterval<T> ReduceGenerics( IInterval<T, T> interval );
	}


	/// <summary>
	/// Abstract class specifying an interval from a value, to a value. Borders may be included or excluded. This type is immutable.
	/// </summary>
	/// <typeparam name = "T">The type used to specify the interval, and used for the calculations.</typeparam>
	/// <typeparam name = "TSize">The type used to specify distances in between two values of <typeparamref name="T"/>.</typeparam>
	public abstract partial class AbstractInterval<T, TSize> : IInterval<T, TSize>
		where T : IComparable<T>
		where TSize : IComparable<TSize>
	{
		readonly T _start;
		/// <summary>
		/// The start of the interval.
		/// </summary>
		public T Start { get { return IsReversed ? _end : _start; } }

		readonly T _end;
		/// <summary>
		/// The end of the interval.
		/// </summary>
		public T End { get { return IsReversed ? _start : _end; } }

		readonly bool _isStartIncluded;
		/// <summary>
		/// Is the value at the start of the interval included in the interval.
		/// </summary>
		public bool IsStartIncluded { get { return IsReversed ? _isEndIncluded : _isStartIncluded; } }

		readonly bool _isEndIncluded;
		/// <summary>
		/// Is the value at the end of the interval included in the interval.
		/// </summary>
		public bool IsEndIncluded { get { return IsReversed ? _isStartIncluded : _isEndIncluded; } }

		/// <summary>
		/// Determines whether the start of the interval lies before or after the end of the interval. true when after, false when before.
		/// </summary>
		public bool IsReversed { get; private set; }

		/// <summary>
		/// Get the value in the center of the interval. Rounded to the nearest correct value.
		/// </summary>
		public T Center { get { return GetValueAt( 0.5 ); } }

		/// <summary>
		/// Get the size of the interval.
		/// </summary>
		public TSize Size { get { return Subtract( _end, _start ); } }


		/// <summary>
		/// Create a new interval with a specified start and end.
		/// </summary>
		/// <param name = "start">The start of the interval.</param>
		/// <param name = "isStartIncluded">Is the value at the start of the interval included in the interval.</param>
		/// <param name = "end">The end of the interval.</param>
		/// <param name = "isEndIncluded">Is the value at the end of the interval included in the interval.</param>
		protected AbstractInterval( T start, bool isStartIncluded, T end, bool isEndIncluded )
		{
			if ( end.CompareTo( start ) == 0 && isStartIncluded != isEndIncluded )
			{
				throw new ArgumentException( "Invalid interval arguments. e.g., ]0, 0]" );
			}

			IsReversed = start.CompareTo( end ) > 0;

			// Internally always assume non-inversed intervals.
			_start = IsReversed ? end : start;
			_isStartIncluded = IsReversed ? isEndIncluded : isStartIncluded;
			_end = IsReversed ? start : end;
			_isEndIncluded = IsReversed ? isStartIncluded : isEndIncluded;
		}


		#region Get operations.

		/// <summary>
		/// Get the value at a given percentage within (0.0 - 1.0) or outside (&lt; 0.0, &gt; 1.0) of the interval. Rounding to nearest neighbour occurs when needed.
		/// TODO: Would it be cleaner not to use a double for percentage, but a generic Percentage type?
		/// </summary>
		/// <param name = "percentage">The percentage in the range of which to return the value.</param>
		/// <returns>The value at the given percentage within the interval.</returns>
		public T GetValueAt( double percentage )
		{
			// Use double math for the calculation, and then cast to the desired type.
			double value = percentage * Convert( Size );

			// Ensure nearest neighbour rounding for integral types.
			if ( IsIntegralType )
			{
				value = Math.Round( value );
			}

			return AddSize( _start, Convert( value ) );
		}

		/// <summary>
		/// Get a percentage how far inside (0.0 - 1.0) or outside (&lt; 0.0, &gt; 1.0) the interval a certain value lies.
		/// For single intervals, '1.0' is returned when inside the interval, '-1.0' otherwise.
		/// </summary>
		/// <param name = "position">The position value to get the percentage for.</param>
		/// <returns>The percentage indicating how far inside (or outside) the interval the given value lies.</returns>
		public double GetPercentageFor( T position )
		{
			double size = Convert( Size );

			// When size is zero, return 1.0 when in interval.
			if ( size == 0 )
			{
				return LiesInInterval( position ) ? 1.0 : -1.0;
			}

			var positionRange = CreateInstance( Start, true, position, true );
			double percentage = Convert( positionRange.Size ) / size;

			// Negate percentage when position lies before the interval.
			int positionCompare = position.CompareTo( Start );
			bool isPositionBeforeInterval = IsReversed
				? positionCompare > 0
				: positionCompare < 0;
			if ( isPositionBeforeInterval )
			{
				percentage *= -1;
			}

			return percentage;
		}

		/// <summary>
		/// Map a value from this range, to a value in another range linearly.
		/// </summary>
		/// <param name = "value">The value to map to another range.</param>
		/// <param name = "range">The range to which to map the value.</param>
		/// <returns>The value, mapped to the given range.</returns>
		public T Map( T value, IInterval<T, TSize> range )
		{
			return Map<T, TSize>( value, range );
		}

		/// <summary>
		/// Map a value from this range, to a value in another range of another type linearly.
		/// </summary>
		/// <typeparam name = "TOther">The type of the other range.</typeparam>
		/// <typeparam name = "TOtherSize">The type used to specify distances in between two values of <typeparamref name="TOther" />.</typeparam>
		/// <param name = "value">The value to map to another range.</param>
		/// <param name = "range">The range to which to map the value.</param>
		/// <returns>The value, mapped to the given range.</returns>
		public TOther Map<TOther, TOtherSize>( T value, IInterval<TOther, TOtherSize> range )
			where TOther : IComparable<TOther>
			where TOtherSize : IComparable<TOtherSize>
		{
			return range.GetValueAt( GetPercentageFor( value ) );
		}

		/// <summary>
		/// Does the given value lie in the interval or not.
		/// </summary>
		/// <param name = "value">The value to check for.</param>
		/// <returns>True when the value lies within the interval, false otherwise.</returns>
		public bool LiesInInterval( T value )
		{
			int startCompare = value.CompareTo( _start );
			int endCompare = value.CompareTo( _end );

			return (startCompare > 0 || (startCompare == 0 && _isStartIncluded))
				&& (endCompare < 0 || (endCompare == 0 && _isEndIncluded));
		}

		/// <summary>
		/// Does the given interval intersect the other interval.
		/// </summary>
		/// <param name = "interval">The interval to check for intersection.</param>
		/// <returns>True when the intervals intersect, false otherwise.</returns>
		public bool Intersects( IInterval<T, TSize> interval )
		{
			// Use 'true' start (smallest) and end (largest) of the passed interval.
			if ( interval.IsReversed )
			{
				interval = interval.Reverse();
			}

			int rightOfCompare = interval.Start.CompareTo( _end );
			int leftOfCompare = interval.End.CompareTo( _start );

			bool liesRightOf = rightOfCompare > 0 || (rightOfCompare == 0 && !(interval.IsStartIncluded && _isEndIncluded));
			bool liesLeftOf = leftOfCompare < 0 || (leftOfCompare == 0 && !(interval.IsEndIncluded && _isStartIncluded));

			return !(liesRightOf || liesLeftOf);
		}

		/// <summary>
		/// Limit a given value to this range. When the value is smaller/bigger than the range, snap it to the range border.
		/// TODO: For now this does not take into account whether the start or end of the range is included. Is this possible?
		/// </summary>
		/// <param name = "value">The value to limit.</param>
		/// <returns>The value limited to the range.</returns>
		public T Clamp( T value )
		{
			return value.CompareTo( _start ) < 0
				? _start
				: value.CompareTo( _end ) > 0
					? _end
					: value;
		}

		/// <summary>
		/// Limit a given range to this range.
		/// When part of the given range lies outside of this range, it isn't included in the resulting range.
		/// </summary>
		/// <param name = "range">The range to limit to this range.</param>
		/// <returns>The given range, which excludes all parts lying outside of this range. Null when empty.</returns>
		public IInterval<T, TSize> Clamp( IInterval<T, TSize> range )
		{
			var intersection = Intersection( range );
			if ( intersection == null )
			{
				return null;
			}

			// Use 'true' start (smallest) and end (largest) for the comparisons.
			if ( range.IsReversed )
			{
				range = range.Reverse();
			}
			if ( intersection.IsReversed )
			{
				intersection = intersection.Reverse();
			}

			bool thisIsSmaller = _start.CompareTo( range.Start ) <= 0;
			bool thisIsBigger = _end.CompareTo( range.End ) >= 0;
			var clamped = CreateInstance(
				thisIsSmaller ? range.Start : _start,
				thisIsSmaller ? intersection.IsStartIncluded : _isStartIncluded,
				thisIsBigger ? range.End : _end,
				thisIsBigger ? intersection.IsEndIncluded : _isEndIncluded );

			return IsReversed ? clamped.Reverse() : clamped;
		}

		/// <summary>
		/// Split the interval into two intervals at the given point, or nearest valid point.
		/// </summary>
		/// <param name = "atPoint">The point where to split.</param>
		/// <param name = "option">Option which specifies in which intervals the split point ends up.</param>
		/// <param name = "before">The interval in which to store the part before the point, if any, null otherwise.</param>
		/// <param name = "after">The interval in which to store the part after the point, if any, null otherwise.</param>
		public void Split( T atPoint, SplitOption option, out IInterval<T, TSize> before, out IInterval<T, TSize> after )
		{
			if ( atPoint.CompareTo( _start ) < 0 || atPoint.CompareTo( _end ) > 0 )
			{
				throw new ArgumentException(
					"The point specifying where to split the interval does not lie within the interval range.", "atPoint" );
			}

			// Part before.
			bool includeInLeft = option.EqualsAny( SplitOption.Left, SplitOption.Both );
			if ( atPoint.CompareTo( _start ) != 0 || includeInLeft )
			{
				before = CreateInstance(
					Start, IsStartIncluded,
					atPoint,
					includeInLeft );
			}
			else
			{
				before = null;
			}

			// Part after.
			bool includeInRight = option.EqualsAny( SplitOption.Right, SplitOption.Both );
			if ( atPoint.CompareTo( _end ) != 0 || includeInRight )
			{
				after = CreateInstance(
					atPoint,
					includeInRight,
					End, IsEndIncluded );
			}
			else
			{
				after = null;
			}
		}

		/// <summary>
		/// Subtract a given interval from the current interval.
		/// </summary>
		/// <param name = "subtract">The interval to subtract from this interval.</param>
		/// <returns>The resulting intervals after subtraction.</returns>
		public List<IInterval<T, TSize>> Subtract( IInterval<T, TSize> subtract )
		{
			// Use 'true' start (smallest) and end (largest) for the comparisons.
			if ( subtract.IsReversed )
			{
				subtract = subtract.Reverse();
			}

			// Subtracting empty intervals never changes the original interval.
			double size = Convert( subtract.Size );
			if ( size == 0 && !subtract.IsStartIncluded && !subtract.IsEndIncluded )
			{
				return new List<IInterval<T, TSize>> { this };
			}

			var result = new List<IInterval<T, TSize>>();

			if ( !Intersects( subtract ) )
			{
				// Nothing to subtract.
				result.Add( this );
			}
			else
			{
				bool startInInterval = LiesInInterval( subtract.Start );
				bool endInInterval = LiesInInterval( subtract.End );

				// Add remaining section at the start.   
				if ( startInInterval )
				{
					int startCompare = subtract.Start.CompareTo( _start );
					if ( startCompare > 0 || (startCompare == 0 && _isStartIncluded && !subtract.IsStartIncluded) )
					{
						IInterval<T, TSize> start = CreateInstance( _start, _isStartIncluded, subtract.Start, !subtract.IsStartIncluded );
						if ( IsReversed )
						{
							start = start.Reverse();
						}
						result.Add( start );
					}
				}

				// Add remaining section at the back.
				if ( endInInterval )
				{
					int endCompare = subtract.End.CompareTo( _end );
					if ( endCompare < 0 || (endCompare == 0 && _isEndIncluded && !subtract.IsEndIncluded) )
					{
						IInterval<T, TSize> back = CreateInstance( subtract.End, !subtract.IsEndIncluded, _end, _isEndIncluded );
						if ( IsReversed )
						{
							back = back.Reverse();
						}
						result.Add( back );
					}
				}
			}

			if ( IsReversed )
			{
				result.Reverse();
			}
			return result;
		}

		/// <summary>
		/// Returns the intersection of this interval with another.
		/// </summary>
		/// <param name = "interval">The interval to get the intersection for.</param>
		/// <returns>The intersection of this interval with the given other. Null when no intersection.</returns>
		public IInterval<T, TSize> Intersection( IInterval<T, TSize> interval )
		{
			if ( !Intersects( interval ) )
			{
				return null;
			}

			// Use 'true' start (smallest) and end (largest) of the passed interval.
			if ( interval.IsReversed )
			{
				interval = interval.Reverse();
			}

			int startCompare = _start.CompareTo( interval.Start );
			int endCompare = _end.CompareTo( interval.End );

			var intersection = CreateInstance(
				startCompare > 0 ? _start : interval.Start,
				startCompare == 0
					? _isStartIncluded && interval.IsStartIncluded // On matching boundary, only include when they both include the boundary.
					: startCompare > 0
						? _isStartIncluded
						: interval.IsStartIncluded, // Otherwise, use the corresponding boundary.
				endCompare < 0 ? _end : interval.End,
				endCompare == 0
					? _isEndIncluded && interval.IsEndIncluded
					: endCompare < 0
						? _isEndIncluded
						: interval.IsEndIncluded
				);

			return IsReversed ? intersection.Reverse() : intersection;
		}

		public override bool Equals( object obj )
		{
			var interval = obj as IInterval<T, TSize>;

			if ( interval == null )
			{
				return false;
			}

			return IsStartIncluded == interval.IsStartIncluded
				&& IsEndIncluded == interval.IsEndIncluded
				&& Start.CompareTo( interval.Start ) == 0
				&& End.CompareTo( interval.End ) == 0
				&& IsReversed == interval.IsReversed;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + _start.GetHashCode();
				hash = hash * 23 + _end.GetHashCode();
				hash = hash * 23 + _isStartIncluded.GetHashCode();
				hash = hash * 23 + _isEndIncluded.GetHashCode();
				hash = hash * 23 + IsReversed.GetHashCode();
				return hash;
			}
		}

		#endregion // Get operations.


		#region Enumeration

		/// <summary>
		/// Get values for each step within the interval.
		/// </summary>
		/// <param name="step">The step size between each value.</param>
		public IEnumerable<T> GetValues( TSize step )
		{
			return new Enumerator( this, step, Subtract, AddSize, Convert, Convert, CreateInstance );
		}

		/// <summary>
		/// Get values for each step within the interval, anchored to multiples of a specified anchor value.
		/// </summary>
		/// <param name="step">The step size between each value.</param>
		/// <param name="anchor">The value to which multiples of step are anchored.</param>
		public IEnumerable<T> GetValues( TSize step, T anchor )
		{
			return new Enumerator( this, step, Subtract, AddSize, Convert, Convert, CreateInstance, anchor );
		}

		/// <summary>
		/// Execute an action each step in an interval.
		/// </summary>
		/// <param name = "step">The size of the steps.</param>
		/// <param name = "stepAction">The operation to execute.</param>
		public void EveryStepOf( TSize step, Action<T> stepAction )
		{
			foreach ( var i in GetValues( step ) )
			{
				stepAction( i );
			}
		}

		#endregion  // Enumeration


		#region Modifiers

		/// <summary>
		/// Returns an expanded interval of the current interval up to the given value (and including).
		/// When the value lies within the interval the returned interval is the same.
		/// </summary>
		/// <param name = "value">The value up to which to expand the interval.</param>
		public IInterval<T, TSize> ExpandTo( T value )
		{
			return ExpandTo( value, true );
		}

		/// <summary>
		/// Returns an expanded interval of the current interval up to the given value.
		/// When the value lies within the interval the returned interval is the same.
		/// </summary>
		/// <param name = "value">The value up to which to expand the interval.</param>
		/// <param name = "include">Include the value to which is expanded in the interval.</param>
		public IInterval<T, TSize> ExpandTo( T value, bool include )
		{
			T start = _start;
			T end = _end;
			bool isStartIncluded = _isStartIncluded;
			bool isEndIncluded = _isEndIncluded;

			// Modify interval when needed.
			int startCompare = value.CompareTo( _start );
			int endCompare = value.CompareTo( _end );
			if ( startCompare <= 0 )
			{
				start = value;
				isStartIncluded |= include;
			}
			if ( endCompare >= 0 )
			{
				end = value;
				isEndIncluded |= include;
			}

			var extended = CreateInstance( start, isStartIncluded, end, isEndIncluded );
			return IsReversed ? extended.Reverse() : extended;
		}

		/// <summary>
		/// Returns an interval offsetted from the current interval by a specified amount.
		/// </summary>
		/// <param name="amount">How much to move the interval.</param>
		public IInterval<T, TSize> Move( TSize amount )
		{
			return CreateInstance(
				AddSize( Start, amount ),
				IsStartIncluded,
				AddSize( End, amount ),
				IsEndIncluded );
		}

		/// <summary>
		/// Returns a scaled version of the current interval.
		/// </summary>
		/// <param name="scale">
		/// Percentage to scale the interval up or down.
		/// Smaller than 1.0 to scale down, larger to scale up.
		/// </param>
		/// <param name="aroundPercentage">The percentage inside the interval around which to scale.</param>
		public IInterval<T, TSize> Scale( double scale, double aroundPercentage = 0.5 )
		{
			return Scale( scale, null, aroundPercentage );
		}

		/// <summary>
		/// Returns a scaled version of the current interval, but prevents the interval from exceeding the values specified in a passed limit.
		/// This is useful to prevent <see cref="ArgumentOutOfRangeException" /> during calculations for certain types.
		/// </summary>
		/// <param name="scale">
		/// Percentage to scale the interval up or down.
		/// Smaller than 1.0 to scale down, larger to scale up.
		/// </param>
		/// <param name="limit">The limit which the interval snaps to when scaling exceeds it.</param>
		/// <param name="aroundPercentage">The percentage inside the interval around which to scale.</param>
		public IInterval<T, TSize> Scale( double scale, IInterval<T, TSize> limit, double aroundPercentage = 0.5 )
		{
			TSize scaledSize = Convert( Convert( Size ) * scale );
			TSize sizeDiff = SubtractSizes( Size, scaledSize ); // > 0 larger, < 0 smaller

			TSize startAddition = Convert( Convert( sizeDiff ) * aroundPercentage );
			bool startExceeded = false;
			if ( limit != null )
			{
				TSize maxStartSubtraction = Subtract( limit.Start, Start );
				startExceeded = scale > 1 && startAddition.CompareTo( maxStartSubtraction ) < 0;
			}
			T start = startExceeded ? limit.Start : AddSize( _start, startAddition );

			TSize endSubtraction = SubtractSizes( sizeDiff, startAddition );
			bool endExceeded = false;
			if ( limit != null )
			{
				TSize maxEndAddition = Subtract( End, limit.End );
				endExceeded = scale > 1 && maxEndAddition.CompareTo( endSubtraction ) > 0;
			}
			T end = endExceeded ? limit.End : SubtractSize( _end, endSubtraction );

			var scaled = CreateInstance( start, _isStartIncluded, end, _isEndIncluded );
			return IsReversed ? scaled.Reverse() : scaled;
		}

		/// <summary>
		/// Returns a reversed version of the current interval, swapping the start position with the end position.
		/// </summary>
		public IInterval<T, TSize> Reverse()
		{
			var interval = (AbstractInterval<T, TSize>)Clone();
			interval.IsReversed = !IsReversed;

			return interval;
		}

		#endregion  // Modifiers


		public object Clone()
		{
			var interval = (AbstractInterval<T, TSize>)CreateInstance( _start, _isStartIncluded, _end, _isEndIncluded );
			interval.IsReversed = IsReversed;

			return interval;
		}

		/// <summary>
		/// True when values within this interval are of an integral type, false otherwise. 
		/// </summary>
		protected abstract bool IsIntegralType { get; }
		/// <summary>
		/// Create a new interval of the current type with a specified start and end.
		/// </summary>
		/// <param name = "start">The start of the interval.</param>
		/// <param name = "isStartIncluded">Is the value at the start of the interval included in the interval.</param>
		/// <param name = "end">The end of the interval.</param>
		/// <param name = "isEndIncluded">Is the value at the end of the interval included in the interval.</param>
		protected abstract IInterval<T, TSize> CreateInstance( T start, bool isStartIncluded, T end, bool isEndIncluded );
		protected abstract double Convert( TSize value );
		protected abstract TSize Convert( double value );
		protected abstract TSize Subtract( T value1, T value2 );
		protected abstract T SubtractSize( T value, TSize size );
		protected abstract TSize SubtractSizes( TSize value1, TSize value2 );
		protected abstract T AddSize( T value, TSize size );
	}
}
