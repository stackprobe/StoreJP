using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0004
	{
		private StreamWriter ReportWriter;

		public void Test01()
		{
			ReportWriter = new StreamWriter(Path.Combine(SCommon.GetOutputDir(), "未使用リソース.txt"), false, Encoding.UTF8);

			// ----

			Search(@"C:\Dev\Game");
			//Search(@"C:\Dev");
			//Search(@"C:\DevBin");

			// ----

			ReportWriter.Dispose();
			ReportWriter = null;

			ProcMain.WriteLog("done!");
		}

		private void Search(string rootDir)
		{
			rootDir = SCommon.MakeFullPath(rootDir); // 2bs

			if (!Directory.Exists(rootDir))
				throw new Exception("no rootDir");

			Queue<string> q = new Queue<string>();

			q.Enqueue(rootDir);

			while (1 <= q.Count)
			{
				string dir = q.Dequeue();
				string[] subDirs = Directory.GetDirectories(dir);
				string projectDir = subDirs.FirstOrDefault(subDir => Regex.IsMatch(Path.GetFileName(subDir), "^[A-Za-z]+20200001$")); // Silvia20200001 etc.

				if (projectDir != null) // ? dir == DevプロジェクトDIR -> ゲームのプロジェクトであればチェックを行う。
				{
					string resourceDir = Path.Combine(dir, "Resource");
					string srcFileMusics = Path.Combine(projectDir, Path.GetFileName(projectDir), "Musics.cs");
					string srcFilePictures = Path.Combine(projectDir, Path.GetFileName(projectDir), "Pictures.cs");
					string srcFileSoundEffects = Path.Combine(projectDir, Path.GetFileName(projectDir), "SoundEffects.cs");

					if (
						Directory.Exists(resourceDir) &&
						File.Exists(srcFileMusics) &&
						File.Exists(srcFilePictures) &&
						File.Exists(srcFileSoundEffects)
						)
					{
						try
						{
							CheckUnusedResource(projectDir, srcFileMusics, srcFilePictures, srcFileSoundEffects, resourceDir);
						}
						catch (Exception ex)
						{
							ReportWriter.WriteLine(ex);
						}
					}
					else
					{
						if (
							Directory.Exists(resourceDir) ||
							File.Exists(srcFileMusics) ||
							File.Exists(srcFilePictures) ||
							File.Exists(srcFileSoundEffects)
							)
						{
							ReportWriter.WriteLine(dir + " <-- 破損したプロジェクト？");
						}
					}
				}
				else // ? dir != DevプロジェクトDIR -> サブディレクトリを探す。
				{
					foreach (string subDir in subDirs)
						q.Enqueue(subDir);
				}
			}
		}

		private const string TEMP_PROJECT_DIR = @"C:\temp\_TempCSSolution";

		private class ResourceDeclarationInfo
		{
			public string FilePath;
			public int Start;
			public int End;
		}

		private List<ResourceDeclarationInfo> ResourceDeclarations;
		private List<string> ResourcePathsSourceSide;

		private void CheckUnusedResource(string projectDir, string srcFileMusics, string srcFilePictures, string srcFileSoundEffects, string resourceDir)
		{
			ReportWriter.WriteLine(SCommon.ToParentPath(projectDir));

			ResourceDeclarations = new List<ResourceDeclarationInfo>();
			CollectResourceDeclarations(srcFileMusics);
			CollectResourceDeclarations(srcFilePictures);
			CollectResourceDeclarations(srcFileSoundEffects);

			// コピー元をクリーンする。
			SCommon.Batch(
				new string[]
				{
					"CALL C:\\Factory\\SetEnv.bat",
					"CALL qq",
				},
				projectDir,
				SCommon.StartProcessWindowStyle_e.MINIMIZED
				);

			SCommon.DeletePath(TEMP_PROJECT_DIR);
			SCommon.CopyDir(projectDir, TEMP_PROJECT_DIR);

			// コピー先をビルドする。
			SCommon.Batch(
				new string[]
				{
					"CALL C:\\Factory\\SetEnv.bat",
					"cx **",
				},
				TEMP_PROJECT_DIR,
				SCommon.StartProcessWindowStyle_e.MINIMIZED
				);

			string outputFile = Path.Combine(
				TEMP_PROJECT_DIR,
				Path.GetFileName(projectDir),
				"bin",
				"Release",
				Path.GetFileName(projectDir) + ".exe"
				);

			if (!File.Exists(outputFile))
				throw new Exception("ビルド失敗");

			foreach (ResourceDeclarationInfo info in ResourceDeclarations)
			{
				ProcMain.WriteLog(string.Join(" ", info.FilePath, info.Start, info.End));

				SCommon.DeletePath(TEMP_PROJECT_DIR);
				SCommon.CopyDir(projectDir, TEMP_PROJECT_DIR);

				string sourceFile = SCommon.ChangeRoot(info.FilePath, projectDir, TEMP_PROJECT_DIR);

				if (!File.Exists(sourceFile))
					throw new Exception("no sourceFile");

				string[] sourceLines = File.ReadAllLines(sourceFile, Encoding.UTF8);

				if (sourceLines.Length < info.End)
					throw new Exception("Bad info");

				// 当該リソース宣言を除去
				{
					string[] lines = sourceLines.ToArray(); // Clone

					for (int index = info.Start; index < info.End; index++)
						lines[index] = "";

					File.WriteAllLines(sourceFile, lines, Encoding.UTF8);
				}

				// コピー先をビルドする。
				SCommon.Batch(
					new string[]
					{
						"CALL C:\\Factory\\SetEnv.bat",
						"cx **",
					},
					TEMP_PROJECT_DIR,
					SCommon.StartProcessWindowStyle_e.MINIMIZED
					);

				if (File.Exists(outputFile)) // ? ビルド成功
				{
					ReportWriter.WriteLine("\t" + sourceLines[info.Start].Trim() + " <-- 不使用フィールド");
				}
			}
			ResourceDeclarations = null;

			ResourcePathsSourceSide = new List<string>();
			CollectResourcePathsSourceSide(srcFileMusics);
			CollectResourcePathsSourceSide(srcFilePictures);
			CollectResourcePathsSourceSide(srcFileSoundEffects);

			string[] unusedResFiles;

			// Set unusedResFiles
			{
				string[] resPathsResDirSide = GetResourcePathsResDirSide(resourceDir);

				List<string> only1 = new List<string>();
				List<string> both1 = new List<string>();
				List<string> both2 = new List<string>();
				List<string> only2 = new List<string>();

				SCommon.Merge(ResourcePathsSourceSide, resPathsResDirSide, SCommon.CompIgnoreCase, only1, both1, both2, only2);

				unusedResFiles = only2.ToArray();
			}

			foreach (string unusedResFile in unusedResFiles)
			{
				ReportWriter.WriteLine("\t" + unusedResFile + " <-- 不使用ファイル");
			}
			ResourcePathsSourceSide = null;

			ReportWriter.WriteLine("");
		}

		private void CollectResourceDeclarations(string srcFile)
		{
			string[] lines = File.ReadAllLines(srcFile, Encoding.UTF8);

			for (int index = 0; index < lines.Length; )
			{
				string line = lines[index];

				if (
					line.StartsWith("\t\tpublic static Music ") ||
					line.StartsWith("\t\tpublic static Music[") ||
					line.StartsWith("\t\tpublic static Picture ") ||
					line.StartsWith("\t\tpublic static Picture[") ||
					line.StartsWith("\t\tpublic static SoundEffect ") ||
					line.StartsWith("\t\tpublic static SoundEffect[")
					)
				{
					int end;

					if (RemoveCPPComment(lines[index + 1]) == "\t\t{") // ? 複数行のフィールド初期化_有り
					{
						for (int c = index + 2; ; c++)
						{
							if (RemoveCPPComment(lines[c]) == "\t\t};") // ? 複数行のフィールド初期化_閉じ
							{
								end = c + 1;
								break;
							}

							if (GetIndentCount(lines[c]) == 2)
								throw new Exception("フィールド初期化内に想定外のインデント幅の行");
						}
					}
					else // ? 複数行のフィールド初期化_無し
					{
						end = index + 1;
					}

					ResourceDeclarations.Add(new ResourceDeclarationInfo()
					{
						FilePath = srcFile,
						Start = index,
						End = end,
					});

					index = end;
				}
				else
				{
					index++;
				}
			}
		}

		private string RemoveCPPComment(string line) // ソースコードの行からコメント(C++)を除去
		{
			int p = line.IndexOf('/');

			if (p != -1)
			{
				line = line.Substring(0, p);
				line = line.TrimEnd();
			}
			return line;
		}

		private int GetIndentCount(string line) // ret: インデントの長さ
		{
			for (int c = 0; ; c++)
				if (line.Length <= c || line[c] != '\t')
					return c;
		}

		private void CollectResourcePathsSourceSide(string srcFile)
		{
			string[] lines = File.ReadAllLines(srcFile, Encoding.UTF8);

			foreach (string line in lines)
			{
				string[] encl = SCommon.ParseEnclosed(RemoveCPPComment(line), "@\"", "\"");

				if (encl != null)
				{
					string resPath = encl[2];

					if (!SCommon.IsFairRelPath(resPath, -1))
						throw new Exception("Bad resPath: " + resPath);

					ResourcePathsSourceSide.Add(resPath);
				}
			}
		}

		private string[] GetResourcePathsResDirSide(string dir)
		{
			List<string> dest = new List<string>();

			foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
			{
				string fileName = Path.GetFileName(file);

				if (SCommon.EqualsIgnoreCase(fileName, "_source.txt")) // 除外
					continue;

				dest.Add(SCommon.ChangeRoot(file, dir));
			}
			return dest.ToArray();
		}
	}
}
