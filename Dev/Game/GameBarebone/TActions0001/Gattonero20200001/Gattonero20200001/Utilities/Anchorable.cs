using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Utilities
{
	/// <summary>
	/// このインスタンスが有効である間、係留しておく。
	/// 同時に複数のインスタンスを生成してはならない。
	/// </summary>
	/// <typeparam name="MYSELF">自分自身(このクラスを継承したサブクラス自身の型)</typeparam>
	public abstract class Anchorable<MYSELF> : IDisposable where MYSELF : Anchorable<MYSELF>
	{
		public static MYSELF I = null;

		public Anchorable()
		{
			if (I != null)
				throw new Exception("係留済み");

			I = (MYSELF)this;
		}

		public void Dispose()
		{
			I = null;
		}
	}
}
