using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：4
	//Tomb Guardians
	//坟墓守卫
	//Summon two 2/2 Zombies with <b>Taunt</b>. Spend 4 <b>Corpses</b> togive them <b>Reborn</b>.
	//召唤两个2/2并具有<b>嘲讽</b>的僵尸。消耗4份<b>残骸</b>，使其获得<b>复生</b>。
	class Sim_RLK_118 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_118t3);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			
			int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
			Minion m1 = p.callKidAndReturn(kid, pos, ownplay);
			Minion m2 = p.callKidAndReturn(kid, pos, ownplay);

			if (ownplay && p.getCorpseCount() >= 4)
			{
				p.corpseConsumption(4);
				m1.reborn = true;
				m2.reborn = true;
			}
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_MINION_CAP,1), //需要一个空位
			};
        }
		
	}
}
