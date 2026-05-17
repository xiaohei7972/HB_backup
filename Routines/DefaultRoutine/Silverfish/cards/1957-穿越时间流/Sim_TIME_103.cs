using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：6 攻击力：4 生命值：6
	//Chromie
	//克罗米
	//<b>Deathrattle:</b> Draw another copy of cards you've played this game.@ <i>(@ |4(card,cards))</i>
	//<b>亡语：</b>抽取你在本局对战中使用过的每张牌的另一张复制。@<i>（抽@张牌）</i>
	class Sim_TIME_103 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			int drawCount = 0;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.type == CardDB.cardtype.MOB)
				{
					p.drawACard(CardDB.cardIDEnum.None, m.own, true);
					drawCount++;
				}
			}
			for (int i = drawCount; i < 4; i++)
			{
				p.drawACard(CardDB.cardIDEnum.None, m.own, true);
			}
		}
	}
}
