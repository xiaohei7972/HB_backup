using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：6 攻击力：4 生命值：6
	//Deathwarden
	//死亡守望者
	//<b>Taunt</b>. <b>Deathrattle:</b> Destroy a random enemy minion.
	//<b>嘲讽</b>。<b>亡语：</b>随机消灭一个敌方随从。
	class Sim_TIME_603 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			List<Minion> targets = m.own ? p.enemyMinions : p.ownMinions;
			if (targets.Count > 0)
			{
				Minion target = p.searchRandomMinion(targets, searchmode.searchLowestHP);
				if (target != null)
				{
					p.minionGetDestroyed(target);
				}
			}
		}
	}
}
