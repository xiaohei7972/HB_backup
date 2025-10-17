using System;
using System.Collections.Generic;
using System.Text;
using Triton.Game;
using Triton.Game.Mapping;
namespace HREngine.Bots
{
	//随从 中立 费用：5 攻击力：4 生命值：6
	//Carnivorous Cubicle
	//食肉格块
	//[x]<b>Battlecry:</b> Destroy afriendly minion.Summon a copy of it at the end of your turns.
	//<b>战吼：</b>消灭一个友方随从。在你的回合结束时，召唤一个它的复制。
	class Sim_WORK_042 : SimTemplate
	{
		 CardDB.Card kid = null;
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				kid = target.handcard.card;
				p.minionGetDestroyed(target);
				// own.handcard.card.TAG_SCRIPT_DATA_NUM_1 = target.entitiyID;
			}
		}

		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
		{

            //读游戏的方式
            /* if (triggerEffectMinion.own == turnEndOfOwner && triggerEffectMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1 != 0)
			{
				GameState gameState = GameState.Get();
				Entity entity = gameState.GetEntity(triggerEffectMinion.handcard.card.TAG_SCRIPT_DATA_NUM_1);
				CardDB.cardIDEnum cardIDEnum = CardDB.Instance.cardIdstringToEnum(entity.GetCardId());
				CardDB.Card kid = CardDB.Instance.getCardDataFromID(cardIDEnum);
				p.callKid(kid, triggerEffectMinion.zonepos, true);
			} */
            if (kid != null)
            {
                p.callKid(kid,triggerEffectMinion.zonepos,triggerEffectMinion.own);
            }
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY), // 需要选择一个目标
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET), // 目标只能是随从
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET), // 目标只能是友方
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE) // 没有目标时也可以使用

			};

		}
	}
}
