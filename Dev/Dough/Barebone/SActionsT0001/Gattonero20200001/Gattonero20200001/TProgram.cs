using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Games;
using Charlotte.Games.SActions;

namespace Charlotte
{
	public static class TProgram
	{
		public static void Run()
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4();
			}
		}

		private static void Main3()
		{
			// テスト系 -- リリース版では使用しない。
#if DEBUG
			// -- choose one --

			//Logo.Run();
			TitleMenu.Run();
			//using (new SAGameMaster()) { GameStatus.I.Cheat(); SAGameMaster.I.Run("Test0001"); }

			// --
#endif
		}

		private static void Main4()
		{
			Logo.Run();
		}
	}
}
