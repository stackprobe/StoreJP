using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Tiles
{
	public class TATile_River : TATile
	{
		public override TATile.Kind_e GetKind()
		{
			return Kind_e.RIVER;
		}

		public override void Draw(I2Point tilePt, D2Point drawPt)
		{
			DD.Draw(Pictures.Grass, drawPt);

			bool stranger8 = !this.IsRiver(tilePt.X, tilePt.Y - 1);
			bool stranger2 = !this.IsRiver(tilePt.X, tilePt.Y + 1);
			bool stranger4 = !this.IsRiver(tilePt.X - 1, tilePt.Y);
			bool stranger6 = !this.IsRiver(tilePt.X + 1, tilePt.Y);

			bool stranger1 = !this.IsRiver(tilePt.X - 1, tilePt.Y + 1);
			bool stranger3 = !this.IsRiver(tilePt.X + 1, tilePt.Y + 1);
			bool stranger7 = !this.IsRiver(tilePt.X - 1, tilePt.Y - 1);
			bool stranger9 = !this.IsRiver(tilePt.X + 1, tilePt.Y - 1);

			int mode_lt;
			int mode_rt;
			int mode_rb;
			int mode_lb;

			if (stranger4 && stranger8)
			{
				mode_lt = 0;
			}
			else if (stranger4)
			{
				mode_lt = 1;
			}
			else if (stranger8)
			{
				mode_lt = 2;
			}
			else if (stranger7)
			{
				mode_lt = 3;
			}
			else
			{
				mode_lt = 4;
			}

			if (stranger6 && stranger8)
			{
				mode_rt = 0;
			}
			else if (stranger6)
			{
				mode_rt = 1;
			}
			else if (stranger8)
			{
				mode_rt = 2;
			}
			else if (stranger9)
			{
				mode_rt = 3;
			}
			else
			{
				mode_rt = 4;
			}

			if (stranger6 && stranger2)
			{
				mode_rb = 0;
			}
			else if (stranger6)
			{
				mode_rb = 1;
			}
			else if (stranger2)
			{
				mode_rb = 2;
			}
			else if (stranger3)
			{
				mode_rb = 3;
			}
			else
			{
				mode_rb = 4;
			}

			if (stranger4 && stranger2)
			{
				mode_lb = 0;
			}
			else if (stranger4)
			{
				mode_lb = 1;
			}
			else if (stranger2)
			{
				mode_lb = 2;
			}
			else if (stranger1)
			{
				mode_lb = 3;
			}
			else
			{
				mode_lb = 4;
			}

			int koma = (DD.ProcFrame / 4) % 8;

			DD.Draw(Pictures.River[0, mode_lt * 2 + 0, koma], drawPt + new D2Point(-8, -8));
			DD.Draw(Pictures.River[1, mode_rt * 2 + 0, koma], drawPt + new D2Point(8, -8));
			DD.Draw(Pictures.River[1, mode_rb * 2 + 1, koma], drawPt + new D2Point(8, 8));
			DD.Draw(Pictures.River[0, mode_lb * 2 + 1, koma], drawPt + new D2Point(-8, 8));
		}

		private bool IsRiver(int x, int y)
		{
			if (
				x < 0 || TAGame.I.Field.Table_W <= x ||
				y < 0 || TAGame.I.Field.Table_H <= y
				)
				return true;

			return TAGame.I.Field.GetTile(new I2Point(x, y)) is TATile_River;
		}

		public override void DrawSimply(I2Point tilePt, D2Point drawPt)
		{
			TATileCommon.DrawSimply(tilePt, drawPt, "川", new I3Color(255, 255, 255), new I3Color(0, 0, 255), new I3Color(0, 128, 255));
		}
	}
}
