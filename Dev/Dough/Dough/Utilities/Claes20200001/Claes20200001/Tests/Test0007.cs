using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// CsvFileReader, CsvFileWriter テスト
	/// </summary>
	public class Test0007
	{
		private static string RES_TEST_CSV = @"

ID,Product_Name,Price,Stock_Quantity
1,Widget A,10.99,100
2,Widget B,15.49,75
3,Widget C,8.75,120
4,Widget D,5.99,200
5,Widget E,12.00,50

日本語,123,abcdef,
いろはにほへと,,
,,,
end

";

		public void Test01()
		{
			string testCsv = RES_TEST_CSV.Trim();

			using (WorkingDir wd = new WorkingDir())
			{
				string csvFile = wd.MakePath() + ".csv";
				string destCsvFile = wd.MakePath() + ".csv";
				string[][] rows;

				File.WriteAllText(csvFile, testCsv, SCommon.ENCODING_SJIS);

				using (CsvFileReader reader = new CsvFileReader(csvFile))
				{
					rows = reader.ReadToEnd();
				}

				using (CsvFileWriter writer = new CsvFileWriter(destCsvFile))
				{
					writer.WriteRows(rows);
				}

				string destCsv = File.ReadAllText(destCsvFile, SCommon.ENCODING_SJIS);
				destCsv = destCsv.Trim();
				destCsv = destCsv.Replace("\n", "\r\n"); // CsvFileWriter の改行は LF なので CR-LF に置き換える。

				if (testCsv != destCsv)
					throw null;
			}

			Console.WriteLine("OK! (TEST-0007-01)");
		}
	}
}
