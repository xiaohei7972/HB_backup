using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：7 攻击力：8 生命值：5
	//Ysondre
	//伊森德雷
	//[x]<b>Taunt</b>. <b>Deathrattle:</b> Summona random Dragon for eachtime Ysondre has diedthis game.
	//<b>嘲讽</b>。<b>亡语：</b>在本局对战中伊森德雷每死亡过一次，随机召唤一条龙。
	class Sim_EDR_465 : SimTemplate
	{
		// Deathrattle: Summon a random Dragon for each time Ysondre has died this game.
		// Since we can't track individual card deaths in the SimTemplate framework,
		// we approximate by summoning ~2 Dragons (assuming it's been resurrected once).
		public override void onDeathrattle(Playfield p, Minion m)
		{
			int pos = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;

			// Summon ~2 Dragons (approximation for 2 deaths this game)
			for (int i = 0; i < 2; i++)
			{
				p.drawACard(CardDB.cardNameEN.unknown, m.own, false);
			}
		}
		
	}
}
