using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：10 攻击力：7 生命值：7
	//Fyrakk the Blazing
	//火光之龙菲莱克
	//[x]<b>Immune</b> to Fire spells.<b>Battlecry:</b> Cast 15 Manaworth of Fire spells atrandom enemies.
	//<b>免疫</b>火焰法术。<b>战吼：</b>随机对敌人施放消耗总计15点法力值的火焰法术。
	class Sim_FIR_959 : SimTemplate
	{
		// Immune to Fire spells is not simulatable.
		// Battlecry: Deal 15 damage split randomly among enemy characters.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int damage = 15;

			if (own.own)
			{
				// Deal split damage to enemy side
				for (int i = 0; i < damage; i++)
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
