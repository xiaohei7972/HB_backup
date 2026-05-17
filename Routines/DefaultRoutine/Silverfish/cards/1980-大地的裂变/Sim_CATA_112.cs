using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：1 攻击力：2 生命值：1
	//Twilight Trickster
	//暮色戏法师
	//Battlecry: Discover a Dragon. If you're holding a Dragon, Discover from your opponent's class instead.
	//战吼：发现一张龙牌。如果你的手牌中有龙牌，改为从对手的职业中发现。
	class Sim_CATA_112 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Check if holding a Dragon (race value 2)
			bool hasDragon = false;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if ((int)hc.card.race == 2)
				{
					hasDragon = true;
					break;
				}
			}

			if (hasDragon)
			{
				// Discover from opponent's class - simplified: draw a random card
				p.drawACard(CardDB.cardIDEnum.None, own.own, true);
				p.evaluatePenality -= 3;
			}
			else
			{
				// Discover a Dragon - simplified: draw a random Dragon
				// Use a common Dragon card ID for simulation
				CardDB.cardIDEnum[] dragons = {
					CardDB.cardIDEnum.BRM_022,  // Blackwing Technician
					CardDB.cardIDEnum.BRM_031,  // Chromaggus
					CardDB.cardIDEnum.EX1_561,  // Alexstrasza
					CardDB.cardIDEnum.DRG_242,  // Faerie Dragon
					CardDB.cardIDEnum.BT_726,   // Azure Drake
				};
				System.Random rnd = new System.Random();
				CardDB.cardIDEnum chosen = dragons[rnd.Next(dragons.Length)];
				p.drawACard(chosen, own.own, true);
				p.evaluatePenality -= 3;
			}
		}
	}
}
