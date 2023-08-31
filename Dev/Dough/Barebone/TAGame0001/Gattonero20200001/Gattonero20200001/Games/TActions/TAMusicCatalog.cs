using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public class TAMusicCatalog
	{
		private static TAMusicCatalog _i = null;

		public static TAMusicCatalog I
		{
			get
			{
				if (_i == null)
					_i = new TAMusicCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Music> Name2Music = SCommon.CreateDictionary<Music>();

		private TAMusicCatalog()
		{
			AddItems();
		}

		private void AddItems()
		{
			AddItem("WanderersCity", Musics.WanderersCity);
			AddItem("SunBeams", Musics.SunBeams);
		}

		private void AddItem(string name, Music music)
		{
			this.Names.Add(name);
			this.Name2Music.Add(name, music);
		}

		public IList<string> GetNames()
		{
			return this.Names;
		}

		public Music GetMusic(string name)
		{
			if (!this.Name2Music.ContainsKey(name))
				throw new Exception("Bad name: " + name);

			Music music = this.Name2Music[name];
			return music;
		}
	}
}
