using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 生锈的号角 Rusty Horn
    //Give a minion <b>Taunt</b>.
    //使一个随从获得<b>嘲讽</b>。 
    class Sim_PART_003 : SimTemplate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
                p.minionGetTaunt(target);
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