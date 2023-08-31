using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.ToCreatablePath テスト
	/// </summary>
	public class Test0016
	{
		public void Test01()
		{
			string outDir = SCommon.GetOutputDir();

			for (int c = 0; c < 100; c++)
			{
				SCommon.CreateDir(SCommon.ToCreatablePath(Path.Combine(outDir, "RamenSobaUdon")));
			}
			for (int c = 0; c < 100; c++)
			{
				File.WriteAllBytes(SCommon.ToCreatablePath(Path.Combine(outDir, "ShuumaiGyouza.txt")), SCommon.EMPTY_BYTES);
			}
			Console.WriteLine("done! (TEST-0016-01)");
		}
	}
}
