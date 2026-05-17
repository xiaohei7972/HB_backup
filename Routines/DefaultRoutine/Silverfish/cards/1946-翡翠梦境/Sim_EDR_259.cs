using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：8 攻击力：9 生命值：7
	//Ursol
	//乌索尔
	//[x]<b>Battlecry:</b> Cast the highestCost spell from your hand as_an Aura that lasts @ turns.
	//<b>战吼：</b>将你手牌中法力值消耗最高的法术变为持续@回合的光环并施放。
	class Sim_EDR_259 : SimTemplate
	{
		// Battlecry: Cast the highest-cost spell from hand as an Aura.
		// Find the highest-cost spell and simulate its recurring effect.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int highestCost = 0;
			Handmanager.Handcard highestSpell = null;

			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.SPELL && hc.card.cost > highestCost)
				{
					highestCost = hc.card.cost;
					highestSpell = hc;
				}
			}

			if (highestSpell != null)
			{
				// The aura casts the spell effect each turn.
				// As approximation, deal damage to all enemies based on spell cost.
				if (own.own)
				{
					int dmg = Math.Max(1, highestCost);

					// Deal damage to all enemy minions
					foreach (Minion m in p.enemyMinions)
					{
						p.minionGetDamageOrHeal(m, dmg);
					}
				}
			}
		}

		// Aura: cast the spell effect again at end of each turn
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (turnEndOfOwner == triggerEffectMinion.own && triggerEffectMinion.own)
			{
				// Simplified: deal recurring damage to all enemy minions
				foreach (Minion m in p.enemyMinions)
				{
					p.minionGetDamageOrHeal(m, 2);
				}
			}
		}
		
	}
}
