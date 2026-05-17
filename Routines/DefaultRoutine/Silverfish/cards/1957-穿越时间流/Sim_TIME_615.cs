using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：8
	//Forgotten Millennium
	//遗忘纪元
	//Fill your hand with Undead. Spend Health instead of Mana for them next turn.
	//用随机亡灵牌填满你的手牌。在本回合中，这些牌消耗生命值，而非法力值。
	class Sim_TIME_615 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (!ownplay) return;
			// Fill hand with random Undead cards
			int cardsToDraw = 10 - p.owncards.Count;
			for (int i = 0; i < cardsToDraw; i++)
			{
				p.drawACard(CardDB.cardIDEnum.None, ownplay, true);
			}
		}
	}
}
