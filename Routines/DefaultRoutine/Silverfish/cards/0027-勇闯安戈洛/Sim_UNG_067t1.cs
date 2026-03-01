using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_UNG_067t1 : SimTemplate //* 水晶核心 Crystal Core
	{
		//For the rest of the game, your minions are 5/5.
		//在本局对战的剩余时间内，你的所有随从变为5/5。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (ownplay)
            {
                p.ownCrystalCore = 5;
                foreach (Minion m in p.ownMinions)
                {
                    p.minionSetAttackToX(m, 4);
                    p.minionSetHealthtoX(m, 4);
                }

                foreach (Handmanager.Handcard handcard in p.owncards)
                {
                    handcard.addattack += 4 - handcard.card.Attack;
                    handcard.addHp += 4 - handcard.card.Health;
                }
            }
        }
    }
}
