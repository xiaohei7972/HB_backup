using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 萨满祭司 费用：10 攻击力：10 生命值：10
	//Morchok
	//莫卓克
	//[x]<b>Battlecry:</b> Draw a cardand reduce its Cost by (10).Repeat this with excessCost reduction.
	//<b>战吼：</b>抽一张牌，并使其法力值消耗减少（10）点。如果减少的溢出量还有剩余，重复此效果。
	class Sim_CATA_570 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int reductionPool = 10;
			int maxIterations = 10; // safety cap
			int iteration = 0;

			while (reductionPool > 0 && iteration < maxIterations)
			{
				iteration++;
				if (p.ownDeckSize <= 0) break;
				if (p.owncards.Count >= 10) break; // Hand full

				// Draw a card from deck
				p.drawACard(CardDB.cardIDEnum.None, own.own, true);

				// Get the last drawn card (most recently added to hand)
				if (p.owncards.Count == 0) break;
				Handmanager.Handcard hc = p.owncards[p.owncards.Count - 1];
				int cardCost = hc.manacost;

				if (cardCost <= reductionPool)
				{
					// Full reduction — card costs 0
					hc.manacost = 0;
					reductionPool -= cardCost;
				}
				else
				{
					// Partial reduction
					hc.manacost -= reductionPool;
					reductionPool = 0;
				}
			}

			p.evaluatePenality -= 8;
		}
	}
}
