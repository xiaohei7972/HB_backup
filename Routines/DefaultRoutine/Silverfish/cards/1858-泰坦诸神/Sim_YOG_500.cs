using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：7 攻击力：7 生命值：6
	//General Vezax
	//维扎克斯将军
	//[x]<b>Rush</b><b>Battlecry:</b> Gain 4 Armor.<b>Deathrattle:</b> Lose 4 Armorto resummon this.
	//<b>突袭</b>。<b>战吼：</b>获得4点护甲值。<b>亡语：</b>失去4点护甲值以再次召唤本随从。
	class Sim_YOG_500 : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.YOG_500);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.minionGetArmor(own.own ? p.ownHero : p.enemyHero, 4);
		}
		public override void onDeathrattle(Playfield p, Minion m)
		{
			if (p.ownHero.armor >= 4)
			{
				p.minionGetArmor(m.own ? p.ownHero : p.enemyHero, -4);
				p.callKid(kid, m.zonepos - 1, m.own);

			}
		}
	}
}
