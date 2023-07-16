using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.Shootings.Enemies;

namespace Charlotte.Games.Shootings.Scenarios
{
	public class SHScenario_Test0001 : SHScenario
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Musics.SunBeams.Play();

			for (; ; )
			{
				if (SCommon.CRandom.GetRate() < 0.01)
				{
					SHGame.I.Enemies.Add(new SHEnemy_Test0001(
						GameConfig.ScreenSize.W + 100.0,
						SCommon.CRandom.GetDoubleRange(50.0, GameConfig.ScreenSize.H - 50.0)
						));
				}
				yield return true;
			}
		}

		protected override IEnumerable<bool> E_DrawWall()
		{
			const int TILE_W = 128;
			const int TILE_H = 128;

			int l = 0;
			int t = 0;

			for (; ; )
			{
				for (int x = l; x < GameConfig.ScreenSize.W + TILE_W; x += TILE_W)
				{
					for (int y = t; y < GameConfig.ScreenSize.H + TILE_H; y += TILE_H)
					{
						I3Color color;

						color = new I3Color(0, 64, 0);

						DD.SetBright(color.ToD3Color());
						DD.Draw(Pictures.WhiteBox, I4Rect.XYWH(x, y, TILE_W, TILE_H).ToD4Rect());
					}
				}
				DD.DrawCurtain(-0.3);

				l -= 7;
				t += 3;

				while (l < 0) l += TILE_W;
				while (l > 0) l -= TILE_W;
				while (t < 0) t += TILE_H;
				while (t > 0) t -= TILE_H;

				yield return true;
			}
		}
	}
}
