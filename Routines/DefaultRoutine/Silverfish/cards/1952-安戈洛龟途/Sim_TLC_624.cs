using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：6 攻击力：5 生命值：7
	//Nablya, the Watcher
	//观察者娜博亚
	//[x]<b>Battlecry:</b> Summon copies of your damaged minions.Give the copies <b>Rush</b>.
	//<b>战吼：</b>召唤你受伤的随从的复制，使复制获得<b>突袭</b>。
	class Sim_TLC_624 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			foreach (Minion m in own.own ? p.ownMinions.ToArray() : p.enemyMinions.ToArray())
			{
				if (m.wounded)
				{
					Minion summoned = p.callKidAndReturn(m.handcard.card, m.zonepos, own.own);
					if (summoned != null)
					{
						summoned.setMinionToMinion(m);
						p.minionGetRush(summoned);
					}
				}
			}


		}

	}
}
