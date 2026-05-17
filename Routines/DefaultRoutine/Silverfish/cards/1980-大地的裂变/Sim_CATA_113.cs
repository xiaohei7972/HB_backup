using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：4
	//Verdant Phoenix
	//翠绿凤凰
	//Deal $4 damage to a minion and $1 to all other enemies. Costs (1) less if you cast a spell this turn.
	//对一个随从造成$4点伤害，并对所有其他敌人造成$1点伤害。如果你在本回合施放过法术，本牌的法力值消耗减少（1）点。
	class Sim_CATA_113 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target == null) return;

			int dmgMain = p.getSpellDamageDamage(4);
			int dmgOthers = p.getSpellDamageDamage(1);

			// Deal 4 damage to target minion
			p.minionGetDamageOrHeal(target, dmgMain);

			// Deal 1 damage to all other enemies (enemy minions and enemy hero)
			foreach (Minion m in p.enemyMinions)
			{
				if (m != target && m.Hp > 0)
				{
					p.minionGetDamageOrHeal(m, dmgOthers);
				}
			}
			// Also damage enemy hero
			if (p.enemyHero != target && p.enemyHero.Hp > 0)
			{
				p.minionGetDamageOrHeal(p.enemyHero, dmgOthers);
			}

			p.evaluatePenality -= 6;
		}

		public override int CalculateManaCost(Playfield p, Handmanager.Handcard hc, int OriginalManaCost)
		{
			int newCost = OriginalManaCost;

			// Costs (1) less if you cast a spell this turn
			if (p.spellsplayedSinceRecalc > 0)
			{
				newCost -= 1;
			}

			return Math.Max(0, newCost);
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
			};
		}
	}
}
