using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 萨满祭司 费用：7 攻击力：3 生命值：5
	//Glugg the Gulper
	//暴食巨鳗格拉格
	//[x]<b>Colossal +3</b> After a friendly minion dies,gain its original stats.
	//<b>巨型+3</b>在一个友方随从死亡后，获得其原始属性值。
	class Sim_TSC_639 : SimTemplate
	{
		CardDB.Card ColossalDerivative = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_639t);
		CardDB.Card ColossalDerivative1 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_639t2);
		CardDB.Card ColossalDerivative2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TSC_639t3);
		public override void SummonColossal(Playfield p, Minion m)
		{
			p.callKid(ColossalDerivative, m.zonepos, m.own);
			p.callKid(ColossalDerivative1, m.zonepos, m.own);
			p.callKid(ColossalDerivative2, m.zonepos, m.own);
		}

		public override void onMinionDiedTrigger(Playfield p, Minion triggerEffectMinion, Minion diedMinion)
		{
			p.minionGetBuffed(triggerEffectMinion, diedMinion.handcard.card.Attack, diedMinion.handcard.card.Health);
		}

	}
}
