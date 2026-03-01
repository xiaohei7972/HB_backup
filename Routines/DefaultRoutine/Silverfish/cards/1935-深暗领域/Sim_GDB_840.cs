using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 猎人 费用：2 攻击力：0 生命值：2
	//Extraterrestrial Egg
	//异星虫卵
	//[x]<b>Deathrattle:</b> Summon a3/5 Beast that attacks thelowest Health enemy.
	//<b>亡语：</b>召唤一只3/5的野兽并使其攻击生命值最低的敌人。
	class Sim_GDB_840 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GDB_840t);
		public override void onDeathrattle(Playfield p, Minion m)
		{
			Minion summoned = p.callKidAndReturn(kid, m.zonepos - 1, m.own);
			List<Minion> minions = new List<Minion>(m.own ? p.enemyMinions : p.ownMinions);
			if(m.own) minions.Add(p.enemyHero);
			else minions.Add(p.ownHero);
            if (summoned != null)
            {
                Minion lowestHpMinion = p.searchRandomMinion(minions, searchmode.searchLowestHP);
				p.minionAttacksMinion(summoned,lowestHpMinion);
            }
		}
		
	}
}
