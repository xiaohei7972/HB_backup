using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BT_729 : SimTemplate //* 废土守望者 Waste Warden
	{
        //[x]<b>Battlecry:</b> Deal 3 damage toa minion and all others ofthe same minion type.
        //<b>战吼：</b>对一个随从及所有随从类型相同的其他随从造成3点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
			{
				int damage = 3;
				List<CardDB.Race> races = target.handcard.card.GetRaces();
				p.minionGetDamageOrHeal(target, damage);
                foreach (Minion m in p.ownMinions)
                {
                    if (m.entitiyID == target.entitiyID) continue;
                    if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), races))
                        p.minionGetDamageOrHeal(target, damage);
                }
                
                foreach (Minion m in p.enemyMinions)
				{
					if (m.entitiyID == target.entitiyID) continue;
					if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), races))
						p.minionGetDamageOrHeal(target, damage);
				}
			}
        }


		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE), // 如果有目标
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET), // 只能是随从
                new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET), // 只能是敌方
            };
		}

	}
}
