using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// 音楽リソース
	/// このクラスのインスタンスはプロセスで有限個であること。
	/// 原則的に以下のクラスの静的フィールドとして植え込むこと。
	/// -- Musics
	/// </summary>
	public class Music
	{
		private static DU.Collector<Music> Instances = new DU.Collector<Music>();

		public static void Touch()
		{
			foreach (Music instance in Instances.Iterate())
			{
				if (instance.SmallResourceFlag == null)
					instance.SmallResourceFlag = instance.FileDataGetter().Length <= GameConfig.MaxSizeOfSmallResource;

				if (instance.SmallResourceFlag.Value)
					instance.GetHandle();
				else
					instance.TryUnload();
			}
		}

		public static void TryUnloadAll()
		{
			foreach (Music instance in Instances.Iterate())
				instance.TryUnload();
		}

		private bool? SmallResourceFlag = null;

		private Func<DU.LzData> FileDataGetter;
		private Action<Music> PostLoaded = instance => { };

		private int Handle; // -1 == 未ロード

		/// <summary>
		/// リソースからループありの音楽をロードする。
		/// </summary>
		/// <param name="resPath">リソースのパス</param>
		/// <param name="loopStartPosition">ループ開始位置</param>
		/// <param name="loopLength">ループの長さ</param>
		public Music(string resPath, long loopStartPosition, long loopLength)
			: this(resPath)
		{
			long loopEndPosition = loopStartPosition + loopLength;

			this.PostLoaded = instance =>
			{
				if (DX.SetLoopSamplePosSoundMem(loopStartPosition, this.Handle) != 0) // ? 失敗 // ループ開始位置
					throw new Exception("SetLoopSamplePosSoundMem failed");

				if (DX.SetLoopStartSamplePosSoundMem(loopEndPosition, this.Handle) != 0) // ? 失敗 // ループ終了位置
					throw new Exception("SetLoopStartSamplePosSoundMem failed");
			};
		}

		/// <summary>
		/// リソースから音楽をロードする。
		/// </summary>
		/// <param name="resPath">リソースのパス</param>
		public Music(string resPath)
			: this(() => DD.GetResFileData(resPath))
		{ }

		/// <summary>
		/// 音楽データの取得メソッドから音楽をロードする。
		/// </summary>
		/// <param name="getFileData">音楽データの取得メソッド</param>
		public Music(Func<DU.LzData> getFileData)
		{
			this.FileDataGetter = getFileData;
			this.Handle = -1;

			Instances.Add(this);
		}

		public int GetHandle()
		{
			if (this.Handle == -1)
			{
				byte[] fileData = this.FileDataGetter().Data.Value;
				int handle = -1;

				DU.PinOn(fileData, p => handle = DX.LoadSoundMemByMemImage(p, (ulong)fileData.Length));

				if (handle == -1) // ? 失敗
					throw new Exception("LoadSoundMemByMemImage failed");

				this.Handle = handle;
				this.PostLoaded(this);
			}
			return this.Handle;
		}

		public void TryUnload()
		{
			if (this.Handle != -1)
			{
				if (DU.IsSoundPlaying(this.Handle)) // ? 再生中 -> アンロードしない。
					return;

				if (DX.DeleteSoundMem(this.Handle) != 0) // ? 失敗
					throw new Exception("DeleteSoundMem failed");

				this.Handle = -1;
			}
		}

		private static Queue<Func<bool>> TaskSequence = new Queue<Func<bool>>();
		private static int LastVolume = -1;

		public static void EachFrame()
		{
			if (!DD.ExecuteTaskSequence(TaskSequence) && Playing != null) // ? アイドル状態 && 再生中
			{
				int volume = DD.RateToByte(GameSetting.MusicVolume);

				if (LastVolume != volume) // ? 前回の音量と違う -> 音量が変更されたので、新しい音量を適用する。
				{
					if (DX.ChangeVolumeSoundMem(DD.RateToByte(GameSetting.MusicVolume), Playing.GetHandle()) != 0) // ? 失敗
						throw new Exception("ChangeVolumeSoundMem failed");

					LastVolume = volume;
				}
			}
		}

		private static Music Playing = null;

		public void Play()
		{
			if (Playing != this)
			{
				FadeOut();
				TaskSequence.Enqueue(SCommon.Supplier(this.E_Play()));
				Playing = this;
			}
		}

		public static void FadeOut(int frameMax = 60)
		{
			if (Playing != null)
			{
				TaskSequence.Enqueue(SCommon.Supplier(Playing.E_FadeOut(frameMax)));
				Playing = null;
			}
		}

		private IEnumerable<bool> E_Play()
		{
			if (DX.ChangeVolumeSoundMem(0, this.GetHandle()) != 0) // ? 失敗
				throw new Exception("ChangeVolumeSoundMem failed");

			yield return true;

			if (DX.PlaySoundMem(this.GetHandle(), DX.DX_PLAYTYPE_LOOP, 1) != 0) // ? 失敗
				throw new Exception("PlaySoundMem failed");

			yield return true;
			yield return true;
			yield return true;

			if (DX.ChangeVolumeSoundMem(DD.RateToByte(GameSetting.MusicVolume), this.GetHandle()) != 0) // ? 失敗
				throw new Exception("ChangeVolumeSoundMem failed");

			yield return true;
		}

		private IEnumerable<bool> E_FadeOut(int frameMax)
		{
			foreach (Scene scene in Scene.Create(frameMax))
			{
				if (DX.ChangeVolumeSoundMem(DD.RateToByte(GameSetting.MusicVolume * (1.0 - scene.Rate)), this.GetHandle()) != 0) // ? 失敗
					throw new Exception("ChangeVolumeSoundMem failed");

				yield return true;
			}

			if (DX.StopSoundMem(this.GetHandle()) != 0) // ? 失敗
				throw new Exception("StopSoundMem failed");

			yield return true;
		}
	}
}
