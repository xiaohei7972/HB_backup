using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：5 攻击力：2 生命值：6
	//Heir of Hereafter
	//后世之嗣
	//[x]<b>Taunt</b><b>Battlecry:</b> Gain +2/+2 foreach damaged minion.
	//<b>嘲讽</b>。<b>战吼：</b>每有一个受伤的随从，便获得+2/+2。
	class Sim_TIME_871 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			foreach (Minion m in p.enemyMinions)
			{
				if (m.untouchable) continue;
				if (m.wounded)
					p.minionGetBuffed(own, 2, 2);

			}
			foreach (Minion m in p.ownMinions)
			{
				if (m.untouchable) continue;
				if (m.wounded)
					p.minionGetBuffed(own, 2, 2);

			}
		}

	}
}
