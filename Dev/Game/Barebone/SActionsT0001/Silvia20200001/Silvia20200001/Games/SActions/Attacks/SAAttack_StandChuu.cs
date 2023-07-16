﻿using System;
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
	/// 立ち・中攻撃
	/// </summary>
	public class SAAttack_StandChuu : SAAttack
	{
		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				//int FRAME_PER_KOMA = 1;
				int FRAME_PER_KOMA = 2;
				//int FRAME_PER_KOMA = 3;

				int koma = frame / FRAME_PER_KOMA;

				if (Pictures.Tewi_中攻撃.Length <= koma)
					break;

				double x = SAGame.I.Player.X + 20 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0);
				double y = SAGame.I.Player.Y - 16;
				double xZoom = SAGame.I.Player.FacingLeft ? -1.0 : 1.0;
				bool facingLeft = SAGame.I.Player.FacingLeft;

				if (frame == 4 * FRAME_PER_KOMA)
				{
					SAGame.I.Shots.Add(new SAShot_OneTime(
						20,
						Crash.CreateRect(D4Rect.XYWH(
							SAGame.I.Player.X + 55.0 * (SAGame.I.Player.FacingLeft ? -1.0 : 1.0),
							SAGame.I.Player.Y - 15.0,
							110.0,
							130.0
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
						Pictures.Tewi_中攻撃[koma],
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
