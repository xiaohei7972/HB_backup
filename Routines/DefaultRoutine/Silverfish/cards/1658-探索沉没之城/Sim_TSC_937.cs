using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 牧师 费用：6 攻击力：6 生命值：5
	//Crabatoa
	//可拉巴托亚
	//<b>Colossal +2</b>Your Crabatoa Claws have +2 Attack.
	//<b>巨型+2</b>你的可拉巴托亚的钳子拥有+2攻击力。
	class Sim_TSC_937 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_937t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_937t3);
		CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_937t);

		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative, m.zonepos, m.own);
		}

		public override void onAuraStarts(Playfield p, Minion m)
		{
			Weapon ownWeapon = m.own ? p.ownWeapon : p.enemyWeapon;
			List<Minion> minions = new List<Minion>(m.own ? p.ownMinions : p.enemyMinions);
			foreach (Minion minion in minions)
			{
				if (minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_937t || minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_937t3)
					p.minionGetTempBuff(minion, 2, 0);
			}
			if (weapon.Equals(ownWeapon))
			{
				ownWeapon.Angr += 2;
			}
		}
		public override void onAuraEnds(Playfield p, Minion m)
		{
			Weapon ownWeapon = m.own ? p.ownWeapon : p.enemyWeapon;
			List<Minion> minions = m.own ? p.ownMinions : p.enemyMinions;
			foreach (Minion minion in minions)
			{
				if (minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_937t || minion.handcard.card.cardIDenum == CardDB.cardIDEnum.TSC_937t3)
					p.minionGetTempBuff(minion, -2, 0);
			}
			if (weapon.Equals(ownWeapon))
			{
				ownWeapon.Angr -= 2;
			}
		}

	}
}
