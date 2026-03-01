using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 战士 费用：5
	//Blackrock 'n' Roll
	//黑石摇滚
	//Give all minions in your deck Attack and Health equal to their Cost.
	//使你牌库中的所有随从牌获得等同于其法力值消耗的攻击力和生命值。
	class Sim_ETC_417 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (ownplay)
			{
				foreach (Handmanager.Handcard handcard in p.owncards)
                {
                    if (handcard.card.type == CardDB.cardtype.MOB)
                    {
                        handcard.addattack++;
                        handcard.addHp++;
                    }
                }
			}
		}
		
	}
}
