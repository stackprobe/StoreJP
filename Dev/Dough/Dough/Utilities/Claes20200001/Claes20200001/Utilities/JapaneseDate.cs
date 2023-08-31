using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Utilities
{
	public class JapaneseDate
	{
		/// <summary>
		/// (西暦)年月日
		/// 最小の日付 == 1/1/1
		/// 最大の日付 == 214748/12/31
		/// </summary>
		private int YMD;

		/// <summary>
		/// 日付からインスタンスを生成する。
		/// </summary>
		/// <param name="ymd">日付</param>
		public JapaneseDate(int ymd)
			: this(SimpleDateTime.FromTimeStamp(ymd * 1000000L))
		{ }

		/// <summary>
		/// 日時からインスタンスを生成する。
		/// 時刻は捨てられる。
		/// </summary>
		/// <param name="ymd">日時</param>
		public JapaneseDate(SimpleDateTime dateTime)
		{
			this.YMD = (int)Math.Min(2147481231, dateTime.ToTimeStamp() / 1000000L);
		}

		/// <summary>
		/// (西暦)年月日を取得する。
		/// </summary>
		/// <returns>(西暦)年月日</returns>
		public int GetYMD()
		{
			return this.YMD;
		}

		/// <summary>
		/// (西暦)年
		/// </summary>
		public int Y
		{
			get
			{
				return this.YMD / 10000;
			}
		}

		/// <summary>
		/// 月
		/// </summary>
		public int M
		{
			get
			{
				return (this.YMD / 100) % 100;
			}
		}

		/// <summary>
		/// 日
		/// </summary>
		public int D
		{
			get
			{
				return this.YMD % 100;
			}
		}

		#region Eras

		private class EraInfo
		{
			public int FirstYMD;
			public string Name;

			public EraInfo(int firstYMD, string name)
			{
				this.FirstYMD = firstYMD;
				this.Name = name;
			}
		}

		private static EraInfo[] Eras = new EraInfo[]
		{
			new EraInfo(10101, "西暦"),
			new EraInfo(18680101, "明治"),
			new EraInfo(19120730, "大正"),
			new EraInfo(19261225, "昭和"),
			new EraInfo(19890108, "平成"),
			new EraInfo(20190501, "令和"),
		};

		#endregion

		private class JYearInfo
		{
			public string Gengou;
			public string Year;
		}

		private JYearInfo JYear = null;

		private JYearInfo GetJYear()
		{
			if (this.JYear == null)
				this.JYear = this.GetJYear_Main();

			return this.JYear;
		}

		private JYearInfo GetJYear_Main()
		{
			int eraIndex = SCommon.GetRange(Eras, v => v.FirstYMD - this.YMD)[1] - 1;

			if (eraIndex == -1)
				throw null; // never

			EraInfo era = Eras[eraIndex];
			int y = this.Y - era.FirstYMD / 10000 + 1;
			string sy = y == 1 ? "元" : "" + y;

			return new JYearInfo()
			{
				Gengou = era.Name,
				Year = sy,
			};
		}

		/// <summary>
		/// 元号
		/// </summary>
		public string Gengou
		{
			get
			{
				return this.GetJYear().Gengou;
			}
		}

		/// <summary>
		/// 和暦・年
		/// </summary>
		public string Nen
		{
			get
			{
				return this.GetJYear().Year;
			}
		}

		/// <summary>
		/// 和暦の文字列表現を返す。
		/// 但し年月日は全角
		/// 例：令和元年５月２５日
		/// </summary>
		/// <returns>和暦の文字列表現</returns>
		public override string ToString()
		{
			return HanDigToZenDig(string.Format("{0}{1}年{2}月{3}日", this.Gengou, this.Nen, this.M, this.D));
		}

		// ====
		// ここまで定番機能
		// ====

		/// <summary>
		/// 日付(和暦)文字列からインスタンスを生成する。
		/// </summary>
		/// <param name="str">日付(和暦)文字列</param>
		/// <returns>インスタンス</returns>
		public static JapaneseDate Create(string str)
		{
			if (string.IsNullOrEmpty(str))
				throw new ArgumentException("和暦変換エラー：空の日付");

			str = ZenDigToHanDig(str);
			str = str.Replace("元", "" + 1);

			EraInfo era = Eras.FirstOrDefault(v => str.Contains(v.Name));

			if (era == null)
				throw new ArgumentException("和暦変換エラー：不明な元号");

			string[] symd = SCommon.Tokenize(str, SCommon.DECIMAL, true, true);

			if (symd.Length != 3)
				throw new ArgumentException("和暦変換エラー：不明な年月日");

			if (symd.Any(v => 9 < v.Length))
				throw new ArgumentException("和暦変換エラー：不正な年月日");

			int[] ymd = symd.Select(v => int.Parse(v)).ToArray();

			int y = ymd[0];
			int m = ymd[1];
			int d = ymd[2];

			return new JapaneseDate((era.FirstYMD / 10000 - 1 + y) * 10000 + m * 100 + d);
		}

		private static char[] HAN_DIG = SCommon.DECIMAL.ToArray();
		private static char[] ZEN_DIG = SCommon.MBC_DECIMAL.ToArray();

		private static string HanDigToZenDig(string str)
		{
			return P_ReplaceChars(str, HAN_DIG, ZEN_DIG);
		}

		private static string ZenDigToHanDig(string str)
		{
			return P_ReplaceChars(str, ZEN_DIG, HAN_DIG);
		}

		private static string P_ReplaceChars(string str, char[] srcChrs, char[] destChrs)
		{
			StringBuilder buff = new StringBuilder(str.Length);

			foreach (char f_chr in str)
			{
				char chr = f_chr;

				for (int index = 0; index < srcChrs.Length; index++)
					if (chr == srcChrs[index])
						chr = destChrs[index];

				buff.Append(chr);
			}
			return buff.ToString();
		}
	}
}
