using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：5 攻击力：5 生命值：5
	//Emerald Drake
	//翡翠幼龙
	//<b>Battlecry:</b> If you have <b>Spell Damage</b>, gain +1/+1 and draw a card.
	//<b>战吼：</b>如果你拥有<b>法术伤害</b>，获得+1/+1并抽一张牌。
	class Sim_EDR_103 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Check if the player has Spell Damage on their side
			bool hasSpellDamage = false;
			if (own.own)
			{
				if (p.spellpower > 0) hasSpellDamage = true;
			}
			else
			{
				if (p.enemyspellpower > 0) hasSpellDamage = true;
			}

			if (hasSpellDamage)
			{
				p.minionGetBuffed(own, 1, 1);
				p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
				p.evaluatePenality -= 3;
			}
		}

	}
}
