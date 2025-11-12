using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：2
	//Air Raid
	//空中团战
	//<b>Twinspell</b>Summon two 1/1 Silver_Hand Recruits with <b>Taunt</b>.
	//<b>双生法术</b>召唤两个1/1并具有<b>嘲讽</b>的白银之手新兵。
	class Sim_RLK_Prologue_YOD_012 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);
		
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			for (int i = 0; i < 2; i++)
			{
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				Minion m = p.callKidAndReturn(kid, pos, ownplay);
				if (m != null)
				{
					m.taunt = true;
				}
			}
			p.drawACard(CardDB.cardIDEnum.YOD_012ts, ownplay, true);
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINION_CAP,1),
			};
        }
		
	}
}
