using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 牧师 费用：3 攻击力：2 生命值：1
	//Tough Crowd
	//挑剔的观众
	//<b>Outcast:</b> Return a minion to its owner's hand.
	//<b>流放：</b>将一个随从移回其拥有者的手牌。
	class Sim_JAM_020 : SimTemplate
	{
        public override void onCardPlay(Playfield p, Minion own, bool ownplay, Minion target, int choice, bool outcast)
        {
			if (target != null)
			{
				p.minionReturnToHand(target,target.own,0);
			}
        }
		public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),  // 如果有目标
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),   // 目标必须是随从
            };
        }
		
	}
}
