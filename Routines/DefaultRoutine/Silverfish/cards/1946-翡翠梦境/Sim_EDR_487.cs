using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：7 攻击力：6 生命值：6
	//Wallow, the Wretched
	//瓦洛，污邪古树
	//While this is in your handor deck, it gains a copy of every <b>Dark Gift</b> given to your minions.
	//当本牌在你的手牌或牌库中时，会获得你的随从获得的每项<b>黑暗之赐</b>的复制。
	class Sim_EDR_487 : SimTemplate
	{
		// This minion gains copies of all Dark Gifts granted to friendly minions.
		// Dark Gifts are buffs applied to minions. We simulate this by getting
		// a large buff that represents accumulated Dark Gifts.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Simulate accumulated Dark Gifts as a significant stat boost
			// Dark Gifts typically give +4/+5 or similar buffs, and we assume
			// multiple dark gifts have been applied during the game.
			p.minionGetBuffed(own, 4, 5);
			own.divineShield = true;
			own.taunt = true;
		}
		
	}
}
