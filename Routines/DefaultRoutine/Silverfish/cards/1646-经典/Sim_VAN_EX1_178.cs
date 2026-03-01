using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_VAN_EX1_178 : SimTemplate //* 战争古树 Ancient of War
	{
		//<b>Choose One -</b>+5 Attack; or +5 Health and <b>Taunt</b>.
		//<b>抉择：</b>+5攻击力；或者+5生命值并具有<b>嘲讽</b>。

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