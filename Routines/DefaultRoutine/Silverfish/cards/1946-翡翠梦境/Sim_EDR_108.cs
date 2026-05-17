using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：2 攻击力：1 生命值：3
	//Acolyte of the Dream
	//梦境学徒
	//<b>Deathrattle:</b> Add a random Druid spell to your hand.
	//<b>亡语：</b>随机获取一张德鲁伊法术牌。
	class Sim_EDR_108 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			p.drawACard(CardDB.cardNameEN.unknown, m.own, true);
		}

	}
}
