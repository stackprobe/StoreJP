using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte
{
	public static class Common
	{
		public static bool IsMbcKana(char chr)
		{
			return 0x30a1 <= (int)chr && (int)chr <= 0x30fc; // ? カタカナ(30A1～30FA) || 中点(30FB) || 長音記号(30FC)
		}

		private static HashSet<char> _mbcChars = null;

		public static bool IsMbcChar(char chr)
		{
			if (_mbcChars == null)
				_mbcChars = new HashSet<char>(SCommon.GetJChars().ToArray());

			return _mbcChars.Contains(chr);
		}

		/// <summary>
		/// アスペクト比を維持して指定サイズを指定領域いっぱいに広げる。
		/// 戻り値：
		/// -- new D4Rect[] { interior, exterior }
		/// ---- interior == 指定領域の内側に張り付く拡大領域
		/// ---- exterior == 指定領域の外側に張り付く拡大領域
		/// </summary>
		/// <param name="size">指定サイズ</param>
		/// <param name="rect">指定領域</param>
		/// <returns>拡大領域の配列</returns>
		public static D4Rect[] EnlargeFull(D2Size size, D4Rect rect)
		{
			double w_h = (rect.H * size.W) / size.H; // 高さを基準にした幅
			double h_w = (rect.W * size.H) / size.W; // 幅を基準にした高さ

			D4Rect rect1;
			D4Rect rect2;

			rect1.L = rect.L + (rect.W - w_h) / 2.0;
			rect1.T = rect.T;
			rect1.W = w_h;
			rect1.H = rect.H;

			rect2.L = rect.L;
			rect2.T = rect.T + (rect.H - h_w) / 2.0;
			rect2.W = rect.W;
			rect2.H = h_w;

			D4Rect interior;
			D4Rect exterior;

			if (w_h < rect.W)
			{
				interior = rect1;
				exterior = rect2;
			}
			else
			{
				interior = rect2;
				exterior = rect1;
			}
			return new D4Rect[] { interior, exterior };
		}
	}
}
