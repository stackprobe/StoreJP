﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// ArraySpliceSequencer テスト
	/// </summary>
	public class Test0015
	{
		public void Test01()
		{
			Console.WriteLine("TEST-0015-01");

			Test01_a(10, 1000);
			Test01_a(100, 100);
			Test01_a(1000, 10);

			Console.WriteLine("OK!");
		}

		public void Test01_a(int testDataScale, int testCount)
		{
			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				Test01_b(testDataScale);
			}
			Console.WriteLine("OK");
		}

		private void Test01_b(int testDataScale)
		{
			string[] words = new string[]
			{
				"Sio",
				"Miso",
				"Syoyu",
				"Hakata",
				"Sapporo",
				"Kitakata",
			};

			string src = string.Join("", SCommon.Generate(SCommon.CRandom.GetInt(testDataScale), () => SCommon.CRandom.ChooseOne(words)));
			string word1 = SCommon.CRandom.ChooseOne(words);
			string word2 = SCommon.CRandom.ChooseOne(words);

			string ans1 = src.Replace(word1, word2);
			string ans2;

			{
				ArraySpliceSequencer<char> ass = new ArraySpliceSequencer<char>(src.ToArray());
				int index = 0;

				for (; ; )
				{
					index = src.IndexOf(word1, index);

					if (index == -1)
						break;

					ass.Splice(index, word1.Length, word2.ToArray());
					index += word1.Length;
				}
				ans2 = new string(ass.GetArray());
			}

			if (ans1 != ans2) // ? 不一致
				throw null;
		}

		public void Test02()
		{
			Console.WriteLine("TEST-0015-02");

			Test02_a(100, 10000);
			Test02_a(300, 3000);
			Test02_a(1000, 1000);
			Test02_a(3000, 300);
			Test02_a(10000, 100);

			Console.WriteLine("OK!");
		}

		public void Test02_a(int testDataScale, int testCount)
		{
			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				Test02_b(testDataScale, testDataScale);
				Test02_b(testDataScale, testDataScale / 3);
				Test02_b(testDataScale, testDataScale / 10);
				Test02_b(testDataScale, testDataScale / 30);
			}
			Console.WriteLine("OK");
		}

		private void Test02_b(int testDataScale, int subTestDataScale)
		{
			if (subTestDataScale < 2) throw null; // 2bs

			char[] TEST_CHARS = SCommon.HALF.ToArray();
			string src = new string(SCommon.Generate(
				SCommon.CRandom.GetInt(testDataScale),
				() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

			string ans1 = src;
			string ans2;

			int index1 = 0;
			int index2 = 0;

			ArraySpliceSequencer<char> ass = new ArraySpliceSequencer<char>(src.ToArray());

			for (; ; )
			{
				int span = SCommon.CRandom.GetInt(subTestDataScale);

				index1 += span;
				index2 += span;

				int removeLength = SCommon.CRandom.GetInt(subTestDataScale);

				if (ans1.Length < index1 + removeLength)
					break;

				string newPart = new string(SCommon.Generate(
					SCommon.CRandom.GetInt(subTestDataScale),
					() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

				ans1 = ans1.Substring(0, index1) + newPart + ans1.Substring(index1 + removeLength);
				ass.Splice(index2, removeLength, newPart.ToArray());

				index1 += newPart.Length;
				index2 += removeLength;
			}
			ans2 = new string(ass.GetArray());

			if (ans1 != ans2) // ? 不一致
				throw null;
		}

		public void Test03()
		{
			Console.WriteLine("TEST-0015-03");

			Test03_a(100, 30000);
			Test03_a(300, 10000);
			Test03_a(1000, 3000);
			Test03_a(3000, 1000);
			Test03_a(10000, 300);

			Console.WriteLine("OK!");
		}

		public void Test03_a(int testDataScale, int testCount)
		{
			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				Test03_b(testDataScale, testDataScale);
				Test03_b(testDataScale, testDataScale / 3);
				Test03_b(testDataScale, testDataScale / 10);
				Test03_b(testDataScale, testDataScale / 30);
			}
			Console.WriteLine("OK");
		}

		private void Test03_b(int testDataScale, int subTestDataScale)
		{
			if (subTestDataScale < 2) throw null; // 2bs

			char[] TEST_CHARS = SCommon.HALF.ToArray();
			string src = new string(SCommon.Generate(
				SCommon.CRandom.GetInt(testDataScale),
				() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

			string ans1 = src;
			string ans2;

			int index1 = 0;
			int index2 = 0;

			ArraySpliceSequencer<char> ass = new ArraySpliceSequencer<char>(src.ToArray());

			for (; ; )
			{
				int span = SCommon.CRandom.GetInt(subTestDataScale);

				index1 += span;
				index2 += span;

				int removeLength = SCommon.CRandom.GetInt(subTestDataScale);

				if (ans1.Length < index1 + removeLength)
					break;

				switch (SCommon.CRandom.GetInt(3))
				{
					case 0:
						{
							string newPart = new string(SCommon.Generate(
								SCommon.CRandom.GetInt(subTestDataScale),
								() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

							ans1 = ans1.Substring(0, index1) + newPart + ans1.Substring(index1 + removeLength);
							ass.Splice(index2, removeLength, newPart.ToArray());

							index1 += newPart.Length;
							index2 += removeLength;
						}
						break;

					case 1:
						{
							int readStart = SCommon.CRandom.GetInt(subTestDataScale);

							string newPart = new string(SCommon.Generate(
								SCommon.CRandom.GetInt(subTestDataScale) + readStart,
								() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

							ans1 = ans1.Substring(0, index1) + newPart.Substring(readStart) + ans1.Substring(index1 + removeLength);
							ass.Splice(index2, removeLength, newPart.ToArray(), readStart);

							index1 += newPart.Length - readStart;
							index2 += removeLength;
						}
						break;

					case 2:
						{
							int readStart = SCommon.CRandom.GetInt(subTestDataScale);
							int readLength = SCommon.CRandom.GetInt(subTestDataScale);

							string newPart = new string(SCommon.Generate(
								SCommon.CRandom.GetInt(subTestDataScale) + readStart + readLength,
								() => SCommon.CRandom.ChooseOne(TEST_CHARS)).ToArray());

							ans1 = ans1.Substring(0, index1) + newPart.Substring(readStart, readLength) + ans1.Substring(index1 + removeLength);
							ass.Splice(index2, removeLength, newPart.ToArray(), readStart, readLength);

							index1 += readLength;
							index2 += removeLength;
						}
						break;

					default:
						throw null; // never
				}
			}
			ans2 = new string(ass.GetArray());

			if (ans1 != ans2) // ? 不一致
				throw null;
		}

		public void Test04()
		{
			char[] aRet = new ArraySpliceSequencer<char>("ABCxxxxxEFGHzzMN".ToArray())
				.Splice(3, 5, "D".ToArray())
				.Splice(12, 2, "IJKL".ToArray())
				.GetArray();
			string ret = new string(aRet);

			if (ret != "ABCDEFGHIJKLMN")
				throw null;

			// ----

			aRet = new ArraySpliceSequencer<char>(new char[0])
				.Splice(0, 0, "YYYY".ToArray())
				.Splice(0, 0, "/MM/DD".ToArray())
				.Splice(0, 0, " ".ToArray())
				.Splice(0, 0, "HH:".ToArray())
				.Splice(0, 0, "II:SS".ToArray())
				.GetArray();
			ret = new string(aRet);

			if (ret != "YYYY/MM/DD HH:II:SS")
				throw null;

			// ----

			Console.WriteLine("OK! (TEST-0015-04)");
		}
	}
}
