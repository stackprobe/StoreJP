using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.GetSJISBytes テスト
	/// </summary>
	public class Test0013
	{
		public void Test01()
		{
			string str = new string(Enumerable.Range(0, 0x22).Select(chr => (char)chr).ToArray());
			byte[] data = SCommon.GetSJISBytes(str);
			byte[] expect = Encoding.ASCII.GetBytes("?????????\t\n??\r?????????????????? !");

			if (SCommon.Comp(data, expect) != 0) // ? 不一致
				throw null;

			Console.WriteLine("OK! (TEST-0013-01)");
		}
	}
}
