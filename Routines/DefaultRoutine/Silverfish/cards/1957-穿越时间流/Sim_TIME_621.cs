using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：3 攻击力：3 生命值：4
	//Scalebane Saboteur
	//鳞甲龙破坏者
	//<b>Battlecry:</b> Give your opponent a Poisoned Coin <i>(deals 2 damage when drawn)</i>.
	//<b>战吼：</b>将一张毒硬币塞入对手的手牌<i>（抽到时造成2点伤害）</i>。
	class Sim_TIME_621 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Give opponent a Poisoned Coin (deals 2 damage when drawn)
			if (own.own)
			{
				// Draw a card for the opponent (poisoned coin effect)
				// The card will deal 2 damage when drawn
				if (p.enemyHand.Count < 10)
				{
					p.enemyHand.Add(new Handmanager.Handcard());
				}
				// Deal 2 damage to enemy hero as immediate effect (simplified)
				p.minionGetDamageOrHeal(p.enemyHero, 2);
			}
		}
	}
}
