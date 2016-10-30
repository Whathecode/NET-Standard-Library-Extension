using System;
using System.Collections.Generic;


namespace Whathecode.System
{
	public static partial class Extensions
	{
		/// <summary>
		/// Gets the value associated with the specified key and passes it to delegate which can use it.
		/// </summary>
		/// <typeparam name = "TKey">The type of the keys in the dictionary.</typeparam>
		/// <typeparam name = "TValue">The type of the values in the dictionary.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "key">The key of the element to use the value of if it's contained within the dictionary.</param>
		/// <param name = "useValue">The action to perform with the value if the key is contained within the dictionary.</param>
		/// <returns>true when the key was present in the dictionary and the action was performed, false otherwise.</returns>
		public static bool TryUseValue<TKey, TValue>( this Dictionary<TKey, TValue> source, TKey key, Action<TValue> useValue )
		{
			TValue value;
			if ( source.TryGetValue( key, out value ) )
			{
				useValue( value );

				return true;
			}

			return false;
		}

		/// <summary>
		/// Adds/updates or removes a certain key from the dictionary. The key is removed when the value is null.
		/// </summary>
		/// <typeparam name = "TKey">The type of the keys in the dictionary.</typeparam>
		/// <typeparam name = "TValue">The type of the values in the dictionary.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "key">The key of the element in the dictionary to update.</param>
		/// <param name = "value">The new value for the key.</param>
		public static void Update<TKey, TValue>( this Dictionary<TKey, TValue> source, TKey key, TValue value )
			where TValue : class
		{
			if ( source == null || key == null )
			{
				throw new ArgumentNullException( "Both source and key should be different from null." );
			}

			source.Update( key, value, v => v != null );
		}

		/// <summary>
		/// Adds/updates or removes a certain key from the dictionary.
		/// </summary>
		/// <typeparam name = "TKey">The type of the keys in the dictionary.</typeparam>
		/// <typeparam name = "TValue">The type of the values in the dictionary.</typeparam>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "key">The key of the element in the dictionary to update.</param>
		/// <param name = "value">The new value for the key.</param>
		/// <param name = "validValue">
		/// Determines whether or not the new value is a valid value. When it returns true the key is updated with the new value,
		/// otherwise the key is removed from the dictionary.
		/// </param>
		public static void Update<TKey, TValue>( this Dictionary<TKey, TValue> source, TKey key, TValue value, Func<TValue, bool> validValue )
		{
			if ( source == null || key == null || validValue == null )
			{
				throw new ArgumentNullException( "Both source, key, and validValue should be different from null." );
			}

			if ( validValue( value ) )
			{
				source[ key ] = value;
			}
			else
			{
				source.Remove( key );
			}
		}
	}
}