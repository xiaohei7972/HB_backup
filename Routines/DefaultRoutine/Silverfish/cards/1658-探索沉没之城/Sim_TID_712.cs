using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：10 攻击力：7 生命值：7
	//Neptulon the Tidehunter
	//猎潮者耐普图隆
	//[x]<b>Colossal +2</b>, <b>Rush</b>, <b>Windfury</b>Whenever Neptulon attacks,if you control any Hands,they attack instead.
	//<b>巨型+2</b><b>突袭</b>，<b>风怒</b>，每当耐普图隆攻击时，如果你控制着任意耐普图隆之手，改为由手攻击。
	class Sim_TID_712 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_712t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_712t2);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative1, m.zonepos, m.own);
		}
		public override void onMinionAttack(Playfield p, Minion attacker, Minion target, ref bool terminatedAttack)
		{
			List<Minion> minions = new List<Minion>(attacker.own ? p.ownMinions : p.enemyMinions);
			foreach (Minion minion in minions)
			{
				if (minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TID_712t || minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TID_712t2)
				{
					terminatedAttack = true;
					break;
				}
			}

			foreach (Minion minion in minions)
			{
				if (minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TID_712t || minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TID_712t2)
				{
					p.minionAttacksMinion(minion, target, true);
				}
			}
		}

	}
}
