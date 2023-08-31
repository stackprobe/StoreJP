using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Tiles
{
	public class TATile_Tree : TATile
	{
		public override TATile.Kind_e GetKind()
		{
			return Kind_e.WALL;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			DD.Draw(Pictures.Grass, drawPt);

			bool drawed = false;

			if ((tilePt.X + tilePt.Y) % 2 == 0)
			{
				if (this.IsWall2x2(tilePt.X - 1, tilePt.Y - 1))
				{
					DD.Draw(Pictures.Tree[4], drawPt);
					drawed = true;
				}
				if (this.IsWall2x2(tilePt.X, tilePt.Y))
				{
					DD.Draw(Pictures.Tree[1], drawPt);
					drawed = true;
				}
			}
			else
			{
				if (this.IsWall2x2(tilePt.X, tilePt.Y - 1))
				{
					DD.Draw(Pictures.Tree[2], drawPt);
					drawed = true;
				}
				if (this.IsWall2x2(tilePt.X - 1, tilePt.Y))
				{
					DD.Draw(Pictures.Tree[3], drawPt);
					drawed = true;
				}
			}
			if (!drawed)
			{
				DD.Draw(Pictures.Tree[0], drawPt);
			}
		}

		private bool IsWall2x2(int x, int y)
		{
			return
				this.IsWall(x + 0, y + 0) &&
				this.IsWall(x + 0, y + 1) &&
				this.IsWall(x + 1, y + 0) &&
				this.IsWall(x + 1, y + 1);
		}

		private bool IsWall(int x, int y)
		{
			if (
				x < 0 || TAGame.I.Field.Table_W <= x ||
				y < 0 || TAGame.I.Field.Table_H <= y
				)
				return true;

			return TAGame.I.Field.GetTile(new I2Point(x, y)) is TATile_Tree;
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			TATileCommon.DrawSimply(tilePt, drawPt, "木", new I3Color(255, 255, 255), new I3Color(0, 128, 0), new I3Color(0, 255, 0));
		}
	}
}
