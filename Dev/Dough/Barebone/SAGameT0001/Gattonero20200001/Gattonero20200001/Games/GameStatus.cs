using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games
{
	/// <summary>
	/// ゲームの内部状態
	/// インスタンスの初期状態は「ゲームスタート時の状態」でなければならない。
	/// </summary>
	public class GameStatus
	{
		private static GameStatus _i = null;

		public static GameStatus I
		{
			get
			{
				if (_i == null)
					_i = new GameStatus();

				return _i;
			}
		}

		/// <summary>
		/// リセットする。
		/// -- ゲームスタート時の状態に戻す。
		/// </summary>
		public static void Reset()
		{
			_i = null;
		}

		public string LastSavedFieldName = "Default";

		/// <summary>
		/// 体力の最大値
		/// 1～
		/// </summary>
		public int HPMax = 10;

		/// <summary>
		/// 体力
		/// -1 == 死亡
		/// 0 == (不使用・予約)
		/// 1～ == 残り体力
		/// </summary>
		public int HP = 10;

		private GameStatus()
		{
			// none
		}

		/// <summary>
		/// 開発・デバッグ用にステータスを強化する。
		/// </summary>
		public void Cheat()
		{
			// none
		}

		private const string SERIALIZED_DATA_START_MARK = "<GameStatus>";
		private const string SERIALIZED_DATA_END_MARK = "</GameStatus>";

		public string Serialize()
		{
			List<object> dest = new List<object>();

			dest.Add(SERIALIZED_DATA_START_MARK);

			// ---- 保存データここから ----

			dest.Add(this.LastSavedFieldName);
			dest.Add(this.HPMax);
			dest.Add(this.HP);

			// ---- 保存データここまで ----

			dest.Add(SERIALIZED_DATA_END_MARK);

			return SCommon.Serializer.I.Join(dest.Select(v => v.ToString()).ToArray());
		}

		public void Deserialize(string serializedString)
		{
			string[] src = SCommon.Serializer.I.Split(serializedString);
			int c = 0;

			if (src[c++] != SERIALIZED_DATA_START_MARK)
				throw new Exception("Bad SERIALIZED_DATA_START_MARK");

			// ---- 保存データここから ----

			this.LastSavedFieldName = src[c++];
			this.HPMax = int.Parse(src[c++]);
			this.HP = int.Parse(src[c++]);

			// ---- 保存データここまで ----

			if (src[c++] != SERIALIZED_DATA_END_MARK)
				throw new Exception("Bad SERIALIZED_DATA_END_MARK");
		}
	}
}
