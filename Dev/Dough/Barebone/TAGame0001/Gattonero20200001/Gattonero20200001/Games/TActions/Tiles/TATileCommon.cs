using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Tiles
{
	public static class TATileCommon
	{
		/// <summary>
		/// 汎用・編集モード向け描画
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		/// <param name="tilePicture">タイル画像</param>
		public static void DrawSimply(I2Point tilePt, D2Point drawPt, Picture tilePicture)
		{
			DD.Draw(tilePicture, drawPt);
		}

		/// <summary>
		/// 汎用・編集モード向け描画
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// 表示テキスト：
		/// -- 全角文字1文字または半角文字2文字を想定する。
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		/// <param name="text">表示テキスト</param>
		/// <param name="textColor">表示テキスト色</param>
		/// <param name="textBorderColor">表示テキストの縁の色</param>
		/// <param name="backColor">タイル(文字の背景)の色</param>
		public static void DrawSimply(I2Point tilePt, D2Point drawPt, string text, I3Color textColor, I3Color textBorderColor, I3Color backColor)
		{
			DD.SetBright(backColor.ToD3Color());
			DD.Draw(Pictures.WhiteBox, D4Rect.XYWH(drawPt.X, drawPt.Y, TAConsts.TILE_W, TAConsts.TILE_H));

			DD.SetPrint((int)drawPt.X - 8, (int)drawPt.Y - 8, 0);
			DD.SetPrintColor(textColor);
			DD.SetPrintBorder(textBorderColor, 1);
			DD.Print(text);
		}
	}
}
