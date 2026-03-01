using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：0
	//Devouring Swarm
	//集群撕咬
	//[x]Choose an enemy minion.Your minions attack it,then return any that die to your hand.
	//选择一个敌方随从，使你的所有随从攻击该随从，并将死亡的友方随从移回你的手牌。
	class Sim_SW_460 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
            {
				List<Minion> minions = ownplay ? p.ownMinions : p.enemyMinions;
				foreach (Minion minion in minions.ToArray())
				{
					p.minionAttacksMinion(minion, target);
                    if (minion.Hp <= 0)
                    {
						p.drawACard(minion.handcard.card.cardIDenum, minion.own, true);
                    }
					if (target.Hp <= 0)
                        break;
				}
            }
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS,1)
			};
        }
		
	}
}
