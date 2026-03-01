using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 隐秘力场 Finicky Cloakfield
    //Give a friendly minion <b>Stealth</b> until your next turn.
    //直到你的下个回合，使一个友方随从获得<b>潜行</b>。 
    class Sim_PART_004 : SimTemplate
    {




        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                target.stealth = true;
                target.conceal = true;
            }
        }



        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
            };
        }
    }

}