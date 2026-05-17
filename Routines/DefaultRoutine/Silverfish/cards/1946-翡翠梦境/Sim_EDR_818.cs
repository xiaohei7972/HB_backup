using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：7 攻击力：7 生命值：7
	//Nythendra
	//尼珊德拉
	//[x]<b>Taunt</b>. <b>Deathrattle:</b> Splitinto 1/1 Beetles. At thestart of your turn, reformwith any remaining.
	//<b>嘲讽</b>。<b>亡语：</b>分裂为1/1的甲虫。在你的回合开始时，剩余的甲虫会重组为尼珊德拉。
	class Sim_EDR_818 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EDR_818t); // 1/1 Nythendric Beetle

		// Deathrattle: Split into 7 x 1/1 Beetles (7 HP = 7 beetles).
		public override void onDeathrattle(Playfield p, Minion m)
		{
			int pos = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;

			// Summon 7 Beetles (matching original HP)
			for (int i = 0; i < 7; i++)
			{
				p.callKid(kid, pos, m.own);
			}
		}
		
	}
}
