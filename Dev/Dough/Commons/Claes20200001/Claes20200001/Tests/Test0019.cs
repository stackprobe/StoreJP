using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// ArgsReader テスト
	/// </summary>
	public class Test0019
	{
		public void Test01()
		{
			Test01_a("/X", "/X");
			Test01_a("/X", "/x");
			Test01_a("/x", "/X");
			Test01_a("/x", "/x");

			Console.WriteLine("OK! (TEST-0019-01)");
		}

		private void Test01_a(string arg, string spell)
		{
			ArgsReader ar = new ArgsReader(new string[] { spell });

			if (!ar.ArgIs(arg))
				throw null; // BUG
		}

		public void Test02()
		{
			ArgsReader ar = new ArgsReader(new string[] { "/X", "/Y", "/Z" });

			if (ar.ArgIs("/Z")) throw null;
			if (ar.ArgIs("/Y")) throw null;
			if (!ar.ArgIs("/X")) throw null;

			if (ar.ArgIs("/Z")) throw null;
			if (ar.ArgIs("/X")) throw null;
			if (!ar.ArgIs("/Y")) throw null;

			if (ar.ArgIs("/Y")) throw null;
			if (ar.ArgIs("/X")) throw null;
			if (!ar.ArgIs("/Z")) throw null;

			if (ar.ArgIs("/Z")) throw null;
			if (ar.ArgIs("/Y")) throw null;
			if (ar.ArgIs("/X")) throw null;

			Console.WriteLine("OK! (TEST-0019-02)");
		}
	}
}
