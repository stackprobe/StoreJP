using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Tiles
{
	public class SATile_石壁 : SATile
	{
		public override bool IsWall()
		{
			return true;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			DD.Draw(Pictures.石壁, drawPt);
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			SATileCommon.DrawSimply(tilePt, drawPt, Pictures.石壁);
		}
	}
}
