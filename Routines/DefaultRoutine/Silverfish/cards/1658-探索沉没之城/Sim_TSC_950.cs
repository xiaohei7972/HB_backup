using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 猎人 费用：7 攻击力：5 生命值：5
	//Hydralodon
	//海卓拉顿
	//[x]<b>Colossal +2</b><b>Battlecry:</b> Give your_Hydralodon Heads <b>Rush</b>.
	//<b>巨型+2</b><b>战吼：</b>使你的海卓拉顿之头获得<b>突袭</b>。
	class Sim_TSC_950 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_950t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_950t2);

		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative, m.zonepos, m.own);
		}

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			List<Minion> minions = new List<Minion>(own.own ? p.ownMinions : p.enemyMinions);

			foreach (Minion minion in minions)
			{
				if (minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_950t || minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_950t2)
				{
					p.minionGetRush(minion);
				}
			}
		}

	}
}
