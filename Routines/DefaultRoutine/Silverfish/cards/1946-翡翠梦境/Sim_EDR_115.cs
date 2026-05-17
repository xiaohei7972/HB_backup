using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：4
	//Nature's Wrath
	//自然之怒
	//Deal $4 damage to a minion. If it dies, summon a 4/4 Ancient.
	//对一个随从造成$4点伤害。如果其死亡，召唤一个4/4的古树。
	class Sim_EDR_115 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EDR_209t5); // 5/5 Ancient with Taunt (stand-in for 4/4 Ancient)

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target != null)
			{
				int damage = ownplay ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
				p.minionGetDamageOrHeal(target, damage);

				// Check if the target died from this damage
				if (target.Hp <= 0)
				{
					int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
					p.callKid(kid, pos, ownplay);
					p.evaluatePenality -= 3;
				}
				p.evaluatePenality -= 1;
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
