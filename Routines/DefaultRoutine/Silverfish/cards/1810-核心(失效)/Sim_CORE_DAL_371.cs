using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：4
	//Marked Shot
	//标记射击
	//Deal $4 damage to_a_minion. <b>Discover</b>_a_spell.
	//对一个随从造成$4点伤害。<b>发现</b>一张法术牌。
	class Sim_CORE_DAL_371 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
				p.minionGetDamageOrHeal(target, dmg);
				p.drawACard(CardDB.cardNameEN.unknown, ownplay, true);
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
