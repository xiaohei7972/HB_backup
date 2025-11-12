using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //地标 术士 费用：4
    //Forge of Wills
    //意志熔炉
    //Choose a friendly minion. Summon a Giant with its stats and <b>Rush</b>.
    //选择一个友方随从。召唤一个具有其属性值和<b>突袭</b>的巨人。
    class Sim_TTN_465 : SimTemplate
    {
        public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
        {
            // 检查目标是否为有效友方随从
            if (target != null && target.own)
            {
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_465t);
                kid.Health = target.Hp;
                kid.Attack = target.Angr;
                kid.dormant = 1;
                // 假设巨人的卡牌ID为 "GIANT_CARD_ID"，需要替换为实际的ID
                p.callKid(kid, p.ownMinions.Count, true);
            }
        }

        public override PlayReq[] GetUseAbilityReqs()
        {
            return new PlayReq[]
            {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要一个目标才能使用
                new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET), // 目标必须是友方随从
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET), // 目标必须是一个随从
            };
        }
    }
}
