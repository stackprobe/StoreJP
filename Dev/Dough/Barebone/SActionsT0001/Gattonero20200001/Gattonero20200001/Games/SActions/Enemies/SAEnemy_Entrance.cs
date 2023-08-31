using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.SActions.Enemies
{
	public class SAEnemy_Entrance : SAEnemy
	{
		/// <summary>
		/// フィールドに侵入した方向
		/// see: SAGameMaster.IntoDirection
		/// </summary>
		public int IntoDirection;

		public SAEnemy_Entrance(double x, double y, int intoDirection)
			: base(x, y, 0, 0, false)
		{
			this.IntoDirection = intoDirection;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				yield return true;
			}
		}
	}
}
