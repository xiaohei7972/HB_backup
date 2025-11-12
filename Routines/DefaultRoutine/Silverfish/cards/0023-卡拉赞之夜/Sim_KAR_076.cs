using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 火焰之地传送门 Firelands Portal
    //Deal $6 damage. Summon a random6-Cost minion.
    //造成$6点伤害。随机召唤一个法力值消耗为（6）的随从。 
    class Sim_KAR_076 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            int dmg = (ownplay) ? p.getSpellDamageDamage(6) : p.getEnemySpellDamageDamage(6);
            p.minionGetDamageOrHeal(target, dmg);
            p.callKid(p.getRandomCardForManaMinion(6), pos, ownplay);
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要目标
                new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET), // 需要敌方目标
            };
        }
    }
}