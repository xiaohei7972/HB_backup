using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 猎人 费用：7 攻击力：6 生命值：6
	//Ebyssian
	//艾比西安
	//[x]<b>Battlecry:</b> Your Dragonshave <b>Rush</b> this game.<i>(While in hand, play a Dragon tobecome a 12/12 Dragon!)</i>
	//<b>战吼：</b>在本局对战中，你的龙拥有<b>突袭</b>。<i>（当本牌在你手中时，使用一张龙牌即可将本牌变为12/12的龙！）</i>
	class Sim_CATA_553 : SimTemplate
	{
		private static readonly CardDB.cardIDEnum EbyssianDragon = CardDB.cardIDEnum.CATA_553t;

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Grant all friendly Dragons Rush this game
			int dragonsBuffed = 0;

			// Buff dragons on board
			foreach (Minion m in p.ownMinions)
			{
				if (m.Hp <= 0) continue;
				if (m.handcard.card.race == CardDB.Race.DRAGON)
				{
					p.minionGetRush(m);
					dragonsBuffed++;
				}
			}

			// Mark this game state so future dragons will also get Rush
			// Store in TAG_SCRIPT_DATA to track
			own.TAG_SCRIPT_DATA_NUM_2 = 1; // Flag: "Dragons have Rush this game"

			p.evaluatePenality -= 5 + dragonsBuffed * 2;
		}

		// While in hand, when you play a Dragon, transform Ebyssian into a 12/12 Dragon
		public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Handmanager.Handcard triggerhc)
		{
			if (!wasOwnCard) return;

			// If a friendly Dragon is being played and Ebyssian is still in hand, transform it
			if (hc.card.race == CardDB.Race.DRAGON && hc.card.cardIDenum != EbyssianDragon)
			{
				// Transform Ebyssian into 12/12 Dragon form
				CardDB.Card transformedCard = CardDB.Instance.getCardDataFromID(EbyssianDragon);
				if (transformedCard != null)
				{
					triggerhc.card = transformedCard;
					triggerhc.manacost = transformedCard.cost;
				}
			}
		}

		// When new minions are summoned, if Dragons-have-Rush flag is set, grant Rush
		public override void onMinionIsSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
		{
			if (triggerEffectMinion.own != summonedMinion.own) return;
			if (triggerEffectMinion.Hp <= 0) return;
			if (triggerEffectMinion.TAG_SCRIPT_DATA_NUM_2 == 1 && summonedMinion.handcard.card.race == CardDB.Race.DRAGON)
			{
				p.minionGetRush(summonedMinion);
			}
		}
	}
}
