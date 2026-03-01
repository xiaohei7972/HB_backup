using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 梦境 费用：0
	//Corrupted Nightmare
	//已腐蚀的梦魇
	//Give a minion <b>Immune</b> this turn and +5/+5.
	//使一个随从获得+5/+5和在本回合中的<b>免疫</b>。
	class Sim_EDR_846t1 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				p.minionGetBuffed(target, 5, 5);
				target.immune = true;
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
