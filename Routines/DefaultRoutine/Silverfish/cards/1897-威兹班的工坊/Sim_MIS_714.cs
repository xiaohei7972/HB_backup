using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //法术 潜行者 费用：3
    //Funhouse Mirror
    //哈哈镜
    //Summon a copyof an enemy minion.It attacks the original.
    //召唤一个敌方随从的一个复制并使其攻击本体。
    class Sim_MIS_714 : SimTemplate
    {

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                // 召唤一个敌方随从的复制
                int position = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
                Minion summoned = p.callKidAndReturn(target.handcard.card, position, ownplay);
                if (summoned != null)
                {
                    p.minionAttacksMinion(summoned, target, true);
                }

            }
        }
    }
}
