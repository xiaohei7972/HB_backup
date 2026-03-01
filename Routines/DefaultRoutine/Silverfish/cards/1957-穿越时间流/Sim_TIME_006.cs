using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 法师 费用：1
	//Mirror Dimension
	//镜像维度
	//Summon a 0/4 minion with <b>Taunt</b>. If you are holding a Dragon, summon another.
	//召唤一个0/4并具有<b>嘲讽</b>的随从。如果你的手牌中有龙牌，再召唤一个。
	class Sim_TIME_006 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_006t1);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			p.callKid(kid, p.getPosition(ownplay), ownplay);
			if (p.anyRaceCardInHand(CardDB.Race.DRAGON))
			{
				p.callKid(kid, p.getPosition(ownplay), ownplay);

			}
		}

	}
}
