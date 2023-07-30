using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Tiles
{
	public class SATile_地中 : SATile
	{
		public override bool IsWall()
		{
			return true;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			Picture picture;

			if (tilePt.Y < 1 || SAGame.I.Field.GetTile(new I2Point(tilePt.X, tilePt.Y - 1)) is SATile_地中)
				picture = Pictures.地中;
			else
				picture = Pictures.地面;

			DD.Draw(picture, drawPt);
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			SATileCommon.DrawSimply(tilePt, drawPt, Pictures.地中);
		}
	}
}
