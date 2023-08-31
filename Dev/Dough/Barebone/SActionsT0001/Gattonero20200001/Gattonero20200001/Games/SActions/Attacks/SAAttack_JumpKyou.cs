using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.SActions.Shots;

namespace Charlotte.Games.SActions.Attacks
{
	/// <summary>
	/// ジャンプ・強攻撃
	/// </summary>
	public class SAAttack_JumpKyou : SAAttack
	{
		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				if (Inputs.A.GetInput() == 1) // ? ジャンプ押下
					break;

				int koma = frame;

				koma += 2; // 最初の2コマを飛ばす。
				if (6 <= koma) // koma == 6 以降は(1コマ/2フレーム)
				{
					koma -= 6;
					koma /= 2;
					koma += 6;
				}
				if (8 <= koma) koma -= 2; // koma == 6, 7 の 2 回目
				if (8 <= koma) koma -= 2; // koma == 6, 7 の 3 回目
				if (8 <= koma) koma -= 2; // koma == 6, 7 の 4 回目

				if (Pictures.Tewi_ジャンプ強攻撃.Length <= koma)
					break;

				double x = SAGame.I.Player.X;
				double y = SAGame.I.Player.Y;
				double xZoom = SAGame.I.Player.FacingLeft ? -1.0 : 1.0;
				bool facingLeft = SAGame.I.Player.FacingLeft;

				if (4 <= frame && frame < 20) // 16回
				{
					SAGame.I.Shots.Add(new SAShot_OneTime(
						2,
						Crash.CreateCircle(
							new D2Point(
								SAGame.I.Player.X + 2.0 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0),
								SAGame.I.Player.Y + 2.0
								),
							70.0
							)
						));
				}

				SAAttackCommon.ProcPlayer_移動();
				SAAttackCommon.ProcPlayer_Fall();

				SAAttackCommon.ProcPlayer_側面();
				SAAttackCommon.ProcPlayer_脳天();

				if (SAAttackCommon.ProcPlayer_接地())
					break;

				SAAttackCommon.ProcPlayer_Status();

				double plA = 1.0;

				if (1 <= SAGame.I.Player.InvincibleFrame)
				{
					plA = 0.5;
				}
				else
				{
					SAAttackCommon.ProcPlayer_当たり判定();
				}

				SAGame.I.Player.Draw_TL.Add(DD.Once(() =>
				{
					DD.SetAlpha(plA);
					DD.SetZoom(xZoom, 1.0);
					DD.Draw(
						Pictures.Tewi_ジャンプ強攻撃[koma],
						new D2Point(
							x - SAGame.I.Camera.X,
							y - SAGame.I.Camera.Y
							)
						);
				}));

				yield return true;
			}
		}
	}
}
