using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions.Shots
{
	/// <summary>
	/// レーザー
	/// </summary>
	public class SAShot_Laser : SAShot
	{
		private bool FacingLeft;
		private bool UwamukiFlag;
		private bool ShitamukiFlag;

		public SAShot_Laser(double x, double y, bool facingLeft, bool uwamukiFlag, bool shitamukiFlag)
			: base(x, y, 1, true)
		{
			this.FacingLeft = facingLeft;
			this.UwamukiFlag = uwamukiFlag;
			this.ShitamukiFlag = shitamukiFlag;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			const double LASER_R = 2.0;

			double x1 = this.X - LASER_R;
			double x2 = this.X + LASER_R;
			double y1 = this.Y - LASER_R;
			double y2 = this.Y + LASER_R;

			if (this.UwamukiFlag)
			{
				y1 = SAGame.I.Camera.Y;
				y2 = this.Y;
			}
			else if (this.ShitamukiFlag)
			{
				y1 = this.Y;
				y2 = SAGame.I.Camera.Y + GameConfig.ScreenSize.H;
			}
			else if (this.FacingLeft)
			{
				x1 = SAGame.I.Camera.X;
				x2 = this.X;
			}
			else
			{
				x1 = this.X;
				x2 = SAGame.I.Camera.X + GameConfig.ScreenSize.W;
			}

			// ? 照射距離が短すぎる -> 照射しない。
			if (
				x2 < x1 + 1.0 ||
				y2 < y1 + 1.0
				)
				goto endFunc;

			DD.SetAlpha(0.2 + 0.1 * Math.Sin(DD.ProcFrame / 2.0));
			DD.SetBright(new D3Color(0.0, 1.0, 1.0));
			DD.Draw(
				Pictures.WhiteBox,
				D4Rect.LTRB(
					x1 - SAGame.I.Camera.X,
					y1 - SAGame.I.Camera.Y,
					x2 - SAGame.I.Camera.X,
					y2 - SAGame.I.Camera.Y
					)
				);

			if (DD.ProcFrame % 5 == 0) // 間隔適当
				this.Crash = Crash.CreateRect(D4Rect.LTRB(x1, y1, x2, y2));

			// このフレームで生き残るために1回だけ真を返す。
			// -- 自弾が死亡するとクラッシュが無視される。
			yield return true;

		endFunc:
			;

			//yield break;
		}
	}
}
