using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_BRM_033 : SimTemplate //* 黑翼技师 Blackwing Technician
//<b>Battlecry:</b> If you're holding a Dragon, gain +1/+1.
//<b>战吼：</b>如果你的手牌中有龙牌，便获得+1/+1。 
    {
        

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON)) p.minionGetBuffed(m, 1, 1);
			}
			else
			{
                if (p.enemyAnzCards >= 2) p.minionGetBuffed(m, 1, 1);
			}
        }
    }
}