using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public static class TAEffects
	{
		// memo: Once, Delay, Keep は DD にあるよ。

		public static IEnumerable<bool> Explode(double x, double y, double scale)
		{
			foreach (Scene scene in Scene.Create(15))
			{
				DD.SetAlpha(0.7);
				DD.SetBright(new D3Color(1.0, 0.75, 0.5));
				DD.SetZoom(scale * scene.Rate);
				DD.Draw(Pictures.WhiteCircle, new D2Point(x - TAGame.I.Camera.X, y - TAGame.I.Camera.Y));

				yield return true;
			}
		}
	}
}
