using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_AT_123 : SimTemplate //* 冰喉 Chillmaw
//[x]<b>Taunt</b><b>Deathrattle:</b> If you're holdinga Dragon, deal 3 damageto all minions.
//<b>嘲讽，亡语：</b>如果你的手牌中有龙牌，则对所有随从造成3点伤害。 
	{
		

		public override void onDeathrattle(Playfield p, Minion m)
        {
			if(m.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON)) p.allMinionsGetDamage(3);
			}
			else
			{
				if (p.enemyAnzCards >= 1) p.allMinionsGetDamage(3);
			}
        }
    }
}