using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：3 攻击力：3 生命值：2
	//Moonlit Trickster
	//月光欺诈者
	//<b>Battlecry:</b> Choose a minion. It gains <b>Stealth</b>. <b>Combo:</b> Give it +3 Attack.
	//<b>战吼：</b>选择一个随从，使其获得<b>潜行</b>。<b>连击：</b>并使其获得+3攻击力。
	class Sim_EDR_104 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				// Give Stealth
				target.stealth = true;

				// Combo: Give it +3 Attack if at least one card was played before this
				if (p.cardsPlayedThisTurn > 0)
				{
					p.minionGetBuffed(target, 3, 0);
				}

				p.evaluatePenality -= 2;
			}
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
