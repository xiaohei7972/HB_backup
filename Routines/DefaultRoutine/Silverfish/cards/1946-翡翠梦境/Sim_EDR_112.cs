using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：6
	//Dream Portal
	//梦境传送门
	//<b>Rewind</b>. Summon 3 random 3-Cost minions.
	//<b>回响</b>。召唤3个法力值消耗为（3）的随机随从。
	class Sim_EDR_112 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			// Rewind — re-casts spells played this turn (complex mechanic, approximated)
			// Summon 3 random 3-Cost minions
			for (int i = 0; i < 3; i++)
			{
				CardDB.Card kid = p.getRandomCardForManaMinion(3);
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(kid, pos, ownplay);
			}

			p.evaluatePenality -= 5;
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}

	}
}
