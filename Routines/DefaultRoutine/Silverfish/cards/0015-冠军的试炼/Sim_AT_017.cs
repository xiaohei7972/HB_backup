using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_AT_017 : SimTemplate //* 暮光守护者 Twilight Guardian
//<b>Battlecry:</b> If you're holding a Dragon, gain +1 Attack and <b>Taunt</b>.
//<b>战吼：</b>如果你的手牌中有龙牌，便获得+1攻击力和<b>嘲讽</b>。 
	{
		

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON))
				{
					p.minionGetBuffed(m, 1, 0);
					m.taunt = true;
                    p.anzOwnTaunt++;
                }
			}
			else
			{
				if (p.enemyAnzCards >= 2)
				{
					p.minionGetBuffed(m, 1, 0);
					m.taunt = true;
                    p.anzEnemyTaunt++;
                }					
			}
        }
    }
}