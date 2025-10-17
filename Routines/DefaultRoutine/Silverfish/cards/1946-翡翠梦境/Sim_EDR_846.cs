using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 中立 费用：8
	//Shaladrassil
	//莎拉达希尔
	//[x]Get all 5 Dream cards.If you've played a higherCost card while holdingthis, corrupt them!
	//获取全部5张梦境牌。如果你在此牌在你手中时使用过法力值消耗更高的牌，腐蚀这些梦境牌！
	class Sim_EDR_846 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (hc.poweredUp > 0)
			{
				p.drawACard(CardDB.cardIDEnum.EDR_846t1, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.EDR_846t2, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.EDR_846t3, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.EDR_846t4, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.EDR_846t5, ownplay, true);
			}
			else
			{
				p.drawACard(CardDB.cardIDEnum.DREAM_01, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.DREAM_02, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.DREAM_03, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.DREAM_04, ownplay, true);
				p.drawACard(CardDB.cardIDEnum.DREAM_05, ownplay, true);

			}

		}
	}
}
