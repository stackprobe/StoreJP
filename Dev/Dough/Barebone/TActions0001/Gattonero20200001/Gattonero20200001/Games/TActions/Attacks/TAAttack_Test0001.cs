using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Attacks
{
	public class TAAttack_Test0001 : TAAttack
	{
		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (
					Inputs.A.GetInput() == 1 ||
					Inputs.B.GetInput() == 1
					)
					break;

				TAAttackCommon.ProcPlayer_移動();
				TAAttackCommon.ProcPlayer_位置訂正();
				TAAttackCommon.ProcPlayer_Status();

				double plA = 1.0;

				if (1 <= TAGame.I.Player.InvincibleFrame)
				{
					plA = 0.5;
				}
				else
				{
					TAAttackCommon.ProcPlayer_当たり判定();
				}

				DD.TL.Add(DD.Once(() =>
				{
					DD.SetPrint(
						(int)TAGame.I.Player.X - TAGame.I.Camera.X - 80,
						(int)TAGame.I.Player.Y - TAGame.I.Camera.Y - 60,
						0
						);
					DD.SetPrintBorder(new I3Color(0, 0, 192), 1);
					DD.Print("TAAttack_Test0001 テスト");
				}));

				TAGame.I.Player.Draw_TL.Add(DD.Once(() =>
				{
					DD.SetAlpha(plA);
					DD.Draw(
						TACommon.GetWalkPicture(Pictures.CirnoWalk, TAGame.I.Player.FaceDirection, 0),
						new D2Point(
							TAGame.I.Player.X - TAGame.I.Camera.X,
							TAGame.I.Player.Y - TAGame.I.Camera.Y - 12.0
							)
						);
				}));

				yield return true;
			}
		}
	}
}
