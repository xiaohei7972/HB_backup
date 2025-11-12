using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    //随从 战士 费用：9 攻击力：6 生命值：9
    //Muensterosity
    //芝士怪物
    //[x]<b>Taunt</b>. At the end ofyour turn, summon anElemental with stats__equal to this minion's.
    //<b>嘲讽</b>在你的回合结束时，召唤一个属性值等同于本随从的元素。
    class Sim_VAC_339 : SimTemplate
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_339t);
        public override void onTurnEndsTrigger(Playfield p, Minion m, bool turnEndOfOwner)
        {
            // 只在随从所有者的回合结束时触发
            if (turnEndOfOwner == m.own)
            {
                Minion kid1 = p.callKidAndReturn(kid, m.zonepos, m.own);
                if (kid1 != null)
                {

                    kid1.Angr = m.Angr;
                    kid1.Hp = m.Hp;
                    kid1.maxHp = m.Hp;
                    // kid1.cost = Math.Min(10, (m.Hp + m.Angr) / 2);
                }
            }

        }
    }
}



