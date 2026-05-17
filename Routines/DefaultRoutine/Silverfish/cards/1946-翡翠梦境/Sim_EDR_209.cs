using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：10 攻击力：5 生命值：8
	//Forest Lord Cenarius
	//森林之王塞纳留斯
	//[x]<b>Choose Thrice -</b> Give yourother minions +1/+3; orSummon a 5/5 Ancientwith <b>Taunt</b>.
	//<b>选择三次：</b>使你的其他随从获得+1/+3；或者召唤一棵5/5并具有<b>嘲讽</b>的古树。
	class Sim_EDR_209 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EDR_209t5); // 5/5 Ancient with Taunt

		// Choose Thrice: The player selects from two options three times.
		// This simulation approximates by applying the buff effect once and summoning one 5/5.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Buff other friendly minions +1/+3
			foreach (Minion m in p.ownMinions)
			{
				if (m.entityID != own.entityID)
				{
					p.minionGetBuffed(m, 1, 3);
				}
			}
			// Summon a 5/5 Ancient with Taunt
			int pos = p.ownMinions.Count;
			p.callKid(kid, pos, own.own);
		}
		
	}
}
