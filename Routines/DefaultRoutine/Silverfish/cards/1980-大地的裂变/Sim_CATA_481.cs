using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 牧师 费用：5 攻击力：5 生命值：3
	//Iso'rath
	//厄索拉斯
	//[x]<b>Battlecry:</b> Devour 2 randomcards from the opponent's hand,then go <b>Dormant</b> for 2 turns.<b>Deathrattle:</b> Return them.
	//<b>战吼：</b>随机吞食对手的两张手牌，然后<b>休眠</b>2回合。<b>亡语：</b>吐回吞食的牌。
	class Sim_CATA_481 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Devour 2 random cards from opponent's hand
			// Cannot directly access enemy hand list, so simulate by reducing count
			int cardsToDevour = System.Math.Min(2, p.enemyAnzCards);

			if (cardsToDevour > 0 && own.own)
			{
				// Opponent loses hand cards
				p.enemyAnzCards -= cardsToDevour;
				p.enemycarddraw -= cardsToDevour;
				p.evaluatePenality -= cardsToDevour * 5; // disrupting opponent's hand is strong
			}

			// Go Dormant for 2 turns
			own.dormant = 2;
		}

		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Return devoured cards to opponent's hand
			// Simulate: opponent draws 2 random cards
			for (int i = 0; i < 2; i++)
			{
				p.drawACard(CardDB.cardIDEnum.None, !m.own, true);
			}

			// Opponent getting cards back is bad for us
			if (m.own)
			{
				p.evaluatePenality += 4;
				p.enemycarddraw += 2;
			}
		}
	}
}
