using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Hex テスト
	/// </summary>
	public class Test0005
	{
		public void Test01()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0005-01, " + testcnt); // cout

				byte[] data = SCommon.CRandom.GetBytes(SCommon.CRandom.GetInt(1000));
				string str = SCommon.Hex.I.GetString(data);

				if (str == null)
					throw null;

				if (str.Length != data.Length * 2)
					throw null;

				if (!Regex.IsMatch(str, "^[0-9a-f]*$"))
					throw null;

				byte[] retData = SCommon.Hex.I.GetBytes(str);

				if (retData == null)
					throw null;

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;
			}
			Console.WriteLine("OK!");
		}

		public void Test02()
		{
			Console.WriteLine("TEST-0005-02");

			Test02_a("0123456789ABCDEF", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef });
			Test02_a("0123456789AbCdEf", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef });
			Test02_a("0123456789aBcDeF", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef });
			Test02_a("0123456789abcdef", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef });

			Console.WriteLine("OK!");
		}

		private void Test02_a(string str, byte[] expectBytes)
		{
			byte[] bytes = SCommon.Hex.I.GetBytes(str);

			if (SCommon.Comp(bytes, expectBytes) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK");
		}
	}
}
