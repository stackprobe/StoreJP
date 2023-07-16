using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte
{
	public static class Pictures
	{
		public static Picture Dummy = new Picture(@"General\Dummy.png");
		public static Picture WhiteBox = new Picture(@"General\WhiteBox.png");
		public static Picture WhiteCircle = new Picture(@"General\WhiteCircle.png");
		public static Picture Transparent = new Picture(@"General\Transparent.png");
		public static Picture Copyright = new Picture(@"Handmade\Copyright.png");

		public static Picture KAZUKIcghvbnkm = new Picture(@"Picture\ぱくたそ\KAZUKIcghvbnkm.png");

		public static Picture CirnoStand = new Picture(@"Picture\えむくろ\CirnoStand.png");
		public static Picture CirnoAttack = new Picture(@"Picture\えむくろ\CirnoAttack.png");
		public static Picture[] CirnoRun = new Picture[]
		{
			new Picture(@"Picture\えむくろ\CirnoRun_001.png"),
			new Picture(@"Picture\えむくろ\CirnoRun_002.png"),
			new Picture(@"Picture\えむくろ\CirnoRun_003.png"),
			new Picture(@"Picture\えむくろ\CirnoRun_004.png"),
		};
		public static Picture CirnoJump = new Picture(@"Picture\えむくろ\CirnoJump.png");

		public static Picture 石壁 = new Picture(@"Picture\出所不詳\石壁.png");
	}
}
