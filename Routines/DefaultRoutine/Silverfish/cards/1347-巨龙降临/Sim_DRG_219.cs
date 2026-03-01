using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_219 : SimTemplate //* 闪电吐息 Lightning Breath
	{
		//[x]Deal $4 damage to aminion. If you're holdinga Dragon, also damageits neighbors.
		//对一个随从造成$4点伤害。如果你的手牌中有龙牌，则同样对其相邻随从造成伤害。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target != null)
			{

				if (ownplay)
				{
					int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
					if (p.anyRaceCardInHand(CardDB.Race.DRAGON))
					{
						p.minionGetDamageOrHeal(target, dmg);
						List<Minion> temp = (target.own) ? p.enemyMinions : p.ownMinions;
						foreach (Minion m in temp.ToArray())
						{
							if (target.zonepos == m.zonepos + 1 || target.zonepos + 1 == m.zonepos)
							{
								p.minionGetDamageOrHeal(m, dmg);
							}
						}
					}
					else
					{
						p.minionGetDamageOrHeal(target, dmg);
					}
				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}