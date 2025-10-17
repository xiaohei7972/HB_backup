using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：2
	//Air Raid
	//空中团战
	//Summon two 1/1 Silver Hand Recruits with <b>Taunt</b>.
	//召唤两个1/1并具有<b>嘲讽</b>的白银之手新兵。
	class Sim_RLK_Prologue_YOD_012ts : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			// kid = (CardDB.Card)kid.Clone();
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
			kid.tank = true;
			p.callKid(kid, pos, ownplay);
			p.callKid(kid, pos, ownplay);
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINION_CAP,1),
			};
        }
		
	}
}
