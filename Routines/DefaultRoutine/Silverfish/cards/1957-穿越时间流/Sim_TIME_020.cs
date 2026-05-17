using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 恶魔猎手 费用：2 攻击力：12 生命值：12
	//Broxigar
	//布洛克斯加
	//[x]<b>Fabled, Charge</b><b>Start of Game:</b> Disappear.Kill all 4 Demons from Argusto reappear in hand.
	//<b>奇闻冲锋。</b><b>对战开始时：</b>消失。消灭全部4个来自阿古斯的恶魔以重新出现在[x]手牌中。
	class Sim_TIME_020 : SimTemplate
	{
		private static int demonsKilled = 0;

		public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
		{
			if (triggerEffectMinion.own && demonsKilled >= 4)
			{
				p.drawACard(CardDB.cardIDEnum.TIME_020, triggerEffectMinion.own, true);
				demonsKilled = 0;
			}
		}

		public override void onMinionDiedTrigger(Playfield p, Minion triggerEffectMinion, Minion diedMinion)
		{
			if (diedMinion.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_020t2t ||
				diedMinion.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_020t3t ||
				diedMinion.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_020t4t ||
				diedMinion.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_020t5t)
			{
				demonsKilled++;
			}
		}
	}
}
