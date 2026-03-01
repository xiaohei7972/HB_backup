using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 殊死一搏 Desperate Stand
    //Give a minion "<b>Deathrattle:</b> Return this to life with 1 Health."
    //使一个随从获得“<b>亡语：</b>回到战场，并具有1点生命值。” 
    class Sim_ICC_244 : SimTemplate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                target.Deathrattle = true;
                target.desperatestand++;
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