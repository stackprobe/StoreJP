using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// SCommon.ParseEnclosed
	/// </summary>
	public class Test0017
	{
		private static string RES_TEXT = @"

<!DOCTYPE html>
<html>
	<head>
		<title>Example of Strong Tag</title>
	</head>
	<body>
		<h1>Emphasizing Text</h1>
		<p>There is an <strong>important</strong> section in this document.</p>
		<p>In this paragraph, we will use a word that we want to <strong>emphasize</strong>.</p>
		<p>Finally, we have something to say <strong>loud and clear</strong>.</p>
	</body>
</html>

";

		public void Test01()
		{
			string text = RES_TEXT;

			for (; ; )
			{
				string[] encl = SCommon.ParseEnclosed(text, "<strong>", "</strong>"); // 次の <strong> ... </strong> を探す。

				if (encl == null) // ? 見つからなかった。-> 検索終了
					break;

				string innerText = encl[2]; // <strong> と </strong> の間の部分

				Console.WriteLine("innerText = \"" + innerText + "\"");

				text = encl[4]; // </strong> 以降
			}
			Console.WriteLine("done! (TEST-0017-01)");
		}
	}
}
