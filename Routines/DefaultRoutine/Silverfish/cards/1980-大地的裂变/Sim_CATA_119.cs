using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：8 攻击力：8 生命值：8
	//Clockwork Giant
	//发条巨人
	//Costs (1) less for each card in your opponent's hand.
	//你的对手每有一张手牌，本牌的法力值消耗便减少（1）点。
	class Sim_CATA_119 : SimTemplate
	{
		public override int CalculateManaCost(Playfield p, Handmanager.Handcard hc, int OriginalManaCost)
		{
			int newCost = OriginalManaCost;

			// Reduce cost by 1 for each card in opponent's hand
			int reduction = p.enemyAnzCards;
			newCost -= reduction;

			return Math.Max(0, newCost);
		}
	}
}
