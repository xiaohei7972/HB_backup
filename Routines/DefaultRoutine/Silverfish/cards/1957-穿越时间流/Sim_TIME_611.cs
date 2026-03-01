using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：2
	//Timestop
	//时间停滞
	//Deal $3 damage.<b>Freeze</b> two random enemy minions.
	//造成$3点伤害。随机<b>冻结</b>两个敌方随从。
	class Sim_TIME_611 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int i = 0;
				int damage = ownplay ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
				p.minionGetDamageOrHeal(target, damage);
				foreach (Minion m in ownplay ? p.enemyMinions : p.ownMinions)
				{
					if (i == 2)
						break;
					p.minionGetFrozen(m);
				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
			  new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
			};
		}
	}
}
