using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions
{
	public static class SACommon
	{
		/// <summary>
		/// フィールド上の座標(ドット単位)からマップ上の座標(テーブルのインデックス)を取得する。
		/// </summary>
		/// <param name="pt">フィールド上の座標(ドット単位)</param>
		/// <returns>マップ上の座標(テーブルのインデックス)</returns>
		public static I2Point ToTablePoint(D2Point pt)
		{
			return new I2Point(
				(int)Math.Floor(pt.X / SAConsts.TILE_W),
				(int)Math.Floor(pt.Y / SAConsts.TILE_H)
				);
		}

		/// <summary>
		/// マップ上の座標(テーブルのインデックス)からフィールド上の座標(ドット単位)を取得する。
		/// 戻り値はタイルの中心座標である。
		/// </summary>
		/// <param name="pt">マップ上の座標(テーブルのインデックス)</param>
		/// <returnsフィールド上の座標(ドット単位)</returns>
		public static D2Point ToFieldPoint(I2Point pt)
		{
			return new D2Point(
				(double)(pt.X * SAConsts.TILE_W + SAConsts.TILE_W / 2.0),
				(double)(pt.Y * SAConsts.TILE_H + SAConsts.TILE_H / 2.0)
				);
		}

		/// <summary>
		/// マップ上のＸ座標(ドット単位)からマップセルの中心Ｘ座標(ドット単位)を取得する。
		/// </summary>
		/// <param name="x">マップ上のＸ座標(ドット単位)</param>
		/// <returns>マップセルの中心Ｘ座標(ドット単位)</returns>
		public static double ToTileCenterX(double x)
		{
			return ToTileCenter(new D2Point(x, 0.0)).X;
		}

		/// <summary>
		/// マップ上のＹ座標(ドット単位)からマップセルの中心Ｙ座標(ドット単位)を取得する。
		/// </summary>
		/// <param name="y">マップ上のＹ座標(ドット単位)</param>
		/// <returns>マップセルの中心Ｙ座標(ドット単位)</returns>
		public static double ToTileCenterY(double y)
		{
			return ToTileCenter(new D2Point(0.0, y)).Y;
		}

		/// <summary>
		/// マップ上の座標(ドット単位)からマップセルの中心座標(ドット単位)を取得する。
		/// </summary>
		/// <param name="pt">マップ上の座標(ドット単位)</param>
		/// <returns>マップセルの中心座標(ドット単位)</returns>
		public static D2Point ToTileCenter(D2Point pt)
		{
			return ToFieldPoint(ToTablePoint(pt));
		}

		public static bool IsOut(D2Point pt, D4Rect rect, double margin = 0.0)
		{
			return !Crash.IsCrashed_Circle_Rect(pt, margin, rect);
		}

		public static bool IsOutOfCamera(D2Point pt, double margin = 0.0)
		{
			return IsOut(pt, new I4Rect(SAGame.I.Camera.X, SAGame.I.Camera.Y, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect(), margin);
		}
	}
}
