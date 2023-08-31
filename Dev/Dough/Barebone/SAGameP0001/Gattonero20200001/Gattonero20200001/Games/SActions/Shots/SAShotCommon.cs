using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
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

		public static bool Is自弾とプレイヤーの間には壁がある(SAShot shot, bool 水平方向に進む自弾)
		{
			D2Point a;
			D2Point b;

			if (水平方向に進む自弾)
			{
				a = new D2Point(SAGame.I.Player.X, shot.Y);
				b = new D2Point(shot.X, shot.Y);
			}
			else // ? 垂直方向に進む自弾
			{
				a = new D2Point(shot.X, SAGame.I.Player.Y);
				b = new D2Point(shot.X, shot.Y);
			}
			int div = (int)(DD.GetDistance(a, b) * 1.5 / SAConsts.TILE_W);

			div = Math.Max(div, 3);

			foreach (Scene scene in Scene.Create(div))
			{
				D2Point pt = DD.AToBRate(a, b, scene.Rate);

				if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(pt)).IsWall())
					return true;
			}
			return false;
		}
	}
}
