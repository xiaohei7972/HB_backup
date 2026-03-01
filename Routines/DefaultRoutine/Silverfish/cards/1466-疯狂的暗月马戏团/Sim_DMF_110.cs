using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DMF_110 : SimTemplate //* 吐火艺人 Fire Breather
	{
		//<b>Battlecry:</b> Deal 2 damage to all minions except Demons.
		//<b>战吼：</b>对所有非恶魔随从造成2点伤害。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			foreach (Minion m in p.ownMinions.ToArray())
			{
				if (!RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.DEMON))
					p.minionGetDamageOrHeal(m, 2);
			}
			foreach (Minion m in p.enemyMinions.ToArray())
			{
				if (!RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.DEMON))
					p.minionGetDamageOrHeal(m, 2);
			}
		}

	}
}
