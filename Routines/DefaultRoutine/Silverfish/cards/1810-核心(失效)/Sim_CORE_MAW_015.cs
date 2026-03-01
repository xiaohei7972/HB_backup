using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：3
	//Jury Duty
	//陪审义务
	//[x]Summon twoSilver Hand Recruits.Give your Silver HandRecruits +1/+1.
	//召唤两个白银之手新兵。使你的白银之手新兵获得+1/+1。
	class Sim_CORE_MAW_015 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			for (int i = 0; i < 2; i++)
			{
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(kid, pos, ownplay);
			}
			foreach (Minion m in ownplay ? p.ownMinions : p.enemyMinions)
			{
				if (m.handcard.card.SilverHandRecruit)
				{
					p.minionGetBuffed(m, 1, 1);
				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS,1),
			};
		}

	}
}
