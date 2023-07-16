using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Shootings.Shots
{
	public static class SHShotCommon
	{
		/// <summary>
		/// 汎用・消滅イベント
		/// </summary>
		/// <param name="shot">自弾</param>
		public static void Killed(SHShot shot)
		{
			DD.TL.Add(SCommon.Supplier(SHEffects.Explode(shot.X, shot.Y, 0.5)));
		}
	}
}
