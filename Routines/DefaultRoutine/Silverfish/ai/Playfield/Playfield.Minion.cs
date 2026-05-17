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
    // 随从管理：随从创建/放置/状态修改
    public partial class Playfield
    {
        public Minion createNewMinion(Handmanager.Handcard hc, int zonepos, bool own)
        {
            // 初始化随从对象并复制手牌信息
            Minion m = new Minion
            {
                handcard = new Handmanager.Handcard(hc),
                own = own,
                isHero = false, //是否为英雄
                entityID = hc.entity,  //实体id
                playedThisTurn = true,  //从手牌打出
                numAttacksThisTurn = 0, //这回合攻击过几次
                extraAttacksThisTurn = 0,//这回合额外攻击次数
                zonepos = zonepos,  //位置
                name = hc.card.nameEN,  //英文名
                nameCN = hc.card.nameCN,    //中文名

                divineShield = hc.card.Shield,   //圣盾
                windfury = hc.card.windfury,    //风怒
                rush = hc.card.Rush ? 1 : 0,    //突袭
                lifesteal = hc.card.lifesteal,  //吸血
                poisonous = hc.card.poisonous,  //剧毒
                stealth = hc.card.Stealth,  //潜行
                reborn = hc.card.reborn,    //复生
                taunt = hc.card.tank,   //嘲讽

                charge = hc.card.Charge ? 1 : 0,
                Spellburst = hc.card.Spellburst, // 法术迸发
                dormant = hc.card.dormant,  //休眠
                untouchable = hc.card.dormant > 0 || hc.card.untouchable,   //不可接触
            };

            if (hc.MODULAR_ENTITY_PART_1 != 0 && hc.MODULAR_ENTITY_PART_2 != 0)
            {

            }

            // 如果己方所有元素随从具有吸血效果
            if (this.prozis.ownElementalsHaveLifesteal > 0 && (TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL)
            {
                m.lifesteal = true;
            }

            // 根据水晶核心状态设置随从攻击力和生命值
            if (this.ownCrystalCore > 0)
            {
                m.Angr = m.Hp = m.maxHp = ownCrystalCore;
            }
            else
            {
                m.Angr = hc.card.Attack + hc.addattack;
                m.Hp = hc.card.Health + hc.addHp;
                m.maxHp = hc.card.Health;
            }

            // 重置手牌的额外攻击力和生命值加成
            hc.addattack = 0;
            hc.addHp = 0;

            // 特殊随从效果处理
            // if (m.name == CardDB.cardNameEN.lightspawn)
            // {
            //     m.Angr = m.Hp;
            // }

            // 更新随从是否可以立即攻击
            m.updateReadyness();

            // 计算种族优先级，影响评分
            // m.synergy = own ?
            //     prozis.penman.getClassRacePriorityPenality(this.ownHeroStartClass, (TAG_RACE)m.handcard.card.race) :
            //     prozis.penman.getClassRacePriorityPenality(this.enemyHeroStartClass, (TAG_RACE)m.handcard.card.race);

            if (m.synergy > 0 && hc.card.Stealth)
            {
                m.synergy++;
            }

            // 触发召唤时的效果
            this.triggerAMinionIsSummoned(m);

            // 激活光环效果
            m.handcard.card.sim_card.onAuraStarts(this, m);

            // 应用区域增益效果
            this.minionGetOrEraseAllAreaBuffs(m, true);

            return m;
        }

        /// <summary>
        /// 将随从放置到战场上的指定位置，并处理战吼、触发效果及其他相关逻辑。
        /// </summary>
        /// <param name="hc">手牌信息</param>
        /// <param name="choice">选择效果（如果有）</param>
        /// <param name="zonepos">随从放置位置</param>
        public void placeAmobSomewhere(Handmanager.Handcard hc, int choice, int zonepos)
        {
            // 创建一个新的随从并设置其为手牌中打出
            Minion m = createNewMinion(hc, zonepos, true);
            // m.playedFromHand = true;

            // 处理"空降歹徒"的效果（如果当前随从是海盗，则尝试召唤）
            CardDB.Card parachuteBrigand = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_056);
            if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE)
            {
                foreach (Handmanager.Handcard handcard in this.owncards.ToArray())
                {
                    if (handcard.card.cardIDenum == CardDB.cardIDEnum.DRG_056 && this.ownMinions.Count < 7)
                    {
                        this.callKid(parachuteBrigand, zonepos, true);
                        this.removeCard(handcard);
                        break;
                    }
                }
            }

            // 处理"海盗帕奇斯"的效果（如果当前随从是海盗且帕奇斯在牌库中，则召唤它）
            CardDB.Card patches = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CFM_637);
            if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE && this.prozis.patchesInDeck && this.ownMinions.Count < 7)
            {
                this.callKid(patches, zonepos, true);
                this.prozis.patchesInDeck = false;

                // 从牌库中移除帕奇斯
                foreach (KeyValuePair<CardDB.cardIDEnum, int> dc in this.prozis.turnDeck.ToArray())
                {
                    if (dc.Key == CardDB.cardIDEnum.CFM_637)
                    {
                        this.prozis.turnDeck.Remove(dc.Key);
                        this.ownDeckSize--;
                        break;
                    }
                }
            }

            // 将随从添加到战场
            addMinionToBattlefield(m);

            // 触发随从的打出效果、抉择
            m.handcard.card.sim_card.onCardPlay(this, m, hc.target, choice, hc);
            // 触发随从的战吼效果
            m.handcard.card.sim_card.getBattlecryEffect(this, m, hc.target, choice);

            // 如果随从处于流放位置（手牌的最左或最右），触发流放效果
            if (hc.position == 1 || hc.position == this.owncards.Count)
            {
                m.handcard.card.sim_card.onOutcast(this, m);
            }

            // 处理铜须的双倍战吼效果
            if (this.ownBrannBronzebeard > 0)
            {
                for (int i = 0; i < this.ownBrannBronzebeard; i++)
                {
                    m.handcard.card.sim_card.getBattlecryEffect(this, m, hc.target, choice);
                }
            }

            // 触发伤害效果
            doDmgTriggers();

            // 触发敌方奥秘
            secretTrigger_MinionIsPlayed(m);

            //TODO:随从卡牌打出后触发的效果,给术士游客用的
            foreach (Minion triggerEffectMinion in this.ownMinions.ToArray())
            {
                // 如果随从没被沉默,则触发打出随从后的方法
                if (!triggerEffectMinion.silenced)
                {
                    // 调用随从卡牌的打出随从后触发方法
                    triggerEffectMinion.handcard.card.sim_card.AfterMinionPlayed(this, m, m.own, triggerEffectMinion);
                }
            }

            foreach (Minion triggerEffectMinion in this.enemyMinions.ToArray())
            {
                // 如果随从没被沉默,则触发打出随从后的方法
                if (!triggerEffectMinion.silenced)
                {
                    // 调用随从卡牌的打出随从后触发方法
                    triggerEffectMinion.handcard.card.sim_card.AfterMinionPlayed(this, m, m.own, triggerEffectMinion);
                }
            }
            // 处理任务进度
            if (this.ownQuest.Id != CardDB.cardIDEnum.None)
            {
                ownQuest.trigger_MinionWasPlayed(m);
                // 如果任务完成，执行奖励效果
                if (ownQuest.maxProgress <= ownQuest.questProgress)
                {
                    switch (this.ownQuest.Id)
                    {
                        case CardDB.cardIDEnum.SW_028:
                            // 寻找并抽取武器牌
                            foreach (KeyValuePair<CardDB.cardIDEnum, int> kvp in this.prozis.turnDeck)
                            {
                                CardDB.Card card = CardDB.Instance.getCardDataFromID(kvp.Key);
                                if (card.type == CardDB.cardtype.WEAPON)
                                {
                                    this.drawACard(kvp.Key, true, true);
                                    break;
                                }
                            }
                            // 更新任务为下一阶段
                            ownQuest = new Questmanager.QuestItem() { Id = CardDB.cardIDEnum.SW_028t, questProgress = 0, maxProgress = 2 };
                            break;

                        case CardDB.cardIDEnum.SW_028t:
                            // 对敌方角色造成2点伤害
                            minionGetDamageOrHeal(getEnemyCharTargetForRandomSingleDamage(2), 2, true);
                            // 更新任务为下一阶段
                            ownQuest = new Questmanager.QuestItem() { Id = CardDB.cardIDEnum.SW_028t2, questProgress = 0, maxProgress = 2 };
                            break;

                        case CardDB.cardIDEnum.SW_028t2:
                            // 抽取任务完成奖励牌
                            drawACard(CardDB.cardIDEnum.SW_028t5, true, true);
                            // 如果当前法力水晶为4，减少评估惩罚
                            if (this.ownMaxMana == 4) evaluatePenality -= 20;
                            // 重置任务
                            ownQuest.Reset();
                            break;
                    }
                }
            }

            // 记录日志信息
            if (logging) Helpfunctions.Instance.logg("added " + m.handcard.card.nameEN);
        }

        /// <summary>
        /// 将随从添加到战场，并处理相关触发效果。
        /// </summary>
        /// <param name="m">要添加的随从对象</param>
        /// <param name="isSummon">是否为召唤随从，默认为 true</param>
        public void addMinionToBattlefield(Minion m, bool isSummon = true)
        {
            // 根据随从所属，获取对应的随从列表
            List<Minion> temp = m.own ? this.ownMinions : this.enemyMinions;

            // 将随从插入到指定的位置，如果位置不合法则添加到列表末尾
            if (m.zonepos >= 1 && m.zonepos <= temp.Count)
            {
                temp.Insert(m.zonepos - 1, m);
            }
            else
            {
                temp.Add(m);
            }

            // 更新触发器，标记随从列表发生了变化
            if (m.own)
            {
                this.tempTrigger.ownMinionsChanged = true;
                if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.PET))
                // if (m.handcard.card.race == CardDB.Race.PET || m.handcard.card.race == CardDB.Race.ALL)
                {
                    this.tempTrigger.ownBeastSummoned++;
                }

                if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.DRAGON))
                // if (m.handcard.card.race == CardDB.Race.DRAGON || m.handcard.card.race == CardDB.Race.ALL)
                {
                    this.tempTrigger.ownDragonSummoned++;

                }
                if (m.handcard.card.Treant)
                {
                    this.tempTrigger.ownTreantSummoned++;
                }
            }
            else
            {
                this.tempTrigger.enemyMininsChanged = true;
            }

            // 触发随从被召唤的事件和奥秘
            triggerAMinionWasSummoned(m);
            doDmgTriggers();

            // 更新随从的准备状态
            m.updateReadyness();
        }

        /// <summary>
        /// 在战场上召唤一个随从，如果空间允许的话。
        /// </summary>
        /// <param name="c">要召唤的随从卡牌</param>
        /// <param name="zonepos">召唤随从的位置</param>
        /// <param name="own">是否为己方</param>
        /// <param name="spawnKid">是否生成随从</param>
        /// <param name="oneMoreIsAllowed">是否允许额外的随从</param>
        public void callKid(CardDB.Card c, int zonepos, bool own, bool spawnKid = true, bool oneMoreIsAllowed = false)
        {
            // 默认允许的最大随从数量为7，如果允许额外的随从，则加1
            int allowed = 7 + (oneMoreIsAllowed ? 1 : 0);

            // 检查己方随从数量是否已达上限
            if (own)
            {
                if (this.ownMinions.Count >= allowed)
                {
                    // 如果随从数量已达上限，则不再召唤新的随从
                    return;
                }
            }
            else
            {
                // 检查敌方随从数量是否已达上限
                if (this.enemyMinions.Count >= allowed)
                {
                    // 如果随从数量已达上限，则不再召唤新的随从
                    return;
                }
            }

            // 确定随从的位置（位置索引从1开始）
            int mobplace = zonepos + 1;

            // 创建随从并触发相关效果
            Handmanager.Handcard hc = new Handmanager.Handcard(c) { entity = this.getNextEntity() };
            Minion m = createNewMinion(hc, mobplace, own);

            if (own && this.ownLegionInvasion && m.handcard.card.cardIDenum == CardDB.cardIDEnum.TTN_960t5)
            {
                m.Hp += 2;
                m.taunt = true;
            }
            else if (!own && this.enemyAbilityReady && m.handcard.card.cardIDenum == CardDB.cardIDEnum.TTN_960t5)
            {
                m.Hp += 2;
                m.taunt = true;
            }

            // 将随从放置到战场上并触发相关效果
            addMinionToBattlefield(m);
        }

        /// <summary>
        /// 在战场上召唤一个随从，如果空间允许的话。
        /// </summary>
        /// <param name="c">要召唤的随从卡牌</param>
        /// <param name="zonepos">召唤随从的位置</param>
        /// <param name="own">是否为己方</param>
        /// <param name="spawnKid">是否生成随从</param>
        /// <param name="oneMoreIsAllowed">是否允许额外的随从</param>
        /// <returns>召唤的随从,场面满了则返回null</returns>
        public Minion callKidAndReturn(CardDB.Card c, int zonepos, bool own, bool spawnKid = false, bool oneMoreIsAllowed = false)
        {
            // 默认允许的最大随从数量为7，如果允许额外的随从，则加1
            int allowed = 7 + (oneMoreIsAllowed ? 1 : 0);

            // 检查己方随从数量是否已达上限
            if (own)
            {
                if (this.ownMinions.Count >= allowed)
                {
                    // 如果随从数量已达上限，则不再召唤新的随从
                    return null;
                }
            }
            else
            {
                // 检查敌方随从数量是否已达上限
                if (this.enemyMinions.Count >= allowed)
                {
                    // 如果随从数量已达上限，则不再召唤新的随从
                    return null;
                }
            }

            // 确定随从的位置（位置索引从1开始）
            int mobplace = zonepos + 1;

            // 创建随从并触发相关效果
            Handmanager.Handcard hc = new Handmanager.Handcard(c) { entity = this.getNextEntity() };
            Minion m = createNewMinion(hc, mobplace, own);
            // 将随从放置到战场上并触发相关效果
            addMinionToBattlefield(m);
            return m;
        }

        /// <summary>
        /// 冻结一个随从，如果场上有摩尔克，则抽取目标随从的复制牌。
        /// </summary>
        /// <param name="target">要冻结的目标随从</param>
        public void minionGetFrozen(Minion target)
        {
            // 将目标随从标记为冻结状态
            target.frozen = true;

            // 如果目标是英雄，则直接返回
            if (target.isHero) return;

            // 遍历己方随从
            for (int i = 0; i < this.ownMinions.Count; i++)
            {
                Minion m = this.ownMinions[0];
                if (!m.silenced)
                    m.handcard.card.sim_card.onMinionFrozen(this, m, target);
            }
            // 遍历敌方随从

            for (int i = 0; i < this.enemyMinions.Count; i++)
            {
                Minion m = this.enemyMinions[0];
                if (!m.silenced)
                    m.handcard.card.sim_card.onMinionFrozen(this, m, target);
            }

        }

        /// <summary>
        /// 使指定随从沉默，移除其所有的能力和增益效果，但不会导致随从死亡。
        /// </summary>
        /// <param name="m">要被沉默的随从</param>
        public void minionGetSilenced(Minion m)
        {
            // 调用随从的 becomeSilence 方法，实现沉默效果
            // 该方法会移除随从的所有能力、增益效果以及任何附加效果（如嘲讽、风怒等）
            m.becomeSilence(this);

            // 注意：沉默不会导致随从死亡
            // 因此，尽管随从可能会失去某些关键效果，它仍然会保留最基本的生命值和攻击力
        }

        /// <summary>
        /// 使所有随从（己方或敌方）沉默，移除它们的所有能力和增益效果。
        /// </summary>
        /// <param name="own">如果为 true，则沉默己方随从；如果为 false，则沉默敌方随从。</param>
        public void allMinionsGetSilenced(bool own)
        {
            // 获取需要沉默的随从列表
            List<Minion> temp = (own) ? this.ownMinions : this.enemyMinions;

            // 遍历列表中的每个随从，并使其沉默
            foreach (Minion m in temp.ToArray())
            {
                m.becomeSilence(this); // 调用随从的沉默方法，移除其所有能力和增益效果
            }
        }

        public void minionGetDestroyed(Minion m)
        {
            if (m.own)
            {
                // 如果随从本回合出场且没有冲锋，计算并记录潜在的损失伤害值
                if (m.playedThisTurn && m.charge == 0)
                {
                    this.lostDamage += m.Hp * 2 + m.Angr * 2 + (m.windfury ? m.Angr : 0) +
                                       ((m.handcard.card.isSpecialMinion && !m.taunt) ? 20 : 0);
                }
            }

            // 销毁随从
            if (m.Hp > 0)
            {
                m.Hp = 0;  // 设置随从的生命值为0
                m.minionDied(this);  // 触发随从死亡事件
            }
        }

        /// <summary>
        /// 移除指定的随从，并且触发onAuraEnds离场效果
        /// </summary>
        /// <param name="m">要移除的随从</param>
        public void RemoveMinionWithoutDeathrattle(Minion m)
        {
            // 触发onAuraEnds离场效果
            m.handcard.card.sim_card.onAuraEnds(this, m);

            // 移除随从
            this.ownMinions.Remove(m); // 将随从从当前战场移除

        }

        /// <summary>
        /// 销毁场上所有的随从，包括己方和敌方随从。
        /// </summary>
        public void allMinionsGetDestroyed()
        {
            // 销毁己方所有随从
            foreach (Minion m in this.ownMinions.ToArray())
            {
                if (m.handcard.card.type != CardDB.cardtype.MOB || m.untouchable) continue;
                this.minionGetDestroyed(m);
            }

            // 销毁敌方所有随从
            foreach (Minion m in this.enemyMinions.ToArray())
            {
                if (m.handcard.card.type != CardDB.cardtype.MOB || m.untouchable) continue;
                this.minionGetDestroyed(m);
            }
        }



        /// <summary>
        /// 为指定随从增加护甲值，并更新手牌中的法术玉石进度。
        /// </summary>
        /// <param name="m">要增加护甲的随从对象。</param>
        /// <param name="armor">增加的护甲值。</param>
        public void minionGetArmor(Minion m, int armor)
        {
            // 剃刀沼泽摇滚明星
            if (this.anzOwnRazorfenRockstar > 0)
                armor = this.anzOwnRazorfenRockstar * 2;
            m.armor += armor;  // 增加随从的护甲值

            // 遍历己方手牌，处理与护甲相关的卡牌效果
            /* foreach (Handmanager.Handcard hc in this.owncards.ToArray())
            {
                // 检查是否持有 "小型法术玉石" 卡牌
                if (hc.card.nameCN == CardDB.cardNameCN.小型法术玉石)
                {
                    // 累积护甲值
                    hc.SCRIPT_DATA_NUM_1 += armor;

                    // 如果护甲值累积达到或超过 3 点，升级卡牌
                    if (hc.SCRIPT_DATA_NUM_1 >= 3)
                    {
                        // 重置护甲累积值
                        hc.SCRIPT_DATA_NUM_1 = 0;

                        // 将 "小型法术玉石" 升级为 "中型法术玉石"（假设 ID 为 LOOT_051t1）
                        hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_051t1);
                    }
                }

                // 检查是否持有 "法术玉石" 卡牌
                if (hc.card.nameCN == CardDB.cardNameCN.法术玉石)
                {
                    // 累积护甲值
                    hc.SCRIPT_DATA_NUM_1 += armor;

                    // 如果护甲值累积达到或超过 3 点，升级卡牌
                    if (hc.SCRIPT_DATA_NUM_1 >= 3)
                    {
                        // 重置护甲累积值
                        hc.SCRIPT_DATA_NUM_1 = 0;

                        // 将 "法术玉石" 升级为 "大型法术玉石"（假设 ID 为 LOOT_051t2）
                        hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_051t2);
                    }
                }
            } */

            // 触发英雄获得护甲的事件
            this.triggerAHeroGotArmor(m.own, armor);

            // 处理友方随从中处于冷却中的地标，ID为VAC_517（远足步道）
            // if (m.own && m.isHero)
            // {
            //     foreach (Minion ownMinion in this.ownMinions)
            //     {
            //         if (ownMinion.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_517 && ownMinion.CooldownTurn > 0)
            //         {
            //             CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_517);
            //             ownMinion.CooldownTurn = 0;
            //             ownMinion.handcard.card.CooldownTurn = 0;
            //             ownMinion.Ready = true;
            //             Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
            //         }
            //     }
            // }
        }

        /// <summary>
        /// 将随从返回到玩家的手牌中，考虑手牌数量和法力值变化。
        /// </summary>
        /// <param name="m">要返回手牌的随从对象。</param>
        /// <param name="own">是否是己方随从。</param>
        /// <param name="manachange">返回手牌后法力值的变化。</param>
        public void minionReturnToHand(Minion m, bool own, int manachange)
        {
            List<Minion> temp = (m.own) ? this.ownMinions : this.enemyMinions;

            // 移除随从的光环效果
            m.handcard.card.sim_card.onAuraEnds(this, m);
            temp.Remove(m);

            if (own)
            {
                CardDB.Card c = m.handcard.card;
                Handmanager.Handcard hc = new Handmanager.Handcard
                {
                    card = c,
                    position = this.owncards.Count + 1,
                    entity = m.entityID,
                    manacost = c.calculateManaCost(this) + manachange
                };

                if (this.owncards.Count < 10)
                {
                    this.owncards.Add(hc);
                    this.triggerCardsChanged(true); // 触发手牌变化事件
                }
                else
                {
                    this.drawACard(CardDB.cardNameEN.unknown, true); // 如果手牌已满，直接抽一张随机卡（疲劳伤害）
                }
            }
            else
            {
                this.drawACard(CardDB.cardNameEN.unknown, false); // 敌方随从返回时，抽一张随机卡
            }

            // 更新随从状态
            if (m.own)
            {
                this.tempTrigger.ownMinionsChanged = true;
            }
            else
            {
                this.tempTrigger.enemyMininsChanged = true;
            }
        }

        /// <summary>
        /// 将随从返回到玩家的牌库中，更新牌库大小。
        /// </summary>
        /// <param name="m">要返回牌库的随从对象。</param>
        /// <param name="own">是否是己方随从。</param>
        public void minionReturnToDeck(Minion m, bool own)
        {
            List<Minion> temp = (m.own) ? this.ownMinions : this.enemyMinions;

            // 移除随从的光环效果
            m.handcard.card.sim_card.onAuraEnds(this, m);
            temp.Remove(m);

            // 更新随从状态
            if (m.own)
            {
                this.tempTrigger.ownMinionsChanged = true;
                this.ownDeckSize++; // 增加己方牌库大小
            }
            else
            {
                this.tempTrigger.enemyMininsChanged = true;
                this.enemyDeckSize++; // 增加敌方牌库大小
            }
        }

        /// <summary>
        /// 将随从变形为指定的新随从，并处理相应的状态变化。
        /// </summary>
        /// <param name="m">要变形的随从对象。</param>
        /// <param name="c">变形后的新卡牌对象。</param>
        public void minionTransform(Minion m, CardDB.Card c)
        {
            // 移除当前随从的光环效果
            m.handcard.card.sim_card.onAuraEnds(this, m);

            Handmanager.Handcard hc = new Handmanager.Handcard(c) { entity = m.entityID };

            // 处理随从的嘲讽状态
            if (m.taunt)
            {
                if (m.own) this.anzOwnTaunt--;
                else this.anzEnemyTaunt--;
            }
            Minion summoned = createNewMinion(hc, m.zonepos, m.own);
            // 将当前随从变形为新随从
            m.setMinionToMinion(summoned);

            // 如果变形后的随从具有嘲讽，更新嘲讽数量
            if (m.taunt)
            {
                if (m.own) this.anzOwnTaunt++;
                else this.anzEnemyTaunt++;
            }

            // 激活新随从的光环效果并应用区域性BUFF
            m.handcard.card.sim_card.onAuraStarts(this, m);
            this.minionGetOrEraseAllAreaBuffs(m, true);
            // 更新随从状态
            m.updateReadyness();
            if (m.own)
            {
                this.tempTrigger.ownMinionsChanged = true;
            }
            else
            {
                this.tempTrigger.enemyMininsChanged = true;
            }

            if (logging)
            {
                Helpfunctions.Instance.logg("minion transformed: " + m.name + " " + m.Angr);
            }
        }

        /// <summary>
        /// 控制一个随从，改变其所有权并处理相关状态。
        /// </summary>
        /// <param name="m">要被控制的随从对象。</param>
        /// <param name="newOwner">新的拥有者是否为己方。</param>
        /// <param name="canAttack">被控制后是否能够攻击。</param>
        /// <param name="forced">是否强制控制，忽略随从数量上限。</param>
        public void minionGetControlled(Minion m, bool newOwner, bool canAttack, bool forced = false)
        {
            List<Minion> newOwnerList = newOwner ? this.ownMinions : this.enemyMinions;
            List<Minion> oldOwnerList = newOwner ? this.enemyMinions : this.ownMinions;
            bool isFreeSpace = true;

            if (newOwnerList.Count >= 7)
            {
                if (!forced) return; // 如果新拥有者随从已满且不强制，直接返回
                else isFreeSpace = false; // 强制控制时忽略随从数量限制
            }

            this.tempTrigger.ownMinionsChanged = true;
            this.tempTrigger.enemyMininsChanged = true;

            // 处理嘲讽状态的更新
            if (m.taunt)
            {
                if (newOwner)
                {
                    this.anzEnemyTaunt--;
                    if (isFreeSpace) this.anzOwnTaunt++;
                }
                else
                {
                    if (isFreeSpace) this.anzEnemyTaunt++;
                    this.anzOwnTaunt--;
                }
            }

            // 结束当前随从的光环效果和区域BUFF
            m.handcard.card.sim_card.onAuraEnds(this, m);
            this.minionGetOrEraseAllAreaBuffs(m, false);

            // 从原拥有者的随从列表中移除
            oldOwnerList.Remove(m);

            if (isFreeSpace)
            {
                // 更改随从所有权并标记为本回合被召唤
                m.playedThisTurn = true;
                m.own = !m.own;

                // 添加到新拥有者的随从列表中，并激活相关光环和BUFF
                newOwnerList.Add(m);
                m.handcard.card.sim_card.onAuraStarts(this, m);
                this.minionGetOrEraseAllAreaBuffs(m, true);

                // 如果随从具有冲锋或指定可以攻击，则设置为可攻击状态
                if (m.charge >= 1 || canAttack)
                {
                    this.minionGetCharge(m);
                }
                else
                {
                    m.updateReadyness();
                }
            }
        }

        /// <summary>
        /// 将磁力效果应用到一个机械随从上，将两个随从合并并传递相关属性。
        /// </summary>
        /// <param name="mOwn">执行磁力效果的随从。</param>
        public void Magnetic(Minion mOwn)
        {
            List<Minion> temp = mOwn.own ? this.ownMinions : this.enemyMinions;

            // 遍历随从列表，寻找目标随从进行磁力合并
            foreach (Minion m in temp.ToArray())
            {
                if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.MECHANICAL) && m.zonepos == mOwn.zonepos + 1)
                {
                    // 将mOwn的属性传递给目标机械随从m
                    this.minionGetBuffed(m, mOwn.Angr, mOwn.Hp);
                    if (mOwn.taunt) m.taunt = true;
                    if (mOwn.divineShield) m.divineShield = true;
                    if (mOwn.lifesteal) m.lifesteal = true;
                    if (mOwn.poisonous) m.poisonous = true;
                    if (mOwn.rush != 0) this.minionGetRush(m);
                    m.updateReadyness();

                    // 消除被合并的随从mOwn
                    this.minionGetSilenced(mOwn);
                    this.minionGetDestroyed(mOwn);

                    // 处理磁力效果后的触发器
                    // if (m.Ready) this.evaluatePenality -= 35; //Todo: 不引入打分
                    break;
                }
            }
        }

        public void minionGetLifesteal(Minion m)
        {
            if (m.lifesteal) return;
            m.lifesteal = true;
        }
        /// <summary>
        /// 给随从赋予圣盾效果
        /// </summary>
        /// <param name="m"></param>
        public void minionGetDivineShield(Minion m)
        {
            if (m.divineShield) return;
            m.divineShield = true;
        }
        /// <summary>
        /// 给随从赋予嘲讽效果
        /// </summary>
        /// <param name="m"></param>
        public void minionGetTaunt(Minion m)
        {
            if (m.taunt) return;
            m.taunt = true;
            if (m.own) this.anzOwnTaunt++;
            else this.anzEnemyTaunt++;
        }
        /// <summary>
        /// 给随从赋予风怒效果，使其能够在回合中进行两次攻击。
        /// </summary>
        /// <param name="m">目标随从。</param>
        public void minionGetWindfurry(Minion m)
        {
            if (m.windfury) return; // 如果已经有风怒效果，直接返回
            m.windfury = true;
            m.updateReadyness(); // 更新随从状态
        }

        /// <summary>
        /// 给随从赋予冲锋效果，使其能够在召唤的回合进行攻击。
        /// </summary>
        /// <param name="m">目标随从。</param>
        public void minionGetCharge(Minion m)
        {
            m.charge++;
            m.updateReadyness(); // 更新随从状态
        }

        /// <summary>
        /// 给随从赋予突袭效果，使其能够在召唤的回合攻击敌方随从。
        /// </summary>
        /// <param name="m">目标随从。</param>
        public void minionGetRush(Minion m)
        {
            if (m.cantAttack) return; // 如果随从无法攻击，直接返回
            m.rush = 1;
            m.updateReadyness(); // 更新随从状态

            // 如果随从没有冲锋且是本回合召唤的，限制其不能攻击英雄
            if (m.charge == 0 && m.playedThisTurn)
            {
                // Helpfunctions.Instance.ErrorLog("将赋予" + m.handcard.card.chnName + "突袭，因为不具备冲锋，本回合无法攻击！");
                m.cantAttackHeroes = true;
            }
        }

        /// <summary>
        /// 移除随从的冲锋效果。
        /// </summary>
        /// <param name="m">目标随从。</param>
        public void minionLostCharge(Minion m)
        {
            m.charge--;
            m.updateReadyness(); // 更新随从状态
        }

        /// <summary>
        /// 给随从临时增加攻击和生命值，如果该随从为英雄且处于德鲁伊任务线中，会触发相应任务进度。
        /// </summary>
        /// <param name="m">目标随从。</param>
        /// <param name="tempAttack">增加的临时攻击力。</param>
        /// <param name="tempHp">增加的临时生命值。</param>
        public void minionGetTempBuff(Minion m, int tempAttack, int tempHp)
        {
            // 如果随从没有被沉默且是光耀之子，则不进行buff处理
            // if (!m.silenced && m.name == CardDB.cardNameEN.lightspawn) return;

            // 防止攻击力减到负数
            if (tempAttack < 0 && -tempAttack > m.Angr)
            {
                tempAttack = -m.Angr;
            }
            m.tempAttack += tempAttack;
            m.Angr += tempAttack;

            // if (m.isHero)
            // {
            //     // 处理德鲁伊任务线的进度
            //     switch (this.ownQuest.Id)
            //     {
            //         case CardDB.cardIDEnum.SW_428:
            //             this.ownQuest.questProgress += tempAttack;
            //             if (this.ownQuest.questProgress >= this.ownQuest.maxProgress)
            //             {
            //                 this.evaluatePenality += (this.ownQuest.questProgress - this.ownQuest.maxProgress) * 5;
            //                 this.ownQuest = new Questmanager.QuestItem() { Id = CardDB.cardIDEnum.SW_428t, questProgress = 0, maxProgress = 5 };
            //                 minionGetArmor(this.ownHero, 5);
            //             }
            //             break;
            //         case CardDB.cardIDEnum.SW_428t:
            //             this.ownQuest.questProgress += tempAttack;
            //             if (this.ownQuest.questProgress >= this.ownQuest.maxProgress)
            //             {
            //                 this.evaluatePenality += (this.ownQuest.questProgress - this.ownQuest.maxProgress) * 5;
            //                 this.ownQuest = new Questmanager.QuestItem() { Id = CardDB.cardIDEnum.SW_428t2, questProgress = 0, maxProgress = 6 };
            //                 drawACard(CardDB.cardIDEnum.None, true, false);
            //                 minionGetArmor(this.ownHero, 5);
            //             }
            //             break;
            //         case CardDB.cardIDEnum.SW_428t2:
            //             this.ownQuest.questProgress += tempAttack;
            //             if (this.ownQuest.questProgress >= this.ownQuest.maxProgress)
            //             {
            //                 drawACard(CardDB.cardIDEnum.SW_428t4, true, true);
            //                 this.ownQuest.Reset();
            //             }
            //             break;
            //     }
            // }
            m.updateReadyness();
        }

        /// <summary>
        /// 给随从增加邻近buff的攻击力，更新其攻击力的值。
        /// </summary>
        /// <param name="m">目标随从。</param>
        /// <param name="angr">增加的攻击力。</param>
        /// <param name="vert">增加的生命值。</param>
        public void minionGetAdjacentBuff(Minion m, int angr, int vert)
        {
            // if (!m.silenced && m.name == CardDB.cardNameEN.lightspawn) return;
            m.Angr += angr;
            m.AdjacentAngr += angr;
        }

        /// <summary>
        /// 给一方所有随从增加攻击和生命值buff。
        /// </summary>
        /// <param name="own">true表示己方，false表示敌方。</param>
        /// <param name="attackbuff">增加的攻击力。</param>
        /// <param name="hpbuff">增加的生命值。</param>
        public void allMinionOfASideGetBuffed(bool own, int attackbuff, int hpbuff)
        {
            List<Minion> temp = own ? this.ownMinions : this.enemyMinions;
            foreach (Minion m in temp.ToArray())
            {
                minionGetBuffed(m, attackbuff, hpbuff);
            }
        }

        /// <summary>
        /// 给指定随从增加攻击和生命值buff，处理特定随从的特殊效果。
        /// </summary>
        /// <param name="m">目标随从。</param>
        /// <param name="attackbuff">增加的攻击力。</param>
        /// <param name="hpbuff">增加的生命值。</param>
        public void minionGetBuffed(Minion m, int attackbuff, int hpbuff)
        {
            if (m.untouchable || m.handcard.card.type == CardDB.cardtype.LOCATION) return;

            // 处理攻击力buff
            if (attackbuff != 0)
            {
                m.Angr = Math.Max(0, m.Angr + attackbuff);
            }

            // 处理生命值buff
            if (hpbuff != 0)
            {
                if (hpbuff > 0)
                {
                    m.Hp += hpbuff;
                    m.maxHp += hpbuff;
                }
                else
                {
                    m.maxHp += hpbuff;
                    if (m.maxHp < m.Hp)
                    {
                        m.Hp = m.maxHp;
                    }
                }
                m.wounded = (m.maxHp != m.Hp);
            }

            if (m.Hp <= 0)
            {
                m.minionDied(this);
            }

            // 特殊随从：光耀之子
            // if (m.name == CardDB.cardNameEN.lightspawn && !m.silenced)
            // {
            //     m.Angr = m.Hp;
            // }

            // 特殊随从：血色骑士赛丹
            // if (m.name == CardDB.cardNameEN.saidanthescarlet && !m.silenced)
            // {
            //     m.Angr += attackbuff * 2;
            //     m.Hp += hpbuff * 2;
            // }
        }

        /// <summary>
        /// 给克苏恩增加攻击力、生命值和嘲讽buff，若克苏恩在场上，则应用buff。
        /// </summary>
        /// <param name="attackbuff">增加的攻击力。</param>
        /// <param name="hpbuff">增加的生命值。</param>
        /// <param name="tauntbuff">增加的嘲讽buff。</param>
        public void cthunGetBuffed(int attackbuff, int hpbuff, int tauntbuff)
        {
            this.anzOgOwnCThunAngrBonus += attackbuff;
            this.anzOgOwnCThunHpBonus += hpbuff;
            this.anzOgOwnCThunTaunt += tauntbuff;

            // 如果克苏恩在场，则应用buff
            foreach (Minion m in this.ownMinions)
            {
                if (m.name == CardDB.cardNameEN.cthun)
                {
                    this.minionGetBuffed(m, attackbuff, hpbuff);
                    if (tauntbuff > 0)
                    {
                        m.taunt = true;
                        this.anzOwnTaunt++;
                    }
                }
            }
        }

        /// <summary>
        /// 移除随从的圣盾效果，并触发相关触发器。
        /// </summary>
        /// <param name="m">目标随从。</param>
        public void minionLosesDivineShield(Minion m)
        {
            m.divineShield = false;
            if (m.own) this.tempTrigger.ownMinionLosesDivineShield++;
            else this.tempTrigger.enemyMinionLosesDivineShield++;
        }

        /// <summary>
        /// 将随从的攻击力设置为指定的数值，并更新所有区域性增益或减益。
        /// </summary>
        /// <param name="m">目标随从</param>
        /// <param name="newAngr">新的攻击力值</param>
        public void minionSetAttackToX(Minion m, int newAngr)
        {
            // 如果随从未被沉默，并且是光耀之子（lightspawn），则不改变其攻击力
            // if (!m.silenced && m.name == CardDB.cardNameEN.lightspawn) return;

            // 设置随从的攻击力为新值，并重置临时攻击力增益
            m.Angr = newAngr;
            m.tempAttack = 0;

            // 重新计算并应用所有区域性增益或减益
            this.minionGetOrEraseAllAreaBuffs(m, true);
        }

        /// <summary>
        /// 将随从的生命值设置为指定的数值，并更新所有区域性增益或减益。
        /// </summary>
        /// <param name="m">目标随从</param>
        /// <param name="newHp">新的生命值</param>
        public void minionSetHealthtoX(Minion m, int newHp)
        {
            // 移除当前的区域性增益或减益
            minionGetOrEraseAllAreaBuffs(m, false);

            // 设置随从的生命值和最大生命值为新值
            m.Hp = newHp;
            m.maxHp = newHp;

            // 如果随从曾经受伤且未被沉默，则停止激怒效果
            if (m.wounded && !m.silenced) m.handcard.card.sim_card.onEnrageStop(this, m);

            // 标记随从为未受伤状态
            m.wounded = false;

            // 重新计算并应用所有区域性增益或减益
            minionGetOrEraseAllAreaBuffs(m, true);
            if (m.Hp <= 0)
            {
                m.minionDied(this);
            }
        }

        /// <summary>
        /// 将随从的攻击力设置为与其生命值相同的数值。
        /// </summary>
        /// <param name="m">目标随从</param>
        public void minionSetAttackToHealth(Minion m)
        {
            // 设置随从的攻击力为其当前的生命值，并重置临时攻击力增益
            m.Angr = m.Hp;
            m.tempAttack = 0;

            // 重新计算并应用所有区域性增益或减益
            this.minionGetOrEraseAllAreaBuffs(m, true);
        }

        /// <summary>
        /// 交换随从的攻击力和生命值，并处理相应的状态变化。
        /// </summary>
        /// <param name="m">目标随从</param>
        public void minionSwapAngrAndHP(Minion m)
        {
            // 记录随从在交换前是否受伤
            bool woundedbef = m.wounded;

            // 交换攻击力和生命值
            int temp = m.Angr;
            m.Angr = m.Hp;
            m.Hp = temp;
            m.maxHp = temp;

            // 将随从标记为未受伤状态，如果之前受伤，则停止激怒效果
            m.wounded = false;
            if (woundedbef) m.handcard.card.sim_card.onEnrageStop(this, m);

            // 如果交换后随从的生命值小于等于0，标记为死亡
            if (m.Hp <= 0)
            {
                if (m.own) this.tempTrigger.ownMinionsDied++;
                else this.tempTrigger.enemyMinionsDied++;
            }

            // 重新计算并应用所有区域性增益或减益
            this.minionGetOrEraseAllAreaBuffs(m, true);
        }

        public void CallMinionCopy(Minion originMinion, bool own, int callCount = 1, bool surroundSummon = false)
        {
            //做空判断,源随从为空直接退出
            if (originMinion == null) return;
            //如果是环绕召唤，则需要判断源随从位置
            if (surroundSummon)
            {
                //左边召唤还是右边召唤
                bool LeftOrRight = true;
                //根据源随从是否为己方随从，获取起使召唤位置
                int zonepos = originMinion.own ? originMinion.zonepos : (own ? this.ownMinions.Count : this.enemyMinions.Count);
                for (int i = 0; i < callCount; i++)
                {
                    //获取召唤位置
                    int position = LeftOrRight ? zonepos - 1 : zonepos;
                    //召唤并且返回召唤的对象
                    Minion summoned = this.callKidAndReturn(originMinion.handcard.card, position, own);
                    //判断召唤对象是否为空
                    if (summoned != null)
                        //将召唤对象属性设置为源随从
                        summoned.setMinionToMinion(originMinion);
                    LeftOrRight = !LeftOrRight;
                }
            }
            else
            {
                //非环绕召唤，默认从源随从右边开始召唤
                int zonepos = originMinion.own ? originMinion.zonepos : (own ? this.ownMinions.Count : this.enemyMinions.Count);

                for (int i = 0; i < callCount; i++)
                {
                    //根据源随从是否为己方随从，获取起使召唤位置
                    int position = zonepos;

                    //召唤并且返回召唤的对象
                    Minion summoned = this.callKidAndReturn(originMinion.handcard.card, position, own);
                    //判断召唤对象是否为空
                    if (summoned != null)
                        //将召唤对象属性设置为源随从
                        summoned.setMinionToMinion(originMinion);
                }

            }

        }
    }
}
