using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// RandomUnit テスト
	/// </summary>
	public class Test0003
	{
		public void Test01()
		{
			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				if (testcnt % 100 == 0) Console.WriteLine("TEST-0003-01, " + testcnt); // cout

				bool[] table = SCommon.Generate(SCommon.CRandom.GetRange(1, 300), () => SCommon.CRandom.GetBoolean()).ToArray();
				RandomUnit ru = new RandomUnit_01() { Table = table };

				for (int c = 0; c < 100; c++)
				{
					foreach (bool flag in table)
						if (ru.GetBoolean() != flag)
							throw null;

					foreach (bool flag in table)
						if (ru.GetSign() != (flag ? 1 : -1))
							throw null;
				}
			}
			Console.WriteLine("OK!");
		}

		private class RandomUnit_01 : RandomUnit
		{
			public bool[] Table;
			private int RdIndex = 0;

			protected override byte[] GetBlock()
			{
				return this.NextBytes();
			}

			public byte[] NextBytes()
			{
				int size = SCommon.CRandom.GetRange(1, 100);
				byte[] data = new byte[size];

				for (int c = 0; c < size; c++)
					data[c] = this.NextByte();

				return data;
			}

			private byte NextByte()
			{
				int value = 0;

				for (int c = 0; c < 8; c++)
					if (this.NextBit())
						value |= 1 << c;

				return (byte)value;
			}

			private bool NextBit()
			{
				return this.Table[this.RdIndex++ % this.Table.Length];
			}
		}

		public void Test02()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0003-02, " + testcnt); // cout

				uint[] table = SCommon.Generate(SCommon.CRandom.GetRange(1, 1000), () => SCommon.CRandom.GetUInt32()).ToArray();
				RandomUnit ru = new RandomUnit_02() { Table = table };

				//for (int c = 0; c < 100; c++)
				foreach (uint value in table)
					if (ru.GetUInt32() != value)
						throw null;
			}
			Console.WriteLine("OK!");
		}

		private class RandomUnit_02 : RandomUnit
		{
			public uint[] Table;

			protected override byte[] GetBlock()
			{
				return SCommon.Join(this.Table.Select(value => new byte[]
				{
					(byte)(value & 0xff),
					(byte)((value >> 8) & 0xff),
					(byte)((value >> 16) & 0xff),
					(byte)((value >> 24) & 0xff),
				})
				.ToArray());
			}
		}

		public void Test03()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0003-03, " + testcnt); // cout

				ulong[] table = SCommon.Generate(SCommon.CRandom.GetRange(1, 1000), () => SCommon.CRandom.GetULong64()).ToArray();
				RandomUnit ru = new RandomUnit_03() { Table = table };

				//for (int c = 0; c < 100; c++)
				foreach (ulong value in table)
					if (ru.GetULong64() != value)
						throw null;
			}
			Console.WriteLine("OK!");
		}

		private class RandomUnit_03 : RandomUnit
		{
			public ulong[] Table;

			protected override byte[] GetBlock()
			{
				return SCommon.Join(this.Table.Select(value => new byte[]
				{
					(byte)(value & 0xff),
					(byte)((value >> 8) & 0xff),
					(byte)((value >> 16) & 0xff),
					(byte)((value >> 24) & 0xff),
					(byte)((value >> 32) & 0xff),
					(byte)((value >> 40) & 0xff),
					(byte)((value >> 48) & 0xff),
					(byte)((value >> 56) & 0xff),
				})
				.ToArray());
			}
		}

		public void Test04()
		{
			for (int c = 0; c < 100; c++)
			{
				Console.WriteLine(SCommon.CRandom.GetRate().ToString("F20"));
			}
			Console.WriteLine("done! (TEST-0003-04)");
		}

		public void Test05()
		{
			Test05_a(3.0, 7.0);
			Test05_a(-3.0, 7.0);
			Test05_a(-3.0, 3.0);
			Test05_a(-7.0, 3.0);
			Test05_a(-7.0, -3.0);

			Console.WriteLine("done! (TEST-0003-05)");
		}

		private void Test05_a(double minval, double maxval)
		{
			List<double> values = new List<double>();

			for (int c = 0; c < 100; c++)
			{
				values.Add(SCommon.CRandom.GetDoubleRange(minval, maxval));
			}
			values.Sort(SCommon.Comp);

			File.WriteAllLines(
				Path.Combine(SCommon.GetOutputDir(), string.Format("({0})-({1}).txt", minval, maxval)),
				values.Select(value => value.ToString()),
				Encoding.ASCII
				);
		}

		public void Test06()
		{
			RandomUnit ru = new RandomUnit_06();

			if (0x000000a5U != ru.GetUInt8()) throw null;
			if (0x0000a5a5U != ru.GetUInt16()) throw null;
			if (0x00a5a5a5U != ru.GetUInt24()) throw null;
			if (0xa5a5a5a5U != ru.GetUInt32()) throw null;

			if (0x000000a5a5a5a5a5UL != ru.GetULong40()) throw null;
			if (0x0000a5a5a5a5a5a5UL != ru.GetULong48()) throw null;
			if (0x00a5a5a5a5a5a5a5UL != ru.GetULong56()) throw null;
			if (0xa5a5a5a5a5a5a5a5UL != ru.GetULong64()) throw null;

			Console.WriteLine("OK! (TEST-0003-06)");
		}

		private class RandomUnit_06 : RandomUnit
		{
			protected override byte[] GetBlock()
			{
				return new byte[] { 0xa5 };
			}
		}

		public void Test07()
		{
			Test07_a(100, 900);
			Test07_a(10000, 90000);
			Test07_a(1000000, 9000000);
			Test07_a(100000000, 900000000);
			Test07_a(10000000000, 90000000000);
			Test07_a(1000000000000, 9000000000000);
			Test07_a(100000000000000, 900000000000000);
			Test07_a(10000000000000000, 90000000000000000);
			Test07_a(1000000000000000000, 9000000000000000000);

			Test07_a(-900, -100);
			Test07_a(-90000, -10000);
			Test07_a(-9000000, -1000000);
			Test07_a(-900000000, -100000000);
			Test07_a(-90000000000, -10000000000);
			Test07_a(-9000000000000, -1000000000000);
			Test07_a(-900000000000000, -100000000000000);
			Test07_a(-90000000000000000, -10000000000000000);
			Test07_a(-9000000000000000000, -1000000000000000000);

			Console.WriteLine("OK! (TEST-0003-07)");
		}

		private void Test07_a(long minval, long maxval)
		{
			for (int c = 0; c < 1000000; c++)
			{
				long value = SCommon.CRandom.GetLongRange(minval, maxval);

				if (value < minval)
					throw null;

				if (value > maxval)
					throw null;

				// 最初のうちちょっとだけ表示する。
				if (c < 10)
					Console.WriteLine(minval + ", " + maxval + " ==> " + value); // cout
			}
			Console.WriteLine("OK");
		}
	}
}
