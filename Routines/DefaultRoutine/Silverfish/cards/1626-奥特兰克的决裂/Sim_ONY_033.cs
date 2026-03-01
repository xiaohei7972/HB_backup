using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 术士 费用：6
	//Impfestation
	//小鬼侵染
	//Summon a 3/3Dread Imp to attack each enemy minion.
	//每有一个敌方随从，便召唤一个3/3的恐惧小鬼并使其攻击对应敌方随从。
	class Sim_ONY_033 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BAR_914t3);

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			Dictionary<Minion, Minion> keyValues = new Dictionary<Minion, Minion>();
			foreach (Minion minion in ownplay ? p.ownMinions.ToArray() : p.enemyMinions.ToArray())
			{
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				Minion summoned = p.callKidAndReturn(kid, pos, ownplay);
				if (summoned != null)
				{
					keyValues.Add(summoned, minion);
				}
			}
			foreach(KeyValuePair<Minion,Minion> item in keyValues)
            {
                p.minionAttacksMinion(item.Key,item.Value);
            }
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS,1), //对手场上最少需要一个随从
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS,1), // 需要一个空位
			};
		}

	}
}
