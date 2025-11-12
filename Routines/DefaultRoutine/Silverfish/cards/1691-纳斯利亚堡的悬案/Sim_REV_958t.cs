using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：4 攻击力：2 生命值：4
	//Buffet Biggun
	//餐会巨仆
	//<b>Infused.</b> <b>Battlecry:</b> Summon two Silver Hand Recruits. Give them +2 Attack and <b>Divine Shield</b>.
	//<b>已注能</b><b>战吼：</b>召唤两个白银之手新兵。使其获得+2攻击力和<b>圣盾</b>。
	class Sim_REV_958t : SimTemplate
	{
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			Minion m1 = p.callKidAndReturn(kid, own.zonepos - 1, own.own);
			m1.Angr += 2;
			m1.divineshild = true;
			Minion m2 = p.callKidAndReturn(kid, own.zonepos, own.own);
			m2.Angr += 2;
			m2.divineshild = true;
		}
		
	}
}
