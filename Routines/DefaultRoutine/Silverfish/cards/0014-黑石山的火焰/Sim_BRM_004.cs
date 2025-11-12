using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_BRM_004 : SimTemplate //* 暮光雏龙 Twilight Whelp
//<b>Battlecry:</b> If you're holding a Dragon, gain +2 Health.
//<b>战吼：</b>如果你的手牌中有龙牌，便获得+2生命值。 
    {
        

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON)) p.minionGetBuffed(m, 0, 2);
			}
			else
			{
                if (p.enemyAnzCards >= 2) p.minionGetBuffed(m, 0, 2);
			}
        }
    }
}