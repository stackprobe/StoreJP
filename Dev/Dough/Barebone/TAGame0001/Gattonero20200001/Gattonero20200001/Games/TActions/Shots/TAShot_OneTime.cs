using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Shots
{
	/// <summary>
	/// コンストラクタに指定された当たり判定を、このフレームだけ適用する。
	/// 手持ち武器を振り回した時の当たり判定などを想定
	/// -- 自機からの攻撃の当たり判定を自弾に設定する仕組みのため、自弾を経由する必要がある。
	/// </summary>
	public class TAShot_OneTime : TAShot
	{
		private Crash OneTimeCrash;

		public TAShot_OneTime(int attackPoint, Crash crash)
			: base(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2, attackPoint, true)
		{
			this.OneTimeCrash = crash;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			this.Crash = this.OneTimeCrash;

			// このフレームで生き残るために1回だけ真を返す。
			// -- 自弾が死亡するとクラッシュが無視される。
			yield return true;
		}
	}
}
