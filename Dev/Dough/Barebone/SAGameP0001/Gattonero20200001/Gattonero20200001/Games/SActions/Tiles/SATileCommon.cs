using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Tiles
{
	public static class SATileCommon
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
	}
}
