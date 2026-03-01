using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：5
	//Clean the Scene
	//净场
	//[x]<b>Infused</b>Destroy all minions with 6 or less Attack.
	//<b>已注能</b>消灭所有攻击力小于或等于6的随从。
	class Sim_REV_252t : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			foreach (Minion m in p.ownMinions.ToArray())
            {
                if (m.untouchable) continue;
				if (m.Angr <= 6)
                {
                    p.minionGetDestroyed(m);
                }
            }
            foreach (Minion m in p.enemyMinions.ToArray())
            {
                if (m.untouchable) continue;
                if (m.Angr <= 6)
                {
                    p.minionGetDestroyed(m);
                }
            }
        }
		
	}
}
