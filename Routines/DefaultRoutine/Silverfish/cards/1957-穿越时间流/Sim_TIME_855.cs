using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 法师 费用：3
	//Arcane Barrage
	//奥术弹幕
	//Deal $3 damage to an enemy and $2 damage to two other random ones.
	//对一个敌人造成$3点伤害，并随机对两个其他敌人造成$2点伤害。
	class Sim_TIME_855 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int damage = ownplay ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
				int damage1 = ownplay ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
				List<Minion> minions = new List<Minion>((ownplay ? p.enemyMinions : p.ownMinions));
				minions.Remove(target);
				p.minionGetDamageOrHeal(target, damage);
				if (minions.Count > 0)
				{
					p.minionGetDamageOrHeal(minions[0], damage1);

				}
				if (minions.Count > 1)
				{
					p.minionGetDamageOrHeal(minions[1], damage1);

				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
			};
		}
	}
}
