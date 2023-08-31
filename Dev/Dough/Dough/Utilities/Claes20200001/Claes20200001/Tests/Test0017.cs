﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Utilities;

namespace Charlotte.Tests
{
	/// <summary>
	/// PrimeTester テスト
	/// </summary>
	public class Test0017
	{
		public void Test01()
		{
			Test01_a(1000000, 1000000);
			Test01_a(100000, 5000000000); // == UINT_MAX * 1.164*
			Test01_a(10000, 9000000000000); // == UINT_MAX * 2095.47579*
			Test01_a(1000, 1000000000000000); // == ULONG_MAX * 0.0000542*
			Test01_a(300, 100000000000000000); // == ULONG_MAX * 0.00542*

			ProcMain.WriteLog("OK!");
		}

		private void Test01_a(int testCount, ulong scale)
		{
			ProcMain.WriteLog(string.Join(", ", "TEST-0017-01", testCount, scale)); // cout

			int countOfPrime = 0;

			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				ulong n = SCommon.CRandom.GetULong(scale);

				bool ans1 = Test01_b1(n);
				bool ans2 = Test01_b2(n);

				if (ans1 != ans2)
					throw null;

				if (ans1)
					countOfPrime++;
			}
			ProcMain.WriteLog(countOfPrime);
			ProcMain.WriteLog("OK");
		}

		private bool Test01_b1(ulong n)
		{
			return PrimeTester.IsPrime(n);
		}

		private bool Test01_b2(ulong n)
		{
			if (n < 2)
				return false;

			if (n == 2)
				return true;

			if (n % 2 == 0)
				return false;

			for (ulong d = 3; d * d <= n; d += 2)
				if (n % d == 0)
					return false;

			return true;
		}

		// ====

		public void Test02()
		{
			Test02_a(30, 1000000000); // == UINT_MAX * 0.23283*
			Test02_a(30, 10000000000); // == UINT_MAX * 2.3283*
			Test02_a(30, 100000000000);
			Test02_a(30, 1000000000000);
			Test02_a(30, 10000000000000);
			Test02_a(30, 100000000000000);
			Test02_a(30, 1000000000000000);
			Test02_a(30, 10000000000000000);
			Test02_a(20, 100000000000000000); // == ULONG_MAX * 0.00542*
			Test02_a(10, 300000000000000000); // == ULONG_MAX * 0.01626*
			Test02_a(3, 1000000000000000000); // == ULONG_MAX * 0.05421*
			Test02_a(3, 3000000000000000000); // == ULONG_MAX * 0.16263*
			Test02_a(3, 10000000000000000000); // == ULONG_MAX * 0.5421*
			Test02_a(3, 18440000000000000000); // == ULONG_MAX * 0.9996*

			ProcMain.WriteLog("OK!");
		}

		private void Test02_a(int bothCountLimit, ulong scale)
		{
			ProcMain.WriteLog(string.Join(", ", "TEST-0017-02", bothCountLimit, scale)); // cout

			int countOfPrime = 0;
			int countOfComposite = 0;

			while (countOfPrime < bothCountLimit || countOfComposite < bothCountLimit)
			{
				ulong n = SCommon.CRandom.GetULong(scale);

				bool ans1 = Test02_b1(n);
				bool ans2 = Test02_b2(n);

				if (ans1 != ans2)
					throw null;

				if (ans1)
					countOfPrime++;
				else
					countOfComposite++;
			}
			ProcMain.WriteLog("P : C == " + countOfPrime + " : " + countOfComposite);
			ProcMain.WriteLog("OK");
		}

		private bool Test02_b1(ulong n)
		{
			return PrimeTester.IsPrime(n);
		}

		private bool Test02_b2(ulong n)
		{
			if (n < 2)
				return false;

			if (n == 2)
				return true;

			if (n % 2 == 0)
				return false;

			for (ulong d = 3; d * d <= n; d += 2)
				if (n % d == 0)
					return false;

			return true;
		}

		// ====

		public void Test03()
		{
			ulong PRIMES_RANGE_MIN = 18446744073709550000;
			ulong PRIMES_RANGE_MAX = 18446744073709551615; // ULONG_MAX

			// PRIMES_RANGE_MIN 以上 PRIMES_RANGE_MAX 以下の範囲の素数一覧
			// -- by Prime4096 @ 2022.9.17
			//
			ulong[] PRIMES = new ulong[]
			{
				18446744073709550009,
				18446744073709550033,
				18446744073709550047,
				18446744073709550099,
				18446744073709550111,
				18446744073709550129,
				18446744073709550141,
				18446744073709550147,
				18446744073709550237,
				18446744073709550293,
				18446744073709550341,
				18446744073709550381,
				18446744073709550537,
				18446744073709550539,
				18446744073709550591,
				18446744073709550593,
				18446744073709550671,
				18446744073709550681,
				18446744073709550717,
				18446744073709550719,
				18446744073709550771,
				18446744073709550773,
				18446744073709550791,
				18446744073709550873,
				18446744073709551113,
				18446744073709551163,
				18446744073709551191,
				18446744073709551253,
				18446744073709551263,
				18446744073709551293,
				18446744073709551337,
				18446744073709551359,
				18446744073709551427,
				18446744073709551437,
				18446744073709551521,
				18446744073709551533,
				18446744073709551557,
			};

			for (ulong n = PRIMES_RANGE_MIN; ; n++)
			{
				bool ans1 = PrimeTester.IsPrime(n);
				bool ans2 = PRIMES.Contains(n);

				Console.WriteLine(n + " --> " + ans1 + ", " + ans2);

				if (ans1 != ans2)
					throw null;

				if (n == PRIMES_RANGE_MAX)
					break;
			}

			Console.WriteLine("OK! (TEST-0017-03)");
		}
	}
}
