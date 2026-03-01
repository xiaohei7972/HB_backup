using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_EX1_178a : SimTemplate //* Rooted
    {
        //+5 Health and Taunt.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                p.minionGetBuffed(target, 0, 5);
                p.minionGetTaunt(target);
            }
        }
    }
}