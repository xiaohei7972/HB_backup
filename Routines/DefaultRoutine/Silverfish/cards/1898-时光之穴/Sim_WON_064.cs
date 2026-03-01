using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：2
	//Shadow Word: Forbid
	//暗言术：禁
	//<b>Tradeable</b>Destroy a 4-Attack minion. <b>Corrupt:</b> Destroy ALL 4-Attack minions.
	//<b>可交易</b>消灭一个攻击力为4的随从。<b>腐蚀：</b>消灭所有攻击力为4的随从。
	class Sim_WON_064 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				p.minionGetDestroyed(target);
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE), //如果有目标
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),//目标只能是随从
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_EXACT_ATTACK,4),//目标只能是攻击力为4的
			};
		}

	}
}
