using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：5 攻击力：4 生命值：6
	//Runic Guardian
	//符文守卫
	//Spell Damage +2. Battlecry: Draw a spell.
	//法术伤害+2。战吼：抽一张法术牌。
	class Sim_CATA_122 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Draw a spell from deck
			bool drawn = false;
			foreach (CardDB.Card c in p.ownDeck)
			{
				if (c.type == CardDB.cardtype.SPELL)
				{
					p.drawACard(c.cardIDenum, own.own, true);
					drawn = true;
					break;
				}
			}

			if (!drawn)
			{
				// No spell in deck, draw a random card instead
				p.drawACard(CardDB.cardIDEnum.None, own.own, true);
			}

			p.evaluatePenality -= 3;
		}
	}
}
