using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //地标 战士 费用：4
    //Crimson Expanse
    //赤红岩床
    //[x]Choose a damagedminion. Summon a copyof it that goes <b>Dormant</b>for one turn.
    //选择一个受伤的随从，召唤一个它的<b>休眠</b>一回合的复制。
    class Sim_DEEP_019 : SimTemplate
    {
        public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
        {
            // 检查目标是否为有效的受伤随从
            if (triggerMinion.own ? p.ownMinions.Count < 7 : p.enemyMinions.Count < 7)
                if (target != null && target.wounded)
                {
                    // 复制随从的卡牌信息
                    // 召唤一个随从作为复制，且休眠一回合
                    Minion SummonMinion = p.callKidAndReturn(target.handcard.card, p.ownMinions.Count, target.own);
                    SummonMinion.setMinionToMinion(target);
                    SummonMinion.dormant = 1;
                }
        }

        public override PlayReq[] GetUseAbilityReqs()
        {
            return new PlayReq[]
            {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要一个目标才能使用
                new PlayReq(CardDB.ErrorType2.REQ_DAMAGED_TARGET), // 目标必须是一个受伤的随从
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),  // 目标必须是一个随从
            };
        }
    }
}
