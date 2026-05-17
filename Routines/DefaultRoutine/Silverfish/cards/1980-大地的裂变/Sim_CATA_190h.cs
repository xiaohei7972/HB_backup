using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//英雄 无效的 费用：10
	//Deathwing, Worldbreaker
	//灭世者死亡之翼
	//[x]<b>Battlecry:</b> Choose {0}|4(Cataclysm,Cataclysms) to unleash!<i><b>Herald</b> twice to upgrade.</i>@[x]<b>Battlecry:</b> Choose {0}|4(Cataclysm,Cataclysms) to unleash!<i><b>Herald</b> once to upgrade.</i>@[x]<b>Battlecry:</b> Choose {0}|4(Cataclysm,Cataclysms) to unleash!
	//<b>战吼：</b>选择并释放{0}项灾变！<i><b>兆示</b>两次后升级。</i>@<b>战吼：</b>选择并释放{0}项灾变！<i><b>兆示</b>一次后升级。</i>@<b>战吼：</b>选择并释放{0}项灾变！
	class Sim_CATA_190h : SimTemplate
	{
		// Cataclysm spell pool
		private static readonly CardDB.cardIDEnum[] CataclysmPool = {
			CardDB.cardIDEnum.CATA_190t10, // Dragon's Reign
			CardDB.cardIDEnum.CATA_190t11,
			CardDB.cardIDEnum.CATA_190t12,
			CardDB.cardIDEnum.CATA_190t13,
			CardDB.cardIDEnum.CATA_190t14,
		};

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			// Choose a random Cataclysm to unleash (basic single choice implementation)
			System.Random rnd = new System.Random();
			CardDB.cardIDEnum chosenCataclysm = CataclysmPool[rnd.Next(CataclysmPool.Length)];
			p.drawACard(chosenCataclysm, ownplay, true);

			// Hero card standard: gain 5 Armor
			p.minionGetDamageOrHeal(p.ownHero, 0); // ensure hero exists
			if (ownplay)
			{
				p.ownHero.armor += 5;
				p.evaluatePenality -= 15; // hero card + cataclysm is very strong
			}
		}
	}
}
