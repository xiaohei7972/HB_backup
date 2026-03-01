using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 锐鳞骑士 Scalerider
    //<b>Battlecry:</b> If you're holding a Dragon, deal 2 damage.
    //<b>战吼：</b>如果你的手牌中有龙牌，则造成2点伤害。
    class Sim_DRG_081 : SimTemplate

    {
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (p.anyRaceCardInHand(CardDB.Race.DRAGON) && target != null) p.minionGetDamageOrHeal(target, 2);
        }


        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND),
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),
            };
        }
    }
}
