using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：4 攻击力：5 生命值：3
	//Husk, Eternal Reaper
	//永时收割者哈斯克
	//[x]<b>Battlecry:</b> Give your hero"<b>Deathrattle:</b> Spend all <b>Corpses</b> to resurrect a copy."
	//<b>战吼：</b>使你的英雄获得"<b>亡语：</b>消耗所有残骸，复活一个本随从的复制。"
	class Sim_TIME_618 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own)
			{
				// Give hero the deathrattle effect via the existing TIME_618t token
				p.ownHero.handcard.card.deathrattle = true;
				// Set up the hero's deathrattle2 to reference TIME_618t's behavior
				p.ownHero.deathrattle2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_618t);
			}
		}
	}
}
