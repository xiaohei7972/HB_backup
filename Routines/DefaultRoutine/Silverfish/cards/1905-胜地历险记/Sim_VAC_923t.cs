using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//地标 圣骑士 费用：5
	//Sanc'Azel
	//圣沙泽尔
	//[x]Give a friendly minion+@ Attack and <b>Rush</b>.Turn back into a minion.
	//使一个友方随从获得+@攻击力和<b>突袭</b>。变回随从。
	class Sim_VAC_923t : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_923);
		public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
		{
			if (target != null)
			{
				kid.Attack = triggerMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1;
				kid.Health = triggerMinion.Hp;
				kid.CooldownTurn = triggerMinion.CooldownTurn;

				p.minionGetBuffed(target, triggerMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1 < 3 ? 3 : triggerMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1, 0);
				p.minionGetRush(target);
				p.callKid(kid, triggerMinion.zonepos - 1, triggerMinion.own);
			}
		}

		// public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
		// {

		// 		if (target != null)
		// 		{
		// 			int Angr = triggerMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1;
		// 			int Hp = triggerMinion.Hp;

		// 			p.minionGetBuffed(target, triggerMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1, 0);
		// 			p.minionGetRush(target);

		// 			Handmanager.Handcard hc = new Handmanager.Handcard(kid) { entity = p.getNextEntity() };
		// 			Minion m = p.createNewMinion(hc, triggerMinion.zonepos, triggerMinion.own);
		// 			triggerMinion.setMinionToMinion(m);
		// 			triggerMinion.Angr = Angr;
		// 			triggerMinion.Hp = Hp - 1;

		// 	}

		// }

		public override PlayReq[] GetUseAbilityReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要一个目标才能使用
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET), // 目标必须是友方随从
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET), // 目标必须是一个随从
			};
		}

	}
}
