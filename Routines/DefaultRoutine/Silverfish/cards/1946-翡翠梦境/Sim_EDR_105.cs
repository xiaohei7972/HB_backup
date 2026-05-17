using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：1 攻击力：1 生命值：1
	//Enchanted Acorn
	//附魔橡果
	//<b>Deathrattle:</b> Summon a 2/2 Treant with <b>Taunt</b>.
	//<b>亡语：</b>召唤一个2/2并具有<b>嘲讽</b>的树人。
	class Sim_EDR_105 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk9); // 2/2 Treant

		public override void onDeathrattle(Playfield p, Minion m)
		{
			int pos = m.own ? p.ownMinions.Count : p.enemyMinions.Count;
			Minion summoned = p.callKidAndReturn(kid, pos, m.own);
			if (summoned != null)
			{
				summoned.taunt = true;
			}
		}

	}
}
