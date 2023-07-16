using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Shootings.Shots
{
	/// <summary>
	/// 通常弾
	/// </summary>
	public class SHShot_Test0001 : SHShot
	{
		public SHShot_Test0001(double x, double y)
			: base(x, y, 1, false)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				this.X += 10;

				DD.SetAlpha(0.5);
				DD.Draw(Pictures.Dummy, D4Rect.XYWH(this.X, this.Y, 16.0, 16.0));
				// old
				/*
				DD.SetAlpha(0.5);
				DD.SetZoom(16.0 / Pictures.Dummy.W, 16.0 / Pictures.Dummy.H);
				DD.Draw(Pictures.Dummy, new D2Point(this.X, this.Y));
				//*/

				this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), 16.0);

				yield return !SHCommon.IsOutOfScreen(new D2Point(this.X, this.Y), 16.0);
			}
		}
	}
}
