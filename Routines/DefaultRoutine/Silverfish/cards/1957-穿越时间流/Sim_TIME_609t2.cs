using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 猎人 费用：3 攻击力：2 生命值：4
	//Ranger Initiate Vereesa
	//游侠新兵温蕾萨
	//[x]<b>Battlecry:</b> Give minions inyour deck +1/+1. If you'veplayed Alleria or Sylvanas,repeat for each.
	//<b>战吼：</b>使你牌库中的随从牌获得+1/+1。如果你使用过奥蕾莉亚或希尔瓦娜斯，每使用过一位，重复一次。
	class Sim_TIME_609t2 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			foreach (CardDB.Card card in p.ownDeck)
			{
				if (card.type == CardDB.cardtype.MOB) // 检查是否为随从卡牌
				{
					card.Attack += 1; // 增加攻击力
					card.Health += 1; // 增加生命值
				}
			}
		}

	}
}
