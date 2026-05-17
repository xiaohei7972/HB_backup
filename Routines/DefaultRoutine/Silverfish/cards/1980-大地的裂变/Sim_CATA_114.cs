using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：4 攻击力：4 生命值：4
	//Dragon Sculptor
	//塑龙师
	//Battlecry: Choose a Dragon in your hand, transform it into a random Dragon costing (1) more.
	//战吼：选择你手牌中的一张龙牌，将其变形成为一张法力值消耗增加（1）点的随机龙牌。
	class Sim_CATA_114 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Find a Dragon card in hand
			Handmanager.Handcard dragonCard = null;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if ((int)hc.card.race == 2) // Dragon race
				{
					dragonCard = hc;
					break;
				}
			}

			if (dragonCard == null) return;

			// Calculate new cost (original + 1)
			int newCost = dragonCard.card.cost + 1;

			// Get a random Dragon costing (1) more
			// Simplified: replace with a random Dragon card
			CardDB.cardIDEnum[] dragons = {
				CardDB.cardIDEnum.BRM_022,
				CardDB.cardIDEnum.BRM_031,
				CardDB.cardIDEnum.EX1_561,
				CardDB.cardIDEnum.NEW1_030,
				CardDB.cardIDEnum.DRG_242,
				CardDB.cardIDEnum.BT_726,
			};

			System.Random rnd = new System.Random();
			CardDB.cardIDEnum chosen = dragons[rnd.Next(dragons.Length)];
			CardDB.Card newCard = CardDB.Instance.getCardDataFromID(chosen);

			// Replace the card in hand
			dragonCard.card = newCard;
			dragonCard.manacost = newCost;

			p.evaluatePenality -= 4;
		}
	}
}
