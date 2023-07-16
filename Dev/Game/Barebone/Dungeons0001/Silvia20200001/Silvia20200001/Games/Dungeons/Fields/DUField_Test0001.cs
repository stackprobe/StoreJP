using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Dungeons.Fields
{
	public class DUField_Test0001 : DUField
	{
		#region Resource

		private static readonly string RES_MAP = @"

+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
|  |        |        |     G  |        G  |
+  +GG+  +--+  +--+  +  +--+  +--+--+--+  +
|  |  |  |     |  G  G  |  |              |
+  +--+  +--+  +--+  +  +  +  +  +--+--+--+
|  |        G  |     |  |  |     |        |
+  +GG+--+--+  +--+--+  +  +  +--+  +  +--+
|                    |  |  |  |        G  |
+--+--+--+--+  +--+  +  +  +  +GG+  +  +--+
|  G        |  |  |  |  |  |  G  |        |
+--+  +GG+  +--+  +--+  +GG+  +--+--+--+--+
|     |  |              |           G     |
+GG+--+--+--+--+--+--+  +  +  +  +  +  +  +
|                    |  |           G     |
+--+--+--+--+--+--+--+GG+--+--+--+--+--+--+
|                                         |
+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

";

		#endregion

		public static DUField_Test0001 Create()
		{
			string[] lines = SCommon.TextToLines(RES_MAP.Trim());

			int w = lines[0].Length;
			int h = lines.Length;

			if (lines.Any(line => line.Length != w))
				throw null;

			if (w < 4 || w % 3 != 1)
				throw null;

			if (h < 3 || h % 2 != 1)
				throw null;

			w /= 3;
			h /= 2;

			Tile_t[,] table = new Tile_t[w, h];

			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					string s1 = lines[y * 2 + 2].Substring(x * 3 + 0, 1);
					string s2 = lines[y * 2 + 2].Substring(x * 3 + 1, 2);
					string s3 = lines[y * 2 + 2].Substring(x * 3 + 3, 1);
					string s4 = lines[y * 2 + 1].Substring(x * 3 + 0, 1);
					string s5 = lines[y * 2 + 1].Substring(x * 3 + 1, 2);
					string s6 = lines[y * 2 + 1].Substring(x * 3 + 3, 1);
					string s7 = lines[y * 2 + 0].Substring(x * 3 + 0, 1);
					string s8 = lines[y * 2 + 0].Substring(x * 3 + 1, 2);
					string s9 = lines[y * 2 + 0].Substring(x * 3 + 3, 1);

					if (s1 != "+")
						throw null;

					if (s3 != "+")
						throw null;

					if (s7 != "+")
						throw null;

					if (s9 != "+")
						throw null;

					Tile_t tile = new Tile_t();

					if (s8 == "  ")
						tile.Dir8.Mode = 0;
					else if (s8 == "--")
						tile.Dir8.Mode = 1;
					else if (s8 == "GG")
						tile.Dir8.Mode = 2;
					else
						throw null;

					if (s2 == "  ")
						tile.Dir2.Mode = 0;
					else if (s2 == "--")
						tile.Dir2.Mode = 1;
					else if (s2 == "GG")
						tile.Dir2.Mode = 2;
					else
						throw null;

					if (s4 == " ")
						tile.Dir4.Mode = 0;
					else if (s4 == "|")
						tile.Dir4.Mode = 1;
					else if (s4 == "G")
						tile.Dir4.Mode = 2;
					else
						throw null;

					if (s6 == " ")
						tile.Dir6.Mode = 0;
					else if (s6 == "|")
						tile.Dir6.Mode = 1;
					else if (s6 == "G")
						tile.Dir6.Mode = 2;
					else
						throw null;

					table[x, y] = tile;
				}
			}
			return new DUField_Test0001(w, h, table);
		}

		private struct TileDirection_t
		{
			/// <summary>
			/// モード
			/// 値：
			/// -- 0 == 空間
			/// -- 1 == 壁
			/// -- 2 == ゲート
			/// </summary>
			public int Mode;
		}

		private struct Tile_t
		{
			public TileDirection_t Dir2;
			public TileDirection_t Dir4;
			public TileDirection_t Dir6;
			public TileDirection_t Dir8;
		}

		private Tile_t[,] Table;

		private DUField_Test0001(int w, int h, Tile_t[,] table)
			: base(new I2Size(w, h))
		{
			this.Table = table;
		}

		public override void Initialize()
		{
			DUGame.I.X = SCommon.CRandom.GetInt(this.Table_W);
			DUGame.I.Y = SCommon.CRandom.GetInt(this.Table_H);
			DUGame.I.Direction = SCommon.CRandom.GetInt(4) * 2 + 2;

			Musics.SunBeams.Play();
		}

		protected override int P_GetWall(I2Point tilePt, int direction)
		{
			Tile_t tile = this.Table[tilePt.X, tilePt.Y];
			TileDirection_t tileDirection;

			switch (direction)
			{
				case 2: tileDirection = tile.Dir2; break;
				case 4: tileDirection = tile.Dir4; break;
				case 6: tileDirection = tile.Dir6; break;
				case 8: tileDirection = tile.Dir8; break;

				default:
					throw null; // never
			}
			return tileDirection.Mode;
		}

		public override Picture GetBackgroundPicture()
		{
			return Pictures.DungeonBackground;
		}

		public override Picture GetWallPicture()
		{
			return Pictures.DungeonWall;
		}

		public override Picture GetGatePicture()
		{
			return Pictures.DungeonGate;
		}
	}
}
