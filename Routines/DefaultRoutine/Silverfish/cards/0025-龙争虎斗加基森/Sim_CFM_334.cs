using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_CFM_334 : SimTemplate //* 走私货物 Smuggler's Crate
//Give a random Beast in your hand +2/+2.
//随机使你手牌中的一张野兽牌获得+2/+2。 
	{
		

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (ownplay)
            {
                Handmanager.Handcard handcard = p.searchRandomMinionInHand(p.owncards, searchmode.searchLowestCost, GAME_TAGs.CARDRACE, TAG_RACE.PET);
                if (handcard != null)
                {
                    handcard.addattack += 2;
                    handcard.addHp += 2;
                    p.anzOwnExtraAngrHp += 4;
                }
            }
        }
    }
}
