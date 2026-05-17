using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 德鲁伊 费用：2 攻击力：2 生命值：2
	//Felwood Treant
	//邪能树人
	//Battlecry: Gain a temporary Mana Crystal. If you spent 4 Mana while holding this, it's permanent.
	//战吼：获得一个临时的法力水晶。如果在持有本牌时你消耗过4点法力值，则该法力水晶变为永久。
	class Sim_CATA_131 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Gain 1 mana crystal (max 10)
			if (p.ownMaxMana < 10)
			{
				p.ownMaxMana++;
				p.mana++;
			}
			else if (p.mana < 10)
			{
				p.mana++;
			}
			// Reward for gaining mana
			p.evaluatePenality -= 5;
		}
	}
}
