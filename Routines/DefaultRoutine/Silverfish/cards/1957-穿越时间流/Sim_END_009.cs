using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：4
	//Splintered Reality
	//碎裂现实
	//Summon two 2/2 Treants. They gain +1/+1 for each friendly Treant that died this game.
	//召唤两个2/2的树人。在本局对战中，每有一个友方树人死亡，使这些树人获得+1/+1。
	class Sim_END_009 : SimTemplate
	{
		private CardDB.Card treant = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;

			for (int i = 0; i < 2; i++)
			{
				p.callKid(treant, pos, ownplay);
			}

			p.evaluatePenality -= 4;
		}
	}
}
