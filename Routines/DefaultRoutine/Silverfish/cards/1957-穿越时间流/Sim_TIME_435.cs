using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：6 攻击力：6 生命值：2
	//Eternus
	//伊特努丝
	//[x]<b>Battlecry:</b> Take control ofan enemy minion with this__minion's Health or less.
	//<b>战吼：</b>夺取一个生命值小于或等于本随从的敌方随从的控制权。
	class Sim_TIME_435 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				p.minionGetDestroyed(target);
				int pos = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(target.handcard.card, pos, own.own);
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_NUM_MINION_SLOTS, 1),
			};
		}
	}
}
