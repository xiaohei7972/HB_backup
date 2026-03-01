using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：2 攻击力：1 生命值：4
	//Behemoth's Lure
	//巨鳗的诱饵
	//[x]At the end of your turn,force a random enemyminion to attack the__Blackwater Behemoth.
	//在你的回合结束时，随机迫使一个敌方随从攻击黑水巨鳗。
	class Sim_TSC_216t : SimTemplate
	{
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (triggerEffectMinion.own == turnEndOfOwner)
			{
				List<Minion> minions = new List<Minion>(triggerEffectMinion.own ? p.ownMinions : p.enemyMinions);
				List<Minion> enemyMinions = new List<Minion>(triggerEffectMinion.own ? p.enemyMinions : p.ownMinions);
				foreach (Minion m in minions)
				{
					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_216)
					{
						foreach (Minion m2 in enemyMinions)
						{
							if (!m2.untouchable)
							{
								p.minionAttacksMinion(m2, m);
							}
							break;
						}
						break;
					}
				}
			}
		}

	}
}
