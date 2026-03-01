using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：3 攻击力：2 生命值：5
	//Ravenous Kraken
	//饥饿的海怪
	//<b>Battlecry:</b> Destroya friendly minion. <b>Deathrattle:</b> Summon a copy of it.
	//<b>战吼：</b>消灭一个友方随从。<b>亡语：</b>召唤一个被消灭随从的复制。
	class Sim_TTN_754 : SimTemplate
	{
		CardDB.Card kid = null;
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				kid = target.handcard.card;
				p.minionGetDestroyed(target);
				
			}
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			if(kid != null)
            {
				p.callKid(kid, m.zonepos - 1, m.own);
            }
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
        }
		
	}
}
