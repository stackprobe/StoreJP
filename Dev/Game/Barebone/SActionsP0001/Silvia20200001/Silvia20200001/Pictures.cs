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

		public static Picture[,] PlayerStands = new Picture[,]
		{
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_00_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_00_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_01_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_01_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_02_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_02_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_03_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_03_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_04_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Stand_04_01.png"),
			},
		};
		public static Picture[,] PlayerTalks = new Picture[,]
		{
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_00_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_00_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_01_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_01_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_02_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_02_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_03_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_03_01.png"),
			},
			{
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_04_00.png"),
				new Picture(@"Picture\パタパタまじっく\ポチットさん\Talk_04_01.png"),
			},
		};
		public static Picture[] PlayerAttackDash = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackDash_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackDash_01.png"),
		};
		public static Picture[] PlayerAttackWalk = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackWalk_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackWalk_01.png"),
		};
		public static Picture[] PlayerDamage = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Damage_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Damage_01.png"),
		};
		public static Picture[] PlayerDash = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Dash_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Dash_01.png"),
		};
		public static Picture[] PlayerJump = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Jump_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Jump_01.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Jump_02.png"),
		};
		public static Picture[] PlayerStop = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Stop_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Stop_01.png"),
		};
		public static Picture[] PlayerWalk = new Picture[]
		{
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Walk_00.png"),
			new Picture(@"Picture\パタパタまじっく\ポチットさん\Walk_01.png"),
		};
		public static Picture PlayerAttack = new Picture(@"Picture\パタパタまじっく\ポチットさん\Attack.png");
		public static Picture PlayerAttackJump = new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackJump.png");
		public static Picture PlayerAttackShagami = new Picture(@"Picture\パタパタまじっく\ポチットさん\AttackShagami.png");
		public static Picture PlayerShagami = new Picture(@"Picture\パタパタまじっく\ポチットさん\Shagami.png");

		public static Picture 石壁 = new Picture(@"Picture\出所不詳\石壁.png");
	}
}
