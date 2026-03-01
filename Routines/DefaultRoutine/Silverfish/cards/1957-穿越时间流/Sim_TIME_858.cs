using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 法师 费用：7 攻击力：5 生命值：5
	//Temporal Construct
	//时空构造体
	//[x]<b>Battlecry:</b> Deal 5 damageto an enemy minion.Draw cards equal to theexcess damage.
	//<b>战吼：</b>对一个敌方随从造成5点伤害，然后抽牌，数量等同于超过目标生命值的伤害。
	class Sim_TIME_858 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				p.minionGetDamageOrHeal(target, 5);
				if (target.Hp < 0)
				{
					for (int i = 0; i < (-target.Hp); i++)
					{
						p.drawACard(CardDB.cardIDEnum.None, own.own);

					}
				}
			}
		}
		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
			};
		}

	}
}
