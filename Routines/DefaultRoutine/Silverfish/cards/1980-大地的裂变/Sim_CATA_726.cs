using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 术士 费用：9 攻击力：6 生命值：6
	//Cho'gall, Mastermind
	//古加尔，暮光主谋
	//[x]<b>Colossal +2</b>Your Arms and Soldiersdestroy minions in the_enemy's deck instead.
	//<b>巨型+2</b>你的手臂和士兵改为消灭敌方牌库中的随从。
	class Sim_CATA_726 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Cho'gall modifies Arms/Soldiers: destroy minions from enemy deck instead of board.
			// For AI simulation: remove minion cards from enemy deck (reduce deck size)
			int minionsDestroyed = 3; // Arms + Soldiers destroy ~3 minions from deck

			if (own.own && p.enemyDeckSize > 0)
			{
				// Reduce enemy deck size (simulating destroying minions from it)
				int actualDestroyed = System.Math.Min(minionsDestroyed, p.enemyDeckSize);
				p.enemyDeckSize -= actualDestroyed;

				// Also reduce enemy card draw tracking
				p.enemycarddraw -= actualDestroyed;
				p.evaluatePenality -= actualDestroyed * 4; // removing enemy resources is strong
			}
		}
	}
}
