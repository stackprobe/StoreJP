using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.GetIndex テスト
	/// </summary>
	public class Test0008
	{
		public void Test01()
		{
			for (int a = 0; a < 10; a++)
			{
				for (int b = 0; b <= 1; b++)
				{
					for (int c = 0; c < 10; c++)
					{
						string str = new string('A', a) + new string('B', b) + new string('C', c);
						char target = 'B';
						int expect = b == 1 ? a : -1;

						Test01_a(str, target, expect);
					}
				}
			}
			Console.WriteLine("OK!");
		}

		private void Test01_a(string str, char target, int expect)
		{
			int ret = SCommon.GetIndex(str.ToCharArray(), target, (a, b) => (int)a - (int)b);

			Console.WriteLine(string.Join(", ", "TEST-0008-01", str, target, expect, ret)); // cout

			if (ret != expect)
				throw null;

			Console.WriteLine("OK");
		}

		public void Test02()
		{
			Test02_a(1000000, 2000000, 300000, 300000);
			Test02_a(1000000, 2000000, 100000, 100000);
			Test02_a(1000000, 2000000, 30000, 30000);
			Test02_a(1000000, 2000000, 10000, 10000);
			Test02_a(1000000, 2000000, 3000, 3000);
			Test02_a(1000000, 2000000, 1000, 1000);
			Test02_a(1000000, 2000000, 300, 300);
			Test02_a(1000000, 2000000, 100, 100);
			Test02_a(1000000, 2000000, 30, 30);
			Test02_a(1000000, 2000000, 10, 10);

			Console.WriteLine("OK!");
		}

		private void Test02_a(int minValue, int maxValue, int valueStepScale, int testCount)
		{
			Console.WriteLine(string.Join(", ", "TEST-0008-02", minValue, maxValue, valueStepScale, testCount)); // cout

			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				List<int> list = new List<int>();

				for (
					int value = SCommon.CRandom.GetRange(minValue, minValue + valueStepScale);
					value <= maxValue;
					value += SCommon.CRandom.GetRange(1, valueStepScale)
					)
					list.Add(value);

				int expect = SCommon.CRandom.GetInt(list.Count);
				int target = list[expect];

				// ----

				int ret = SCommon.GetIndex(list, target, (a, b) => a - b);

				if (ret != expect)
					throw null;
			}
			Console.WriteLine("OK");
		}

		public void Test03()
		{
			Test03_a(1000000, 2000000, 300000, 300000);
			Test03_a(1000000, 2000000, 100000, 100000);
			Test03_a(1000000, 2000000, 30000, 30000);
			Test03_a(1000000, 2000000, 10000, 10000);
			Test03_a(1000000, 2000000, 3000, 3000);
			Test03_a(1000000, 2000000, 1000, 1000);
			Test03_a(1000000, 2000000, 300, 300);
			Test03_a(1000000, 2000000, 100, 100);
			Test03_a(1000000, 2000000, 30, 30);
			Test03_a(1000000, 2000000, 10, 10);

			Console.WriteLine("OK!");
		}

		private void Test03_a(int minValue, int maxValue, int valueStepScale, int testCount)
		{
			Console.WriteLine(string.Join(", ", "TEST-0008-03", minValue, maxValue, valueStepScale, testCount)); // cout

			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				List<int> list = new List<int>();

				for (
					int value = SCommon.CRandom.GetRange(minValue, minValue + valueStepScale);
					value <= maxValue;
					value += SCommon.CRandom.GetRange(1, valueStepScale)
					)
					list.Add(value);

				int target = SCommon.DesertElement(list, SCommon.CRandom.GetInt(list.Count));
				int expect = -1;

				// ----

				int ret = SCommon.GetIndex(list, target, (a, b) => a - b);

				if (ret != expect)
					throw null;
			}
			Console.WriteLine("OK");
		}
	}
}
