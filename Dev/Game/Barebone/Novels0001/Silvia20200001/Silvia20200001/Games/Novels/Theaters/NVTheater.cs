using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.Novels.Theaters
{
	/// <summary>
	/// シアター・上映装置
	/// </summary>
	public abstract class NVTheater
	{
		/// <summary>
		/// ノベルパートに入るときのイベント・シーン・処理
		/// </summary>
		public abstract void Enter();

		/// <summary>
		/// ノベルパートから出るときのイベント・シーン・処理
		/// </summary>
		public abstract void Leave();

		/// <summary>
		/// シナリオ中のコマンドを処理する。
		/// -- このシアター固有
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <param name="arguments">パラメータ列</param>
		public abstract void Invoke(string command, string[] arguments);

		private Func<bool> _draw = null;

		public void Draw()
		{
			if (_draw == null)
				_draw = SCommon.Supplier(this.E_Draw());

			if (!_draw())
				throw null; // never
		}

		/// <summary>
		/// 描画を行う。
		/// </summary>
		/// <returns>タスク：常に真</returns>
		protected abstract IEnumerable<bool> E_Draw();
	}
}
