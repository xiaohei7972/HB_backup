using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 战士 费用：1 攻击力：2 生命值：1
	//Hand of Ragnaros (Colossal appendage)
	//拉格纳罗斯之手
	//Deathrattle: Deal 2 damage to a random enemy.
	//亡语：对一个随机敌人造成2点伤害。
	class Sim_CATA_150t : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			if (p.enemyMinions.Count > 0)
			{
				Minion target = p.enemyMinions[0];
				p.minionGetDamageOrHeal(target, 2);
			}
			else
			{
				p.minionGetDamageOrHeal(p.enemyHero, 2);
			}
		}
	}
}
