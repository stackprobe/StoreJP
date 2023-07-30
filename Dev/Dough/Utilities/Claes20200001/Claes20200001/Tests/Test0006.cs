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
	/// JapaneseDate テスト
	/// </summary>
	public class Test0006
	{
		public void Test01()
		{
			string file = SCommon.NextOutputPath() + ".txt";

			using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
			{
				for (int y = 1; y <= 9999; y++)
				{
					if (y % 100 == 0) Console.WriteLine(string.Join(", ", "TEST-0006-01", y)); // cout

					for (int m = 1; m <= 12; m++)
					{
						int daysOfMonth = DateTime.DaysInMonth(y, m);

						for (int d = 1; d <= daysOfMonth; d++)
						{
							JapaneseDate date = new JapaneseDate(y * 10000 + m * 100 + d);

							writer.WriteLine(string.Format("{0:D4}/{1:D2}/{2:D2} ⇒ {3}"
								, y
								, m
								, d
								, date.ToString()));
						}
					}
				}
			}
			Console.WriteLine("OK!");
		}

		public void Test02()
		{
			SCommon.ToThrowPrint(() => Test02_a(int.MinValue));
			SCommon.ToThrowPrint(() => Test02_a(int.MinValue + 1));
			SCommon.ToThrowPrint(() => Test02_a(int.MinValue + 2));
			SCommon.ToThrowPrint(() => Test02_a(int.MinValue + 3));
			SCommon.ToThrowPrint(() => Test02_a(-3));
			SCommon.ToThrowPrint(() => Test02_a(-2));
			SCommon.ToThrowPrint(() => Test02_a(-1));
			SCommon.ToThrowPrint(() => Test02_a(0));
			SCommon.ToThrowPrint(() => Test02_a(1));
			SCommon.ToThrowPrint(() => Test02_a(2));
			SCommon.ToThrowPrint(() => Test02_a(3));
			SCommon.ToThrowPrint(() => Test02_a(10097));
			SCommon.ToThrowPrint(() => Test02_a(10098));
			SCommon.ToThrowPrint(() => Test02_a(10099));
			SCommon.ToThrowPrint(() => Test02_a(10100));
			SCommon.ToThrowPrint(() => Test02_a((int.MaxValue / 10000) * 10000 + 1232));
			SCommon.ToThrowPrint(() => Test02_a((int.MaxValue / 10000) * 10000 + 1233));
			SCommon.ToThrowPrint(() => Test02_a((int.MaxValue / 10000) * 10000 + 1234));
			SCommon.ToThrowPrint(() => Test02_a((int.MaxValue / 10000) * 10000 + 1235));
			SCommon.ToThrowPrint(() => Test02_a(int.MaxValue - 3));
			SCommon.ToThrowPrint(() => Test02_a(int.MaxValue - 2));
			SCommon.ToThrowPrint(() => Test02_a(int.MaxValue - 1));
			SCommon.ToThrowPrint(() => Test02_a(int.MaxValue));

			// ----

			Test02_a(10101);
			Test02_a(10102);
			Test02_a(10103);
			Test02_a(10104);
			Test02_a((int.MaxValue / 10000) * 10000 + 1228);
			Test02_a((int.MaxValue / 10000) * 10000 + 1229);
			Test02_a((int.MaxValue / 10000) * 10000 + 1230);
			Test02_a((int.MaxValue / 10000) * 10000 + 1231);

			// ----

			Console.WriteLine("OK! (TEST-0006-02)");
		}

		private void Test02_a(int ymd)
		{
			Console.WriteLine(new JapaneseDate(ymd));

			// ----

			{
				JapaneseDate date = new JapaneseDate(ymd);
				string str = date.ToString();
				JapaneseDate date2 = JapaneseDate.Create(str);

				if (date2.GetYMD() != ymd)
					throw new Exception("日付不一致");
			}
		}

		public void Test03()
		{
			for (int y = 1; y <= 9999; y++)
			{
				if (y % 100 == 0) Console.WriteLine(string.Join(", ", "TEST-0006-03", y)); // cout

				for (int m = 1; m <= 12; m++)
				{
					int daysOfMonth = DateTime.DaysInMonth(y, m);

					for (int d = 1; d <= daysOfMonth; d++)
					{
						JapaneseDate date = new JapaneseDate(y * 10000 + m * 100 + d);
						string str = date.ToString();
						JapaneseDate date2 = JapaneseDate.Create(str);

						if (
							date2.Y != y ||
							date2.M != m ||
							date2.D != d
							)
							throw null; // bug !!!
					}
				}
			}
			Console.WriteLine("OK!");
		}
	}
}
