using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 自然平衡 Naturalize
    //Destroy a minion.Your opponent draws 2_cards.
    //消灭一个随从，你的对手抽两张牌。 
    class Sim_EX1_161 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                p.minionGetDestroyed(target);
                p.drawACard(CardDB.cardIDEnum.None, !ownplay);
                p.drawACard(CardDB.cardIDEnum.None, !ownplay);
            }
        }


        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
            };
        }
    }
}