using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// HTTPClient テスト
	/// </summary>
	public class Test0004
	{
		public void Test01()
		{
			Test01_a("https://www.google.com");
			Test01_a("https://www.youtube.com");
			Test01_a("https://www.amazon.co.jp");

			ProcMain.WriteLog("OK! (TEST-0004-01)");
		}

		private void Test01_a(string url)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string resFile = wd.MakePath();

				HTTPClient hc = new HTTPClient(url)
				{
					ConnectTimeoutMillis = 43200000, // 12 hour
					TimeoutMillis = 86400000, // 1 day
					IdleTimeoutMillis = 180000, // 3 min
					ResBodySizeMax = 8000000000000000, // 8 PB (8000 TB)
					ResFile = resFile,
				};

				hc.Get();

				File.Copy(resFile, SCommon.NextOutputPath() + ".txt");
			}
		}
	}
}
