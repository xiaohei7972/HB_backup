using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：5
	//Ward of Earth
	//大地庇护
	//Gain 5 Armor. Summon a random 5-Cost minion and give it <b>Taunt</b>.
	//获得5点护甲值。随机召唤一个法力值消耗为（5）的随从并使其获得<b>嘲讽</b>。
	class Sim_EDR_060 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 5);
			int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
			Minion summonMinion = p.callKidAndReturn(p.getRandomCardForManaMinion(5), pos, ownplay);
			if (summonMinion != null)
			{
				summonMinion.taunt = true;
				if (summonMinion.own)
				{
					p.anzOwnTaunt++;
				}
				else
				{
					p.anzEnemyTaunt++;
				}
			}
		}

	}
}
