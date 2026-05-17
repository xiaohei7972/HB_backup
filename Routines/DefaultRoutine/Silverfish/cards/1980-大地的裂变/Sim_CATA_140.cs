using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 德鲁伊 费用：8 攻击力：4 生命值：12
	//Merithra of the Dream
	//梦境之母梅瑞瑟拉
	//Battlecry: Fill your hand with random Dragons. If you spent 25 Mana while holding this, they cost (1).
	//战吼：用随机龙牌填满你的手牌。如果在持有本牌时你消耗过25点法力值，这些牌的法力值消耗变为（1）点。
	class Sim_CATA_140 : SimTemplate
	{
		// Dragon card IDs for random fill - common Dragons
		private static readonly CardDB.cardIDEnum[] DragonPool = {
			CardDB.cardIDEnum.BRM_022,  // Blackwing Technician
			CardDB.cardIDEnum.BRM_031,  // Chromaggus
			CardDB.cardIDEnum.YOD_010,  // Cobalt Spellkin
			CardDB.cardIDEnum.EX1_561,  // Alexstrasza
			CardDB.cardIDEnum.NEW1_030, // Deathwing
			CardDB.cardIDEnum.EX1_562,  // Onyxia
			CardDB.cardIDEnum.DRG_242,  // Faerie Dragon
			CardDB.cardIDEnum.BT_726,   // Azure Drake
		};

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Fill hand with random Dragons (up to hand limit of 10)
			int cardsToDraw = 10 - p.owncards.Count;
			if (cardsToDraw <= 0) return;

			System.Random rnd = new System.Random();
			for (int i = 0; i < cardsToDraw; i++)
			{
				CardDB.cardIDEnum dragonId = DragonPool[rnd.Next(DragonPool.Length)];
				p.drawACard(dragonId, own.own, true);
			}

			// Reward: filling hand is very strong
			p.evaluatePenality -= cardsToDraw * 3;
		}
	}
}
