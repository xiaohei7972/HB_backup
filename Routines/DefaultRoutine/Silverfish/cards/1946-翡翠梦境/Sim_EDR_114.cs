using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：9 攻击力：5 生命值：12
	//Emerald Oracle
	//翡翠先知
	//<b>Battlecry:</b> <b>Discover</b> 3 cards from your deck. Choose one to get the other two as well.
	//<b>战吼：</b>从你的牌库中<b>发现</b>3张牌。选择一张，其另外两张也会被获取。
	class Sim_EDR_114 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Discover 3 cards from deck — approximated by drawing 3 cards
			// The 'choose one to get the other two as well' means you get all 3
			p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
			p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
			p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
			p.evaluatePenality -= 7;
		}

	}
}
