using System;
using System.Reflection;
using Whathecode.System.Reflection.Extensions;
using Xunit;


namespace Whathecode.Tests.System.Reflection.Extensions
{
    public class MemberInfoExtensionsTest
    {
		class Members
		{
			public int Field = 0 ;
			public int Property { get; }
			public event Action Event = delegate { };
			public class InnerClass { }
		}


		[Theory]
		[InlineData( "Field", typeof( int ) )]
		[InlineData( "Property", typeof( int ) )]
		[InlineData( "Event", typeof( Action ) )]
		[InlineData( "InnerClass", typeof( Members.InnerClass ) )]
		public void GetMemberTypeTest( string member, Type expectedType )
		{
			Type type = typeof( Members );

			Assert.Equal( expectedType, type.GetMember( member )[ 0 ].GetMemberType() );
		}
    }
}
