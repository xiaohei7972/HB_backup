using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：1
	//Unleash the Crocolisks
	//放出鳄鱼
	//Gain 10 Armor.Summon two 2/3 Beasts for your opponent.
	//获得10点护甲值。为你的对手召唤两只2/3的鳄鱼。
	class Sim_TIME_873 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_873t);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 10);
			p.callKid(kid, (ownplay ? p.enemyMinions.Count : p.ownMinions.Count), !ownplay);
			p.callKid(kid, (ownplay ? p.enemyMinions.Count : p.ownMinions.Count), !ownplay);

		}

	}
}
