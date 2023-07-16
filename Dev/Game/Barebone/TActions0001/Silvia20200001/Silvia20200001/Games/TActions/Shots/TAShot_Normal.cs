using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Shots
{
	public class TAShot_Normal : TAShot
	{
		private int Direction; // この自弾の進行方向(8方向_テンキー方式)

		public TAShot_Normal(double x, double y, int direction)
			: base(x, y, 3, false)
		{
			this.Direction = direction;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			D2Point speed = TACommon.GetDirectionSpeed(this.Direction, 20.0);

			for (int frame = 0; ; frame++)
			{
				this.X += speed.X;
				this.Y += speed.Y;

				if (TACommon.IsOutOfCamera(new D2Point(this.X, this.Y))) // カメラの外に出たら(画面から見えなくなったら)消滅する。
					break;

				if (TAGame.I.Field.IsWall(TACommon.ToTablePoint(new D2Point(this.X, this.Y)))) // 壁に当たったら自滅する。
				{
					this.Kill();
					break;
				}

				DD.SetBright(new D3Color(0.0, 1.0, 0.5));
				DD.Draw(Pictures.WhiteCircle, D4Rect.XYWH(this.X - TAGame.I.Camera.X, this.Y - TAGame.I.Camera.Y, 20.0, 20.0));
				// old
				/*
				DD.SetBright(new D3Color(0.0, 1.0, 0.5));
				DD.SetZoom(20.0 / Pictures.WhiteCircle.W, 20.0 / Pictures.WhiteCircle.H);
				DD.Draw(Pictures.WhiteCircle, new D2Point(this.X - TAGame.I.Camera.X, this.Y - TAGame.I.Camera.Y));
				//*/

				this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), 10.0);

				yield return true;
			}
		}
	}
}
