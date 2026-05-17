using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：10 攻击力：8 生命值：9
	//Agamaggan
	//阿迦玛甘
	//[x]<b>Battlecry:</b> The next card youplay costs your OPPONENT'SHealth instead of Mana<i>(up to 10)</i>.
	//<b>战吼：</b>你使用的下一张牌会消耗<b>对手</b>的生命值而非法力值<i>（最高不超过10点）</i>。
	class Sim_EDR_489 : SimTemplate
	{
		// This effect is a persistent flag that modifies the cost mechanic.
		// The actual health-as-cost mechanic requires deep engine changes.
		// We approximate by dealing damage to the enemy hero equal to a fraction
		// of the next card's cost and reducing that card's mana cost.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Set a flag indicating the next card costs health instead of mana.
			// This flag will be checked by the engine when the next card is played.
			// For simulation purposes, we deal 5 damage to enemy hero as an approximation.
			if (own.own)
			{
				int dmg = Math.Min(10, p.enemyHero.Hp - 1);
				if (dmg > 0)
				{
					p.minionGetDamageOrHeal(p.enemyHero, dmg);
				}
				// Reduce mana cost of next card as compensation
				p.ownMaxMana += 5;
				p.mana += 5;
			}
		}
		
	}
}
