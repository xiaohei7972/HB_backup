using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 德鲁伊 费用：3
	//Verdant Grove
	//翠绿林地
	//Summon a 2/3 Treant. If you have 5+ mana, summon another.
	//召唤一个2/3的树人。如果你拥有5点或以上的法力值，再召唤一个。
	class Sim_EDR_111 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_tk11); // 2/3 Treant with Taunt stand-in

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
			p.callKid(kid, pos, ownplay);

			// Check if player has 5 or more mana
			if (p.mana >= 5)
			{
				pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(kid, pos, ownplay);
				p.evaluatePenality -= 2;
			}
			else
			{
				p.evaluatePenality -= 1;
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
