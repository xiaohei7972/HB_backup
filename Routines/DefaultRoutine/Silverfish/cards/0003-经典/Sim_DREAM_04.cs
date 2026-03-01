using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 梦境 Dream
    //Return an enemy minion to your opponent's hand.
    //将一个随从移回其拥有者的手牌。
    class Sim_DREAM_04 : SimTemplate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
                p.minionReturnToHand(target, target.own, 0);
        }



        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
                new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
            };
        }
    }
}