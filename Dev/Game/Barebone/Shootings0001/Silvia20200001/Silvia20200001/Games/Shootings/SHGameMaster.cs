using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Shootings.Scenarios;

namespace Charlotte.Games.Shootings
{
	public class SHGameMaster : Anchorable<SHGameMaster>
	{
		public void Run(SHScenario scenario)
		{
			using (new SHGame())
			{
				SHGame.I.Run(scenario);
			}
		}
	}
}
