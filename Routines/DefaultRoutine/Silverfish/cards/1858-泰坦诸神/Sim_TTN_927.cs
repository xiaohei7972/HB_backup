using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：3 攻击力：3 生命值：4
	//Conservator Nymph
	//护园森灵
	//<b>Battlecry:</b> Transform a friendly Treant into a 5/5 Ancient with <b>Taunt</b>.
	//<b>战吼：</b>将一个友方树人变形成为5/5并具有<b>嘲讽</b>的古树。
	class Sim_TTN_927 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_903t4);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				p.minionTransform(target, kid);
			}
		}


		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG,CardDB.Specialtags.Treant),
			};
		}

	}
}
