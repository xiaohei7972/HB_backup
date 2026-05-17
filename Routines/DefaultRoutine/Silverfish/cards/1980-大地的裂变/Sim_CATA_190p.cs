using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//英雄技能 中立 费用：2
	//Ruthless
	//无情
	//+$a5 Attack this turn.
	//在本回合中+$a5攻击力。
	class Sim_CATA_190p : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			// Give hero +5 Attack this turn
			if (ownplay)
			{
				p.minionGetTempBuff(p.ownHero, 5, 0);
				p.evaluatePenality -= 4;
			}
			else
			{
				p.minionGetTempBuff(p.enemyHero, 5, 0);
				p.evaluatePenality += 4;
			}
		}
	}
}
