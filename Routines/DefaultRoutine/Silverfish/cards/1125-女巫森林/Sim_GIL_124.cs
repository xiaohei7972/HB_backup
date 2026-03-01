using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 苔藓恐魔 Mossy Horror
    //<b>Battlecry:</b> Destroy all other_minions with 2_or_less_Attack.
    //<b>战吼：</b>消灭所有其他攻击力小于或等于2的随从。 
    class Sim_GIL_124 : SimTemplate
    {


        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            foreach (Minion m in p.ownMinions.ToArray())
            {
                if (m.untouchable || m.entitiyID == own.entitiyID) continue;
                if (m.Angr <= 2)
                {
                    p.minionGetDestroyed(m);
                }
            }
            foreach (Minion m in p.enemyMinions.ToArray())
            {
                if (m.untouchable || m.entitiyID == own.entitiyID) continue;
                if (m.Angr <= 2)
                {
                    p.minionGetDestroyed(m);
                }
            }
        }
    }
}