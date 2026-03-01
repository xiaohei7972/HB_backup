using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_CORE_EX1_178 : SimTemplate //* ancientofwar
    {
        //Choose One - +5 Attack; or +5 Health and Taunt.

        public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (choice == 1 || p.ownFandralStaghelm > 1)
            {
                p.minionGetBuffed(own, 5, 0);
            }

            if (choice == 2 || p.ownFandralStaghelm > 1)
            {
                p.minionGetBuffed(own, 0, 5);
                p.minionGetTaunt(own);
            }
        }
    }
}