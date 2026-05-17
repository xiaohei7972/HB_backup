using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 中立 费用：0
	//Soul Fragment
	//灵魂残片
	//Casts When Drawn. Summon a random minion from your hand.
	//抽到时施放。随机从你的手牌中召唤一个随从。
	class Sim_CATA_118 : SimTemplate
	{
		public override void castsWhenDrawn(Playfield p, Handmanager.Handcard handcard, bool wasOwnCard)
		{
			// Summon a random minion from hand
			List<Handmanager.Handcard> minionsInHand = new List<Handmanager.Handcard>();
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.MOB)
				{
					minionsInHand.Add(hc);
				}
			}

			if (minionsInHand.Count == 0) return;

			// Pick a random minion
			System.Random rnd = new System.Random();
			Handmanager.Handcard chosen = minionsInHand[rnd.Next(minionsInHand.Count)];

			// Summon it
			int pos = p.ownMinions.Count;
			p.callKid(chosen.card, pos, wasOwnCard);

			// Remove the summoned minion from hand
			if (p.owncards.Contains(chosen))
			{
				p.owncards.Remove(chosen);
			}

			p.evaluatePenality -= 4;
		}
	}
}
