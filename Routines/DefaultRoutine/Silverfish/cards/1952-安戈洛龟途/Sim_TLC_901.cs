using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 恶魔猎手 费用：2
	//Fumigate
	//烟雾熏蒸
	//Deal $3 damage to a minion and all others of the same minion type.
	//对一个随从及所有相同类型的其他随从造成$3点伤害。
	class Sim_TLC_901 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int damage = ownplay ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
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
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要一个目标
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET), // 只能是随从
                new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET), // 只能是敌方
            };
		}

	}
}
