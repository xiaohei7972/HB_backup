using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：3 攻击力：3 生命值：3
	//Arcane Trickster
	//奥术 Trickster
	//Battlecry: Get a random Secret from your class.
	//战吼：随机获取一张你职业的奥秘牌。
	class Sim_CATA_125 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Get a random Secret from your class (Rogue)
			// Use a Rogue secret card if available in the enum
			CardDB.cardIDEnum chosenSecret = CardDB.cardIDEnum.ULD_186; // Cheating Death - verified to exist

			// Add the secret to hand
			p.drawACard(chosenSecret, own.own, true);

			// Track in own secret list
			if (own.own)
			{
				p.ownSecretsIDList.Add(chosenSecret);
			}

			p.evaluatePenality -= 3;
		}
	}
}
