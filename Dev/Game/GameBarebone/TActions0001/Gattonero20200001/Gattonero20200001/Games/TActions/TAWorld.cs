using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public class TAWorld
	{
		private static TAWorld _i = null;

		public static TAWorld I
		{
			get
			{
				if (_i == null)
					_i = new TAWorld();

				return _i;
			}
		}

		private const string WORLD_RES_FILE = "Field\\TA\\00_World.csv";

		private string[][] Rows;
		private Dictionary<string, I2Point> Name2Position = SCommon.CreateDictionary<I2Point>();

		private TAWorld()
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string file = wd.MakePath();

				File.WriteAllBytes(file, DD.GetResFileData(WORLD_RES_FILE).Data.Value);

				using (CsvFileReader reader = new CsvFileReader(file))
				{
					this.Rows = reader.ReadToEnd();
				}
			}

			for (int y = 0; y < this.Rows.Length; y++)
			{
				for (int x = 0; x < this.Rows[y].Length; x++)
				{
					string name = this.Rows[y][x];

					if (name != "")
					{
						this.Name2Position.Add(name, new I2Point(x, y));
					}
				}
			}
		}

		public string GetNextFieldName(string name, int direction)
		{
			if (string.IsNullOrEmpty(name))
				throw new Exception("Bad name");

			if (!this.Name2Position.ContainsKey(name))
				throw new Exception("Unknown name: " + name);

			I2Point fieldPt = this.Name2Position[name];

			switch (direction)
			{
				case 4:
					fieldPt.X--;
					break;

				case 6:
					fieldPt.X++;
					break;

				case 8:
					fieldPt.Y--;
					break;

				case 2:
					fieldPt.Y++;
					break;

				default:
					throw null; // never
			}
			name = this.Rows[fieldPt.Y][fieldPt.X];

			if (string.IsNullOrEmpty(name))
				throw new Exception("Bad next name");

			return name;
		}
	}
}
