using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：4 攻击力：3 生命值：3
	//Mischievous Imp
	//调皮的小鬼
	//[x]<b>InfusedBattlecry:</b> Summon two copies of this.
	//<b>已注能战吼：</b>召唤本随从的两个复制。
	class Sim_REV_244t : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			Minion sunmmoned = p.callKidAndReturn(own.handcard.card, own.zonepos, own.own);
			Minion sunmmoned1 = p.callKidAndReturn(own.handcard.card, own.zonepos - 1, own.own);
			if (sunmmoned != null)
			{
				sunmmoned.setMinionToMinion(own);
			}
			if (sunmmoned1 != null)
			{
				sunmmoned1.setMinionToMinion(own);
			}
		}

	}
}
