using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 潜行者 费用：7 攻击力：8 生命值：10
	//Blackwater Behemoth
	//黑水巨鳗
	//<b>Colossal +1</b><b>Lifesteal</b>
	//<b>巨型+1</b><b>吸血</b>
	class Sim_TSC_216 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_216t);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos - 1, m.own);
		}

	}
}
