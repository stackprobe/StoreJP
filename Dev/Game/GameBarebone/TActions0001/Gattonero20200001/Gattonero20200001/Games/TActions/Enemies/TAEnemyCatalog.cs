using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.TActions.Enemies
{
	public class TAEnemyCatalog
	{
		private static TAEnemyCatalog _i = null;

		public static TAEnemyCatalog I
		{
			get
			{
				if (_i == null)
					_i = new TAEnemyCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<TAEnemy>> Creators = SCommon.CreateDictionary<Func<TAEnemy>>();

		private static double X = default(double);
		private static double Y = default(double);

		private TAEnemyCatalog()
		{
			AddItems();
		}

		public readonly static string DEFAULT_ENEMY_NAME = "None";

		private void AddItems()
		{
			AddItem(DEFAULT_ENEMY_NAME, () => new TAEnemy_None());
			AddItem("Entrance(West)", () => new TAEnemy_Entrance(X, Y, 4));
			AddItem("Entrance(East)", () => new TAEnemy_Entrance(X, Y, 6));
			AddItem("Entrance(North)", () => new TAEnemy_Entrance(X, Y, 8));
			AddItem("Entrance(South)", () => new TAEnemy_Entrance(X, Y, 2));
			AddItem("Entrance(Center)", () => new TAEnemy_Entrance(X, Y, 5));
			AddItem("SavePoint", () => new TAEnemy_SavePoint(X, Y));
			AddItem("Test0001", () => new TAEnemy_Test0001(X, Y));
			AddItem("Test0002", () => new TAEnemy_Test0002(X, Y));
			AddItem("Test0003", () => new TAEnemy_Test0003(X, Y));
		}

		private void AddItem(string name, Func<TAEnemy> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public TAEnemy CreateEnemy(string name, double x, double y)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			X = x;
			Y = y;

			TAEnemy enemy = this.Creators[name]();

			X = default(double);
			Y = default(double);

			return enemy;
		}
	}
}
