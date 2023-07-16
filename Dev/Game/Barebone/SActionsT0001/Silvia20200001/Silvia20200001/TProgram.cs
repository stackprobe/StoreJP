using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Games;
using Charlotte.Games.SActions;
using Charlotte.Games.SActions.Fields;

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
			//TitleMenu.Run();
			using (new SAGameMaster()) { SAGameMaster.I.Run(SAField_Test0001.Create(0)); }

			// --
#endif
		}

		private static void Main4()
		{
			Logo.Run();
		}
	}
}
