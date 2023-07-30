using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Walls
{
	public class SAWall_Test0001 : SAWall
	{
		protected virtual Picture GetPicture()
		{
			return Pictures.Wall0001;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			Picture wallPicture = this.GetPicture();

			double cameraRangeX = (double)(SAGame.I.Field.W - GameConfig.ScreenSize.W);
			double cameraRangeY = (double)(SAGame.I.Field.H - GameConfig.ScreenSize.H);
			double cameraWallRate = 0.5;

			Func<D2Point, D4Rect> cameraToDrawRect = camera =>
			{
				double x = GameConfig.ScreenSize.W / 2.0 - (camera.X - cameraRangeX / 2.0) * cameraWallRate;
				double y = GameConfig.ScreenSize.H / 2.0 - (camera.Y - cameraRangeY / 2.0) * cameraWallRate;

				return D4Rect.XYWH(x, y, wallPicture.W, wallPicture.H);
			};

			cameraWallRate = P_FindThreshold(0.0, 1.0, value =>
			{
				cameraWallRate = value;
				D4Rect rect = cameraToDrawRect(new D2Point(0.0, 0.0));

				return -10.0 < rect.L || -10.0 < rect.T;
			});

			for (; ; )
			{
				DD.Draw(wallPicture, cameraToDrawRect(SAGame.I.CameraForCalc));

				yield return true;
			}
		}

		private static double P_FindThreshold(double p1, double p2, Func<double, bool> sensor)
		{
			bool v1 = sensor(p1);
			bool v2 = sensor(p2);

			if (v1 == v2)
				//throw new Exception("no threshold");
				return (p1 + p2) / 2.0;

			double pm;

			for (int c = 0; ; c++)
			{
				pm = (p1 + p2) / 2.0;

				if (20 < c) // rough limit
					break;

				bool vm = sensor(pm);

				if (v1 == vm)
					p1 = pm;
				else
					p2 = pm;
			}
			return pm;
		}
	}
}
