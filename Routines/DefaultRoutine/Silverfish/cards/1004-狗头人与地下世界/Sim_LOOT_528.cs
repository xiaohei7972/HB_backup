using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//* 暮光侍僧 Twilight Acolyte
	//<b>Battlecry:</b> If you're holdinga Dragon, swap this minion's Attack with another minion's.
	//<b>战吼：</b>如果你的手牌中有龙牌，则将此随从的攻击力与另一个随从交换。 
	class Sim_LOOT_528 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
		{
			if (m.own)
			{
				if (p.anyRaceCardInHand(CardDB.Race.DRAGON))
				{
					if (target == null) return;

					m.Angr = target.Angr;
					target.Angr = m.Angr;
				}
			}
			else
			{
				if (p.enemyAnzCards >= 2)
				{
					if (target == null) return;

					m.Angr = target.Angr;
					target.Angr = m.Angr;
				}
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}