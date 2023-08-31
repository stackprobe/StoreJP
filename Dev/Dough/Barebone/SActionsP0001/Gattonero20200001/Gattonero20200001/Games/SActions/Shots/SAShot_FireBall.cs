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
	/// ファイアボール
	/// </summary>
	public class SAShot_FireBall : SAShot
	{
		private bool FacingLeft;
		private bool UwamukiFlag;
		private bool ShitamukiFlag;

		public SAShot_FireBall(double x, double y, bool facingLeft, bool uwamukiFlag, bool shitamukiFlag)
			: base(x, y, 2, false)
		{
			this.FacingLeft = facingLeft;
			this.UwamukiFlag = uwamukiFlag;
			this.ShitamukiFlag = shitamukiFlag;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			const double R = 20.0;

			double xAdd;
			double yAdd;

			if (this.UwamukiFlag)
			{
				xAdd = 4.0;
				yAdd = -8.0;
			}
			else if (this.ShitamukiFlag)
			{
				xAdd = 4.0;
				yAdd = 8.0;
			}
			else
			{
				xAdd = 8.0;
				yAdd = 0.0;

			}

			// 薄い壁すり抜け防止
			{
				if (this.UwamukiFlag || this.ShitamukiFlag)
				{
					if (SAShotCommon.Is自弾とプレイヤーの間には壁がある(this, false))
						this.Y = SAGame.I.Player.Y;
				}
				else
				{
					if (SAShotCommon.Is自弾とプレイヤーの間には壁がある(this, true))
						this.X = SAGame.I.Player.X;
				}
			}

			int bouncedCount = 0;

			for (int frame = 0; ; frame++)
			{
				this.X += xAdd * (this.FacingLeft ? -1 : 1);
				this.Y += yAdd;

				yAdd += 0.8; // += 重力加速度

				yAdd = Math.Min(yAdd, 19.0); // 落下速度制限

				// 跳ね返り
				{
					bool bounced = false;

					if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X - R, this.Y))).IsWall())
					{
						this.FacingLeft = false;
						bounced = true;
					}
					if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X + R, this.Y))).IsWall())
					{
						this.FacingLeft = true;
						bounced = true;
					}
					if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X, this.Y - R))).IsWall() && yAdd < 0.0)
					{
						yAdd *= -0.98;
						bounced = true;
					}
					if (SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X, this.Y + R))).IsWall() && 0.0 < yAdd)
					{
						yAdd *= -0.98;
						bounced = true;

						while (
							!SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X, this.Y))).IsWall() &&
							SAGame.I.Field.GetTile(SACommon.ToTablePoint(new D2Point(this.X, this.Y + R))).IsWall()
							)
							this.Y -= 1.0;
					}

					if (bounced)
					{
						bouncedCount++;

						if (20 <= bouncedCount) // ? 跳ね返り回数オーバー
						{
							DD.TL.Add(SCommon.Supplier(SAEffects.Explode(this.X, this.Y, 2.0)));
							break;
						}
					}
				}

				DD.SetSize(new D2Size(R * 2.0, R * 2.0));
				DD.SetBright(new I3Color(255, 128, 0).ToD3Color());
				DD.Draw(Pictures.WhiteCircle, new D2Point(this.X - SAGame.I.Camera.X, this.Y - SAGame.I.Camera.Y));

				this.Crash = Crash.CreateCircle(new D2Point(this.X, this.Y), R);

				yield return !SACommon.IsOutOfCamera(new D2Point(this.X, this.Y)); // カメラから出たら消滅する。
			}
		}
	}
}
