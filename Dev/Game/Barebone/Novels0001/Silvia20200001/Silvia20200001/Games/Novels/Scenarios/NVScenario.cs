using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Novels.Theaters;

namespace Charlotte.Games.Novels.Scenarios
{
	/// <summary>
	/// シナリオ・脚本
	/// </summary>
	public abstract class NVScenario
	{
		/// <summary>
		/// シアター・上映装置
		/// </summary>
		public readonly NVTheater Theater = null;

		public NVScenario(NVTheater theater)
		{
			this.Theater = theater;
		}

		/// <summary>
		/// シナリオを返す。
		/// 特殊な行：
		/// -- 空行 == 無視する。
		/// -- 半角スラッシュ(/)のみの行 == ページの区切り
		/// -- 半角セミコロン(;)で始まる行 == コメント
		/// -- 半角アットマーク(@)で始まる行 == コマンド
		/// 空のページは無視する。
		/// </summary>
		/// <returns>シナリオ</returns>
		public abstract string GetScenario();

		/// <summary>
		/// 次のシナリオを返す。
		/// 呼び出し側へ制御を戻す場合は null を返すこと。
		/// 最後の選択肢の選択インデックスは以下を参照すること。
		/// -- NVGame.I.ChoosedIndex
		/// </summary>
		/// <returns>次のシナリオ(null:次のシナリオは無い)</returns>
		public abstract NVScenario GetNextScenario();
	}
}
