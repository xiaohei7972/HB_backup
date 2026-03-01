using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_SCH_604 : SimTemplate //* 数量压制 Overwhelm
    {
        //Deal $2 damage to a minion. Deal one more damage for each Beast you control.
        //对一个随从造成$2点伤害。你每控制一只野兽，便多造成一点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                int damage = ownplay ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
                foreach (Minion m in p.ownMinions)
                {
                    if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.PET)) damage++;
                }
                p.minionGetDamageOrHeal(target, 2);
            }
        }


        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
            };
        }
    }
}
