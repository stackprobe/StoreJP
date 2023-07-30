using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.SActions.Enemies
{
	public class SAEnemy_None : SAEnemy
	{
		public SAEnemy_None()
			: base(0.0, 0.0, 0, 0, false)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			yield break;
		}
	}
}
