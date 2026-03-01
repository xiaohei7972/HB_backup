using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//* 暗言术：毁 Shadow Word: Ruin
	//Destroy all minions with 5 or more Attack.
	//消灭所有攻击力大于或等于5的随从。
	class Sim_EX1_197 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			foreach (Minion m in p.ownMinions.ToArray())
			{
				if (m.untouchable) continue;
				if (m.Angr >= 5)
				{
					p.minionGetDestroyed(m);
				}
			}
			foreach (Minion m in p.enemyMinions.ToArray())
			{
				if (m.untouchable) continue;
				if (m.Angr >= 5)
				{
					p.minionGetDestroyed(m);
				}
			}
		}
	}
}
