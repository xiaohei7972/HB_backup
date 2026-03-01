using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //* 恐龙学 Dinomancy
    //Your Hero Power becomes 'Give a Beast +2/+2.'
    //你的英雄技能变成“使一个野兽获得+3/+3”。 
    class Sim_UNG_917 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            p.setNewHeroPower(CardDB.cardIDEnum.UNG_917t1, ownplay);
        }
    }
}