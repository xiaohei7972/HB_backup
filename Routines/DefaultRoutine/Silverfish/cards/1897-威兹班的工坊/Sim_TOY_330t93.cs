using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：2 攻击力：4 生命值：4
	//Haywire Module
	//失控模块
	//[x]At the end of your turn,deal 3 damage to your hero.
	//在你的回合结束时，对你的英雄造成3点伤害。
	class Sim_TOY_330t93 : SimTemplate
	{
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (triggerEffectMinion.own == turnEndOfOwner)
			{
				p.minionGetDamageOrHeal(triggerEffectMinion.own ? p.ownHero : p.enemyHero, 3);
			}
		}

	}
}
