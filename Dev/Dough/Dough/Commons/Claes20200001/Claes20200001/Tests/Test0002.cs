using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Base64 テスト
	/// </summary>
	public class Test0002
	{
		public void Test01()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0002-01, " + testcnt); // cout

				byte[] data = SCommon.CRandom.GetBytes(SCommon.CRandom.GetInt(1000));
				string str = SCommon.Base64.I.Encode(data);

				if (str == null)
					throw null;

				if (str.Length % 4 != 0)
					throw null;

				if (!Regex.IsMatch(str, "^[A-Za-z0-9+/]*=*$"))
					throw null;

				byte[] retData = SCommon.Base64.I.Decode(str);

				if (retData == null)
					throw null;

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;
			}
			Console.WriteLine("OK!");
		}

		public void Test02()
		{
			Test02_a(SCommon.ALPHA_UPPER + SCommon.ALPHA_LOWER + SCommon.DECIMAL + "+/");
			Test02_a(SCommon.ASCII);
			Test02_a(SCommon.HALF);
			Test02_a(SCommon.HALF + SCommon.MBC_ASCII);
			Test02_a(SCommon.HALF + SCommon.MBC_ASCII + "いろはにほへと★日本語");

			Console.WriteLine("OK!");
		}

		private void Test02_a(string testChars)
		{
			Console.WriteLine(string.Join(", ", "TEST-0002-02", testChars)); // cout

			char[] TEST_CHARS = testChars.ToArray();

			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				string str = new string(SCommon.Generate(SCommon.CRandom.GetInt(1000), () => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

				byte[] data = SCommon.Base64.I.Decode(str); // でたらめな入力文字列

				if (data == null)
					throw null;
			}
			Console.WriteLine("OK");
		}

		public void Test03()
		{
			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				byte[] data = SCommon.CRandom.GetBytes(SCommon.CRandom.GetInt(30));

				// ----
				// そのまま

				string str = SCommon.Base64.I.Encode(data);
				byte[] retData = SCommon.Base64.I.Decode(str);

				Console.WriteLine(str + " --> " + SCommon.Hex.I.GetString(retData));

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;

				// ----
				// パディング無し

				str = SCommon.Base64.I.EncodeNoPadding(data);
				retData = SCommon.Base64.I.Decode(str);

				Console.WriteLine(str + " --> " + SCommon.Hex.I.GetString(retData));

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;

				// ----
				// 余計な文字をランダムに挿入

				str = SCommon.Base64.I.Encode(data);
				str = RandomInsert(str, 0.3, new string[] { "\r\n", "\t", " ", "い", "ろは", "にほへ" });
				retData = SCommon.Base64.I.Decode(str);

				Console.WriteLine(str + " --> " + SCommon.Hex.I.GetString(retData));

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;

				// ----
				// 余計な文字をランダムに挿入 + パディング無し

				str = SCommon.Base64.I.EncodeNoPadding(data);
				str = RandomInsert(str, 0.3, new string[] { "\r\n", "\t", " ", "い", "ろは", "にほへ" });
				retData = SCommon.Base64.I.Decode(str);

				Console.WriteLine(str + " --> " + SCommon.Hex.I.GetString(retData));

				if (SCommon.Comp(data, retData) != 0) // ? 不一致
					throw null;

				// ----

				Console.WriteLine("OK");
			}
			Console.WriteLine("OK! (TEST-0002-03)");
		}

		private string RandomInsert(string str, double rate, string[] ptns)
		{
			StringBuilder buff = new StringBuilder();

			for (int index = 0; ; index++)
			{
				while (SCommon.CRandom.GetRate() < rate)
					buff.Append(SCommon.CRandom.ChooseOne(ptns));

				if (str.Length <= index)
					break;

				buff.Append(str[index]);
			}
			return buff.ToString();
		}
	}
}
