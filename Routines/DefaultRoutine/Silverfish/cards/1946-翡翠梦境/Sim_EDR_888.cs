using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：8 攻击力：8 生命值：6
	//Malorne the Waywatcher
	//护路者玛洛恩
	//[x]<b>Battlecry:</b> <b>Discover</b> a <b>Legendary</b>Wild God. If you've <b>Imbued</b>your Hero Power 4 times,set its Cost to (1).
	//<b>战吼：</b><b>发现</b>一张<b>传说</b>荒野之神。如果你已<b>灌注</b>过你的英雄技能4次，则将发现的荒野之神的法力值消耗变为（1）点。
	class Sim_EDR_888 : SimTemplate
	{
		// Battlecry: Discover a Legendary Wild God card.
		// If you've Imbued Hero Power 4 times, the discovered card costs (1).
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Discover a card (represented as drawing a card)
			p.drawACard(CardDB.cardNameEN.unknown, own.own, true);

			// Reduce the cost of the last drawn card to simulate the (1) cost discount
			// if imbue condition is met (simplified: always apply discount)
			if (own.own && p.owncards.Count > 0)
			{
				Handmanager.Handcard lastCard = p.owncards[p.owncards.Count - 1];
				lastCard.manacost = Math.Min(lastCard.manacost, 1);
			}
		}
		
	}
}
