using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 德鲁伊 费用：4 攻击力：4 生命值：5
	//Broodwatcher
	//育雏观察者
	//Battlecry: Get two 3/3 Whelps with Taunt. If you spent 8 Mana while holding this, summon them.
	//战吼：获取两张3/3并具有嘲讽的雏龙。如果在持有本牌时你消耗过8点法力值，召唤这些雏龙。
	class Sim_CATA_132 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Draw 2 Whelps (3/3 Taunt) - add to hand
			CardDB.Card whelp = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_116); // placeholder - would use actual whelp card ID
			for (int i = 0; i < 2; i++)
			{
				p.drawACard(CardDB.cardIDEnum.EX1_116, own.own, true);
			}
			// Moderate reward for card advantage
			p.evaluatePenality -= 4;
		}
	}
}
