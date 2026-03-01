using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：8 攻击力：6 生命值：5
	//Ozumat
	//厄祖玛特
	//[x]<b>Colossal +6</b> <b>Deathrattle:</b> For each ofOzumat's Tentacles, destroy   a random enemy minion.
	//<b>巨型+6</b><b>亡语：</b>每有一条厄祖玛特的触须，随机消灭一个敌方随从。
	class Sim_TID_711 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t2);
		CardDB.Card ColossalDerivative2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t3);
		CardDB.Card ColossalDerivative3 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t4);
		CardDB.Card ColossalDerivative4 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t5);
		CardDB.Card ColossalDerivative5 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TID_711t6);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative1, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative2, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative3, m.zonepos, m.own);
			p.callKid(ColossalDerivative4, m.zonepos, m.own);
			p.callKid(ColossalDerivative5, m.zonepos, m.own);
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			List<Minion> minions = new List<Minion>(m.own ? p.ownMinions : p.enemyMinions);
			List<Minion> enemyMinions = new List<Minion>(m.own ? p.enemyMinions : p.ownMinions);
			int destroyedCount = 0;
			foreach (Minion minion in minions)
			{
				var cardId = minion.handcard.card.cardIDenum;
				if (cardId == CardDB.cardIDEnum.TID_711t || cardId == CardDB.cardIDEnum.TID_711t2 || cardId == CardDB.cardIDEnum.TID_711t3 ||
				cardId == CardDB.cardIDEnum.TID_711t4 || cardId == CardDB.cardIDEnum.TID_711t5 || cardId == CardDB.cardIDEnum.TID_711t6)
				{
					destroyedCount++;
				}
			}
			foreach (Minion minion in enemyMinions)
			{
				if (minion.untouchable || minion.handcard.card.type == CardDB.cardtype.LOCATION)
				{
					continue;
				}
				if (destroyedCount == 0)
				{
					break;
				}
				p.minionGetDestroyed(minion);
				destroyedCount--;
			}


		}

	}
}
