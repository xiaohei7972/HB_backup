using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：4 攻击力：4 生命值：5
	//Talanji of the Graves
	//墓地尊主塔兰吉
	//[x]<b>Fabled.</b> <b>Battlecry:</b> If you've played a Fabled, <b>Discover</b> a Boon.
	//<b>奇闻</b><b>战吼：</b>如果你使用过一张奇闻牌，<b>发现</b>一项恩泽。
	class Sim_TIME_619 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (!own.own) return;
			// Check if any Fabled card has been played this game
			// Fabled TIME cards: TIME_020, TIME_209, TIME_609, TIME_850, TIME_619
			bool hasFabled = false;
			CardDB.cardIDEnum[] fabledCards = new CardDB.cardIDEnum[]
			{
				CardDB.cardIDEnum.TIME_020,
				CardDB.cardIDEnum.TIME_209,
				CardDB.cardIDEnum.TIME_609,
				CardDB.cardIDEnum.TIME_850,
				CardDB.cardIDEnum.TIME_619
			};
			foreach (var cid in fabledCards)
			{
				if (p.ownGraveyard.ContainsKey(cid))
				{
					hasFabled = true;
					break;
				}
			}
			if (hasFabled)
			{
				// Discover a Boon - draw a random boon card
				p.drawACard(CardDB.cardIDEnum.None, own.own, true);
			}
		}
	}
}
