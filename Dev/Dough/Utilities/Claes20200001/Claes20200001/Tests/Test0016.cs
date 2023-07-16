using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// UniqueStringIssuer テスト
	/// </summary>
	public class Test0016
	{
		public void Test01()
		{
			UniqueStringIssuer usi = new TestUSI_01();

			for (int d = 1; d <= 5; d++)
			{
				for (int c = 0; c < 100000; c++) // 10 % 発行
					usi.Issue();

				Console.WriteLine("{0} % 発行しました。", d * 10);
			}

			SCommon.ToThrowPrint(() =>
			{
				for (int c = 0; c < 100000; c++) // 10 % 発行
					usi.Issue();
			});

			Console.WriteLine("OK! (TEST-0016-01)");
		}

		private class TestUSI_01 : UniqueStringIssuer
		{
			protected override string Generate()
			{
				return "" + SCommon.CRandom.GetInt(1000000);
			}
		}
	}
}
