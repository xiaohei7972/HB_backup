using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 猎人 费用：2
	//Toxic Arrow
	//剧毒箭矢
	//Deal $2 damage to a minion. If it survives, give it <b>Poisonous</b>.
	//对一个随从造成$2点伤害，如果它依然存活，则使其获得<b>剧毒</b>。
	class Sim_CORE_ICC_049 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
				p.minionGetDamageOrHeal(target, dmg);
				if (target.Hp > 0) target.poisonous = true;
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
