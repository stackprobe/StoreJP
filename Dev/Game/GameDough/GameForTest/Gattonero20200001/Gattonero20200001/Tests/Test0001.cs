using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void Test01()
		{
			DD.FreezeInput();

			for (; ; )
			{
				if (Inputs.START.GetInput() == 1)
					break;

				DD.EachFrame();
			}
			DD.FreezeInput();
		}
	}
}
