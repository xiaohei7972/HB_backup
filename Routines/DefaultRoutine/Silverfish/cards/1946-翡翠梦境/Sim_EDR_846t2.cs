using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 梦境 费用：0
	//Corrupted Dream
	//已腐蚀的梦境
	//Shuffle a minion into its owner's deck.
	//将一个随从洗入其拥有者的牌库。
	class Sim_EDR_846t2 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				p.minionReturnToDeck(target, target.own);
			}
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
        }
		
	}
}
