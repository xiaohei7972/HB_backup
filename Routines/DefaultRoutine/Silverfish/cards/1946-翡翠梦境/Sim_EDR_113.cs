using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：0 攻击力：1 生命值：1
	//Wisp of the Dream
	//梦境小精灵
	//<b>Battlecry:</b> Gain 1 Mana Crystal this turn only.
	//<b>战吼：</b>在本回合中，获得1个法力水晶。
	class Sim_EDR_113 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own)
			{
				p.mana += 1;
			}
			p.evaluatePenality -= 1;
		}

	}
}
