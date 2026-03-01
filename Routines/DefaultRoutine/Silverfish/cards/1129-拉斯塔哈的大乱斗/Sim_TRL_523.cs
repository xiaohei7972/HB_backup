using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_TRL_523 : SimTemplate //* 火树巫医 Firetree Witchdoctor
//[x]<b>Battlecry:</b> If you're holdinga Dragon, <b>Discover</b> a spell.
//<b>战吼：</b>如果你的手牌中有龙牌，便<b>发现</b>一张法术牌。 
	{
		

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON)) p.drawACard(CardDB.cardNameEN.frostbolt, m.own, true);
			}
        }
    }
}