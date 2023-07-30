using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Drawings;

namespace Charlotte.Games.SActions.Enemies
{
	public class SAEnemy_SavePoint : SAEnemy
	{
		public SAEnemy_SavePoint(double x, double y)
			: base(x, y, 0, 0, false)
		{ }

		private bool SavedFlag = false;

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (SavedFlag)
				{
					if (DD.GetDistance(new D2Point(SAGame.I.Player.X, SAGame.I.Player.Y), new D2Point(this.X, this.Y)) > 150.0) // ? 当たり判定_復活
					{
						this.SavedFlag = false;
					}
				}
				else
				{
					if (DD.GetDistance(new D2Point(SAGame.I.Player.X, SAGame.I.Player.Y), new D2Point(this.X, this.Y)) < 50.0) // 当たり判定
					{
						SaveMenu.Run();
						this.SavedFlag = true;
					}
				}

				if (!SACommon.IsOutOfCamera(new D2Point(this.X, this.Y), 100.0)) // カメラ外では描画しない。
				{
					DD.SetBright(new I3Color(255, 128, 0).ToD3Color());
					DD.SetRotate(DD.ProcFrame / 30.0);
					DD.SetSize(new D2Size(40.0, 40.0));
					DD.Draw(Pictures.WhiteBox, new D2Point(this.X - SAGame.I.Camera.X, this.Y - SAGame.I.Camera.Y));

					//this.Crash = null; // アイテム系につき当たり判定はセットしない。
				}
				yield return true;
			}
		}
	}
}
