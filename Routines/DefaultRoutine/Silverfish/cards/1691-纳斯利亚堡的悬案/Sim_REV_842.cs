using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：1
	//Promotion
	//晋升
	//[x]Give a Silver HandRecruit +3/+3and <b>Taunt</b>.
	//使一个白银之手新兵获得+3/+3和<b>嘲讽</b>。
	class Sim_REV_842 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				p.minionGetBuffed(target, 3, 3);
				if (!target.taunt)
				{
					target.taunt = true;
					if (target.own) p.anzOwnTaunt++;
					else p.anzEnemyTaunt++;
				}
			}
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_SILVER_HAND_RECRUIT),
			};
        }
		
	}
}
