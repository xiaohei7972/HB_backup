using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：7 攻击力：4 生命值：8
	//Chrono-Lord Deios
	//时空领主戴欧斯
	//[x]Your <b>Battlecries</b>,<b>Deathrattles</b>, Hero Power,and end of turn effectstrigger twice.
	//你的<b>战吼</b>，<b>亡语</b>，英雄技能和回合结束效果会触发两次。
	class Sim_TIME_064 : SimTemplate
	{
		public override void onAuraStarts(Playfield p, Minion own)
		{
			if (own.own)
			{
				p.ownBrannBronzebeard++;
				p.ownBaronRivendare++;
				p.ownTurnEndEffectsTriggerTwice++;
			}
			else
			{
				p.enemyBrannBronzebeard++;
				p.enemyBaronRivendare++;
				p.enemyTurnEndEffectsTriggerTwice++;
			}
		}

		public override void onAuraEnds(Playfield p, Minion m)
		{
			if (m.own)
			{
				p.ownBrannBronzebeard--;
				p.ownBaronRivendare--;
				p.ownTurnEndEffectsTriggerTwice--;
			}
			else
			{
				p.enemyBrannBronzebeard--;
				p.enemyBaronRivendare--;
				p.enemyTurnEndEffectsTriggerTwice--;
			}
		}
	}
}
