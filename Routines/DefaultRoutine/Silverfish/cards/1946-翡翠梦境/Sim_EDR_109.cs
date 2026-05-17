using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：7 攻击力：5 生命值：5
	//Ancient of Lore
	//知识古树
	//<b>Choose One:</b> Draw 2 cards OR Restore 8 Health to your hero.
	//<b>抉择：</b>抽两张牌或者为你的英雄恢复8点生命值。
	class Sim_EDR_109 : SimTemplate
	{
		public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (choice == 1 || (p.ownFandralStaghelm > 0 && own.own))
			{
				// Draw 2 cards
				p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
				p.drawACard(CardDB.cardNameEN.unknown, own.own, true);
				p.evaluatePenality -= 4;
			}
			if (choice == 2 || (p.ownFandralStaghelm > 0 && own.own))
			{
				// Restore 8 Health to your hero
				Minion hero = own.own ? p.ownHero : p.enemyHero;
				p.minionGetDamageOrHeal(hero, -8);
				p.evaluatePenality -= 3;
			}
		}

	}
}
