using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Tokenize テスト
	/// </summary>
	public class Test0010
	{
		public void Test01()
		{
			Test01_a("ABC:DEF:GHI:JKL", ":", -1, new string[] { "ABC", "DEF", "GHI", "JKL" });
			Test01_a("ABC:DEF:GHI:JKL", ":", 2, new string[] { "ABC", "DEF:GHI:JKL" });
			Test01_a("ABC:DEF:GHI:JKL", ":", 3, new string[] { "ABC", "DEF", "GHI:JKL" });
			Test01_a("ABC:DEF:GHI:JKL", ":", 4, new string[] { "ABC", "DEF", "GHI", "JKL" });
			Test01_a("ABC:DEF:GHI:JKL", ":", 5, new string[] { "ABC", "DEF", "GHI", "JKL" });

			Console.WriteLine("OK! (TEST-0010-01)");
		}

		private void Test01_a(string str, string delimiters, int limit, string[] expect)
		{
			string[] tokens = SCommon.Tokenize(str, delimiters, false, false, limit);

			if (tokens == null)
				throw null;

			if (SCommon.Comp(tokens, expect, SCommon.Comp) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK");
		}

		public void Test02()
		{
			Test02_a("...---...", ".", false, new string[] { "", "", "", "---", "", "", "" });
			Test02_a("...---...", ".", true, new string[] { "---" });

			Console.WriteLine("OK! (TEST-0010-02)");
		}

		private void Test02_a(string str, string delimiters, bool ignoreEmpty, string[] expect)
		{
			string[] tokens = SCommon.Tokenize(str, delimiters, false, ignoreEmpty);

			if (tokens == null)
				throw null;

			if (SCommon.Comp(tokens, expect, SCommon.Comp) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK");
		}

		public void Test03()
		{
			Test03_a("2023/04/05 23:30:59", "/ :", false
				, new string[] { "2023", "04", "05", "23", "30", "59" });
			Test03_a("2023/04/05 23:30:59", SCommon.DECIMAL, true
				, new string[] { "2023", "04", "05", "23", "30", "59" });

			Console.WriteLine("OK! (TEST-0010-03)");
		}

		private void Test03_a(string str, string delimiters, bool meaningFlag, string[] expect)
		{
			string[] tokens = SCommon.Tokenize(str, delimiters, meaningFlag);

			if (tokens == null)
				throw null;

			if (SCommon.Comp(tokens, expect, SCommon.Comp) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK");
		}

		public void Test04()
		{
			Test04_a("", "", new string[] { "" });
			Test04_a("", ":.;,", new string[] { "" });
			Test04_a("A:B.C;D,E", ":.;,", new string[] { "A", "B", "C", "D", "E" });
			Test04_a(":.;,", ":.;,", new string[] { "", "", "", "", "" });
			Test04_a("AB-CD-EF", "", new string[] { "AB-CD-EF" });

			Console.WriteLine("OK! (TEST-0010-04)");
		}

		private void Test04_a(string str, string delimiters, string[] expect)
		{
			string[] tokens = SCommon.Tokenize(str, delimiters);

			if (tokens == null)
				throw null;

			if (SCommon.Comp(tokens, expect, SCommon.Comp) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK");
		}
	}
}
