using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 牧师 费用：3 攻击力：3 生命值：3
	//Shadow Mender
	//暗影治愈者
	//Battlecry: Restore 3 Health to all friendly characters.
	//战吼：为所有友方角色恢复3点生命值。
	class Sim_CATA_123 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int heal = 3;

			// Heal own hero
			p.minionGetDamageOrHeal(p.ownHero, -heal);

			// Heal all friendly minions
			foreach (Minion m in p.ownMinions)
			{
				if (m.Hp > 0)
				{
					p.minionGetDamageOrHeal(m, -heal);
				}
			}

			// Healing value
			int healedCount = 1 + p.ownMinions.Count; // hero + minions
			p.evaluatePenality -= healedCount * 2;
		}
	}
}
