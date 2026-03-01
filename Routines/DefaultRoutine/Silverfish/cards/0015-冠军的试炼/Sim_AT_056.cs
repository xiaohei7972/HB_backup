using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 强风射击 Powershot
    //Deal $2 damage to a minion and the minions next to it.
    //对一个随从及其相邻的随从造成$2点伤害。 
    class Sim_AT_056 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
                p.minionGetDamageOrHeal(target, dmg);
                List<Minion> temp = new List<Minion>((target.own) ? p.enemyMinions : p.ownMinions);
                foreach (Minion m in temp)
                {
                    if (target.zonepos == m.zonepos + 1 || target.zonepos + 1 == m.zonepos)
                    {
                        p.minionGetDamageOrHeal(m, dmg);
                    }
                }
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