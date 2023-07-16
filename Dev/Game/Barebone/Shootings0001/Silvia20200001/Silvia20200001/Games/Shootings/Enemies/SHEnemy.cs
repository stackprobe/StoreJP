using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Shootings.Shots;

namespace Charlotte.Games.Shootings.Enemies
{
	/// <summary>
	/// 敵
	/// </summary>
	public abstract class SHEnemy
	{
		// memo:
		// プレイヤーに撃破されない(自弾に当たらない)敵を作る場合 HP == 0 にすること。
		// -- アイテム・敵弾など
		// プレイヤーに当たらない敵を作る場合 E_Draw において Crash を設定しないこと。
		// -- アイテム・イベントなど
		// -- プレイヤーによるアイテムの取得(プレイヤーとアイテムの当たり判定)はアイテム側が自力で行うこと。

		public double X;
		public double Y;

		/// <summary>
		/// 体力
		/// 0 == 無敵
		/// -1 == 死亡
		/// </summary>
		public int HP;

		public enum Kind_e
		{
			通常敵 = 1,
			アイテム,
			ボス,
		}

		public Kind_e Kind;

		// 敵：
		// -- new Enemy(x, y, hp, Kind_e.通常敵)
		// 敵弾：
		// -- new Enemy(x, y, 0, Kind_e.通常敵)
		// アイテム：
		// -- new Enemy(x, y, 0, Kind_e.アイテム) -- 当たり判定を設置しない。
		// ボス：
		// -- new Enemy(x, y, hp, Kind_e.ボス敵)

		public SHEnemy(double x, double y, int hp, Kind_e kind)
		{
			this.X = x;
			this.Y = y;
			this.HP = hp;
			this.Kind = kind;
		}

		/// <summary>
		/// この敵を消滅させるか
		/// 撃破された場合、画面外に出た場合などこの敵を消滅させたい場合 true をセットすること。
		/// これにより「フレームの最後に」敵リストから除去される。
		/// </summary>
		public bool DeadFlag
		{
			set
			{
				if (value)
					this.HP = -1;
				else
					throw null; // never
			}

			get
			{
				return this.HP == -1;
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
		/// -- 当たり判定を設定する。-- プレイヤーに当たらないなら設定しない。
		/// -- 必要に応じて敵を追加する。== Game.I.Enemies.Add(shot);
		/// -- 必要に応じて敵(自分自身)を削除する。== this.DeadFlag = true; または this.Kill(); または yield return false;
		/// ---- 敵(自分以外)を削除するには anotherEnemy.DeadFlag = true; または anotherEnemy.Kill();
		/// </summary>
		/// <returns>タスク：この敵は生存しているか</returns>
		protected abstract IEnumerable<bool> E_Draw();

		/// <summary>
		/// 被弾した。
		/// 体力の減少などは呼び出し側でやっている。
		/// -- 自弾の攻撃力は既に引かれていることに注意！
		/// </summary>
		/// <param name="shot">この敵が被弾したプレイヤーの弾</param>
		/// <param name="damagePoint">削られた体力</param>
		public void Damaged(SHShot shot, int damagePoint)
		{
			this.P_Damaged(shot, damagePoint);
		}

		/// <summary>
		/// この敵の固有の被弾イベント
		/// </summary>
		/// <param name="shot">この敵が被弾したプレイヤーの弾</param>
		/// <param name="damagePoint">削られた体力</param>
		protected virtual void P_Damaged(SHShot shot, int damagePoint)
		{
			SHEnemyCommon.Damaged(this, shot, damagePoint);
		}

		/// <summary>
		/// Killed 複数回実行回避のため、DeadFlag をチェックして Killed を実行する。
		/// 注意：HP を減らして -1 になったとき Kill を呼んでも(DeadFlag == true になるため) Killed は実行されない！
		/// -- HP == -1 の可能性がある場合は HP = 0; を忘れずに！
		/// </summary>
		/// <param name="destroyed">プレイヤー等(の攻撃行動)によって撃破されたか</param>
		public void Kill(bool destroyed = false)
		{
			if (!this.DeadFlag)
			{
				this.DeadFlag = true;
				this.Killed(destroyed);
			}
		}

		/// <summary>
		/// 撃破されて消滅した。
		/// マップから離れすぎて消された場合・シナリオ的に消された場合などでは呼び出されない。
		/// </summary>
		/// <param name="destroyed">プレイヤー等(の攻撃行動)によって撃破されたか</param>
		private void Killed(bool destroyed)
		{
			this.P_Killed(destroyed);
		}

		/// <summary>
		/// この敵の固有の消滅イベント
		/// </summary>
		/// <param name="destroyed">プレイヤー等(の攻撃行動)によって撃破されたか</param>
		protected virtual void P_Killed(bool destroyed)
		{
			SHEnemyCommon.Killed(this, destroyed);
		}
	}
}
