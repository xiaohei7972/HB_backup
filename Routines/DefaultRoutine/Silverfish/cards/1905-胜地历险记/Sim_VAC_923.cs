using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：5 攻击力：3 生命值：8
	//Sanc'Azel
	//圣沙泽尔
	//<b>Rush</b>After this attacks, turninto a location.
	//<b>突袭</b>在本随从攻击后，变成地标。
	class Sim_VAC_923 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_923t);
        public override void afterMinionAttack(Playfield p, Minion attacker, Minion defender, bool dontcount)
        {
           if (attacker.Hp > 0)
			{
				kid.Attack = attacker.Angr;
				kid.Health = attacker.Hp;
				kid.CooldownTurn = attacker.CooldownTurn;

				p.callKid(kid, attacker.zonepos - 1, attacker.own);

			}
        }
		// public override void afterMinionAttack(Playfield p, Minion attacker, Minion defender, bool dontcount)
		// {
		// 	if (attacker.Hp > 0)
		// 	{
		// 		int Angr = attacker.Angr;
		// 		int Hp = attacker.Hp;
		// 		Handmanager.Handcard hc = new Handmanager.Handcard(kid) { entity = p.getNextEntity() };
		// 		Minion m = p.createNewMinion(hc, attacker.zonepos, attacker.own);
		// 		attacker.setMinionToMinion(m);
		// 		attacker.handcard.card.TAG_SCRIPT_DATA_NUM_1 = Angr;
		// 		attacker.Hp = Hp;

		// 	}
		// }
	}
}
