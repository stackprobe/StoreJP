using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// 画像リソース
	/// このクラスのインスタンスはプロセスで有限個であること。
	/// 原則的に以下のクラスの静的フィールドとして植え込むこと。
	/// -- Pictures
	/// </summary>
	public class Picture
	{
		private static DU.Collector<Picture> Instances = new DU.Collector<Picture>();

		public static void Touch()
		{
			foreach (Picture instance in Instances.Iterate())
			{
				if (instance.SmallResourceFlag == null)
					instance.SmallResourceFlag = instance.SmallResourceFlagGetter == null ? false : instance.SmallResourceFlagGetter();

				if (instance.SmallResourceFlag.Value)
					instance.GetHandle();
				else
					instance.Unload();
			}
		}

		public static void UnloadAll()
		{
			foreach (Picture instance in Instances.Iterate())
				instance.Unload();
		}

		private bool? SmallResourceFlag = null;
		private Func<bool> SmallResourceFlagGetter = null; // null == 無効

		public class PictureDataInfo
		{
			public int Handle;
			public int W;
			public int H;
		}

		private Func<PictureDataInfo> PictureDataGetter;
		private Action<int> HandleUnloader;

		private PictureDataInfo PictureData = null;

		/// <summary>
		/// リソースから画像を作成する。
		/// </summary>
		/// <param name="resPath">リソースのパス</param>
		public Picture(string resPath)
			: this(() => DD.GetResFileData(resPath))
		{
			this.SmallResourceFlagGetter = () => DD.GetResFileData(resPath).Length <= GameConfig.MaxSizeOfSmallResource;
		}

		/// <summary>
		/// 画像データの取得メソッドから画像を作成する。
		/// </summary>
		/// <param name="getFileData">画像データの取得メソッド</param>
		public Picture(Func<DU.LzData> getFileData)
			: this(() => DU.GetPictureData(getFileData().Data.Value))
		{ }

		/// <summary>
		/// ローダーを指定して画像を作成する。
		/// </summary>
		/// <param name="loader">ローダー</param>
		public Picture(Func<PictureDataInfo> loader)
			: this(loader, handle =>
			{
				if (DX.DeleteGraph(handle) != 0) // ? 失敗
					throw new Exception("DeleteGraph failed");
			})
		{ }

		/// <summary>
		/// ローダーとアンローダーを指定して画像を作成する。
		/// </summary>
		/// <param name="loader">ローダー</param>
		/// <param name="unloader">アンローダー</param>
		public Picture(Func<PictureDataInfo> loader, Action<int> unloader)
		{
			this.PictureDataGetter = loader;
			this.HandleUnloader = unloader;

			Instances.Add(this);
		}

		private PictureDataInfo GetPictureData()
		{
			if (this.PictureData == null)
				this.PictureData = this.PictureDataGetter();

			return this.PictureData;
		}

		public void Unload()
		{
			if (this.PictureData != null)
			{
				this.HandleUnloader(this.PictureData.Handle);
				this.PictureData = null;
			}
		}

		public int GetHandle()
		{
			return this.GetPictureData().Handle;
		}

		public int W
		{
			get
			{
				return this.GetPictureData().W;
			}
		}

		public int H
		{
			get
			{
				return this.GetPictureData().H;
			}
		}
	}
}
