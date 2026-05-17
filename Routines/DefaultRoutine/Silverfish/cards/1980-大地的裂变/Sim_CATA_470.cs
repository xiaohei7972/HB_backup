using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：4 攻击力：4 生命值：4
	//Victor Nefarius
	//维克多·奈法里奥斯
	//[x]<b>Battlecry:</b> Craft a customUndead Dragon. If you're holding a Dragon, reduce theCreation's Cost by (3).
	//<b>战吼：</b>制造一条自定义的亡灵龙。如果你的手牌中有龙牌，制造的这条龙的法力值消耗减少（3）点。
	class Sim_CATA_470 : SimTemplate
	{
		private static readonly CardDB.cardIDEnum UndeadDragon = CardDB.cardIDEnum.CATA_470t1;

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (p.owncards.Count >= 10) return; // Hand full

			// Create an Undead Dragon token in hand
			p.drawACard(UndeadDragon, own.own, true);

			// Check if holding a Dragon
			bool holdingDragon = false;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.race == CardDB.Race.DRAGON)
				{
					holdingDragon = true;
					break;
				}
			}

			// If holding Dragon, reduce the created card's cost by 3
			if (holdingDragon && p.owncards.Count > 0)
			{
				Handmanager.Handcard created = p.owncards[p.owncards.Count - 1];
				if (created.card.cardIDenum == UndeadDragon)
				{
					created.manacost = Math.Max(0, created.manacost - 3);
				}
			}

			p.evaluatePenality -= holdingDragon ? 6 : 3;
		}
	}
}
