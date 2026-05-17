using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 萨满祭司 费用：10
	//Volcanic Eruption
	//火山喷发
	//Deal $6 damage to all enemies. Overload: (2)
	//对所有敌人造成$6点伤害。过载：（2）
	class Sim_CATA_115 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int dmg = p.getSpellDamageDamage(6);

			// Deal 6 damage to all enemies (enemy minions and enemy hero)
			foreach (Minion m in p.enemyMinions)
			{
				if (m.Hp > 0)
				{
					p.minionGetDamageOrHeal(m, dmg);
				}
			}
			p.minionGetDamageOrHeal(p.enemyHero, dmg);

			// Overload: (2)
			if (ownplay)
			{
				p.ueberladung += 2;
			}

			p.evaluatePenality -= 10;
		}
	}
}
