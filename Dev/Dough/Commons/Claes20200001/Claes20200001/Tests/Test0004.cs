using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Serializer テスト
	/// </summary>
	public class Test0004
	{
		public void Test01()
		{
			char[] TEST_CHARS = (SCommon.HALF + SCommon.MBC_ASCII + "いろはにほへと★日本語").ToArray();

			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				if (testcnt % 100 == 0) Console.WriteLine("TEST-0004-01, " + testcnt); // cout

				string[] lines = SCommon.Generate(
					SCommon.CRandom.GetInt(100),
					() => new string(SCommon.Generate(
						SCommon.CRandom.GetInt(100),
						() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray())).ToArray();

				string strSrlz = SCommon.Serializer.I.Join(lines);

				if (strSrlz == null)
					throw null;

				if (strSrlz == "")
					throw null;

				if (!Regex.IsMatch(strSrlz, "^[0-9][A-Za-z0-9+/]*[0-9]$"))
					throw null;

				string[] retLines = SCommon.Serializer.I.Split(strSrlz);

				if (retLines == null)
					throw null;

				if (SCommon.Comp(lines, retLines, SCommon.Comp) != 0) // ? 不一致
					throw null;
			}
			Console.WriteLine("OK!");
		}
	}
}
