using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Attacks
{
	/// <summary>
	/// アタック
	/// プレイヤーの攻撃モーション
	/// その他プレイヤーの特殊なモーション
	/// </summary>
	public abstract class TAAttack
	{
		private Func<bool> _eachFrame = null;

		public bool EachFrame()
		{
			if (1 <= TAGame.I.Player.DamageFrame) // ダメージを受けたら即終了
				return false;

			if (_eachFrame == null)
				_eachFrame = SCommon.Supplier(this.E_Draw());

			return _eachFrame();
		}

		/// <summary>
		/// プレイヤーの攻撃モーション
		/// 毎フレーム実行する。
		/// 偽を返すまで、プレイヤーの入力・被弾処理などは実行されない。
		/// -- 偽を返したフレームから再開される。
		/// 描画も行うこと。
		/// -- 描画は以下に対して行うこと。
		/// ---- TAGame.I.Player.Draw_TL
		/// </summary>
		/// <returns>タスク：このモーションを継続するか</returns>
		protected abstract IEnumerable<bool> E_Draw();
	}
}
