using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：2
	//Embers of Strength
	//力量余烬
	//[x]Summon two 1/2Guards with <b>Taunt</b>.<b>Manathirst (6):</b> Givethem +1/+2.
	//召唤两个1/2并具有<b>嘲讽</b>的守卫。<b>法力渴求（6）：</b>使其获得+1/+2。
	class Sim_RLK_960 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_960t);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{

			if (ownplay ? p.ownMaxMana >= 6 : p.enemyMaxMana >= 6)
			{
				for (int i = 0; i < 2; i++)
				{
					int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
					p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);

				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
					p.callKid(kid, pos, ownplay);

				}
			}

		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS,1), // 需要一个空位
			};
		}

	}
}
