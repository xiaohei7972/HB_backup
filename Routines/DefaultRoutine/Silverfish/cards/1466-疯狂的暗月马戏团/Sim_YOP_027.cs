using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_YOP_027 : SimTemplate //* 套索射击 Bola Shot
	{
		//Deal $1 damage to a minion and $2 damage to its neighbors.
		//对一个随从造成$1点伤害，并对其相邻的随从造成$2点伤害。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int dmg = p.getSpellDamageDamage(1);
				int dmg1 = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
				p.minionGetDamageOrHeal(target, dmg);
				List<Minion> temp = new List<Minion>((target.own) ? p.ownMinions : p.enemyMinions);
				p.minionGetDamageOrHeal(target, dmg);
				foreach (Minion m in temp)
				{
					if (m.zonepos + 1 == target.zonepos || m.zonepos - 1 == target.zonepos)
						m.getDamageOrHeal(dmg1, p, true, false); // isMinionAttack=true because it is extra damage (we calc clear lostDamage)
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
