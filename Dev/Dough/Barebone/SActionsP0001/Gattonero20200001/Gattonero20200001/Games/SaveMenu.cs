using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.SActions;

namespace Charlotte.Games
{
	public static class SaveMenu
	{
		public static void Run()
		{
			const double BLUR_RATE = 0.5;

			// Interlude
			{
				using (DU.FreeScreen.Section())
				{
					DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
				}

				foreach (Scene scene in Scene.Create(30))
				{
					DD.Draw(DU.FreeScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
					DD.Blur(scene.Rate * BLUR_RATE);

					DD.EachFrame();
				}
			}

			string[] items = GameSetting.SaveDataSlots.Select(v => v.SerializedString == "" ?
				"----" :
				"[" + v.SavedDateTime + "]　" + v.Description).Concat(new string[] { "(キャンセル)" }).ToArray();

			SimpleMenu menu = new SimpleMenu(20, 40, 20, GameConfig.ScreenSize.W, "セーブするスロットを選択して下さい。", items);

			DD.FreezeInput();

			for (; ; )
			{
				DD.Draw(DU.FreeScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
				DD.Blur(BLUR_RATE);

				if (menu.Draw())
					break;

				DD.EachFrame();
			}
			DD.FreezeInput();

			if (menu.SelectedIndex < GameSetting.SAVE_DATA_SLOT_NUM)
			{
				GameStatus.I.LastSavedFieldName = SAGame.I.Field.Name;

				GameSetting.SaveDataSlot slot = GameSetting.SaveDataSlots[menu.SelectedIndex];

				slot.SavedDateTime = SimpleDateTime.Now();
				slot.Description = SAGame.I.Field.Description;
				slot.SerializedString = GameStatus.I.Serialize();

				SoundEffects.Save.Play();
			}
			else
			{
				// none
			}

			// Interlude
			{
				foreach (Scene scene in Scene.Create(30))
				{
					DD.Draw(DU.FreeScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
					DD.Blur((1.0 - scene.Rate) * BLUR_RATE);

					DD.EachFrame();
				}

				using (DD.MainScreen.Section()) // ゲームメインに戻ったフレームの描画を補填する。
				{
					DD.Draw(DU.FreeScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
				}
			}
		}
	}
}
