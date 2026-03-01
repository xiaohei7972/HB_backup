using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 南瓜宝宝 Squashling
    //[x]<b>Echo</b><b>Battlecry:</b> Restore #2 Health.
    //<b>回响，战吼：</b>恢复#2点生命值。 
    class Sim_GIL_835 : SimTemplate
    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {

                int heal = (own.own) ? p.getMinionHeal(2) : p.getEnemyMinionHeal(2);
                p.minionGetDamageOrHeal(target, -heal);
            }
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
            };
        }
    }
}