using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;

namespace Charlotte.Games.SActions
{
	public class SAGameMaster : Anchorable<SAGameMaster>
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
		/// フィールドからフィールドへ移動したか
		/// </summary>
		public bool FieldToFieldFlag = false;

		public void Run(string fieldName)
		{
			for (; ; )
			{
				using (new SAGame())
				{
					SAGame.I.Run(fieldName);

					switch (SAGame.I.ExitDirection)
					{
						case 4:
						case 6:
						case 8:
						case 2:
							fieldName = SAWorld.I.GetNextFieldName(fieldName, SAGame.I.ExitDirection);
							IntoDirection = 10 - SAGame.I.ExitDirection;
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
