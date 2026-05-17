using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 萨满祭司 费用：8 攻击力：2 生命值：8
	//Al'Akir, Lord of Storms
	//奥拉基尔，风暴之主
	//[x]<b>Colossal +2, <b>Rush</b>, Windfury</b><b>Battlecry:</b> Get 2 minions withCost equal to this minion'sAttack. They cost (1).
	//<b>巨型+2<b>突袭。</b>风怒。</b><b>战吼：</b>获取2张法力值消耗等同于本随从攻击力的随从牌，这两张牌的法力值消耗为（1）点。
	class Sim_CATA_153 : SimTemplate
	{
		// Pool of 2-cost minions (Al'Akir has 2 Attack base)
		private static readonly CardDB.cardIDEnum[] MinionPool2Cost = {
			CardDB.cardIDEnum.CS2_120,  // River Crocolisk 2/3
			CardDB.cardIDEnum.EX1_048,  // Kobold Geomancer 2/2
			CardDB.cardIDEnum.EX1_396,  // Amani Berserker 2/3
		};

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Get 2 minions with Cost equal to this minion's Attack
			// Base Attack is 2, so get 2-cost minions
			int attack = own.Angr;
			System.Random rnd = new System.Random();

			for (int i = 0; i < 2; i++)
			{
				// Draw 2 minions of appropriate cost
				CardDB.cardIDEnum minionId = MinionPool2Cost[rnd.Next(MinionPool2Cost.Length)];
				p.drawACard(minionId, own.own, true);
			}

			// Card advantage is strong + Colossal tokens add value
			p.evaluatePenality -= 8;
		}
	}
}
