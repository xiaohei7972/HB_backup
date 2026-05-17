using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_AV_100 : SimTemplate //* 德雷克塔尔 drekthar
	{
		//<b>战吼：</b>如果该随从的法力值消耗大于你牌库中的所有随从牌，则召唤其中的2个。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            // 遍历卡组
            bool check = true;
            foreach (KeyValuePair<CardDB.cardIDEnum, int> kvp in p.prozis.turnDeck)
            {
                // ID 转卡
                CardDB.cardIDEnum deckCard = kvp.Key;
                CardDB.Card deckSpell = CardDB.Instance.getCardDataFromID(deckCard);
                if (deckSpell.type == CardDB.cardtype.MOB && deckSpell.cost >= own.handcard.card.cost)
                {
                    check = false;
                }
            }

            if (check)
            {
                // 召唤牌库中2个法力值消耗小于该随从的随从
                int summoned = 0;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> kvp in p.prozis.turnDeck)
                {
                    if (summoned >= 2) break;
                    CardDB.Card deckMinion = CardDB.Instance.getCardDataFromID(kvp.Key);
                    if (deckMinion.type == CardDB.cardtype.MOB)
                    {
                        List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
                        int pos = temp.Count;
                        p.callKid(deckMinion, pos, own.own);
                        summoned++;
                    }
                }
            }
        }
		
	}
}
