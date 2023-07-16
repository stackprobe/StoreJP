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
			: this(SCommon.SimpleDateTime.FromTimeStamp(ymd * 1000000L))
		{ }

		/// <summary>
		/// 日時からインスタンスを生成する。
		/// 時刻は捨てられる。
		/// </summary>
		/// <param name="ymd">日時</param>
		public JapaneseDate(SCommon.SimpleDateTime dateTime)
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
			public char? Alphabet;

			public EraInfo(int firstYMD, string name, char? alphabet = null)
			{
				this.FirstYMD = firstYMD;
				this.Name = name;
				this.Alphabet = alphabet;
			}
		}

		private static EraInfo[] Eras = new EraInfo[]
		{
			new EraInfo(18680101, "明治", 'M'),
			new EraInfo(19120730, "大正", 'T'),
			new EraInfo(19261225, "昭和", 'S'),
			new EraInfo(19890108, "平成", 'H'),
			new EraInfo(20190501, "令和", 'R'),
		};

		#endregion

		private class JYearInfo
		{
			public string Gengou;
			public int Y;

			public string Year
			{
				get
				{
					return this.Y == 1 ? "元" : "" + this.Y;
				}
			}
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
			string gengou;
			int y;

			if (eraIndex == -1)
			{
				gengou = "西暦";
				y = this.Y;
			}
			else
			{
				EraInfo era = Eras[eraIndex];
				gengou = era.Name;
				y = this.Y - era.FirstYMD / 10000 + 1;
			}

			return new JYearInfo()
			{
				Gengou = gengou,
				Y = y,
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
		/// 和暦・年(文字列)
		/// </summary>
		public string SY
		{
			get
			{
				return this.GetJYear().Year;
			}
		}

		/// <summary>
		/// 和暦・年(整数)
		/// </summary>
		public int IY
		{
			get
			{
				return this.GetJYear().Y;
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
			return HanDigToZenDig(this.ToHalfString());
		}

		/// <summary>
		/// 和暦の文字列表現を返す。
		/// 例：令和元年5月25日
		/// </summary>
		/// <returns>和暦の文字列表現</returns>
		public string ToHalfString()
		{
			return this.ToString("{0}{1}年{3}月{4}日");
		}

		/// <summary>
		/// 和暦の文字列表現を返す。
		/// 但し年数は整数表記
		/// 例：令和1年5月25日
		/// </summary>
		/// <returns>和暦の文字列表現</returns>
		public string ToIntYString()
		{
			return this.ToString("{0}{2}年{3}月{4}日");
		}

		public string ToString(string format)
		{
			return string.Format(format, this.Gengou, this.SY, this.IY, this.M, this.D);
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

			// 正規化
			str = RemoveBlank(str);
			str = ZenDigAlpToHanDigAlp(str);
			str = str.ToUpper();

			// 元年の解消
			str = str.Replace("元", "1");

			EraInfo era = Eras
				.Concat(new EraInfo[] { new EraInfo(10101, "西暦") })
				.FirstOrDefault(v => str.Contains(v.Name) || (v.Alphabet != null && str.Contains(v.Alphabet.Value)));

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

		private static string RemoveBlank(string str)
		{
			return new string(str.Where(chr => ' ' < chr && chr != '　').ToArray());
		}

		private static char[] ZEN_DIG_ALP = (SCommon.MBC_DECIMAL + SCommon.MBC_ALPHA_UPPER + SCommon.MBC_ALPHA_LOWER).ToArray();
		private static char[] HAN_DIG_ALP = (SCommon.DECIMAL + SCommon.ALPHA_UPPER + SCommon.ALPHA_LOWER).ToArray();

		private static string ZenDigAlpToHanDigAlp(string str)
		{
			return new string(str.Select(chr =>
			{
				for (int index = 0; index < ZEN_DIG_ALP.Length; index++)
					if (chr == ZEN_DIG_ALP[index])
						return HAN_DIG_ALP[index];

				return chr;
			})
			.ToArray());
		}

		private static char[] HAN_DIG = SCommon.DECIMAL.ToArray();
		private static char[] ZEN_DIG = SCommon.MBC_DECIMAL.ToArray();

		private static string HanDigToZenDig(string str)
		{
			return new string(str.Select(chr =>
			{
				for (int index = 0; index < HAN_DIG.Length; index++)
					if (chr == HAN_DIG[index])
						return ZEN_DIG[index];

				return chr;
			})
			.ToArray());
		}
	}
}
