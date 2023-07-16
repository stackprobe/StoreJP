using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.Novels.Theaters
{
	public class NVTheater_Test0001 : NVTheater
	{
		public override void Enter()
		{
			DD.SetCurtain(0.0);
			DD.FreezeInput(20);
		}

		public override void Leave()
		{
			using (DU.FreeScreen.Section())
			{
				DD.Draw(DD.LastMainScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
			}

			DD.SetCurtain(-1.0);

			foreach (Scene scene in Scene.Create(40))
			{
				DD.Draw(DU.FreeScreen.GetPicture(), new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
				DD.EachFrame();
			}
		}

		private Picture LastBackground = Pictures.Transparent;
		private Picture Background = Pictures.Transparent;
		private double Background_A = 1.0;

		private class ActorInfo
		{
			private const double BASE_Z = 0.5;

			public string Name = "no-name";
			public Picture Picture = Pictures.Transparent;
			public double X = GameConfig.ScreenSize.W / 2.0;
			public double Y = GameConfig.ScreenSize.H / 2.0;
			public double Z = 1.0;
			public Func<bool> DrawTask = null;

			public void Draw()
			{
				if (this.DrawTask != null)
				{
					if (this.DrawTask())
						return;

					this.DrawTask = null;
				}

				DD.SetZoom(this.Z * BASE_Z);
				DD.Draw(this.Picture, new D2Point(this.X, this.Y));
			}

			public IEnumerable<bool> DrawTask_Move(double x1, double y1, double z1, double x2, double y2, double z2)
			{
				foreach (Scene scene in Scene.Create(30))
				{
					double s = DD.SCurve(scene.Rate);
					double x = DD.AToBRate(x1, x2, s);
					double y = DD.AToBRate(y1, y2, s);
					double z = DD.AToBRate(z1, z2, s);

					DD.SetZoom(z * BASE_Z);
					DD.Draw(this.Picture, new D2Point(x, y));

					yield return true;
				}
			}

			public IEnumerable<bool> DrawTask_ChangePicture(Picture lastPicture, Picture picture)
			{
				foreach (Scene scene in Scene.Create(30))
				{
					DD.SetZoom(this.Z * BASE_Z);
					DD.SetAlpha(DD.Parabola(scene.Rate * 0.5 + 0.5));
					DD.Draw(lastPicture, new D2Point(this.X, this.Y));

					DD.SetZoom(this.Z * BASE_Z);
					DD.SetAlpha(DD.Parabola(scene.Rate * 0.5));
					DD.Draw(picture, new D2Point(this.X, this.Y));

					yield return true;
				}
			}
		}

		private List<ActorInfo> Actors = new List<ActorInfo>();

		public override void Invoke(string command, string[] arguments)
		{
			int c = 0;

			if (NVTheaterCommon.Invoke(command, arguments))
			{
				// none
			}
			else if (command == "音楽再生")
			{
				Music music;

				switch (arguments[c++])
				{
					case "Remo": music = Musics.RemotestLibrary; break;
					case "SunB": music = Musics.SunBeams; break;

					default:
						throw null; // never
				}
				music.Play();
			}
			else if (command == "音楽停止")
			{
				Music.FadeOut();
			}
			else if (command == "背景変更")
			{
				Picture picture;

				switch (arguments[c++])
				{
					case "UKIc": picture = Pictures.KAZUKIcghvbnkm; break;
					case "7842": picture = Pictures.KAZ7842_gyftdrhfyg; break;

					default:
						throw null; // never
				}

				this.LastBackground = this.Background;
				this.Background = picture;
				this.Background_A = 0.0;
			}
			else if (command == "キャラクタ作成")
			{
				this.Actors.Add(new ActorInfo()
				{
					Name = arguments[c++],
				});
			}
			else if (command == "キャラクタ位置")
			{
				string name = arguments[c++];
				ActorInfo actor = this.Actors.First(v => v.Name == name);
				double x;
				double y;
				double z;

				switch (arguments[c++])
				{
					case "左端": x = 100.0; y = 300.0; z = 1.0; break;
					case "左中": x = 250.0; y = 300.0; z = 1.0; break;
					case "中央": x = 430.0; y = 300.0; z = 1.0; break;
					case "右中": x = 710.0; y = 300.0; z = 1.0; break;
					case "右端": x = 860.0; y = 300.0; z = 1.0; break;

					case "左端(BU)": x = 100.0; y = 500.0; z = 2.0; break;
					case "左中(BU)": x = 250.0; y = 500.0; z = 2.0; break;
					case "中央(BU)": x = 430.0; y = 500.0; z = 2.0; break;
					case "右中(BU)": x = 710.0; y = 500.0; z = 2.0; break;
					case "右端(BU)": x = 860.0; y = 500.0; z = 2.0; break;

					default:
						throw null; // never
				}

				actor.DrawTask = SCommon.Supplier(actor.DrawTask_Move(actor.X, actor.Y, actor.Z, x, y, z));
				actor.X = x;
				actor.Y = y;
				actor.Z = z;
			}
			else if (command == "キャラクタ画像")
			{
				string name = arguments[c++];
				ActorInfo actor = this.Actors.First(v => v.Name == name);
				Picture picture;

				switch (arguments[c++])
				{
					case "退場": picture = Pictures.Transparent; break;

					case "ゆかり(普)": picture = Pictures.結月ゆかり_普; break;
					case "ゆかり(喜)": picture = Pictures.結月ゆかり_喜; break;

					case "マキ(普)": picture = Pictures.弦巻マキ_普; break;
					case "マキ(怒)": picture = Pictures.弦巻マキ_怒; break;

					default:
						throw null; // never
				}

				actor.DrawTask = SCommon.Supplier(actor.DrawTask_ChangePicture(actor.Picture, picture));
				actor.Picture = picture;
			}
			else
			{
				throw new Exception("不明なコマンド");
			}
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				DD.Approach(ref this.Background_A, 1.0, 0.97);

				DD.Draw(this.LastBackground, new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));
				DD.SetAlpha(this.Background_A);
				DD.Draw(this.Background, new D2Point(GameConfig.ScreenSize.W / 2.0, GameConfig.ScreenSize.H / 2.0));

				foreach (ActorInfo actor in this.Actors)
					actor.Draw();

				yield return true;
			}
		}
	}
}
