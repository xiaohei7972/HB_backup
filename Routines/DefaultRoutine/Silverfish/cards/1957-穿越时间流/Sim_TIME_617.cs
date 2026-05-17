using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：3
	//Deathchill Blight
	//死亡寒疫
	//Deal $3 damage to all minions. Shuffle a Corpse Egg.
	//对所有随从造成$3点伤害。将一份残骸之卵洗入你的牌库。
	class Sim_TIME_617 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int dmg = ownplay ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
			p.allMinionsGetDamage(dmg);
			// Shuffle a Corpse Egg into the deck (represented by adding 3 corpses)
			if (ownplay)
			{
				p.addCorpses(3);
			}
		}
	}
}
