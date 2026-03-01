using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_881 : SimTemplate //* 动员布道 Invigorating Sermon
	{
		//Give +1/+1 to all minions in your hand, deck, and battlefield.
		//使你手牌，牌库以及战场中的所有随从获得+1/+1。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (ownplay)
			{
				// 场地
				foreach (Minion m in p.ownMinions)
				{
					p.minionGetBuffed(m, 1, 1);
				}
				// 手牌
				foreach (Handmanager.Handcard handcard in p.owncards)
				{
					handcard.addattack += 1;
					handcard.addHp += 1;
					p.anzOwnExtraAngrHp += 2;
				}
				p.evaluatePenality -= 5;
				// TODO 卡组
			}
		}

	}
}
