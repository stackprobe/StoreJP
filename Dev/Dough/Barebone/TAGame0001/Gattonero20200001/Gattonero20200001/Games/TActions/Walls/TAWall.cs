using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.TActions.Walls
{
	/// <summary>
	/// 背景
	/// </summary>
	public abstract class TAWall
	{
		private Func<bool> _draw = null;

		/// <summary>
		/// 背景を描画する。
		/// </summary>
		public void Draw()
		{
			if (_draw == null)
				_draw = SCommon.Supplier(this.E_Draw());

			if (!_draw())
				throw null; // never
		}

		/// <summary>
		/// 背景を描画する。
		/// </summary>
		/// <returns>タスク：常に真</returns>
		protected abstract IEnumerable<bool> E_Draw();
	}
}
