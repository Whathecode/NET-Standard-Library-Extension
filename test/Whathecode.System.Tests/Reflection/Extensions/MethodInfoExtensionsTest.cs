using System;
using System.Reflection;
using Whathecode.System.Reflection.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public class MethodInfoExtensionsTest
	{
		#region Common test members

		const string TestString = "bleh";
		readonly MethodInfo _toUpperMethod = typeof( string ).GetMethod( "ToUpper", Type.EmptyTypes );

		#endregion // Common test members


		[Fact]
		public void CreateDelegateTest()
		{
			Func<string> toUpper = _toUpperMethod.CreateDelegate<Func<string>>( TestString );
			Assert.Equal( TestString.ToUpper(), toUpper() );
		}

		[Fact]
		public void CreateDynamicInstanceDelegateTest()
		{
			Func<string, string> toUpper = _toUpperMethod.CreateOpenInstanceDelegate<Func<string, string>>();
			Assert.Equal( TestString.ToUpper(), toUpper( TestString ) );
		}
	}
}