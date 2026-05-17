using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 猎人 费用：3 攻击力：4 生命值：3
	//Darkscale Broodmother
	//暗鳞巢母
	//Battlecry: If you're holding a Dragon, refresh 2 Mana Crystals.
	//战吼：如果你的手牌中有龙牌，复原两个法力水晶。
	class Sim_CATA_111 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Check if holding a Dragon (race value 2 in Hearthstone)
			bool hasDragon = false;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if ((int)hc.card.race == 2) // DRAGON race
				{
					hasDragon = true;
					break;
				}
			}

			if (hasDragon)
			{
				p.mana = System.Math.Min(p.ownMaxMana, p.mana + 2);
				p.evaluatePenality -= 4;
			}
		}
	}
}
