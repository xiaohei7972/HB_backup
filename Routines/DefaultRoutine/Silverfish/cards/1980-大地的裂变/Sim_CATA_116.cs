using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：3 攻击力：2 生命值：4
	//Obsidian Smith
	//黑曜石铁匠
	//Battlecry: If you're holding a Dragon, give your weapon +2/+2.
	//战吼：如果你的手牌中有龙牌，使你的武器获得+2/+2。
	class Sim_CATA_116 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Check if holding a Dragon
			bool hasDragon = false;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if ((int)hc.card.race == 2)
				{
					hasDragon = true;
					break;
				}
			}

			if (!hasDragon) return;

			// Give your weapon +2/+2
			if (own.own)
			{
				if (p.ownWeapon.Angr > 0 || p.ownWeapon.Durability > 0)
				{
					p.ownWeapon.Angr += 2;
					p.ownWeapon.Durability += 2;
					p.evaluatePenality -= 4;
				}
			}
			else
			{
				if (p.enemyWeapon.Angr > 0 || p.enemyWeapon.Durability > 0)
				{
					p.enemyWeapon.Angr += 2;
					p.enemyWeapon.Durability += 2;
				}
			}
		}
	}
}
