using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Triton.Common.LogUtilities;

namespace HREngine.Bots
{
    public partial class Playfield
    {
        /// <summary>
        /// 根据奥秘更新目标。
        /// </summary>
        private Minion UpdateTargetBasedOnSecret(int newTarget, Minion target)
        {
            if (newTarget >= 1)
            {
                foreach (Minion m in this.ownMinions)
                {
                    if (m.entityID == newTarget)
                    {
                        return m;
                    }
                }
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entityID == newTarget)
                    {
                        return m;
                    }
                }
                if (this.ownHero.entityID == newTarget) return this.ownHero;
                if (this.enemyHero.entityID == newTarget) return this.enemyHero;
            }
            return target;
        }

        /// <summary>
        /// 处理法术反制或扰咒术的逻辑。
        /// </summary>
        private void HandleCounterspellOrSpellbender(CardDB.Card c, Minion target)
        {
            if (!target.own && (prozis.penman.attackBuffDatabase.ContainsKey(c.nameEN) || prozis.penman.healthBuffDatabase.ContainsKey(c.nameEN)))
            {
                // 可在此引入打分逻辑
            }
        }

        /// <summary>
        /// 敌方 - 根据奥秘更新目标。
        /// </summary>
        /// <param name="newTarget">奥秘可能更改后的新目标。</param>
        /// <param name="target">当前的目标。</param>
        /// <returns>更新后的目标。</returns>
        private Minion EnemyUpdateTargetBasedOnSecret(int newTarget, Minion target)
        {
            if (newTarget >= 1)
            {
                // 遍历己方和敌方随从列表，找到新的攻击目标
                foreach (Minion m in this.ownMinions)
                {
                    if (m.entityID == newTarget)
                    {
                        return m;
                    }
                }
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entityID == newTarget)
                    {
                        return m;
                    }
                }
                if (this.ownHero.entityID == newTarget) return this.ownHero;
                if (this.enemyHero.entityID == newTarget) return this.enemyHero;
            }
            return target;
        }

        /// <summary>
        /// 合并多个 SecretItem 对象，返回一个包含所有可能性的 SecretItem 对象。
        /// </summary>
        /// <param name="esl">敌方秘密列表。</param>
        /// <returns>合并后的 SecretItem 对象，如果列表为空则返回 null。</returns>
        private SecretItem getMergedSecretItem(List<SecretItem> esl)
        {
            if (esl == null || esl.Count == 0)
            {
                return null; // 如果列表为空或为 null，则返回 null
            }

            if (esl.Count == 1)
            {
                return esl[0]; // 如果列表中只有一个秘密，直接返回它
            }

            // 创建一个新的 BitArray，大小为 60，初始值为 false
            BitArray mergedData = new BitArray(60, false);

            // 遍历所有 SecretItem，将它们的位数据合并到 mergedData 中
            foreach (SecretItem si in esl)
            {
                mergedData.Or(SecretItem.secretItemToData(si)); // 使用按位或运算符合并位数据
            }

            // 将合并后的位数据转换回 SecretItem 对象并返回
            return SecretItem.dataToSecretItem(mergedData);
        }

        /// <summary>
        /// 处理当角色被攻击时可能触发的敌方奥秘，返回新目标的实体 ID（如果奥秘改变了目标）。
        /// </summary>
        /// <param name="attacker">攻击者。</param>
        /// <param name="defender">防御者。</param>
        /// <returns>新的攻击目标实体 ID，如果没有变化则返回 0。</returns>
        public int secretTrigger_CharIsAttacked(Minion attacker, Minion defender)
        {
            int newTarget = 0; // 用于存储如果奥秘改变攻击目标时的新目标实体 ID
            int triggered = 0; // 记录触发的奥秘数量

            if (this.isOwnTurn && this.enemySecretCount >= 1)
            {
                if (this.enemySecretList.Count == 0)
                {
                    Helpfunctions.Instance.logg("错误：敌方奥秘列表为空，但敌方奥秘数量是" + this.enemySecretCount);
                    if (this.enemyHeroName == HeroEnum.mage)
                    {
                        this.enemySecretList.Add(new SecretItem()); // 添加一个默认的 SecretItem 防止出错
                    }
                }

                if (defender.isHero && !defender.own) // 当防御者是敌方英雄
                {
                    foreach (SecretItem si in this.enemySecretList)
                    {
                        this.evaluatePenality += Ai.Instance.botBase.getSecretPen_CharIsAttacked(this, si, attacker, defender);
                        bool needDamageTrigger = false;

                        if (si.canBe_explosive)  // 爆炸陷阱
                        {
                            triggered++;
                            si.canBe_explosive = false;
                            needDamageTrigger = true;
                        }

                        if (si.canBe_beartrap)  // 熊陷阱
                        {
                            triggered++;
                            si.canBe_beartrap = false;
                            CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_060).sim_card.onSecretPlay(this, false, 0);
                            needDamageTrigger = true;
                        }

                        if (attacker != null && !attacker.isHero && si.canBe_vaporize)  // 气化
                        {
                            triggered++;
                            si.canBe_vaporize = false;
                            CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_594).sim_card.onSecretPlay(this, false, attacker, 0);
                            needDamageTrigger = true;
                        }

                        if (si.canBe_missdirection)  // 误导
                        {
                            if (!(attacker.isHero && this.ownMinions.Count + this.enemyMinions.Count == 0))
                            {
                                triggered++;
                                si.canBe_missdirection = false;
                                CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_533).sim_card.onSecretPlay(this, false, attacker, defender, out newTarget);
                            }
                        }

                        if (si.canBe_icebarrier)  // 冰甲
                        {
                            triggered++;
                            si.canBe_icebarrier = false;
                            CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_289).sim_card.onSecretPlay(this, false, defender, 0);
                        }

                        if (needDamageTrigger)
                        {
                            this.doDmgTriggers(); // 触发伤害后的效果
                        }
                    }
                }

                if (!defender.isHero && !defender.own) // 当防御者是敌方随从
                {
                    foreach (SecretItem si in this.enemySecretList)
                    {
                        if (si.canBe_snaketrap)  // 毒蛇陷阱
                        {
                            triggered++;
                            si.canBe_snaketrap = false;
                            CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_554).sim_card.onSecretPlay(this, false, 0);
                            this.doDmgTriggers();
                        }
                    }
                }

                if (attacker != null && !attacker.isHero && attacker.own) // 当攻击者是我方随从
                {
                    foreach (SecretItem si in this.enemySecretList)
                    {
                        if (si.canBe_freezing)  // 冰冻陷阱
                        {
                            triggered++;
                            si.canBe_freezing = false;
                            CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_611).sim_card.onSecretPlay(this, false, attacker, 0);
                        }
                    }
                }

                foreach (SecretItem si in this.enemySecretList)
                {
                    if (si.canBe_noblesacrifice)  // 英勇牺牲
                    {
                        triggered++;
                        si.canBe_noblesacrifice = false;
                    }
                }
            }

            return newTarget; // 返回新的攻击目标实体 ID
        }

        /// <summary>
        /// 当英雄受到伤害时，触发敌方奥秘。
        /// </summary>
        /// <param name="own">是否是我方英雄。</param>
        /// <param name="dmg">受到的伤害值。</param>
        public void secretTrigger_HeroGotDmg(bool own, int dmg)
        {
            int triggered = 0; // 记录触发的奥秘数量

            // 检查是否是敌方回合且有敌方奥秘存在
            if (own != this.isOwnTurn)
            {
                if (this.isOwnTurn && this.enemySecretCount >= 1)
                {
                    SecretItem si = getMergedSecretItem(this.enemySecretList);

                    // 添加策略的惩罚值
                    this.evaluatePenality += Ai.Instance.botBase.getSecretPen_HeroGotDmg(this, si, own, dmg);

                    // 复仇之眼 - 当英雄受到伤害时，对攻击者造成等量伤害
                    if (si.canBe_eyeforaneye)
                    {
                        triggered++;
                        foreach (SecretItem sii in this.enemySecretList)
                        {
                            sii.canBe_eyeforaneye = false;
                        }
                        CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_132).sim_card.onSecretPlay(this, false, dmg);
                    }

                    // 寒冰屏障 - 当英雄的生命值降到0或以下时，触发免死效果
                    if (si.canBe_iceblock && this.enemyHero.Hp <= 0)
                    {
                        triggered++;
                        foreach (SecretItem sii in this.enemySecretList)
                        {
                            sii.canBe_iceblock = false;
                        }
                        // 这里没有实际调用奥秘效果，只是将其标记为已触发
                        //CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_295).sim_card.onSecretPlay(this, false, this.enemyHero, dmg);
                    }
                }
            }

            // 第一个回合时减少触发奥秘的惩罚值（具体数值根据实际策略而定，暂时不启用）
            if (turnCounter == 0)
            {
                //this.evaluatePenality -= triggered * 50; // Todo:不引入打分
            }
        }

        /// <summary>
        /// 当一个随从被打出时，触发敌方的奥秘。
        /// </summary>
        /// <param name="playedMinion">被打出的随从。</param>
        public void secretTrigger_MinionIsPlayed(Minion playedMinion)
        {
            int triggered = 0; // 记录触发的奥秘数量

            // 检查是否是我方回合并且敌方有奥秘
            if (this.isOwnTurn && playedMinion.own && this.enemySecretCount >= 1)
            {
                SecretItem si = getMergedSecretItem(enemySecretList);

                // 添加策略的惩罚值
                this.evaluatePenality += Ai.Instance.botBase.getSecretPen_MinionIsPlayed(this, si, playedMinion);

                bool needDamageTrigger = false;

                // 魔镜实体 - 复制打出的随从
                if (si.canBe_mirrorentity)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_mirrorentity = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_294).sim_card.onSecretPlay(this, false, playedMinion, 0);
                    needDamageTrigger = true;
                }

                // 悔改 - 将打出的随从的生命值降为1
                if (si.canBe_repentance)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_repentance = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_379).sim_card.onSecretPlay(this, false, playedMinion, 0);
                }

                // 神圣试炼 - 当场上有超过3个随从时，摧毁新打出的随从
                if (si.canBe_sacredtrial && this.ownMinions.Count > 3)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_sacredtrial = false;
                        sii.canBe_snipe = false; // 防止和狙击重叠
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOE_027).sim_card.onSecretPlay(this, false, playedMinion, 0);
                    needDamageTrigger = true;
                }

                // 如果需要触发伤害事件，调用触发伤害的方法
                if (needDamageTrigger) doDmgTriggers();
            }

            // 第一个回合时减少触发奥秘的惩罚值（具体数值根据实际策略而定，暂时不启用）
            if (turnCounter == 0)
            {
                //this.evaluatePenality -= triggered * 50; //Todo: 不引入打分
            }
        }

        /// <summary>
        /// 当一个法术被施放时，触发敌方的奥秘。
        /// </summary>
        /// <param name="target">法术的目标随从。</param>
        /// <param name="c">被施放的法术。</param>
        /// <returns>返回新的目标实体ID，如果没有则返回0。</returns>
        public int secretTrigger_SpellIsPlayed(Minion target, CardDB.Card c)
        {
            int triggered = 0; // 记录触发的奥秘数量
            int retval = 0; // 新的目标实体ID

            // 检查是否是我方回合、法术牌、敌方有奥秘
            if (this.isOwnTurn && c.type == CardDB.cardtype.SPELL && this.enemySecretCount > 0)
            {
                SecretItem si = getMergedSecretItem(enemySecretList);

                // 添加策略的惩罚值
                this.evaluatePenality += Ai.Instance.botBase.getSecretPen_SpellIsPlayed(this, si, target, c);

                // 猫戏法 - 召唤一只4/2的猎豹
                if (si.canBe_cattrick)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_cattrick = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.KAR_004).sim_card.onSecretPlay(this, false, 0);
                    doDmgTriggers();
                }

                // 扰咒术 - 使目标转移到一个新的随从
                if (si.canBe_spellbender && target != null && !target.isHero)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_spellbender = false;
                    }
                    if (!(target.own && prozis.penman.maycauseharmDatabase.ContainsKey(c.nameEN)))
                    {
                        CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.tt_010).sim_card.onSecretPlay(this, false, null, target, out retval);
                    }
                }
            }

            // 在第一个回合时减少触发奥秘的惩罚值（具体数值根据实际策略而定，暂时不启用）
            if (turnCounter == 0)
            {
                //this.evaluatePenality -= triggered * 50; //Todo: 不引入打分
            }

            return retval; // 返回新的目标实体ID，如果没有则返回0
        }

        /// <summary>
        /// 当一个随从死亡时，触发敌方的奥秘。
        /// </summary>
        /// <param name="own">表示随从是否属于己方。</param>
        public void secretTrigger_MinionDied(bool own)
        {
            int triggered = 0; // 记录触发的奥秘数量

            // 检查是否是我方回合，且敌方有奥秘
            if (this.isOwnTurn && !own && this.enemySecretCount >= 1)
            {
                SecretItem si = getMergedSecretItem(enemySecretList);

                // 添加策略的惩罚值
                this.evaluatePenality += Ai.Instance.botBase.getSecretPen_MinionDied(this, si, own);

                // 重生奥秘：复制随从卡牌到对方手牌
                if (si.canBe_duplicate)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_duplicate = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.FP1_018).sim_card.onSecretPlay(this, false, 0);
                }

                // 复生奥秘：将随从复活为1点生命值
                if (si.canBe_redemption)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_redemption = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_136).sim_card.onSecretPlay(this, false, 0);
                }

                // 复仇奥秘：随机为另一个随从+3/+2
                if (si.canBe_avenge)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_avenge = false;
                    }
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.FP1_020).sim_card.onSecretPlay(this, false, 0);
                }
            }

            // 在第一个回合时减少触发奥秘的惩罚值（具体数值根据实际策略而定，暂时不启用）
            if (turnCounter == 0)
            {
                //this.evaluatePenality -= triggered * 50; //触发奥秘惩罚值 50 //Todo: 不引入打分
            }
        }

        /// <summary>
        /// 当英雄技能被使用时，触发敌方的奥秘。
        /// </summary>
        public void secretTrigger_HeroPowerUsed()
        {
            int triggered = 0; // 记录触发的奥秘数量

            // 检查是否为己方回合且敌方有奥秘
            if (this.isOwnTurn && this.enemySecretCount >= 1)
            {
                // 合并敌方所有奥秘为一个对象，便于统一处理
                SecretItem si = getMergedSecretItem(enemySecretList);

                // 添加策略的惩罚值
                this.evaluatePenality += Ai.Instance.botBase.getSecretPen_HeroPowerUsed(this, si);

                // 检查并处理飞镖陷阱奥秘
                if (si.canBe_darttrap)
                {
                    triggered++;
                    foreach (SecretItem sii in this.enemySecretList)
                    {
                        sii.canBe_darttrap = false; // 标记奥秘为不可再触发
                    }
                    // 执行飞镖陷阱的效果
                    CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOE_021).sim_card.onSecretPlay(this, false, 0);
                    doDmgTriggers(); // 触发可能的伤害效果
                }
            }

            // 在第一个回合时减少触发奥秘的惩罚值（具体数值根据实际策略而定，暂时不启用）
            if (turnCounter == 0)
            {
                //this.evaluatePenality -= triggered * 50; //Todo: 不引入打分
            }
        }

        /// <summary>
        /// 根据事件类型检测并返回可能触发的敌方奥秘数量。
        /// </summary>
        /// <param name="type">事件类型，0-随从被打出，1-法术被使用，2-角色被攻击，3-英雄受伤，4-随从死亡，5-英雄技能被使用。</param>
        /// <param name="actedMinionOwn">行动的随从是否为己方。</param>
        /// <param name="actedMinionIsHero">行动的随从是否为英雄。</param>
        /// <param name="target">目标随从。</param>
        /// <returns>可能触发的奥秘数量。</returns>
        public int getSecretTriggersByType(int type, bool actedMinionOwn, bool actedMinionIsHero, Minion target)
        {
            int triggered = 0;
            bool isSpell = false; // 是否为法术类型的事件

            switch (type)
            {
                case 0: // 随从被打出
                    if (this.isOwnTurn && actedMinionOwn && this.enemySecretCount >= 1)
                    {
                        bool canBe_mirrorentity = false;
                        bool canBe_repentance = false;
                        bool canBe_sacredtrial = false;
                        bool canBe_snipe = false;
                        foreach (SecretItem si in this.enemySecretList.ToArray())
                        {
                            if (si.canBe_mirrorentity && !canBe_mirrorentity) { canBe_mirrorentity = true; triggered++; }
                            if (si.canBe_repentance && !canBe_repentance) { canBe_repentance = true; triggered++; }
                            if (si.canBe_sacredtrial && this.ownMinions.Count > 3 && !canBe_sacredtrial) { canBe_sacredtrial = true; canBe_snipe = true; triggered++; }
                            else if (si.canBe_snipe && !canBe_snipe) { canBe_snipe = true; triggered++; }
                        }
                    }
                    break;

                case 1: // 法术被使用
                    if (this.isOwnTurn && isSpell && this.enemySecretCount >= 1)
                    {
                        bool canBe_counterspell = false;
                        bool canBe_spellbender = false;
                        bool canBe_cattrick = false;
                        foreach (SecretItem si in this.enemySecretList)
                        {
                            if (si.canBe_counterspell && !canBe_counterspell) return 1; // 如果是反制法术，直接返回1
                            if (si.canBe_spellbender && target != null && !target.isHero && !canBe_spellbender) { canBe_spellbender = true; triggered++; }
                            if (si.canBe_cattrick && !canBe_cattrick) { canBe_cattrick = true; triggered++; }
                        }
                    }
                    break;

                case 2: // 角色被攻击
                    if (this.isOwnTurn && this.enemySecretCount >= 1)
                    {
                        if (target.isHero && !target.own)
                        {
                            bool canBe_explosive = false;
                            bool canBe_flameward = false;
                            bool canBe_beartrap = false;
                            bool canBe_vaporize = false;
                            bool canBe_missdirection = false;
                            bool canBe_icebarrier = false;
                            foreach (SecretItem si in this.enemySecretList)
                            {
                                if (si.canBe_explosive && !canBe_explosive) { canBe_explosive = true; triggered++; }
                                if (si.canBe_flameward && !canBe_flameward) { canBe_flameward = true; triggered++; }
                                if (si.canBe_beartrap && !canBe_beartrap) { canBe_beartrap = true; triggered++; }
                                if (!actedMinionIsHero && si.canBe_vaporize && !canBe_vaporize) { canBe_vaporize = true; triggered++; }
                                if (si.canBe_missdirection && !canBe_missdirection)
                                {
                                    if (!(actedMinionIsHero && this.ownMinions.Count + this.enemyMinions.Count == 0))
                                    {
                                        canBe_missdirection = true; triggered++;
                                    }
                                }
                                if (si.canBe_icebarrier && !canBe_icebarrier) { canBe_icebarrier = true; triggered++; }
                            }
                        }

                        if (!target.isHero && !target.own)
                        {
                            bool canBe_snaketrap = false;
                            foreach (SecretItem si in this.enemySecretList)
                            {
                                if (si.canBe_snaketrap && !canBe_snaketrap) { canBe_snaketrap = true; triggered++; }
                            }
                        }

                        if (!actedMinionIsHero && actedMinionOwn) // 随从攻击
                        {
                            bool canBe_freezing = false;
                            foreach (SecretItem si in this.enemySecretList)
                            {
                                if (si.canBe_freezing && !canBe_freezing) { canBe_freezing = true; triggered++; }
                            }
                        }

                        bool canBe_noblesacrifice = false;
                        foreach (SecretItem si in this.enemySecretList)
                        {
                            if (si.canBe_noblesacrifice && !canBe_noblesacrifice) { canBe_noblesacrifice = true; triggered++; }
                        }
                    }
                    break;

                case 3: // 英雄受伤
                    if (target.own != this.isOwnTurn)
                    {
                        if (this.isOwnTurn && this.enemySecretCount >= 1)
                        {
                            bool canBe_eyeforaneye = false;
                            bool canBe_iceblock = false;
                            foreach (SecretItem si in this.enemySecretList)
                            {
                                if (si.canBe_eyeforaneye && !canBe_eyeforaneye) { canBe_eyeforaneye = true; triggered++; }
                                if (si.canBe_iceblock && this.enemyHero.Hp <= 0 && !canBe_iceblock) { canBe_iceblock = true; triggered++; }
                            }
                        }
                    }
                    break;

                case 4: // 随从死亡
                    if (this.isOwnTurn && !target.own && this.enemySecretCount >= 1)
                    {
                        bool canBe_duplicate = false;
                        bool canBe_redemption = false;
                        bool canBe_avenge = false;
                        foreach (SecretItem si in this.enemySecretList)
                        {
                            if (si.canBe_duplicate && !canBe_duplicate) { canBe_duplicate = true; triggered++; }
                            if (si.canBe_redemption && !canBe_redemption) { canBe_redemption = true; triggered++; }
                            if (si.canBe_avenge && !canBe_avenge) { canBe_avenge = true; triggered++; }
                        }
                    }
                    break;

                case 5: // 英雄技能被使用
                    if (this.isOwnTurn && this.enemySecretCount >= 1)
                    {
                        bool canBe_darttrap = false;
                        foreach (SecretItem si in this.enemySecretList)
                        {
                            if (si.canBe_darttrap && !canBe_darttrap) { canBe_darttrap = true; triggered++; }
                        }
                    }
                    break;
            }

            return triggered;
        }
    }
}
