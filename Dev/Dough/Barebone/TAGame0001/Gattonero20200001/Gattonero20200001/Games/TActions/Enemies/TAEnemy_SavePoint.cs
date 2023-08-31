using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions.Enemies
{
	public class TAEnemy_SavePoint : TAEnemy
	{
		public TAEnemy_SavePoint(double x, double y)
			: base(x, y, 0, 0, false)
		{ }

		private bool SavedFlag = false;

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (SavedFlag)
				{
					if (DD.GetDistance(new D2Point(TAGame.I.Player.X, TAGame.I.Player.Y), new D2Point(this.X, this.Y)) > 100.0) // ? 当たり判定_復活
					{
						this.SavedFlag = false;
					}
				}
				else
				{
					if (DD.GetDistance(new D2Point(TAGame.I.Player.X, TAGame.I.Player.Y), new D2Point(this.X, this.Y)) < 30.0) // 当たり判定
					{
						SaveMenu.Run();
						this.SavedFlag = true;
					}
				}

				if (!TACommon.IsOutOfCamera(new D2Point(this.X, this.Y), 100.0)) // カメラ外では描画しない。
				{
					DD.SetBright(new I3Color(255, 128, 0).ToD3Color());
					DD.SetRotate(DD.ProcFrame / 30.0);
					DD.SetSize(new D2Size(40.0, 40.0));
					DD.Draw(Pictures.WhiteBox, new D2Point(this.X - TAGame.I.Camera.X, this.Y - TAGame.I.Camera.Y));

					//this.Crash = null; // アイテム系につき当たり判定はセットしない。
				}
				yield return true;
			}
		}
	}
}
