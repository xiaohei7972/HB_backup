using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：9 攻击力：3 生命值：6
	//Arisen Onyxia
	//复活的奥妮克希亚
	//[x]<b>Colossal +2</b>. When yourhero would lose Health onyour turn, gain that muchmax Health instead.
	//<b>巨型+2</b>当你的英雄在你的回合即将失去生命值时，改为获得等量的生命值上限。
	class Sim_CATA_155 : SimTemplate
	{
		public override int ReturnMinionReceiveDamage(Playfield p, Minion m, int damage)
		{
			// When this minion (or friendly hero) would take damage during your turn,
			// convert that damage into max Health for your hero instead.
			if (p.isOwnTurn && m.own && damage > 0)
			{
				// Convert damage to max Health gain for your hero
				// The hero gains max HP equal to the damage prevented
				if (p.ownHero.Hp >= p.ownHero.maxHp)
				{
					// Increase max HP
					p.ownHero.maxHp += damage;
				}
				p.ownHero.Hp += damage; // Heal for the gained amount

				// This is a powerful defensive effect
				p.evaluatePenality -= damage;

				// Prevent the damage from being dealt to this minion
				return 0;
			}

			return damage;
		}
	}
}
