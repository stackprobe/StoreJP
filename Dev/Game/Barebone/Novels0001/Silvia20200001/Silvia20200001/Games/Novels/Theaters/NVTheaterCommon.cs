using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Novels.Theaters
{
	public static class NVTheaterCommon
	{
		/// <summary>
		/// 全てのシナリオで共通のコマンドを処理する。
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <param name="arguments">パラメータ列</param>
		/// <returns>コマンドを処理したか</returns>
		public static bool Invoke(string command, string[] arguments)
		{
			if (command == "選択肢")
			{
				NVGame.I.Choices = arguments;
			}
			else
			{
				return false; // コマンドを処理しなかった。
			}
			return true; // コマンドを処理した。
		}
	}
}
