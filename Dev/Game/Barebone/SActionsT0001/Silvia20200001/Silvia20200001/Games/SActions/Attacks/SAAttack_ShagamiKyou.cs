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
	/// しゃがみ・強攻撃
	/// </summary>
	public class SAAttack_ShagamiKyou : SAAttack
	{
		protected override IEnumerable<bool> E_Draw()
		{
			int zureX = 0;
			int zureY = -16;

			for (int frame = 0; ; frame++)
			{
				//int FRAME_PER_KOMA = 1;
				//int FRAME_PER_KOMA = 2;
				int FRAME_PER_KOMA = 3;

				int koma = frame / FRAME_PER_KOMA;

				if (Pictures.Tewi_しゃがみ強攻撃.Length <= koma)
					break;

				if (frame == 5 * FRAME_PER_KOMA)
				{
					//SAGame.I.Player.X += 54.0 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0); // old
					for (int c = 0; c < 6; c++)
					{
						SAGame.I.Player.X += 9.0 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0);
						SAAttackCommon.ProcPlayer_側面();
					}

					zureX = 16;
				}

				double x = SAGame.I.Player.X + zureX * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0);
				double y = SAGame.I.Player.Y + zureY;
				double xZoom = SAGame.I.Player.FacingLeft ? -1.0 : 1.0;
				bool facingLeft = SAGame.I.Player.FacingLeft;

				if (frame == 6 * FRAME_PER_KOMA)
				{
					SAGame.I.Shots.Add(new SAShot_OneTime(
						30,
						Crash.CreateRect(D4Rect.XYWH(
							SAGame.I.Player.X + 10.0 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0),
							SAGame.I.Player.Y,
							200.0,
							120.0
							))
						));
				}

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
						Pictures.Tewi_しゃがみ強攻撃[koma],
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
