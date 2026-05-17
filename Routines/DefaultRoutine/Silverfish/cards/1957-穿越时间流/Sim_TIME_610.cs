using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：6
	//Shadows of Yesterday
	//昨日之影
	//<b>Rewind</b> Summon four 3/2 Shades with random <b>Bonus Effects</b>.
	//<b>回溯</b>。召唤四个3/2的影子，每个影子分别随机获得一项<b>额外效果</b>。
	class Sim_TIME_610 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			CardDB.Card shade = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_610t2);
			int maxSummons = System.Math.Min(4, 7 - (ownplay ? p.ownMinions.Count : p.enemyMinions.Count));
			for (int i = 0; i < maxSummons; i++)
			{
				int pos = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(shade, pos, ownplay);
			}
		}
	}
}
