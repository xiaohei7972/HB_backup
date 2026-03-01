using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 爆炸射击 Explosive Shot
    //Deal $5 damage to a minion and $2 damage to adjacent ones.
    //对一个随从造成$5点伤害，并对其相邻的随从造成$2点伤害。
    class Sim_VAN_EX1_537 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                int dmg = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);
                int dmg1 = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
                List<Minion> temp = new List<Minion>((target.own) ? p.ownMinions : p.enemyMinions);
                p.minionGetDamageOrHeal(target, dmg);
                foreach (Minion m in temp)
                {
                    if (m.zonepos + 1 == target.zonepos || m.zonepos - 1 == target.zonepos)
                        m.getDamageOrHeal(dmg1, p, true, false); // isMinionAttack=true because it is extra damage (we calc clear lostDamage)
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