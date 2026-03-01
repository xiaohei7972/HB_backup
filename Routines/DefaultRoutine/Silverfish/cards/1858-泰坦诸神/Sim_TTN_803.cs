using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：2
	//Smelt
	//熔铸
	//[x]Give a random minion inyour hand +3/+3. Lose3 Armor to do it again.
	//随机使你手牌中的一张随从牌获得+3/+3。失去3点护甲值以重复一次。
	class Sim_TTN_803 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (ownplay)
            {
                Handmanager.Handcard handcard = p.searchRandomMinionInHand(p.owncards, searchmode.searchLowestCost, GAME_TAGs.Mob);
                if (handcard != null) 
                {
                    handcard.addattack += 3;
                    handcard.addHp += 3;
                    p.anzOwnExtraAngrHp += 6;
                }
            }
		
	    }
	}	
}
