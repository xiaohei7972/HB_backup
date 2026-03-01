using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：9
	//Plague of Death
	//死亡之灾祸
	//<b>Silence</b> and destroy all_minions.
	//<b>沉默</b>并消灭所有随从。
	class Sim_ULD_718 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			foreach (Minion m in p.ownMinions.ToArray())
			{
				if (m.untouchable) continue;
				p.minionGetSilenced(m);
				p.minionGetDestroyed(m);

			}
			foreach (Minion m in p.enemyMinions.ToArray())
			{
				if (m.untouchable) continue;
				p.minionGetSilenced(m);
				p.minionGetDestroyed(m);

			}
		}

	}
}
