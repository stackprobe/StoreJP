using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Walls
{
	public class TAWall_None : TAWall
	{
		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				yield return true;
			}
		}
	}
}
