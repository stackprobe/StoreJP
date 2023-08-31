using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Attacks;
using Charlotte.Games.TActions.Shots;

namespace Charlotte.Games.TActions
{
	/// <summary>
	/// プレイヤーに関する情報と機能
	/// </summary>
	public class TAPlayer
	{
		public double X = 300.0;
		public double Y = 300.0;
		public int FaceDirection = 2; // プレイヤーが向いている方向(8方向_テンキー方式)
		public Crash Crash;
		public int MoveFrame;
		public int AttackFrame;
		public int DamageFrame; // 0 == 無効, 1～ == ダメージ中
		public int InvincibleFrame; // 0 == 無効, 1～ == 無敵時間中

		/// <summary>
		/// プレイヤーの攻撃モーション
		/// その他プレイヤーの特殊なモーション
		/// null == 無効
		/// </summary>
		public TAAttack Attack = null;

		/// <summary>
		/// プレイヤー描画の代替タスクリスト
		/// </summary>
		public List<Func<bool>> Draw_TL = new List<Func<bool>>();

		public void Draw()
		{
			if (DD.ExecuteTasks(this.Draw_TL))
				return;

			Picture picture;
			bool drawFrontFlag = false; // 敵より前面に描画するか
			bool hantoumeiFlag = false;

			{
				int koma = 1;

				if (1 <= this.MoveFrame)
				{
					koma = (TAGame.I.Frame / 5) % 4;

					if (koma == 3)
						koma = 1;
				}
				picture = TACommon.GetWalkPicture(Pictures.CirnoWalk, this.FaceDirection, koma);
			}

			if (1 <= this.DamageFrame || 1 <= this.InvincibleFrame)
			{
				drawFrontFlag = true;
				hantoumeiFlag = true;
			}

			Action routine = () =>
			{
				DD.SetAlpha(hantoumeiFlag ? 0.5 : 1.0);
				DD.SetMosaic();
				DD.SetZoom(2.0);
				DD.Draw(
					picture,
					new D2Point(
						this.X - TAGame.I.Camera.X,
						this.Y - TAGame.I.Camera.Y - 12.0
						)
					);
			};

			if (drawFrontFlag)
			{
				DD.TL.Add(DD.Once(routine));
			}
			else
			{
				routine();
			}
		}
	}
}
