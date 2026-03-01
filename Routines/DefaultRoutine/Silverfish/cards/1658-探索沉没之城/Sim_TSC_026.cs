using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：7 攻击力：6 生命值：5
	//Colaque
	//可拉克
	//[x]<b>Colossal +1</b> <b>Immune</b> while you controlColaque's Shell.
	//<b>巨型+1</b>当你控制可拉克的壳时<b>免疫</b>。
	class Sim_TSC_026 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_026t);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos, m.own);
		}

	}
}
