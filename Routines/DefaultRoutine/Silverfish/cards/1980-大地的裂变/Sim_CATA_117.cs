using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：2
	//Sunderer
	//分裂者
	//Destroy a minion. Its owner gains a Soul Fragment.
	//消灭一个随从。其拥有者获得一张灵魂残片。
	class Sim_CATA_117 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target == null) return;

			// Determine who owns the target
			bool targetOwnedByUs = target.own;

			// Destroy the target minion
			p.minionGetDestroyed(target);

			// Give the owner a Soul Fragment
			// Add to Soul Fragments count (SCH_307t is the Soul Fragment token)
			if (targetOwnedByUs)
			{
				p.AnzSoulFragments++;
				// Add Soul Fragment card to hand (token card for tracking)
				p.drawACard(CardDB.cardIDEnum.SCH_307t, true, true);
			}
			else
			{
				// Enemy gets a Soul Fragment - we don't track enemy fragments directly,
				// but we can add it to their hand count
				p.enemyAnzCards++;
			}

			p.evaluatePenality -= 4;
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}
