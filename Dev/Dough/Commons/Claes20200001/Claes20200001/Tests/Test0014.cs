using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Batch テスト
	/// </summary>
	public class Test0014
	{
		public void Test01()
		{
			using (WorkingDir wd = new WorkingDir())
			{
				SCommon.Batch(
					new string[]
					{
						"ECHO Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Planet-9 > BatchTestOut.txt",
					},
					wd.GetPath(".")
					);

				string str = File.ReadAllText(wd.GetPath("BatchTestOut.txt"), Encoding.ASCII).Trim();

				if (str != "Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Planet-9")
					throw null;
			}
			Console.WriteLine("OK! (TEST-0014-01)");
		}
	}
}
