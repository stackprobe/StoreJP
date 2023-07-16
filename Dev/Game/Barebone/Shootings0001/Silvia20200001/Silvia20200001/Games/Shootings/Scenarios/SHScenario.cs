using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.Shootings.Scenarios
{
	/// <summary>
	/// シナリオ
	/// ステージ進行
	/// </summary>
	public abstract class SHScenario
	{
		private Func<bool> _eachFrame = null;

		public bool EachFrame()
		{
			if (_eachFrame == null)
				_eachFrame = SCommon.Supplier(this.E_EachFrame());

			return _eachFrame();
		}

		/// <summary>
		/// ステージ進行処理
		/// 最初のフレームで音楽の再生を行うこと。
		/// </summary>
		/// <returns>タスク：このシナリオを継続するか</returns>
		protected abstract IEnumerable<bool> E_EachFrame();

		private Func<bool> _drawWall = null;

		public void DrawWall()
		{
			if (_drawWall == null)
				_drawWall = SCommon.Supplier(this.E_DrawWall());

			if (!_drawWall())
				throw null; // never
		}

		/// <summary>
		/// 背景描画を行う。
		/// </summary>
		/// <returns>タスク：常に真</returns>
		protected abstract IEnumerable<bool> E_DrawWall();
	}
}
