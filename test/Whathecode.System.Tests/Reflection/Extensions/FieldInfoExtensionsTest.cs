using System;
using System.Reflection;
using Whathecode.System.Reflection.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public class FieldInfoExtensionsTest
	{
		#pragma warning disable 219, 169, 414
		public class Fields
		{
			public const string TestString = "Test";

			public string Public = TestString;
			string _private = TestString;
			public object StringObject = TestString;
		}
		#pragma warning restore 219, 169, 414


		readonly Fields _fields = new Fields();
		readonly Type _type = typeof( Fields );
		readonly FieldInfo _publicField = typeof( Fields ).GetField( "Public" );
		

		[Fact]
		public void CreateGetterDelegateTest()
		{
			// Public/private.
			Func<string> publicGetter = _publicField.CreateGetter<string>().ClosedOver( _fields );
			Assert.Equal( Fields.TestString, publicGetter() );
			Func<string> privateGetter = _type
				.GetField("_private", BindingFlags.Instance | BindingFlags.NonPublic )
				.CreateGetter<string>().ClosedOver( _fields );
			Assert.Equal( Fields.TestString, privateGetter() );

			// Delegate types which don't correspond, but allowed variance.
			Func<object> upcasting = _publicField.CreateGetter<string>().ClosedOver( _fields );
			Assert.Equal( Fields.TestString, upcasting() );

			// Delegate types which don't correspond, and no variance possible.
			Assert.Throws<ArgumentException>( () => _type.GetField( "StringObject" ).CreateGetter<string>().ClosedOver( _fields ) );
		}

		[Fact]
		public void CreateOpenInstanceGetterDelegateTest()
		{
			Func<Fields, string> openInstance = _publicField.CreateGetter<string>().OpenInstance<Fields>();
			Assert.Equal( Fields.TestString, openInstance( _fields ) );
		}
	}
}