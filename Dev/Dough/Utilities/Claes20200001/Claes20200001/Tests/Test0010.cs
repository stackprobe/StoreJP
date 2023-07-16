using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// Anchorable テスト
	/// </summary>
	public class Test0010
	{
		private class BigShip : Anchorable<BigShip>
		{
			public void Run()
			{
				if (I != this)
					throw null; // 想定外
			}
		}

		public void Test01()
		{
			if (BigShip.I != null)
				throw null; // 想定外

			using (new BigShip())
			{
				BigShip.I.Run();
			}

			if (BigShip.I != null)
				throw null; // 想定外

			using (new BigShip())
			{
				BigShip.I.Run();

				SCommon.ToThrowPrint(() => new BigShip());

				BigShip.I.Run();
			}

			if (BigShip.I != null)
				throw null; // 想定外

			Console.WriteLine("OK! (TEST-0010-01)");
		}

		/*
		private class Test02_Class0001 : Anchorable<int> // 型エラーになる想定
		{ }
		//*/

		/*
		private class Test02_Class0002_a
		{ }
		private class Test02_Class0002 : Anchorable<Test01_Class0002_a> // 型エラーになる想定
		{ }
		//*/

		private class Test02_Class0003_a : Anchorable<Test02_Class0003_a>
		{ }

		private class Test02_Class0003 : Anchorable<Test02_Class0003_a> // 想定外の継承
		{
			public void Run()
			{ }
		}

		public void Test02()
		{
			SCommon.ToThrowPrint(() => new Test02_Class0003());

			Console.WriteLine("OK! (TEST-0010-02)");
		}
	}
}
