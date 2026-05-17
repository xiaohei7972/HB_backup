using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：4 攻击力：3 生命值：5
	//Soul Weaver
	//灵魂编织者
	//<b>Battlecry:</b> Your next Corpse spending this turn costs 2 less.
	//<b>战吼：</b>在本回合中，你下一次消耗残骸的法力值消耗减少（2）点。
	class Sim_TIME_616 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own)
			{
				// Reduce next corpse spending cost by 2
				// Tracked via a flag - reduces mana cost of corpse-spending cards
				p.ownHeroPowerCostLessOnce += 2;
			}
		}
	}
}
