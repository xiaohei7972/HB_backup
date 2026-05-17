using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：1
	//Hero's Call
	//英雄的召唤
	//Discover a Taunt minion. Gain Armor equal to its Cost.
	//发现一张具有嘲讽的随从牌。获得等同于其法力值消耗的护甲值。
	class Sim_CATA_121 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			// Discover a Taunt minion - simplified: add a random taunt minion to hand and gain armor
			CardDB.cardIDEnum[] tauntMinions = {
				CardDB.cardIDEnum.EX1_048,  // Spellbreaker (not taunt, but...)
				CardDB.cardIDEnum.CS2_186,  // War Golem (hasn't taunt)
				CardDB.cardIDEnum.EX1_110,  // Cairne Bloodhoof (no taunt)
				// Better pool of actual taunt minions:
				CardDB.cardIDEnum.EX1_396,  // Mogu'shan Warden (1/7 Taunt, cost 4)
				CardDB.cardIDEnum.CS2_121,  // Frostwolf Grunt (2/2 Taunt, cost 2)
				CardDB.cardIDEnum.EX1_032,  // Sunwalker (4/5 Taunt, Divine Shield, cost 6)
				CardDB.cardIDEnum.CS2_120,  // River Crocolisk (no)
				CardDB.cardIDEnum.EX1_399,  // Lord of the Arena (6/5 Taunt, cost 6)
			};

			System.Random rnd = new System.Random();
			CardDB.cardIDEnum chosenId = tauntMinions[rnd.Next(tauntMinions.Length)];
			CardDB.Card chosen = CardDB.Instance.getCardDataFromID(chosenId);

			// Add the chosen minion to hand
			p.drawACard(chosenId, ownplay, true);

			// Gain Armor equal to its Cost
			int armor = chosen.cost;
			if (ownplay)
			{
				p.minionGetArmor(p.ownHero, armor);
			}
			else
			{
				p.minionGetArmor(p.enemyHero, armor);
			}

			p.evaluatePenality -= (5 + armor);
		}
	}
}
