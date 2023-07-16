using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.Dungeons.Fields;

namespace Charlotte.Games.Dungeons
{
	public static class DUDungeonScreen
	{
		public static class Layout
		{
			public const int SCREEN_W = 970;
			public const int SCREEN_H = 530;

			public static readonly D4Rect FRONT_WALL_0 = new D4Rect(30 * 0, 24 * 0, SCREEN_W - (30 + 30) * 0, SCREEN_H - (24 + 8) * 0);
			public static readonly D4Rect FRONT_WALL_1 = new D4Rect(30 * 8, 24 * 8, SCREEN_W - (30 + 30) * 8, SCREEN_H - (24 + 8) * 8);
			public static readonly D4Rect FRONT_WALL_2 = new D4Rect(30 * 12, 24 * 12, SCREEN_W - (30 + 30) * 12, SCREEN_H - (24 + 8) * 12);
			public static readonly D4Rect FRONT_WALL_3 = new D4Rect(30 * 14, 24 * 14, SCREEN_W - (30 + 30) * 14, SCREEN_H - (24 + 8) * 14);
			public static readonly D4Rect FRONT_WALL_4 = new D4Rect(30 * 15, 24 * 15, SCREEN_W - (30 + 30) * 15, SCREEN_H - (24 + 8) * 15);

			public static readonly D4Rect WALK_FRONT_WALL_0 = new D4Rect(30 * -8, 24 * -8, SCREEN_W - (30 + 30) * -16, SCREEN_H - (24 + 8) * -8);
			public static readonly D4Rect WALK_FRONT_WALL_1 = new D4Rect(30 * 4, 24 * 4, SCREEN_W - (30 + 30) * 4, SCREEN_H - (24 + 8) * 4);
			public static readonly D4Rect WALK_FRONT_WALL_2 = new D4Rect(30 * 10, 24 * 10, SCREEN_W - (30 + 30) * 10, SCREEN_H - (24 + 8) * 10);
			public static readonly D4Rect WALK_FRONT_WALL_3 = new D4Rect(30 * 13, 24 * 13, SCREEN_W - (30 + 30) * 13, SCREEN_H - (24 + 8) * 13);
			public static readonly D4Rect WALK_FRONT_WALL_4 = new D4Rect(30 * 14.5, 24 * 14.5, SCREEN_W - (30 + 30) * 14.5, SCREEN_H - (24 + 8) * 14.5);
		}

		private static VScreen Screen = new VScreen(Layout.SCREEN_W, Layout.SCREEN_H);

		public static VScreen GetScreen()
		{
			return Screen;
		}

		private static DUField Field;
		private static int ViewPointX;
		private static int ViewPointY;
		private static int ViewPointDirection;
		private static bool Walking;

		public static void Draw(DUField field, int x, int y, int direction, bool walking = false)
		{
			Field = field;
			ViewPointX = x;
			ViewPointY = y;
			ViewPointDirection = direction;
			Walking = walking;

			Draw_Main();

			Field = null;
			ViewPointX = default(int);
			ViewPointY = default(int);
			ViewPointDirection = default(int);
			Walking = default(bool);
		}

		private static void Draw_Main()
		{
			using (Screen.Section())
			{
				DD.Draw(Field.GetBackgroundPicture(), new I4Rect(0, 0, Layout.SCREEN_W, Layout.SCREEN_H).ToD4Rect());

				if (Walking)
				{
					DrawLayer(Layout.WALK_FRONT_WALL_4, Layout.WALK_FRONT_WALL_3, 3);
					DrawLayer(Layout.WALK_FRONT_WALL_3, Layout.WALK_FRONT_WALL_2, 2);
					DrawLayer(Layout.WALK_FRONT_WALL_2, Layout.WALK_FRONT_WALL_1, 1);
					DrawLayer(Layout.WALK_FRONT_WALL_1, Layout.WALK_FRONT_WALL_0, 0);
				}
				else
				{
					DrawLayer(Layout.FRONT_WALL_4, Layout.FRONT_WALL_3, 3);
					DrawLayer(Layout.FRONT_WALL_3, Layout.FRONT_WALL_2, 2);
					DrawLayer(Layout.FRONT_WALL_2, Layout.FRONT_WALL_1, 1);
					DrawLayer(Layout.FRONT_WALL_1, Layout.FRONT_WALL_0, 0);
				}
			}
		}

		private static void DrawLayer(D4Rect frontBaseRect, D4Rect behindBaseRect, int y)
		{
			DrawWall(GetWall(0, y, 8), frontBaseRect.Poly, y + 0.5);

			int x;

			for (x = 1; ; x++)
			{
				D4Rect frontRect = frontBaseRect;

				frontRect.L = frontBaseRect.L + x * frontBaseRect.W;

				if (Layout.SCREEN_W <= frontRect.L)
					break;

				DrawWall(GetWall(x, y, 8), frontRect.Poly, y + 0.5);

				frontRect.L = frontBaseRect.L - x * frontBaseRect.W;

				DrawWall(GetWall(-x, y, 8), frontRect.Poly, y + 0.5);
			}
			for (x -= 2; 0 <= x; x--)
			{
				D4Rect frontRect = frontBaseRect;
				D4Rect behindRect = behindBaseRect;

				frontRect.L = frontBaseRect.L + x * frontBaseRect.W;
				behindRect.L = behindBaseRect.L + x * behindBaseRect.W;

				DrawWall(GetWall(x, y, 6), new P4Poly(frontRect.RT, behindRect.RT, behindRect.RB, frontRect.RB), y);

				frontRect.L = frontBaseRect.L - x * frontBaseRect.W;
				behindRect.L = behindBaseRect.L - x * behindBaseRect.W;

				DrawWall(GetWall(-x, y, 4), new P4Poly(behindRect.LT, frontRect.LT, frontRect.LB, behindRect.LB), y);
			}
		}

		private static int GetWall(int x, int y, int direction)
		{
			switch (ViewPointDirection)
			{
				case 4: return Field.GetWall(new I2Point(ViewPointX - y, ViewPointY - x), DUCommon.RotL(direction));
				case 6: return Field.GetWall(new I2Point(ViewPointX + y, ViewPointY + x), DUCommon.RotR(direction));
				case 8: return Field.GetWall(new I2Point(ViewPointX + x, ViewPointY - y), direction);
				case 2: return Field.GetWall(new I2Point(ViewPointX - x, ViewPointY + y), 10 - direction);

				default:
					throw null; // never
			}
		}

		private static void DrawWall(int kind, P4Poly poly, double y)
		{
			Picture picture;

			switch (kind)
			{
				case 0: // 空間
					return;

				case 1: // 壁
					picture = Field.GetWallPicture();
					break;

				case 2: // ゲート
					picture = Field.GetGatePicture();
					break;

				default:
					throw null; // never
			}
			double bright = 1.0 - y / 10.0;

			DD.SetBright(new D3Color(bright, bright, bright));
			DD.Draw(picture, poly);
		}
	}
}
