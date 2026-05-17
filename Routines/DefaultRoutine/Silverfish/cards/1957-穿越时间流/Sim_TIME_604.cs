using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：5
	//Corpse Explosion
	//尸爆
	//Deal $5 damage to a minion. Spend 5 Corpses to deal $5 to all enemies.
	//对一个随从造成$5点伤害。消耗5份残骸，改为对所有敌人造成$5点伤害。
	class Sim_TIME_604 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int dmg = ownplay ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);

			if (ownplay && p.getCorpseCount() >= 5)
			{
				// Spend 5 Corpses, deal 5 to all enemies
				p.corpseConsumption(5);
				p.allCharsOfASideGetDamage(!ownplay, dmg);
			}
			else
			{
				// Deal 5 damage to a single minion
				if (target != null)
				{
					p.minionGetDamageOrHeal(target, dmg);
				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}
