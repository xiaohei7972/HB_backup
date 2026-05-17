using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：2 攻击力：1 生命值：3
	//Dreamcatcher
	//捕梦者
	//<b>Battlecry:</b> <b>Imbue</b> your Hero Power. <b>Deathrattle:</b> Draw a card.
	//<b>战吼：</b><b>灌注</b>你的英雄技能。<b>亡语：</b>抽一张牌。
	class Sim_EDR_101 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Imbue your Hero Power — upgraded hero power effect (set mechanic)
			// The imbue mechanic is handled by the game engine; in the sim
			// we approximate its value as a beneficial effect.
			p.evaluatePenality -= 3;
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			p.drawACard(CardDB.cardNameEN.unknown, m.own, true);
		}

	}
}
