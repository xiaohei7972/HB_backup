using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BOT_234 : SimTemplate //* 萎缩射线 Shrink Ray
//Set the Attack and Health of all minionsto 1.
//将所有随从的攻击力和生命值变为1。 
	{
		

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            foreach (Minion m in p.ownMinions)
            {
				p.minionSetAttackToX(m, 1);
				p.minionSetHealthtoX(m, 1);
            }
            foreach (Minion m in p.enemyMinions)
            {
				p.minionSetAttackToX(m, 1);
				p.minionSetHealthtoX(m, 1);
            }				
		}
	}
}