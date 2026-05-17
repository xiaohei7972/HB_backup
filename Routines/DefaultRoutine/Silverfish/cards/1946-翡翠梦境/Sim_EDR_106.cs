using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：8
	//Ysera's Dream
	//伊瑟拉的梦境
	//Fill your board with 3/3 Emerald Dragons. Your other cards cost (1) less this turn.
	//用3/3的翡翠龙填满你的面板。你的其他牌在本回合中法力值消耗减少（1）点。
	class Sim_EDR_106 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			// Fill board with 3/3 Emerald Dragons (approximated with random 3-Cost minions)
			int slots = ownplay ? (7 - p.ownMinions.Count) : (7 - p.enemyMinions.Count);
			for (int i = 0; i < slots; i++)
			{
				CardDB.Card kid = p.getRandomCardForManaMinion(3);
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(kid, pos, ownplay);
			}

			// Your other cards cost (1) less this turn
			if (ownplay)
			{
				foreach (Handmanager.Handcard handCard in p.owncards)
				{
					handCard.manacost = Math.Max(0, handCard.manacost - 1);
				}
			}

			p.evaluatePenality -= 8;
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}

	}
}
