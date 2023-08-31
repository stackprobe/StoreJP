using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions
{
	public class SAMusicCatalog
	{
		private static SAMusicCatalog _i = null;

		public static SAMusicCatalog I
		{
			get
			{
				if (_i == null)
					_i = new SAMusicCatalog();

				return _i;
			}
		}

		private List<string> Names = new List<string>();
		private Dictionary<string, Music> Name2Music = SCommon.CreateDictionary<Music>();

		private SAMusicCatalog()
		{
			AddItems();
		}

		private void AddItems()
		{
			AddItem("EverlastingSnow", Musics.EverlastingSnow);
			AddItem("WanderersCity", Musics.WanderersCity);
			AddItem("SilentAvalon", Musics.SilentAvalon);
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
