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
		public static Input DIR_2 = new Input(DX.KEY_INPUT_DOWN, 0, "方向・下");
		public static Input DIR_4 = new Input(DX.KEY_INPUT_LEFT, 1, "方向・左");
		public static Input DIR_6 = new Input(DX.KEY_INPUT_RIGHT, 2, "方向・右");
		public static Input DIR_8 = new Input(DX.KEY_INPUT_UP, 3, "方向・上");
		public static Input A = new Input(DX.KEY_INPUT_Z, 4, "決定／ジャンプ");
		public static Input B = new Input(DX.KEY_INPUT_X, 7, "キャンセル／弱攻撃");
		public static Input C = new Input(DX.KEY_INPUT_C, 5, "強攻撃");
		public static Input D = new Input(DX.KEY_INPUT_V, 8, "中攻撃");
		public static Input E = new Input(DX.KEY_INPUT_A, 6, "拡張ボタン３");
		public static Input F = new Input(DX.KEY_INPUT_S, 9, "拡張ボタン４");
		public static Input L = new Input(DX.KEY_INPUT_D, 10, "視点移動");
		public static Input R = new Input(DX.KEY_INPUT_F, 11, "低速移動");
		public static Input START = new Input(DX.KEY_INPUT_SPACE, 13, "スタート／一時停止");
		public static Input DEBUG = new Input(DX.KEY_INPUT_RETURN, 12, "DEBUG");

		/// <summary>
		/// 全ての入力を列挙する。
		/// 注意：デバッグ入力(DEBUG)は含めない。
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
