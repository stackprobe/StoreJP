using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.TActions.Walls
{
	public class TAWallCatalog
	{
		private static TAWallCatalog _i = null;

		public static TAWallCatalog I
		{
			get
			{
				if (_i == null)
					_i = new TAWallCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<TAWall>> Creators = SCommon.CreateDictionary<Func<TAWall>>();

		private TAWallCatalog()
		{
			AddItems();
		}

		private void AddItems()
		{
			AddItem("None", () => new TAWall_None());
		}

		private void AddItem(string name, Func<TAWall> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public TAWall CreateWall(string name)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			TAWall wall = this.Creators[name]();
			return wall;
		}
	}
}
