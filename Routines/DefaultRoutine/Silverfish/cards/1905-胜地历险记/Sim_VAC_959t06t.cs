using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //法术 中立 费用：1
    //Amulet of Critters
    //生灵护符
    //Summon arandom 4-Cost minionand give it <b>Taunt</b>.
    //随机召唤一个法力值消耗为（4）的随从并使其获得<b>嘲讽</b>。
    class Sim_VAC_959t06t : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            // 随机召唤一个法力值消耗为4的随从
            CardDB.Card kid = p.getRandomCardForManaMinion(4);
            int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
            Minion summonedMinion = p.callKidAndReturn(kid, pos, ownplay);
            if (summonedMinion != null)
            {
                summonedMinion.taunt = true;
                if (summonedMinion.own)
                {
                    p.anzOwnTaunt++;
                }
                else
                {
                    p.anzEnemyTaunt++;
                }
            }
        }

    }
}
