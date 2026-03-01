using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 搜寻猎物 On the Hunt
    //Deal $1 damage.Summon a 1/1 Mastiff.
    //造成$1点伤害。召唤一个1/1的獒犬。 
    class Sim_OG_061 : SimTemplate
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.OG_061t);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
                int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
                p.minionGetDamageOrHeal(target, dmg);
                p.callKid(kid, pos, ownplay);
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