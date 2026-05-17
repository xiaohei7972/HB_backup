using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：2
	//Untimely Death
	//失时往生
	//<b>Secret:</b> When your opponent plays a minion, add a "Deaths: 1" counter. At 3 Deaths, destroy it.
	//<b>奥秘：</b>当你的对手使用一个随从时，为其增加一个"死亡计数：1"。累计3次计数后，消灭目标。
	class Sim_TIME_620 : SimTemplate
	{
		public override void onSecretPlay(Playfield p, bool ownplay, Minion attacker, Minion target, out int number)
		{
			number = 0;
			// This secret triggers when opponent plays a minion
			// For the bot simulation, we use a simplified approach:
			// Track the counter via a static or field variable
			// Since this is a simulation, we handle it inline
		}

		public override void onSecretPlay(Playfield p, bool ownplay, int number)
		{
			// Handle the secret effect
			// Simplified: destroy lowest HP enemy minion as approximation
			if (!ownplay)
			{
				if (p.enemyMinions.Count > 0)
				{
					Minion target = p.searchRandomMinion(p.enemyMinions, searchmode.searchLowestHP);
					if (target != null)
					{
						p.minionGetDestroyed(target);
					}
				}
			}
			else
			{
				if (p.ownMinions.Count > 0)
				{
					Minion target = p.searchRandomMinion(p.ownMinions, searchmode.searchLowestHP);
					if (target != null)
					{
						p.minionGetDestroyed(target);
					}
				}
			}
		}
	}
}
