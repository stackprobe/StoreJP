using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.TActions.Enemies
{
	/// <summary>
	/// テスト用_敵
	/// </summary>
	public class TAEnemy_Test0002 : TAEnemy_Test0001
	{
		public TAEnemy_Test0002(double x, double y)
			: base(x, y)
		{ }

		protected override I3Color GetColor()
		{
			return new I3Color(0, 255, 255);
		}
	}
}
