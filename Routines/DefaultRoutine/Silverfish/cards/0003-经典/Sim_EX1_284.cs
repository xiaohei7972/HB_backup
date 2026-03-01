using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 碧蓝幼龙 Azure Drake
    //<b>Spell Damage +1</b><b>Battlecry:</b> Draw a card.
    //<b>法术伤害+1</b>，<b>战吼：</b>抽一张牌。 
    class Sim_EX1_284 : SimTemplate
    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            p.drawACard(CardDB.cardIDEnum.None, own.own);
        }
    }
}