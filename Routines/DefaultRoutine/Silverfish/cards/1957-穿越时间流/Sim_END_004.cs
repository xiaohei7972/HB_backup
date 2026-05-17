using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 死亡骑士 费用：7 攻击力：5 生命值：4
	//Remnant of Rage
	//暴怒余烬
	//Costs (1) less for each minion that died this turn. Battlecry: Draw 2 cards.
	//在本回合中每有一个随从死亡，本牌的法力值消耗便减少（1）点。战吼：抽两张牌。
	class Sim_END_004 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Draw 2 cards
			for (int i = 0; i < 2; i++)
			{
				p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
			}
			p.evaluatePenality -= 6;
		}
	}
}
