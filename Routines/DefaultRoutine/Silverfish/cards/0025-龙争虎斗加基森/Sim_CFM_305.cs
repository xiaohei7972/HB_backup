using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 风驰电掣 Smuggler's Run
    //Give all minions in your hand +1/+1.
    //使你手牌中的所有随从牌获得+1/+1。 
    class Sim_CFM_305 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (ownplay)
            {
                foreach (Handmanager.Handcard handcard in p.owncards)
                {
                    if (handcard.card.type == CardDB.cardtype.MOB)
                    {
                        handcard.addattack++;
                        handcard.addHp++;
                        p.anzOwnExtraAngrHp += 2;
                    }
                }
            }
            
        }

    }
}