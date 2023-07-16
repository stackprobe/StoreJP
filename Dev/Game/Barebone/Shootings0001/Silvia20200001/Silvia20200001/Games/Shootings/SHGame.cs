using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Shootings.Enemies;
using Charlotte.Games.Shootings.Scenarios;
using Charlotte.Games.Shootings.Shots;

namespace Charlotte.Games.Shootings
{
	/// <summary>
	/// ゲームメイン
	/// </summary>
	public class SHGame : Anchorable<SHGame>
	{
		/// <summary>
		/// ゲームメイン処理を終了した理由
		/// 値：
		/// -- 1 == クリア
		/// -- 5 == 呼び出し側に戻る。
		/// -- 901 == 死亡した。
		/// </summary>
		public int ExitReason = 5;

		public SHPlayer Player = new SHPlayer();
		public SHScenario Scenario;

		public int Frame;
		public bool ReturnToCallerRequested = false;

		public List<SHEnemy> Enemies = new List<SHEnemy>();
		public List<SHShot> Shots = new List<SHShot>();

		public void Run(SHScenario scenario)
		{
			Func<bool> f_ゴミ回収 = SCommon.Supplier(this.E_ゴミ回収());

			this.Scenario = scenario;

			// プレイヤーの登場位置をセットする。
			//
			this.Player.X = GameConfig.ScreenSize.W / 4.0;
			this.Player.Y = GameConfig.ScreenSize.H / 2.0;

			this.Player.RebornFrame = 1;

			DD.SetCurtain(-1.0, 0);
			DD.SetCurtain(0.0, 20);

			DD.FreezeInputUntilReleaseWithoutDirection();

			for (this.Frame = 0; ; this.Frame++)
			{
				if (!this.Scenario.EachFrame())
					break;

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

				// プレイヤー行動
				{
					bool deadOrRebornInputLock = 1 <= this.Player.DeadFrame || 1 <= this.Player.RebornFrame;
					bool deadInputLock = 1 <= this.Player.DeadFrame;
					bool dir2 = !deadInputLock && 1 <= Inputs.DIR_2.GetInput();
					bool dir4 = !deadInputLock && 1 <= Inputs.DIR_4.GetInput();
					bool dir6 = !deadInputLock && 1 <= Inputs.DIR_6.GetInput();
					bool dir8 = !deadInputLock && 1 <= Inputs.DIR_8.GetInput();
					bool slow =
						!deadInputLock && 1 <= Inputs.A.GetInput();
					bool attack =
						!deadInputLock && 1 <= Inputs.B.GetInput();

					double xa = 0.0;
					double ya = 0.0;

					if (dir8)
						ya = -1.0;

					if (dir2)
						ya = 1.0;

					if (dir4)
						xa = -1.0;

					if (dir6)
						xa = 1.0;

					double speed;

					if (slow)
						speed = SHConsts.PLAYER_SLOW_SPEED;
					else
						speed = SHConsts.PLAYER_SPEED;

					this.Player.X += xa * speed;
					this.Player.Y += ya * speed;

					this.Player.X = SCommon.ToRange(this.Player.X, 0.0, GameConfig.ScreenSize.W);
					this.Player.Y = SCommon.ToRange(this.Player.Y, 0.0, GameConfig.ScreenSize.H);

					if (attack)
						this.Player.AttackFrame++;
					else
						this.Player.AttackFrame = 0;
				}

				// プレイヤー攻撃
				{
					if (this.Player.AttackFrame % 6 == 1)
					{
						SHGame.I.Shots.Add(new SHShot_Test0001(this.Player.X + 20.0, this.Player.Y - 48.0));
						SHGame.I.Shots.Add(new SHShot_Test0001(this.Player.X + 38.0, this.Player.Y - 16.0));
						SHGame.I.Shots.Add(new SHShot_Test0001(this.Player.X + 38.0, this.Player.Y + 16.0));
						SHGame.I.Shots.Add(new SHShot_Test0001(this.Player.X + 20.0, this.Player.Y + 48.0));

						SoundEffects.PlayerShoot.Play();
					}
				}

				if (1 <= this.Player.DeadFrame) // プレイヤー死亡中の処理
				{
					if (SHConsts.PLAYER_DEAD_FRAME_MAX < ++this.Player.DeadFrame)
					{
						Func<bool> getFalse = () => false;

						if (getFalse()) // ? 終了する (残機不足など)
						{
							this.ExitReason = 901;
							break;
						}
						this.Player.DeadFrame = 0;
						this.Player.RebornFrame = 1;
						goto endOfDead;
					}
					int frame = this.Player.DeadFrame; // 値域 == 2 ～ SHConsts.PLAYER_DEAD_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)SHConsts.PLAYER_DEAD_FRAME_MAX, (double)frame);

					// プレイヤー死亡中の処理
					{
						if (frame == 2) // 初回のみ
						{
							using (DU.FreeScreen.Section())
							{
								DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
							}

							foreach (Scene scene in Scene.Create(20))
							{
								DD.Draw(DU.FreeScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));

								DD.SetAlpha(0.3 + scene.Rate * 0.3);
								DD.SetBright(new D3Color(1.0, 0.0, 0.0));
								DD.Draw(Pictures.WhiteBox, new I4Rect(0, 0, GameConfig.ScreenSize.W, GameConfig.ScreenSize.H).ToD4Rect());

								DD.EachFrame();
							}
							DD.TL.Add(SCommon.Supplier(SHEffects.Explode(this.Player.X, this.Player.Y, 5.0)));
						}
					}
				}
			endOfDead:

				if (1 <= this.Player.RebornFrame) // プレイヤー再登場中の処理
				{
					if (SHConsts.PLAYER_REBORN_FRAME_MAX < ++this.Player.RebornFrame)
					{
						this.Player.RebornFrame = 0;
						this.Player.InvincibleFrame = 1;
						goto endOfReborn;
					}
					int frame = this.Player.RebornFrame; // 値域 == 2 ～ SHConsts.PLAYER_REBORN_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)SHConsts.PLAYER_REBORN_FRAME_MAX, (double)frame);

					// プレイヤー再登場中の処理
					{
						if (frame == 2) // 初回のみ
						{
							this.Player.Reborn_X = -50.0;
							this.Player.Reborn_Y = GameConfig.ScreenSize.H / 2.0;
						}
						DD.Approach(ref this.Player.Reborn_X, this.Player.X, 0.9 - 0.3 * rate);
						DD.Approach(ref this.Player.Reborn_Y, this.Player.Y, 0.9 - 0.3 * rate);
					}
				}
			endOfReborn:

				if (1 <= this.Player.InvincibleFrame) // プレイヤー無敵時間中の処理
				{
					if (SHConsts.PLAYER_INVINCIBLE_FRAME_MAX < ++this.Player.InvincibleFrame)
					{
						this.Player.InvincibleFrame = 0;
						goto endOfInvincible;
					}
					int frame = this.Player.InvincibleFrame; // 値域 == 2 ～ GameConsts.PLAYER_INVINCIBLE_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)SHConsts.PLAYER_INVINCIBLE_FRAME_MAX, (double)frame);

					// プレイヤー無敵時間中の処理
					{
						// none
					}
				}
			endOfInvincible:

				// プレイヤー当たり判定をセットする。
				// -- プレイヤー死亡中・再登場中・無敵時間中など、当たり判定無しの場合は Crash.CreateNone() をセットすること。
				{
					this.Player.Crash = Crash.CreateNone(); // reset

					if (1 <= this.Player.DeadFrame) // ? プレイヤー死亡中
					{
						// noop
					}
					else if (1 <= this.Player.RebornFrame) // ? プレイヤー再登場中
					{
						// noop
					}
					else if (1 <= this.Player.InvincibleFrame) // ? プレイヤー無敵時間中
					{
						// noop
					}
					else
					{
						this.Player.Crash = Crash.CreatePoint(new D2Point(this.Player.X, this.Player.Y));
					}
				}

				// ====
				// 描画ここから
				// ====

				this.Scenario.DrawWall();
				this.Player.Draw();

				foreach (SHEnemy enemy in this.Enemies)
				{
					if (enemy.DeadFlag) // ? 敵：既に死亡
						continue;

					enemy.Crash = Crash.CreateNone(); // reset
					enemy.Draw();
				}
				foreach (SHShot shot in this.Shots)
				{
					if (shot.DeadFlag) // ? 自弾：既に死亡
						continue;

					shot.Crash = Crash.CreateNone(); // reset
					shot.Draw();
				}
				this.DrawFront();

				if (ProcMain.DEBUG && 1 <= Inputs.R.GetInput()) // 当たり判定表示(チート)
				{
					DD.DrawCurtain(-0.7);

					const double ALPHA = 0.7;

					this.Player.Crash.Draw(new I3Color(255, 0, 0).ToD3Color().WithAlpha(1.0));

					foreach (SHEnemy enemy in this.Enemies)
						enemy.Crash.Draw(new I3Color(255, 255, 255).ToD3Color().WithAlpha(ALPHA));

					foreach (SHShot shot in this.Shots)
						shot.Crash.Draw(new I3Color(0, 255, 255).ToD3Color().WithAlpha(ALPHA));
				}

				// ====
				// 描画ここまで
				// ====

				// ====
				// 当たり判定ここから
				// ====

				foreach (SHEnemy enemy in this.Enemies)
				{
					if (
						1 <= enemy.HP && // ? 敵：生存 && 無敵ではない
						!SHCommon.IsOutOfScreen(new D2Point(enemy.X, enemy.Y)) // ? 画面内の敵である。
						)
					{
						foreach (SHShot shot in this.Shots)
						{
							// memo: ボスにボムは効かない！

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
									enemy.HP = 0; // 過剰に削られた分を正す。
									enemy.Kill(true);
									goto nextEnemy; // この敵は死亡したので、この敵について以降の当たり判定は不要
								}

								// ★ 敵_被弾ここまで
							}
						}
					}

					// 衝突判定：敵 x 自機
					if (
						this.Player.RebornFrame == 0 && // ? プレイヤー登場中ではない。
						this.Player.DeadFrame == 0 && // ? プレイヤー死亡中ではない。
						this.Player.InvincibleFrame == 0 && // ? プレイヤー無敵時間中ではない。
						!enemy.DeadFlag && // ? 敵：生存
						!SHCommon.IsOutOfScreen(new D2Point(enemy.X, enemy.Y)) && // ? 画面内の敵である。
						Crash.IsCrashed(enemy.Crash, this.Player.Crash) // ? 衝突
						)
					{
						// ★ 自機_被弾ここから

						this.Player.DeadFrame = 1;

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

			using (DU.FreeScreen.Section())
			{
				DD.Draw(DD.LastMainScreen.GetPicture(), new I2Point(GameConfig.ScreenSize.W / 2, GameConfig.ScreenSize.H / 2).ToD2Point());
			}

			Music.FadeOut();
			DD.SetCurtain(-1.0);

			foreach (Scene scene in Scene.Create(40))
			{
				DD.Draw(DU.FreeScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
				DD.EachFrame();
			}

			// ★★★ ゲームメイン処理の終わり ★★★
		}

		/// <summary>
		/// あまりにも画面から離れすぎている敵・自弾の死亡フラグを立てる。
		/// </summary>
		/// <returns></returns>
		private IEnumerable<bool> E_ゴミ回収()
		{
			for (; ; )
			{
				foreach (SHEnemy enemy in DD.Iterate(this.Enemies))
				{
					if (this.IsProbablyEvacuated(enemy.X, enemy.Y))
						enemy.DeadFlag = true;

					yield return true;
				}
				foreach (SHShot shot in DD.Iterate(this.Shots))
				{
					if (this.IsProbablyEvacuated(shot.X, shot.Y))
						shot.DeadFlag = true;

					yield return true;
				}
				yield return true; // ループ内で1度も実行されない場合を想定
			}
		}

		/// <summary>
		/// 画面から離れすぎているか判定する。
		/// </summary>
		/// <param name="x">画面上の位置(X)</param>
		/// <param name="y">画面上の位置(Y)</param>
		/// <returns>画面から離れすぎているか</returns>
		private bool IsProbablyEvacuated(double x, double y)
		{
			const int MGN_SCREEN_NUM = 3;

			return
				x < -GameConfig.ScreenSize.W * MGN_SCREEN_NUM || GameConfig.ScreenSize.W * (MGN_SCREEN_NUM + 1) < x ||
				y < -GameConfig.ScreenSize.H * MGN_SCREEN_NUM || GameConfig.ScreenSize.H * (MGN_SCREEN_NUM + 1) < y;
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
					DD.PrintLine("当たり判定表示：Rボタンで表示");

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
						// none
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
