using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;

namespace Charlotte.Games.TActions
{
	public class TAGameMaster : Anchorable<TAGameMaster>
	{
		/// <summary>
		/// フィールドに侵入した方向
		/// 値：
		/// -- 4 == 左から入った。
		/// -- 6 == 右から入った。
		/// -- 8 == 上から入った。
		/// -- 2 == 下から入った。
		/// -- 5 == フィールド固有のスタート地点 (ゲームのスタート地点 or ロード地点)
		/// </summary>
		public int IntoDirection = 5;

		/// <summary>
		/// 最後にフィールドを出たときに向いていた方向(8方向_テンキー方式)
		/// </summary>
		public int LastFaceDirection = 2;

		/// <summary>
		/// フィールドからフィールドへ移動したか
		/// </summary>
		public bool FieldToFieldFlag = false;

		public void Run(string fieldName)
		{
			for (; ; )
			{
				using (new TAGame())
				{
					TAGame.I.Run(fieldName);

					switch (TAGame.I.ExitDirection)
					{
						case 4:
						case 6:
						case 8:
						case 2:
							fieldName = TAWorld.I.GetNextFieldName(fieldName, TAGame.I.ExitDirection);
							IntoDirection = 10 - TAGame.I.ExitDirection;
							LastFaceDirection = TAGame.I.Player.FaceDirection;
							FieldToFieldFlag = true;
							break;

						default:
							return;
					}
				}
			}
		}
	}
}
