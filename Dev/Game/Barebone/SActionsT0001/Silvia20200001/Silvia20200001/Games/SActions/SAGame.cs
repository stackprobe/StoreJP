using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.SActions.Attacks;
using Charlotte.Games.SActions.Enemies;
using Charlotte.Games.SActions.Fields;
using Charlotte.Games.SActions.Shots;

namespace Charlotte.Games.SActions
{
	/// <summary>
	/// ゲームメイン
	/// </summary>
	public class SAGame : Anchorable<SAGame>
	{
		/// <summary>
		/// フィールドを出た方向または終了理由
		/// 値：
		/// -- 4 == 左へ出た。
		/// -- 6 == 右へ出た。
		/// -- 8 == 上へ出た。
		/// -- 2 == 下へ出た。
		/// -- 5 == 呼び出し側に戻る。
		/// -- 901 == 死亡した。
		/// </summary>
		public int ExitDirection = 5;

		public SAPlayer Player = new SAPlayer();
		public SAField Field;

		public bool CameraSlideMode; // ? カメラ・スライド_モード中
		public int CameraSlideCount;
		public int CameraSlideX; // -1 ～ 1
		public int CameraSlideY; // -1 ～ 1
		public D2Point CameraForCalc; // カメラ位置計算用
		public I2Point Camera;

		public int Frame;
		public bool ReturnToCallerRequested = false;

		public List<SAEnemy> Enemies = new List<SAEnemy>();
		public List<SAShot> Shots = new List<SAShot>();

		public void Run(SAField field)
		{
			Func<bool> f_ゴミ回収 = SCommon.Supplier(this.E_ゴミ回収());

			this.Field = field;
			this.Field.Initialize();

			DD.SetCurtain(-1.0, 0);
			DD.SetCurtain(0.0, 10);

			DD.FreezeInputUntilReleaseWithoutDirection();

			if (SAGameMaster.I.FieldToFieldFlag)
			{
				//Inputs.B.UnfreezeInputUntilRelease(); // 攻撃の押しっぱなし
			}

			for (this.Frame = 0; ; this.Frame++)
			{
				if (Inputs.START.GetInput() == 1)
				{
					this.Pause();

					if (this.ReturnToCallerRequested)
						break;
				}
				if (ProcMain.DEBUG && Inputs.DEBUG.GetInput() == 1)
				{
					this.DebugPause();
				}

				this.カメラ位置調整(this.Frame == 0);

				if (ProcMain.DEBUG && Keyboard.GetInput(DX.KEY_INPUT_T) == 1) // Attack テスト
				{
					this.Player.Attack = new SAAttack_Test0001();
				}

				if (this.Player.Attack != null) // プレイヤー攻撃中
				{
					if (this.Player.Attack.EachFrame()) // ? このプレイヤー攻撃を継続する。
						goto endOfPlayer;

					this.Player.Attack = null; // プレイヤー攻撃_終了
				}

				bool cameraSlide = false;

				// プレイヤー入力
				{
					bool damageInputLock = 1 <= this.Player.DamageFrame;
					bool move = false;
					bool slow = false;
					bool attack = false;
					bool shagami = false;
					bool uwamuki = false;
					bool shitamuki = false;
					int jump = 0;
					int attack_弱 = 0;
					int attack_中 = 0;
					int attack_強 = 0;

					if (!damageInputLock && 1 <= Inputs.DIR_8.GetInput())
					{
						uwamuki = true;
					}
					if (!damageInputLock && 1 <= Inputs.DIR_2.GetInput())
					{
						shagami = true;
						shitamuki = true;
					}
					if (!damageInputLock && 1 <= Inputs.DIR_4.GetInput())
					{
						this.Player.FacingLeft = true;
						move = true;
					}
					if (!damageInputLock && 1 <= Inputs.DIR_6.GetInput())
					{
						this.Player.FacingLeft = false;
						move = true;
					}
					if (1 <= Inputs.L.GetInput())
					{
						move = false;
						cameraSlide = true;
					}
					if (!damageInputLock && 1 <= Inputs.R.GetInput())
					{
						slow = true;
					}
					if (!damageInputLock && 1 <= Inputs.A.GetInput())
					{
						jump = Inputs.A.GetInput();
					}
					if (!damageInputLock && 1 <= Inputs.B.GetInput())
					{
						attack = true;
						attack_弱 = Inputs.B.GetInput();
					}
					if (!damageInputLock && 1 <= Inputs.C.GetInput())
					{
						attack = true;
						attack_中 = Inputs.C.GetInput();
					}
					if (!damageInputLock && 1 <= Inputs.D.GetInput())
					{
						attack = true;
						attack_強 = Inputs.D.GetInput();
					}

					if (move)
					{
						this.Player.MoveFrame++;
						shagami = false;
						//uwamuki = false;
						//shitamuki = false;
					}
					else
					{
						this.Player.MoveFrame = 0;
					}

					this.Player.MoveSlow = move && slow;

					if (jump == 0)
						this.Player.JumpLock = false;

					if (1 <= this.Player.JumpFrame) // ? ジャンプ中
					{
						if (1 <= jump)
						{
							this.Player.JumpFrame++;
						}
						else
						{
							// ★ ジャンプを中断・終了した。

							this.Player.JumpFrame = 0;

							if (this.Player.YSpeed < 0.0)
								this.Player.YSpeed /= 2.0;
						}
					}
					else // ? 接地中 || 滞空中
					{
						// 事前入力 == 着地前の数フレーム間にジャンプボタンを押し始めてもジャンプできるようにする。
						// 入力猶予 == 落下(地面から離れた)直後の数フレーム間にジャンプボタンを押し始めてもジャンプできるようにする。

						const int 事前入力時間 = 5;
						const int 入力猶予時間 = 10;

						if (this.Player.AirborneFrame < 入力猶予時間 && this.Player.JumpCount == 0) // ? 接地状態からのジャンプが可能な状態
						{
							if (1 <= jump && jump < 事前入力時間 && !this.Player.JumpLock)
							{
								// ★ ジャンプを開始した。

								this.Player.JumpFrame = 1;
								this.Player.JumpCount = 1;

								this.Player.YSpeed = SAConsts.PLAYER_JUMP_SPEED;

								this.Player.JumpLock = true;
							}
							else
							{
								this.Player.JumpCount = 0;
							}
						}
						else // ? 接地状態からのジャンプが「可能ではない」状態
						{
							// 滞空状態に入ったら「通常ジャンプの状態」にする。
							if (this.Player.JumpCount < 1)
								this.Player.JumpCount = 1;

							if (1 <= jump && jump < 事前入力時間 && this.Player.JumpCount < SAConsts.PLAYER_JUMP_MAX && !this.Player.JumpLock)
							{
								// ★ 空中(n-段)ジャンプを開始した。

								this.Player.JumpFrame = 1;
								this.Player.JumpCount++;

								this.Player.YSpeed = SAConsts.PLAYER_JUMP_SPEED;

								DD.TL.Add(SCommon.Supplier(SAEffects.空中ジャンプの足場(this.Player.X, this.Player.Y + SAConsts.PLAYER_接地判定Pt_Y)));

								this.Player.JumpLock = true;
							}
							else
							{
								// noop
							}
						}
					}

					if (this.Player.JumpFrame == 1) // ? ジャンプ開始
					{
						SoundEffects.PlayerJump.Play();
					}

					if (1 <= this.Player.AirborneFrame)
					{
						shagami = false;
						//uwamuki = false;
						//shitamuki = false;
					}
					else
					{
						shitamuki = false;
					}

					if (shagami)
						this.Player.ShagamiFrame++;
					else
						this.Player.ShagamiFrame = 0;

					if (uwamuki)
						this.Player.UwamukiFrame++;
					else
						this.Player.UwamukiFrame = 0;

					if (shitamuki)
						this.Player.ShitamukiFrame++;
					else
						this.Player.ShitamukiFrame = 0;

					if (attack)
						this.Player.AttackFrame++;
					else
						this.Player.AttackFrame = 0;

					if (attack_弱 == 1)
					{
						if (1 <= this.Player.ShagamiFrame)
							this.Player.Attack = new SAAttack_ShagamiJaku();
						else if (1 <= this.Player.AirborneFrame)
							this.Player.Attack = new SAAttack_JumpJaku();
						else
							this.Player.Attack = new SAAttack_StandJaku();
					}
					if (attack_中 == 1)
					{
						if (1 <= this.Player.ShagamiFrame)
							this.Player.Attack = new SAAttack_ShagamiChuu();
						else if (1 <= this.Player.AirborneFrame)
							this.Player.Attack = new SAAttack_JumpChuu();
						else
							this.Player.Attack = new SAAttack_StandChuu();
					}
					if (attack_強 == 1)
					{
						if (1 <= this.Player.ShagamiFrame)
							this.Player.Attack = new SAAttack_ShagamiKyou();
						else if (1 <= this.Player.AirborneFrame)
							this.Player.Attack = new SAAttack_JumpKyou();
						else
							this.Player.Attack = new SAAttack_StandKyou();
					}
				}

				/*
				// プレイヤー攻撃
				//
				if (1 <= this.Frame) // 初回フレームは壁にめり込んでいる可能性があるため...
				{
					bool shitamuki = 1 <= this.Player.AirborneFrame && 1 <= this.Player.ShitamukiFrame;
					bool uwamuki = 1 <= this.Player.UwamukiFrame;

					if (this.Player.AttackFrame % 10 == 1)
					{
						this.Shots.Add(new SAShot_Test0001(this.Player.X, this.Player.Y, this.Player.FacingLeft, uwamuki, shitamuki));
					}
				}
				//*/

				// カメラ位置スライド
				{
					if (cameraSlide)
					{
						if (Inputs.DIR_4.IsPound())
						{
							this.CameraSlideCount++;
							this.CameraSlideX--;
						}
						if (Inputs.DIR_6.IsPound())
						{
							this.CameraSlideCount++;
							this.CameraSlideX++;
						}
						if (Inputs.DIR_8.IsPound())
						{
							this.CameraSlideCount++;
							this.CameraSlideY--;
						}
						if (Inputs.DIR_2.IsPound())
						{
							this.CameraSlideCount++;
							this.CameraSlideY++;
						}
						this.CameraSlideX = SCommon.ToRange(this.CameraSlideX, -1, 1);
						this.CameraSlideY = SCommon.ToRange(this.CameraSlideY, -1, 1);
					}
					else
					{
						if (this.CameraSlideMode && this.CameraSlideCount == 0)
						{
							this.CameraSlideX = 0;
							this.CameraSlideY = 0;
						}
						this.CameraSlideCount = 0;
					}
					this.CameraSlideMode = cameraSlide;
				}

				if (1 <= this.Player.DamageFrame) // ? プレイヤー・ダメージ中
				{
					if (SAConsts.PLAYER_DAMAGE_FRAME_MAX < ++this.Player.DamageFrame)
					{
						this.Player.DamageFrame = 0;
						this.Player.InvincibleFrame = 1;
						goto endOfDamage;
					}
					int frame = this.Player.DamageFrame; // 値域 == 2 ～ SAConsts.PLAYER_DAMAGE_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)SAConsts.PLAYER_DAMAGE_FRAME_MAX, (double)frame);

					// プレイヤー・ダメージ中の処理
					{
						if (frame == 2) // 初回のみ
						{
							DD.TL.Add(SCommon.Supplier(SAEffects.Explode(this.Player.X, this.Player.Y, 2.0)));
							SoundEffects.PlayerDamaged.Play();
						}
						this.Player.X -= (9.0 - 6.0 * rate) * (this.Player.FacingLeft ? -1 : 1);
					}
				}
			endOfDamage:

				if (1 <= this.Player.InvincibleFrame) // ? プレイヤー無敵時間中
				{
					if (SAConsts.PLAYER_INVINCIBLE_FRAME_MAX < ++this.Player.InvincibleFrame)
					{
						this.Player.InvincibleFrame = 0;
						goto endOfInvincible;
					}
					int frame = this.Player.InvincibleFrame; // 値域 == 2 ～ GameConsts.PLAYER_INVINCIBLE_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)SAConsts.PLAYER_INVINCIBLE_FRAME_MAX, (double)frame);

					// プレイヤー無敵時間中の処理
					{
						// none
					}
				}
			endOfInvincible:

				// プレイヤー移動
				{
					if (1 <= this.Player.MoveFrame)
					{
						double speed;

						if (this.Player.MoveSlow)
						{
							speed = this.Player.MoveFrame / 10.0;
							speed = Math.Min(speed, SAConsts.PLAYER_SLOW_SPEED);
						}
						else
						{
							speed = SAConsts.PLAYER_SPEED;
						}
						speed *= this.Player.FacingLeft ? -1 : 1;
						this.Player.X += speed;
					}
					else
					{
						this.Player.X = (double)SCommon.ToInt(this.Player.X);
					}

					// 重力による加速
					this.Player.YSpeed += SAConsts.PLAYER_GRAVITY;

					// 自由落下の最高速度を超えないようにする。
					this.Player.YSpeed = Math.Min(this.Player.YSpeed, SAConsts.PLAYER_FALL_SPEED_MAX);

					// 自由落下
					this.Player.Y += this.Player.YSpeed;
				}

				// プレイヤー位置訂正
				{
					bool touchSide_L =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_側面判定Pt_X, this.Player.Y - SAConsts.PLAYER_側面判定Pt_YT))) ||
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_側面判定Pt_X, this.Player.Y))) ||
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_側面判定Pt_X, this.Player.Y + SAConsts.PLAYER_側面判定Pt_YB)));

					bool touchSide_R =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_側面判定Pt_X, this.Player.Y - SAConsts.PLAYER_側面判定Pt_YT))) ||
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_側面判定Pt_X, this.Player.Y))) ||
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_側面判定Pt_X, this.Player.Y + SAConsts.PLAYER_側面判定Pt_YB)));

					if (touchSide_L && touchSide_R) // -> 壁抜け防止のため再チェック
					{
						touchSide_L = this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_側面判定Pt_X, this.Player.Y)));
						touchSide_R = this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_側面判定Pt_X, this.Player.Y)));
					}

					if (touchSide_L && touchSide_R)
					{
						// noop
					}
					else if (touchSide_L)
					{
						this.Player.X = SACommon.ToTileCenterX(this.Player.X - SAConsts.PLAYER_側面判定Pt_X) + SAConsts.TILE_W / 2 + SAConsts.PLAYER_側面判定Pt_X;
					}
					else if (touchSide_R)
					{
						this.Player.X = SACommon.ToTileCenterX(this.Player.X + SAConsts.PLAYER_側面判定Pt_X) - SAConsts.TILE_W / 2 - SAConsts.PLAYER_側面判定Pt_X;
					}

					bool touchCeiling_L =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_脳天判定Pt_X, this.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y)));

					bool touchCeiling_M =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X, this.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y)));

					bool touchCeiling_R =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_脳天判定Pt_X, this.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y)));

					if ((touchCeiling_L && touchCeiling_R) || touchCeiling_M)
					{
						if (this.Player.YSpeed < 0.0)
						{
							// プレイヤーと天井の反発係数
							//const double K = 1.0;
							const double K = 0.0;

							this.Player.Y = SACommon.ToTileCenterY(this.Player.Y - SAConsts.PLAYER_脳天判定Pt_Y) + SAConsts.TILE_H / 2 + SAConsts.PLAYER_脳天判定Pt_Y;
							this.Player.YSpeed = Math.Abs(Player.YSpeed) * K;
							this.Player.JumpFrame = 0;
						}
					}
					else if (touchCeiling_L)
					{
						this.Player.X = SACommon.ToTileCenterX(this.Player.X - SAConsts.PLAYER_脳天判定Pt_X) + SAConsts.TILE_W / 2 + SAConsts.PLAYER_脳天判定Pt_X;
					}
					else if (touchCeiling_R)
					{
						this.Player.X = SACommon.ToTileCenterX(this.Player.X + SAConsts.PLAYER_脳天判定Pt_X) - SAConsts.TILE_W / 2 - SAConsts.PLAYER_脳天判定Pt_X;
					}

					bool touchGround =
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X - SAConsts.PLAYER_接地判定Pt_X, this.Player.Y + SAConsts.PLAYER_接地判定Pt_Y))) ||
						this.Field.IsWall(SACommon.ToTablePoint(new D2Point(this.Player.X + SAConsts.PLAYER_接地判定Pt_X, this.Player.Y + SAConsts.PLAYER_接地判定Pt_Y)));

					// memo: @ 2022.7.11
					// 上昇中(ジャンプ中)に接地判定が発生することがある。
					// 接地中は重力により PlayerYSpeed がプラスに振れる。
					// -> 接地による位置等の調整は PlayerYSpeed がプラスに振れている場合のみ行う。

					if (touchGround && 0.0 < this.Player.YSpeed)
					{
						this.Player.Y = SACommon.ToTileCenterY(this.Player.Y + SAConsts.PLAYER_接地判定Pt_Y) - SAConsts.TILE_H / 2 - SAConsts.PLAYER_接地判定Pt_Y;
						this.Player.YSpeed = 0.0;
						this.Player.JumpCount = 0;
						this.Player.AirborneFrame = 0;
					}
					else
					{
						this.Player.AirborneFrame++;
					}
				}

				// プレイヤー当たり判定をセットする。
				// -- プレイヤーのダメージ中・無敵時間中など、当たり判定無しの場合は Crash.CreateNone() をセットすること。
				{
					this.Player.Crash = Crash.CreateNone(); // reset

					if (1 <= this.Player.DamageFrame) // ? プレイヤー・ダメージ中
					{
						// noop
					}
					else if (1 <= this.Player.InvincibleFrame) // ? プレイヤー無敵時間中
					{
						// noop
					}
					else if (1 <= this.Player.AirborneFrame)
					{
						this.Player.Crash = Crash.CreatePoint(new D2Point(this.Player.X, this.Player.Y));
					}
					else if (1 <= this.Player.ShagamiFrame)
					{
						this.Player.Crash = Crash.CreatePoint(new D2Point(this.Player.X, this.Player.Y + 25.0));
					}
					else if (1 <= this.Player.MoveFrame)
					{
						this.Player.Crash = Crash.CreatePoint(new D2Point(this.Player.X, this.Player.Y + 10.0));
					}
					else
					{
						this.Player.Crash = Crash.CreateRect(D4Rect.XYWH(
							this.Player.X - 4.0 * (this.Player.FacingLeft ? -1 : 1),
							this.Player.Y + 10.0,
							8.0,
							60.0
							));
					}
				}
			endOfPlayer: // 特殊モーションからの合流点

				if (this.Player.YSpeed < -SCommon.MICRO)
				{
					this.Player.上昇_Frame++;
					this.Player.下降_Frame = 0;
				}
				else if (SCommon.MICRO < this.Player.YSpeed)
				{
					this.Player.上昇_Frame = 0;
					this.Player.下降_Frame++;
				}
				else
				{
					this.Player.上昇_Frame = 0;
					this.Player.下降_Frame = 0;
				}

				if (this.Player.ShagamiFrame == 0)
					this.Player.StandFrame++;
				else
					this.Player.StandFrame = 0;

				if (this.Player.Y < 0.0) // ? フィールドの上側に出た。
				{
					this.ExitDirection = 8;
					break;
				}
				if (this.Field.H < this.Player.Y) // ? フィールドの下側に出た。
				{
					this.ExitDirection = 2;
					break;
				}
				if (this.Player.X < 0.0) // ? フィールドの左側に出た。
				{
					this.ExitDirection = 4;
					break;
				}
				if (this.Field.W < this.Player.X) // ? フィールドの右側に出た。
				{
					this.ExitDirection = 6;
					break;
				}

				// プレイヤーの登場位置の関係などによりプレイヤーの位置が補正された場合を想定して、
				// 念のためカメラ位置の補正を行う。
				//
				if (this.Frame == 0)
				{
					this.カメラ位置調整(true);
				}

				// プレイヤー攻撃 -> プレイヤー入力と同時に行っている。

				// ====
				// 描画ここから
				// ====

				this.DrawWall();
				this.DrawMap();
				this.Player.Draw();

				foreach (SAEnemy enemy in this.Enemies)
				{
					if (enemy.DeadFlag) // ? 敵：既に死亡
						continue;

					enemy.Crash = Crash.CreateNone(); // reset
					enemy.Draw();
				}
				foreach (SAShot shot in this.Shots)
				{
					if (shot.DeadFlag) // ? 自弾：既に死亡
						continue;

					shot.Crash = Crash.CreateNone(); // reset
					shot.Draw();
				}
				this.DrawFront();

				if (this.当たり判定表示)
				{
					// 最後に描画されるようにエフェクトとして追加する。

					DD.TL.Add(DD.Once(() =>
					{
						DD.DrawCurtain(-0.7);

						double dPlX = this.Player.X - this.Camera.X;
						double dPlY = this.Player.Y - this.Camera.Y;

						D3Color BRIGHT = new D3Color(0.0, 1.0, 0.0);
						double ALPHA = 0.3;

						DD.SetBright(BRIGHT);
						DD.SetAlpha(ALPHA);
						DD.Draw(
							Pictures.WhiteBox,
							D4Rect.LTRB(
								dPlX - SAConsts.PLAYER_側面判定Pt_X,
								dPlY - SAConsts.PLAYER_側面判定Pt_YT,
								dPlX + SAConsts.PLAYER_側面判定Pt_X,
								dPlY + SAConsts.PLAYER_側面判定Pt_YB
								)
							);
						DD.SetBright(BRIGHT);
						DD.SetAlpha(ALPHA);
						DD.Draw(
							Pictures.WhiteBox,
							D4Rect.LTRB(
								dPlX - SAConsts.PLAYER_脳天判定Pt_X,
								dPlY - SAConsts.PLAYER_脳天判定Pt_Y,
								dPlX + SAConsts.PLAYER_脳天判定Pt_X,
								dPlY
								)
							);
						DD.SetBright(BRIGHT);
						DD.SetAlpha(ALPHA);
						DD.Draw(
							Pictures.WhiteBox,
							D4Rect.LTRB(
								dPlX - SAConsts.PLAYER_接地判定Pt_X,
								dPlY,
								dPlX + SAConsts.PLAYER_接地判定Pt_X,
								dPlY + SAConsts.PLAYER_接地判定Pt_Y
								)
							);

						ALPHA = 0.7;

						this.Player.Crash.Draw(new I3Color(255, 0, 0).ToD3Color().WithAlpha(ALPHA), this.Camera.ToD2Point());

						foreach (SAEnemy enemy in this.Enemies)
							enemy.Crash.Draw(new I3Color(255, 255, 255).ToD3Color().WithAlpha(ALPHA), this.Camera.ToD2Point());

						foreach (SAShot shot in this.Shots)
							shot.Crash.Draw(new I3Color(0, 255, 255).ToD3Color().WithAlpha(ALPHA), this.Camera.ToD2Point());
					}));
				}

				// ====
				// 描画ここまで
				// ====

				// ====
				// 当たり判定ここから
				// ====

				foreach (SAEnemy enemy in this.Enemies)
				{
					if (1 <= enemy.HP) // ? 敵：生存 && 無敵ではない
					{
						foreach (SAShot shot in this.Shots)
						{
							// 衝突判定：敵 x 自弾
							if (
								!shot.DeadFlag && // ? 自弾：生存
								Crash.IsCrashed(enemy.Crash, shot.Crash) // ? 衝突
								)
							{
								// ★ 敵_被弾ここから

								int damagePoint = Math.Min(enemy.HP, shot.AttackPoint);

								enemy.HP -= shot.AttackPoint;

								if (shot.敵を貫通する)
								{
									// noop
								}
								else // ? 敵を貫通しない -> 自弾の攻撃力と敵のHPを相殺
								{
									if (0 <= enemy.HP) // ? 丁度削りきった || 削りきれなかった -> 攻撃力を使い果たしたので、ショットは消滅
									{
										shot.AttackPoint = 0; // 攻撃力を使い果たした。
										shot.Kill();
									}
									else
									{
										shot.AttackPoint = -enemy.HP; // 過剰に削った分を残りの攻撃力として還元
									}
								}

								if (1 <= enemy.HP) // ? まだ生存している。
								{
									enemy.Damaged(shot, damagePoint);
								}
								else // ? 撃破した。
								{
									enemy.HP = 0; // 過剰に削った分を正す。
									enemy.Kill(true);
									goto nextEnemy; // この敵は死亡したので、この敵について以降の当たり判定は不要
								}

								// ★ 敵_被弾ここまで
							}
						}
					}

					// 衝突判定：敵 x 自機
					if (
						!enemy.DeadFlag && // ? 敵：生存
						Crash.IsCrashed(enemy.Crash, this.Player.Crash) // ? 衝突
						)
					{
						// ★ 自機_被弾ここから

						this.Player.HP -= enemy.AttackPoint;

						if (1 <= this.Player.HP) // ? まだ生存している。
						{
							this.Player.DamageFrame = 1;
						}
						else // ? 死亡した。
						{
							this.Player.HP = -1;
							this.ExitDirection = 901;
							goto endOfGameLoop;
						}

						if (enemy.自機に当たると消滅する)
							enemy.Kill();

						// ★ 自機_被弾ここまで
					}

				nextEnemy:
					;
				}

				// ====
				// 当たり判定ここまで
				// ====

				this.Enemies.RemoveAll(v => v.DeadFlag);
				this.Shots.RemoveAll(v => v.DeadFlag);

				if (!f_ゴミ回収())
					throw null; // never

				DD.EachFrame();

				// ★★★ ゲームループの終わり ★★★
			}
		endOfGameLoop:
			DD.FreezeInput();

			if (this.ExitDirection == 901) // ? 死亡によりゲーム終了
			{
				Action drawPlayer_01 = () =>
				{
					DD.SetZoom(this.Player.FacingLeft ? -1.0 : 1.0, 1.0);
					DD.Draw(
						Pictures.Tewi_大ダメージ[Pictures.Tewi_大ダメージ.Length - 1],
						new D2Point(
							SCommon.ToInt(this.Player.X - this.Camera.X),
							SCommon.ToInt(this.Player.Y - this.Camera.Y)
							)
						);
				};

				// 1フレーム前の画面だと、プレイヤーと敵が当たっていないため不自然に見えるだろう。
				// 今回のフレームの描画を確定し、今回のフレームの画面を死亡確定画面として表示する必要がある。
				// 死亡時は当たり判定からゲームループを抜けるので今回のフレームの描画は完了しているはず。
				//
				DD.EachFrame(); // 今回の描画を確定させてから...
				using (DU.FreeScreen.Section()) // ...画面を保存する。
				{
					DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
				}

				Music.FadeOut();

				foreach (Scene scene in Scene.Create(40)) // 死亡確定画面
				{
					DD.Draw(DU.FreeScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
					DD.EachFrame();
				}
				foreach (Scene scene in Scene.Create(60))
				{
					this.DrawWall();
					this.DrawMap();

					DD.DrawCurtain(scene.Rate * -1.0);

					drawPlayer_01();

					DD.EachFrame();
				}

				DD.TL.Add(SCommon.Supplier(SAEffects.Explode(this.Player.X, this.Player.Y, 5.0)));

				//Music.FadeOut();
				DD.SetCurtain(-1.0);

				foreach (Scene scene in Scene.Create(40))
				{
					DD.SetBright(new I3Color(0, 0, 0).ToD3Color());
					DD.Draw(Pictures.WhiteBox, new I4Rect(0, 0, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect());

					DD.EachFrame();
				}
			}
			else if (this.ExitDirection == 5) // ? メニュー操作によりゲーム終了
			{
				Music.FadeOut();
				DD.SetCurtain(-1.0);

				foreach (Scene scene in Scene.Create(40))
				{
					this.DrawWall();
					this.DrawMap();

					DD.EachFrame();
				}
			}
			else // ? 部屋移動
			{
				double destSlideX = 0.0;
				double destSlideY = 0.0;

				switch (this.ExitDirection)
				{
					case 4:
						destSlideX = GameConfig.ScreenSize.W;
						break;

					case 6:
						destSlideX = -GameConfig.ScreenSize.W;
						break;

					case 8:
						destSlideY = GameConfig.ScreenSize.H;
						break;

					case 2:
						destSlideY = -GameConfig.ScreenSize.H;
						break;

					default:
						throw null; // never
				}

				using (DU.FreeScreen.Section())
				{
					this.DrawWall();
					this.DrawMap();
				}

				foreach (Scene scene in Scene.Create(30))
				{
					double slideX = destSlideX * scene.Rate;
					double slideY = destSlideY * scene.Rate;

					DD.DrawCurtain(-1.0);
					DD.Draw(
						DU.FreeScreen.GetPicture(),
						new D2Point(
							GameConfig.ScreenSize.W / 2.0 + slideX,
							GameConfig.ScreenSize.H / 2.0 + slideY
							)
						);

					DD.EachFrame();
				}
				DD.SetCurtain(-1.0, 0);
			}

			// ★★★ ゲームメイン処理の終わり ★★★
		}

		/// <summary>
		/// マップから離れすぎている敵・自弾の死亡フラグを立てる。
		/// </summary>
		/// <returns>タスク：常に真</returns>
		private IEnumerable<bool> E_ゴミ回収()
		{
			for (; ; )
			{
				foreach (SAEnemy enemy in DD.Iterate(this.Enemies))
				{
					if (this.IsProbablyEvacuated(enemy.X, enemy.Y))
						enemy.DeadFlag = true;

					yield return true;
				}
				foreach (SAShot shot in DD.Iterate(this.Shots))
				{
					if (this.IsProbablyEvacuated(shot.X, shot.Y))
						shot.DeadFlag = true;

					yield return true;
				}
				yield return true; // 敵・自弾ゼロの場合のため
			}
		}

		/// <summary>
		/// 有効なマップから離れすぎているか判定する。
		/// </summary>
		/// <param name="x">フィールド位置(X)</param>
		/// <param name="y">フィールド位置(Y)</param>
		/// <returns>有効なマップから離れすぎているか</returns>
		private bool IsProbablyEvacuated(double x, double y)
		{
			const int MGN_SCREEN_NUM = 3;

			return
				x < -GameConfig.ScreenSize.W * MGN_SCREEN_NUM || this.Field.W + GameConfig.ScreenSize.W * MGN_SCREEN_NUM < x ||
				y < -GameConfig.ScreenSize.H * MGN_SCREEN_NUM || this.Field.H + GameConfig.ScreenSize.H * MGN_SCREEN_NUM < y;
		}

		private void カメラ位置調整(bool 瞬間移動)
		{
			double destCameraX = this.Player.X - GameConfig.ScreenSize.W / 2 + (this.CameraSlideX * GameConfig.ScreenSize.W / 3);
			double destCameraY = this.Player.Y - GameConfig.ScreenSize.H / 2 + (this.CameraSlideY * GameConfig.ScreenSize.H / 3);

			destCameraX = SCommon.ToRange(destCameraX, 0.0, this.Field.W - GameConfig.ScreenSize.W);
			destCameraY = SCommon.ToRange(destCameraY, 0.0, this.Field.H - GameConfig.ScreenSize.H);

			if (this.Field.W - GameConfig.ScreenSize.W < SAConsts.TILE_W) // ? カメラの横の可動域が1タイルより狭い場合
				destCameraX = (this.Field.W - GameConfig.ScreenSize.W) / 2; // 中心に合わせる。

			if (this.Field.H - GameConfig.ScreenSize.H < SAConsts.TILE_H) // ? カメラの縦の可動域が1タイルより狭い場合
				destCameraY = (this.Field.H - GameConfig.ScreenSize.H) / 2; // 中心に合わせる。

			DD.Approach(ref this.CameraForCalc.X, destCameraX, 瞬間移動 ? 0.0 : 0.8);
			DD.Approach(ref this.CameraForCalc.Y, destCameraY, 瞬間移動 ? 0.0 : 0.8);

			this.Camera.X = SCommon.ToInt(this.CameraForCalc.X);
			this.Camera.Y = SCommon.ToInt(this.CameraForCalc.Y);
		}

		private void DrawWall()
		{
			this.Field.DrawWall();
		}

		private void DrawMap()
		{
			int w = this.Field.Table_W;
			int h = this.Field.Table_H;

			int camera_l = this.Camera.X;
			int camera_t = this.Camera.Y;
			int camera_r = camera_l + GameConfig.ScreenSize.W;
			int camera_b = camera_t + GameConfig.ScreenSize.H;

			I2Point lt = SACommon.ToTablePoint(new D2Point(camera_l, camera_t));
			I2Point rb = SACommon.ToTablePoint(new D2Point(camera_r, camera_b));

			// マージン付与
			// -- タイルの範囲をはみ出て描画されるタイルのためにマージンを増やす。
			{
				const int MARGIN = 2; // マージン・タイル数

				lt.X -= MARGIN;
				lt.Y -= MARGIN;
				rb.X += MARGIN;
				rb.Y += MARGIN;
			}

			lt.X = SCommon.ToRange(lt.X, 0, w - 1);
			lt.Y = SCommon.ToRange(lt.Y, 0, h - 1);
			rb.X = SCommon.ToRange(rb.X, 0, w - 1);
			rb.Y = SCommon.ToRange(rb.Y, 0, h - 1);

			for (int x = lt.X; x <= rb.X; x++)
			{
				for (int y = lt.Y; y <= rb.Y; y++)
				{
					I2Point tilePt = new I2Point(x, y);
					D2Point drawPt = new D2Point(
						x * SAConsts.TILE_W + SAConsts.TILE_W / 2 - camera_l,
						y * SAConsts.TILE_H + SAConsts.TILE_H / 2 - camera_t
						);

					this.Field.DrawTile(tilePt, drawPt);
				}
			}
		}

		private void DrawFront()
		{
			// none
		}

		private static VScreen PauseWall = new VScreen(GameConfig.ScreenSize.W, GameConfig.ScreenSize.H);

		/// <summary>
		/// ポーズメニュー
		/// </summary>
		private void Pause()
		{
			using (PauseWall.Section())
			{
				DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
			}

			SimpleMenu menu = new SimpleMenu(24, 30, 16, 400, "PAUSE", new string[]
 			{
				"NOOP",
				"タイトルメニューに戻る",
				"ゲームに戻る",
			});

			menu.NoPound = true;
			menu.CancelByPause = true;

			DD.FreezeInputUntilRelease();

			double blurRate = 0.0;

			for (; ; )
			{
				DD.FreezeInput();

				for (; ; )
				{
					DD.Approach(ref blurRate, 0.5, 0.98);

					DD.Draw(PauseWall.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
					DD.Blur(blurRate);

					if (menu.Draw())
						break;

					DD.EachFrame();
				}
				DD.FreezeInput();

				switch (menu.SelectedIndex)
				{
					case 0:
						// noop
						break;

					case 1:
						this.ReturnToCallerRequested = true;
						goto endOfMenu;

					case 2:
						goto endOfMenu;

					default:
						throw null; // never
				}
			}
		endOfMenu:
			DD.FreezeInputUntilReleaseWithoutDirection();

			PauseWall.Unload();
		}

		private bool 当たり判定表示 = false;

		/// <summary>
		/// ポーズメニュー(デバッグ用)
		/// </summary>
		private void DebugPause()
		{
			SimpleMenu menu = new SimpleMenu(24, 30, 16, 400, "DEBUG", new string[]
 			{
				"強制遅延",
				"当たり判定表示",
				"ゲームに戻る",
			});

			menu.NoPound = true;
			menu.CancelByPause = true;

			DD.FreezeInputUntilRelease();

			for (; ; )
			{
				DD.FreezeInput();

				for (; ; )
				{
					DD.DrawCurtain(1.0);
					DD.DrawCurtain(-0.5);
					DD.SetPrint(410, 10, 20);
					DD.PrintLine("SlowdownLevel: " + DD.SlowdownLevel);
					DD.PrintLine("当たり判定表示：" + this.当たり判定表示);

					if (menu.Draw())
						break;

					DD.EachFrame();
				}
				DD.FreezeInput();

				switch (menu.SelectedIndex)
				{
					case 0:
						DD.SlowdownLevel = (DD.SlowdownLevel + 5) % 15;
						break;

					case 1:
						this.当たり判定表示 ^= true;
						break;

					case 2:
						goto endOfMenu;

					default:
						throw null; // never
				}
			}
		endOfMenu:
			DD.FreezeInputUntilReleaseWithoutDirection();
		}
	}
}
