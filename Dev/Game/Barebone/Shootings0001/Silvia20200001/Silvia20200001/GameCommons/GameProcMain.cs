﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GUICommons;

namespace Charlotte.GameCommons
{
	public static class GameProcMain
	{
		private static Action Initialized;

		public static void GameMain(Form mainForm)
		{
			DD.RunOnUIThread = GetRunOnUIThread(mainForm);

			Thread th = new Thread(() =>
			{
				bool aliving = true;

				Initialized = () =>
				{
					DD.RunOnUIThread(() =>
					{
						if (aliving)
							mainForm.Visible = false;
					});
				};

				Main2();

				DD.RunOnUIThread(() =>
				{
					aliving = false;
					mainForm.Close();
				});
			});

			th.Start();
		}

		/// <summary>
		/// メインスレッドで実行される処理を登録する処理を返す。
		/// 戻り値の処理は以下を満たす。
		/// -- スレッドセーフ
		/// -- 登録された処理は登録された順で実行される。
		/// </summary>
		/// <param name="mainForm">メインフォーム</param>
		/// <returns>処理</returns>
		private static Action<Action> GetRunOnUIThread(Form mainForm)
		{
			Queue<Action> q = new Queue<Action>();
			object SYNCROOT = new object();

			return routine =>
			{
				lock (SYNCROOT)
				{
					q.Enqueue(routine);

					mainForm.BeginInvoke((MethodInvoker)delegate
					{
						Action firstRoutine;

						lock (SYNCROOT)
						{
							firstRoutine = q.Dequeue();
						}

						firstRoutine();
					});
				}
			};
		}

		private static void Main2()
		{
			LibbonDialog.Th = new Thread(LibbonDialog.MainTh);
			LibbonDialog.Th.Start();
			try
			{
				Main3();
			}
			finally
			{
				LibbonDialog.AliveFlag = false;
				LibbonDialog.MainThStandby.Set();
				LibbonDialog.Th.Join();
			}
		}

		private static void Main3()
		{
			try
			{
				Main4();
			}
			catch (DU.CoffeeBreak)
			{
				// noop
			}
			catch (Exception ex)
			{
				WriteErrorTerminationLog(ex);
			}
			finally
			{
				while (1 <= DD.Finalizers.Count)
				{
					try
					{
						SCommon.UnaddElement(DD.Finalizers)();
					}
					catch (Exception ex)
					{
						WriteErrorTerminationLog(ex);
					}
				}
			}
		}

		private static void WriteErrorTerminationLog(Exception ex)
		{
			ProcMain.WriteLog(ex);
			ProcMain.WriteLog(GameConfig.GameTitle);
			ProcMain.WriteLog(GUIProcMain.BuiltDateTime);
		}

		private static void Main4()
		{
			string logSaveDir;
			string logFile;
			string saveDataFile;

			if (ProcMain.DEBUG)
			{
#if DEBUG
				string solutionDirName = Path.GetFileName(SCommon.MakeFullPath(@"..\..\..\.."));

				logSaveDir = @"C:\temp";
				logFile = @"C:\temp\Game.log";
				saveDataFile = @"C:\temp\SaveData_" + solutionDirName + ".dat";
#else
				throw new Exception("DEBUG is True");
#endif
			}
			else
			{
				logSaveDir = DU.WD.MakePath();
				logFile = Path.Combine(ProcMain.SelfDir, "Game.log");
				saveDataFile = Path.Combine(ProcMain.SelfDir, "SaveData.dat");

				SCommon.CreateDir(logSaveDir);
			}

			File.WriteAllBytes(logFile, SCommon.EMPTY_BYTES);

			ProcMain.WriteLog = message =>
			{
				File.AppendAllText(logFile, "[" + DateTime.Now + "] " + message + "\r\n", Encoding.UTF8);
			};

			foreach (string requiredFileName in new string[] { "DxLib.dll", "DxLibDotNet.dll" })
				if (!File.Exists(Path.Combine(ProcMain.SelfDir, requiredFileName)))
					throw new Exception("no file: " + requiredFileName);

			GameSetting.Initialize();

			if (File.Exists(saveDataFile))
				GameSetting.Deserialize(Encoding.ASCII.GetString(DU.Hasher.UnaddHash(DU.SaveDataFileFormatter.Decode(File.ReadAllBytes(saveDataFile)))));

			DD.Save = () =>
			{
				File.WriteAllBytes(saveDataFile, DU.SaveDataFileFormatter.Encode(DU.Hasher.AddHash(Encoding.ASCII.GetBytes(GameSetting.Serialize()))));
			};

			DD.Finalizers.Add(DD.Save);

			DD.MainWindowTitle =
				GameConfig.GameTitle
				+ " / "
				+ GUIProcMain.BuiltDateTime.ToString("yyyy-MM-dd-HH-mm-ss");

			DD.TargetMonitor = DU.GetTargetMonitor_Boot();
			DD.SetLibbon("アプリケーションを起動しています...");

			Icon icon;

			using (MemoryStream mem = new MemoryStream(DD.GetResFileData(@"General\app.ico").Data.Value))
			{
				icon = new Icon(mem);
			}

			// DXLib 初期化 ここから

			DX.SetApplicationLogSaveDirectory(logSaveDir);
			DX.SetOutApplicationLogValidFlag(1); // ログを出力/1:する/0:しない
			DX.SetAlwaysRunFlag(1); // 非アクティブ時に/1:動く/0:止まる
			DX.SetMainWindowText(DD.MainWindowTitle);
			DX.SetGraphMode(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H, 32); // 幅, 高さ, ビット数(16 or 32)
			DX.ChangeWindowMode(1); // 1:ウィンドウ/0:フルスクリーン
			DX.SetWindowIconHandle(icon.Handle);

			if (DX.DxLib_Init() != 0) // ? 失敗
				throw new Exception("DxLib_Init failed");

			DD.Finalizers.Add(() =>
			{
				if (DX.DxLib_End() != 0) // ? 失敗
					throw new Exception("DxLib_End failed");
			});

			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetDrawMode(DX.DX_DRAWMODE_ANISOTROPIC);
			DX.SetWindowSizeChangeEnableFlag(0); // ウィンドウの右下をドラッグでサイズ変更/1:する/0:しない
			DX.SetMouseDispFlag(GameSetting.MouseCursorShow ? 1 : 0); // マウスカーソルを表示/1:する/0:しない

			// DXLib 初期化 ここまで

			Keyboard.Initialize();
			Pad.Initialize();

			if (GameSetting.FullScreen)
				DD.RealScreenSize = new I2Size(DD.TargetMonitor.W, DD.TargetMonitor.H);
			else
				DD.RealScreenSize = GameSetting.UserScreenSize;

			DD.MainScreenDrawRect = new I4Rect(0, 0, DD.RealScreenSize.W, DD.RealScreenSize.H);
			DD.MainScreen = new VScreen(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H);
			DD.LastMainScreen = new VScreen(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H);

			foreach (string resPath in GameConfig.FontFileResPaths)
				DU.AddFontFile(resPath);

			SetRealScreenSize(DD.RealScreenSize.W, DD.RealScreenSize.H);

			DD.SetLibbon(null);

			Initialized();

			TProgram.Run();
		}

		/// <summary>
		/// ゲーム画面サイズを変更する。
		/// このクラス以外からは以下を経由して呼び出すこと。
		/// -- DD.SetRealScreenSize()
		/// </summary>
		/// <param name="w">幅</param>
		/// <param name="h">高さ</param>
		public static void SetRealScreenSize(int w, int h)
		{
			VScreen.StoreImageDataIfLoadedForAll();

			Picture.UnloadAll();
			VScreen.UnloadAll();
			DU.UnloadAllFontHandle();

			DX.SetGraphMode(w, h, 32);
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetDrawMode(DX.DX_DRAWMODE_ANISOTROPIC);
			DX.SetWindowSizeChangeEnableFlag(0);
			DX.SetMouseDispFlag(GameSetting.MouseCursorShow ? 1 : 0);

			VScreen.RestoreImageDataIfStoredForAll();

			int l = DD.TargetMonitor.L + (DD.TargetMonitor.W - w) / 2;
			int t = DD.TargetMonitor.T + (DD.TargetMonitor.H - h) / 2;

			DU.SetMainWindowPosition(l, t);

			DD.MainScreenDrawRect = DD.EnlargeFullInterior(
				GameConfig.ScreenSize.ToD2Size(),
				new I4Rect(0, 0, w, h).ToD4Rect()
				)
				.ToI4Rect();

			DD.RealScreenSize.W = w;
			DD.RealScreenSize.H = h;
		}
	}
}
