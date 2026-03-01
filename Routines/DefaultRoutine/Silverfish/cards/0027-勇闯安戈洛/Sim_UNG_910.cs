using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 凶残撕咬 Grievous Bite
    //Deal $2 damage to a minion and $1 damage to adjacent ones.
    //对一个随从造成$2点伤害，并对其相邻的随从造成$1点伤害。 
    class Sim_UNG_910 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                int dmgMain = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
                int dmgAdj = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
                List<Minion> temp = new List<Minion>((target.own) ? p.ownMinions : p.enemyMinions);
                p.minionGetDamageOrHeal(target, dmgMain);
                foreach (Minion m in temp)
                {
                    if (m.zonepos + 1 == target.zonepos || m.zonepos - 1 == target.zonepos) p.minionGetDamageOrHeal(m, dmgAdj);
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