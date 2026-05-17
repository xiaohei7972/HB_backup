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
    // 攻击与武器管理：攻击处理链、武器装备与耐久
    public partial class Playfield
    {
        private Handmanager.Handcard FindHandCard(Action a)
        {
            if (a.hc == null) return null;
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                if (hc.entity == a.hc.entity)
                {
                    return hc;
                }
            }
            return a.hc;
        }

        /// <summary>
        /// 执行一个操作，并在此过程中更新场面的evaluatePenality值。
        /// </summary>
        /// <param name="aa">要执行的操作。</param>
        public void doAction(Action aa)
        {
            // 查找目标随从或英雄
            Minion trgt = aa.target != null ? FindMinionByEntityId(aa.target.entityID) : null;
            // 查找执行操作的随从或英雄
            Minion o = aa.own != null ? FindMinionByEntityId(aa.own.entityID) : null;
            // 查找要操作的手牌
            Handmanager.Handcard ha = FindHandCard(aa);

            // 创建并执行操作
            Action a = new Action(aa.actionType, ha, o, aa.place, trgt, aa.penalty, aa.druidchoice);

            // 如果是己方回合，记录操作
            if (this.isOwnTurn)
            {
                this.playactions.Add(a);
            }

            // 根据不同的动作类型执行相应的操作逻辑
            switch (a.actionType)
            {
                case actionEnum.attackWithMinion:
                    HandleMinionAttack(a); // 处理随从攻击
                    break;
                case actionEnum.attackWithHero:
                    attackWithWeapon(a.own, a.target, a.penalty); // 处理英雄攻击
                    break;
                case actionEnum.playcard:
                    if (this.isOwnTurn)
                    {
                        PlayACard(a.hc, a.target, a.place, a.druidchoice, a.penalty); // 打出一张卡牌
                        // HandleTamsinRoameEffect(a); // 处2理塔姆辛·罗姆的效果
                        HandlePatchesSummon(a); // 处理帕奇斯召唤
                        HandleQuestCompletion(); // 处理任务完成
                    }
                    break;
                case actionEnum.useHeroPower:
                    PlayHeroPower(a.target, a.penalty, this.isOwnTurn, a.druidchoice); // 使用英雄技能
                    break;
                case actionEnum.trade:
                    HandleTrade(a); // 处理交易操作
                    break;
                case actionEnum.useLocation:
                    // 记录敌方英雄当前的生命值
                    int enemyHeroHpBefore = this.enemyHero.Hp;
                    if (a.target != null)
                    {
                        Minion targetMinon = FindMinionByEntityId(a.target.entityID);
                        // 使用地标
                        useLocation(a.own, targetMinon);
                    }
                    else
                    {
                        // 使用地标
                        useLocation(a.own, a.target);
                    }
                    // 计算英雄技能执行后敌方英雄的生命值差
                    int damageDealt = enemyHeroHpBefore - this.enemyHero.Hp;
                    if (damageDealt > 0)
                    {
                        this.damageDealtToEnemyHeroThisTurn += damageDealt;
                    }
                    break;
                case actionEnum.useTitanAbility:
                    // 记录敌方英雄当前的生命值
                    enemyHeroHpBefore = this.enemyHero.Hp;
                    if (a.target != null)
                    {
                        Minion targetMinon = FindMinionByEntityId(a.target.entityID);
                        // 使用泰坦技能
                        useTitanAbility(a.own, a.titanAbilityNO, targetMinon);
                    }
                    else
                    {
                        // 使用泰坦技能
                        useTitanAbility(a.own, a.titanAbilityNO, a.target);
                    }
                    // 计算英雄技能执行后敌方英雄的生命值差
                    damageDealt = enemyHeroHpBefore - this.enemyHero.Hp;
                    if (damageDealt > 0)
                    {
                        this.damageDealtToEnemyHeroThisTurn += damageDealt;
                    }
                    break;
                case actionEnum.forge:
                    HandleForge(a); // 处理锻造操作
                    break;
                case actionEnum.launchStarship:
                case actionEnum.useUnderfelRift:
                case actionEnum.rewind: break;
            }
            // UpdateHash((int)aa.actionType, o.entityID, 12, aa.penalty, evaluatePenality);



            // 更新当前回合的操作计数
            if (this.isOwnTurn)
            {
                this.optionsPlayedThisTurn++;
            }
            else
            {
                this.enemyOptionsDoneThisTurn++;
            }

        }

        /// <summary>
        /// 根据实体ID查找随从或英雄。
        /// </summary>
        /// <param name="entityID">实体ID</param>
        /// <returns></returns>
        public Minion FindMinionByEntityId(int entityID)
        {
            if (entityID <= 0) return null;

            if (entityID == this.ownHero.entityID) return this.ownHero;
            if (entityID == this.enemyHero.entityID) return this.enemyHero;

            foreach (var m in ownMinions)
            {
                if (m.entityID == entityID) return m;
            }
            foreach (var m in enemyMinions)
            {
                if (m.entityID == entityID) return m;
            }
            return null;
        }

        /// <summary>
        /// 处理随从攻击逻辑。
        /// </summary>
        private void HandleMinionAttack(Action a)
        {
            this.evaluatePenality += a.penalty;
            Minion target = a.target;

            int newTarget = this.secretTrigger_CharIsAttacked(a.own, target);
            if (newTarget >= 1)
            {
                target = FindMinionByEntityId(newTarget);
            }

            if (a.own != null && a.own.Hp >= 1)
            {
                minionAttacksMinion(a.own, target);
            }
        }

        /// <summary>
        /// 处理塔姆辛·罗姆的特效。
        /// </summary>
        private void HandleTamsinRoameEffect(Action a)
        {
            if (this.anzTamsinroame > 0 && a.hc.card.SpellSchool == CardDB.SpellSchool.SHADOW && a.hc.card.getManaCost(this, a.hc.getManaCost(this)) > 0)
            {
                for (int i = 0; i < this.anzTamsinroame; i++)
                {
                    this.drawACard(a.hc.card.cardIDenum, true, true);
                    this.owncards[this.owncards.Count - 1].manacost = 0;
                    this.evaluatePenality -= 10;
                }
            }
        }

        /// <summary>
        /// 处理帕奇斯的召唤逻辑。
        /// </summary>
        private void HandlePatchesSummon(Action a)
        {
            if (patchesInDeck && (a.hc.card.race == CardDB.Race.PIRATE || a.hc.card.race == CardDB.Race.ALL))
            {
                if (this.ownMinions.Any(m => m.handcard.card.nameCN == CardDB.cardNameCN.海盗帕奇斯) ||
                    this.owncards.Any(hc => hc.card.nameCN == CardDB.cardNameCN.海盗帕奇斯))
                {
                    this.patchesInDeck = false;
                }

                if (this.patchesInDeck)
                {
                    var patchCard = this.prozis.turnDeck.ToArray().FirstOrDefault(kvp => kvp.Key == CardDB.cardIDEnum.CFM_637);
                    if (!patchCard.Equals(default(KeyValuePair<CardDB.cardIDEnum, int>)))
                    {
                        this.callKid(CardDB.Instance.getCardDataFromID(patchCard.Key), this.ownMinions.Count, true);
                        if (this.deckAngrBuff > 0)
                        {
                            this.minionGetBuffed(this.ownMinions.Last(), this.deckAngrBuff, this.deckHpBuff);
                        }
                    }
                    this.patchesInDeck = false;
                }
            }
        }

        /// <summary>
        /// 处理任务完成后的奖励发放。
        /// </summary>
        private void HandleQuestCompletion()
        {
            if (ownQuest.questProgress == ownQuest.maxProgress && ownQuest.Id != CardDB.cardIDEnum.None)
            {
                this.drawACard(ownQuest.Reward(), true);
                ownQuest.Reset();
            }
        }

        /// <summary>
        /// 处理卡牌交易操作。
        /// </summary>
        private void HandleTrade(Action a)
        {
            this.mana -= a.hc.card.TradeCost;
            this.drawACard(CardDB.cardIDEnum.None, true);
            removeCard(a.hc);

            if (this.prozis.turnDeck.ContainsKey(a.hc.card.cardIDenum))
            {
                this.prozis.turnDeck[a.hc.card.cardIDenum]++;
            }
            else
            {
                this.prozis.turnDeck.Add(a.hc.card.cardIDenum, 1);
            }
        }

        /// <summary>
        /// 处理卡牌锻造操作
        /// </summary>
        /// <param name="a"></param>
        private void HandleForge(Action a)
        {
            this.mana -= a.hc.card.ForgeCost;
            a.hc.card = CardDB.Instance.getCardDataFromDbfID(a.hc.card.CollectionRelatedCardDataBaseId.ToString());
            // a.card.card.Forged = true;
        }

        /// <summary>
        /// 处理随从之间的攻击逻辑，包括攻击前后触发的效果和伤害计算。
        /// </summary>
        /// <param name="attacker">攻击方的随从或英雄。</param>
        /// <param name="defender">防守方的随从或英雄。</param>
        /// <param name="dontcount">是否不计入攻击次数，默认值为 false。</param>
        public void minionAttacksMinion(Minion attacker, Minion defender, bool dontcount = false)
        {
            // 如果攻击者或防御者已死亡，或攻击者/防御者无法成为攻击目标，则返回
            if (attacker.Hp <= 0 || attacker.untouchable || defender.untouchable) return;

            int oldHp = defender.Hp;

            // 处理英雄攻击的特殊情况
            if (attacker.isHero)
            {
                HandleHeroAttack(attacker, defender, oldHp);
                return;
            }

            attacker.stealth = false;

            // 处理随从的攻击次数和状态
            if (!dontcount)
            {
                attacker.numAttacksThisTurn++;
                attacker.updateReadyness();
                // if ((attacker.windfury && attacker.numAttacksThisTurn == 2) || !attacker.windfury)
                // {
                //     attacker.Ready = false;
                // }
            }
            else
            {
                attacker.numAttacksThisTurn++;
                attacker.extraAttacksThisTurn++;
                attacker.updateReadyness();

            }

            // 日志记录
            if (logging) Helpfunctions.Instance.logg(".attack with " + attacker.name + " A " + attacker.Angr + " H " + attacker.Hp);

            int attackerAngr = attacker.Angr;
            int defAngr = defender.Angr;

            // 触发攻击前的事件
            // this.triggerAMinionIsGoingToAttack(attacker, defender);
            if (triggerAMinionIsGoingToAttack(attacker, defender))
            {
                return;
            }


            int dmg1 = AdjustDamageForWeapon(attacker, attacker.Angr);

            // 防御者受到伤害
            bool defenderHasDivineShield = defender.divineShield; // 攻击前，防御者是否具有圣盾
            oldHp = defender.Hp;
            defender.getDamageOrHeal(dmg1, this, true, false);
            bool defenderGotDmg = oldHp > defender.Hp;

            // 更新 damageDealtToEnemyHeroThisTurn 字段
            if (defender.isHero && defenderGotDmg)
            {
                this.damageDealtToEnemyHeroThisTurn += (oldHp - defender.Hp); // 记录本次攻击对敌方英雄造成的伤害
            }

            if (defenderGotDmg)
            {
                HandleDefenderDamageEffects(attacker, defender, oldHp, defenderHasDivineShield);
            }

            if (defender.isHero)
            {
                doDmgTriggers();
                return;
            }

            HandleOverkillAndHonorableKill(attacker, defender, oldHp, attackerAngr, defenderHasDivineShield);

            // 攻击者受到伤害
            bool attackerGotDmg = false;
            if (!dontcount)
            {
                oldHp = attacker.Hp;
                attacker.getDamageOrHeal(defAngr, this, true, false);
                attackerGotDmg = oldHp > attacker.Hp;
                if (attackerGotDmg)
                {
                    HandleAttackerDamageEffects(attacker, defender, oldHp);
                }
            }

            // 触发剧毒效果
            if (defenderGotDmg && attacker.poisonous && !defender.isHero)
            {
                minionGetDestroyed(defender);
            }

            if (attackerGotDmg && defender.poisonous && !attacker.isHero)
            {
                minionGetDestroyed(attacker);
            }

            HandlePostAttackEffects(attacker, defender, dontcount);

            this.doDmgTriggers();
        }

        /// <summary>
        /// 处理英雄攻击的逻辑，包括攻击力调整和特殊效果触发。
        /// </summary>
        private void HandleHeroAttack(Minion attacker, Minion defender, int oldHp)
        {
            //英雄攻击时方法
            this.ownWeapon.card.sim_card.onHeroattack(this, this.ownHero, defender);
            //当英雄攻击时,随从触发效果
            foreach (Minion m in this.ownMinions.ToArray())
            {
                if (!m.silenced)
                {
                    m.handcard.card.sim_card.onHeroattack(this, m, defender, this.ownHero);
                }
            }
            int dmg = AdjustDamageForWeapon(attacker, attacker.Angr);
            defender.getDamageOrHeal(dmg, this, true, false);

            // 如果防御者是敌方英雄，更新 damageDealtToEnemyHeroThisTurn 字段
            if (defender.isHero)
            {
                this.damageDealtToEnemyHeroThisTurn += (oldHp - defender.Hp); // 记录本次攻击对敌方英雄造成的伤害
            }

            HandleWeaponSpecialEffects(attacker, defender, oldHp);

            // 触发英雄攻击后的事件
            // if (this.ownWeapon != null && this.ownWeapon.card.sim_card != null)
            if (this.ownWeapon != null)
            {
                List<miniEnch> miniEnchs = this.ownWeapon.enchants;
                System.Action combinedAction = null;
                if (miniEnchs.Count > 0)
                {
                    foreach (miniEnch item in miniEnchs)
                    {
                        System.Action additionalAction = null;

                        if (item.CARDID == CardDB.cardIDEnum.TTN_092t1) // 守护秩序
                        {
                            additionalAction = () =>
                            {
                                // 守护秩序效果：在你的英雄攻击后，抽一张牌
                                this.drawACard(CardDB.cardIDEnum.None, true);
                            };
                        }
                        else if (item.CARDID == CardDB.cardIDEnum.TTN_092t2) // 统帅风范
                        {
                            additionalAction = () =>
                            {
                                // 统帅风范效果：在你的英雄攻击后，召唤一个3/3并具有嘲讽的执行者
                                this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_092e2t), 0, ownHero.own);
                            };
                        }
                        else if (item.CARDID == CardDB.cardIDEnum.TTN_092t3) // 迅疾挥剑
                        {
                            additionalAction = () =>
                            {
                                // 迅疾挥剑效果：英雄在攻击时免疫
                                this.ownHero.immuneWhileAttacking = true;
                            };
                        }

                        // 将每个附魔的效果追加到 combinedAction 中
                        if (additionalAction != null)
                        {
                            if (combinedAction == null)
                            {
                                combinedAction = additionalAction;
                            }
                            else
                            {
                                combinedAction += additionalAction;
                            }
                        }
                    }
                }
                // 调用 ExecuteHeroAttackWithAction 方法，传递组合后的附魔效果
                this.ownWeapon.card.sim_card.ExecuteHeroAttackWithAction(this, this.ownHero, defender, combinedAction);
                //英雄攻击后方法
                this.ownWeapon.card.sim_card.afterHeroattack(this, this.ownHero, defender);
            }
            //当英雄攻击后,随从触发效果
            foreach (Minion m in this.ownMinions.ToArray())
            {
                if (!m.silenced)
                {
                    m.handcard.card.sim_card.afterHeroattack(this, m, defender, this.ownHero);
                }
            }

            // 处理英雄攻击后"小型法术欧珀石"的升级逻辑
            /* foreach (Handmanager.Handcard hc in this.owncards)
            {
                if (hc.card.nameEN == CardDB.cardNameEN.lesseropalspellstone)
                {
                    hc.SCRIPT_DATA_NUM_1 += 1; // 累积攻击次数
                    if (hc.SCRIPT_DATA_NUM_1 >= 2)
                    {
                        hc.SCRIPT_DATA_NUM_1 = 0; // 重置计数
                        hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_645t); // 升级到法术欧珀石
                    }
                }
                else if (hc.card.nameEN == CardDB.cardNameEN.opalspellstone)
                {
                    hc.SCRIPT_DATA_NUM_1 += 1; // 累积攻击次数
                    if (hc.SCRIPT_DATA_NUM_1 >= 2)
                    {
                        hc.SCRIPT_DATA_NUM_1 = 0; // 重置计数
                        hc.card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TOY_645t1); // 升级到大型法术欧珀石
                    }
                }
            } */

            doDmgTriggers();
        }

        /// <summary>
        /// 根据武器调整伤害值，处理武器的耐久度变化。
        /// </summary>
        private int AdjustDamageForWeapon(Minion attacker, int dmg)
        {
            switch (attacker.own ? this.ownWeapon.name : this.enemyWeapon.name)
            {
                case CardDB.cardNameEN.bulwarkofazzinoth:
                    dmg = 1;
                    DecreaseWeaponDurability(attacker.own);
                    break;
            }
            return dmg;
        }

        /// <summary>
        /// 减少武器的耐久度。
        /// </summary>
        private void DecreaseWeaponDurability(bool own)
        {
            if (own)
            {
                this.ownWeapon.Durability--;
            }
            else
            {
                this.enemyWeapon.Durability--;
            }
        }

        /// <summary>
        /// 处理武器的特殊效果，如墓地复仇、巨型符文剑等。
        /// </summary>
        private void HandleWeaponSpecialEffects(Minion attacker, Minion defender, int oldHp)
        {
            if (attacker.own)
            {
                switch (this.ownWeapon.name)
                {
                    case CardDB.cardNameEN.massiveruneblade:
                        if (defender.isHero)
                        {
                            // 巨型符文剑效果：对英雄的伤害翻倍
                            this.ownHero.Angr *= 2;
                        }
                        break;

                    case CardDB.cardNameEN.gravevengeance:
                        if (oldHp > defender.Hp)
                        {
                            // 墓地复仇效果：如果造成了伤害，触发相应的效果
                            this.triggerAMinionDealedDmg(attacker, oldHp - defender.Hp, true);
                        }
                        break;

                    case CardDB.cardNameEN.icebreaker:
                        if (defender.frozen && oldHp > defender.Hp)
                        {
                            // 破冰者效果：如果攻击冻结目标，则直接摧毁目标
                            minionGetDestroyed(defender);
                        }
                        break;
                }
            }
            else
            {
                switch (this.enemyWeapon.name)
                {
                    case CardDB.cardNameEN.bulwarkofazzinoth:
                        // 阿兹诺斯战盾效果：每次攻击只受到1点伤害，并减少耐久度
                        this.enemyWeapon.Durability--;
                        break;
                }
            }
        }

        /// <summary>
        /// 处理防御者受到伤害后的效果，如冻结、吸血等。
        /// </summary>
        private void HandleDefenderDamageEffects(Minion attacker, Minion defender, int oldHp, bool defenderHasDivineShield)
        {
            if (!attacker.silenced)
            {
                switch (attacker.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.voodoohexxer:
                    case CardDB.cardNameEN.snowchugger:
                    case CardDB.cardNameEN.waterelemental:
                        if (!defender.silenced) minionGetFrozen(defender);
                        break;
                }
            }
            this.triggerAMinionDealedDmg(attacker, defender.GotDmgValue, true); // attacker did dmg

            // 处理吸血效果
            if (attacker.lifesteal)
            {
                HealHero(attacker.own, oldHp - defender.Hp);
            }
        }

        /// <summary>
        /// 处理攻击者受到伤害后的效果，如冻结、吸血等。
        /// </summary>
        private void HandleAttackerDamageEffects(Minion attacker, Minion defender, int oldHp)
        {
            if (!defender.silenced)
            {
                switch (defender.handcard.card.nameEN)
                {
                    case CardDB.cardNameEN.voodoohexxer:
                    case CardDB.cardNameEN.voodoohexxer_ICC_088:
                    case CardDB.cardNameEN.snowchugger:
                    case CardDB.cardNameEN.chillomatic:
                    case CardDB.cardNameEN.waterelemental:
                    case CardDB.cardNameEN.waterelemental_CS2_033:
                    case CardDB.cardNameEN.waterelemental_Story_11_WaterElemental:
                    case CardDB.cardNameEN.waterelemental_ICC_833t:
                    case CardDB.cardNameEN.waterelemental_VAC_509t:
                    case CardDB.cardNameEN.waterelemental_VAN_CS2_033:
                    case CardDB.cardNameEN.iceshard_YOD_029t:
                    case CardDB.cardNameEN.snowman:
                    case CardDB.cardNameEN.snowbrute:
                    case CardDB.cardNameEN.frozenstagguard:
                    case CardDB.cardNameEN.sindragosaswing:
                    case CardDB.cardNameEN.sindragosaswing_NX2_037t2:
                    case CardDB.cardNameEN.icehoofprotector:
                        minionGetFrozen(attacker);
                        break;
                    default:
                        break;
                }
            }

            // 处理吸血效果
            if (defender.lifesteal)
            {
                HealHero(defender.own, oldHp - attacker.Hp);
            }

            this.triggerAMinionDealedDmg(defender, attacker.GotDmgValue, false); // defender did dmg
        }

        /// <summary>
        /// 处理超杀和荣誉击杀效果。
        /// </summary>
        private void HandleOverkillAndHonorableKill(Minion attacker, Minion defender, int oldHp, int attackerAngr, bool defenderHasDivineShield)
        {
            if (oldHp < attackerAngr && !defenderHasDivineShield)
            {
                if (!attacker.isHero)
                {
                    attacker.handcard.card.sim_card.OnOverkill(this, attacker);
                }
                else
                {
                    Weapon weapon = attacker.own ? this.ownWeapon : this.enemyWeapon;
                    weapon.card.sim_card.OnOverkill(this, weapon);
                }
            }

            if (oldHp == attackerAngr && !defenderHasDivineShield)
            {
                if (!attacker.isHero)
                {
                    attacker.handcard.card.sim_card.OnHonorableKill(this, attacker, defender);
                }
                else
                {
                    Weapon weapon = attacker.own ? this.ownWeapon : this.enemyWeapon;
                    weapon.card.sim_card.OnHonorableKill(this, weapon, defender);
                }
            }
        }

        /// <summary>
        /// 为英雄恢复生命值，处理吸血效果。
        /// </summary>
        private void HealHero(bool own, int amount)
        {
            this.minionGetDamageOrHeal(own ? this.ownHero : this.enemyHero, -amount);
        }

        /// <summary>
        /// 处理攻击后触发的效果。
        /// </summary>
        private void HandlePostAttackEffects(Minion attacker, Minion defender, bool dontcount)
        {
            if (!attacker.silenced && !dontcount)
            {
                //调用随从攻击后的sim方法
                attacker.handcard.card.sim_card.afterMinionAttack(this, attacker, defender, dontcount);
            }
            if (!defender.silenced)
            {
                defender.handcard.card.sim_card.AfterAttacked(this, defender, attacker);
            }
            // switch (attacker.name)
            // {
            //     case CardDB.cardNameEN.parkpanther:
            //         if (attacker.own)
            //         {
            //             this.minionGetTempBuff(this.ownHero, 3, 0);
            //         }
            //         break;
            //     case CardDB.cardNameEN.theboogeymonster:
            //         if (!defender.isHero && defender.Hp < 1 && attacker.Hp > 0) this.minionGetBuffed(attacker, 2, 2);
            //         break;
            //     case CardDB.cardNameEN.overlordrunthak:
            //         foreach (Handmanager.Handcard hc in this.owncards)
            //         {
            //             if (hc.card.type == CardDB.cardtype.MOB)
            //             {
            //                 hc.addattack++;
            //                 hc.addHp++;
            //                 this.anzOwnExtraAngrHp += 2;
            //             }
            //         }
            //         break;
            //     case CardDB.cardNameEN.windupburglebot:
            //         if (!defender.isHero && attacker.Hp > 0) this.drawACard(CardDB.cardNameEN.unknown, attacker.own);
            //         break;
            //     case CardDB.cardNameEN.lotusassassin:
            //         if (!defender.isHero && defender.Hp < 1 && attacker.Hp > 0) attacker.stealth = true;
            //         break;
            //     case CardDB.cardNameEN.lotusillusionist:
            //         if (defender.isHero) this.minionTransform(attacker, this.getRandomCardForManaMinion(6));
            //         break;
            //     case CardDB.cardNameEN.viciousfledgling:
            //         if (defender.isHero) this.getBestAdapt(attacker);
            //         break;
            //     case CardDB.cardNameEN.knuckles:
            //         if (!defender.isHero && attacker.Hp > 0) this.minionAttacksMinion(attacker, attacker.own ? this.enemyHero : this.ownHero, true);
            //         break;
            //     case CardDB.cardNameEN.finjatheflyingstar:
            //         if (!defender.isHero && defender.Hp < 1)
            //         {
            //             SummonMurlocs(attacker);
            //         }
            //         break;
            //     case CardDB.cardNameEN.giantsandworm:
            //         if (!defender.isHero && defender.Hp < 1 && attacker.Hp > 0)
            //         {
            //             attacker.numAttacksThisTurn = 0;
            //             attacker.Ready = true;
            //         }
            //         break;
            //     case CardDB.cardNameEN.drakonidslayer:
            //     case CardDB.cardNameEN.magnatauralpha:
            //     case CardDB.cardNameEN.lakethresher:
            //     case CardDB.cardNameEN.darkmoonrabbit:
            //     case CardDB.cardNameEN.foereaper4000:
            //         if (!attacker.silenced && !dontcount)
            //         {
            //             AttackAdjacentMinions(attacker, defender);
            //         }
            //         break;
            // }
        }

        /// <summary>
        /// 处理攻击相邻随从的逻辑。
        /// </summary>
        public void AttackAdjacentMinions(Minion attacker, Minion defender)
        {
            List<Minion> temp = (attacker.own) ? this.enemyMinions : this.ownMinions;
            foreach (Minion mnn in temp)
            {
                if (mnn.zonepos + 1 == defender.zonepos || mnn.zonepos - 1 == defender.zonepos)
                {
                    this.minionAttacksMinion(attacker, mnn, true);
                }
            }
        }

        /// <summary>
        /// 处理芬杰的效果，召唤鱼人。
        /// </summary>
        public void SummonMurlocs(Minion attacker)
        {
            if (attacker.own)
            {
                int count = Math.Min(7 - this.ownMinions.Count, 2);
                foreach (KeyValuePair<CardDB.cardIDEnum, int> cid in this.prozis.turnDeck)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(cid.Key);
                    if ((TAG_RACE)c.race == TAG_RACE.MURLOC)
                    {
                        for (int i = 0; i < cid.Value && count > 0; i++, count--)
                        {
                            this.callKid(c, this.ownMinions.Count, true);
                        }
                    }
                }
            }
            else
            {
                this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_168), this.enemyMinions.Count, false);
                this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_168), this.enemyMinions.Count, false);
            }
        }

        /// <summary>
        /// 处理英雄使用武器进行攻击的逻辑，包括武器特效、任务进度更新、目标判定等。
        /// </summary>
        /// <param name="hero">执行攻击的英雄随从。</param>
        /// <param name="target">攻击的目标随从或英雄。</param>
        /// <param name="penality">该操作的惩罚值。</param>
        public void attackWithWeapon(Minion hero, Minion target, int penality)
        {
            bool own = hero.own; // 判断攻击方是否为己方
            Weapon weapon = own ? this.ownWeapon : this.enemyWeapon; // 获取当前使用的武器
            this.attacked = true; // 标记已执行攻击
            this.evaluatePenality += penality; // 累加惩罚值
            hero.numAttacksThisTurn++; // 更新攻击次数

            // 更新英雄是否准备好攻击的状态
            hero.updateReadyness();

            // 特殊武器处理：愚者之剑
            if (weapon.name == CardDB.cardNameEN.foolsbane && !hero.frozen)
            {
                hero.Ready = true;
            }

            // 特殊武器处理：铁刃护手
            if (weapon.card.nameCN == CardDB.cardNameCN.铁刃护手 && target.isHero)
            {
                this.evaluatePenality += 1000;
            }

            // 处理武器的吸血效果
            if (weapon.lifesteal)
            {
                this.minionGetDamageOrHeal(hero, -hero.Angr);
            }

            // 处理保护甲板支线任务
            /* if (this.sideQuest.maxProgress != 1000 && this.sideQuest.Id == CardDB.cardIDEnum.DRG_317)
            {
                this.sideQuest.questProgress++;
                if (this.sideQuest.questProgress >= this.sideQuest.maxProgress)
                {
                    this.drawACard(CardDB.cardIDEnum.CS2_005, true, true);
                    this.drawACard(CardDB.cardIDEnum.CS2_005, true, true);
                    this.drawACard(CardDB.cardIDEnum.CS2_005, true, true);
                    this.sideQuest.Reset();
                }
            } */

            // 处理特殊武器效果
            /*  switch (weapon.name)
             {
                 case CardDB.cardNameEN.truesilverchampion:
                     int heal = own ? this.getMinionHeal(2) : this.getEnemyMinionHeal(2);
                     this.minionGetDamageOrHeal(hero, -heal);
                     doDmgTriggers(); // 触发伤害效果
                     break;
                 case CardDB.cardNameEN.piranhalauncher:
                     int pos = own ? this.ownMinions.Count : this.enemyMinions.Count;
                     this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CFM_337t), pos, own);
                     break;
                 case CardDB.cardNameEN.vinecleaver:
                     int pos2 = own ? this.ownMinions.Count : this.enemyMinions.Count;
                     this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t), pos2, own);
                     this.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t), pos2, own);
                     break;
                 case CardDB.cardNameEN.foolsbane:
                     if (!hero.frozen)
                     {
                         hero.Ready = true; // 愚者之剑的效果使得英雄在攻击后仍然准备好
                     }
                     break;
                 case CardDB.cardNameEN.brassknuckles:
                     if (own)
                     {
                         // 为手牌中的随机随从+1/+1
                         Handmanager.Handcard hc = this.searchRandomMinionInHand(this.owncards, searchmode.searchLowestCost, GAME_TAGs.Mob);
                         if (hc != null)
                         {
                             hc.addattack++;
                             hc.addHp++;
                             this.anzOwnExtraAngrHp += 2;
                         }
                     }
                     else
                     {
                         if (this.enemyAnzCards > 0)
                         {
                             this.anzEnemyExtraAngrHp += this.enemyAnzCards * 2 - 1;
                         }
                     }
                     break;
             } */

            if (logging)
            {
                Helpfunctions.Instance.logg("attack with weapon target: " + target.entityID);
            }

            // 如果目标是英雄，则处理触发的秘密，并可能改变攻击目标
            if (target.isHero)
            {
                int newTarget = this.secretTrigger_CharIsAttacked(hero, target);
                if (newTarget >= 1)
                {
                    // 搜索新的攻击目标
                    foreach (Minion m in this.ownMinions)
                    {
                        if (m.entityID == newTarget)
                        {
                            target = m;
                            break;
                        }
                    }
                    foreach (Minion m in this.enemyMinions)
                    {
                        if (m.entityID == newTarget)
                        {
                            target = m;
                            break;
                        }
                    }
                    if (this.ownHero.entityID == newTarget) target = this.ownHero;
                    if (this.enemyHero.entityID == newTarget) target = this.enemyHero;
                }
            }

            // 执行攻击逻辑
            this.minionAttacksMinion(hero, target);

            // 处理嗜血狂怒的效果：攻击随从时不损失武器耐久
            if (own)
            {
                if (this.ownWeapon.name == CardDB.cardNameEN.gorehowl && !target.isHero)
                {
                    this.ownWeapon.Angr--;
                    hero.Angr--;
                }
                else
                {
                    this.lowerWeaponDurability(1, true); // 减少武器耐久度
                }
            }
            else
            {
                if (this.enemyWeapon.name == CardDB.cardNameEN.gorehowl && !target.isHero)
                {
                    this.enemyWeapon.Angr--;
                    hero.Angr--;
                }
                else
                {
                    this.lowerWeaponDurability(1, false); // 减少敌方武器耐久度
                }
            }

            // 处理友方地标VAC_929（惊险悬崖）的冷却状态
            // foreach (Minion m in this.ownMinions)
            // {
            //     if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_929 && m.CooldownTurn > 0)
            //     {
            //         CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_929);
            //         m.CooldownTurn = 0;
            //         m.handcard.card.CooldownTurn = 0;
            //         m.Ready = true;
            //         Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
            //     }
            // }
        }

        /// <summary>
        /// 为英雄装备武器，并处理相关的状态和触发效果。
        /// </summary>
        /// <param name="c">要装备的武器卡牌</param>
        /// <param name="own">是否为己方英雄</param>
        public void equipWeapon(CardDB.Card c, bool own)
        {
            // 获取对应的英雄
            Minion hero = own ? this.ownHero : this.enemyHero;

            // 处理己方英雄的武器装备逻辑
            if (own)
            {
                if (this.ownWeapon.Durability > 0)
                {
                    bool calcLostWeaponDamage = true;

                    // 处理特定武器的损失计算
                    switch (c.nameEN)
                    {
                        case CardDB.cardNameEN.rustyhook:
                        case CardDB.cardNameEN.poisoneddagger:
                        case CardDB.cardNameEN.wickedknife:
                            if (this.ownWeapon.Angr <= c.Attack && this.ownWeapon.Durability < this.ownWeapon.card.Durability)
                            {
                                calcLostWeaponDamage = false;
                            }
                            break;
                    }

                    // 检查是否存在"毁灭战舰"随从，如果存在则不计算武器损失
                    foreach (Minion m in this.ownMinions)
                    {
                        if (m.handcard.card.nameCN == CardDB.cardNameCN.毁灭战舰)
                        {
                            calcLostWeaponDamage = false;
                        }
                    }

                    // 计算丢失的武器伤害值
                    if (calcLostWeaponDamage)
                    {
                        if (this.ownWeapon.card.cardIDenum == c.cardIDenum) this.lostDamage += 100;
                        if (this.ownWeapon.card.nameCN == CardDB.cardNameCN.海盗之锚) this.lostWeaponDamage += 10;
                        if (this.ownWeapon.card.Durability > 0 && this.ownWeapon.Durability >= this.ownWeapon.card.Durability) this.lostWeaponDamage += 10;
                        if (this.ownWeapon.card.nameCN == CardDB.cardNameCN.逝者之剑) this.lostWeaponDamage += 10;
                        this.lostWeaponDamage += this.ownWeapon.Durability * this.ownWeapon.Angr;
                        if (this.ownWeapon.Angr >= c.Attack) this.lostWeaponDamage += 10;
                    }

                    // 破坏当前武器
                    this.lowerWeaponDurability(1000, true);
                }

                // 装备新武器
                this.ownWeapon.equip(c);

            }
            else
            {
                // 敌方英雄装备武器
                this.lowerWeaponDurability(1000, false);
                this.enemyWeapon.equip(c);
            }

            // 更新英雄的属性
            hero.Angr += c.Attack;
            hero.windfury = c.windfury;
            hero.updateReadyness();
            hero.immuneWhileAttacking = (c.nameEN == CardDB.cardNameEN.gladiatorslongbow);

            // 更新己方或敌方随从状态
            List<Minion> temp = own ? this.ownMinions : this.enemyMinions;
            foreach (Minion m in temp.ToArray())
            {
                switch (m.name)
                {
                    case CardDB.cardNameEN.southseadeckhand:
                        if (m.playedThisTurn) minionGetCharge(m);
                        break;
                    case CardDB.cardNameEN.buccaneer:
                        if (own) this.ownWeapon.Angr++;
                        else this.enemyWeapon.Angr++;
                        break;
                    case CardDB.cardNameEN.smalltimebuccaneer:
                        this.minionGetBuffed(m, 2, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// 降低武器的耐久度，如果耐久度降为0，处理武器的亡语效果，并更新英雄的状态。
        /// </summary>
        /// <param name="value">要降低的耐久度值。</param>
        /// <param name="own">标识是否为己方的武器。</param>
        public void lowerWeaponDurability(int value, bool own)
        {
            // 处理己方武器的耐久度
            if (own)
            {
                if (this.ownWeapon.Durability <= 0 || this.ownWeapon.immune) return; // 如果武器已无耐久度或具有免疫效果，则直接返回

                this.ownWeapon.Durability -= value; // 减少武器的耐久度
                if (this.ownWeapon.Durability <= 0) // 如果武器耐久度降为0或以下
                {
                    HandleWeaponBreak(this.ownWeapon, true); // 处理己方武器破碎的逻辑
                }
            }
            // 处理敌方武器的耐久度
            else
            {
                if (this.enemyWeapon.Durability <= 0 || this.enemyWeapon.immune) return; // 如果武器已无耐久度或具有免疫效果，则直接返回

                this.enemyWeapon.Durability -= value; // 减少武器的耐久度
                if (this.enemyWeapon.Durability <= 0) // 如果武器耐久度降为0或以下
                {
                    HandleWeaponBreak(this.enemyWeapon, false); // 处理敌方武器破碎的逻辑
                }
            }
        }

        /// <summary>
        /// 处理武器破碎后的逻辑，包括亡语触发、英雄攻击力更新、随从效果处理等。
        /// </summary>
        /// <param name="weapon">要处理的武器。</param>
        /// <param name="own">标识是否为己方的武器。</param>
        private void HandleWeaponBreak(Weapon weapon, bool own)
        {
            if (weapon.card.deathrattle)
            {
                Minion m = new Minion { own = own };
                weapon.card.sim_card.onDeathrattle(this, m); // 触发武器的亡语效果
            }

            // 更新英雄的攻击力
            if (own)
            {
                this.ownHero.Angr = Math.Max(0, this.ownHero.Angr - weapon.Angr);
                this.ownWeapon = new Weapon(); // 重置己方武器
                this.ownHero.windfury = false; // 移除风怒效果
                UpdateMinionsAfterWeaponBreak(this.ownMinions, true); // 更新己方随从的状态
                this.ownHero.updateReadyness(); // 更新英雄的准备状态
            }
            else
            {
                this.enemyHero.Angr = Math.Max(0, this.enemyHero.Angr - weapon.Angr);
                this.enemyWeapon = new Weapon(); // 重置敌方武器
                this.enemyHero.windfury = false; // 移除风怒效果
                UpdateMinionsAfterWeaponBreak(this.enemyMinions, false); // 更新敌方随从的状态
                this.enemyHero.updateReadyness(); // 更新英雄的准备状态
            }
        }

        /// <summary>
        /// 在武器破碎后，更新随从的状态，包括移除增益、减少攻击力等。
        /// </summary>
        /// <param name="minions">随从列表。</param>
        /// <param name="own">标识随从是否为己方。</param>
        private void UpdateMinionsAfterWeaponBreak(List<Minion> minions, bool own)
        {
            foreach (Minion m in minions)
            {
                switch (m.name)
                {
                    case CardDB.cardNameEN.southseadeckhand:
                        if (m.playedThisTurn)
                        {
                            m.charge--; // 减少冲锋次数
                            m.updateReadyness(); // 更新随从的准备状态
                        }
                        break;

                    case CardDB.cardNameEN.smalltimebuccaneer:
                        this.minionGetBuffed(m, -2, 0); // 移除因武器存在而获得的攻击力增益
                        break;

                    case CardDB.cardNameEN.graveshambler:
                        if (!m.silenced)
                        {
                            this.minionGetBuffed(m, 1, 1); // 增加墓穴践踏者的攻击力和生命值
                        }
                        break;
                }
            }
        }
    }
}
