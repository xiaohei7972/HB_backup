using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 敲响警钟 Sound the Bells!
    //<b>Echo</b>Give a minion +1/+2.
    //<b>回响</b>使一个随从获得+1/+2。 
    class Sim_GIL_145 : SimTemplate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 1, 2);
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
