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
    // 触发器与亡语：伤害/治疗/死亡/召唤触发器、亡语处理、灌注
    public partial class Playfield
    {
        public void doDmgTriggers()
        {
            // 处理角色治疗触发的效果（如获得攻击力等）
            if (this.tempTrigger.charsGotHealed >= 1)
            {
                triggerACharGotHealed();
            }

            // 处理随从治疗触发的效果（如抽卡等）
            if (this.tempTrigger.minionsGotHealed >= 1)
            {
                triggerAMinionGotHealed();
            }

            // 处理随从受伤触发的效果（如抽卡、获得护甲或攻击力等）
            if (this.tempTrigger.ownMinionsGotDmg + this.tempTrigger.enemyMinionsGotDmg >= 1)
            {
                triggerAMinionGotDmg();
            }

            // 处理随从死亡触发的效果（如抽卡、获得攻击力和生命值、召唤随从等）
            if (this.tempTrigger.ownMinionsDied + this.tempTrigger.enemyMinionsDied >= 1)
            {
                triggerAMinionDied();

                // 更新随从状态变化的标志位
                if (this.tempTrigger.ownMinionsDied >= 1)
                {
                    this.tempTrigger.ownMinionsChanged = true;
                }
                if (this.tempTrigger.enemyMinionsDied >= 1)
                {
                    this.tempTrigger.enemyMininsChanged = true;
                }

                // 重置死亡触发器计数
                ResetMinionDeathTriggers();
            }

            // 处理随从失去圣盾触发的效果（如抽卡、获得护甲或攻击力等）
            if (this.tempTrigger.ownMinionLosesDivineShield + this.tempTrigger.enemyMinionLosesDivineShield >= 1)
            {
                triggerAMinionLosesDivineShield();
            }

            // 更新当前游戏状态，如随从的位置和状态等
            updateBoards();

            // 处理"小型法术尖晶石"卡牌升级的逻辑
            /* if (this.owncards.Any())
            {
                foreach (Handmanager.Handcard hc in this.owncards.ToArray())
                {
                    if (hc.card.nameEN == CardDB.cardNameEN.lesserspinelspellstone)
                    {
                        hc.SCRIPT_DATA_NUM_1 += this.tempTrigger.ownMinionsDied;
                        if (hc.SCRIPT_DATA_NUM_1 >= 5)
                        {
                            hc.SCRIPT_DATA_NUM_1 = 0;
                            hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_825t); // 升级到法术尖晶石
                        }
                    }
                    else if (hc.card.nameEN == CardDB.cardNameEN.spinelspellstone)
                    {
                        hc.SCRIPT_DATA_NUM_1 += this.tempTrigger.ownMinionsDied;
                        if (hc.SCRIPT_DATA_NUM_1 >= 5)
                        {
                            hc.SCRIPT_DATA_NUM_1 = 0;
                            hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_825t2); // 升级到大型法术尖晶石
                        }
                    }
                }
            } */

            // 处理友方地标VAC_425（大地之末号）的冷却状态
            // foreach (Minion m in this.ownMinions)
            // {
            //     if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_425 && m.CooldownTurn > 0)
            //     {
            //         CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_425);
            //         m.CooldownTurn = 0;
            //         m.handcard.card.CooldownTurn = 0;
            //         m.Ready = true;
            //         Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
            //     }
            // }

            // 如果在处理伤害触发器的过程中又触发了新的事件，递归处理这些事件
            if (HasPendingTriggers())
            {
                doDmgTriggers(); // 递归调用以处理新触发的事件
            }
        }

        /// <summary>
        /// 重置随从死亡触发器的计数。
        /// </summary>
        private void ResetMinionDeathTriggers()
        {
            this.tempTrigger.ownMinionsDied = 0;
            this.tempTrigger.enemyMinionsDied = 0;
            this.tempTrigger.ownBeastDied = 0;
            this.tempTrigger.enemyBeastDied = 0;
            this.tempTrigger.ownMurlocDied = 0;
            this.tempTrigger.enemyMurlocDied = 0;
            this.tempTrigger.ownMechanicDied = 0;
            this.tempTrigger.enemyMechanicDied = 0;
            this.tempTrigger.ownUndeadDied = 0;
            this.tempTrigger.enemyUndeadDied = 0;
            this.tempTrigger.ownTreantDied = 0;
        }

        /// <summary>
        /// 判断是否有待处理的触发器。
        /// </summary>
        /// <returns>如果有待处理的触发器返回 true，否则返回 false。</returns>
        private bool HasPendingTriggers()
        {
            return this.tempTrigger.charsGotHealed + this.tempTrigger.minionsGotHealed +
                   this.tempTrigger.ownMinionsGotDmg + this.tempTrigger.enemyMinionsGotDmg +
                   this.tempTrigger.ownMinionsDied + this.tempTrigger.enemyMinionsDied >= 1;
        }


        /// <summary>
        /// 触发当角色被治疗时的效果，检查场上所有随从并执行相应的逻辑。
        /// </summary>
        public void triggerACharGotHealed()
        {
            int healedAmount = this.tempTrigger.charsGotHealed; // 获取触发的治疗量
            this.tempTrigger.charsGotHealed = 0; // 重置治疗触发计数

            // 处理己方随从的治疗触发效果
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.silenced) continue; // 如果随从被沉默，跳过处理

                // 根据随从的名称触发相应的治疗效果
                switch (mnn.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.lightwarden:
                    case CardDB.cardNameEN.holychampion:
                    case CardDB.cardNameEN.shadowboxer:
                    case CardDB.cardNameEN.hoodedacolyte:
                    case CardDB.cardNameEN.aiextra1:
                        mnn.handcard.card.sim_card.onACharGotHealed(this, mnn, healedAmount);
                        break;

                    case CardDB.cardNameEN.blackguard:
                        if (ownHero.GotHealedValue > 0) // 如果己方英雄被治疗
                        {
                            mnn.handcard.card.sim_card.onACharGotHealed(this, mnn, ownHero.GotHealedValue);
                        }
                        break;
                }
            }

            // 处理敌方随从的治疗触发效果
            foreach (Minion mnn in this.enemyMinions)
            {
                if (mnn.silenced) continue; // 如果随从被沉默，跳过处理

                // 根据随从的名称触发相应的治疗效果
                switch (mnn.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.lightwarden:
                    case CardDB.cardNameEN.holychampion:
                    case CardDB.cardNameEN.shadowboxer:
                    case CardDB.cardNameEN.hoodedacolyte:
                    case CardDB.cardNameEN.aiextra1:
                        mnn.handcard.card.sim_card.onACharGotHealed(this, mnn, healedAmount);
                        break;

                    case CardDB.cardNameEN.blackguard:
                        if (enemyHero.GotHealedValue > 0) // 如果敌方英雄被治疗
                        {
                            mnn.handcard.card.sim_card.onACharGotHealed(this, mnn, enemyHero.GotHealedValue);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 触发随从被治疗时的效果，遍历己方和敌方随从，执行相应的逻辑。
        /// </summary>
        public void triggerAMinionGotHealed()
        {
            int healedMinionsCount = this.tempTrigger.minionsGotHealed; // 获取触发的随从治疗次数
            this.tempTrigger.minionsGotHealed = 0; // 重置随从治疗触发计数

            // 处理己方随从的治疗触发效果
            foreach (Minion mnn in this.ownMinions.ToArray()) // 使用 ToArray 防止在遍历过程中修改集合
            {
                if (mnn.silenced) continue; // 如果随从被沉默，跳过处理

                // 根据随从的名称触发相应的治疗效果
                switch (mnn.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.northshirecleric: // 北郡牧师
                    case CardDB.cardNameEN.manageode: // 法力侏儒
                    case CardDB.cardNameEN.aiextra1:
                        mnn.handcard.card.sim_card.onAMinionGotHealedTrigger(this, mnn, healedMinionsCount);
                        break;
                }
            }

            // 处理敌方随从的治疗触发效果
            foreach (Minion mnn in this.enemyMinions.ToArray()) // 使用 ToArray 防止在遍历过程中修改集合
            {
                if (mnn.silenced) continue; // 如果随从被沉默，跳过处理

                // 根据随从的名称触发相应的治疗效果
                switch (mnn.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.northshirecleric: // 北郡牧师
                    case CardDB.cardNameEN.manageode: // 法力侏儒
                    case CardDB.cardNameEN.aiextra1:
                        mnn.handcard.card.sim_card.onAMinionGotHealedTrigger(this, mnn, healedMinionsCount);
                        break;
                }
            }
        }

        /// <summary>
        /// 触发随从或英雄受到伤害后的效果，遍历己方和敌方随从，并执行相应的逻辑。
        /// </summary>
        public void triggerAMinionGotDmg()
        {
            // 记录己方和敌方随从及英雄受到伤害的次数
            int anzOwnMinionsGotDmg = this.tempTrigger.ownMinionsGotDmg;
            int anzEnemyMinionsGotDmg = this.tempTrigger.enemyMinionsGotDmg;
            int anzOwnHeroGotDmg = this.tempTrigger.ownHeroGotDmg;
            int anzEnemyHeroGotDmg = this.tempTrigger.enemyHeroGotDmg;

            // 处理己方随从受到伤害后的效果
            foreach (Minion m in this.ownMinions.ToArray())
            {
                if (m.silenced)
                {
                    m.anzGotDmg = 0; // 如果随从被沉默，重置其受到伤害的计数
                    continue;
                }

                // 触发随从的受到伤害效果
                m.handcard.card.sim_card.onMinionGotDmgTrigger(this, m, anzOwnMinionsGotDmg, anzEnemyMinionsGotDmg, anzOwnHeroGotDmg, anzEnemyHeroGotDmg);

                // 重置随从的伤害相关计数器
                m.anzGotDmg = 0;
                m.GotDmgValue = 0;
            }

            // 处理敌方随从受到伤害后的效果
            foreach (Minion m in this.enemyMinions.ToArray())
            {
                if (m.silenced)
                {
                    m.anzGotDmg = 0; // 如果随从被沉默，重置其受到伤害的计数
                    continue;
                }

                // 触发随从的受到伤害效果
                m.handcard.card.sim_card.onMinionGotDmgTrigger(this, m, anzOwnMinionsGotDmg, anzEnemyMinionsGotDmg, anzOwnHeroGotDmg, anzEnemyHeroGotDmg);

                // 重置随从的伤害相关计数器
                m.anzGotDmg = 0;
                m.GotDmgValue = 0;
            }

            // 重置英雄的伤害相关计数器
            this.ownHero.anzGotDmg = 0;
            this.enemyHero.anzGotDmg = 0;

            this.tempTrigger.ownMinionsGotDmg = 0;
            this.tempTrigger.enemyMinionsGotDmg = 0;
            this.tempTrigger.ownHeroGotDmg = 0;
            this.tempTrigger.enemyHeroGotDmg = 0;
        }

        /// <summary>
        /// 处理随从失去圣盾后的触发效果，遍历己方和敌方随从，并执行相应的逻辑。
        /// </summary>
        public void triggerAMinionLosesDivineShield()
        {
            int anzOwn = this.tempTrigger.ownMinionLosesDivineShield; // 记录己方随从失去圣盾的次数
            int anzEnemy = this.tempTrigger.enemyMinionLosesDivineShield; // 记录敌方随从失去圣盾的次数
            this.tempTrigger.ownMinionLosesDivineShield = 0; // 重置己方随从失去圣盾的计数器
            this.tempTrigger.enemyMinionLosesDivineShield = 0; // 重置敌方随从失去圣盾的计数器

            // 处理己方随从失去圣盾的效果
            if (anzOwn > 0)
            {
                foreach (Minion m in this.ownMinions.ToArray())
                {
                    if (m.silenced) continue; // 如果随从被沉默，跳过处理
                    m.handcard.card.sim_card.onMinionLosesDivineShield(this, m, anzOwn); // 触发随从失去圣盾后的效果
                }

                // 检查己方武器是否为 "光之悲伤" 并增加其攻击力
                if (this.ownWeapon.name == CardDB.cardNameEN.lightssorrow)
                {
                    this.ownWeapon.Angr += anzOwn;
                }

                // 检查己方武器是否为 "棱彩珠宝包" 并触发其效果
                if (this.ownWeapon.name == CardDB.cardNameEN.prismaticjewelkit)
                {
                    this.ownWeapon.card.sim_card.onMinionLosesDivineShield(this, new Minion(), anzOwn);
                }
            }

            // 处理敌方随从失去圣盾的效果
            if (anzEnemy > 0)
            {
                foreach (Minion m in this.enemyMinions.ToArray())
                {
                    if (m.silenced) continue; // 如果随从被沉默，跳过处理
                    m.handcard.card.sim_card.onMinionLosesDivineShield(this, m, anzEnemy); // 触发随从失去圣盾后的效果
                }

                // 检查敌方武器是否为 "光之悲伤" 并增加其攻击力
                if (this.enemyWeapon.name == CardDB.cardNameEN.lightssorrow)
                {
                    this.enemyWeapon.Angr += anzEnemy;
                }
            }
        }

        /// <summary>
        /// 触发随从死亡后的效果，处理己方和敌方随从的死亡触发器，并执行相应的逻辑。
        /// </summary>
        public void triggerAMinionDied()
        {
            // 更新当前回合己方和敌方随从的死亡计数
            this.ownMinionsDiedTurn += this.tempTrigger.ownMinionsDied;
            this.enemyMinionsDiedTurn += this.tempTrigger.enemyMinionsDied;

            // 处理己方随从的死亡触发效果
            TriggerMinionDiedEffects(this.ownMinions);

            // 处理敌方随从的死亡触发效果
            TriggerMinionDiedEffects(this.enemyMinions);

            // // 处理己方手牌中 "伯瓦尔·弗塔根" 的攻击力增加效果
            // foreach (Handmanager.Handcard hc in this.owncards)
            // {
            //     if (hc.card.nameEN == CardDB.cardNameEN.bolvarfordragon_GVG_063)
            //     {
            //         hc.addattack += this.tempTrigger.ownMinionsDied; // 每死亡一个己方随从，攻击力增加
            //     }
            // }

            // 处理己方和敌方武器 "鲨鱼之颚" 的攻击力增加效果
            // HandleWeaponEffectOnMinionDeath(this.ownWeapon, true);
            // HandleWeaponEffectOnMinionDeath(this.enemyWeapon, false);

            // 处理英雄技能 "死者复生" 的召唤效果
            // HandleHeroAbilitySummon(this.ownHeroAblility, this.tempTrigger.enemyMinionsDied, true);
            // HandleHeroAbilitySummon(this.enemyHeroAblility, this.tempTrigger.ownMinionsDied, false);

            // 处理注能卡牌
            if (this.ownMinions.Any(m => m.handcard.card.cardIDenum == CardDB.cardIDEnum.MAW_031))
            {
                InfuseDeckCards();
            }
            else
            {
                InfuseHandCards();
            }
        }

        /// <summary>
        /// 处理随从的死亡触发效果
        /// </summary>
        /// <param name="minions">场上的随从列表（己方或敌方）</param>
        private void TriggerMinionDiedEffects(List<Minion> minions)
        {
            foreach (Minion m in minions.ToArray())
            {
                if (m.silenced || m.Hp > 0) continue; // 如果随从被沉默或未死亡，跳过处理
                m.handcard.card.sim_card.onMinionDiedTrigger(this, m, m); // 触发随从的死亡效果
            }
        }

        /// <summary>
        /// 处理英雄技能 "死者复生" 的召唤效果
        /// </summary>
        /// <param name="ability">英雄技能对应的卡牌</param>
        /// <param name="minionsDied">触发技能的随从死亡数量</param>
        /// <param name="isOwn">是否为己方英雄技能</param>
        private void HandleHeroAbilitySummon(Handmanager.Handcard ability, int minionsDied, bool isOwn)
        {
            if (ability.card.nameEN == CardDB.cardNameEN.raisedead && minionsDied > 0)
            {
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(
                    (ability.card.cardIDenum == CardDB.cardIDEnum.NAX4_04H) ?
                    CardDB.cardIDEnum.NAX4_03H :
                    CardDB.cardIDEnum.NAX4_03
                );
                for (int i = 0; i < minionsDied; i++)
                {
                    this.callKid(kid, isOwn ? this.ownMinions.Count : this.enemyMinions.Count, isOwn); // 召唤对应的随从
                }
            }
        }

        /// <summary>
        /// 注能己方牌库中的卡牌
        /// </summary>
        private void InfuseDeckCards()
        {
            foreach (CardDB.Card item in this.ownDeck)
            {
                if (item.Infuse && !item.Infused)
                {
                    Handmanager.Handcard hc = new Handmanager.Handcard(item);
                    InfuseCard(hc); // 对卡牌进行注能处理
                }
            }
        }

        /// <summary>
        /// 注能己方手牌中的卡牌
        /// </summary>
        private void InfuseHandCards()
        {
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                // if (hc.card.Infuse && !hc.card.Infused)
                if (hc.card.Infuse && !hc.card.Infused)
                {
                    InfuseCard(hc); // 对卡牌进行注能处理
                }
            }
        }

        /// <summary>
        /// 处理单张手牌的注能逻辑
        /// </summary>
        /// <param name="hc">需要注能的手牌</param>
        private void InfuseCard(Handmanager.Handcard hc)
        {
            // 累计注能值
            hc.SCRIPT_DATA_NUM_1 += 1;

            if (hc.SCRIPT_DATA_NUM_1 >= hc.card.InfuseNum)
            {
                // 重置注能值
                hc.SCRIPT_DATA_NUM_1 = 0;
                if (hc.card.Infuse)
                {
                    switch (hc.card.cardIDenum)
                    {
                        case CardDB.cardIDEnum.REV_906t:
                            {
                                hc.card.TAG_SCRIPT_DATA_NUM_2++; // 无限注能，伤害增加1点
                            }
                            break;
                        default:
                            {
                                // 升级卡牌并标记为已注能
                                hc.card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString());
                                hc.card.Infused = true;
                            }
                            break;
                    }
                }
                /*                 // 特殊处理德纳修斯大帝和猎手阿尔迪莫
                                if (hc.card.cardIDenum != CardDB.cardIDEnum.REV_906 && hc.card.cardIDenum != CardDB.cardIDEnum.REV_906t &&
                                    hc.card.cardIDenum != CardDB.cardIDEnum.REV_353 && hc.card.cardIDenum != CardDB.cardIDEnum.REV_353t && hc.card.cardIDenum != CardDB.cardIDEnum.REV_353t2)
                                {
                                    // 升级卡牌并标记为已注能
                                    hc.card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString());
                                    hc.card.Infused = true;
                                }
                                else if (hc.card.cardIDenum == CardDB.cardIDEnum.REV_906)
                                {
                                    hc.card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString());

                                }
                                else if (hc.card.cardIDenum == CardDB.cardIDEnum.REV_906t)
                                {
                                    hc.card.TAG_SCRIPT_DATA_NUM_2++; // 无限注能，伤害增加1点
                                }
                                else if (hc.card.cardIDenum == CardDB.cardIDEnum.REV_353)
                                {
                                    hc.card = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(hc.card.cardIDenum.ToString() + "t"));
                                }
                                else if (hc.card.cardIDenum == CardDB.cardIDEnum.REV_353t)
                                {
                                    hc.card = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(hc.card.cardIDenum.ToString() + "t2"));
                                    hc.card.Infused = true; // 标记为已注能
                                } */
            }
        }

        /// <summary>
        /// 处理武器在随从死亡时的效果，主要针对 "巨颚" 的攻击力增加。
        /// </summary>
        /// <param name="weapon">要处理的武器。</param>
        /// <param name="own">标识武器是否为己方。</param>
        private void HandleWeaponEffectOnMinionDeath(Weapon weapon, bool own)
        {
            if (weapon.name == CardDB.cardNameEN.jaws)
            {
                int bonus = 0;

                // 计算己方和敌方已死亡且具有亡语的随从数
                foreach (Minion m in this.ownMinions)
                {
                    if (m.Hp < 1 && m.handcard.card.deathrattle && !m.silenced) bonus++;
                }
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.Hp < 1 && m.handcard.card.deathrattle && !m.silenced) bonus++;
                }

                // 增加武器的攻击力
                weapon.Angr += bonus * 2;
            }
        }

        /// <summary>
        /// 触发随从即将攻击前的效果，根据随从的类型和附加的效果执行相应的逻辑。
        /// </summary>
        /// <param name="attacker">即将攻击的随从。</param>
        /// <param name="defender">攻击的目标。</param>
        /// <returns>返回flag，表示defender是否死亡,返回true时死亡，false存活，默认返回false</returns>
        public bool triggerAMinionIsGoingToAttack(Minion attacker, Minion defender)
        {
            bool flag = false;
            if (!attacker.silenced)
            {
                //调用随从攻击时的sim方法
                attacker.handcard.card.sim_card.onMinionAttack(this, attacker, defender, ref flag);
                //当随从死亡时flag为true,表示随从死亡
                if (defender.Hp <= 0)
                    flag = true;
            }
            if (!defender.silenced)
            {
                defender.handcard.card.sim_card.OnAttacked(this, defender, attacker);
            }

            // 处理随从上附加的智慧祝福效果：每次攻击时抽取一张牌
            if (attacker.ownBlessingOfWisdom >= 1)
            {
                for (int i = 0; i < attacker.ownBlessingOfWisdom; i++)
                {
                    this.drawACard(CardDB.cardNameEN.unknown, true);
                }
            }
            if (attacker.enemyBlessingOfWisdom >= 1)
            {
                for (int i = 0; i < attacker.enemyBlessingOfWisdom; i++)
                {
                    this.drawACard(CardDB.cardNameEN.unknown, false);
                }
            }

            // 处理随从上附加的真言术：耀效果：每次攻击时为英雄恢复4点生命值
            if (attacker.ownPowerWordGlory >= 1)
            {
                int heal = this.getMinionHeal(4);
                for (int i = 0; i < attacker.ownPowerWordGlory; i++)
                {
                    this.minionGetDamageOrHeal(this.ownHero, -heal);
                }
            }
            if (attacker.enemyPowerWordGlory >= 1)
            {
                int heal = this.getEnemyMinionHeal(4);
                for (int i = 0; i < attacker.enemyPowerWordGlory; i++)
                {
                    this.minionGetDamageOrHeal(this.enemyHero, -heal);
                }
            }
            return flag;
        }

        /// <summary>
        /// 处理随从造成伤害后的触发效果。
        /// </summary>
        /// <param name="m">造成伤害的随从。</param>
        /// <param name="dmgDone">造成的伤害值。</param>
        /// <param name="isAttacker">是否是攻击者造成的伤害。</param>
        public void triggerAMinionDealedDmg(Minion m, int dmgDone, bool isAttacker)
        {
            // 仅有少数卡牌具有此触发效果
            m.handcard.card.sim_card.onDamageDealtByMinion(this, m, dmgDone, isAttacker);

            // 处理随从的吸血效果
            if (m.lifesteal && isAttacker && dmgDone > 0)
            {
                if (m.own)
                {
                    // 如果己方有奥金尼灵魂祭司或暗影之握效果，则吸血效果反转为伤害自己
                    if (this.anzOwnAuchenaiSoulpriest > 0 || this.embracetheshadow > 0)
                    {
                        dmgDone *= -1;
                    }
                    // 为己方英雄治疗等同于伤害值的生命值
                    this.minionGetDamageOrHeal(this.ownHero, -dmgDone);
                }
                else
                {
                    // 如果敌方有奥金尼灵魂祭司，则吸血效果反转为伤害敌方英雄
                    if (this.anzEnemyAuchenaiSoulpriest > 1)
                    {
                        dmgDone *= -1;
                    }
                    // 为敌方英雄治疗等同于伤害值的生命值
                    this.minionGetDamageOrHeal(this.enemyHero, -dmgDone);
                }
            }
        }

        /// <summary>
        /// 处理一张卡牌即将被打出的事件，根据场上的随从和武器触发相应的效果。
        /// </summary>
        /// <param name="hc">即将被打出的手牌。</param>
        /// <param name="own">标识是否为己方打出的卡牌。</param>
        public void triggerACardWillBePlayed(Handmanager.Handcard hc, bool own)
        {
            if (own)
            {
                // 处理特殊随从效果
                if (anzOwnDragonConsort > 0 && (TAG_RACE)hc.card.race == TAG_RACE.DRAGON) anzOwnDragonConsort = 0; // 龙族侍女效果
                if (ownBeastCostLessOnce > 0 && (TAG_RACE)hc.card.race == TAG_RACE.PET) ownBeastCostLessOnce = 0; // 野兽费用减免效果
                if (nextElementalReduction > 0 && (TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) nextElementalReduction = 0; // 下一张元素随从牌的法力值消耗减少量
                if (thisTurnNextElementalReduction > 0 && (TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) thisTurnNextElementalReduction = 0; // 本回合下一张元素费用减免效果

                int burly = 0;
                int ssm = 0;

                // 触发己方随从的效果
                foreach (Minion m in this.ownMinions.ToArray())
                {
                    if (!m.silenced)
                    {
                        m.handcard.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, m);
                        // m.handcard.card.sim_card.onCardWasPlayed(this, hc.card, own, m);
                    }
                }

                // 触发敌方随从的效果
                foreach (Minion m in this.enemyMinions.ToArray())
                {
                    if (m.name == CardDB.cardNameEN.troggzortheearthinator)
                    {
                        burly++;
                    }
                    // //敌方魔能机甲
                    // if (m.name == CardDB.cardNameEN.felreaver)
                    // {
                    //     m.handcard.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, m);

                    // }
                    // // 处理敌方随从食人魔巫术师的效果
                    // if (m.handcard.card.nameCN == CardDB.cardNameCN.食人魔巫术师 && hc.card.type == CardDB.cardtype.SPELL)
                    // {
                    //     ssm++;
                    // }
                    m.handcard.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, m);

                }

                // 处理腐蚀卡的效果
                List<Handmanager.Handcard> afterCorrput = new List<Handmanager.Handcard>();
                foreach (Handmanager.Handcard ohc in this.owncards)
                {
                    if (ohc.card.Corrupt && hc.manacost > ohc.manacost)
                    {
                        // 腐蚀卡的处理
                        ohc.card = CardDB.Instance.getCardDataFromDbfID(ohc.card.CollectionRelatedCardDataBaseId.ToString());
                        ohc.manacost = ohc.getManaCost(this);
                        // if (ohc.card.nameCN == CardDB.cardNameCN.大力士)
                        // {
                        //     ohc.manacost = 0;
                        // }
                        afterCorrput.Add(ohc);
                    }
                }
                foreach (Handmanager.Handcard ohc in afterCorrput)
                {
                    this.removeCard(ohc);
                    this.drawACard(ohc.card.cardIDenum, true, true);
                    foreach (Handmanager.Handcard ahc in this.owncards)
                    {
                        if (ahc.card.cardIDenum == ohc.card.cardIDenum)
                        {
                            ahc.manacost = ohc.manacost;
                        }
                    }
                }

                // 处理己方英雄技能和武器的特殊效果
                // if (this.ownHeroAblility.card.nameEN == CardDB.cardNameEN.voidform)
                // {
                //     this.ownHeroAblility.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, this.ownHeroAblility);
                //     // this.enemyHeroAblility.card.sim_card.onCardWasPlayed(this, hc.card, own, this.enemyHeroAblility);


                // }
                // if (this.ownWeapon.name == CardDB.cardNameEN.atiesh)
                // {
                //     this.callKid(this.getRandomCardForManaMinion(hc.manacost), this.ownMinions.Count, own);
                //     this.lowerWeaponDurability(1, own);
                // }

                // 触发敌方随从的召唤效果
                for (int i = 0; i < burly; i++)
                {
                    this.callKid(CardDB.Instance.burlyrockjaw, this.enemyMinions.Count, !own);
                }
                for (int i = 0; i < ssm; i++)
                {
                    this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.SCH_710t), this.enemyMinions.Count, !own);
                    foreach (Minion m in this.enemyMinions)
                    {
                        if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.SCH_710t)
                        {
                            m.taunt = true;
                            if (m.own) this.anzOwnTaunt++;
                            else this.anzEnemyTaunt++;
                        }
                    }
                }
            }
            else
            {
                // 处理敌方出牌时的特殊效果
                int burly = 0;
                foreach (Minion m in this.enemyMinions.ToArray())
                {
                    if (!m.silenced)
                    {
                        m.handcard.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, m);
                        // m.handcard.card.sim_card.onCardWasPlayed(this, hc.card, own, m);

                    }
                }
                foreach (Minion m in this.ownMinions)
                {
                    if (m.name == CardDB.cardNameEN.troggzortheearthinator)
                    {
                        burly++;
                    }
                    //我方魔能机甲
                    if (m.name == CardDB.cardNameEN.felreaver)
                    {
                        m.handcard.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, m);

                    }
                }

                // 处理敌方英雄技能和武器的特殊效果
                // if (this.enemyHeroAblility.card.nameEN == CardDB.cardNameEN.voidform)
                // {
                //     this.enemyHeroAblility.card.sim_card.onCardIsGoingToBePlayed(this, hc, own, this.enemyHeroAblility);
                //     // this.enemyHeroAblility.card.sim_card.onCardWasPlayed(this, hc.card, own, this.enemyHeroAblility);

                // }
                // if (this.enemyWeapon.name == CardDB.cardNameEN.atiesh)
                // {
                //     this.callKid(this.getRandomCardForManaMinion(hc.manacost), this.enemyMinions.Count, own);
                //     this.lowerWeaponDurability(1, own);
                // }

                // 触发己方随从的召唤效果
                for (int i = 0; i < burly; i++)
                {
                    this.callKid(CardDB.Instance.burlyrockjaw, this.ownMinions.Count, own);
                }
            }
        }

        /// <summary>
        /// 处理一个随从被召唤时的触发效果，根据随从所属的阵营分别处理己方或敌方的召唤事件。
        /// </summary>
        /// <param name="m">被召唤的随从。</param>
        public void triggerAMinionIsSummoned(Minion m)
        {
            // 判断随从是否为己方随从
            if (m.own)
            {
                // 遍历己方随从并触发对应的召唤事件
                foreach (Minion mnn in this.ownMinions.ToArray())
                {
                    // 如果随从被沉默，则跳过
                    if (mnn.silenced) continue;
                    // 调用随从卡牌的召唤触发方法
                    mnn.handcard.card.sim_card.onMinionIsSummoned(this, mnn, m);
                }
            }
            else
            {
                // 遍历敌方随从并触发对应的召唤事件
                foreach (Minion mnn in this.enemyMinions.ToArray())
                {
                    // 如果随从被沉默，则跳过
                    if (mnn.silenced) continue;
                    // 调用随从卡牌的召唤触发方法
                    mnn.handcard.card.sim_card.onMinionIsSummoned(this, mnn, m);
                }
            }
        }

        /// <summary>
        /// 处理随从被召唤后的触发效果，根据随从的阵营分别触发己方或敌方的事件。
        /// </summary>
        /// <param name="mnn">被召唤的随从。</param>
        public void triggerAMinionWasSummoned(Minion mnn)
        {
            if (mnn.own)
            {
                // 处理己方任务的随从召唤触发效果
                if (this.ownQuest.Id != CardDB.cardIDEnum.None)
                {
                    this.ownQuest.trigger_MinionWasSummoned(mnn);
                }

                // 如果召唤的随从具有嘲讽属性，则增加己方嘲讽随从计数
                if (mnn.taunt)
                {
                    anzOwnTaunt++;
                }

                // 如果己方英雄能力为洛瑟玛的力量，并且召唤的是银色黎明新兵，则为其添加圣盾
                if (this.LothraxionsPower && mnn.name == CardDB.cardNameEN.silverhandrecruit)
                {
                    mnn.divineShield = true;
                }

                // 遍历己方随从，触发每个随从的"随从被召唤"效果
                foreach (Minion m in this.ownMinions.ToArray())
                {
                    // 跳过被沉默的随从和刚刚被召唤的随从自身
                    if (m.silenced || m.entityID == mnn.entityID) continue;
                    // 触发随从的 onMinionWasSummoned 事件
                    m.handcard.card.sim_card.onMinionWasSummoned(this, m, mnn);
                }

                // 如果己方装备了正义之剑，则为召唤的随从添加 +1/+1 并降低武器耐久度
                switch (this.ownWeapon.name)
                {
                    case CardDB.cardNameEN.swordofjustice:
                        this.minionGetBuffed(mnn, 1, 1);
                        this.lowerWeaponDurability(1, true);
                        break;
                }
            }
            else
            {
                // 处理敌方任务的随从召唤触发效果
                if (this.enemyQuest.Id != CardDB.cardIDEnum.None)
                {
                    this.enemyQuest.trigger_MinionWasSummoned(mnn);
                }

                // 如果召唤的随从具有嘲讽属性，则增加敌方嘲讽随从计数
                if (mnn.taunt)
                {
                    anzEnemyTaunt++;
                }

                // 遍历敌方随从，触发每个随从的"随从被召唤"效果
                foreach (Minion m in this.enemyMinions.ToArray())
                {
                    // 跳过被沉默的随从和刚刚被召唤的随从自身
                    if (m.silenced || m.entityID == mnn.entityID) continue;
                    // 触发随从的 onMinionWasSummoned 事件
                    m.handcard.card.sim_card.onMinionWasSummoned(this, m, mnn);
                }

                // 如果敌方装备了正义之剑，则为召唤的随从添加 +1/+1 并降低武器耐久度
                switch (this.enemyWeapon.name)
                {
                    case CardDB.cardNameEN.swordofjustice:
                        this.minionGetBuffed(mnn, 1, 1);
                        this.lowerWeaponDurability(1, false);
                        break;
                }
            }
            mnn.handcard.card.sim_card.SummonColossal(this, mnn);

        }

        public void doDeathrattles(List<Minion> deathrattleMinions)
        {
            // 遍历所有触发亡语的随从
            foreach (Minion m in deathrattleMinions)
            {
                // 如果随从未被沉默且拥有亡语效果，触发亡语
                if (!m.silenced && m.handcard.card.deathrattle)
                {
                    m.handcard.card.sim_card.onDeathrattle(this, m);
                }

                // 探险帽效果，增加手牌中的探险帽数量
                if (m.explorershat > 0)
                {
                    for (int i = 0; i < m.explorershat; i++)
                    {
                        drawACard(CardDB.cardNameEN.explorershat, m.own, true);
                    }
                }

                // 返回手牌效果，将该随从卡牌返回手牌
                if (m.returnToHand > 0)
                {
                    drawACard(m.handcard.card.cardIDenum, m.own, true);
                }

                // 感染效果，增加一张河爪豺狼人到手牌
                if (m.infest > 0)
                {
                    for (int i = 0; i < m.infest; i++)
                    {
                        drawACard(CardDB.cardNameEN.rivercrocolisk, m.own, true);
                    }
                }

                // 先祖之魂效果，复活一个相同的随从
                if (m.ancestralspirit > 0)
                {
                    for (int i = 0; i < m.ancestralspirit; i++)
                    {
                        CardDB.Card kid = m.handcard.card;
                        int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                        callKid(kid, pos, m.own, false, true);
                    }
                }

                // 绝望之声效果，复活随从，血量为1
                if (m.desperatestand > 0)
                {
                    for (int i = 0; i < m.desperatestand; i++)
                    {
                        CardDB.Card kid = m.handcard.card;
                        List<Minion> tmp = m.own ? this.ownMinions : this.enemyMinions;
                        int pos = tmp.Count;
                        callKid(kid, pos, m.own, false, true);

                        if (tmp.Count >= 1)
                        {
                            Minion summonedMinion = tmp[pos];
                            if (summonedMinion.handcard.card.cardIDenum == kid.cardIDenum)
                            {
                                summonedMinion.Hp = 1;
                                summonedMinion.wounded = false;
                                if (summonedMinion.Hp < summonedMinion.maxHp)
                                    summonedMinion.wounded = true;
                            }
                        }
                    }
                }

                // 森林之魂效果，召唤一个树人
                for (int i = 0; i < m.souloftheforest; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t); // 树人
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                }

                // 钢铁战犀效果，召唤一个厚甲战马
                for (int i = 0; i < m.stegodon; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_810); // 厚甲战马
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                }

                // 生命孢子效果，召唤两个植物
                for (int i = 0; i < m.livingspores; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_999t2t1); // 植物
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                    callKid(kid, pos, m.own, false, true);
                }
                //绵羊面具,对所有随从造成2点伤害
                for (int i = 0; i < m.sheepmask; i++)
                {
                    allMinionsGetDamage(2);
                }

                //通灵之光，召唤一个狂暴僵尸
                for (int i = 0; i < m.itsnecrolit; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_018t); // 狂暴僵尸
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                }
                for (int i = 0; i < m.greybud; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DMF_734); // 格雷布
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                }
                for (int i = 0; i < m.infected; i++)
                {
                    CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_653); //感染的食尸鬼
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    callKid(kid, pos, m.own, false, true);
                }
                for (int i = 0; i < m.finalsession; i++)//诅咒之旅
                {
                    CardDB.Card kid = m.handcard.card; //死亡的随从
                    int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                    kid.dormant = 2;
                    callKid(kid, pos, m.own, false, true);
                    callKid(kid, pos, m.own, false, true);
                }

                // 如果有额外的亡语效果，触发它
                if (m.deathrattle2 != null)
                {
                    m.deathrattle2.sim_card.onDeathrattle(this, m);
                }

                // 处理里文戴尔男爵效果，双倍触发亡语
                if ((m.own && this.ownBaronRivendare >= 1) || (!m.own && this.enemyBaronRivendare >= 1))
                {
                    int r = m.own ? this.ownBaronRivendare : this.enemyBaronRivendare;
                    for (int j = 0; j < r; j++)
                    {
                        if (!m.silenced && m.handcard.card.deathrattle)
                        {
                            m.handcard.card.sim_card.onDeathrattle(this, m);
                        }

                        // 再次执行上述所有效果以模拟双倍触发
                        if (m.explorershat > 0)
                        {
                            for (int i = 0; i < m.explorershat; i++)
                            {
                                drawACard(CardDB.cardNameEN.explorershat, m.own, true);
                            }
                        }

                        if (m.returnToHand > 0)
                        {
                            drawACard(m.handcard.card.cardIDenum, m.own, true);
                        }

                        if (m.infest > 0)
                        {
                            for (int i = 0; i < m.infest; i++)
                            {
                                drawACard(CardDB.cardNameEN.rivercrocolisk, m.own, true);
                            }
                        }

                        if (m.ancestralspirit > 0)
                        {
                            for (int i = 0; i < m.ancestralspirit; i++)
                            {
                                CardDB.Card kid = m.handcard.card;
                                int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                                callKid(kid, pos, m.own);
                            }
                        }

                        if (m.desperatestand > 0)
                        {
                            for (int i = 0; i < m.desperatestand; i++)
                            {
                                CardDB.Card kid = m.handcard.card;
                                List<Minion> tmp = m.own ? this.ownMinions : this.enemyMinions;
                                int pos = tmp.Count;
                                callKid(kid, pos, m.own, false, true);

                                if (tmp.Count >= 1)
                                {
                                    Minion summonedMinion = tmp[pos];
                                    if (summonedMinion.handcard.card.cardIDenum == kid.cardIDenum)
                                    {
                                        summonedMinion.Hp = 1;
                                        summonedMinion.wounded = false;
                                        if (summonedMinion.Hp < summonedMinion.maxHp)
                                            summonedMinion.wounded = true;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < m.souloftheforest; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t); // 树人
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own);
                        }

                        for (int i = 0; i < m.stegodon; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_810); // 厚甲战马
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own);
                        }

                        for (int i = 0; i < m.livingspores; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_999t2t1); // 植物
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own);
                            callKid(kid, pos, m.own);
                        }

                        //通灵之光，召唤一个狂暴僵尸
                        for (int i = 0; i < m.itsnecrolit; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_018t); // 狂暴僵尸
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own, false, true);
                        }
                        for (int i = 0; i < m.greybud; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DMF_734); // 格雷布
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own, false, true);
                        }
                        for (int i = 0; i < m.infected; i++)
                        {
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.RLK_653); //感染的食尸鬼
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            callKid(kid, pos, m.own, false, true);
                        }
                        for (int i = 0; i < m.finalsession; i++)//诅咒之旅
                        {
                            CardDB.Card kid = m.handcard.card; //死亡的随从
                            int pos = m.own ? this.ownMinions.Count : this.enemyMinions.Count;
                            kid.dormant = 2;
                            callKid(kid, pos, m.own, false, true);
                            callKid(kid, pos, m.own, false, true);
                        }

                        if (m.deathrattle2 != null)
                        {
                            m.deathrattle2.sim_card.onDeathrattle(this, m);
                        }
                    }
                }
            }
        }
    }
}
