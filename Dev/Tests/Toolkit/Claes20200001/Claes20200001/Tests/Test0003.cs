using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0003
	{
		private class FileDataInfo
		{
			public string StrPath;
			public long Length;
			public DateTime LastWriteTime;
			public bool LastWriteTimeChanged = false;

			public FileDataInfo(string file)
			{
				file = SCommon.MakeFullPath(file); // 2bs

				FileInfo info = new FileInfo(file);

				this.StrPath = file;
				this.Length = info.Length;
				this.LastWriteTime = info.LastWriteTime;
			}

			private string _hash = null;

			private string Hash
			{
				get
				{
					if (_hash == null)
					{
						ProcMain.WriteLog("Calculating hash...");
						ProcMain.WriteLog("< " + this.StrPath);

						_hash = SCommon.Hex.I.GetString(SCommon.GetSHA512File(this.StrPath));

						ProcMain.WriteLog("> " + _hash);
					}
					return _hash;
				}
			}

			public bool IsSameFile(FileDataInfo another)
			{
				return
					this.Length == another.Length &&
					this.Hash == another.Hash;
			}
		}

		private List<FileDataInfo> HomeFiles = new List<FileDataInfo>();
		private List<FileDataInfo> DevEnvFiles = new List<FileDataInfo>();

		public void Test01()
		{
			CollectFileData(HomeFiles, @"C:\home\Resource");
			CollectResourceFileData(DevEnvFiles, @"C:\Dev");
			CollectResourceFileData(DevEnvFiles, @"C:\DevBin");

			Sort(HomeFiles);
			Sort(DevEnvFiles);

			foreach (FileDataInfo deFile in DevEnvFiles)
			{
				foreach (FileDataInfo hmFile in HomeFiles)
				{
					if (deFile.IsSameFile(hmFile)) // ? 同じファイル
					{
						if (deFile.LastWriteTime != hmFile.LastWriteTime) // 更新日時が違っていれば更新する。
						{
							deFile.LastWriteTime = hmFile.LastWriteTime;
							deFile.LastWriteTimeChanged = true;
						}
						break; // 同じファイルが見つかったので、この deFile については検索終了
					}
				}
			}

			SCommon.ForEachPair(DevEnvFiles, (a, b) =>
			{
				if (a.IsSameFile(b)) // ? 同じファイル
				{
					if (a.LastWriteTime != b.LastWriteTime) // 更新日時が違っていれば更新する。
					{
						if (a.LastWriteTime < b.LastWriteTime) // ? aの方が古い(aの方がオリジナル) -> bを更新する。
						{
							b.LastWriteTime = a.LastWriteTime;
							b.LastWriteTimeChanged = true;
						}
						else // ? bの方が古い(bの方がオリジナル) -> aを更新する。
						{
							a.LastWriteTime = b.LastWriteTime;
							a.LastWriteTimeChanged = true;
						}
					}
				}
			});

			foreach (FileDataInfo deFile in DevEnvFiles)
			{
				if (deFile.LastWriteTimeChanged)
				{
					ProcMain.WriteLog("Change LastWriteTime");
					ProcMain.WriteLog("< " + deFile.LastWriteTime);
					ProcMain.WriteLog("> " + deFile.StrPath);

					new FileInfo(deFile.StrPath).LastWriteTime = deFile.LastWriteTime;
				}
			}
			ProcMain.WriteLog("done!");
		}

		private void CollectResourceFileData(List<FileDataInfo> dest, string rootDir)
		{
			rootDir = SCommon.MakeFullPath(rootDir);

			if (!Directory.Exists(rootDir))
				throw new Exception("no rootDir");

			Queue<string> q = new Queue<string>();

			q.Enqueue(rootDir);

			while (1 <= q.Count)
			{
				string dir = q.Dequeue();
				string resDir = Path.Combine(dir, "Resource");

				if (Directory.Exists(resDir))
				{
					CollectFileData(dest, resDir);
				}
				else
				{
					foreach (string subDir in Directory.GetDirectories(dir))
						q.Enqueue(subDir);
				}
			}
		}

		private void CollectFileData(List<FileDataInfo> dest, string rootDir)
		{
			rootDir = SCommon.MakeFullPath(rootDir);

			if (!Directory.Exists(rootDir))
				throw new Exception("no rootDir");

			foreach (string file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
				dest.Add(new FileDataInfo(file));
		}

		private void Sort(List<FileDataInfo> files)
		{
			files.Sort((a, b) => SCommon.CompIgnoreCase(a.StrPath, b.StrPath));
		}
	}
}
