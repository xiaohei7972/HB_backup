using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 战士 费用：8 攻击力：8 生命值：8
	//Ragnaros, the Great Fire
	//大灾变拉格纳罗斯
	//Colossal +2 - At the end of your turn, trigger your minions' Deathrattles.
	//巨型+2 —— 在你的回合结束时，触发你的所有随从的亡语。
	class Sim_CATA_150 : SimTemplate
	{
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (!turnEndOfOwner || triggerEffectMinion.own != turnEndOfOwner)
				return;

			// Trigger all friendly minions' deathrattles at end of turn
			foreach (Minion m in p.ownMinions.ToArray())
			{
				if (m.Hp <= 0 || m.silenced) continue;
				// Trigger the deathrattle directly
				m.handcard.card.sim_card.onDeathrattle(p, m);
			}
			// Also trigger own deathrattle if applicable
			if (triggerEffectMinion.Hp > 0 && !triggerEffectMinion.silenced)
			{
				triggerEffectMinion.handcard.card.sim_card.onDeathrattle(p, triggerEffectMinion);
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}
	}
}
