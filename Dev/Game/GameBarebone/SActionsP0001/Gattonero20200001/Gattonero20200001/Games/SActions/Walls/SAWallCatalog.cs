using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.SActions.Walls
{
	public class SAWallCatalog
	{
		private static SAWallCatalog _i = null;

		public static SAWallCatalog I
		{
			get
			{
				if (_i == null)
					_i = new SAWallCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<SAWall>> Creators = SCommon.CreateDictionary<Func<SAWall>>();

		private SAWallCatalog()
		{
			AddItems();
		}

		private void AddItems()
		{
			AddItem("Test0001", () => new SAWall_Test0001());
			AddItem("Test0002", () => new SAWall_Test0002());
			AddItem("Test0003", () => new SAWall_Test0003());
		}

		private void AddItem(string name, Func<SAWall> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public SAWall CreateWall(string name)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			SAWall wall = this.Creators[name]();
			return wall;
		}
	}
}
