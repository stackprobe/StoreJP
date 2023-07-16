using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Novels.Theaters;

namespace Charlotte.Games.Novels.Scenarios
{
	public class NVScenario_Test0003 : NVScenario
	{
		public NVScenario_Test0003()
			: base(new NVTheater_Test0001())
		{ }

		public override string GetScenario()
		{
			return @"
/
２つ目の選択肢を選びました。

@キャラクタ作成 ゆかり
@キャラクタ位置 ゆかり 左端
@キャラクタ画像 ゆかり ゆかり(普)
@キャラクタ位置 ゆかり 中央

; 背景初回=2回
@背景変更 UKIc
@背景変更 UKIc

@音楽停止

/
このルートはここで終わりです。

";
		}

		public override NVScenario GetNextScenario()
		{
			return null;
		}
	}
}
