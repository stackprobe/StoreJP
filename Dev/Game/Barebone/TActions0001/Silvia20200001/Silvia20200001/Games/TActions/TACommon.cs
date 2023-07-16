using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public static class TACommon
	{
		/// <summary>
		/// フィールド上の座標(ドット単位)からマップ上の座標(テーブルのインデックス)を取得する。
		/// </summary>
		/// <param name="pt">フィールド上の座標(ドット単位)</param>
		/// <returns>マップ上の座標(テーブルのインデックス)</returns>
		public static I2Point ToTablePoint(D2Point pt)
		{
			return new I2Point(
				(int)Math.Floor(pt.X / TAConsts.TILE_W),
				(int)Math.Floor(pt.Y / TAConsts.TILE_H)
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
				(double)(pt.X * TAConsts.TILE_W + TAConsts.TILE_W / 2.0),
				(double)(pt.Y * TAConsts.TILE_H + TAConsts.TILE_H / 2.0)
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
			return IsOut(pt, new I4Rect(TAGame.I.Camera.X, TAGame.I.Camera.Y, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect(), margin);
		}

		/// <summary>
		/// 方向転換する。
		/// 方向：8方向+0vec_テンキー方式
		/// </summary>
		/// <param name="direction">回転前の方向</param>
		/// <param name="count">回転する回数(1回につき時計回りに45度回転する,負の値=反時計回り)</param>
		/// <returns>回転後の方向</returns>
		public static int Rotate(int direction, int count)
		{
			if (direction < 1 || 9 < direction)
				throw new Exception("Bad direction");

			if (count <= -8 || 8 <= count)
				count %= 8;

			int[] ROT_CLW = new int[] { default(int), 4, 1, 2, 7, 5, 3, 8, 9, 6 }; // 時計回り
			int[] ROT_CCW = new int[] { default(int), 2, 3, 6, 1, 5, 9, 4, 7, 8 }; // 反時計回り

			for (; 0 < count; count--)
				direction = ROT_CLW[direction];

			for (; count < 0; count++)
				direction = ROT_CCW[direction];

			return direction;
		}

		public static D2Point GetDirectionSpeed(int direction, double speed)
		{
			double nanameSpeed = speed / TAConsts.ROOT_OF_2;

			switch (direction)
			{
				case 4: return new D2Point(-speed, 0.0);
				case 6: return new D2Point(speed, 0.0);
				case 8: return new D2Point(0.0, -speed);
				case 2: return new D2Point(0.0, speed);

				case 1: return new D2Point(-nanameSpeed, nanameSpeed);
				case 3: return new D2Point(nanameSpeed, nanameSpeed);
				case 7: return new D2Point(-nanameSpeed, -nanameSpeed);
				case 9: return new D2Point(nanameSpeed, -nanameSpeed);

				default:
					throw null; // never
			}
		}

		/// <summary>
		/// 歩行画像テーブルから歩行画像を得る。
		/// 想定：点睛集積
		/// </summary>
		/// <param name="table">歩行画像テーブル</param>
		/// <param name="faceDirection">向いている方角(8方向_テンキー方式)</param>
		/// <param name="koma">コマ数(0～2)</param>
		/// <returns>歩行画像</returns>
		public static Picture GetWalkPicture(Picture[,] table, int faceDirection, int koma)
		{
			int l;
			int t;

			switch (faceDirection)
			{
				case 2: l = 0; t = 0; break; // 下
				case 4: l = 0; t = 1; break; // 左
				case 6: l = 0; t = 2; break; // 右
				case 8: l = 0; t = 3; break; // 上

				case 1: l = 3; t = 0; break; // 左下
				case 3: l = 3; t = 1; break; // 右下
				case 7: l = 3; t = 2; break; // 左上
				case 9: l = 3; t = 3; break; // 右上

				default:
					throw null; // never
			}
			return table[l + koma, t];
		}
	}
}
