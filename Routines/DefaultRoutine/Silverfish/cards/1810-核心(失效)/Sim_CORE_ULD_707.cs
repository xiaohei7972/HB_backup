using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：5
	//Plague of Wrath
	//愤怒之灾祸
	//Destroy all damaged minions.
	//消灭所有受伤的随从。
	class Sim_CORE_ULD_707 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			foreach (Minion m in p.ownMinions.ToArray())
			{
				if (m.untouchable) continue;
				if (m.wounded)
				{
					p.minionGetDestroyed(m);
				}
			}
			foreach (Minion m in p.enemyMinions.ToArray())
			{
				if (m.untouchable) continue;
				if (m.wounded)
				{
					p.minionGetDestroyed(m);
				}
			}
		}
		
	}
}
