using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：4
	//Burden of Pride
	//骄傲罪责
	//[x]Summon three 1/3Jailers with <b>Taunt</b>. If youhave 20 or less Health,give them +1/+1.
	//召唤三个1/3并具有<b>嘲讽</b>的狱卒。如果你的生命值小于或等于20点，使其获得+1/+1。
	class Sim_CORE_REV_334 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.REV_334t);

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
			Minion hero = ownplay ? p.ownHero : p.enemyHero;
			if (hero.Hp <= 20)
			{

				p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);
				p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);
				p.minionGetBuffed(p.callKidAndReturn(kid, pos, ownplay), 1, 2);


			}
			else
			{

				p.callKid(kid, pos, ownplay);
				p.callKid(kid, pos, ownplay);
				p.callKid(kid, pos, ownplay);
			}

		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINION_CAP,1),
			};
		}

	}
}
