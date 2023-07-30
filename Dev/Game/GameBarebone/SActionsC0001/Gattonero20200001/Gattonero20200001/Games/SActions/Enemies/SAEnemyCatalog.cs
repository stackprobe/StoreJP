using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Enemies
{
	public class SAEnemyCatalog
	{
		private static SAEnemyCatalog _i = null;

		public static SAEnemyCatalog I
		{
			get
			{
				if (_i == null)
					_i = new SAEnemyCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Func<SAEnemy>> Creators = SCommon.CreateDictionary<Func<SAEnemy>>();

		private static double X = default(double);
		private static double Y = default(double);

		private SAEnemyCatalog()
		{
			AddItems();
		}

		public readonly static string DEFAULT_ENEMY_NAME = "None";

		private void AddItems()
		{
			AddItem(DEFAULT_ENEMY_NAME, () => new SAEnemy_None());
			AddItem("Entrance(West)", () => new SAEnemy_Entrance(X, Y, 4));
			AddItem("Entrance(East)", () => new SAEnemy_Entrance(X, Y, 6));
			AddItem("Entrance(North)", () => new SAEnemy_Entrance(X, Y, 8));
			AddItem("Entrance(South)", () => new SAEnemy_Entrance(X, Y, 2));
			AddItem("Entrance(Center)", () => new SAEnemy_Entrance(X, Y, 5));
			AddItem("SavePoint", () => new SAEnemy_SavePoint(X, Y));
			AddItem("Test0001", () => new SAEnemy_Test0001(X, Y));
			AddItem("Test0002", () => new SAEnemy_Test0002(X, Y));
			AddItem("Test0003", () => new SAEnemy_Test0003(X, Y));
		}

		private void AddItem(string name, Func<SAEnemy> creator)
		{
			this.Names.Add(name);
			this.Creators.Add(name, creator);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public SAEnemy CreateEnemy(string name, double x, double y)
		{
			if (!this.Creators.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			X = x;
			Y = y;

			SAEnemy enemy = this.Creators[name]();

			X = default(double);
			Y = default(double);

			return enemy;
		}
	}
}
