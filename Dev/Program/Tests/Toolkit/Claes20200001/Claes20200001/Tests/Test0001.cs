using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	public class Test0001
	{
		private const string INPUT_ROOT_DIR = @"C:\temp";

		public void Test01()
		{
			foreach (string file in Directory.GetFiles(INPUT_ROOT_DIR))
			{
				Console.WriteLine("< " + file); // cout

				if (Path.GetExtension(file).ToLower() != ".html")
					throw null;

				string name = Path.GetFileName(file);

				{
					int p = name.IndexOf(" _ ");

					if (p == -1)
						throw null;

					name = name.Substring(0, p);
				}

				if (name == "")
					throw null;

				// old -- 理事長とか漢字ひらがなを含むので
				/*
				if (name.Any(v => !Common.IsMbcKana(v)))
					throw null;
				//*/

				if (name.Any(v => !Common.IsMbcChar(v)))
					throw null;

				string text = File.ReadAllText(file, Encoding.UTF8);

				for (int start = 0; ; )
				{
					string searchPtn = "\" alt=\"" + name + " ";
					int p = text.IndexOf(searchPtn, start);

					if (p == -1)
						break;

					int o = p - 1;
					int q = p + searchPtn.Length;
					int r = q;

					for (; text[o] != '"'; o--) ;
					for (; text[r] != '"'; r++) ;

					o++;

					string imageFile = text.Substring(o, p - o);
					imageFile = SCommon.MakeFullPath(Path.Combine(INPUT_ROOT_DIR, imageFile));
					string imageKind = text.Substring(q, r - q);

					if (!File.Exists(imageFile))
						throw null;

					if (Path.GetExtension(imageFile).ToLower() != ".png")
						throw null;

					if (imageKind == "")
						imageKind = "イラスト";

					imageKind = imageKind.Replace("&lt;small&gt;STARTING&lt;br&gt;FUTURE&lt;/small&gt;", "スターティングフューチャー");

					if (imageKind.Any(v => SCommon.GetJChars().IndexOf(v) == -1))
						throw null;

					// -- choose one --

					//string destFileName = name + "_" + imageKind + ".png";
					string destFileName = imageKind + "_" + name + ".png";

					// --

					File.Copy(imageFile, Path.Combine(SCommon.GetOutputDir(), destFileName));

					// ====
					// ====
					// ====

					start = r + 1;
				}
			}

			// 画像の編集
			foreach (string file in Directory.GetFiles(SCommon.GetOutputDir()))
			{
				Console.WriteLine("* " + file); // cout

				Canvas canvas = Canvas.LoadFromFile(file);

				{
					Canvas dest = new Canvas(canvas.W, canvas.H);

					dest.Fill(new I4Color(255, 255, 255, 255));
					dest.DrawImage(canvas, 0, 0, true);

					canvas = dest;
				}

				canvas = canvas.SetMargin((dot, x, y) => dot.R == 255 && dot.G == 255 && dot.B == 255, new I4Color(255, 255, 255, 255), 10);
				canvas.Save(file);
			}
		}
	}
}
