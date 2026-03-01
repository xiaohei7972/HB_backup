
using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 墓地之力 Grave Strength
    //使你的所有随从获得+1攻击力。消耗5具&lt;b&gt;尸体&lt;/b&gt;，改为获得+3攻击力。
    //Give your minions +1Attack. Spend 5 &lt;b&gt;Corpses&lt;/b&gt;to give them +3 instead.
    class Sim_RLK_707 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            int addattack = 1;
            if (p.getCorpseCount() >= 5)
            {
                p.corpseConsumption(5);
                addattack = 3;
            }
            p.allMinionOfASideGetBuffed(ownplay, addattack, 0);
        }
    }


}
