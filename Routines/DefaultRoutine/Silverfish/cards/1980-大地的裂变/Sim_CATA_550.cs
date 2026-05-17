using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 猎人 费用：7 攻击力：2 生命值：12
	//Magmaw
	//熔喉
	//<b>Colossal +99</b>Summon any leftover appendages when there is room.@<b>Colossal +99</b>Summon any leftover appendages when there is room. <i>({0} left!)</i>
	//<b>巨型+99</b>当场上有空位时，召唤剩余的肢节。@<b>巨型+99</b>当场上有空位时，召唤剩余的肢节。<i>（还剩{0}个！）</i>
	class Sim_CATA_550 : SimTemplate
	{
		private static readonly CardDB.cardIDEnum[] Appendages = {
			CardDB.cardIDEnum.CATA_550t,
			CardDB.cardIDEnum.CATA_550t2,
			CardDB.cardIDEnum.CATA_550t3,
			CardDB.cardIDEnum.CATA_550t4,
			CardDB.cardIDEnum.CATA_550t5,
			CardDB.cardIDEnum.CATA_550t6,
		};

		// Summon as many Colossal appendages as will fit
		public override void SummonColossal(Playfield p, Minion m)
		{
			bool own = m.own;
			int maxSlots = 7;
			int currentCount = own ? p.ownMinions.Count : p.enemyMinions.Count;
			int slotsAvailable = maxSlots - currentCount;
			if (slotsAvailable <= 0) return;

			int pos = own ? p.ownMinions.Count : p.enemyMinions.Count;
			int appendagesToSummon = Math.Min(slotsAvailable, Appendages.Length);

			for (int i = 0; i < appendagesToSummon; i++)
			{
				CardDB.Card kid = CardDB.Instance.getCardDataFromID(Appendages[i]);
				p.callKid(kid, pos, own);
			}

			p.evaluatePenality -= appendagesToSummon * 2;
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}
	}
}
