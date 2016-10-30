namespace Whathecode.System.Algorithm
{
	/// <summary>
	/// A gate which skips a specified amount of entries before opening.
	/// </summary>
	public class SkipGate : AbstractGate
	{
		readonly int _skipCount;
		int _curCount;


		/// <summary>
		/// Create a new gate which skips a specified amount of entries before opening.
		/// </summary>
		/// <param name = "skipCount">The amount of times entry is denied before opening.</param>
		/// <param name = "autoReset">Whether the gate is reset after entry is successful.</param>
		public SkipGate( int skipCount, bool autoReset = false )
			: base( autoReset )
		{
			_skipCount = skipCount;
		}


		protected override bool TryEnterGate()
		{
			return ++_curCount > _skipCount;
		}

		protected override void ResetGate()
		{
			_curCount = 0;
		}
	}
}
