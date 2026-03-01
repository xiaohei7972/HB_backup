using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //法术 圣骑士 费用：6
    //Power Spike
    //大力扣杀
    //Deal $4 damage.Give a random friendly minion +4/+4.
    //造成$4点伤害。随机使一个友方随从获得+4/+4。
    class Sim_VAC_915 : SimTemplate
    {

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                int dmg = ownplay ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
                p.minionGetDamageOrHeal(target, dmg); // 对目标造成4点伤害
                foreach (Minion m in ownplay ? p.ownMinions : p.enemyMinions)
                {
                    if (m.untouchable) continue;
                    p.minionGetBuffed(m, 4, 4);
                    break;
                }
                // buff不需要管
            }
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[]{
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
            };
        }
    }
}
