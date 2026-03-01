using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_FP1_019 : SimTemplate //* 剧毒之种 Poison Seeds
//Destroy all minions and summon 2/2 Treants to replace them.
//消灭所有随从，并召唤等量的2/2树人代替他们。 
	{
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);


		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
            int pos1 = ownplay ? p.enemyMinions.Count : p.ownMinions.Count;
            int ownanz = p.ownMinions.Count;
            int enemanz = p.enemyMinions.Count;
            p.allMinionsGetDestroyed();
            for (int i = 0; i < ownanz; i++)
            {
                p.callKid(kid, pos, true);
            }
            for (int i = 0; i < enemanz; i++)
            {
                p.callKid(kid, pos1, false);
            }
		}

	}
}