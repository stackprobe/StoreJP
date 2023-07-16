using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Shootings.Enemies
{
	/// <summary>
	/// テスト用_敵
	/// </summary>
	public class SHEnemy_Test0001 : SHEnemy
	{
		public SHEnemy_Test0001(double x, double y)
			: base(x, y, 30, Kind_e.通常敵)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 1; ; frame++)
			{
				D2Point speed = DD.AngleToPoint(
					DD.GetAngle(SHGame.I.Player.X - this.X, SHGame.I.Player.Y - this.Y),
					2.5
					);

				this.X += speed.X;
				this.Y += speed.Y;

				DD.Draw(Pictures.Dummy, D4Rect.XYWH(this.X, this.Y, 128.0, 128.0));
				// old
				/*
				DD.SetZoom(128.0 / Pictures.Dummy.W, 128.0 / Pictures.Dummy.H);
				DD.Draw(Pictures.Dummy, new D2Point(this.X, this.Y));
				//*/

				this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), 64.0);

				yield return true; // 自機をホーミングするので、画面外に出て行かない。
			}
		}
	}
}
