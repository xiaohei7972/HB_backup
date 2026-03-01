using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    // 随从 中立 费用：3 攻击力：2 生命值：4
    // Hozen Roughhouser
    // 粗暴的猢狲
    // Whenever another friendly Pirate attacks, give it +1/+1.
    // 每当其他友方海盗攻击时，使其获得+1/+1。
    class Sim_VAC_938 : SimTemplate
    {
        // 当友方随从攻击时触发此方法
        public override void onMinionAttack(Playfield p, Minion triggerEffectMinion, Minion attacker, Minion defender)
        {
            if (triggerEffectMinion.entitiyID != attacker.entitiyID && attacker.own && RaceUtils.IsRaceOrAll(attacker.handcard.card.race, CardDB.Race.PIRATE))
            {
                p.minionGetBuffed(attacker, 1, 1);
            }
        }
    }
}
