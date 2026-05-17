using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：9 攻击力：4 生命值：6
	//Endtime Murozond
	//末世的姆诺兹多
	//<b>Battlecry:</b> Fill your board with random Dragons.Fully heal your hero. Skip your next turn.
	//<b>战吼：</b>用随机的龙填满你的面板。为你的英雄恢复所有生命值。跳过你的下个回合。
	class Sim_END_037 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int pos = own.zonepos;
			int boardSize = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
			for (int i = boardSize; i < 7; i++)
			{
				p.callKid(p.getRandomCardForManaMinion(9), pos, own.own);
			}

			Minion hero = own.own ? p.ownHero : p.enemyHero;
			int healAmount = hero.maxHp - hero.Hp;
			if (healAmount > 0)
			{
				int heal = (own.own) ? p.getMinionHeal(healAmount) : p.getEnemyMinionHeal(healAmount);
				if (heal > 0) p.minionGetDamageOrHeal(hero, -heal);
			}
		}
	}
}
