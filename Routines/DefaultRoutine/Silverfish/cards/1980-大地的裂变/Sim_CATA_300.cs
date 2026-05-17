using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：7 攻击力：5 生命值：9
	//The Black Blood
	//黑血
	//[x]<b>Colossal +3. </b>After you restore Healthto a character, attack arandom enemy minion.
	//<b>巨型+3</b>在你为一个角色恢复生命值后，随机攻击一个敌方随从。
	class Sim_CATA_300 : SimTemplate
	{
		public override void onAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion, int minionsGotHealed)
		{
			// After any friendly heal, attack a random enemy minion
			if (minionsGotHealed <= 0) return;
			if (triggerEffectMinion.Hp <= 0) return;

			// Attack a random enemy minion for each heal instance
			for (int i = 0; i < minionsGotHealed; i++)
			{
				if (p.enemyMinions.Count == 0) break;
				if (triggerEffectMinion.Hp <= 0) break;

				// Pick the first enemy minion (effectively "random" in AI sim)
				Minion target = p.searchRandomMinion(p.enemyMinions, searchmode.searchLowestHP);
				if (target == null || target.Hp <= 0) continue;

				// This minion attacks the target (deals attack damage, takes damage back)
				p.minionGetDamageOrHeal(target, triggerEffectMinion.Angr);
				p.minionGetDamageOrHeal(triggerEffectMinion, target.Angr);
			}
		}
	}
}
