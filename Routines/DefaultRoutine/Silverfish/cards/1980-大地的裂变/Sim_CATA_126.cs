using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：2 攻击力：2 生命值：3
	//Emerald Hatchling
	//翡翠雏龙
	//Choose One: Summon a 2/2 Whelp; or Give all friendly minions +2 Armor.
	//抉择：召唤一个2/2的雏龙；或者使所有友方随从获得+2护甲值。
	class Sim_CATA_126 : SimTemplate
	{
		public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
		{
			// choice 1 = Summon a 2/2 Whelp
			// choice 2 = Give all friendly minions +2 Armor

			if (choice == 1)
			{
				// Summon a 2/2 Whelp
				// Use a 2/2 token placeholder (Treant from Soul of the Forest)
				CardDB.Card whelp = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);
				int pos = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(whelp, pos, own.own);
				p.evaluatePenality -= 3;
			}
			else if (choice == 2)
			{
				// Give all friendly minions +2 Armor
				List<Minion> side = own.own ? p.ownMinions : p.enemyMinions;
				foreach (Minion m in side)
				{
					if (m.Hp > 0)
					{
						p.minionGetArmor(m, 2);
					}
				}
				p.evaluatePenality -= side.Count * 2;
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}
	}
}
