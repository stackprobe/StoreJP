using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	public static class GameMain
	{
		// MEMO: 各種クラス・メソッドの使い方は 20230999_DevBin/20230930_DevDevLabo を参照して下さい。

		public static void Run()
		{
			for (; ; )
			{
				if (Inputs.START.GetInput() == 1)
					break;

				DD.EachFrame();
			}
		}
	}
}
