using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Attacks
{
	/// <summary>
	/// アタック共通
	/// ゲームメインに実装されている(非アタック中と)共通する処理も実装する。
	/// -- 共通化は難しそうなのでゲームメインと重複して実装する。
	/// </summary>
	public static class SAAttackCommon
	{
		// プレイヤー動作セット
		// -- この辺やっとけば良いんじゃないか的な
		//
		// ProcPlayer_移動();
		// ProcPlayer_Fall();
		//
		// ProcPlayer_側面();
		// ProcPlayer_脳天();
		// ProcPlayer_接地();
		//
		// ProcPlayer_Status();
		// ProcPlayer_当たり判定();
		//
		// プレイヤーの描画 -> SAGame.I.Player.Draw_TL
		//

		// ======================
		// ==== プレイヤー動作 ====
		// ======================

		public static void ProcPlayer_移動()
		{
			if (SlideCamera())
				return;

			double speed;

			if (1 <= Inputs.R.GetInput())
				speed = SAConsts.PLAYER_SLOW_SPEED;
			else
				speed = SAConsts.PLAYER_SPEED;

			// 攻撃中は左右の方向転換を抑止する。

			if (1 <= Inputs.DIR_4.GetInput())
			{
				SAGame.I.Player.X -= speed;
				//SAGame.I.Player.FacingLeft = true; // 抑止
			}
			if (1 <= Inputs.DIR_6.GetInput())
			{
				SAGame.I.Player.X += speed;
				//SAGame.I.Player.FacingLeft = false; // 抑止
			}
		}

		public static void ProcPlayer_Fall()
		{
			if (1 <= SAGame.I.Player.JumpFrame) // ? ジャンプ中(だった)
			{
				if (Inputs.A.GetInput() <= 0) // ? ジャンプを中断・終了した。
				{
					SAGame.I.Player.JumpFrame = 0;

					if (SAGame.I.Player.YSpeed < 0.0)
						SAGame.I.Player.YSpeed /= 2.0;
				}
			}

			// 重力による加速
			SAGame.I.Player.YSpeed += SAConsts.PLAYER_GRAVITY;

			// 自由落下の最高速度を超えないようにする。
			SAGame.I.Player.YSpeed = Math.Min(SAGame.I.Player.YSpeed, SAConsts.PLAYER_FALL_SPEED_MAX);

			// 自由落下
			SAGame.I.Player.Y += SAGame.I.Player.YSpeed;
		}

		// ===============================
		// ==== プレイヤー動作・接地系判定 ====
		// ===============================

		public static bool IsPlayer_側面()
		{
			return GetPlayer_側面() != 0;
		}

		public static int GetPlayer_側面()
		{
			bool touchSide_L =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y - SAConsts.PLAYER_側面判定Pt_YT))).IsWall() ||
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y))).IsWall() ||
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y + SAConsts.PLAYER_側面判定Pt_YB))).IsWall();

			bool touchSide_R =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y - SAConsts.PLAYER_側面判定Pt_YT))).IsWall() ||
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y))).IsWall() ||
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y + SAConsts.PLAYER_側面判定Pt_YB))).IsWall();

			return (touchSide_L ? 1 : 0) | (touchSide_R ? 2 : 0);
		}

		public static int GetPlayer_側面Sub()
		{
			bool touchSide_L =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y))).IsWall();

			bool touchSide_R =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_側面判定Pt_X, SAGame.I.Player.Y))).IsWall();

			return (touchSide_L ? 1 : 0) | (touchSide_R ? 2 : 0);
		}

		public static bool IsPlayer_脳天()
		{
			bool touchCeiling_L =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_脳天判定Pt_X, SAGame.I.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y))).IsWall();

			bool touchCeiling_M =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X, SAGame.I.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y))).IsWall();

			bool touchCeiling_R =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_脳天判定Pt_X, SAGame.I.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y))).IsWall();

			return (touchCeiling_L && touchCeiling_R) || touchCeiling_M;
		}

		public static bool IsPlayer_接地()
		{
			bool touchGround =
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X - SAConsts.PLAYER_接地判定Pt_X, SAGame.I.Player.Y + SAConsts.PLAYER_接地判定Pt_Y))).IsWall() ||
				SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(SAGame.I.Player.X + SAConsts.PLAYER_接地判定Pt_X, SAGame.I.Player.Y + SAConsts.PLAYER_接地判定Pt_Y))).IsWall();

			return touchGround;
		}

		// ===============================
		// ==== プレイヤー動作・接地系処理 ====
		// ===============================

		public static bool ProcPlayer_側面()
		{
			int flag = GetPlayer_側面();

			if (flag == 3) // 左右両方 -> 壁抜け防止のため再チェック
			{
				flag = GetPlayer_側面Sub();
			}

			if (flag == 3) // 左右両方
			{
				// noop
			}
			else if (flag == 1) // 左側面
			{
				SAGame.I.Player.X = SACommon.ToTileCenterX(SAGame.I.Player.X - SAConsts.PLAYER_側面判定Pt_X) + SAConsts.TILE_W / 2 + SAConsts.PLAYER_側面判定Pt_X;
			}
			else if (flag == 2) // 右側面
			{
				SAGame.I.Player.X = SACommon.ToTileCenterX(SAGame.I.Player.X + SAConsts.PLAYER_側面判定Pt_X) - SAConsts.TILE_W / 2 - SAConsts.PLAYER_側面判定Pt_X;
			}
			else if (flag == 0) // なし
			{
				// noop
			}
			else
			{
				throw null; // never
			}
			return flag != 0;
		}

		public static bool ProcPlayer_脳天()
		{
			bool ret = IsPlayer_脳天();

			if (ret)
			{
				SAGame.I.Player.Y = SACommon.ToTileCenterY(SAGame.I.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y) + SAConsts.TILE_H / 2 + SAConsts.PLAYER_脳天判定Pt_Y;
				SAGame.I.Player.YSpeed = Math.Max(0.0, SAGame.I.Player.YSpeed);
			}
			return ret;
		}

		public static bool ProcPlayer_接地()
		{
			bool ret = IsPlayer_接地();

			if (ret)
			{
				SAGame.I.Player.Y = SACommon.ToTileCenterY(SAGame.I.Player.Y + SAConsts.PLAYER_接地判定Pt_Y) - SAConsts.TILE_H / 2 - SAConsts.PLAYER_接地判定Pt_Y;
				SAGame.I.Player.YSpeed = Math.Min(0.0, SAGame.I.Player.YSpeed);
			}
			return ret;
		}

		// ============================
		// ==== プレイヤー動作・その他 ====
		// ============================

		public static void ProcPlayer_Status()
		{
			if (1 <= SAGame.I.Player.DamageFrame && SAConsts.PLAYER_DAMAGE_FRAME_MAX < ++SAGame.I.Player.DamageFrame)
			{
				SAGame.I.Player.DamageFrame = 0;
				SAGame.I.Player.InvincibleFrame = 1;
			}
			if (1 <= SAGame.I.Player.InvincibleFrame && SAConsts.PLAYER_INVINCIBLE_FRAME_MAX < ++SAGame.I.Player.InvincibleFrame)
			{
				SAGame.I.Player.InvincibleFrame = 0;
			}
		}

		public static void ProcPlayer_当たり判定()
		{
			SAGame.I.Player.Crash = Crash.CreatePoint(new D2Point(SAGame.I.Player.X, SAGame.I.Player.Y));
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
				if (Inputs.DIR_8.IsPound())
				{
					SAGame.I.CameraSlideCount++;
					SAGame.I.CameraSlideY--;
				}
				if (Inputs.DIR_2.IsPound())
				{
					SAGame.I.CameraSlideCount++;
					SAGame.I.CameraSlideY++;
				}
				if (Inputs.DIR_4.IsPound())
				{
					SAGame.I.CameraSlideCount++;
					SAGame.I.CameraSlideX--;
				}
				if (Inputs.DIR_6.IsPound())
				{
					SAGame.I.CameraSlideCount++;
					SAGame.I.CameraSlideX++;
				}
				SAGame.I.CameraSlideX = SCommon.ToRange(SAGame.I.CameraSlideX, -1, 1);
				SAGame.I.CameraSlideY = SCommon.ToRange(SAGame.I.CameraSlideY, -1, 1);
				SAGame.I.CameraSlideMode = true;
			}
			else
			{
				if (SAGame.I.CameraSlideMode && SAGame.I.CameraSlideCount == 0)
				{
					SAGame.I.CameraSlideX = 0;
					SAGame.I.CameraSlideY = 0;
				}
				SAGame.I.CameraSlideCount = 0;
				SAGame.I.CameraSlideMode = false;
			}
			return SAGame.I.CameraSlideMode;
		}

		// =================================
		// ==== プレイヤー動作系 (ここまで) ====
		// =================================
	}
}
