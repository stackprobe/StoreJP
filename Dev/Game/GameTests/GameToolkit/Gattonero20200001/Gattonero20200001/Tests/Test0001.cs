using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Tests
{
	public class Test0001
	{
		private const string TESTEE_MUSIC_FILE_DIR = @"C:\temp";

		private const long POSITION_MARGIN = 88200;

		private string TesteeMusicFile;
		private int TesteeMusicHandle = -1;
		private long PlayStartBeforePosition = 44100;
		private long LoopStartPosition = 441000;
		private long LoopEndPosition = 661500;

		public void Test01()
		{
			TesteeMusicFile = GetTesteeMusicFile();
			TesteeMusicHandle = -1;

			DD.FreezeInput();

			const int SELECT_ITEM_COUNT = 3;
			int selectIndex = 0;

			for (; ; )
			{
				if (Keyboard.IsPound(DX.KEY_INPUT_UP))
					selectIndex--;

				if (Keyboard.IsPound(DX.KEY_INPUT_DOWN))
					selectIndex++;

				selectIndex += SELECT_ITEM_COUNT;
				selectIndex %= SELECT_ITEM_COUNT;

				int valueAdd = 0;

				if (Keyboard.IsPound(DX.KEY_INPUT_LEFT))
					valueAdd = -1;

				if (Keyboard.IsPound(DX.KEY_INPUT_RIGHT))
					valueAdd = 1;

				if (Keyboard.IsPound(DX.KEY_INPUT_PGDN))
					valueAdd = -10;

				if (Keyboard.IsPound(DX.KEY_INPUT_PGUP))
					valueAdd = 10;

				if (valueAdd != 0)
				{
					switch (selectIndex)
					{
						case 0: AddLongValueTo(valueAdd, ref PlayStartBeforePosition); break;
						case 1: AddLongValueTo(valueAdd, ref LoopStartPosition); break;
						case 2: AddLongValueTo(valueAdd, ref LoopEndPosition); break;
					}
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_I) == 1 || Keyboard.GetInput(DX.KEY_INPUT_RETURN) == 1)
				{
					switch (selectIndex)
					{
						case 0: InputLongValue(ref PlayStartBeforePosition); break;
						case 1: InputLongValue(ref LoopStartPosition); break;
						case 2: InputLongValue(ref LoopEndPosition); break;
					}
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_P) == 1)
				{
					RestartTesteeMusic();
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_S) == 1)
				{
					StopTesteeMusic();
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_L) == 1)
				{
					ProcMain.WriteLog("----");
					ProcMain.WriteLog("LoopStartPosition   :   " + LoopStartPosition);
					ProcMain.WriteLog("LoopEndPosition     :   " + LoopEndPosition);
					ProcMain.WriteLog("LoopLength          :   " + (LoopEndPosition - LoopStartPosition));
				}

				// 値の矯正
				{
					if (LoopStartPosition < 0)
						LoopStartPosition = 0;

					if (LoopEndPosition < LoopStartPosition + POSITION_MARGIN)
						LoopEndPosition = LoopStartPosition + POSITION_MARGIN;

					if (PlayStartBeforePosition < 0)
						PlayStartBeforePosition = 0;

					if (PlayStartBeforePosition > LoopEndPosition)
						PlayStartBeforePosition = LoopEndPosition;
				}

				DD.SetPrint(50, 50, 25);
				DD.PrintLine("MUSIC LOOP TEST");
				DD.PrintLine("----");
				DD.PrintLine((selectIndex == 0 ? "[>]" : "[ ]") + "   PlayStartBeforePosition   :   " + PlayStartBeforePosition + "   <-- ループ終了位置からこれだけ戻る。");
				DD.PrintLine((selectIndex == 1 ? "[>]" : "[ ]") + "   LoopStartPosition         :   " + LoopStartPosition);
				DD.PrintLine((selectIndex == 2 ? "[>]" : "[ ]") + "   LoopEndPosition           :   " + LoopEndPosition);
				DD.PrintLine("----");
				DD.PrintLine("LoopLength                      :   " + (LoopEndPosition - LoopStartPosition));
				DD.PrintLine("----");
				DD.PrintLine("カーソル上下 ... 選択変更");
				DD.PrintLine("カーソル左右 ... 増減");
				DD.PrintLine("I / ENTER ... 直接入力");
				DD.PrintLine("P ... 再生");
				DD.PrintLine("S ... 停止");
				DD.PrintLine("L ... ログに書き出す。");
				DD.PrintLine("----");
				DD.PrintLine("サンプリングレートはたいてい 44100 Hz == 1 sec です。");

				DD.EachFrame();
			}
		}

		private void InputLongValue(ref long value)
		{
			string line = "";

			DD.FreezeInput();

			for (; ; )
			{
				if (Keyboard.GetInput(DX.KEY_INPUT_0) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD0) == 1) line += "0";
				if (Keyboard.GetInput(DX.KEY_INPUT_1) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD1) == 1) line += "1";
				if (Keyboard.GetInput(DX.KEY_INPUT_2) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD2) == 1) line += "2";
				if (Keyboard.GetInput(DX.KEY_INPUT_3) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD3) == 1) line += "3";
				if (Keyboard.GetInput(DX.KEY_INPUT_4) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD4) == 1) line += "4";
				if (Keyboard.GetInput(DX.KEY_INPUT_5) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD5) == 1) line += "5";
				if (Keyboard.GetInput(DX.KEY_INPUT_6) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD6) == 1) line += "6";
				if (Keyboard.GetInput(DX.KEY_INPUT_7) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD7) == 1) line += "7";
				if (Keyboard.GetInput(DX.KEY_INPUT_8) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD8) == 1) line += "8";
				if (Keyboard.GetInput(DX.KEY_INPUT_9) == 1 || Keyboard.GetInput(DX.KEY_INPUT_NUMPAD9) == 1) line += "9";

				if (Keyboard.GetInput(DX.KEY_INPUT_DELETE) == 1 || Keyboard.GetInput(DX.KEY_INPUT_BACK) == 1)
				{
					if (1 <= line.Length)
					{
						line = line.Substring(0, line.Length - 1);
					}
				}

				// 値の矯正
				{
					while (line != "0" && line.StartsWith("0"))
					{
						line = line.Substring(1);
					}
					if (19 < line.Length)
					{
						line = line.Substring(0, 19);
					}
				}

				if (Keyboard.GetInput(DX.KEY_INPUT_RETURN) == 1)
				{
					break;
				}

				DD.SetPrint(50, 50, 25);
				DD.PrintLine("> " + line + ((DD.ProcFrame / 30) % 2 == 0 ? "_" : " "));
				DD.PrintLine("# " + value);

				DD.EachFrame();
			}
			DD.FreezeInput();

			if (line != "")
			{
				value = long.Parse(line);
			}
		}

		private void AddLongValueTo(long valueAdd, ref long value)
		{
			value += valueAdd;
		}

		private void RestartTesteeMusic()
		{
			if (TesteeMusicHandle != -1)
			{
				StopTesteeMusic();

				// ----

				DX.DeleteSoundMem(TesteeMusicHandle);
				TesteeMusicHandle = -1;
			}
			byte[] fileData = File.ReadAllBytes(TesteeMusicFile);
			int handle = -1;

			DU.PinOn(fileData, p => handle = DX.LoadSoundMemByMemImage(p, (ulong)fileData.Length));

			if (handle == -1) // ? 失敗
				throw new Exception("LoadSoundMemByMemImage failed");

			if (DX.SetLoopSamplePosSoundMem(LoopStartPosition, handle) != 0) // ? 失敗
				throw new Exception("SetLoopSamplePosSoundMem failed");

			if (DX.SetLoopStartSamplePosSoundMem(LoopEndPosition, handle) != 0) // ? 失敗
				throw new Exception("SetLoopStartSamplePosSoundMem failed");

			TesteeMusicHandle = handle;

			// ----

			PlayTesteeMusic();
		}

		private void PlayTesteeMusic()
		{
			if (TesteeMusicHandle == -1) // ? not loaded -> noop
				return;

			DX.SetCurrentPositionSoundMem(LoopEndPosition - PlayStartBeforePosition, TesteeMusicHandle); // 再生開始位置を指定する。
			DX.PlaySoundMem(TesteeMusicHandle, DX.DX_PLAYTYPE_LOOP, 0); // 再生
		}

		private void StopTesteeMusic()
		{
			if (TesteeMusicHandle == -1) // ? not loaded -> noop
				return;

			DX.StopSoundMem(TesteeMusicHandle); // 停止
		}

		private string GetTesteeMusicFile()
		{
			foreach (string file in Directory.GetFiles(TESTEE_MUSIC_FILE_DIR).OrderBy(SCommon.CompIgnoreCase))
				if (IsMusicFile(file))
					return file;

			throw new Exception("no TesteeMusicFile");
		}

		private bool IsMusicFile(string file)
		{
			string ext = Path.GetExtension(file).ToLower();

			return
				ext == ".mp3" ||
				ext == ".wav";
		}
	}
}
