using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	public static class ContinueMenu
	{
		/// <summary>
		/// コンテニュー(ロード)画面
		/// セーブデータ：
		/// -- キャンセルした場合は空文字列("")を返す。
		/// </summary>
		/// <returns>セーブデータ(SerializedString)</returns>
		public static string Run()
		{
			string[] items = GameSetting.SaveDataSlots.Select(v => v.SerializedString == "" ?
				"----" :
				"[" + v.SavedDateTime + "]　" + v.Description).Concat(new string[] { "(戻る)" }).ToArray();

			SimpleMenu menu = new SimpleMenu(20, 40, 20, GameConfig.ScreenSize.W, "ロードするスロットを選択して下さい。", items);

			for (; ; )
			{
				DD.FreezeInput();

				for (; ; )
				{
					DD.Draw(Pictures.KAZ7842gyftdrhfyg, new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());

					if (menu.Draw())
						break;

					DD.EachFrame();
				}
				DD.FreezeInput();

				if (menu.SelectedIndex == GameSetting.SAVE_DATA_SLOT_NUM)
					break;

				GameSetting.SaveDataSlot slot = GameSetting.SaveDataSlots[menu.SelectedIndex];
				string serializedString = slot.SerializedString;

				if (serializedString != "") // ? セーブデータ有り
					return serializedString;
			}
			return ""; // キャンセル
		}
	}
}
