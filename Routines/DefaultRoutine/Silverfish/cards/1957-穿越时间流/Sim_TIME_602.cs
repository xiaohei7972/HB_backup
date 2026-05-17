using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：4 攻击力：3 生命值：4
	//Relentless Shade
	//无尽暗影
	//<b>Deathrattle:</b> Summon a 3/2 Shade with "<b>Deathrattle:</b> Summon a 2/1 Shade."
	//<b>亡语：</b>召唤一个3/2并具有"亡语：召唤一个2/1暗影"的暗影。
	class Sim_TIME_602 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Summon a 3/2 Shade
			int pos = m.own ? p.ownMinions.Count : p.enemyMinions.Count;
			p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.None), pos, m.own);
			if (pos < (m.own ? p.ownMinions.Count : p.enemyMinions.Count))
			{
				Minion shade = m.own ? p.ownMinions[pos] : p.enemyMinions[pos];
				p.minionGetBuffed(shade, 3, 2);
			}
		}
	}
}
