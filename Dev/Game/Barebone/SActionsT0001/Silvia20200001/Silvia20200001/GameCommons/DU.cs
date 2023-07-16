using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// この名前空間内で使用される共通機能・便利機能をこのクラスに集約する。
	/// </summary>
	public static class DU
	{
		public class CoffeeBreak : Exception
		{ }

		private static Lazy<WorkingDir> _wd = new Lazy<WorkingDir>(() => new WorkingDir());

		/// <summary>
		/// 各機能自由に使ってよい作業フォルダ
		/// </summary>
		public static WorkingDir WD
		{
			get
			{
				return _wd.Value;
			}
		}

		/// <summary>
		/// 各機能自由に使ってよいスクリーン
		/// </summary>
		public static VScreen FreeScreen = new VScreen(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H);

		/// <summary>
		/// 指定されたクラスの静的フィールドを初期化する。
		/// </summary>
		/// <param name="type">クラスのタイプ</param>
		private static void InitializeStaticFields(Type type)
		{
			type.TypeInitializer.Invoke(null, null);
		}

		#region Touch

		private static bool TouchCalled = false;

		/// <summary>
		/// 小さなリソースを程よくロードされた状態にする。
		/// 幕間で適宜実行すると良い。
		/// 備忘：スクリーンの画像は解放するけどスクリーン自体は解放しないので内容は失われないよ。
		/// </summary>
		public static void Touch()
		{
			if (!TouchCalled)
			{
				TouchCalled = true;

				DU.InitializeStaticFields(typeof(Musics));
				DU.InitializeStaticFields(typeof(Pictures));
				DU.InitializeStaticFields(typeof(SoundEffects));
			}

			Music.Touch();
			Picture.Touch();
			SoundEffect.Touch();
		}

		/// <summary>
		/// ロードされた全てのリソースを解放する。
		/// 別に実行しなくても良い。
		/// 注意：スクリーンの内容は失われる。
		/// </summary>
		public static void Detach()
		{
			Picture.UnloadAll();
			VScreen.UnloadAll();
			DU.UnloadAllFontHandle();
			Music.TryUnloadAll();
			SoundEffect.TryUnloadAll();
		}

		#endregion

		public static void Pin<T>(T data)
		{
			GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);

			DD.Finalizers.Add(() =>
			{
				h.Free();
			});
		}

		public static void PinOn<T>(T data, Action<IntPtr> routine)
		{
			GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				routine(pinnedData.AddrOfPinnedObject());
			}
			finally
			{
				pinnedData.Free();
			}
		}

		public class Collector<T>
		{
			private List<T> Inner = new List<T>();

			public void Add(T element)
			{
				this.Inner.Add(element);
			}

			public IEnumerable<T> Iterate()
			{
				return DD.Iterate(this.Inner);
			}
		}

		private static I2Point GetMousePosition()
		{
			return new I2Point(Cursor.Position.X, Cursor.Position.Y);
		}

		private static I4Rect[] Monitors = null;

		private static I4Rect[] GetAllMonitor()
		{
			if (Monitors == null)
			{
				Monitors = Screen.AllScreens.Select(screen => new I4Rect(
					screen.Bounds.Left,
					screen.Bounds.Top,
					screen.Bounds.Width,
					screen.Bounds.Height
					))
					.ToArray();
			}
			return Monitors;
		}

		private static I2Point GetMainWindowPosition()
		{
			Win32APIWrapper.POINT p;

			p.X = 0;
			p.Y = 0;

			Win32APIWrapper.W_ClientToScreen(Win32APIWrapper.GetMainWindowHandle(), out p);

			return new I2Point(p.X, p.Y);
		}

		private static I2Point GetMainWindowCenterPosition()
		{
			I2Point p = GetMainWindowPosition();

			p.X += DD.RealScreenSize.W / 2;
			p.Y += DD.RealScreenSize.H / 2;

			return p;
		}

		/// <summary>
		/// 起動時におけるターゲット画面を取得する。
		/// </summary>
		/// <returns>画面の領域</returns>
		public static I4Rect GetTargetMonitor_Boot()
		{
			I2Point mousePos = GetMousePosition();

			foreach (I4Rect monitor in GetAllMonitor())
			{
				if (
					monitor.L <= mousePos.X && mousePos.X < monitor.R &&
					monitor.T <= mousePos.Y && mousePos.Y < monitor.B
					)
					return monitor;
			}
			return GetAllMonitor()[0]; // 何故か見つからない -> 適当な画面を返す。
		}

		/// <summary>
		/// 現在のターゲット画面を取得する。
		/// </summary>
		/// <returns>画面の領域</returns>
		public static I4Rect GetTargetMonitor()
		{
			I2Point mainWinCenterPt = GetMainWindowCenterPosition();

			foreach (I4Rect monitor in GetAllMonitor())
			{
				if (
					monitor.L <= mainWinCenterPt.X && mainWinCenterPt.X < monitor.R &&
					monitor.T <= mainWinCenterPt.Y && mainWinCenterPt.Y < monitor.B
					)
					return monitor;
			}
			return GetAllMonitor()[0]; // 何故か見つからない -> 適当な画面を返す。
		}

		public static void SetMainWindowPosition(int l, int t)
		{
			DX.SetWindowPosition(l, t);

			I2Point p = DU.GetMainWindowPosition();

			l += l - p.X;
			t += t - p.Y;

			DX.SetWindowPosition(l, t);
		}

		/// <summary>
		/// コンピュータを起動してから経過した時間を返す。
		/// 単位：ミリ秒
		/// </summary>
		/// <returns>時間(ミリ秒)</returns>
		public static long GetCurrentTime()
		{
			return DX.GetNowHiPerformanceCount() / 1000L;
		}

		public static bool IsSoundPlaying(int handle)
		{
			bool playing;

			switch (DX.CheckSoundMem(handle))
			{
				case 0: // 停止中
					playing = false;
					break;

				case 1: // 再生中
					playing = true;
					break;

				default:
					throw new Exception("CheckSoundMem failed");
			}
			return playing;
		}

		public static Picture.PictureDataInfo GetPictureData(byte[] fileData)
		{
			if (fileData == null)
				throw new Exception("Bad fileData (null)");

			if (fileData.Length == 0)
				throw new Exception("Bad fileData (zero bytes)");

			int softImage = -1;

			DU.PinOn(fileData, p => softImage = DX.LoadSoftImageToMem(p, fileData.Length));

			if (softImage == -1)
				throw new Exception("LoadSoftImageToMem failed");

			int w;
			int h;

			if (DX.GetSoftImageSize(softImage, out w, out h) != 0) // ? 失敗
				throw new Exception("GetSoftImageSize failed");

			if (w < 1 || SCommon.IMAX < w)
				throw new Exception("Bad w");

			if (h < 1 || SCommon.IMAX < h)
				throw new Exception("Bad h");

			// RGB -> RGBA
			{
				int newSoftImage = DX.MakeARGB8ColorSoftImage(w, h);

				if (newSoftImage == -1) // ? 失敗
					throw new Exception("MakeARGB8ColorSoftImage failed");

				if (DX.BltSoftImage(0, 0, w, h, softImage, 0, 0, newSoftImage) != 0) // ? 失敗
					throw new Exception("BltSoftImage failed");

				if (DX.DeleteSoftImage(softImage) != 0) // ? 失敗
					throw new Exception("DeleteSoftImage failed");

				softImage = newSoftImage;
			}

			int handle = DX.CreateGraphFromSoftImage(softImage);

			if (handle == -1) // ? 失敗
				throw new Exception("CreateGraphFromSoftImage failed");

			if (DX.DeleteSoftImage(softImage) != 0) // ? 失敗
				throw new Exception("DeleteSoftImage failed");

			return new Picture.PictureDataInfo()
			{
				Handle = handle,
				W = w,
				H = h,
			};
		}

		#region Font

		public static void AddFontFile(string resPath)
		{
			string dir = DU.WD.MakePath();
			string file = Path.Combine(dir, Path.GetFileName(resPath));
			byte[] fileData = DD.GetResFileData(resPath).Data.Value;

			SCommon.CreateDir(dir);
			File.WriteAllBytes(file, fileData);

			P_AddFontFile(file);

			DD.Finalizers.Add(() => P_RemoveFontFile(file));
		}

		private static void P_AddFontFile(string file)
		{
			if (Win32APIWrapper.W_AddFontResourceEx(file, Win32APIWrapper.FR_PRIVATE, IntPtr.Zero) == 0) // ? 失敗
				throw new Exception("W_AddFontResourceEx failed");
		}

		private static void P_RemoveFontFile(string file)
		{
			UnloadAllFontHandle(); // HACK: ざっくり過ぎる。

			if (Win32APIWrapper.W_RemoveFontResourceEx(file, Win32APIWrapper.FR_PRIVATE, IntPtr.Zero) == 0) // ? 失敗
				throw new Exception("W_RemoveFontResourceEx failed");
		}

		public static int GetFontHandle(string fontName, int fontSize)
		{
			if (string.IsNullOrEmpty(fontName))
				throw new Exception("Bad fontName");

			if (fontSize < 1 || SCommon.IMAX < fontSize)
				throw new Exception("Bad fontSize");

			return Fonts.GetHandle(fontName, fontSize);
		}

		public static void UnloadAllFontHandle()
		{
			Fonts.UnloadAll();
		}

		private static class Fonts
		{
			private static Dictionary<string, int> Handles = SCommon.CreateDictionary<int>();

			private static string GetKey(string fontName, int fontSize)
			{
				return string.Join("_", fontName, fontSize);
			}

			public static int GetHandle(string fontName, int fontSize)
			{
				string key = GetKey(fontName, fontSize);

				if (!Handles.ContainsKey(key))
					Handles.Add(key, CreateHandle(fontName, fontSize));

				return Handles[key];
			}

			public static void UnloadAll()
			{
				foreach (int handle in Handles.Values)
					ReleaseHandle(handle);

				Handles.Clear();
			}

			private static int CreateHandle(string fontName, int fontSize)
			{
				int handle = DX.CreateFontToHandle(
					fontName,
					fontSize,
					6,
					DX.DX_FONTTYPE_ANTIALIASING_8X8,
					-1,
					0
					);

				if (handle == -1) // ? 失敗
					throw new Exception("CreateFontToHandle failed");

				return handle;
			}

			private static void ReleaseHandle(int handle)
			{
				if (DX.DeleteFontToHandle(handle) != 0) // ? 失敗
					throw new Exception("DeleteFontToHandle failed");
			}
		}

		#endregion

		public static void UpdateButtonCounter(ref int counter, bool status)
		{
			if (1 <= counter) // ? 前回は押していた。
			{
				if (status) // ? 今回も押している。
				{
					counter++; // 押している。
				}
				else // ? 今回は離している。
				{
					counter = -1; // 離し始めた。
				}
			}
			else // ? 前回は離していた。
			{
				if (status) // ? 今回は押している。
				{
					counter = 1; // 押し始めた。
				}
				else // ? 今回も離している。
				{
					counter = 0; // 離している。
				}
			}
		}

		private const int POUND_FIRST_DELAY = 17;
		private const int POUND_AFTER_DELAY = 4;

		public static bool IsPound(int count)
		{
			return count == 1 || POUND_FIRST_DELAY < count && (count - POUND_FIRST_DELAY) % POUND_AFTER_DELAY == 1;
		}

		public static bool IsPound(int count, int firstDelay, int afterDelay)
		{
			return count == 1 || firstDelay < count && (count - firstDelay) % afterDelay == 1;
		}

		public static class SaveDataFileFormatter
		{
			private const int SEGMENT_SIZE = 80;

			public static byte[] Encode(byte[] data)
			{
				if (data == null)
					throw new Exception("Bad data");

				using (MemoryStream mem = new MemoryStream())
				{
					for (int index = 0; index < data.Length; index++)
					{
						if (1 <= index && index % SEGMENT_SIZE == 0)
						{
							mem.WriteByte(0x0d); // CR
							mem.WriteByte(0x0a); // LF
						}
						mem.WriteByte(data[index]);
					}
					if (data.Length % SEGMENT_SIZE != 0)
					{
						int count = SEGMENT_SIZE - data.Length % SEGMENT_SIZE;

						while (0 <= --count)
						{
							mem.WriteByte((byte)'!');
						}
					}
					mem.WriteByte(0x0d); // CR
					mem.WriteByte(0x0a); // LF

					return mem.ToArray();
				}
			}

			public static byte[] Decode(byte[] data)
			{
				if (data == null)
					throw new Exception("Bad data");

				return data.Where(chr => (byte)'+' <= chr && chr <= (byte)'z').ToArray();
			}
		}

		public static class Hasher
		{
			private static byte[] COUNTER_SHUFFLE = Encoding.ASCII.GetBytes("Gattonero-2023-04-05_COUNTER_SHUFFLE_{e43e01aa-ca4f-43d3-8be7-49cd60e9415e}_");
			private const int HASH_SIZE = 20;

			public static byte[] AddHash(byte[] data)
			{
				if (data == null)
					throw new Exception("Bad data");

				return SCommon.Join(new byte[][] { GetHash(data), data });
			}

			public static byte[] UnaddHash(byte[] data)
			{
				try
				{
					return UnaddHash_Main(data);
				}
				catch (Exception ex)
				{
					throw new Exception("読み込まれたデータは破損しているかバージョンが異なります。", ex);
				}
			}

			private static byte[] UnaddHash_Main(byte[] data)
			{
				if (data == null)
					throw new Exception("Bad data");

				if (data.Length < HASH_SIZE)
					throw new Exception("Bad Length");

				byte[] hash = SCommon.GetPart(data, 0, HASH_SIZE);
				byte[] retData = SCommon.GetPart(data, HASH_SIZE);
				byte[] recalcedHash = GetHash(retData);

				if (SCommon.Comp(hash, recalcedHash) != 0)
					throw new Exception("Bad hash");

				return retData;
			}

			private static byte[] GetHash(byte[] data)
			{
				byte[] hash = Encoding.ASCII.GetBytes(SCommon.Base64.I.Encode(SCommon.GetSHA512(new byte[][] { COUNTER_SHUFFLE, data }).Take(15).ToArray()));

				if (hash.Length != HASH_SIZE) // 2bs
					throw null; // never

				return hash;
			}
		}

		public class LzData
		{
			public readonly int Length;
			public readonly Lazy<byte[]> Data;

			public static LzData PhysicalFile(string file)
			{
				file = SCommon.ToFullPath(file);

				if (!File.Exists(file))
					throw new Exception("no file: " + file);

				FileInfo info = new FileInfo(file);

				if ((long)int.MaxValue < info.Length)
					throw new Exception("Bad file: " + file);

				return new LzData((int)info.Length, () => File.ReadAllBytes(file));
			}

			public LzData(int length, Func<byte[]> getData)
			{
				this.Length = length;
				this.Data = new Lazy<byte[]>(getData);
			}
		}
	}
}
