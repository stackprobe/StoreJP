﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Drawings
{
	/// <summary>
	/// アルファ値の無い色または色の比率を表す。
	/// 各色は 0.0 ～ 1.0 を想定する。
	/// </summary>
	public struct D3Color
	{
		public double R;
		public double G;
		public double B;

		public D3Color(double r, double g, double b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		public D4Color WithAlpha(double a = 1.0)
		{
			return new D4Color(this.R, this.G, this.B, a);
		}

		public I3Color ToI3Color()
		{
			return new I3Color(
				SCommon.ToInt(this.R * 255.0),
				SCommon.ToInt(this.G * 255.0),
				SCommon.ToInt(this.B * 255.0)
				);
		}
	}
}
