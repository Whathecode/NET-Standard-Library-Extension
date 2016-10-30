using System;
using System.Reflection;
using Whathecode.System.Reflection;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
	public partial class Extensions
	{
		public class EventInfoTest
		{
			class StaticEvents
			{
				#pragma warning disable 67
				public event Action NonStatic;
				static public event Action Static;

				static event Action CustomEvent;
				public event Action CustomNonStatic
				{
					add { CustomEvent += value; }
					remove { CustomEvent -= value; }
				}
				static public event Action CustomStatic
				{
					add { CustomEvent += value; }
					remove { CustomEvent -= value; }
				}
				#pragma warning restore 67
			}


			[Theory]
			[InlineData( "NonStatic", false )]
			[InlineData( "Static", true )]
			[InlineData( "CustomNonStatic", false )]
			[InlineData( "CustomStatic", true )]
			public void IsStaticTest( string member, bool expectedStatic )
			{
				Assert.Equal( expectedStatic, typeof( StaticEvents ).GetEvent( member ).IsStatic() );
			}
		}
	}
}
