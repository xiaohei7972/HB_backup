using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_AT_071 : SimTemplate //* 阿莱克丝塔萨的勇士 Alexstrasza's Champion
//<b>Battlecry:</b> If you're holding a Dragon, gain +1 Attack and <b>Charge</b>.
//<b>战吼：</b>如果你的手牌中有龙牌，便获得+1攻击力和<b>冲锋</b>。 
	{
		

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON))
				{
					p.minionGetBuffed(m, 1, 0);
					p.minionGetCharge(m);

				}
			}
			else
			{
				if (p.enemyAnzCards >= 2)
				{
					p.minionGetBuffed(m, 1, 0);
					p.minionGetCharge(m);
				}
			}
        }
    }
}