using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：7 攻击力：4 生命值：8
	//Vulcanos
	//沃坎诺斯
	//[x]<b>Colossal +2</b>At the end of your turn,deal 2 damage to allother minions.
	//<b>巨型+2</b>在你的回合结束时，对所有其他随从造成2点伤害。
	class Sim_CATA_488 : SimTemplate
	{
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			// Only trigger on the owner's turn end
			if (!turnEndOfOwner || triggerEffectMinion.own != turnEndOfOwner)
				return;

			// Deal 2 damage to all friendly minions except this one
			foreach (Minion m in p.ownMinions)
			{
				if (m.Hp <= 0) continue;
				if (m.entityID == triggerEffectMinion.entityID) continue;
				p.minionGetDamageOrHeal(m, 2);
			}

			// Deal 2 damage to all enemy minions
			foreach (Minion m in p.enemyMinions)
			{
				if (m.Hp <= 0) continue;
				p.minionGetDamageOrHeal(m, 2);
			}
		}
	}
}
