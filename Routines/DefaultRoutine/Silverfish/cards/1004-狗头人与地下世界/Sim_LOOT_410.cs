using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 破晓之龙 Duskbreaker
    //<b>Battlecry:</b> If you're holdinga Dragon, deal 3 damage to all other minions.
    //<b>战吼：</b>如果你的手牌中有龙牌，则对所有其他随从造成3点伤害。
    class Sim_LOOT_410 : SimTemplate
    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.anyRaceCardInHand(CardDB.Race.DRAGON))
            {
                p.allMinionsGetDamage(3, own.entitiyID);
            }
        }

    }
}
