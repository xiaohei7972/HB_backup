using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 死亡骑士 费用：4 攻击力：4 生命值：3
	//Wicked Blightspawn
	//邪秽枯萎之卵
	//Reborn. Deathrattle: Equip a 1/2 Dagger. If you already have a weapon equipped, give it +2 Attack instead.
	//复生。亡语：装备一把1/2的匕首。如果你已经装备了武器，改为使其获得+2攻击力。
	class Sim_END_002 : SimTemplate
	{
		private CardDB.Card dagger = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_082); // Wicked Knife

		public override void onDeathrattle(Playfield p, Minion m)
		{
			bool own = m.own;
			if (own && p.ownWeapon.Durability > 0)
			{
				// Already have weapon -> +2 Attack
				p.ownWeapon.Angr += 2;
				p.evaluatePenality -= 3;
			}
			else if (!own && p.enemyWeapon.Durability > 0)
			{
				p.enemyWeapon.Angr += 2;
			}
			else
			{
				// No weapon -> equip dagger
				p.equipWeapon(dagger, own);
				if (own) p.evaluatePenality -= 2;
			}
		}
	}
}
