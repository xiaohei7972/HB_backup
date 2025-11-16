using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：2 攻击力：1 生命值：3
	//Soldier's Caravan
	//士兵车队
	//[x]At the start of your turn,summon two 1/1Silver Hand Recruits.
	//在你的回合开始时，召唤两个1/1的白银之手新兵。
	class Sim_RLK_Prologue_BAR_871 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);
		
		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
		{
			if (triggerEffectMinion.own ==turnStartOfOwner)
			{
            	p.callKid(kid, triggerEffectMinion.zonepos -1, triggerEffectMinion.own);
            	p.callKid(kid, triggerEffectMinion.zonepos, triggerEffectMinion.own);
			}
		}
		
	}
}
