using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Shots;

namespace Charlotte.Games.TActions.Enemies
{
	/// <summary>
	/// テスト用_敵
	/// </summary>
	public class TAEnemy_Test0001 : TAEnemy
	{
		public TAEnemy_Test0001(double x, double y)
			: base(x, y, 10, 3, false)
		{ }

		private const int HIT_BACK_FRAME_MAX = 10;

		private int HitBackFrame = 0; // 0 == 無効, 1～ ヒットバック中

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				double SPEED = 2.0;
				double xBuru = 0.0;
				double yBuru = 0.0;

				if (1 <= this.HitBackFrame)
				{
					int frm = this.HitBackFrame - 1;

					if (HIT_BACK_FRAME_MAX < frm)
					{
						this.HitBackFrame = 0;
						goto endOfHitBack;
					}
					this.HitBackFrame++;

					// ----

					double rate = (double)frm / HIT_BACK_FRAME_MAX;

					SPEED = 0.0;
					xBuru = (1.0 - rate) * SCommon.CRandom.GetDoubleRange(-30.0, 30.0);
					yBuru = (1.0 - rate) * SCommon.CRandom.GetDoubleRange(-30.0, 30.0);
				}
			endOfHitBack:

				switch (frame / 60 % 4)
				{
					case 0: this.X += SPEED; break;
					case 1: this.Y += SPEED; break;
					case 2: this.X -= SPEED; break;
					case 3: this.Y -= SPEED; break;

					default:
						throw null; // never
				}

				if (!TACommon.IsOutOfCamera(new D2Point(this.X, this.Y), 100.0))
				{
					if (1 <= this.HitBackFrame)
						DD.SetBright(new D3Color(1.0, 0.8, 1.0));
					else
						DD.SetBright(new D3Color(1.0, 0.5, 0.0));

					DD.Draw(
						Pictures.WhiteBox,
						D4Rect.XYWH(
							this.X - TAGame.I.Camera.X + xBuru,
							this.Y - TAGame.I.Camera.Y + yBuru,
							100.0,
							100.0
							)
						);
					// old
					/*
					DD.SetZoom(100.0 / Pictures.WhiteBox.W, 100.0 / Pictures.WhiteBox.H);
					DD.Draw(
						Pictures.WhiteBox,
						new D2Point(
							this.X - TAGame.I.Camera.X + xBuru,
							this.Y - TAGame.I.Camera.Y + yBuru
							)
						);
					//*/

					DD.SetPrintBorder(new I3Color(128, 64, 0), 1);
					DD.SetPrint(
						(int)this.X - TAGame.I.Camera.X - 46,
						(int)this.Y - TAGame.I.Camera.Y - 46,
						20
						);
					DD.PrintLine("敵(仮)");
					DD.PrintLine("HP:" + this.HP);

					//this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), 50.0);
					this.Crash = Crash.CreateRect(D4Rect.XYWH(this.X, this.Y, 100.0, 100.0));
				}
				yield return true;
			}
		}

		protected override void P_Damaged(TAShot shot, int damagePoint)
		{
			this.HitBackFrame = 1;
			TAEnemyCommon.Damaged(this, shot, damagePoint);
		}
	}
}
