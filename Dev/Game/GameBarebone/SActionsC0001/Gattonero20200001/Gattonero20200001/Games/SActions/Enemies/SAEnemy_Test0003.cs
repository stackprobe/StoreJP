using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Enemies
{
	/// <summary>
	/// テスト用_敵
	/// </summary>
	public class SAEnemy_Test0003 : SAEnemy_Test0001
	{
		public SAEnemy_Test0003(double x, double y)
			: base(x, y)
		{ }

		protected override I3Color GetColor()
		{
			return new I3Color(255, 0, 255);
		}
	}
}
