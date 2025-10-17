using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：3 攻击力：3 生命值：3
	//Demolition Renovator
	//拆迁修理工
	//<b>Tradeable</b><b>Battlecry:</b> Destroy an enemy location.
	//<b>可交易</b><b>战吼：</b>摧毁一个敌方地标。
	class Sim_REV_023 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{

				p.minionGetDestroyed(target);

			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE), // 没目标时也能用
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET), // 目标只能是敌方
				new PlayReq(CardDB.ErrorType2.REQ_LOCATION_TARGET), // 目标只能是地标
			};
		}
		
	}
}
