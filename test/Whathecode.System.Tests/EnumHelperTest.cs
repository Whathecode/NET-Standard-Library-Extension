using System;
using System.Collections.Generic;
using Whathecode.System;
using Whathecode.System.Linq;
using Xunit;


namespace Whathecode.Tests.System
{
	/// <summary>
	/// Unit tests for <see href="EnumHelper{T}" />.
	/// </summary>
	public class EnumHelperTest
	{
		[Flags]
		public enum FlagsEnum
		{
			Flag1,
			Flag2,
			Flag3,
			Flag4,
			Flag5
		}


		[Fact]
		public void GetFlaggedValuesTest()
		{
			const FlagsEnum flags = FlagsEnum.Flag1 | FlagsEnum.Flag3;
			IEnumerable<FlagsEnum> setFlags = EnumHelper<FlagsEnum>.GetFlaggedValues( flags );

			Assert.True( setFlags.ContainsOnly( new[] { FlagsEnum.Flag1, FlagsEnum.Flag3 } ) );
		}
	}
}
