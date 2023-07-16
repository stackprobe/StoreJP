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

				RandomUnit ru = new RandomUnit(new RandomNumberGenerator_01() { Table = table });

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

		private class RandomNumberGenerator_01 : RandomUnit.IRandomNumberGenerator
		{
			public bool[] Table;
			private int RdIndex = 0;

			public byte[] GetBlock()
			{
				return this.NextBytes();
			}

			private byte[] NextBytes()
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

			public void Dispose()
			{
				// noop
			}
		}

		public void Test02()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0003-02, " + testcnt); // cout

				uint[] table = SCommon.Generate(SCommon.CRandom.GetRange(1, 1000), () => SCommon.CRandom.GetUInt32()).ToArray();

				RandomUnit ru = new RandomUnit(new RandomNumberGenerator_02() { Table = table });

				//for (int c = 0; c < 100; c++)
				foreach (uint value in table)
					if (ru.GetUInt32() != value)
						throw null;
			}
			Console.WriteLine("OK!");
		}

		private class RandomNumberGenerator_02 : RandomUnit.IRandomNumberGenerator
		{
			public uint[] Table;

			public byte[] GetBlock()
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

			public void Dispose()
			{
				// noop
			}
		}

		public void Test03()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				if (testcnt % 1000 == 0) Console.WriteLine("TEST-0003-03, " + testcnt); // cout

				ulong[] table = SCommon.Generate(SCommon.CRandom.GetRange(1, 1000), () => SCommon.CRandom.GetULong64()).ToArray();

				RandomUnit ru = new RandomUnit(new RandomNumberGenerator_03() { Table = table });

				//for (int c = 0; c < 100; c++)
				foreach (ulong value in table)
					if (ru.GetULong64() != value)
						throw null;
			}
			Console.WriteLine("OK!");
		}

		private class RandomNumberGenerator_03 : RandomUnit.IRandomNumberGenerator
		{
			public ulong[] Table;

			public byte[] GetBlock()
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

			public void Dispose()
			{
				// noop
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
	}
}
