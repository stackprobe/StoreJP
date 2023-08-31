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

		protected virtual I3Color GetColor()
		{
			return new I3Color(255, 255, 0);
		}

		private const int HIT_BACK_FRAME_MAX = 10;

		private int HitBackFrame = 0; // 1～ == ヒットバック中, 0 == 無効

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				double SPEED = 2.0;
				double zureX = 0.0;
				double zureY = 0.0;

				if (1 <= this.HitBackFrame)
				{
					if (HIT_BACK_FRAME_MAX < ++this.HitBackFrame)
					{
						this.HitBackFrame = 0;
						goto endOfHitBack;
					}

					double rate = (double)this.HitBackFrame / HIT_BACK_FRAME_MAX;

					SPEED = 0.0;
					zureX = (1.0 - rate) * SCommon.CRandom.GetDoubleRange(-30.0, 30.0);
					zureY = (1.0 - rate) * SCommon.CRandom.GetDoubleRange(-30.0, 30.0);
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

				if (!TACommon.IsOutOfCamera(new D2Point(this.X, this.Y), 100.0)) // カメラ外では描画しない。
				{
					DD.SetBright(this.GetColor().ToD3Color());
					DD.Draw(
						Pictures.WhiteBox,
						D4Rect.XYWH(
							this.X - TAGame.I.Camera.X + zureX,
							this.Y - TAGame.I.Camera.Y + zureY,
							100.0,
							100.0
							)
						);

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
