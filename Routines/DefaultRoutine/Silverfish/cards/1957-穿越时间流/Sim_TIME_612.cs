using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：2 攻击力：2 生命值：3
	//Frozen Champion
	//冰霜勇士
	//<b>Deathrattle:</b> Give a random friendly minion <b>Immune</b> this turn.
	//<b>亡语：</b>使一个随机友方随从在本回合中获得<b>免疫</b>。
	class Sim_TIME_612 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			List<Minion> targets = m.own ? p.ownMinions : p.enemyMinions;
			if (targets.Count > 0)
			{
				Minion target = p.searchRandomMinion(targets, searchmode.searchLowestHP);
				if (target != null)
				{
					target.immune = true;
				}
			}
		}
	}
}
