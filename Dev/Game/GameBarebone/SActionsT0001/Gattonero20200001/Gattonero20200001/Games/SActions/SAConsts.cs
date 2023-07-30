using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.SActions
{
	public static class SAConsts
	{
		// タイルのサイズ(ドット単位)
		//
		public const int TILE_W = 32;
		public const int TILE_H = 32;

		public const int PLAYER_DAMAGE_FRAME_MAX = 20;
		public const int PLAYER_INVINCIBLE_FRAME_MAX = 60;

		/// <summary>
		/// プレイヤーキャラクタのジャンプ回数の上限
		/// 1 == 通常
		/// 2 == 2-段ジャンプまで可能
		/// 3 == 3-段ジャンプまで可能
		/// ...
		/// </summary>
		public const int PLAYER_JUMP_MAX = 2;

		/// <summary>
		/// プレイヤーキャラクタの重力加速度
		/// </summary>
		public const double PLAYER_GRAVITY = 0.6;

		/// <summary>
		/// プレイヤーキャラクタの落下最高速度
		/// </summary>
		public const double PLAYER_FALL_SPEED_MAX = 10.0;

		/// <summary>
		/// プレイヤーキャラクタの(横移動)速度
		/// </summary>
		public const double PLAYER_SPEED = 6.0;

		/// <summary>
		/// プレイヤーキャラクタの低速移動時の(横移動)速度
		/// </summary>
		public const double PLAYER_SLOW_SPEED = 2.0;

		/// <summary>
		/// プレイヤーキャラクタのジャンプ初速度
		/// </summary>
		public const double PLAYER_JUMP_SPEED = -16.0;

		// 滞空中に壁に突進しても、脳天判定・接地判定に引っ掛からないように側面判定を先に行う。
		// -- ( 脳天判定Pt_X < 側面判定Pt_X && 接地判定Pt_X < 側面判定Pt_X ) を維持すること。
		// 上昇が速すぎると、脳天判定より先に側面判定に引っ掛かってしまう可能性がある。
		// -- ( -(PLAYER_JUMP_SPEED) < 脳天判定Pt_Y - 側面判定Pt_YT ) を維持すること。
		// 下降が速すぎると、接地判定より先に側面判定に引っ掛かってしまう可能性がある。
		// -- ( PLAYER_FALL_SPEED_MAX < 接地判定Pt_Y - 側面判定Pt_YB ) を維持すること。

		public const double PLAYER_側面判定Pt_X = 10.0;
		public const double PLAYER_側面判定Pt_YT = 17.0;
		public const double PLAYER_側面判定Pt_YB = 31.0;
		public const double PLAYER_脳天判定Pt_X = 9.0;
		public const double PLAYER_脳天判定Pt_Y = 36.0;
		public const double PLAYER_接地判定Pt_X = 9.0;
		public const double PLAYER_接地判定Pt_Y = 46.0;
	}
}
