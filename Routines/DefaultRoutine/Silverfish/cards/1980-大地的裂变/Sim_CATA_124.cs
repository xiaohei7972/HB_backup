using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：2 攻击力：2 生命值：3
	//Netherwalker
	//虚空行者
	//Battlecry: Discover a Demon from your deck.
	//战吼：从你的牌库中发现一张恶魔牌。
	class Sim_CATA_124 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Search deck for a Demon
			CardDB.Card demonCard = null;
			foreach (CardDB.Card c in p.ownDeck)
			{
				if ((int)c.race == 3) // Demon race
				{
					demonCard = c;
					break;
				}
			}

			if (demonCard != null)
			{
				// Draw the Demon from deck
				p.drawACard(demonCard.cardIDenum, own.own, true);
				p.evaluatePenality -= 4;
			}
			else
			{
				// No Demon in deck, draw a random card
				p.drawACard(CardDB.cardIDEnum.None, own.own, true);
				p.evaluatePenality -= 1;
			}
		}
	}
}
