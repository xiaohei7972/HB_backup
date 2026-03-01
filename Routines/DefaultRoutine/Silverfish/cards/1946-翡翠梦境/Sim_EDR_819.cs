using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：9 攻击力：6 生命值：14
	//Ursoc
	//乌索克
	//<b>Battlecry:</b> Attack ALLother minions.<b>Deathrattle:</b> Resurrect any this killed.
	//<b>战吼：</b>攻击所有其他随从。<b>亡语：</b>复活本随从消灭的随从。
	class Sim_EDR_819 : SimTemplate
	{
		List<CardDB.Card> kids = new List<CardDB.Card>();
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			List<Minion> minions = new List<Minion>();
			minions.AddRange(p.enemyMinions);
			minions.AddRange(p.ownMinions);

			foreach (Minion m in minions.ToArray())
			{
				if (m.untouchable) continue;
				p.minionAttacksMinion(own, m);
			}
		}
		public override void afterMinionAttack(Playfield p, Minion attacker, Minion defender, bool dontcount)
		{
			if (defender.Hp <= 0)
			{
				kids.Add(CardDB.Instance.getCardDataFromID(defender.handcard.card.cardIDenum));
			}

		}
		public override void onDeathrattle(Playfield p, Minion m)
		{
			foreach (CardDB.Card kid in kids)
			{
				p.callKid(kid, m.zonepos - 1, m.own);
			}

		}
	}
}
