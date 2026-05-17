using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：2
	//Forest's Gift
	//森林馈赠
	//Give a friendly minion +1/+1 for each minion you control.
	//每控制一个随从，使一个友方随从获得+1/+1。
	class Sim_CATA_138 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target == null) return;

			int buff = p.ownMinions.Count;
			if (buff > 0)
			{
				p.minionGetBuffed(target, buff, buff);
				p.evaluatePenality -= buff * 2;
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}
