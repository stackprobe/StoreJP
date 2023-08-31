using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Generate テスト
	/// </summary>
	public class Test0011
	{
		public void Test01()
		{
			int counter = 1000;

			Func<int> generator = () =>
			{
				counter += 30;
				return counter;
			};

			int[] list = SCommon.Generate(5, generator).ToArray();

			if (
				list == null ||
				list.Length != 5 ||
				list[0] != 1030 ||
				list[1] != 1060 ||
				list[2] != 1090 ||
				list[3] != 1120 ||
				list[4] != 1150
				)
				throw null;

			// ----

			if (SCommon.Generate(10, () => 1).Count() != 10) throw null;
			if (SCommon.Generate(20, () => 1).Count() != 20) throw null;
			if (SCommon.Generate(30, () => 1).Count() != 30) throw null;
			if (SCommon.Generate(40, () => 1).Count() != 40) throw null;
			if (SCommon.Generate(50, () => 1).Count() != 50) throw null;

			// ----

			if (SCommon.Generate(0, () => 1).Count() != 0) throw null;
			if (SCommon.Generate(-1, () => 1).Take(1000).Count() != 1000) throw null;
			if (SCommon.Generate(-1, () => 1).Take(2345).Count() != 2345) throw null;

			// ----

			Console.WriteLine("OK! (TEST-0011-01)");
		}
	}
}
