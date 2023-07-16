using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.Dungeons
{
	public static class DUCommon
	{
		private static int[] ROT_L_TBL = new int[] { 6, 2, 8, 4 };
		private static int[] ROT_R_TBL = new int[] { 4, 8, 2, 6 };

		/// <summary>
		/// 左を向く。
		/// </summary>
		/// <param name="direction">元の方向(4方向_テンキー方式)</param>
		/// <returns>新しい方向(4方向_テンキー方式)</returns>
		public static int RotL(int direction)
		{
			return ROT_L_TBL[direction / 2 - 1];
		}

		/// <summary>
		/// 右を向く。
		/// </summary>
		/// <param name="direction">元の方向(4方向_テンキー方式)</param>
		/// <returns>新しい方向(4方向_テンキー方式)</returns>
		public static int RotR(int direction)
		{
			return ROT_R_TBL[direction / 2 - 1];
		}
	}
}
