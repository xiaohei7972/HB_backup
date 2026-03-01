using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：2
	//Shadow Word: Forbid
	//暗言术：禁
	//<b>Tradeable</b><b>Corrupted</b>. Destroy ALL 4-Attack minions.
	//<b>可交易</b>。<b>已腐蚀</b>消灭所有攻击力为4的随从。
	class Sim_WON_064ts : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			foreach (Minion m in p.ownMinions.ToArray())
            {
                if (m.untouchable) continue;
				if (m.Angr == 4)
                {
                    p.minionGetDestroyed(m);
                }
            }
            foreach (Minion m in p.enemyMinions.ToArray())
            {
                if (m.untouchable) continue;
                if (m.Angr == 4)
                {
                    p.minionGetDestroyed(m);
                }
            }
        }
		
	}
}
