using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 中立 费用：1
	//Amulet of Strides
	//挺进护符
	//[x]Reduce the Cost of allcards in your hand by (1).<i>(Except for spells!)</i>
	//使你手牌中的所有卡牌的法力值消耗减少（1）点。<i>（法术牌除外！）</i>
	class Sim_VAC_959t10 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            // 使你手牌中的所有卡牌的法力值消耗减少1点
            foreach (Handmanager.Handcard handcard in p.owncards)
            {
				if (handcard.card.type != CardDB.cardtype.SPELL)
				{
					handcard.manacost = Math.Max(0, handcard.manacost - 1);
				}            
            }
        }
	}
}
