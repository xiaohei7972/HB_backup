using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_KAR_062 : SimTemplate //* 虚空幽龙史学家 Netherspite Historian
//<b>Battlecry:</b> If you're holding a Dragon, <b>Discover</b>a Dragon.
//<b>战吼：</b>如果你的手牌中有龙牌，便<b>发现</b>一张龙牌。 
	{
		
		
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
			if(own.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON))
				{
					p.drawACard(CardDB.cardNameEN.drakonidcrusher, own.own, true);
                }
			}
			else
			{
				if (p.enemyAnzCards >= 2)
				{
					p.drawACard(CardDB.cardNameEN.drakonidcrusher, own.own, true);
                }					
			}
        }
    }
}