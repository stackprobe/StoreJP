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
	/// 波動ビーム
	/// </summary>
	public class SAShot_WaveBeam : SAShot
	{
		private bool FacingLeft;
		private bool UwamukiFlag;
		private bool ShitamukiFlag;

		public SAShot_WaveBeam(double x, double y, bool facingLeft, bool uwamukiFlag, bool shitamukiFlag)
			: base(x, y, 5, false)
		{
			this.FacingLeft = facingLeft;
			this.UwamukiFlag = uwamukiFlag;
			this.ShitamukiFlag = shitamukiFlag;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			const double R = 8.0;
			const double SPEED = 10.0;

			double baseRad = SCommon.CRandom.GetInt(2) == 0 ? 0.0 : Math.PI;

			for (int frame = 0; ; frame++)
			{
				if (this.UwamukiFlag)
					this.Y -= SPEED;
				else if (this.ShitamukiFlag)
					this.Y += SPEED;
				else
					this.X += SPEED * (this.FacingLeft ? -1 : 1);

				double x = this.X;
				double y = this.Y;

				double waveZure = Math.Sin(baseRad + frame / 2.0) * 50.0;

				if (this.UwamukiFlag || this.ShitamukiFlag)
					x += waveZure;
				else
					y += waveZure;

				DD.SetSize(new D2Size(R * 2.0, R * 2.0));
				DD.SetBright(new I3Color(255, 128, 255).ToD3Color());
				DD.Draw(Pictures.WhiteCircle, new D2Point(x - SAGame.I.Camera.X, y - SAGame.I.Camera.Y));

				this.Crash = Crash.CreateCircle(new D2Point(x, y), R);

				yield return !SACommon.IsOutOfCamera(new D2Point(x, y)); // カメラから出たら消滅する。
			}
		}
	}
}
