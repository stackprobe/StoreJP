using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.TActions.Enemies
{
	public class TAEnemy_None : TAEnemy
	{
		public TAEnemy_None()
			: base(0.0, 0.0, 0, 0, false)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			yield break;
		}
	}
}
