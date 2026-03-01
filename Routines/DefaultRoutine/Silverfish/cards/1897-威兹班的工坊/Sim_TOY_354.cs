using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：4
	//R.C. Rampage
	//遥控狂潮
	//Summon six 1/1 Hounds. Any that can't fit give the others +1/+1.
	//召唤六只1/1的猎犬。每有一只放不下的猎犬，使其余猎犬获得+1/+1。
	class Sim_TOY_354 : SimTemplate
	{
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_538t);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            int houndsToSummon = 6;  // 需要召唤的猎犬数量
            int availableSpace = ownplay ? 7 - p.ownMinions.Count : 7 - p.enemyMinions.Count;  // 检查场上空位
            int houndsSummoned = Math.Min(houndsToSummon, availableSpace);  // 实际能够召唤的猎犬数量
            int buffAmount = houndsToSummon - houndsSummoned;  // 每有一只放不下的猎犬，增加的buff数量
            List<Minion> minions = new List<Minion>();
            // 召唤猎犬
            for (int i = 0; i < houndsSummoned; i++)
            {
                minions.Add(p.callKidAndReturn(kid, ownplay ? p.ownMinions.Count : p.enemyMinions.Count, ownplay));
            }

            // 给场上的猎犬增加+1/+1
            if (buffAmount > 0)
            {
                foreach (Minion m in minions)
                {
                    p.minionGetBuffed(m, buffAmount, buffAmount);
                }
            }
        }
    }
}
