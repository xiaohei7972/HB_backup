using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：2 攻击力：1 生命值：4
	//Coconut Cannoneer
	//椰子火炮手
	//After an adjacent minion attacks, deal 1 damage to a random enemy.
	//在相邻的随从攻击后，随机对一个敌人造成1点伤害。
	class Sim_VAC_532 : SimTemplate
	{

		public override void afterMinionAttack(Playfield p, Minion triggerEffectMinion, Minion attacker, Minion defender)
		{
			if (attacker.zonepos == triggerEffectMinion.zonepos - 1 || attacker.zonepos == triggerEffectMinion.zonepos + 1)
			{
				p.DealDamageToRandomCharacter(triggerEffectMinion.own, 1);
			}
		}
	}
}
