using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Shootings
{
	public static class SHCommon
	{
		public static bool IsOut(D2Point pt, D4Rect rect, double margin = 0.0)
		{
			return !Crash.IsCrashed_Circle_Rect(pt, margin, rect);
		}

		public static bool IsOutOfScreen(D2Point pt, double margin = 0.0)
		{
			return IsOut(pt, new I4Rect(0, 0, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect(), margin);
		}
	}
}
