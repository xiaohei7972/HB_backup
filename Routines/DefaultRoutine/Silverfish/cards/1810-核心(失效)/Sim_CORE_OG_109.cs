using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：2 攻击力：3 生命值：2
	//Darkshire Librarian
	//夜色镇图书管理员
	//<b>Battlecry:</b>Discard a random card. <b>Deathrattle:</b>Draw a card.
	//<b>战吼：</b>随机弃一张牌。<b>亡语：</b>抽一张牌。
	class Sim_CORE_OG_109 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.discardCards(1, own.own);
        }
        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.None, m.own);
        }
		
	}
}
