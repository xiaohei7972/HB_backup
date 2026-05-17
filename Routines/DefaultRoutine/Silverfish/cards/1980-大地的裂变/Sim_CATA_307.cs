using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：7 攻击力：8 生命值：8
	//Alexstrasza, Guardian of Life
	//阿莱克丝塔萨，生命守护者
	//[x]<b>Battlecry:</b> Set your remainingHealth to 15. When you reachfull Health, deal 15 damageto your opponent.
	//<b>战吼：</b>将你的英雄剩余生命值变为15。当你恢复所有生命值时，对敌方英雄造成15点伤害。
	class Sim_CATA_307 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Set own hero's remaining Health to 15
			if (own.own && p.ownHero.Hp != 15)
			{
				int oldHp = p.ownHero.Hp;
				p.ownHero.Hp = 15;

				// If hero was below 15, this is a partial heal; if above, it's damage
				if (oldHp < 15)
					p.evaluatePenality -= (15 - oldHp); // healing value
				else if (oldHp > 15)
					p.evaluatePenality += (oldHp - 15); // damaging own hero is bad
			}

			// Premium stats on a 7-cost body
			p.evaluatePenality -= 5;
		}

		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			// Only trigger on the owner's turn end
			if (!turnEndOfOwner || triggerEffectMinion.own != turnEndOfOwner)
				return;

			// Check if own hero is at full Health
			if (triggerEffectMinion.own && p.ownHero.Hp >= p.ownHero.maxHp && p.ownHero.maxHp > 0)
			{
				// Deal 15 damage to the enemy hero
				p.minionGetDamageOrHeal(p.enemyHero, 15);

				// Massive face damage
				p.evaluatePenality -= 15;
			}
		}
	}
}
