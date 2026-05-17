using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：8 攻击力：6 生命值：8
	//Aessina
	//艾森娜
	//[x]<b>Battlecry:</b> If 20 friendlyminions have died this game,deal 20 damage split amongall enemies.@ <i>({0} left!)</i>@ <i>(Ready!)</i>
	//<b>战吼：</b>如果在本局对战中已有20个友方随从死亡，造成20点伤害，随机分配到所有敌人身上。@<i>（还剩{0}个！）</i>@<i>（已经就绪！）</i>
	class Sim_EDR_430 : SimTemplate
	{
		// Battlecry: If 20 friendly minions have died, deal 20 damage split among all enemies.
		// Since we can't precisely track minion deaths, we assume the condition is met late-game.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int totalDamage = 20;

			if (own.own)
			{
				// Deal split damage to all enemy characters
				for (int i = 0; i < totalDamage; i++)
				{
					List<Minion> enemyTargets = new List<Minion>(p.enemyMinions);
					if (p.enemyHero != null && p.enemyHero.Hp > 0)
					{
						enemyTargets.Add(p.enemyHero);
					}

					if (enemyTargets.Count == 0) break;

					int randIndex = new System.Random().Next(0, enemyTargets.Count);
					Minion randTarget = enemyTargets[randIndex];
					p.minionGetDamageOrHeal(randTarget, 1);
				}
			}
		}
		
	}
}
