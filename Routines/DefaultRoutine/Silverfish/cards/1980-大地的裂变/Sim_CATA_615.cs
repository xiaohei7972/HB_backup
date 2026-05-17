using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：4 攻击力：5 生命值：4
	//Genn, Cursed King
	//吉恩，咒厄国王
	//[x]While holding this, if therest of your hand is all evenor all odd, transform intothe 6/5 Worgen King.
	//当本牌在你手牌中时，如果你其他手牌的法力值消耗均为偶数或奇数，变形成为6/5的狼人国王。
	class Sim_CATA_615 : SimTemplate
	{
		private static readonly CardDB.cardIDEnum WorgenKing = CardDB.cardIDEnum.CATA_615t;

		// Check hand condition at start of turn (while holding)
		public override void onTurnStartTrigger(Playfield p, Handmanager.Handcard hc, bool turnStartOfOwner)
		{
			if (!turnStartOfOwner) return;
			CheckAndTransform(p, hc);
		}

		// Also check when other cards are played (hand changes)
		public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Handmanager.Handcard triggerhc)
		{
			if (!wasOwnCard) return;
			// After card is played, check if Genn should transform
			CheckAndTransform(p, triggerhc);
		}

		private void CheckAndTransform(Playfield p, Handmanager.Handcard gennCard)
		{
			if (gennCard == null || gennCard.card.cardIDenum == WorgenKing) return;

			bool allEven = true;
			bool allOdd = true;

			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc == gennCard) continue; // Skip Genn itself
				int cost = hc.manacost;
				if (cost % 2 != 0) allEven = false;
				if (cost % 2 == 0) allOdd = false;
			}

			// If all other cards are even or all are odd, transform
			if (allEven || allOdd)
			{
				CardDB.Card worgenCard = CardDB.Instance.getCardDataFromID(WorgenKing);
				if (worgenCard != null)
				{
					gennCard.card = worgenCard;
					gennCard.manacost = worgenCard.cost;
					p.evaluatePenality -= 4;
				}
			}
		}
	}
}
