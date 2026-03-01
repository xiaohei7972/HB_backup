
using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //随从 中立 费用：0 攻击力：0 生命值：0
	//Zilliax Deluxe 3000
	//奇利亚斯豪华版3000型
	//While building your deck,customize your very ownZilliax Deluxe 3000!
	//你可以在构筑套牌时打造专属于自己的奇利亚斯豪华版3000型！
    class Sim_TOY_330t5 : SimTemplate // 避免报错
    {
        CardDB.Card module1;
		CardDB.Card module2;
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			own.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.getBattlecryEffect(p,own,target,choice);
			module2.sim_card.getBattlecryEffect(p,own,target,choice);
		}

        public override void onAuraStarts(Playfield p, Minion m)
        {
            m.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.onAuraStarts(p,m);
			module2.sim_card.onAuraStarts(p,m);
        }
        public override void onAuraEnds(Playfield p, Minion m)
        {
            m.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.onAuraEnds(p,m);
			module2.sim_card.onAuraEnds(p,m);
        }
        public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            triggerEffectMinion.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.onTurnStartTrigger(p,triggerEffectMinion,turnStartOfOwner);
			module2.sim_card.onTurnStartTrigger(p,triggerEffectMinion,turnStartOfOwner);
        }

        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            triggerEffectMinion.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.onTurnStartTrigger(p,triggerEffectMinion,turnEndOfOwner);
			module2.sim_card.onTurnStartTrigger(p,triggerEffectMinion,turnEndOfOwner);
        }

        public override void onDeathrattle(Playfield p, Minion m)
        {
            m.handcard.card.getModularCard(out module1,out module2);
			module1.sim_card.onDeathrattle(p,m);
			module2.sim_card.onDeathrattle(p,m);

        }
    }
}
