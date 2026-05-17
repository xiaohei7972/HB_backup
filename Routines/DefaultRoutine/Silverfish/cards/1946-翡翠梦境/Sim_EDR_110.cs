using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：5 攻击力：3 生命值：6
	//Sporegnasher
	//孢子尖牙怪
	//<b>Poisonous</b>. <b>Deathrattle:</b> Deal 1 damage to a random enemy minion.
	//<b>剧毒</b>。<b>亡语：</b>随机对一个敌方随从造成1点伤害。
	class Sim_EDR_110 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Deal 1 damage to a random enemy minion
			List<Minion> targets = m.own ? p.enemyMinions : p.ownMinions;
			if (targets.Count > 0)
			{
				// Target a random enemy minion (use first as approximation)
				Random rand = new Random();
				Minion target = targets[rand.Next(targets.Count)];
				p.minionGetDamageOrHeal(target, 1);
			}
		}

	}
}
