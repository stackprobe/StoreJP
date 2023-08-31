using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Tiles
{
	/// <summary>
	/// タイル
	/// </summary>
	public abstract class SATile
	{
		/// <summary>
		/// このタイルは壁であるか返す。
		/// </summary>
		/// <returns>壁か</returns>
		public abstract bool IsWall();

		/// <summary>
		/// このタイルを描画する。
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		public abstract void Draw(I2Point tilePt, D2Point drawPt);

		/// <summary>
		/// 編集モード向け描画
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		public abstract void DrawSimply(I2Point tilePt, D2Point drawPt);
	}
}
