using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;
using Charlotte.Games.SActions.Attacks;
using Charlotte.Games.SActions.Shots;

namespace Charlotte.Games.SActions
{
	/// <summary>
	/// プレイヤーに関する情報と機能
	/// 唯一のインスタンスをゲームメインに保持する。
	/// </summary>
	public class SAPlayer
	{
		public double X = 300.0;
		public double Y = 300.0;
		public double YSpeed;
		public bool FacingLeft;
		public Crash Crash;
		public int MoveFrame;
		public bool MoveSlow; // ? 低速移動
		public bool JumpLock; // ? ジャンプ・ロック -- ジャンプしたらボタンを離すまでロックする。
		public int JumpFrame;
		public int JumpCount;
		public int AirborneFrame; // 0 == 接地状態, 1～ == 滞空状態
		public int ShagamiFrame; // 0 == 無効, 1～ == しゃがみ中
		public int UwamukiFrame; // 0 == 無効, 1～ == 上向き中
		public int ShitamukiFrame; // 0 == 無効, 1～ == 下向き中
		public int AttackFrame; // 0 == 無効, 1～ == 攻撃中
		public int DamageFrame; // 0 == 無効, 1～ == ダメージ中
		public int InvincibleFrame; // 0 == 無効, 1～ == 無敵時間中

		/// <summary>
		/// 体力
		/// -1 == 死亡
		/// 0 == (不使用・予約)
		/// 1～ == 残り体力
		/// </summary>
		public int HP = 10;

		public bool FacingTop
		{
			get { return 1 <= this.UwamukiFrame; }
		}

		public int 上昇_Frame;
		public int 下降_Frame;
		public int StandFrame = SCommon.IMAX / 2; // 0 == 無効, 1～ == しゃがんでいない(立っている・跳んでいる)

		/// <summary>
		/// プレイヤーの攻撃モーション
		/// その他プレイヤーの特殊なモーション
		/// null == 無効
		/// </summary>
		public SAAttack Attack = null;

		/// <summary>
		/// プレイヤー描画の代替タスクリスト
		/// </summary>
		public List<Func<bool>> Draw_TL = new List<Func<bool>>();

		/// <summary>
		/// プレイヤーの移動と描画
		/// </summary>
		public void Draw()
		{
			if (DD.ExecuteTasks(this.Draw_TL))
				return;

			Picture picture;
			double angle = 0.0;
			bool drawFrontFlag = false; // 敵より前面に描画するか
			bool hantoumeiFlag = false;

			// ShitamukiFrame -- しゃがみ無し

			if (this.AirborneFrame != 0) // 滞空状態
			{
				picture = Pictures.CirnoJump;
			}
			else if (1 <= this.MoveFrame) // 移動
			{
				int koma = DD.ProcFrame / (this.MoveSlow ? 12 : 6);

				if (koma < 4)
				{
					// none
				}
				else
				{
					koma %= 4;

					if (koma == 0)
					{
						koma = 2;
					}
				}
				picture = Pictures.CirnoRun[koma];
			}
			else if (1 <= this.AttackFrame && this.UwamukiFrame == 0)
			{
				picture = Pictures.CirnoAttack;
			}
			else // 立ち
			{
				picture = Pictures.CirnoStand;
			}

			if (1 <= this.DamageFrame) // 被弾モーション
			{
				picture = Pictures.CirnoStand;
				angle = ((DD.ProcFrame / 3) % 4) * (Math.PI / 2.0);
			}
			if (1 <= this.DamageFrame || 1 <= this.InvincibleFrame)
			{
				drawFrontFlag = true;
			}
			if (1 <= this.InvincibleFrame)
			{
				hantoumeiFlag = true;
			}

			Action routine = () =>
			{
				DD.SetRotate(angle);
				DD.SetAlpha(hantoumeiFlag ? 0.5 : 1.0);
				DD.SetZoom(this.FacingLeft ? -1.0 : 1.0, 1.0);
				DD.Draw(
					picture,
					new D2Point(
						this.X - SAGame.I.Camera.X,
						this.Y - SAGame.I.Camera.Y
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
