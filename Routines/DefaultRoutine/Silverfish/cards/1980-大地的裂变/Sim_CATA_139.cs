using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：6 攻击力：0 生命值：5
	//Wickerfang
	//柳牙
	//<b>Colossal +4</b>After one of Wickerfang's Legs gains stats, this gains them too.
	//<b>巨型+4</b>在柳牙的腿获得属性值后，本随从也会获得。
	class Sim_CATA_139 : SimTemplate
	{
		// Leg token card IDs
		private static readonly CardDB.cardIDEnum Leg1 = CardDB.cardIDEnum.CATA_139t;
		private static readonly CardDB.cardIDEnum Leg2 = CardDB.cardIDEnum.CATA_139t2;
		private static readonly CardDB.cardIDEnum Leg3 = CardDB.cardIDEnum.CATA_139t3;
		private static readonly CardDB.cardIDEnum Leg4 = CardDB.cardIDEnum.CATA_139t4;

		// Summon Colossal legs
		public override void SummonColossal(Playfield p, Minion m)
		{
			int pos = m.own ? p.ownMinions.Count : p.enemyMinions.Count;
			bool own = m.own;
			p.callKid(CardDB.Instance.getCardDataFromID(Leg1), pos, own);
			p.callKid(CardDB.Instance.getCardDataFromID(Leg2), pos, own);
			p.callKid(CardDB.Instance.getCardDataFromID(Leg3), pos, own);
			p.callKid(CardDB.Instance.getCardDataFromID(Leg4), pos, own);
		}

		// At end of turn, sync all leg stat gains to Wickerfang
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (triggerEffectMinion.own != turnEndOfOwner) return;
			if (triggerEffectMinion.Hp <= 0 || triggerEffectMinion.silenced) return;

			int totalLegAtkBuff = 0;
			int totalLegHpBuff = 0;

			List<Minion> minions = triggerEffectMinion.own ? p.ownMinions : p.enemyMinions;
			foreach (Minion m in minions)
			{
				if (m.Hp <= 0 || m.silenced) continue;
				CardDB.cardIDEnum cid = m.handcard.card.cardIDenum;
				if (cid == Leg1 || cid == Leg2 || cid == Leg3 || cid == Leg4)
				{
					// Leg base stats: assume legs start at some base and gains accumulate
					totalLegAtkBuff += m.Angr;
					totalLegHpBuff += m.maxHp;
				}
			}

			// Apply leg stats to Wickerfang (as bonus, not cumulative base)
			if (totalLegAtkBuff > 0 || totalLegHpBuff > 0)
			{
				// Give Wickerfang stats equal to the highest leg stats
				int avgAtk = totalLegAtkBuff / Math.Max(1, 4);
				int avgHp = totalLegHpBuff / Math.Max(1, 4);
				if (avgAtk > triggerEffectMinion.Angr || avgHp > triggerEffectMinion.maxHp)
				{
					int atkBuff = Math.Max(0, avgAtk - triggerEffectMinion.Angr);
					int hpBuff = Math.Max(0, avgHp - triggerEffectMinion.maxHp);
					p.minionGetBuffed(triggerEffectMinion, atkBuff, hpBuff);
					p.evaluatePenality -= (atkBuff + hpBuff);
				}
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
