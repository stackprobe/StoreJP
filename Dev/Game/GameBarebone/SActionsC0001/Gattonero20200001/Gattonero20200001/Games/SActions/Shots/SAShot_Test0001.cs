using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Shots
{
	/// <summary>
	/// テスト用_自弾
	/// </summary>
	public class SAShot_Test0001 : SAShot
	{
		private bool FacingLeft;
		private bool UwamukiFlag;
		private bool ShitamukiFlag;

		public SAShot_Test0001(double x, double y, bool facingLeft, bool uwamukiFlag, bool shitamukiFlag)
			: base(x, y, 1, false)
		{
			this.FacingLeft = facingLeft;
			this.UwamukiFlag = uwamukiFlag;
			this.ShitamukiFlag = shitamukiFlag;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			if (this.UwamukiFlag)
			{
				this.Y -= 20.0;
			}
			else if (this.ShitamukiFlag)
			{
				this.X += 5.0 * (this.FacingLeft ? -1 : 1);
				this.Y += 25.0;
			}
			else
			{
				this.X += 30.0 * (this.FacingLeft ? -1 : 1);
				this.Y += 5.0;
			}

			for (; ; )
			{
				const double SPEED = 20.0;
				//const double SPEED = 8.0; // old

				const double CRASH_R = 10.0;
				//const double CRASH_R = 5.0; // old

				if (this.UwamukiFlag)
				{
					this.Y -= SPEED;
				}
				else if (this.ShitamukiFlag)
				{
					this.Y += SPEED;
				}
				else
				{
					this.X += SPEED * (this.FacingLeft ? -1 : 1);
				}

				if (SACommon.IsOutOfCamera(new D2Point(this.X, this.Y))) // カメラから出たら消滅する。
					break;

				if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X, this.Y))).IsWall()) // 壁に当たったら自滅する。
				{
					this.Kill();
					break;
				}

				DD.SetBright(new D3Color(0.0, 1.0, 0.5));
				DD.Draw(Pictures.WhiteCircle, D4Rect.XYWH(this.X - SAGame.I.Camera.X, this.Y - SAGame.I.Camera.Y, 20.0, 20.0));
				// old
				/*
				DD.SetZoom(0.1);
				DD.Draw(Pictures.Dummy, new D2Point(this.X - SAGame.I.Camera.X, this.Y - SAGame.I.Camera.Y));
				//*/

				this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), CRASH_R);

				yield return true;
			}
		}
	}
}
