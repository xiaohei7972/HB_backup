using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：3
	//Ready the Fleet
	//整备团队
	//Give +1/+2 to a friendly minion and your other minions that share a type with it.
	//使一个友方随从和具有相同类型的其他友方随从获得+1/+2。
	class Sim_TLC_441 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				List<CardDB.Race> races = target.handcard.card.GetRaces();
				p.minionGetBuffed(target, 1, 2);
				foreach (Minion m in ownplay ? p.ownMinions : p.enemyMinions)
				{
					if (m.entitiyID == target.entitiyID) continue;
					if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), races))
						p.minionGetBuffed(m, 1, 2);
				}
			}
		}


		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}

	}
}
