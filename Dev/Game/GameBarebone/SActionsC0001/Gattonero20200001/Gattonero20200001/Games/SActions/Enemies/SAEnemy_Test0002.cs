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
	public class SAEnemy_Test0002 : SAEnemy_Test0001
	{
		public SAEnemy_Test0002(double x, double y)
			: base(x, y)
		{ }

		protected override I3Color GetColor()
		{
			return new I3Color(0, 255, 255);
		}
	}
}
