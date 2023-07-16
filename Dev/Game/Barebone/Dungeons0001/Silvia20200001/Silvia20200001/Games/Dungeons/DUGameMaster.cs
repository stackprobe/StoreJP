using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Dungeons.Fields;

namespace Charlotte.Games.Dungeons
{
	public class DUGameMaster : Anchorable<DUGameMaster>
	{
		public void Run()
		{
			using (new DUGame())
			{
				DUGame.I.Run(DUField_Test0001.Create());
			}
		}
	}
}
