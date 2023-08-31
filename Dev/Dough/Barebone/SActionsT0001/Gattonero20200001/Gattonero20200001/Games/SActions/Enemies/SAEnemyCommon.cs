using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.SActions.Shots;

namespace Charlotte.Games.SActions.Enemies
{
	public static class SAEnemyCommon
	{
		/// <summary>
		/// 汎用・被弾イベント
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <param name="shot">被弾した自弾</param>
		/// <param name="damagePoint">削られた体力</param>
		public static void Damaged(SAEnemy enemy, SAShot shot, int damagePoint)
		{
			SoundEffects.EnemyDamaged.Play();
		}

		/// <summary>
		/// 汎用・消滅イベント
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <param name="destroyed">プレイヤー等(の攻撃行動)によって撃破されたか</param>
		public static void Killed(SAEnemy enemy, bool destroyed)
		{
			if (destroyed) // ? 撃破された。
			{
				DD.TL.Add(SCommon.Supplier(SAEffects.Explode(enemy.X, enemy.Y, 3.0)));
				SoundEffects.EnemyKilled.Play();
			}
			else // ? 自滅・消滅 etc.
			{
				DD.TL.Add(SCommon.Supplier(SAEffects.Explode(enemy.X, enemy.Y, 1.0)));
			}
		}
	}
}
