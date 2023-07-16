using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public static class TAPlayerPosition
	{
		private static Around A2;
		private static Around A3;

		public static D2Point Correct(D2Point pt)
		{
			int x = SCommon.ToInt(pt.X);
			int y = SCommon.ToInt(pt.Y);

			A2 = new Around(x, y, 2);
			A3 = new Around(x, y, 3);

			I2Point a2RelPtBk = A2.RelativePoint;

			Correct_A2();

			if (
				a2RelPtBk.X != A2.RelativePoint.X ||
				a2RelPtBk.Y != A2.RelativePoint.Y
				)
			{
				x = A2.CenterPoint.X + A2.RelativePoint.X;
				y = A2.CenterPoint.Y + A2.RelativePoint.Y;
			}
			return new D2Point((double)x, (double)y);
		}

		private static void Correct_A2()
		{
			int pattern =
				(A2.Table[0, 1] == 1 ? 1 : 0) | // 左下
				(A2.Table[1, 1] == 1 ? 2 : 0) | // 右下
				(A2.Table[0, 0] == 1 ? 4 : 0) | // 左上
				(A2.Table[1, 0] == 1 ? 8 : 0);  // 右上

			switch (pattern)
			{
				case 0: // 壁なし
					break;

				case 15: // 壁の中
					for (int x = 0; x < 3; x++)
					{
						for (int y = 0; y < 3; y++)
						{
							if (A3.Table[x, y] == 0)
							{
								A2.RelativePoint.X += (x - 1) * 10;
								A2.RelativePoint.Y += (y - 1) * 10;
								break;
							}
						}
					}
					A2.RelativePoint.X += SCommon.CRandom.GetSign() * 10;
					A2.RelativePoint.Y += SCommon.CRandom.GetSign() * 10;
					break;

				case 1: // 左下のみ
				case 4: // 左上のみ
				case 5: // 左
				case 7: // 右上のみ空き
				case 13: // 右下のみ空き
					A2.TurnX();
					Correct_A2();
					A2.RelativePoint.X *= -1;
					break;

				case 8: // 右上のみ
				case 9: // 右上と左下
				case 12: // 上
				case 14: // 左下のみ空き
					A2.TurnY();
					Correct_A2();
					A2.RelativePoint.Y *= -1;
					break;

				case 2: // 右下のみ
					if (-16 < A2.RelativePoint.X && -16 < A2.RelativePoint.Y)
					{
						const int SLIDE_BORDER = 5;
						const int SLIDE_SPEED = 1;

						if (A2.RelativePoint.X < A2.RelativePoint.Y)
						{
							A2.RelativePoint.X = -16;

							if (A2.RelativePoint.Y < SLIDE_BORDER)
								A2.RelativePoint.Y -= SLIDE_SPEED;
						}
						else
						{
							A2.RelativePoint.Y = -16;

							if (A2.RelativePoint.X < SLIDE_BORDER)
								A2.RelativePoint.X -= SLIDE_SPEED;
						}
					}
					break;

				case 10: // 右
					A2.RelativePoint.X = -16;
					break;

				case 3: // 下
					A2.RelativePoint.Y = -16;
					break;

				case 6: // 左上と右下
					if (A2.RelativePoint.X < A2.RelativePoint.Y)
					{
						A2.RelativePoint.X = -16;
						A2.RelativePoint.Y = 16;
					}
					else
					{
						A2.RelativePoint.X = 16;
						A2.RelativePoint.Y = -16;
					}
					break;

				case 11: // 左上のみ空き
					A2.RelativePoint.X = -16;
					A2.RelativePoint.Y = -16;
					break;

				default:
					throw null; // never
			}
		}

		/// <summary>
		/// 周辺テーブル
		/// </summary>
		private class Around
		{
			/// <summary>
			/// 周辺テーブルの幅・高さ
			/// </summary>
			public int Size;

			/// <summary>
			/// 周辺テーブル
			/// 周辺のマップセルを集めたテーブル
			/// 値：
			/// -- 0 == 空間
			/// -- 1 == 壁
			/// </summary>
			public int[,] Table;

			/// <summary>
			/// 周辺テーブルの中心座標(ドット単位)
			/// </summary>
			public I2Point CenterPoint;

			/// <summary>
			/// 周辺テーブルの中心から指定座標までの相対座標(ドット単位)
			/// </summary>
			public I2Point RelativePoint;

			/// <summary>
			/// 周辺テーブルを作成する。
			/// </summary>
			/// <param name="x">指定座標(Ｘ座標・ドット単位)</param>
			/// <param name="y">指定座標(Ｙ座標・ドット単位)</param>
			/// <param name="size">周辺テーブルの幅・高さ</param>
			public Around(int x, int y, int size)
			{
				I2Point pt = new I2Point(x, y);

				this.Size = size;
				this.Table = new int[size, size];

				// 周辺テーブルの左上へ移動
				x -= (size - 1) * TAConsts.TILE_W / 2;
				y -= (size - 1) * TAConsts.TILE_H / 2;

				const int TMP_SPAN = 1000;

				// マップ座標(ドット単位) -> マップテーブル座標
				x += TAConsts.TILE_W * TMP_SPAN;
				y += TAConsts.TILE_W * TMP_SPAN;
				x /= TAConsts.TILE_W;
				y /= TAConsts.TILE_H;
				x -= TMP_SPAN;
				y -= TMP_SPAN;

				for (int xc = 0; xc < size; xc++)
					for (int yc = 0; yc < size; yc++)
						this.Table[xc, yc] = TAGame.I.Field.IsGround(new I2Point(x + xc, y + yc)) ? 0 : 1;

				// マップテーブル座標 -> マップ座標(ドット単位)
				x += TMP_SPAN;
				y += TMP_SPAN;
				x *= TAConsts.TILE_W;
				y *= TAConsts.TILE_H;
				x -= TAConsts.TILE_W * TMP_SPAN;
				y -= TAConsts.TILE_W * TMP_SPAN;

				// 周辺テーブルの中心へ移動
				x += size * TAConsts.TILE_W / 2;
				y += size * TAConsts.TILE_H / 2;

				this.CenterPoint = new I2Point(x, y);
				this.RelativePoint = new I2Point(pt.X - x, pt.Y - y);
			}

			public void TurnX()
			{
				this.RelativePoint.X *= -1;

				for (int x = 0; x < this.Size / 2; x++)
					for (int y = 0; y < this.Size; y++)
						SCommon.Swap(ref this.Table[x, y], ref this.Table[this.Size - 1 - x, y]);
			}

			public void TurnY()
			{
				this.RelativePoint.Y *= -1;

				for (int x = 0; x < this.Size; x++)
					for (int y = 0; y < this.Size / 2; y++)
						SCommon.Swap(ref this.Table[x, y], ref this.Table[x, this.Size - 1 - y]);
			}
		}
	}
}
