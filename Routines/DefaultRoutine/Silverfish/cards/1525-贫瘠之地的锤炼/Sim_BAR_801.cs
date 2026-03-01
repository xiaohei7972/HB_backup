using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_801 : SimTemplate //* 击伤猎物 Wound Prey
	{
		//Deal $1 damage. Summon a 1/1 Hyena with <b>Rush</b>.
		//造成$1点伤害。召唤一只1/1并具有<b>突袭</b>的土狼。
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BAR_035t);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int dmg = ownplay ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
				p.minionGetDamageOrHeal(target, dmg);
				p.callKid(kid, p.ownMinions.Count, true);
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
			};
		}
	}
}
