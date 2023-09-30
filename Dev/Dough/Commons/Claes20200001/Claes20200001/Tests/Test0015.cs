using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.Compress, SCommon.Decompress, SCommon.CompressFile, SCommon.DecompressFile テスト
	/// </summary>
	public class Test0015
	{
		public void Test01()
		{
			for (int c = 0; c <= 200; c++)
			{
				byte[] data = Enumerable.Repeat((byte)'A', c).ToArray();
				byte[] encData = SCommon.Compress(data);

				if (encData == null)
					throw null;

				if (c % 20 == 0) Console.WriteLine(string.Join(", ", "TEST-0015-01", c, data.Length, encData.Length));

				{
					byte[] decData = SCommon.Decompress(encData);

					if (decData == null)
						throw null;

					if (SCommon.Comp(data, decData) != 0) // ? 不一致
						throw null;
				}

				foreach (int limit in new int[] { 0, 30, 70, 100, 150, 200 })
				{
					if (c <= limit)
					{
						byte[] decData = SCommon.Decompress(encData, limit);

						if (decData == null)
							throw null;

						if (SCommon.Comp(data, decData) != 0) // ? 不一致
							throw null;
					}
					else
					{
						SCommon.ToThrow(() => SCommon.Decompress(encData, limit));
					}
				}
			}
			Console.WriteLine("OK!");
		}

		public void Test02()
		{
			for (int c = 0; c <= 200; c++)
			{
				byte[] data = Enumerable.Repeat((byte)'A', c).ToArray();
				byte[] encData = Test02_CompressByFile(data);

				if (c % 20 == 0) Console.WriteLine(string.Join(", ", "TEST-0015-02", c, data.Length, encData.Length));

				{
					byte[] decData = Test02_DecompressByFile(encData);

					if (SCommon.Comp(data, decData) != 0) // ? 不一致
						throw null;
				}

				foreach (long limit in new int[] { 0, 30, 70, 100, 150, 200 })
				{
					if (c <= limit)
					{
						byte[] decData = Test02_DecompressByFileLimit(encData, limit);

						if (SCommon.Comp(data, decData) != 0) // ? 不一致
							throw null;
					}
					else
					{
						Test02_DecompressByFileLimitThrow(encData, limit);
					}
				}
			}
			Console.WriteLine("OK!");
		}

		private byte[] Test02_CompressByFile(byte[] data)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string rFile = wd.MakePath();
				string wFile = wd.MakePath();

				File.WriteAllBytes(rFile, data);

				SCommon.CompressFile(rFile, wFile);

				if (!File.Exists(wFile))
					throw null;

				return File.ReadAllBytes(wFile);
			}
		}

		private byte[] Test02_DecompressByFile(byte[] data)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string rFile = wd.MakePath();
				string wFile = wd.MakePath();

				File.WriteAllBytes(rFile, data);

				SCommon.DecompressFile(rFile, wFile);

				if (!File.Exists(wFile))
					throw null;

				return File.ReadAllBytes(wFile);
			}
		}

		private byte[] Test02_DecompressByFileLimit(byte[] data, long limit)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string rFile = wd.MakePath();
				string wFile = wd.MakePath();

				File.WriteAllBytes(rFile, data);

				SCommon.DecompressFile(rFile, wFile, limit);

				if (!File.Exists(wFile))
					throw null;

				return File.ReadAllBytes(wFile);
			}
		}

		private void Test02_DecompressByFileLimitThrow(byte[] data, long limit)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string rFile = wd.MakePath();
				string wFile = wd.MakePath();

				File.WriteAllBytes(rFile, data);

				SCommon.ToThrow(() => SCommon.DecompressFile(rFile, wFile, limit));
			}
		}
	}
}
