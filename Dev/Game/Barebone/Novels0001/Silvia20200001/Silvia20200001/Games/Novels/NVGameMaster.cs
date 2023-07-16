using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Drawings;
using Charlotte.Utilities;
using Charlotte.GameCommons;
using Charlotte.Games.Novels.Scenarios;

namespace Charlotte.Games.Novels
{
	public class NVGameMaster : Anchorable<NVGameMaster>
	{
		public bool ReturnToCallerRequested = false;

		public void Run(NVScenario scenario)
		{
			scenario.Theater.Enter();

			for (; ; )
			{
				using (new NVGame())
				{
					NVGame.I.Run(scenario);

					if (NVGame.I.ReturnToCallerRequested)
					{
						this.ReturnToCallerRequested = true;
						break;
					}

					// 次のシナリオへ進む
					{
						NVScenario nextScenario = NVGame.I.Scenario.GetNextScenario();

						if (nextScenario == null) // ? 次のシナリオは無い
							break;

						scenario = nextScenario;
					}
				}
			}
			scenario.Theater.Leave();
		}
	}
}
