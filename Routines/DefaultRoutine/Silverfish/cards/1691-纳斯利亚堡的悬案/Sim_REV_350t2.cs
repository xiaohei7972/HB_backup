using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：2
	//Frenzied Fangs
	//狂暴利齿
	//<b>Infused</b>Summon two 2/1 Bats. Give them +1/+2.
	//<b>已注能</b>召唤两只2/1的蝙蝠。使其获得+1/+2。
	class Sim_REV_350t2 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.REV_350t);

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
			p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);
			p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);

		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINION_CAP,1),
			};
		}

	}
}
