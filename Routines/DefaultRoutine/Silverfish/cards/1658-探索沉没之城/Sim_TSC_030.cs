using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：7 攻击力：4 生命值：5
	//The Leviathan
	//海兽号
	//[x]<b>Colossal +1</b><b>Rush</b>, <b>Divine Shield</b>After this attacks,<b>Dredge</b>.
	//<b>巨型+1</b><b>突袭</b>，<b>圣盾</b>。在本随从攻击后，<b>探底</b>。
	class Sim_TSC_030 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_030t2);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos, m.own);
		}
		
	}
}
