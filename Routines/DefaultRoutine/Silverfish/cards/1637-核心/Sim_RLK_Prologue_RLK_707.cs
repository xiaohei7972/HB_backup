using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：4
	//Grave Strength
	//墓地之力
	//[x]Give your minions +1Attack. Spend 5 <b>Corpses</b>to give them +3 instead.
	//使你的所有随从获得+1攻击力。消耗5份<b>残骸</b>，改为获得+3攻击力。
	class Sim_RLK_Prologue_RLK_707 : SimTemplate
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
