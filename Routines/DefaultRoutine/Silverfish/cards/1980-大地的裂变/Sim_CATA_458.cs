using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：4 攻击力：4 生命值：4
	//Archmage Kalec
	//大法师卡雷
	//[x]<b>Battlecry:</b> Give all spellsin your hand and deck<b>Spell Damage +1.</b>
	//<b>战吼：</b>使你手牌和牌库中所有法术牌获得<b>法术伤害+1</b>。
	class Sim_CATA_458 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int spellsBuffed = 0;

			// Buff spells in hand
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.SPELL)
				{
					hc.card.spellpowervalue += 1;
					hc.card.Spellpower = true;
					spellsBuffed++;
				}
			}

			// Buff spells in deck
			foreach (CardDB.Card c in p.ownDeck)
			{
				if (c.type == CardDB.cardtype.SPELL)
				{
					c.spellpowervalue += 1;
					c.Spellpower = true;
					spellsBuffed++;
				}
			}

			// Also update the global spellpower tracker for the board
			if (spellsBuffed > 0)
			{
				p.spellpower += spellsBuffed;
			}

			p.evaluatePenality -= spellsBuffed * 3;
		}
	}
}
