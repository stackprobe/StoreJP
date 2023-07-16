using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Dungeons.Fields
{
	// memo:
	// 慣習的な呼び方：
	// マップデータのセルの集合として(将棋盤のように)見た場合
	// -- 全体を「マップ」または「テーブル」、個々のマス目を「タイル」と呼ぶ。

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
	public abstract class DUField
	{
		// マップ(テーブル)の大きさ
		//
		public readonly int Table_W;
		public readonly int Table_H;

		public DUField(I2Size tableSize)
		{
			if (tableSize.W < 1)
				throw new Exception("Bad tableSize.W");

			if (tableSize.H < 1)
				throw new Exception("Bad tableSize.H");

			this.Table_W = tableSize.W;
			this.Table_H = tableSize.H;
		}

		/// <summary>
		/// 初期化する。
		/// するべきこと：
		/// -- プレイヤー位置の設定
		/// -- プレイヤー方向の設定
		/// -- 音楽の再生
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// マップ内の指定タイルの指定方向の壁を取得する。
		/// タイルの位置はマップ内・マップ外どちらもあり得る。
		/// see: P_GetWall
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="direction">方向(4方向_テンキー方式)</param>
		/// <returns>壁の状態</returns>
		public int GetWall(I2Point tilePt, int direction)
		{
			// ? マップ外
			if (
				tilePt.X < 0 || this.Table_W <= tilePt.X ||
				tilePt.Y < 0 || this.Table_H <= tilePt.Y
				)
				return 0;

			return this.P_GetWall(tilePt, direction);
		}

		/// <summary>
		/// マップ内の指定タイルの指定方向の壁を取得する。
		/// タイルの位置は常にマップ内である。
		/// 壁の状態：
		/// -- 0 == 空間
		/// -- 1 == 壁
		/// -- 2 == ゲート
		/// </summary>
		/// <param name="tilePt">タイルの位置</param>
		/// <param name="direction">方向(4方向_テンキー方式)</param>
		/// <returns>壁の状態</returns>
		protected abstract int P_GetWall(I2Point tilePt, int direction);

		/// <summary>
		/// ダンジョンの背景の画像を返す。
		/// 描画先スクリーンのサイズ：
		/// -- DUDungeonScreen.Layout.SCREEN_W
		/// -- DUDungeonScreen.Layout.SCREEN_H
		/// </summary>
		/// <returns>ダンジョンの背景の画像</returns>
		public abstract Picture GetBackgroundPicture();

		/// <summary>
		/// ダンジョンの壁の画像を返す。
		/// 推奨画像サイズ == 350 x 250
		/// </summary>
		/// <returns>ダンジョンの壁の画像</returns>
		public abstract Picture GetWallPicture();

		/// <summary>
		/// ダンジョンのゲートの画像を返す。
		/// 推奨画像サイズ == 350 x 250
		/// </summary>
		/// <returns>ダンジョンのゲートの画像</returns>
		public abstract Picture GetGatePicture();
	}
}
