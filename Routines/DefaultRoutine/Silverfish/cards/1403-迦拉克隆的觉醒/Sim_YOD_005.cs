using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 新鲜气息 Fresh Scent
    //<b>Twinspell</b>Give a Beast +2/+2.
    //<b>双生法术</b>使一个野兽获得+2/+2。
    class Sim_YOD_005 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 2, 2);
            }
        }
        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_WITH_RACE, CardDB.Race.PET),
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
            };
        }
    }
}
