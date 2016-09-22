using System;
using Whathecode.System;
using Xunit;


namespace Whathecode.Tests.System
{
	public class AbstractDisposableTest
    {
		class NoUnmanagedResources : AbstractDisposable
		{
			public void PublicInstanceOperation()
			{
				// Each public instance method should call this to prevent operations after the object has been disposed.
				ThrowExceptionIfDisposed();
			}

			protected override void FreeManagedResources() { }

			protected override void FreeUnmanagedResources() { }
		}


		[Fact]
		public void OnDisposedTest()
		{
			// TODO: Create Assert method to test whether events are called.
			bool eventCalled = false;
			NoUnmanagedResources toDispose = new NoUnmanagedResources();
			toDispose.OnDisposed += () => eventCalled = true;
			toDispose.Dispose();

			Assert.True( eventCalled );
		}

		[Fact]
		public void ThrowExceptionIfDisposedTest()
		{
			NoUnmanagedResources toDispose = new NoUnmanagedResources();
			toDispose.Dispose();

			Assert.Throws<ObjectDisposedException>( () => toDispose.PublicInstanceOperation() );
		}
	}
}
