using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：3 攻击力：3 生命值：4
	//Gemstone Hoarder
	//宝石囤积者
	//[x]<b>Battlecry:</b> Choose a card inyour hand to discard.<b>Deathrattle:</b> Get it back.It costs (1) less.
	//<b>战吼：</b>选择你手牌中的一张牌并弃掉。<b>亡语：</b>重新获取弃掉的牌，其法力值消耗减少（1）点。
	class Sim_CATA_897 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Discard a random card from hand (simplified: discard lowest or random)
			if (p.owncards.Count == 0) return;

			// Choose a random card to discard
			System.Random rnd = new System.Random();
			int discardIndex = rnd.Next(p.owncards.Count);
			Handmanager.Handcard discarded = p.owncards[discardIndex];

			// Store the discarded card info on this minion for Deathrattle
			own.TAG_SCRIPT_DATA_NUM_1 = (int)discarded.card.cardIDenum;
			own.TAG_SCRIPT_DATA_NUM_2 = discarded.manacost;

			// Discard the card
			p.owncards.RemoveAt(discardIndex);

			p.evaluatePenality += 1; // Small penalty for discarding
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Get the discarded card back, costing (1) less
			CardDB.cardIDEnum storedCardId = (CardDB.cardIDEnum)m.TAG_SCRIPT_DATA_NUM_1;
			int storedCost = m.TAG_SCRIPT_DATA_NUM_2;

			if (storedCardId != CardDB.cardIDEnum.None)
			{
				// Add card back to hand
				p.drawACard(storedCardId, m.own, true);

				// Reduce cost by 1 on the newly added card
				if (m.own && p.owncards.Count > 0)
				{
					Handmanager.Handcard returned = p.owncards[p.owncards.Count - 1];
					if (returned.card.cardIDenum == storedCardId)
					{
						returned.manacost = Math.Max(0, storedCost - 1);
					}
				}

				p.evaluatePenality -= 3;
			}
		}
	}
}
