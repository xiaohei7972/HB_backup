using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 恶魔猎手 费用：9 攻击力：9 生命值：6
	//Dread Leviathan
	//恐怖海兽
	//[x]<b>Taunt</b>. <b>Battlecry:</b> Choosean enemy minion to steal 3_Health from, three times.
	//<b>嘲讽</b>。<b>战吼：</b>选择一个敌方随从，偷取其3点生命值，触发三次。
	class Sim_CATA_699 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target == null) return;

			int totalStolen = 0;

			for (int i = 0; i < 3; i++)
			{
				if (target.Hp <= 0) break;

				// Steal 3 Health: remove from target, add to own
				int steal = Math.Min(3, target.Hp);
				if (steal <= 0) break;

				p.minionGetDamageOrHeal(target, -steal);
				p.minionGetBuffed(own, 0, steal);
				totalStolen += steal;
			}

			p.evaluatePenality -= totalStolen * 2;
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}
