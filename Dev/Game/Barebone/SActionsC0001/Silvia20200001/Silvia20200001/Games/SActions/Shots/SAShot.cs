﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Shots
{
	/// <summary>
	/// 自弾(プレイヤーの弾)
	/// </summary>
	public abstract class SAShot
	{
		public double X;
		public double Y;

		/// <summary>
		/// 攻撃力
		/// -1 == 死亡
		/// 1～ == 攻撃力
		/// </summary>
		public int AttackPoint;

		public bool 敵を貫通する;

		public SAShot(double x, double y, int attackPoint, bool 敵を貫通する)
		{
			this.X = x;
			this.Y = y;
			this.AttackPoint = attackPoint;
			this.敵を貫通する = 敵を貫通する;
		}

		/// <summary>
		/// この自弾を消滅させるか
		/// 敵に当たった場合、画面外に出た場合などこの自弾を消滅させたい場合 true をセットすること。
		/// これにより「フレームの最後に」自弾リストから除去される。
		/// </summary>
		public bool DeadFlag
		{
			set
			{
				if (value)
					this.AttackPoint = -1;
				else
					throw null; // never
			}

			get
			{
				return this.AttackPoint == -1;
			}
		}

		/// <summary>
		/// 現在のフレームにおける当たり判定を保持する。
		/// -- 描画(E_Draw)によって設定される。
		/// </summary>
		public Crash Crash = Crash.CreateNone();

		private Func<bool> _draw = null;

		public void Draw()
		{
			if (_draw == null)
				_draw = SCommon.Supplier(this.E_Draw());

			if (!_draw())
				this.DeadFlag = true;
		}

		/// <summary>
		/// 現在のフレームにおける描画を行う。
		/// するべきこと：
		/// -- 行動・移動
		/// -- 描画
		/// -- 当たり判定を設定する。-- 敵に当たらないなら設定しない。
		/// -- 必要に応じて自弾を追加する。== Game.I.Shots.Add(shot);
		/// -- 必要に応じて自弾(自分自身)を削除する。== this.DeadFlag = true; または this.Kill(); または yield return false;
		/// ---- 自弾(自分以外)を削除するには anotherShot.DeadFlag = true; または anotherShot.Kill();
		/// </summary>
		/// <returns>タスク：この自弾は生存しているか</returns>
		protected abstract IEnumerable<bool> E_Draw();

		/// <summary>
		/// この自弾を消滅させる。
		/// Killed 複数回実行回避のため、DeadFlag をチェックして Killed を実行する。
		/// </summary>
		public void Kill()
		{
			if (!this.DeadFlag)
			{
				this.DeadFlag = true;
				this.Killed();
			}
		}

		/// <summary>
		/// 何かと衝突して消滅した。
		/// フィールドから離れすぎて消された場合・シナリオ的に消された場合などでは呼び出されない。
		/// </summary>
		private void Killed()
		{
			this.P_Killed();
		}

		/// <summary>
		/// この自弾の固有の消滅イベント
		/// </summary>
		protected virtual void P_Killed()
		{
			SAShotCommon.Killed(this);
		}
	}
}
