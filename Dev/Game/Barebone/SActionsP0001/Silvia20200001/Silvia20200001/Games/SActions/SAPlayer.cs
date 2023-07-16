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

		private int GlanceBehindFrame = 0;

		public int EquippedWeapon = 0;

		/// <summary>
		/// プレイヤーの移動と描画
		/// </summary>
		public void Draw()
		{
			if (DD.ExecuteTasks(this.Draw_TL))
				return;

			if (this.GlanceBehindFrame == 0 && SCommon.CRandom.GetRate() < 0.002) // キョロキョロするか
				this.GlanceBehindFrame = 150 + (int)(SCommon.CRandom.GetRate() * 90.0);

			DD.Countdown(ref this.GlanceBehindFrame);

			Picture picture = Pictures.PlayerStands[120 < this.GlanceBehindFrame ? 1 : 0, (DD.ProcFrame / 20) % 2];
			double xZoom = this.FacingLeft ? -1 : 1;
			bool drawFrontFlag = false; // 敵より前面に描画するか
			bool hantoumeiFlag = false;

			// 立ち >

			if (1 <= this.AirborneFrame)
			{
				picture = Pictures.PlayerJump[0];
			}
			else if (1 <= this.ShagamiFrame)
			{
				picture = Pictures.PlayerShagami;
			}
			else if (1 <= this.MoveFrame)
			{
				if (this.MoveSlow)
				{
					picture = Pictures.PlayerWalk[(DD.ProcFrame / 10) % 2];
				}
				else
				{
					picture = Pictures.PlayerDash[(DD.ProcFrame / 5) % 2];
				}
			}

			// < 立ち

			// 攻撃中 >

			if (1 <= this.AttackFrame && this.UwamukiFrame == 0 && this.ShitamukiFrame == 0)
			{
				picture = Pictures.PlayerAttack;

				if (1 <= this.AirborneFrame)
				{
					picture = Pictures.PlayerAttackJump;
				}
				else if (1 <= this.ShagamiFrame)
				{
					picture = Pictures.PlayerAttackShagami;
				}
				else if (1 <= this.MoveFrame)
				{
					if (this.MoveSlow)
					{
						picture = Pictures.PlayerAttackWalk[(DD.ProcFrame / 10) % 2];
					}
					else
					{
						picture = Pictures.PlayerAttackDash[(DD.ProcFrame / 5) % 2];
					}
				}
			}

			// < 攻撃中

			if (1 <= this.DamageFrame) // 被弾モーション
			{
				picture = Pictures.PlayerDamage[0];
				xZoom *= -1;
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
				DD.SetZoom(xZoom, 1.0);
				DD.Draw(
					picture,
					new D2Point(
						this.X - SAGame.I.Camera.X,
						this.Y - SAGame.I.Camera.Y - 18
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

		public void Shoot()
		{
			const double Y_ADD_SHAGAMI = 8.0;
			const double Y_ADD_STAND = -8.0;

			bool shagami = 1 <= SAGame.I.Player.ShagamiFrame;
			bool uwamuki = 1 <= SAGame.I.Player.UwamukiFrame;
			bool shitamuki = 1 <= SAGame.I.Player.ShitamukiFrame;

			switch (this.EquippedWeapon)
			{
				case 0: // Normal
					if (this.AttackFrame % 6 == 1)
					{
						double x = this.X;
						double y = this.Y;

						if (uwamuki)
						{
							y -= 20.0;
						}
						else if (shitamuki)
						{
							y += 20.0;
						}
						else
						{
							x += 0.0 * (this.FacingLeft ? -1 : 1);

							if (shagami)
								y += Y_ADD_SHAGAMI;
							else
								y += Y_ADD_STAND;
						}
						SAGame.I.Shots.Add(new SAShot_Normal(x, y, this.FacingLeft, uwamuki, shitamuki));
					}
					break;

				case 1: // FireBall
					if (this.AttackFrame % 12 == 1)
					{
						double x = this.X;
						double y = this.Y;

						if (uwamuki)
						{
							y -= 65.0;
						}
						else if (shitamuki)
						{
							y += 50.0;
						}
						else
						{
							x += 50.0 * (this.FacingLeft ? -1 : 1);

							if (shagami)
								y += Y_ADD_SHAGAMI;
							else
								y += Y_ADD_STAND;
						}
						SAGame.I.Shots.Add(new SAShot_FireBall(x, y, this.FacingLeft, uwamuki, shitamuki));
					}
					break;

				case 2: // Laser
					if (1 <= this.AttackFrame)
					{
						double x = this.X;
						double y = this.Y;

						if (uwamuki)
						{
							if (this.MoveFrame == 0)
								x -= 4.0 * (this.FacingLeft ? -1 : 1);

							y -= 50.0;
						}
						else if (shitamuki)
						{
							y += 50.0;
						}
						else
						{
							x += 38.0 * (this.FacingLeft ? -1 : 1);

							if (shagami)
								y += Y_ADD_SHAGAMI;
							else
								y += Y_ADD_STAND;
						}
						SAGame.I.Shots.Add(new SAShot_Laser(x, y, this.FacingLeft, uwamuki, shitamuki));
					}
					break;

				case 3: // WaveBeam
					if (this.AttackFrame % 12 == 1)
					{
						double x = this.X;
						double y = this.Y;

						if (uwamuki)
						{
							y -= 50.0;
						}
						else if (shitamuki)
						{
							y += 50.0;
						}
						else
						{
							x += 38.0 * (this.FacingLeft ? -1 : 1);

							if (shagami)
								y += Y_ADD_SHAGAMI;
							else
								y += Y_ADD_STAND;
						}
						SAGame.I.Shots.Add(new SAShot_WaveBeam(x, y, this.FacingLeft, uwamuki, shitamuki));
					}
					break;

				default:
					throw null; // never
			}
		}
	}
}
