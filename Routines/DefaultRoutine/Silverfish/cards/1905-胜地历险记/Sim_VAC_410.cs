using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //法术 猎人 费用：6
    //Furious Fowls
    //猛禽狂怒
    //Choose an enemy. Summon two 3/2 Birds with <b>Immune</b> while attacking to attack it.
    //选择一个敌人。召唤两只3/2并具有攻击时<b>免疫</b>的小鸟，攻击选中的敌人。
    class Sim_VAC_410 : SimTemplate
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_410t);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {
                List<Minion> minions = new List<Minion>();
                // 召唤两只3/2的小鸟
                for (int i = 0; i < 2; i++)
                {
                    int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
                    Minion summoned = p.callKidAndReturn(kid, pos, ownplay, false);
                    if (summoned != null)
                    {
                        minions.Add(summoned);
                    }
                }
                foreach (Minion m in minions)
                {
                    // 让小鸟立即攻击选中的敌人
                    p.minionAttacksMinion(m, target);
                }
            }
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要一个目标
                new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET), // 目标只能是敌人
            };
        }
    }
}

