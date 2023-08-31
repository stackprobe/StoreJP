using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.TActions.Tiles
{
	public class TATileCatalog
	{
		private static TATileCatalog _i = null;

		public static TATileCatalog I
		{
			get
			{
				if (_i == null)
					_i = new TATileCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<TATile>> Creators = SCommon.CreateDictionary<Func<TATile>>();

		private TATileCatalog()
		{
			AddItems();
		}

		public readonly static string DEFAULT_TILE_NAME = "Grass";

		private void AddItems()
		{
			AddItem(DEFAULT_TILE_NAME, () => new TATile_Grass());
			AddItem("River", () => new TATile_River());
			AddItem("Tree", () => new TATile_Tree());
		}

		private void AddItem(string name, Func<TATile> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public TATile CreateTile(string name)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			TATile tile = this.Creators[name]();
			return tile;
		}
	}
}
