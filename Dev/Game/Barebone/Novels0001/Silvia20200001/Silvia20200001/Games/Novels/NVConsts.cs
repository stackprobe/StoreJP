using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;

namespace Charlotte.Games.Novels
{
	public static class NVConsts
	{
		/// <summary>
		/// 現在のページのテキストを表示し終えてから次ページへ遷移させないための入力抑止時間(フレーム数)
		/// </summary>
		public const int NEXT_PAGE_INPUT_INTERVAL = 10;

		/// <summary>
		/// 高速モード(スキップモード)が有効になるまでの決定ボタン押下時間(フレーム数)
		/// </summary>
		public const int FAST_MODE_INPUT_INTERVAL = 40;

		/// <summary>
		/// メッセージ表示速度(1文字表示するためのフレーム数)
		/// </summary>
		public const int MESSAGE_SPEED = 3;

		/// <summary>
		/// 高速モード(スキップモード)の速さ(ページを進めるボタンを連打する間隔(フレーム数))
		/// </summary>
		public static int FAST_MODE_SPEED = 2;

		public const int MESSAGE_L = 10;
		public const int MESSAGE_T = 450;
		public const int MESSAGE_Y_STEP = 30;
		public const int MESSAGE_FONT_SIZE = 16;
		public static readonly string MESSAGE_FONT_NAME = "Kゴシック";
		public static readonly I3Color MESSAGE_COLOR = new I3Color(255, 255, 255);

		public const int BACKLOG_L = 10;
		public const int BACKLOG_T = 10;
		public const int BACKLOG_Y_STEP = 28;
		public const int BACKLOG_Y_MAX = 19;
		public const int BACKLOG_FONT_SIZE = 16;
		public static readonly string BACKLOG_FONT_NAME = "Kゴシック";
		public static readonly I3Color BACKLOG_COLOR = new I3Color(255, 255, 0);
	}
}
