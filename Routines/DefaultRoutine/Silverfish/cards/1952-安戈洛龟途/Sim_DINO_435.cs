using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：5 攻击力：3 生命值：4
	//Crater Experiment
	//环形山实验体
	//<b>Kindred:</b> Summon acopy of this.
	//<b>延系：</b>召唤一个本随从的复制。
	class Sim_DINO_435 : SimTemplate
	{
		public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (own.handcard.poweredUp > 0)
			{
				p.callKid(own.handcard.card, own.zonepos, own.own);

			}
		}

	}
}
