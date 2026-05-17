using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//地标 圣骑士 费用：2
	//Future Gnomeregan
	//未来的诺莫瑞根
	//[x]Give a minion+2/+1, <b>Divine_Shield</b>,and "<b>Deathrattle:</b> Deal 2damage to the enemy hero."
	//使一个随从获得+2/+1，<b>圣盾</b>和“<b>亡语：</b>对敌方英雄造成2点伤害。”
	class Sim_TIME_044t2 : SimTemplate
	{
        public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 2, 1);
                target.enchs.Add(CardDB.cardIDEnum.TIME_044t1e);
                p.minionGetDivineShield(target);
            }

        }

        public override PlayReq[] GetUseAbilityReqs()
        {
            return new PlayReq[]
            {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
            };
        }

    }
}
