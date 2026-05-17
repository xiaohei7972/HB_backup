using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：5 攻击力：4 生命值：4
	//Nozdormu, Bronze Aspect
	//诺兹多姆，青铜守护巨龙
	//[x]At the end of your turn, giveyour minions <b>Divine Shield</b>.Any that already had one__gain +3/+3 instead.
	//在你的回合结束时，使你的随从获得<b>圣盾</b>，已有圣盾的随从改为获得+3/+3。
	class Sim_CATA_473 : SimTemplate
	{
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (!turnEndOfOwner || triggerEffectMinion.own != turnEndOfOwner) return;
			if (triggerEffectMinion.Hp <= 0 || triggerEffectMinion.silenced) return;

			foreach (Minion m in p.ownMinions)
			{
				if (m.Hp <= 0) continue;
				if (m.divineShield)
				{
					// Already has Divine Shield — gain +3/+3 instead
					p.minionGetBuffed(m, 3, 3);
				}
				else
				{
					// Give Divine Shield
					p.minionGetDivineShield(m);
				}
			}

			p.evaluatePenality -= p.ownMinions.Count * 3;
		}
	}
}
