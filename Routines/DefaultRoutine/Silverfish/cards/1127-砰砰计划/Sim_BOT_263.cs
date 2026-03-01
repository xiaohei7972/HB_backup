using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BOT_263 : SimTemplate //* 灵魂灌注 Soul Infusion
//Give theleft-most minion in your hand +2/+2.
//使你手牌中最左边的随从牌获得+2/+2。 
	{
		

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (ownplay)
            {
                foreach (Handmanager.Handcard handcard in p.owncards)
                {
					if (handcard.card.type == CardDB.cardtype.MOB)
					{
                        handcard.addattack += 2;;
                        handcard.addHp += 2;
                        p.anzOwnExtraAngrHp += 4;
					}   
                }                
            }
        }
	}
}