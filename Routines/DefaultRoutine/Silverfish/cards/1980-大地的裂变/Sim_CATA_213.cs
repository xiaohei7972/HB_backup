using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：6 攻击力：6 生命值：6
	//Vyranoth
	//威拉诺兹
	//[x]<b>Battlecry:</b> If the total Costof your starting minions was100, split 100 stats amongminions in your deck.
	//<b>战吼：</b>如果你的套牌中随从牌的法力值消耗之和为100，使你牌库中的随从获得总计100点的属性值。
	class Sim_CATA_213 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Count total minion cost in deck and hand (to estimate starting deck)
			int totalMinionCost = 0;
			int deckMinionCount = 0;

			foreach (CardDB.Card c in p.ownDeck)
			{
				if (c.type == CardDB.cardtype.MOB)
				{
					totalMinionCost += c.cost;
					deckMinionCount++;
				}
			}

			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.MOB)
				{
					totalMinionCost += hc.card.cost;
					deckMinionCount++;
				}
			}

			// Condition: total minion cost must be exactly 100
			if (totalMinionCost == 100 && deckMinionCount > 0)
			{
				// Split 100 stats evenly between attack and health
				int totalStats = 100;
				int perMinionAtk = (totalStats / 2) / deckMinionCount;
				int perMinionHp = (totalStats / 2) / deckMinionCount;

				// Apply buffs to minions already on board that came from deck
				foreach (Minion m in p.ownMinions)
				{
					if (m.Hp <= 0) continue;
					if (m.handcard.card.type == CardDB.cardtype.MOB && !m.playedFromHand)
						continue; // already on board when summoned
					// Buff minions that were in starting deck (approximation)
					p.minionGetBuffed(m, perMinionAtk, perMinionHp);
				}

				// Apply to hand minions as addattack (they get buff when played)
				foreach (Handmanager.Handcard hc in p.owncards)
				{
					if (hc.card.type == CardDB.cardtype.MOB)
					{
						hc.addattack += perMinionAtk;
						// Store health buff in TAG_SCRIPT_DATA_NUM_1 as a proxy
					}
				}

				p.evaluatePenality -= 12;
			}
		}
	}
}
