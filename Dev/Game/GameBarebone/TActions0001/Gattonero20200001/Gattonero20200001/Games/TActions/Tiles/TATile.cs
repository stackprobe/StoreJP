using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;

namespace Charlotte.Games.TActions.Tiles
{
	/// <summary>
	/// タイル
	/// </summary>
	public abstract class TATile
	{
		/// <summary>
		/// タイルの種別
		/// </summary>
		public enum Kind_e
		{
			/// <summary>
			/// 地面・通路
			/// 通り抜け：
			/// -- 自機：可
			/// -- 自弾：可
			/// </summary>
			GROUND = 1,

			/// <summary>
			/// 川・水域
			/// 通り抜け：
			/// -- 自機：不可
			/// -- 自弾：可
			/// </summary>
			RIVER,

			/// <summary>
			/// 壁
			/// 通り抜け：
			/// -- 自機：不可
			/// -- 自弾：不可
			/// </summary>
			WALL,
		}

		/// <summary>
		/// このタイルの種別を返す。
		/// </summary>
		/// <returns>タイルの種別</returns>
		public abstract Kind_e GetKind();

		/// <summary>
		/// 地面・通路か判定する。
		/// </summary>
		/// <returns>地面・通路か</returns>
		public bool IsGround()
		{
			return this.GetKind() == Kind_e.GROUND;
		}

		/// <summary>
		/// 川・水域か判定する。
		/// </summary>
		/// <returns>川・水域か</returns>
		public bool IsRiver()
		{
			return this.GetKind() == Kind_e.RIVER;
		}

		/// <summary>
		/// 壁か判定する。
		/// </summary>
		/// <returns>壁か</returns>
		public bool IsWall()
		{
			return this.GetKind() == Kind_e.WALL;
		}

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
