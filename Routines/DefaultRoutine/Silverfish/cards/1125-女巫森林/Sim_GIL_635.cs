using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：2 攻击力：2 生命值：2
	//Cathedral Gargoyle
	//教堂石像兽
	//<b>Battlecry:</b> If you're holding a Dragon, gain <b>Taunt</b> and <b>Divine Shield</b>.
	//<b>战吼：</b>如果你的手牌中有龙牌，则获得<b>嘲讽</b>和<b>圣盾</b>。
	class Sim_GIL_635 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if(own.own)
			{
				if(p.anyRaceCardInHand(CardDB.Race.DRAGON))
				{
					own.taunt = true;
                    own.divineshild = true;
					p.anzOwnTaunt++;

                }
			}
			else
			{
				if (p.enemyAnzCards >= 2)
				{
					own.divineshild = true;
					own.taunt = true;
                    p.anzEnemyTaunt++;
                }					
			}
        }
		
		
	}
}
