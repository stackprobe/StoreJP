using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions
{
	public static class SAEffects
	{
		// memo: Once, Delay, Keep は DD にあるよ。

		public static IEnumerable<bool> Explode(double x, double y, double scale)
		{
			foreach (Scene scene in Scene.Create(15))
			{
				DD.SetAlpha(0.7);
				DD.SetBright(new D3Color(1.0, 0.75, 0.5));
				DD.SetZoom(scale * scene.Rate);
				DD.Draw(Pictures.WhiteCircle, new D2Point(x - SAGame.I.Camera.X, y - SAGame.I.Camera.Y));

				yield return true;
			}
		}

		public static IEnumerable<bool> 空中ジャンプの足場(double x, double y)
		{
			foreach (Scene scene in Scene.Create(10))
			{
				DD.SetAlpha(1.0 - scene.Rate);
				DD.SetBright(new D3Color(0.5, 1.0, 1.0));
				DD.SetZoom(0.5 + 1.0 * scene.Rate);
				DD.Draw(Pictures.WhiteCircle, new D2Point(x - SAGame.I.Camera.X, y - SAGame.I.Camera.Y));

				yield return true;
			}
		}
	}
}
