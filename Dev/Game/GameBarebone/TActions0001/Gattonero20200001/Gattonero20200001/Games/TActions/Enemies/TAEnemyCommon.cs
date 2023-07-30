using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Shots;

namespace Charlotte.Games.TActions.Enemies
{
	public static class TAEnemyCommon
	{
		/// <summary>
		/// 汎用・被弾イベント
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <param name="shot">被弾した自弾</param>
		/// <param name="damagePoint">削られた体力</param>
		public static void Damaged(TAEnemy enemy, TAShot shot, int damagePoint)
		{
			SoundEffects.EnemyDamaged.Play();
		}

		/// <summary>
		/// 汎用・消滅イベント
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <param name="destroyed">プレイヤー等(の攻撃行動)によって撃破されたか</param>
		public static void Killed(TAEnemy enemy, bool destroyed)
		{
			if (destroyed) // ? 撃破された。
			{
				DD.TL.Add(SCommon.Supplier(TAEffects.Explode(enemy.X, enemy.Y, 3.0)));
				SoundEffects.EnemyKilled.Play();
			}
			else // ? 自滅・消滅 etc.
			{
				DD.TL.Add(SCommon.Supplier(TAEffects.Explode(enemy.X, enemy.Y, 1.0)));
			}
		}
	}
}
