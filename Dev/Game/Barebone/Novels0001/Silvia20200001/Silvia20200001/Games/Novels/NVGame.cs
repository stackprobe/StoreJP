using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Novels.Scenarios;

namespace Charlotte.Games.Novels
{
	/// <summary>
	/// ゲームメイン
	/// </summary>
	public class NVGame : Anchorable<NVGame>
	{
		public NVScenario Scenario;
		public string[] Pages;
		public int PageIndex = 0;
		public bool ReturnToCallerRequested = false;
		public int MessageCharCount;
		public int MessageFrame;

		/// <summary>
		/// 選択肢
		/// シナリオ内のコマンドからセットされる。
		/// null == 無効
		/// </summary>
		public string[] Choices = null;

		/// <summary>
		/// 最後に選択された選択肢のインデックス
		/// 0～ == 選択されたインデックス
		/// -1 == 無効
		/// </summary>
		public int ChoosedIndex = -1;

		public void Run(NVScenario scenario)
		{
			this.Scenario = scenario;
			this.Pages = this.GetPages();

			DD.FreezeInputUntilRelease();

			for (this.PageIndex = 0; this.PageIndex < this.Pages.Length; this.PageIndex++)
			{
				Action[] commands;
				string[] messages;

				this.ParsePageData(this.Pages[this.PageIndex], out commands, out messages);

				foreach (Action command in commands)
					command();

				if (this.Choices != null)
				{
					this.SelectMenu(messages);
					break;
				}
				this.MessageCharCount = messages.Select(message => message.Length).Sum();
				this.MessageFrame = 0;

				DD.FreezeInput();

				for (; ; this.MessageFrame++)
				{
					if (Inputs.START.GetInput() == 1)
					{
						this.Pause();

						if (this.ReturnToCallerRequested)
							goto endOfGameLoop;
					}
					if (Inputs.B.GetInput() == 1 || Inputs.DIR_8.GetInput() == 1)
					{
						this.Backlog();

						if (this.ReturnToCallerRequested)
							goto endOfGameLoop;
					}
					if (DU.IsPound(Inputs.A.GetInput(), NVConsts.FAST_MODE_INPUT_INTERVAL, NVConsts.FAST_MODE_SPEED))
					{
						int nextPageFrame = this.MessageCharCount * NVConsts.MESSAGE_SPEED + NVConsts.NEXT_PAGE_INPUT_INTERVAL;

						if (nextPageFrame <= this.MessageFrame)
							break;

						this.MessageFrame = nextPageFrame;
					}
					string[] messagesForPrint = this.GetPrintMessages(messages, this.MessageFrame / NVConsts.MESSAGE_SPEED);

					// ====
					// 描画ここから
					// ====

					this.Scenario.Theater.Draw();

					// メッセージ枠
					DD.SetAlpha(0.75);
					DD.SetBright(new I3Color(0, 0, 0).ToD3Color());
					DD.Draw(Pictures.WhiteBox, I4Rect.LTRB(0, 430, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect());

					DD.SetPrint(NVConsts.MESSAGE_L, NVConsts.MESSAGE_T, NVConsts.MESSAGE_Y_STEP, NVConsts.MESSAGE_FONT_NAME, NVConsts.MESSAGE_FONT_SIZE);
					DD.SetPrintColor(NVConsts.MESSAGE_COLOR);

					foreach (string message in messagesForPrint)
					{
						DD.PrintLine(message);
					}

					// ====
					// 描画ここまで
					// ====

					DD.EachFrame();

					// ★★★ ゲームループの終わり ★★★
				}
			}
		endOfGameLoop:
			DD.FreezeInput();

			// ★★★ ゲームメイン処理の終わり ★★★
		}

		private string[] GetPages()
		{
			return SCommon.Separate("\r\n" + SCommon.LinesToText(SCommon.TextToLines(this.Scenario.GetScenario())
				.Where(line => line != "" && line[0] != ';') // 空行とコメント行を除去
				.ToArray()) + "\r\n", "\r\n/\r\n") // ページに分割
				.Where(page => page.Trim() != "") // 空のページを除去
				.ToArray();
		}

		private void ParsePageData(string page, out Action[] commands, out string[] messages)
		{
			string[] lines = SCommon.TextToLines(page)
				.Select(line => line.Trim())
				.Where(line => line != "")
				.ToArray();

			List<Action> commandList = new List<Action>();
			List<string> messageList = new List<string>();

			foreach (string line in lines)
			{
				if (line[0] == '@')
				{
					string[] tokens = SCommon.Tokenize(line.Substring(1), "\t ", false, true);

					if (tokens.Length < 1)
						throw new Exception("Bad tokens");

					commandList.Add(() => this.Scenario.Theater.Invoke(tokens[0], tokens.Skip(1).ToArray()));
				}
				else
				{
					messageList.Add(line);
				}
			}
			commands = commandList.ToArray();
			messages = messageList.ToArray();
		}

		private string[] GetPrintMessages(string[] messages, int charCount)
		{
			List<string> messagesForPrint = new List<string>();

			foreach (string message in messages)
			{
				if (charCount == 0)
					break;

				if (charCount < message.Length)
				{
					messagesForPrint.Add(message.Substring(0, charCount));
					break;
				}
				messagesForPrint.Add(message);
				charCount -= message.Length;
			}
			return messagesForPrint.ToArray();
		}

		/// <summary>
		/// 履歴画面
		/// </summary>
		private void Backlog()
		{
			List<string> lines = new List<string>();
			int backCount = 0;

			for (int index = 0; index <= this.PageIndex; index++)
			{
				Action[] commands;
				string[] messages;

				this.ParsePageData(this.Pages[index], out commands, out messages);

				lines.AddRange(messages);
			}

			DD.FreezeInput();

			for (; ; )
			{
				if (Inputs.START.GetInput() == 1)
				{
					this.Pause();

					if (this.ReturnToCallerRequested)
						break;
				}
				if (Inputs.B.GetInput() == 1)
				{
					break;
				}
				if (Inputs.DIR_8.IsPound())
				{
					if (backCount < lines.Count - 1)
						backCount++;
				}
				if (Inputs.DIR_2.IsPound() || Inputs.A.IsPound())
				{
					if (backCount > 0)
						backCount--;
					else
						break;
				}

				// ====
				// 描画ここから
				// ====

				this.Scenario.Theater.Draw();

				DD.DrawCurtain(-0.75);

				DD.SetPrint(NVConsts.BACKLOG_L, NVConsts.BACKLOG_T, NVConsts.BACKLOG_Y_STEP, NVConsts.BACKLOG_FONT_NAME, NVConsts.BACKLOG_FONT_SIZE);
				DD.SetPrintColor(NVConsts.BACKLOG_COLOR);

				for (int y = 0; y < NVConsts.BACKLOG_Y_MAX; y++)
				{
					int index = lines.Count - backCount - NVConsts.BACKLOG_Y_MAX + y;

					DD.PrintLine(index < 0 ? "" : lines[index]);
				}

				// ====
				// 描画ここまで
				// ====

				DD.EachFrame();
			}
			DD.FreezeInputUntilRelease();
		}

		/// <summary>
		/// 選択肢画面
		/// </summary>
		/// <param name="messages">メッセージ</param>
		private void SelectMenu(string[] messages)
		{
			if (
				messages == null ||
				messages.Length < 1 ||
				messages.Any(v => string.IsNullOrEmpty(v)) ||
				this.Choices == null ||
				this.Choices.Length < 1 ||
				this.Choices.Any(v => string.IsNullOrEmpty(v))
				)
				throw new Exception("Bad params");

			int ITEM_L = 300;
			int ITEM_T = 200;
			int ITEM_W = GameConfig.ScreenSize.W - ITEM_L;
			int ITEM_H = 50;
			int ITEM_Y_STEP = 100;
			int ITEM_TEXT_L = 10;
			int ITEM_TEXT_T = 10;
			int ITEM_TEXT_FONT_SIZE = 32;
			string ITEM_TEXT_FONT_NAME = "Kゴシック";

			int selectedIndex = -1; // -1 == 未選択

			DD.FreezeInput();

			for (; ; )
			{
				if (Inputs.START.GetInput() == 1)
				{
					this.Pause();

					if (this.ReturnToCallerRequested)
						break;
				}
				if (Inputs.B.GetInput() == 1)
				{
					this.Backlog();

					if (this.ReturnToCallerRequested)
						break;
				}
				if (Inputs.A.GetInput() == 1)
				{
					if (selectedIndex == -1) // ? 未選択
					{
						// none
					}
					else
					{
						this.ChoosedIndex = selectedIndex;
						break;
					}
				}
				if (Inputs.DIR_8.GetInput() == 1)
				{
					if (selectedIndex == -1) // ? 未選択
					{
						selectedIndex = this.Choices.Length - 1;
					}
					else
					{
						selectedIndex += this.Choices.Length - 1;
						selectedIndex %= this.Choices.Length;
					}
				}
				if (Inputs.DIR_2.GetInput() == 1)
				{
					if (selectedIndex == -1) // ? 未選択
					{
						selectedIndex = 0;
					}
					else
					{
						selectedIndex++;
						selectedIndex %= this.Choices.Length;
					}
				}

				// ====
				// 描画ここから
				// ====

				this.Scenario.Theater.Draw();

				DD.DrawCurtain(-0.5);

				DD.SetPrint(300, 60, 30, NVConsts.MESSAGE_FONT_NAME, NVConsts.MESSAGE_FONT_SIZE);
				DD.SetPrintColor(NVConsts.MESSAGE_COLOR);

				foreach (string message in messages)
					DD.PrintLine(message);

				for (int index = 0; index < this.Choices.Length; index++)
				{
					bool selected = index == selectedIndex;

					DD.SetAlpha(0.5);
					DD.SetBright((selected ? new I3Color(255, 255, 255) : new I3Color(192, 192, 255)).ToD3Color());
					DD.Draw(Pictures.WhiteBox, new I4Rect(ITEM_L, ITEM_T + index * ITEM_Y_STEP, ITEM_W, ITEM_H).ToD4Rect());

					DD.SetPrint(ITEM_L + ITEM_TEXT_L, ITEM_T + index * ITEM_Y_STEP + ITEM_TEXT_T, 0, ITEM_TEXT_FONT_NAME, ITEM_TEXT_FONT_SIZE);
					DD.SetPrintColor(selected ? new I3Color(255, 255, 255) : new I3Color(48, 48, 64));
					DD.Print(this.Choices[index]);
				}

				// ====
				// 描画ここまで
				// ====

				DD.EachFrame();
			}
			DD.FreezeInputUntilRelease();
		}

		private static VScreen PauseWall = new VScreen(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H);

		/// <summary>
		/// ポーズメニュー
		/// </summary>
		private void Pause()
		{
			using (PauseWall.Section())
			{
				DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
			}

			SimpleMenu menu = new SimpleMenu(24, 30, 16, 400, "PAUSE", new string[]
 			{
				"NOOP",
				"タイトルメニューに戻る",
				"ゲームに戻る",
			});

			menu.NoPound = true;
			menu.CancelByPause = true;

			DD.FreezeInputUntilRelease();

			double blurRate = 0.0;

			for (; ; )
			{
				DD.FreezeInput();

				for (; ; )
				{
					DD.Approach(ref blurRate, 0.5, 0.98);

					DD.Draw(PauseWall.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
					DD.Blur(blurRate);

					if (menu.Draw())
						break;

					DD.EachFrame();
				}
				DD.FreezeInput();

				switch (menu.SelectedIndex)
				{
					case 0:
						// noop
						break;

					case 1:
						this.ReturnToCallerRequested = true;
						goto endOfMenu;

					case 2:
						goto endOfMenu;

					default:
						throw null; // never
				}
			}
		endOfMenu:
			DD.FreezeInputUntilRelease();

			PauseWall.Unload();
		}
	}
}
