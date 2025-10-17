using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 暗影收割者安度因 Shadowreaper Anduin
    //<b>Battlecry:</b> Destroy all minions with 5 or more_Attack.
    //<b>战吼：</b>消灭所有攻击力大于或等于5的随从。 

    class Sim_ICC_830 : SimTemplate
    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            foreach (Minion m in p.ownMinions.ToArray())
            {
                if (m.untouchable) continue;
                if (m.Angr >= 5)
                {
                    p.minionGetDestroyed(m);
                }
            }
            foreach (Minion m in p.enemyMinions.ToArray())
            {
                if (m.untouchable) continue;
                if (m.Angr >= 5)
                {
                    p.minionGetDestroyed(m);
                }
            }

        }

    }
}