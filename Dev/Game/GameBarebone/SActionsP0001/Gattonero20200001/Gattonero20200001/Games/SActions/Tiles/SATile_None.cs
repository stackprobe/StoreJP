using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Tiles
{
	public class SATile_None : SATile
	{
		public override bool IsWall()
		{
			return false;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			// noop
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			// noop
		}
	}
}
