using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 军需官 Quartermaster
    //<b>Battlecry:</b> Give your Silver Hand Recruits +2/+2.
    //<b>战吼：</b>使你的白银之手新兵获得+2/+2。 
    class Sim_GVG_060 : SimTemplate

    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                if (m.handcard.card.SilverHandRecruit) p.minionGetBuffed(m, 2, 2);
            }
        }

    }

}