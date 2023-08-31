using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// WorkingDir テスト
	/// </summary>
	public class Test0007
	{
		public void Test01()
		{
			using (WorkingDir wd = new WorkingDir())
			{
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
			}
			using (WorkingDir wd = new WorkingDir())
			{
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
			}
			using (WorkingDir wd = new WorkingDir())
			{
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
				Console.WriteLine(wd.MakePath());
			}
			Console.WriteLine("done! (TEST-0007-01)");
		}
	}
}
