using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 偷袭 Cheap Shot
    //<b>Echo</b> Deal $2 damage to a_minion.
    //<b>回响</b>对一个随从造成$2点伤害。 
    class Sim_GIL_506 : SimTemplate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.minionGetDamageOrHeal(target, dmg);
            if (ownplay) p.ueberladung++;
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