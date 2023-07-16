using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.SActions.Shots;

namespace Charlotte.Games.SActions.Enemies
{
	/// <summary>
	/// テスト用_敵
	/// </summary>
	public class SAEnemy_Test0001 : SAEnemy
	{
		public SAEnemy_Test0001(double x, double y)
			: base(x, y, 50, 3, false)
		{ }

		private const int HIT_BACK_FRAME_MAX = 10;

		private int HitBackFrame = 0; // 1～ == ヒットバック中, 0 == 無効

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				Picture picture = Pictures.Dummy;
				double SPEED = 2.0;
				double xBuru = 0.0;
				double yBuru = 0.0;

				if (1 <= this.HitBackFrame)
				{
					if (HIT_BACK_FRAME_MAX < ++this.HitBackFrame)
					{
						this.HitBackFrame = 0;
						goto endOfHitBack;
					}

					double rate = (double)this.HitBackFrame / HIT_BACK_FRAME_MAX;

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

				if (!SACommon.IsOutOfCamera(new D2Point(this.X, this.Y), 100.0)) // カメラ外では描画しない。
				{
					DD.Draw(
						picture,
						D4Rect.XYWH(
							this.X - SAGame.I.Camera.X + xBuru,
							this.Y - SAGame.I.Camera.Y + yBuru,
							100.0,
							100.0
							)
						);
					// old
					/*
					DD.SetZoom(100.0 / Pictures.Dummy.W, 100.0 / Pictures.Dummy.H);
					DD.Draw(
						picture,
						new D2Point(
							this.X - SAGame.I.Camera.X + xBuru,
							this.Y - SAGame.I.Camera.Y + yBuru
							)
						);
					//*/

					//this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), 50.0);
					this.Crash = Crash.CreateRect(D4Rect.XYWH(this.X, this.Y, 100.0, 100.0));
				}
				yield return true;
			}
		}

		protected override void P_Damaged(SAShot shot, int damagePoint)
		{
			this.HitBackFrame = 1;
			SAEnemyCommon.Damaged(this, shot, damagePoint);
		}
	}
}
