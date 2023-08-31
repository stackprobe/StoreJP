using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.TActions.Attacks;
using Charlotte.Games.TActions.Enemies;
using Charlotte.Games.TActions.Shots;
using Charlotte.Games.TActions.Tiles;
using Charlotte.Games.TActions.Walls;

namespace Charlotte.Games.TActions
{
	/// <summary>
	/// ゲームメイン
	/// </summary>
	public class TAGame : Anchorable<TAGame>
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

		public TAPlayer Player = new TAPlayer();
		public TAField Field;
		public TAWall Wall;

		public bool CameraSlideMode; // ? カメラ・スライド_モード中
		public int CameraSlideCount;
		public int CameraSlideX; // -1 ～ 1
		public int CameraSlideY; // -1 ～ 1
		public D2Point CameraForCalc; // カメラ位置計算用
		public I2Point Camera;

		public int Frame;
		public bool ReturnToCallerRequested = false;

		public List<TAEnemy> Enemies = new List<TAEnemy>();
		public List<TAShot> Shots = new List<TAShot>();

		public void Run(string fieldName)
		{
			DU.Touch();

			Func<bool> f_ゴミ回収 = SCommon.Supplier(this.E_ゴミ回収());

			this.Field = new TAField(fieldName);
			this.Field.Load();
			this.Field.ReloadEnemies();

			this.Wall = TAWallCatalog.I.CreateWall(this.Field.WallProperty);

			TAMusicCatalog.I.GetMusic(this.Field.MusicProperty).Play();

			this.Player.X = this.Field.W / 2.0;
			this.Player.Y = this.Field.H / 2.0;

			foreach (TAEnemy enemy in this.Enemies)
			{
				if (enemy is TAEnemy_Entrance)
				{
					TAEnemy_Entrance entrance = (TAEnemy_Entrance)enemy;

					if (entrance.IntoDirection == TAGameMaster.I.IntoDirection)
					{
						this.Player.X = enemy.X;
						this.Player.Y = enemy.Y;

						break;
					}
				}
			}
			this.Player.FaceDirection = TAGameMaster.I.LastFaceDirection; // 最後に向いていた方向

			DD.SetCurtain(-1.0, 0);
			DD.SetCurtain(0.0, 10);

			DD.FreezeInputUntilReleaseWithoutDirection();

			if (TAGameMaster.I.FieldToFieldFlag)
			{
				Inputs.A.UnfreezeInputUntilRelease(); // 低速移動の押しっぱなし
				Inputs.B.UnfreezeInputUntilRelease(); // 攻撃の押しっぱなし
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
				if (ProcMain.DEBUG && Keyboard.GetInput(DX.KEY_INPUT_F2) == 1)
				{
					this.Edit();
				}

				this.カメラ位置調整(this.Frame == 0);

				if (ProcMain.DEBUG && Keyboard.GetInput(DX.KEY_INPUT_T) == 1) // Attack テスト
				{
					this.Player.Attack = new TAAttack_Test0001();
				}

				if (this.Player.Attack != null) // プレイヤー攻撃中
				{
					if (this.Player.Attack.EachFrame()) // ? このプレイヤー攻撃を継続する。
						goto endOfPlayer;

					this.Player.Attack = null; // プレイヤー攻撃_終了
				}

				bool cameraSlide = false;

				// プレイヤー入力・移動
				{
					bool damageInputLock = 1 <= this.Player.DamageFrame;
					bool dir2 = !damageInputLock && 1 <= Inputs.DIR_2.GetInput();
					bool dir4 = !damageInputLock && 1 <= Inputs.DIR_4.GetInput();
					bool dir6 = !damageInputLock && 1 <= Inputs.DIR_6.GetInput();
					bool dir8 = !damageInputLock && 1 <= Inputs.DIR_8.GetInput();
					int dir; // 8方向+0vec_テンキー方式

					if (dir2 && dir4)
						dir = 1;
					else if (dir2 && dir6)
						dir = 3;
					else if (dir4 && dir8)
						dir = 7;
					else if (dir6 && dir8)
						dir = 9;
					else if (dir2)
						dir = 2;
					else if (dir4)
						dir = 4;
					else if (dir6)
						dir = 6;
					else if (dir8)
						dir = 8;
					else
						dir = 5;

					if (1 <= Inputs.L.GetInput())
					{
						dir = 5;
						cameraSlide = true;
					}

					bool slow =
						!damageInputLock && 1 <= Inputs.A.GetInput();
					bool attack =
						!damageInputLock && 1 <= Inputs.B.GetInput();

					double speed = TAConsts.PLAYER_SPEED;

					if (slow)
						speed = TAConsts.PLAYER_SLOW_SPEED;

					double nanameSpeed = speed / TAConsts.ROOT_OF_2;

					switch (dir)
					{
						case 4:
							this.Player.X -= speed;
							break;

						case 6:
							this.Player.X += speed;
							break;

						case 8:
							this.Player.Y -= speed;
							break;

						case 2:
							this.Player.Y += speed;
							break;

						case 1:
							this.Player.X -= nanameSpeed;
							this.Player.Y += nanameSpeed;
							break;

						case 3:
							this.Player.X += nanameSpeed;
							this.Player.Y += nanameSpeed;
							break;

						case 7:
							this.Player.X -= nanameSpeed;
							this.Player.Y -= nanameSpeed;
							break;

						case 9:
							this.Player.X += nanameSpeed;
							this.Player.Y -= nanameSpeed;
							break;

						case 5:
							break;

						default:
							throw null; // never
					}
					if (dir != 5 && !slow && !attack)
						this.Player.FaceDirection = dir;

					if (dir != 5)
						this.Player.MoveFrame++;
					else
						this.Player.MoveFrame = 0;

					if (this.Player.MoveFrame == 0) // 立ち止まったら座標を整数にする。
					{
						this.Player.X = (double)SCommon.ToInt(this.Player.X);
						this.Player.Y = (double)SCommon.ToInt(this.Player.Y);
					}

					if (attack)
						this.Player.AttackFrame++;
					else
						this.Player.AttackFrame = 0;
				}

				// プレイヤー攻撃
				{
					if (this.Player.AttackFrame % 5 == 1)
					{
						TAGame.I.Shots.Add(new TAShot_Normal(this.Player.X, this.Player.Y, this.Player.FaceDirection));
					}
				}

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
					if (TAConsts.PLAYER_DAMAGE_FRAME_MAX < ++this.Player.DamageFrame)
					{
						this.Player.DamageFrame = 0;
						this.Player.InvincibleFrame = 1;
						goto endOfDamage;
					}
					int frame = this.Player.DamageFrame; // 値域 == 2 ～ TAConsts.PLAYER_DAMAGE_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)TAConsts.PLAYER_DAMAGE_FRAME_MAX, (double)frame);

					// プレイヤー・ダメージ中の処理
					{
						if (frame == 2) // 初回のみ
						{
							SoundEffects.PlayerDamaged.Play();
						}

						D2Point speed = TACommon.GetDirectionSpeed(this.Player.FaceDirection, 5.0) * -1.0;

						for (int c = 0; c < 5; c++)
						{
							{
								I2Point pt = TACommon.ToTablePoint(new D2Point(this.Player.X, this.Player.Y));

								if (!this.Field.GetTile(pt).IsGround()) // ? 歩行可能な場所ではない -> これ以上ヒットバックさせない。
									break;
							}

							this.Player.X += speed.X;
							this.Player.Y += speed.Y;
						}
					}
				}
			endOfDamage:

				if (1 <= this.Player.InvincibleFrame) // ? プレイヤー無敵時間中
				{
					if (TAConsts.PLAYER_INVINCIBLE_FRAME_MAX < ++this.Player.InvincibleFrame)
					{
						this.Player.InvincibleFrame = 0;
						goto endOfInvincible;
					}
					int frame = this.Player.InvincibleFrame; // 値域 == 2 ～ GameConsts.PLAYER_INVINCIBLE_FRAME_MAX
					double rate = DD.RateAToB(2.0, (double)TAConsts.PLAYER_INVINCIBLE_FRAME_MAX, (double)frame);

					// プレイヤー無敵時間中の処理
					{
						// none
					}
				}
			endOfInvincible:

				// プレイヤー移動 -> プレイヤー入力と同時に行っている。

				// プレイヤー位置訂正
				{
					D2Point pt = new D2Point(this.Player.X, this.Player.Y);

					pt = TAPlayerPosition.Correct(pt);

					this.Player.X = pt.X;
					this.Player.Y = pt.Y;
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
					else
					{
						this.Player.Crash = Crash.CreatePoint(new D2Point(this.Player.X, this.Player.Y));
					}
				}
			endOfPlayer: // 特殊モーションからの合流点

				if (this.Player.X < 0.0) // ? マップの左側に出た。
				{
					this.ExitDirection = 4;
					break;
				}
				if (this.Field.W < this.Player.X) // ? マップの右側に出た。
				{
					this.ExitDirection = 6;
					break;
				}
				if (this.Player.Y < 0.0) // ? マップの上側に出た。
				{
					this.ExitDirection = 8;
					break;
				}
				if (this.Field.H < this.Player.Y) // ? マップの下側に出た。
				{
					this.ExitDirection = 2;
					break;
				}

				// プレイヤーの登場位置の関係などによりプレイヤーの位置が補正された場合を想定して、
				// 念のためカメラ位置の補正を行う。
				//
				if (this.Frame == 0)
				{
					this.カメラ位置調整(true);
				}

				// ====
				// 描画ここから
				// ====

				this.DrawWall();
				this.DrawMap();
				this.Player.Draw();

				foreach (TAEnemy enemy in this.Enemies)
				{
					if (enemy.DeadFlag) // ? 敵：既に死亡
						continue;

					enemy.Crash = Crash.CreateNone(); // reset
					enemy.Draw();
				}
				foreach (TAShot shot in this.Shots)
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
						DD.DrawCurtain(-0.8);

						const double ALPHA = 0.3;

						this.Player.Crash.Draw(new I3Color(255, 0, 0).ToD3Color().WithAlpha(1.0), this.Camera.ToD2Point());

						foreach (TAEnemy enemy in this.Enemies)
							enemy.Crash.Draw(new I3Color(255, 255, 255).ToD3Color().WithAlpha(ALPHA), this.Camera.ToD2Point());

						foreach (TAShot shot in this.Shots)
							shot.Crash.Draw(new I3Color(0, 255, 255).ToD3Color().WithAlpha(ALPHA), this.Camera.ToD2Point());
					}));
				}

				// ====
				// 描画ここまで
				// ====

				// ====
				// 当たり判定ここから
				// ====

				foreach (TAEnemy enemy in this.Enemies)
				{
					if (1 <= enemy.HP) // ? 敵：生存 && 無敵ではない
					{
						foreach (TAShot shot in this.Shots)
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
										shot.AttackPoint = 1; // 攻撃力を使い果たした。-- 適当な正常値として 1 をセットしておく。
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
									enemy.HP = 1; // 過剰に削った分を正す。-- 適当な正常値として 1 をセットしておく。
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

						GameStatus.I.HP -= enemy.AttackPoint;

						if (1 <= GameStatus.I.HP) // ? まだ生存している。
						{
							this.Player.DamageFrame = 1;
						}
						else // ? 死亡した。
						{
							GameStatus.I.HP = -1;
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
					DD.SetMosaic();
					DD.SetZoom(2.0);
					DD.Draw(
						TACommon.GetWalkPicture(Pictures.CirnoWalk, this.Player.FaceDirection, 1),
						new D2Point(
							(double)(int)(this.Player.X - TAGame.I.Camera.X),
							(double)(int)(this.Player.Y - TAGame.I.Camera.Y - 12.0)
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

				DD.TL.Add(SCommon.Supplier(TAEffects.Explode(this.Player.X, this.Player.Y, 5.0)));

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
				foreach (TAEnemy enemy in DD.Iterate(this.Enemies))
				{
					if (this.IsProbablyEvacuated(enemy.X, enemy.Y))
						enemy.DeadFlag = true;

					yield return true;
				}
				foreach (TAShot shot in DD.Iterate(this.Shots))
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

			if (this.Field.W - GameConfig.ScreenSize.W < TAConsts.TILE_W) // ? カメラの横の可動域が1タイルより狭い場合
				destCameraX = (this.Field.W - GameConfig.ScreenSize.W) / 2; // 中心に合わせる。

			if (this.Field.H - GameConfig.ScreenSize.H < TAConsts.TILE_H) // ? カメラの縦の可動域が1タイルより狭い場合
				destCameraY = (this.Field.H - GameConfig.ScreenSize.H) / 2; // 中心に合わせる。

			DD.Approach(ref this.CameraForCalc.X, destCameraX, 瞬間移動 ? 0.0 : 0.8);
			DD.Approach(ref this.CameraForCalc.Y, destCameraY, 瞬間移動 ? 0.0 : 0.8);

			this.Camera.X = SCommon.ToInt(this.CameraForCalc.X);
			this.Camera.Y = SCommon.ToInt(this.CameraForCalc.Y);
		}

		#region Edit

		private enum EditMode_e
		{
			Tile = 1,
			Enemy,
		}

		private int EditTileIndex;
		private int EditEnemyIndex;
		private EditMode_e EditMode;
		private Dictionary<string, TATile> Edit_Tiles;

		private void Edit()
		{
			this.Field.Load(); // イベント等でマップが書き変わる場合を想定して、再ロードする。

			// フィールド初期化
			this.EditTileIndex = 0;
			this.EditEnemyIndex = 0;
			this.EditMode = EditMode_e.Tile;
			this.Edit_Tiles = SCommon.CreateDictionary<TATile>();

			foreach (string tileName in TATileCatalog.I.GetNames())
				this.Edit_Tiles.Add(tileName, TATileCatalog.I.CreateTile(tileName));

			DD.FreezeInput();
			DX.SetMouseDispFlag(1); // マウスを表示する。

			I2Point lastMousePosition = Mouse.Position;
			int mouseRot = 0;

			const int MOUSE_WHEEL_SENSIT = 1; // ホイールの感度

			for (; ; )
			{
				D2Point mouseFieldPt = new I2Point(this.Camera.X + Mouse.Position.X, this.Camera.Y + Mouse.Position.Y).ToD2Point();
				I2Point mouseTilePt = TACommon.ToTablePoint(mouseFieldPt);

				bool mouseTilePtOutFlag =
					mouseTilePt.X < 0 || this.Field.Table_W <= mouseTilePt.X ||
					mouseTilePt.Y < 0 || this.Field.Table_H <= mouseTilePt.Y;

				mouseRot += Mouse.Rot;

				if (Keyboard.GetInput(DX.KEY_INPUT_F1) == 1) // F1 キー -> HELP
				{
					string RES_HELP = @"

---- HELP ----

F1 = HELP
F2 = Save + Exit
S = Save
L = Load
T = 敵・タイル配置モード切り替え
ホイール(先へ) = 選択(前へ)
ホイール(手前) = 選択(次へ)
左ホールド = セット
右ホールド = クリア
L-Shift          + 左ドラッグ = マップ移動
L-Ctrl           + 左ホールド = スポイト
L-Alt            + 左クリック = 自機をここへ移動
L-Shift + L-Ctrl + 左クリック = 塗りつぶし(実行前に自動セーブ)

----

Press F1 to Exit.

";

					string[] HELP = SCommon.TextToLines(RES_HELP.Trim()).ToArray();

					DD.FreezeInput();

					while (Keyboard.GetInput(DX.KEY_INPUT_F1) != 1)
					{
						DD.SetPrint(20, 20, 20);
						DD.SetPrintBorder(new I3Color(0, 128, 0), 1);

						foreach (string line in HELP)
							DD.PrintLine(line);

						DD.EachFrame();
					}
					DD.FreezeInput();
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_F2) == 1) // F2 キー -> Save + Exit
				{
					this.Field.Save();
					break;
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_S) == 1) // S キー -> Save
				{
					this.Field.Save();

					DD.TL.Add(DD.Keep(60, () =>
					{
						DD.SetPrint(GameConfig.ScreenSize.W - 10, 2, 0);
						DD.SetPrintBorder(new I3Color(0, 0, 255), 1);
						DD.Print("S");
					}));
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_L) == 1) // L キー -> Load
				{
					this.Field.Load();

					DD.TL.Add(DD.Keep(60, () =>
					{
						DD.SetPrint(GameConfig.ScreenSize.W - 10, 2, 0);
						DD.SetPrintBorder(new I3Color(255, 0, 0), 1);
						DD.Print("L");
					}));
				}
				if (Keyboard.GetInput(DX.KEY_INPUT_T) == 1) // T キー -> 敵・タイル配置モード切り替え
				{
					if (this.EditMode == EditMode_e.Tile)
						this.EditMode = EditMode_e.Enemy;
					else
						this.EditMode = EditMode_e.Tile;
				}
				if (mouseRot >= MOUSE_WHEEL_SENSIT) // ホイール(先へ) -> 選択(前へ)
				{
					mouseRot -= MOUSE_WHEEL_SENSIT;

					switch (this.EditMode)
					{
						case EditMode_e.Tile:
							this.EditTileIndex = (this.EditTileIndex + TATileCatalog.I.GetNames().Count - 1) % TATileCatalog.I.GetNames().Count;
							break;

						case EditMode_e.Enemy:
							this.EditEnemyIndex = (this.EditEnemyIndex + TAEnemyCatalog.I.GetNames().Count - 1) % TAEnemyCatalog.I.GetNames().Count;
							break;

						default:
							throw null; // never
					}
				}
				if (mouseRot <= -MOUSE_WHEEL_SENSIT) // ホイール(手前) -> 選択(次へ)
				{
					mouseRot += MOUSE_WHEEL_SENSIT;

					switch (this.EditMode)
					{
						case EditMode_e.Tile:
							this.EditTileIndex = (this.EditTileIndex + 1) % TATileCatalog.I.GetNames().Count;
							break;

						case EditMode_e.Enemy:
							this.EditEnemyIndex = (this.EditEnemyIndex + 1) % TAEnemyCatalog.I.GetNames().Count;
							break;

						default:
							throw null; // never
					}
				}

				if (1 <= Keyboard.GetInput(DX.KEY_INPUT_LSHIFT) && 1 <= Keyboard.GetInput(DX.KEY_INPUT_LCONTROL)) // ? 左シフト・左コントロール_押下
				{
					if (Mouse.L.GetInput() == -1) // 左クリック -> 塗りつぶし
					{
						this.Field.Save(); // 自動セーブ

						if (!mouseTilePtOutFlag)
						{
							switch (this.EditMode)
							{
								case EditMode_e.Tile:
									{
										string fillTileName = this.Edit_GetTileName();
										string targetTileName = this.Field.TileNameTable[mouseTilePt.X, mouseTilePt.Y];

										if (fillTileName != targetTileName)
										{
											this.EditFill(
												mouseTilePt,
												tilePt => this.Field.TileNameTable[tilePt.X, tilePt.Y] == targetTileName,
												tilePt => this.Field.TileNameTable[tilePt.X, tilePt.Y] = fillTileName
												);
										}
									}
									break;

								case EditMode_e.Enemy:
									{
										string fillEnemyName = this.Edit_GetEnemyName();
										string targetEnemyName = this.Field.EnemyNameTable[mouseTilePt.X, mouseTilePt.Y];

										if (fillEnemyName != targetEnemyName)
										{
											this.EditFill(
												mouseTilePt,
												tilePt => this.Field.EnemyNameTable[tilePt.X, tilePt.Y] == targetEnemyName,
												tilePt => this.Field.EnemyNameTable[tilePt.X, tilePt.Y] = fillEnemyName
												);
										}
									}
									break;
							}
						}
					}
				}
				else if (1 <= Keyboard.GetInput(DX.KEY_INPUT_LSHIFT)) // ? 左シフト_押下
				{
					if (1 <= Mouse.L.GetInput()) // 左ドラッグ -> マップ移動
					{
						// 移動
						this.CameraForCalc.X -= Mouse.Position.X - lastMousePosition.X;
						this.CameraForCalc.Y -= Mouse.Position.Y - lastMousePosition.Y;

						// 補正
						this.CameraForCalc.X = SCommon.ToRange(this.CameraForCalc.X, 0.0, this.Field.W - GameConfig.ScreenSize.W);
						this.CameraForCalc.Y = SCommon.ToRange(this.CameraForCalc.Y, 0.0, this.Field.H - GameConfig.ScreenSize.H);

						this.Camera = this.CameraForCalc.ToI2Point();
					}
				}
				else if (1 <= Keyboard.GetInput(DX.KEY_INPUT_LCONTROL)) // ? 左コントロール_押下
				{
					if (1 <= Mouse.L.GetInput()) // 左ホールド -> スポイト
					{
						if (!mouseTilePtOutFlag)
						{
							switch (this.EditMode)
							{
								case EditMode_e.Tile:
									this.Edit_SetTileName(this.Field.TileNameTable[mouseTilePt.X, mouseTilePt.Y]);
									break;

								case EditMode_e.Enemy:
									this.Edit_SetEnemyName(this.Field.EnemyNameTable[mouseTilePt.X, mouseTilePt.Y]);
									break;

								default:
									throw null; // never
							}
						}
					}
				}
				else if (1 <= Keyboard.GetInput(DX.KEY_INPUT_LALT)) // ? 左 ALT 押下
				{
					if (Mouse.L.GetInput() == -1) // 左クリック -> 自機をここへ移動
					{
						this.Player.X = mouseFieldPt.X;
						this.Player.Y = this.CameraForCalc.Y + (double)Mouse.Position.Y;

						DD.TL.Add(SCommon.Supplier(TAEffects.Explode(this.Player.X, this.Player.Y, 2.0))); // アクションが分かるように...
					}
				}
				else // ? シフト系_押下無し
				{
					if (1 <= Mouse.L.GetInput()) // 左ホールド -> セット
					{
						if (!mouseTilePtOutFlag)
						{
							switch (this.EditMode)
							{
								case EditMode_e.Tile:
									this.Field.TileNameTable[mouseTilePt.X, mouseTilePt.Y] = this.Edit_GetTileName();
									break;

								case EditMode_e.Enemy:
									this.Field.EnemyNameTable[mouseTilePt.X, mouseTilePt.Y] = this.Edit_GetEnemyName();
									break;

								default:
									throw null; // never
							}
						}
					}
					if (1 <= Mouse.R.GetInput()) // 右ホールド -> クリア
					{
						if (!mouseTilePtOutFlag)
						{
							switch (this.EditMode)
							{
								case EditMode_e.Tile:
									this.Field.TileNameTable[mouseTilePt.X, mouseTilePt.Y] = TATileCatalog.DEFAULT_TILE_NAME;
									break;

								case EditMode_e.Enemy:
									this.Field.EnemyNameTable[mouseTilePt.X, mouseTilePt.Y] = TAEnemyCatalog.DEFAULT_ENEMY_NAME;
									break;

								default:
									throw null; // never
							}
						}
					}
				}

				this.DrawWall();
				this.Edit_DrawMap();

				DD.SetPrint(2, 2, 0);
				DD.SetPrintBorder(new I3Color(128, 128, 128), 1);
				DD.Print(string.Format("---- EDIT MODE ---- Mode: {0} , Tile: [{1}] {2} , Enemy: [{3}] {4} (F1:HELP)"
					, this.EditMode == EditMode_e.Tile ? "TILE" : "ENEMY"
					, this.EditTileIndex, this.Edit_GetTileName()
					, this.EditEnemyIndex, this.Edit_GetEnemyName()
					));

				lastMousePosition = Mouse.Position;

				DD.EachFrame();
			}
			DD.FreezeInput();
			DX.SetMouseDispFlag(GameSetting.MouseCursorShow ? 1 : 0); // マウスの表示状態を戻す。

			// フィールド・クリア
			this.EditTileIndex = default(int);
			this.EditEnemyIndex = default(int);
			this.EditMode = default(EditMode_e);
			this.Edit_Tiles = null;

			// 再ロード
			this.Field.Load();
			this.Field.ReloadEnemies();
		}

		private void Edit_DrawMap()
		{
			int w = this.Field.Table_W;
			int h = this.Field.Table_H;

			int camera_l = this.Camera.X;
			int camera_t = this.Camera.Y;
			int camera_r = camera_l + GameConfig.ScreenSize.W;
			int camera_b = camera_t + GameConfig.ScreenSize.H;

			I2Point lt = TACommon.ToTablePoint(new D2Point(camera_l, camera_t));
			I2Point rb = TACommon.ToTablePoint(new D2Point(camera_r, camera_b));

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
						x * TAConsts.TILE_W + TAConsts.TILE_W / 2 - camera_l,
						y * TAConsts.TILE_H + TAConsts.TILE_H / 2 - camera_t
						);

					this.Edit_Tiles[this.Field.TileNameTable[x, y]].DrawSimply(tilePt, drawPt);

					if (this.EditMode == EditMode_e.Enemy)
					{
						string enemyName = this.Field.EnemyNameTable[x, y];

						if (enemyName == TAEnemyCatalog.DEFAULT_ENEMY_NAME)
						{
							DD.SetBright(new I3Color(0, 0, 128).ToD3Color());
							DD.SetAlpha(0.25);
							DD.Draw(Pictures.WhiteBox, D4Rect.XYWH(drawPt.X, drawPt.Y, TAConsts.TILE_W, TAConsts.TILE_H));
						}
						else
						{
							DD.SetBright(new I3Color(0, 0, 255).ToD3Color());
							DD.SetAlpha(0.5);
							DD.Draw(Pictures.WhiteBox, D4Rect.XYWH(drawPt.X, drawPt.Y, TAConsts.TILE_W, TAConsts.TILE_H));

							DD.TL.Add(DD.Once(() =>
							{
								DD.SetPrint((int)drawPt.X - 16, (int)drawPt.Y - 8, 0);
								DD.SetPrintBorder(new I3Color(0, 0, 255), 1);
								DD.Print(enemyName);
							}));
						}
					}
				}
			}
		}

		private string Edit_GetTileName()
		{
			return TATileCatalog.I.GetNames()[this.EditTileIndex];
		}

		private string Edit_GetEnemyName()
		{
			return TAEnemyCatalog.I.GetNames()[this.EditEnemyIndex];
		}

		private void Edit_SetTileName(string tileName)
		{
			int index = SCommon.IndexOf(TATileCatalog.I.GetNames(), v => v == tileName);

			if (index == -1)
				throw new Exception("Bad tileName: " + tileName);

			this.EditTileIndex = index;
		}

		private void Edit_SetEnemyName(string enemyName)
		{
			int index = SCommon.IndexOf(TAEnemyCatalog.I.GetNames(), v => v == enemyName);

			if (index == -1)
				throw new Exception("Bad enemyName: " + enemyName);

			this.EditEnemyIndex = index;
		}

		private void EditFill(I2Point startTilePt, Predicate<I2Point> isFillable, Action<I2Point> fill)
		{
			Queue<I2Point> q = new Queue<I2Point>();

			q.Enqueue(startTilePt);

			while (1 <= q.Count)
			{
				I2Point tilePt = q.Dequeue();

				// ? テーブル(マップ)の外側 -> 除外
				if (
					tilePt.X < 0 || this.Field.Table_W <= tilePt.X ||
					tilePt.Y < 0 || this.Field.Table_H <= tilePt.Y
					)
					continue;

				if (!isFillable(tilePt)) // ? 塗りつぶし対象外 -> 除外
					continue;

				fill(tilePt);

				q.Enqueue(new I2Point(tilePt.X - 1, tilePt.Y));
				q.Enqueue(new I2Point(tilePt.X + 1, tilePt.Y));
				q.Enqueue(new I2Point(tilePt.X, tilePt.Y - 1));
				q.Enqueue(new I2Point(tilePt.X, tilePt.Y + 1));
			}
		}

		#endregion

		private void DrawWall()
		{
			this.Wall.Draw();
		}

		private void DrawMap()
		{
			int w = this.Field.Table_W;
			int h = this.Field.Table_H;

			int camera_l = this.Camera.X;
			int camera_t = this.Camera.Y;
			int camera_r = camera_l + GameConfig.ScreenSize.W;
			int camera_b = camera_t + GameConfig.ScreenSize.H;

			I2Point lt = TACommon.ToTablePoint(new D2Point(camera_l, camera_t));
			I2Point rb = TACommon.ToTablePoint(new D2Point(camera_r, camera_b));

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
						x * TAConsts.TILE_W + TAConsts.TILE_W / 2 - camera_l,
						y * TAConsts.TILE_H + TAConsts.TILE_H / 2 - camera_t
						);

					this.Field.GetTile(tilePt).Draw(tilePt, drawPt);
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
					DD.PrintLine("DD.TL.Count: " + DD.TL.Count);

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
