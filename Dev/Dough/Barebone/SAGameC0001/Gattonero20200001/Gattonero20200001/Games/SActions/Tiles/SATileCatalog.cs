using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Tiles
{
	public class SATileCatalog
	{
		private static SATileCatalog _i = null;

		public static SATileCatalog I
		{
			get
			{
				if (_i == null)
					_i = new SATileCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<SATile>> Creators = SCommon.CreateDictionary<Func<SATile>>();

		private SATileCatalog()
		{
			AddItems();
		}

		public readonly static string DEFAULT_TILE_NAME = "None";

		private void AddItems()
		{
			AddItem(DEFAULT_TILE_NAME, () => new SATile_None());
			AddItem("石壁", () => new SATile_石壁());
			AddItem("地中", () => new SATile_地中());
		}

		private void AddItem(string name, Func<SATile> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public SATile CreateTile(string name)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			SATile tile = this.Creators[name]();
			return tile;
		}
	}
}
