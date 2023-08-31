using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Shots
{
	public static class SAShotCommon
	{
		/// <summary>
		/// 汎用・消滅イベント
		/// </summary>
		/// <param name="shot">自弾</param>
		public static void Killed(SAShot shot)
		{
			DD.TL.Add(SCommon.Supplier(SAEffects.Explode(shot.X, shot.Y, 1.0)));
		}
	}
}
