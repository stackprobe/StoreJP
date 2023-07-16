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
			bool drawFrontFlag = false; // 敵より前面に描画するか
			bool hantoumeiFlag = false;

			if (1 <= this.ShagamiFrame) // しゃがみ
			{
				picture = Pictures.Tewi_しゃがみ[Math.Min(this.ShagamiFrame / 3, Pictures.Tewi_しゃがみ.Length - 1)];
			}
			else if (this.AirborneFrame != 0) // 滞空状態
			{
				if (1 <= this.上昇_Frame) // 上昇
				{
					int koma = this.上昇_Frame;
					koma--;
					koma /= 3;
					koma = Math.Min(koma, Pictures.Tewi_ジャンプ_上昇.Length - 1);

					picture = Pictures.Tewi_ジャンプ_上昇[koma];
				}
				else // 下降
				{
					int koma = this.下降_Frame;
					koma--;
					koma /= 3;

					if (Pictures.Tewi_ジャンプ_下降.Length <= koma)
					{
						koma -= Pictures.Tewi_ジャンプ_下降.Length;
						koma %= 3;
						koma = Pictures.Tewi_ジャンプ_下降.Length - 3 + koma;
					}
					picture = Pictures.Tewi_ジャンプ_下降[koma];
				}
			}
			else if (1 <= this.MoveFrame) // 移動
			{
				if (this.MoveSlow)
				{
					picture = Pictures.Tewi_歩く[SAGame.I.Frame / 10 % Pictures.Tewi_歩く.Length];
				}
				else
				{
					picture = Pictures.Tewi_走る[SAGame.I.Frame / 5 % Pictures.Tewi_走る.Length];
				}
			}
			else // 立ち
			{
				int koma = this.StandFrame / 3;

				if (koma < Pictures.Tewi_しゃがみ解除.Length)
				{
					picture = Pictures.Tewi_しゃがみ解除[koma];
				}
				else
				{
					picture = Pictures.Tewi_立ち[SAGame.I.Frame / 10 % Pictures.Tewi_立ち.Length];
				}
			}

			if (1 <= this.DamageFrame) // 被弾モーション
			{
				picture = Pictures.Tewi_大ダメージ[(this.DamageFrame * Pictures.Tewi_大ダメージ.Length) / (SAConsts.PLAYER_DAMAGE_FRAME_MAX + 1)];
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
