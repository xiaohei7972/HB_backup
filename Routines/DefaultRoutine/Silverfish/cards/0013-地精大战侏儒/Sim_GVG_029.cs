using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_GVG_029 : SimTemplate //* Ancestor's Call
    {

        //    Put a random minion from each player's hand into the battlefield.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            Handmanager.Handcard c = null;
            int sum = 10000;
            foreach (Handmanager.Handcard handcard in p.owncards)
            {
                if (handcard.card.type == CardDB.cardtype.MOB)
                {
                    int s = handcard.card.Health + handcard.card.Attack + ((handcard.card.tank) ? 1 : 0) + ((handcard.card.Shield) ? 1 : 0);
                    if (s < sum)
                    {
                        c = handcard;
                        sum = s;
                    }
                }
            }
            if (sum < 9999)
            {
                p.callKid(c.card, p.ownMinions.Count, true, false);
                p.removeCard(c);
                p.triggerCardsChanged(true);
            }

            if (p.enemyAnzCards >= 2)
            {
                p.callKid(c.card, p.enemyMinions.Count, false, false);
                p.enemyAnzCards--;
                p.triggerCardsChanged(false);
            }
        }
    }
}