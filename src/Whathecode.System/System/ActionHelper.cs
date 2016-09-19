using System;
using System.Collections.Generic;


namespace Whathecode.System
{
	/// <summary>
	/// A helper class to do common <see cref = "Action" /> operations.
	/// </summary>
	public class ActionHelper
	{
		/// <summary>
		/// Lists all the possible <see cref = "Action">Action</see> delegates, linked to the amount of parameters the action has.
		/// </summary>
		static readonly Dictionary<int, Type> ActionTypes = new Dictionary<int, Type>
		{
			{ 0, typeof( Action ) },
			{ 1, typeof( Action<> ) },
			{ 2, typeof( Action<,> ) },
			{ 3, typeof( Action<,,> ) },
			{ 4, typeof( Action<,,,> ) },
			{ 5, typeof( Action<,,,,> ) },
			{ 6, typeof( Action<,,,,,> ) },
			{ 7, typeof( Action<,,,,,,> ) },
			{ 8, typeof( Action<,,,,,,,> ) },
			{ 9, typeof( Action<,,,,,,,,> ) },
			{ 10, typeof( Action<,,,,,,,,,> ) },
			{ 11, typeof( Action<,,,,,,,,,,> ) },
			{ 12, typeof( Action<,,,,,,,,,,,> ) },
			{ 13, typeof( Action<,,,,,,,,,,,,> ) },
			{ 14, typeof( Action<,,,,,,,,,,,,,> ) },
			{ 15, typeof( Action<,,,,,,,,,,,,,,> ) },
			{ 16, typeof( Action<,,,,,,,,,,,,,,,> ) }
		};

		/// <summary>
		/// Returns the type of an Action delegate with the given amount of parameters.
		/// </summary>
		/// <param name = "numberOfParameters">The amount of parameters. Must lie between [0, 16].</param>
		/// <returns>Type of the Action delegate with the given amount of parameters.</returns>
		public static Type GetActionType( int numberOfParameters )
		{
			if ( numberOfParameters < 0 || numberOfParameters > 16 )
			{
				throw new ArgumentException( "Number of specified parameters out of range: must lie between [0, 16].", nameof( numberOfParameters ) );
			}

			return ActionTypes[ numberOfParameters ];
		}
	}
}