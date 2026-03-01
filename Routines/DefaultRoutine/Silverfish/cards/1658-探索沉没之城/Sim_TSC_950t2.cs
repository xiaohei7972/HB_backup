using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HREngine.Bots
{
	//随从 猎人 费用：2 攻击力：3 生命值：1
	//Hydralodon Head
	//海卓拉顿之头
	//<b>Deathrattle:</b> If you control Hydralodon, summon 2 Hydralodon Heads.
	//<b>亡语：</b>如果你控制着海卓拉顿，召唤两个海卓拉顿之头。
	class Sim_TSC_950t2 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_950t2);
		public override void onDeathrattle(Playfield p, Minion m)
		{
			List<Minion> minions = new List<Minion>(m.own ? p.ownMinions : p.enemyMinions);
			if (minions.Any((minion) => minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_950))
			{
				p.callKid(kid, m.zonepos - 1, m.own);
				p.callKid(kid, m.zonepos, m.own);
			}

		}

	}
}
