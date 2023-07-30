using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte
{
	public static class SoundEffects
	{
		public static SoundEffect Dummy = new SoundEffect(@"Handmade\Muon1s.wav");

		public static SoundEffect Save = new SoundEffect(@"SoundEffect\ユーフルカ\save-01.wav");
		public static SoundEffect Load = new SoundEffect(@"SoundEffect\ユーフルカ\load.wav");
		public static SoundEffect Buy = new SoundEffect(@"SoundEffect\ユーフルカ\shop.wav");

		public static SoundEffect PlayerDamaged = new SoundEffect(@"SoundEffect\小森平\powerdown03.mp3");
		public static SoundEffect EnemyDamaged = new SoundEffect(@"SoundEffect\小森平\damage5.mp3");
		public static SoundEffect EnemyKilled = new SoundEffect(@"SoundEffect\小森平\game_explosion6.mp3");
	}
}
