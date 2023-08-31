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
	public class SAAttack_Test0001 : SAAttack
	{
		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (Inputs.A.GetInput() == 1)
				{
					break;
				}
				if (1 <= Inputs.B.GetInput() && Inputs.B.GetInput() % 10 == 1)
				{
					SAGame.I.Shots.Add(new SAShot_Test0001(SAGame.I.Player.X, SAGame.I.Player.Y, SAGame.I.Player.FacingLeft, false, false));
				}

				SAAttackCommon.ProcPlayer_移動();
				SAAttackCommon.ProcPlayer_Fall();

				SAAttackCommon.ProcPlayer_側面();
				SAAttackCommon.ProcPlayer_脳天();
				SAAttackCommon.ProcPlayer_接地();

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

				DD.TL.Add(DD.Once(() =>
				{
					DD.SetPrint(
						(int)SAGame.I.Player.X - SAGame.I.Camera.X - 80,
						(int)SAGame.I.Player.Y - SAGame.I.Camera.Y - 60,
						0
						);
					DD.SetPrintBorder(new I3Color(0, 0, 192), 1);
					DD.Print("SAAttack_Test0001 テスト");
				}));

				SAGame.I.Player.Draw_TL.Add(DD.Once(() =>
				{
					DD.SetAlpha(plA);
					DD.Draw(
						Pictures.CirnoStand,
						new D2Point(
							SAGame.I.Player.X - SAGame.I.Camera.X,
							SAGame.I.Player.Y - SAGame.I.Camera.Y
							)
						);
				}));

				yield return true;
			}
		}
	}
}
