using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：4
	//Wildwood Circle
	//野木法阵
	//Shatter. Summon two 2/2 Treants. Give your minions "Deathrattle: Summon a 2/2 Treant."
	//碎裂。召唤两个2/2的树人。使你的所有随从获得"亡语：召唤一个2/2的树人"。
	class Sim_CATA_134 : SimTemplate
	{
		// Treant card ID
		private static readonly CardDB.cardIDEnum Treant = CardDB.cardIDEnum.EX1_158t; // Soul of the Forest treant (2/2)

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;

			// Summon two 2/2 Treants
			for (int i = 0; i < 2; i++)
			{
				p.callKid(TreantInstance, pos, ownplay);
			}

			// Give all friendly minions Deathrattle: Summon a 2/2 Treant
			foreach (Minion m in p.ownMinions)
			{
				if (m.Hp > 0)
				{
					// Add deathrattle enchantment (represented by adding the card ID to enchs)
					if (!m.enchs.Contains(Treant))
					{
						m.enchs.Add(Treant);
					}
				}
			}

			// Board-wide buff is very strong
			p.evaluatePenality -= p.ownMinions.Count * 3;
		}

		private CardDB.Card TreantInstance
		{
			get { return CardDB.Instance.getCardDataFromID(Treant); }
		}
	}
}
