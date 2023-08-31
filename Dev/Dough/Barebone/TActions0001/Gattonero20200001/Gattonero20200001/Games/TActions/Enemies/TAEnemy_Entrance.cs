using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.TActions.Enemies
{
	public class TAEnemy_Entrance : TAEnemy
	{
		/// <summary>
		/// フィールドに侵入した方向
		/// see: TAGameMaster.IntoDirection
		/// </summary>
		public int IntoDirection;

		public TAEnemy_Entrance(double x, double y, int intoDirection)
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
