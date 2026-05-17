using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：1 攻击力：1 生命值：1
	//Treasure Waifu
	//宝藏娘
	//Deathrattle: Add a random Legendary to your hand.
	//亡语：随机将一张传说随从牌置入你的手牌。
	class Sim_CATA_120 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			// Add a random Legendary minion to hand
			// Pool of neutral Legendary minions
			CardDB.cardIDEnum[] legendaries = {
				CardDB.cardIDEnum.EX1_561,  // Alexstrasza
				CardDB.cardIDEnum.NEW1_030, // Deathwing
				CardDB.cardIDEnum.EX1_562,  // Onyxia
				CardDB.cardIDEnum.BRM_031,  // Chromaggus
				CardDB.cardIDEnum.AT_130,   // Chillmaw
			};

			System.Random rnd = new System.Random();
			CardDB.cardIDEnum chosen = legendaries[rnd.Next(legendaries.Length)];
			p.drawACard(chosen, m.own, true);

			p.evaluatePenality -= 4;
		}
	}
}
