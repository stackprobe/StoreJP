using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Attacks
{
	/// <summary>
	/// アタック共通
	/// ゲームメインに実装されている(非アタック中と)共通する処理も実装する。
	/// -- 共通化は難しそうなのでゲームメインと重複して実装する。
	/// </summary>
	public static class TAAttackCommon
	{
		// プレイヤー動作セット
		// -- この辺やっとけば良いんじゃないか的な
		//
		// ProcPlayer_移動();
		// ProcPlayer_位置訂正();
		// ProcPlayer_Status();
		// ProcPlayer_当たり判定();
		//
		// プレイヤーの描画 -> TAGame.I.Player.Draw_TL
		//

		// ======================
		// ==== プレイヤー動作 ====
		// ======================

		public static void ProcPlayer_移動(double speed = TAConsts.PLAYER_SPEED)
		{
			if (SlideCamera())
				return;

			bool dir2 = 1 <= Inputs.DIR_2.GetInput();
			bool dir4 = 1 <= Inputs.DIR_4.GetInput();
			bool dir6 = 1 <= Inputs.DIR_6.GetInput();
			bool dir8 = 1 <= Inputs.DIR_8.GetInput();

			int dir; // 1～9 == { 左下, 下, 右下, 左, 動かない, 右, 左上, 上, 右上 }

			if (dir2 && dir4)
				dir = 1;
			else if (dir2 && dir6)
				dir = 3;
			else if (dir4 && dir8)
				dir = 7;
			else if (dir6 && dir8)
				dir = 9;
			else if (dir2)
				dir = 2;
			else if (dir4)
				dir = 4;
			else if (dir6)
				dir = 6;
			else if (dir8)
				dir = 8;
			else
				dir = 5;

			//double speed = TAConsts.PLAYER_SPEED;
			double nanameSpeed = speed / TAConsts.ROOT_OF_2;

			switch (dir)
			{
				case 4:
					TAGame.I.Player.X -= speed;
					break;

				case 6:
					TAGame.I.Player.X += speed;
					break;

				case 8:
					TAGame.I.Player.Y -= speed;
					break;

				case 2:
					TAGame.I.Player.Y += speed;
					break;

				case 1:
					TAGame.I.Player.X -= nanameSpeed;
					TAGame.I.Player.Y += nanameSpeed;
					break;

				case 3:
					TAGame.I.Player.X += nanameSpeed;
					TAGame.I.Player.Y += nanameSpeed;
					break;

				case 7:
					TAGame.I.Player.X -= nanameSpeed;
					TAGame.I.Player.Y -= nanameSpeed;
					break;

				case 9:
					TAGame.I.Player.X += nanameSpeed;
					TAGame.I.Player.Y -= nanameSpeed;
					break;

				case 5:
					// 立ち止まったら座標を整数にする。
					//
					TAGame.I.Player.X = SCommon.ToInt(TAGame.I.Player.X);
					TAGame.I.Player.Y = SCommon.ToInt(TAGame.I.Player.Y);
					break;

				default:
					throw null; // never
			}
		}

		public static void ProcPlayer_位置訂正()
		{
			D2Point pt = new D2Point(TAGame.I.Player.X, TAGame.I.Player.Y);

			pt = TAPlayerPosition.Correct(pt);

			TAGame.I.Player.X = pt.X;
			TAGame.I.Player.Y = pt.Y;
		}

		public static void ProcPlayer_Status()
		{
			if (1 <= TAGame.I.Player.DamageFrame && TAConsts.PLAYER_DAMAGE_FRAME_MAX < ++TAGame.I.Player.DamageFrame)
			{
				TAGame.I.Player.DamageFrame = 0;
				TAGame.I.Player.InvincibleFrame = 1;
			}
			if (1 <= TAGame.I.Player.InvincibleFrame && TAConsts.PLAYER_INVINCIBLE_FRAME_MAX < ++TAGame.I.Player.InvincibleFrame)
			{
				TAGame.I.Player.InvincibleFrame = 0;
			}
		}

		public static void ProcPlayer_当たり判定()
		{
			TAGame.I.Player.Crash = Crash.CreatePoint(new D2Point(
				TAGame.I.Player.X,
				TAGame.I.Player.Y + 10.0
				));
		}

		// ======================================
		// ==== プレイヤー動作・カメラ位置スライド ====
		// ======================================

		/// <summary>
		/// カメラ位置スライド
		/// </summary>
		/// <returns>カメラ位置スライド_モード中か</returns>
		private static bool SlideCamera()
		{
			if (1 <= Inputs.L.GetInput())
			{
				if (Inputs.DIR_4.IsPound())
				{
					TAGame.I.CameraSlideCount++;
					TAGame.I.CameraSlideX--;
				}
				if (Inputs.DIR_6.IsPound())
				{
					TAGame.I.CameraSlideCount++;
					TAGame.I.CameraSlideX++;
				}
				if (Inputs.DIR_8.IsPound())
				{
					TAGame.I.CameraSlideCount++;
					TAGame.I.CameraSlideY--;
				}
				if (Inputs.DIR_2.IsPound())
				{
					TAGame.I.CameraSlideCount++;
					TAGame.I.CameraSlideY++;
				}
				TAGame.I.CameraSlideX = SCommon.ToRange(TAGame.I.CameraSlideX, -1, 1);
				TAGame.I.CameraSlideY = SCommon.ToRange(TAGame.I.CameraSlideY, -1, 1);
				TAGame.I.CameraSlideMode = true;
			}
			else
			{
				if (TAGame.I.CameraSlideMode && TAGame.I.CameraSlideCount == 0)
				{
					TAGame.I.CameraSlideX = 0;
					TAGame.I.CameraSlideY = 0;
				}
				TAGame.I.CameraSlideCount = 0;
				TAGame.I.CameraSlideMode = false;
			}
			return TAGame.I.CameraSlideMode;
		}

		// =================================
		// ==== プレイヤー動作系 (ここまで) ====
		// =================================
	}
}
