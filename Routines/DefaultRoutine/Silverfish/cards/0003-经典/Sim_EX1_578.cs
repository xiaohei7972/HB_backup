using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_EX1_578 : SimTemplate //* 野蛮之击 Savagery
    {
        //Deal damage equal to your hero's Attack to a minion.
        //对一个随从造成等同于你的英雄攻击力的伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                int dmg = (ownplay) ? p.getSpellDamageDamage(p.ownHero.Angr) : p.getEnemySpellDamageDamage(p.enemyHero.Angr);
                p.minionGetDamageOrHeal(target, dmg);
            }
        }


        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK),//

            };
        }
    }
}