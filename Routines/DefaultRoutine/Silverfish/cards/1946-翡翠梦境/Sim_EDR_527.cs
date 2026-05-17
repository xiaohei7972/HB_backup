using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 牧师 费用：9 攻击力：7 生命值：7
	//Ashamane
	//阿莎曼
	//[x]<b>Battlecry:</b> Fill your handwith copies of cards fromyour opponent's deck.They cost (3) less.
	//<b>战吼：</b>用你对手牌库中牌的复制填满你的手牌，其法力值消耗减少（3）点。
	class Sim_EDR_527 : SimTemplate
	{
		// Fill hand with copies from opponent's deck, costing (3) less.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int cardsToDraw = 10 - p.owncards.Count;
			if (cardsToDraw <= 0) return;

			// Draw from opponent's deck, reduced cost by 3
			for (int i = 0; i < cardsToDraw; i++)
			{
				p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
			}

			// Reduce cost of drawn cards by 3
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				hc.manacost = Math.Max(0, hc.manacost - 3);
			}
		}
		
	}
}
