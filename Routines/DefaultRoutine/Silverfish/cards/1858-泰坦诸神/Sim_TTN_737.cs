using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HREngine.Bots
{
    //随从 巫妖王 费用：8 攻击力：7 生命值：9
    //The Primus
    //兵主
    //<b>Titan</b>After this uses an ability, <b>Discover</b> a card with that Rune.
    //<b>泰坦</b>在本随从使用一个技能后，<b>发现</b>一张对应符文的牌。
    class Sim_TTN_737 : SimTemplate
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_737t2);
        public override void useTitanAbility(Playfield p, Minion triggerMinion, int titanAbilityNO, Minion target)
        {
            switch (titanAbilityNO)
            {
                case 1: // 鲜血符文
                    if (target != null)
                    {
                        p.minionGetDestroyed(target);
                        p.minionGetBuffed(triggerMinion, 0, target.Hp);
                        p.minionGetBuffed(triggerMinion.own ? p.ownHero : p.enemyHero, 0, target.Hp);

                        p.drawACard(CardDB.cardIDEnum.None, true, true);
                    }
                    break;

                case 2: // 邪恶符文

                    // 召唤2个3/3并具有嘲讽和复生的亡灵
                    p.callKid(kid, triggerMinion.zonepos - 1, true, false);
                    p.callKid(kid, triggerMinion.zonepos, true, false);

                    p.drawACard(CardDB.cardIDEnum.None, true, true);
                    break;

                case 3: // 冰霜符文
                    p.drawACard(CardDB.cardIDEnum.None, true, true);
                    break;

            }
        }
    }
}
