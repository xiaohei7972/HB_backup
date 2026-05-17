using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 萨满祭司 费用：3 攻击力：4 生命值：3
	//Crackling Cloudstrider
	//雷鸣流云
	//[x]<b>Battlecry:</b> Choose a spellin your hand that costs(4) or less to absorb. <b>Deathrattle:</b> Cast it.
	//<b>战吼：</b>选择并吸收你手牌中一张法力值消耗小于或等于（4）点的法术牌。<b>亡语：</b>施放该法术。
	class Sim_CATA_563 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Find a spell costing ≤4 in hand to absorb
			Handmanager.Handcard toAbsorb = null;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.SPELL && hc.manacost <= 4)
				{
					toAbsorb = hc;
					break;
				}
			}

			if (toAbsorb == null) return; // No valid spell to absorb

			// Store absorbed spell info on this minion
			own.TAG_SCRIPT_DATA_NUM_1 = (int)toAbsorb.card.cardIDenum;

			// Remove the spell from hand (absorbed)
			p.owncards.Remove(toAbsorb);

			p.evaluatePenality -= 2;
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Cast the absorbed spell
			CardDB.cardIDEnum absorbedSpellId = (CardDB.cardIDEnum)m.TAG_SCRIPT_DATA_NUM_1;

			if (absorbedSpellId != CardDB.cardIDEnum.None)
			{
				// Cast the spell's onCardPlay effect
				CardDB.Card absorbedCard = CardDB.Instance.getCardDataFromID(absorbedSpellId);
				if (absorbedCard != null && absorbedCard.sim_card != null)
				{
					// Cast as if played by owner
					absorbedCard.sim_card.onCardPlay(p, m.own, null, 0, new Handmanager.Handcard { card = absorbedCard });
				}

				p.evaluatePenality -= 3;
			}
		}
	}
}
