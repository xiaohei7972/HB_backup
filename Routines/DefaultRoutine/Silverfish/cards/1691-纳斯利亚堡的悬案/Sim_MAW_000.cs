using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：2 攻击力：1 生命值：1
	//Imp-oster
	//冒牌小鬼
	//<b>Battlecry:</b> Choose a friendly Imp. Transform into a copy of it.
	//<b>战吼：</b>选择一个友方小鬼，变形成为一个它的复制。
	class Sim_MAW_000 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				own.setMinionToMinion(target);
			}
		}


		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG,CardDB.Specialtags.IMP),
			};
		}

	}
}
