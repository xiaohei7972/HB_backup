using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：7 攻击力：7 生命值：7
	//Commander Geddon
	//指挥官迦顿
	//[x]<b>Battlecry:</b> Instead of drawingeach turn, <b>Discover</b> a cardfrom your deck. It costs (3)__less. Destroy the others.
	//<b>战吼：</b>你在每回合开始时的抽牌改为从你的牌库中<b>发现</b>一张牌，其法力值消耗减少（3）点，并摧毁未选的牌。
	class Sim_CATA_591 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Discover a card from your deck: draw 3, pick 1 (costs 3 less), destroy other 2
			// For AI simulation: draw 1 card (the "picked" one) with cost reduction
			p.drawACard(CardDB.cardIDEnum.None, own.own, false);

			// Apply cost reduction to the drawn card (the Discovered one costs 3 less)
			if (p.owncards.Count > 0 && own.own)
			{
				Handmanager.Handcard discovered = p.owncards[p.owncards.Count - 1];
				discovered.manacost = System.Math.Max(0, discovered.manacost - 3);
			}

			// Card selection + cost reduction + deck thinning is strong
			if (own.own)
			{
				p.evaluatePenality -= 10;
			}
		}
	}
}
