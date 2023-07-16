using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Dungeons.Fields;

namespace Charlotte.Games.Dungeons
{
	/// <summary>
	/// ゲームメイン
	/// </summary>
	public class DUGame : Anchorable<DUGame>
	{
		public DUField Field;

		public int X = 0;
		public int Y = 0;
		public int Direction = 8; // 4方向_テンキー方式

		public void Run(DUField field)
		{
			this.Field = field;
			this.Field.Initialize();

			int lastX = -1;
			int lastY = -1;
			int lastDirection = -1;

			// ループ初回でイベント・敵とエンカウントしても良いようにダンジョンを描画しておく
			DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);

			DD.SetCurtain(-1.0, 0);
			DD.SetCurtain(0.0);

			DD.FreezeInputUntilRelease();

			for (; ; )
			{
				// ? ループ初回 || 移動した || 方向転換した
				if (
					lastX != this.X ||
					lastY != this.Y ||
					lastDirection != this.Direction
					)
				{
					// 敵エンカウント・イベントなど此処へ

					lastX = this.X;
					lastY = this.Y;
					lastDirection = this.Direction;
				}

				if (Inputs.START.GetInput() == 1) // 暫定
					break;

				if (1 <= Inputs.DIR_8.GetInput()) // 前進
				{
					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, true);
						this.Draw();
						DD.EachFrame();
					}
					if (this.Field.GetWall(new I2Point(this.X, this.Y), this.Direction) == 1) // ? 壁衝突
					{
						SoundEffects.CrashToWall.Play();
					}
					else
					{
						switch (this.Direction)
						{
							case 4: this.X--; break;
							case 6: this.X++; break;
							case 8: this.Y--; break;
							case 2: this.Y++; break;

							default:
								throw null; // never
						}
					}
					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw();
						DD.EachFrame();
					}
				}
				else if (1 <= Inputs.DIR_4.GetInput()) // 左を向く
				{
					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(scene.Rate);
						DD.EachFrame();
					}
					this.Direction = DUCommon.RotL(this.Direction);

					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(scene.Rate - 1.0);
						DD.EachFrame();
					}
				}
				else if (1 <= Inputs.DIR_6.GetInput()) // 右を向く
				{
					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(-scene.Rate);
						DD.EachFrame();
					}
					this.Direction = DUCommon.RotR(this.Direction);

					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(1.0 - scene.Rate);
						DD.EachFrame();
					}
				}
				else if (1 <= Inputs.DIR_2.GetInput()) // 後ろを向く(右旋回)
				{
					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(-scene.Rate);
						DD.EachFrame();
					}
					this.Direction = DUCommon.RotR(this.Direction);

					foreach (Scene scene in Scene.Create(10))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw((1.0 - scene.Rate) * 2.0 - 1.0);
						DD.EachFrame();
					}
					this.Direction = DUCommon.RotR(this.Direction);

					foreach (Scene scene in Scene.Create(5))
					{
						DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
						this.Draw(1.0 - scene.Rate);
						DD.EachFrame();
					}
				}
				else // 動かない
				{
					// none
				}

				DUDungeonScreen.Draw(this.Field, this.X, this.Y, this.Direction, false);
				this.Draw();
				DD.EachFrame();
			}

			Music.FadeOut();
			DD.SetCurtain(-1.0);

			using (DU.FreeScreen.Section())
			{
				DD.Draw(DD.LastMainScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
			}

			foreach (Scene scene in Scene.Create(40))
			{
				DD.Draw(DU.FreeScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
				DD.EachFrame();
			}
			DD.FreezeInput();
		}

		private static VScreen DungeonView = new VScreen(790, 390);

		/// <summary>
		/// ダンジョン中のゲーム画面の描画を行う。
		/// 主要なゲーム画面であるため、色々なところから呼び出される想定。
		/// </summary>
		public void Draw(double xSlideRate = 0.0)
		{
			DD.Draw(
				Pictures.KAZ7842_gyftdrhfyg,
				new D2Point(
					GameConfig.ScreenSize.W / 2.0,
					GameConfig.ScreenSize.H / 2.0
					)
				);

			using (DungeonView.Section())
			{
				DD.Draw(
					DUDungeonScreen.GetScreen().GetPicture(),
					new D2Point(
						DungeonView.W / 2.0 + xSlideRate * 90.0,
						DungeonView.H / 2.0 - 70.0
						)
					);
			}

			DD.Draw(
				DungeonView.GetPicture(),
				new D2Point(
					GameConfig.ScreenSize.W / 2.0,
					GameConfig.ScreenSize.H / 2.0 - 70.0
					)
				);
		}
	}
}
