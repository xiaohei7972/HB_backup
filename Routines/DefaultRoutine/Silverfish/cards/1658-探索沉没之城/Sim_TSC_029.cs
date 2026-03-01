using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：8 攻击力：5 生命值：7
	//Gaia, the Techtonic
	//盖亚，巨力机甲
	//[x]<b>Colossal +2</b>After a friendly Mechattacks, deal 1 damageto all enemies.
	//<b>巨型+2</b>在一个友方机械攻击后，对所有敌人造成1点伤害。
	class Sim_TSC_029 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_029t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_029t2);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
			p.callKid(ColossalDerivative1, m.zonepos, m.own);
		}

		public override void afterMinionAttack(Playfield p, Minion triggerEffectMinion, Minion attacker, Minion defender)
		{
			if (attacker.own && RaceUtils.MinionBelongsToRace(attacker.handcard.card.GetRaces(), CardDB.Race.MECHANICAL))
			{
				p.allCharsOfASideGetDamage(!triggerEffectMinion.own, 1);
			}
		}

	}
}
