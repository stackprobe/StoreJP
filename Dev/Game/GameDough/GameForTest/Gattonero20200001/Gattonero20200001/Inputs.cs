using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.GameCommons;

namespace Charlotte
{
	public static class Inputs
	{
		public static Input DIR_2 = new Input(DX.KEY_INPUT_DOWN, 0, "DOWN");
		public static Input DIR_4 = new Input(DX.KEY_INPUT_LEFT, 1, "LEFT");
		public static Input DIR_6 = new Input(DX.KEY_INPUT_RIGHT, 2, "RIGHT");
		public static Input DIR_8 = new Input(DX.KEY_INPUT_UP, 3, "UP");
		public static Input A = new Input(DX.KEY_INPUT_A, 4, "A");
		public static Input B = new Input(DX.KEY_INPUT_B, 7, "B");
		public static Input C = new Input(DX.KEY_INPUT_C, 5, "C");
		public static Input D = new Input(DX.KEY_INPUT_D, 8, "D");
		public static Input E = new Input(DX.KEY_INPUT_E, 6, "E");
		public static Input F = new Input(DX.KEY_INPUT_F, 9, "F");
		public static Input L = new Input(DX.KEY_INPUT_L, 10, "L");
		public static Input R = new Input(DX.KEY_INPUT_R, 11, "R");
		public static Input START = new Input(DX.KEY_INPUT_SPACE, 13, "START");
		public static Input DEBUG = new Input(DX.KEY_INPUT_RETURN, 12, "DEBUG");

		/// <summary>
		/// 全ての入力を列挙する。
		/// </summary>
		/// <returns>全ての入力</returns>
		public static Input[] GetAllInput()
		{
			return new Input[]
			{
				DIR_8,
				DIR_2,
				DIR_4,
				DIR_6,
				A,
				B,
				C,
				D,
				E,
				F,
				L,
				R,
				START,
			};
		}
	}
}
