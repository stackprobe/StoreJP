/*
	if n < 18,446,744,073,709,551,616 = 2^64, it is enough to test a = 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, and 37. @ Wikipedia

----

https://zh.wikipedia.org/wiki/%E7%B1%B3%E5%8B%92-%E6%8B%89%E5%AE%BE%E6%A3%80%E9%AA%8C#%E7%AE%97%E6%B3%95%E5%A4%8D%E6%9D%82%E5%BA%A6

Input #1: n > 3, an odd integer to be tested for primality;
Input #2: k, a parameter that determines the accuracy of the test
Output: composite if n is composite, otherwise probably prime

write n - 1 as (2^r)*d with d odd by factoring powers of 2 from n - 1
WitnessLoop: repeat k times:
	pick a random integer a in the range [2, n - 2]
	x <- a^d mod n
	if x = 1 or x = n - 1 then
		continue WitnessLoop
	repeat r - 1 times:
		x <- x^2 mod n
		if x = n - 1 then
			continue WitnessLoop
	return composite
return probably prime

*/

#include "Prime3.h"

static uint64 ModMul(uint64 a, uint64 b, uint64 modulo)
{
	uint a4[4];
	uint b4[4];
	uint c4[4];
	uint m4[4];
	UI128_t aa;
	UI128_t bb;
	UI128_t cc;
	UI128_t mm;

	a4[0] = (uint)((a >>  0) & 0xffffffffui64);
	a4[1] = (uint)((a >> 32) & 0xffffffffui64);
	a4[2] = 0;
	a4[3] = 0;

	b4[0] = (uint)((b >>  0) & 0xffffffffui64);
	b4[1] = (uint)((b >> 32) & 0xffffffffui64);
	b4[2] = 0;
	b4[3] = 0;

	m4[0] = (uint)((modulo >>  0) & 0xffffffffui64);
	m4[1] = (uint)((modulo >> 32) & 0xffffffffui64);
	m4[2] = 0;
	m4[3] = 0;

	aa = ToUI128(a4);
	bb = ToUI128(b4);
	mm = ToUI128(m4);

	cc = UI128_Mul(aa, bb, NULL);
	cc = UI128_Mod(cc, mm, NULL);

	FromUI128(cc, c4);

	return (uint64)c4[1] << 32 | c4[0];
}
static uint64 ModPow(uint64 value, uint64 exponent, uint64 modulo)
{
	uint64 ret = 1;

	for (; ; )
	{
		if (exponent % 2 == 1)
			ret = ModMul(ret, value, modulo);

		exponent /= 2;

		if (exponent == 0)
			break;

		value = ModMul(value, value, modulo);
	}
	return ret;
}
/*
	ミラー・ラビン素数判定法による素数判定
*/
int IsPrime_M(uint64 value)
{
	static uint64 a[] = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
	uint64 d;
	uint64 x;
	uint r;
	uint c;
	uint t;

	if (value <= 37)
	{
		for (t = 0; t < lengthof(a); t++)
			if (a[t] == value)
				return 1;

		return 0;
	}
	if (value % 2 == 0)
		return 0;

	d = value;
//	d = value - 1; // どうせ /= 2 するので

	for (r = 0; (d /= 2) % 2 == 0; r++);

	for (t = 0; t < lengthof(a); t++)
	{
		x = ModPow(a[t], d, value);

		if (x != 1 && x != value - 1)
		{
			for (c = r; ; c--)
			{
				if (!c)
					return 0;

				x = ModPow(x, 2, value);

				if (x == value - 1)
					break;
			}
		}
	}
	return 1;
}
