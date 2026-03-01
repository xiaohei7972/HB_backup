using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：7 攻击力：5 生命值：5
	//Nellie, the Great Thresher
	//奈利，超巨蛇颈龙
	//[x]<b>Colossal +1</b><b>Battlecry:</b> <b>Discover</b> 3 Piratesto crew Nellie's Ship!
	//<b>巨型+1</b><b>战吼：</b><b>发现</b>三个海盗来构成奈利的船员团队！
	class Sim_TSC_660 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_660t);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos, m.own);
		}
		
	}
}
