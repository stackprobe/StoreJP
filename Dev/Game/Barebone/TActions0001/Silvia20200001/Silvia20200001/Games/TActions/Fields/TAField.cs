using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Fields
{
	// memo:
	// 慣習的な呼び方：
	// マップデータのセルの集合として(将棋盤のように)見た場合
	// -- 全体を「マップ」または「テーブル」、個々のマス目を「タイル」と呼ぶ。
	// ドット単位の座標で見たとき
	// -- 全体を「フィールド」と呼ぶ。

	// memo:
	// 用語集：
	// 4方向_テンキー方式      -- { 2, 4, 6, 8 } の値をとり、それぞれ { 下, 左, 右, 上 } を意味する。
	// 4方向+0vec_テンキー方式 -- { 2, 4, 5, 6, 8 } の値をとり、それぞれ { 下, 左, 0vec(零ベクトル), 右, 上 } を意味する。
	// 8方向_テンキー方式      -- { 1, 2, 3, 4, 6, 7, 8, 9 } の値をとり、それぞれ { 左下, 下, 右下, 左, 右, 左上, 上, 右上 } を意味する。
	// 8方向+0vec_テンキー方式 -- { 1, 2, 3, 4, 5, 6, 7, 8, 9 } の値をとり、それぞれ { 左下, 下, 右下, 左, 0vec(零ベクトル), 右, 左上, 上, 右上 } を意味する。

	/// <summary>
	/// フィールド
	/// マップ情報
	/// </summary>
	public abstract class TAField
	{
		// マップ(テーブル)の大きさ
		//
		public readonly int Table_W;
		public readonly int Table_H;

		// フィールドの大きさ(ドット単位)
		//
		public int W { get { return this.Table_W * TAConsts.TILE_W; } }
		public int H { get { return this.Table_H * TAConsts.TILE_H; } }

		public TAField(I2Size tableSize)
		{
			int TABLE_W_MIN = (GameConfig.ScreenSize.W + TAConsts.TILE_W - 1) / TAConsts.TILE_W;
			int TABLE_H_MIN = (GameConfig.ScreenSize.H + TAConsts.TILE_H - 1) / TAConsts.TILE_H;

			if (tableSize.W < TABLE_W_MIN)
				throw new Exception("Bad tableSize.W");

			if (tableSize.H < TABLE_H_MIN)
				throw new Exception("Bad tableSize.H");

			this.Table_W = tableSize.W;
			this.Table_H = tableSize.H;
		}

		/// <summary>
		/// 初期化する。
		/// このフィールドに侵入した方向：
		/// -- TAGameMaster.I.IntoDirection
		/// するべきこと：
		/// -- プレイヤー位置の設定
		/// -- 敵の配置
		/// -- 音楽の再生
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// 隣のフィールドを得る。
		/// </summary>
		/// <param name="direction">このフィールドから見た方向(4方向_テンキー方式)</param>
		/// <returns>隣のフィールド</returns>
		public abstract TAField GetNextField(int direction);

		/// <summary>
		/// マップの外か判定する。
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>マップの外か</returns>
		public bool IsOut(I2Point tilePt)
		{
			return
				tilePt.X < 0 || this.Table_W <= tilePt.X ||
				tilePt.Y < 0 || this.Table_H <= tilePt.Y;
		}

		/// <summary>
		/// 地面か判定する。
		/// see: GetTile, P_GetTile
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>地面か</returns>
		public bool IsGround(I2Point tilePt)
		{
			return this.GetTile(tilePt) == 0;
		}

		/// <summary>
		/// 川(水辺)か判定する。
		/// see: GetTile, P_GetTile
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>川(水辺)か</returns>
		public bool IsRiver(I2Point tilePt)
		{
			return this.GetTile(tilePt) == 1;
		}

		/// <summary>
		/// 壁か判定する。
		/// see: GetTile, P_GetTile
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>壁か</returns>
		public bool IsWall(I2Point tilePt)
		{
			return this.GetTile(tilePt) == 2;
		}

		/// <summary>
		/// マップ内の指定位置のタイル種別を取得する。
		/// タイルの位置はマップ内・マップ外どちらもあり得る。
		/// see: P_GetTile
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>タイル種別</returns>
		private int GetTile(I2Point tilePt)
		{
			// ? マップ外
			if (
				tilePt.X < 0 || this.Table_W <= tilePt.X ||
				tilePt.Y < 0 || this.Table_H <= tilePt.Y
				)
				return 0;

			return this.P_GetTile(tilePt);
		}

		/// <summary>
		/// マップ内の指定位置のタイル種別を取得する。
		/// タイルの位置は常にマップ内である。
		/// 戻り値：
		/// -- 0 == 地面
		/// -- 1 == 川(水辺)
		/// -- 2 == 壁
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <returns>タイル種別</returns>
		protected abstract int P_GetTile(I2Point tilePt);

		/// <summary>
		/// マップ内の指定位置のタイルを描画する。
		/// タイルの位置はマップ内・マップ外どちらもあり得る。
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		public void DrawTile(I2Point tilePt, D2Point drawPt)
		{
			// ? マップ外
			if (
				tilePt.X < 0 || this.Table_W <= tilePt.X ||
				tilePt.Y < 0 || this.Table_H <= tilePt.Y
				)
				return;

			this.P_DrawTile(tilePt, drawPt);
		}

		/// <summary>
		/// マップ内の指定位置のタイルを描画する。
		/// タイルの位置は常にマップ内である。
		/// 描画位置：
		/// -- カメラ位置の適用済み(考慮不要)
		/// -- タイルの中心座標
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="drawPt">描画位置</param>
		protected abstract void P_DrawTile(I2Point tilePt, D2Point drawPt);

		private Func<bool> _drawWall = null;

		/// <summary>
		/// 背景を描画する。
		/// </summary>
		public void DrawWall()
		{
			if (_drawWall == null)
				_drawWall = SCommon.Supplier(this.E_DrawWall());

			if (!_drawWall())
				throw null; // never
		}

		/// <summary>
		/// 背景を描画する。
		/// </summary>
		/// <returns>タスク：常に真</returns>
		protected abstract IEnumerable<bool> E_DrawWall();
	}
}
