using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 恶魔猎手 费用：7 攻击力：3 生命值：6
	//Xhilag of the Abyss
	//渊狱魔犬希拉格
	//[x]<b>Colossal +4</b>At the start of your turn,increase the damage ofXhilag's Stalks by 1.
	//<b>巨型+4</b>在你的回合开始时，希拉格的蔓足造成的伤害提高1点。
	class Sim_TSC_219 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_219t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_219t2);
		CardDB.Card ColossalDerivative2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_219t3);
		CardDB.Card ColossalDerivative3 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_219t4);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative1, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative2, m.zonepos, m.own);
			p.callKid(ColossalDerivative3, m.zonepos, m.own);
		}

		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{
			if (triggerEffectMinion.own == turnEndOfOwner)
			{
				List<Minion> minions = new List<Minion>(triggerEffectMinion.own ? p.ownMinions : p.enemyMinions);
				foreach (Minion m in minions)
				{
					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_219t || m.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_219t2 || m.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_219t || m.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_219t4)
						m.TAG_SCRIPT_DATA_NUM_1++;
				}
			}

		}

	}
}
