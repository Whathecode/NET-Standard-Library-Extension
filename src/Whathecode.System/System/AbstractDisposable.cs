using System;


namespace Whathecode.System
{
	/// <summary>
	/// A base implementation for the Disposable pattern, implementing <see cref="IDisposable" />.
	/// </summary>
	public abstract class AbstractDisposable : IDisposable
	{
		public event Action OnDisposed;
		bool _isCleanedUp = false;


		~AbstractDisposable()
		{
			CleanUp( false );
		}


		public void Dispose()
		{
			CleanUp( true );

			// Destructor doesn't need to be called anymore. The object is already disposed.
			GC.SuppressFinalize( this );
		}

		void CleanUp( bool isDisposing )
		{
			if ( _isCleanedUp )
			{
				return;
			}

			if ( isDisposing )
			{
				FreeManagedResources();
			}
			FreeUnmanagedResources();
			_isCleanedUp = true;

			OnDisposed?.Invoke();
		}

		/// <summary>
		/// This method needs to be called from each public instance method to prevent operations on the object after it has been disposed.
		/// </summary>
		/// <param name = "message">The error message that explains the reason for the exception.</param>
		protected void ThrowExceptionIfDisposed( string message = null )
		{
			if ( _isCleanedUp )
			{
				throw new ObjectDisposedException( null, message );
			}
		}

		/// <summary>
		/// This is only called once, when the managed resources haven't been cleaned up yet.
		/// </summary>
		protected abstract void FreeManagedResources();

		/// <summary>
		/// This is only called once, and should clean up all the unmanaged resources.
		/// </summary>
		protected abstract void FreeUnmanagedResources();
	}
}
