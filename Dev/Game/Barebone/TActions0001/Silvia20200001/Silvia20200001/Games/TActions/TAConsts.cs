using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.TActions
{
	public static class TAConsts
	{
		// タイルのサイズ(ドット単位)
		//
		public const int TILE_W = 32;
		public const int TILE_H = 32;

		public const int MINI_TILE_W = 16;
		public const int MINI_TILE_H = 16;

		public const int PLAYER_DAMAGE_FRAME_MAX = 5;
		public const int PLAYER_INVINCIBLE_FRAME_MAX = 60;

		public const double PLAYER_SPEED = 6.0;
		public const double PLAYER_SLOW_SPEED = 3.0;

		public const double ROOT_OF_2 = 1.414213562373;
	}
}
