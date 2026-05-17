using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：7 攻击力：6 生命值：6
	//Warmaster Blackhorn
	//战争大师黑角
	//<b>Battlecry:</b> Destroy all cards that cost (2) or less in both players' decks.
	//<b>战吼：</b>摧毁双方玩家牌库中所有法力值消耗小于或等于（2）点的牌。
	class Sim_CATA_720 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int ownDestroyed = 0;
			int enemyDestroyed = 0;

			// Destroy low-cost cards in own deck
			List<CardDB.Card> ownToRemove = new List<CardDB.Card>();
			foreach (CardDB.Card c in p.ownDeck)
			{
				if (c.cost <= 2)
				{
					ownToRemove.Add(c);
					ownDestroyed++;
				}
			}
			foreach (CardDB.Card c in ownToRemove)
			{
				p.ownDeck.Remove(c);
			}
			p.ownDeckSize = Math.Max(0, p.ownDeckSize - ownDestroyed);

			// Destroy low-cost cards in enemy deck (simulated via enemy deck tracking)
			// Use deck size and guess to approximate enemy deck destruction
			enemyDestroyed = (p.enemyDeckSize > 0) ? Math.Min(p.enemyDeckSize / 3, 10) : 0;
			p.enemyDeckSize = Math.Max(0, p.enemyDeckSize - enemyDestroyed);

			int totalDestroyed = ownDestroyed + enemyDestroyed;
			// Symmetrical effect: good for control, bad for aggro
			// Slightly beneficial when you have fewer low-cost cards
			p.evaluatePenality -= totalDestroyed;
		}
	}
}
