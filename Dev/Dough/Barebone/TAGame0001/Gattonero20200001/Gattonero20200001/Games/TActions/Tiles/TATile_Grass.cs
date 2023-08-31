using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Tiles
{
	public class TATile_Grass : TATile
	{
		public override TATile.Kind_e GetKind()
		{
			return Kind_e.GROUND;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			DD.Draw(Pictures.Grass, drawPt);
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			TATileCommon.DrawSimply(tilePt, drawPt, Pictures.Grass);
		}
	}
}
