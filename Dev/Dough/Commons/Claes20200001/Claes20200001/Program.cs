using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Charlotte.Commons;
using Charlotte.Tests;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4();
			}
			SCommon.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
			Main4();
			SCommon.Pause();
		}

		private void Main4()
		{
			try
			{
				Main5();
			}
			catch (Exception ex)
			{
				ProcMain.WriteLog(ex);
			}
		}

		private void Main5()
		{
			// -- choose one --

			//new Test0001().Test01(); // SCommon.Base32
			//new Test0001().Test02(); // SCommon.Base32
			//new Test0001().Test03(); // SCommon.Base32
			//new Test0002().Test01(); // SCommon.Base64
			//new Test0002().Test02(); // SCommon.Base64
			//new Test0002().Test03(); // SCommon.Base64
			//new Test0003().Test01(); // RandomUnit
			//new Test0003().Test02(); // RandomUnit
			//new Test0003().Test03(); // RandomUnit
			//new Test0003().Test04(); // RandomUnit
			//new Test0003().Test05(); // RandomUnit
			//new Test0004().Test01(); // SCommon.Serializer
			//new Test0005().Test01(); // SCommon.Hex
			//new Test0005().Test02(); // SCommon.Hex
			//new Test0006().Test01(); // SCommon.TimeStampToSec
			//new Test0006().Test02(); // SCommon.TimeStampToSec
			//new Test0007().Test01(); // WorkingDir
			//new Test0008().Test01(); // SCommon.GetIndex
			//new Test0008().Test02(); // SCommon.GetIndex
			//new Test0008().Test03(); // SCommon.GetIndex
			//new Test0009().Test01(); // SCommon.GetRange
			//new Test0009().Test02(); // SCommon.GetRange
			//new Test0009().Test03(); // SCommon.GetRange
			//new Test0009().Test04(); // SCommon.GetRange
			//new Test0010().Test01(); // SCommon.Tokenize
			//new Test0010().Test02(); // SCommon.Tokenize
			//new Test0010().Test03(); // SCommon.Tokenize
			//new Test0010().Test04(); // SCommon.Tokenize
			//new Test0011().Test01(); // SCommon.Generate
			//new Test0012().Test01(); // SCommon.SimpleDateTime
			//new Test0013().Test01(); // SCommon.GetSJISBytes
			//new Test0014().Test01(); // SCommon.Batch
			//new Test0015().Test01(); // SCommon.Compress, SCommon.Decompress
			//new Test0015().Test02(); // SCommon.CompressFile, SCommon.DecompressFile
			//new Test0016().Test01(); // SCommon.ToCreatablePath
			//new Test0017().Test01(); // SCommon.ParseEnclosed 使用例
			new Test0018().Test01(); // SCommon.GetEnclosed 使用例

			// --
		}
	}
}
