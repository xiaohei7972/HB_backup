using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 西风灯神 Djinni of Zephyrs
    //After you cast a spell on another friendly minion, cast a copy of it on this one.
    //在你对一个其他友方随从施放法术后，将法术效果复制在此随从身上。 
    class Sim_LOE_053 : SimTemplate
    {
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (hc.card.type == CardDB.cardtype.SPELL && hc.target != null && hc.target.own == wasOwnCard)
            {
                if (hc.target.own == triggerEffectMinion.own && hc.target.entitiyID != triggerEffectMinion.entitiyID)
                {
                    hc.card.sim_card.onCardPlay(p, wasOwnCard, triggerEffectMinion, hc.extraParam2, hc);
                }
            }
        }
    }
}