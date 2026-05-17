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
    // 出牌与手牌管理：打出卡牌、英雄技能、手牌/牌库操作
    public partial class Playfield
    {
        public void AddToEnemyHand(Handmanager.Handcard card)
        {
            enemyHand.Add(card);
        }

        /// <summary>
        /// 方法：从敌方手牌中移除卡牌
        /// 该方法接收一个 Handmanager.Handcard 对象作为参数，并从敌方手牌列表中移除该卡牌
        /// </summary>
        /// <param name="card"></param>
        public void RemoveFromEnemyHand(Handmanager.Handcard card)
        {
            enemyHand.Remove(card);
        }

        /// <summary>
        /// 将卡牌添加到牌库
        /// </summary>
        /// <param name="card"></param>
        public void AddToDeck(CardDB.Card card)
        {
            ownDeck.Add(card);
        }

        /// <summary>
        /// 移除牌库中的指定牌
        /// </summary>
        /// <param name="card"></param>
        public void RemoveFromDeck(CardDB.Card card)
        {
            ownDeck.Remove(card);
        }

        public void PlayACard(Handmanager.Handcard hc, Minion target, int position, int choice, int penality)
        {
            if (hc == null) hc = new Handmanager.Handcard();
            CardDB.Card c = hc.card;
            this.evaluatePenality += penality;

            // 记录敌方英雄当前的生命值
            int enemyHeroHpBefore = this.enemyHero.Hp;

            // 更新 lastPlayedCardCost 属性
            this.lastPlayedCardCost = c.getManaCost(this, hc.manacost);

            if (hc.card.race == CardDB.Race.ELEMENTAL)
            {
                this.ownElementalsPlayedThisTurn++;
                if (!this.playedElementalThisTurn)
                {
                    this.playedElementalThisTurn = true;
                    Hrtprozis.Instance.ownConsecutiveElementalTurns++;
                }
            }

            // 处理特殊卡牌费用
            HandleSpecialCardCost(hc);

            // 删除其他发现的选项
            if (hc.discovered)
            {
                RemoveOtherDiscoveredCards(hc);
            }


            if (hc != null)
            {
                //处理微缩和扩大牌
                if (hc.card.Miniaturize == 1 && hc.card.Gigantity == 1)
                {

                }
                //微缩
                else if (hc.card.Miniaturize == 1)
                {
                    Handmanager.Handcard miniCard = new Handmanager.Handcard()
                    {
                        card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString()), // 微型版
                        position = hc.position,
                        manacost = 1,
                        entity = this.getNextEntity()
                    };

                    this.owncards[hc.position - 1] = miniCard;
                    renumHandCards(owncards);
                }
                //扩大
                else if (hc.card.Gigantity == 1)
                {
                    Handmanager.Handcard gigaCard = new Handmanager.Handcard()
                    {
                        card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString()), // 微型版
                        position = hc.position,
                        manacost = 8,
                        entity = this.getNextEntity()
                    };
                    this.owncards[hc.position - 1] = gigaCard;
                    renumHandCards(owncards);
                }
                //回响
                else if (hc.card.Echo || hc.card.nonKeywordEcho)
                {
                    Handmanager.Handcard eachCard = new Handmanager.Handcard()
                    {
                        card = hc.card,
                        position = hc.position,
                        manacost = hc.card.cost,
                        entity = this.getNextEntity()
                    };
                    this.owncards[hc.position - 1] = eachCard;
                }
                else
                {
                    switch (hc.card.cardIDenum)
                    {
                        case CardDB.cardIDEnum.RLK_816t3:
                        case CardDB.cardIDEnum.TIME_042t:
                        case CardDB.cardIDEnum.TLC_241t:
                            {
                                hc = new Handmanager.Handcard() { card = hc.card, position = hc.position, manacost = hc.card.cost };
                            }
                            break;
                            /*case CardDB.cardIDEnum.TLC_241t:
                                {
                                    bool flag = false;
                                    foreach (Minion m in ownMinions)
                                    {
                                        if(m.handcard.card.cardIDenum == CardDB.cardIDEnum.TLC_241)
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        hc = new Handmanager.Handcard() { card = hc.card, position = hc.position, manacost = hc.card.cost };
                                    }
                                }*/

                            break;
                        default:

                            if (hc.enchs.Contains(CardDB.cardIDEnum.WW_337e))
                            {
                                hc = new Handmanager.Handcard(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.WW_337e));
                            }
                            else
                            {
                                removeCard(hc);
                            }
                            break;
                    }

                }
                /*switch (true)
                {
                    case (hc.card.Miniaturize == 1 && hc.card.Gigantity == 1):
                        {
                            break;
                        }
                    case (hc.card.Miniaturize == 1):
                        {
                            Handmanager.Handcard miniCard = new Handmanager.Handcard()
                            {
                                card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString()), // 微型版
                                position = hc.position,
                                manacost = 1,
                                entity = this.getNextEntity()
                            };

                            this.owncards[hc.position - 1] = miniCard;
                            renumHandCards(owncards);
                            break;
                        }
                    case (hc.card.Gigantity == 1):
                        {
                            Handmanager.Handcard gigaCard = new Handmanager.Handcard()
                            {
                                card = CardDB.Instance.getCardDataFromDbfID(hc.card.CollectionRelatedCardDataBaseId.ToString()), // 微型版
                                position = hc.position,
                                manacost = 8,
                                entity = this.getNextEntity()
                            };
                            this.owncards[hc.position - 1] = gigaCard;
                            renumHandCards(owncards);
                            break;
                        }
                    case (hc.card.Echo || hc.card.nonKeywordEcho):
                        {
                            Handmanager.Handcard Echo = new Handmanager.Handcard()
                            {
                                card = hc.card,
                                position = hc.position,
                                manacost = hc.getManaCost(this),
                                entity = this.getNextEntity()
                            };
                            this.owncards[hc.position - 1] = Echo;
                            break;
                        }
                    default:
                        {
                            removeCard(hc); // 从手牌中移除该牌
                        }
                        break;*/



            }


            this.triggerCardsChanged(true); // 触发手牌变化的相关事件

            // 更新元素和自然法术标志
            UpdateElementalAndNatureFlags(c);

            // 处理法术卡牌的效果
            if (c.type == CardDB.cardtype.SPELL)
            {
                HandleSpellCardEffects(hc, target, choice);
            }

            hc.target = target;
            this.triggerACardWillBePlayed(hc, true); // 触发即将打出卡牌的事件

            int newTarget = secretTrigger_SpellIsPlayed(target, c); // 处理奥秘触发
            target = UpdateTargetBasedOnSecret(newTarget, target);

            if (newTarget != -2)
            {
                HandleMinionOrSpellPlay(hc, target, position, choice);
            }

            // 计算卡牌效果执行后敌方英雄的生命值差
            int damageDealt = enemyHeroHpBefore - this.enemyHero.Hp;
            if (damageDealt > 0)
            {
                this.damageDealtToEnemyHeroThisTurn += damageDealt;
            }

            // 处理法术反制/扰咒术
            if (newTarget != 0 && target != null)
            {
                HandleCounterspellOrSpellbender(c, target);
            }

            // 处理增益效果
            ApplyBuffEffect(hc);

            // 鹦鹉乐园，战吼随从牌的法力值消耗减少
            if (this.parrotSanctuaryCount > 0 && hc.card.battlecry)
            {
                this.parrotSanctuaryCount = 0;
                // 处理友方地标VAC_409（鹦鹉乐园）的冷却状态
                // foreach (Minion m in this.ownMinions)
                // {
                //     if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_409 && m.CooldownTurn > 0)
                //     {
                //         CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_409);
                //         m.CooldownTurn = 0;
                //         m.handcard.card.CooldownTurn = 0;
                //         m.Ready = true;
                //         Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
                //     }
                // }
            }

            // 检查是否打出了记录的最后一张牌
            // if (hc.entity == this.lastDrawnCardEntityID)
            // {
            //     // 处理友方地标VAC_334（小玩物小屋）的冷却状态
            //     foreach (Minion m in this.ownMinions)
            //     {
            //         if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_334 && m.CooldownTurn > 0)
            //         {
            //             CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_334);
            //             m.CooldownTurn = 0;
            //             m.handcard.card.CooldownTurn = 0;
            //             m.Ready = true;
            //             Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
            //         }
            //     }

            //     // 重置lastDrawnCardEntityID
            //     this.lastDrawnCardEntityID = -1;
            // }

            // 检查是否打出了法术牌
            // if (c.type == CardDB.cardtype.SPELL)
            // {
            //     // 处理友方地标VAC_522（潮汐之池）的冷却状态
            //     foreach (Minion m in this.ownMinions)
            //     {
            //         if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.VAC_522 && m.CooldownTurn > 0)
            //         {
            //             CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.VAC_522);
            //             m.CooldownTurn = 0;
            //             m.handcard.card.CooldownTurn = 0;
            //             m.Ready = true;
            //             Helpfunctions.Instance.logg("卡牌名称 - " + card.nameCN.ToString() + " " + card.cardIDenum.ToString() + " 地标冷却回合 - 0");
            //         }
            //     }
            // }

            this.cardsPlayedThisTurn++; // 更新本回合打出的牌数
        }

        /// <summary>
        /// 处理卡牌的特殊费用逻辑，如用血量支付法术/鱼人牌的费用。
        /// </summary>
        private void HandleSpecialCardCost(Handmanager.Handcard hc)
        {
            // switch (hc.card.CardAlternateCost)
            // {
            //     case 1:
            //         {
            //             PayHealthForSpell(hc);

            //         }
            //         break;
            //     case 2:
            //         {

            //         }
            //         break;
            //     case 3:
            //         {

            //         }
            //         break;
            //     default:
            {
                if (this.nextSpellThisTurnCostHealth && hc.card.type == CardDB.cardtype.SPELL)
                {
                    PayHealthForSpell(hc);
                }
                else if (this.nextMurlocThisTurnCostHealth && (hc.card.race == CardDB.Race.MURLOC || hc.card.race == CardDB.Race.ALL))
                {
                    PayHealthForMurloc(hc);
                }
                else if (this.nextAnyCardThisTurnCostEnemyHealth)
                {
                    PayEnemyHealthForAnyCard(hc);
                }
                else if (this.ownDemonCostLessOnce > 0 && (hc.card.race == CardDB.Race.DEMON || hc.card.race == CardDB.Race.ALL))
                {
                    this.ownDemonCostLessOnce = 0; // 恶魔卡牌法力消耗减少
                }
                else
                {
                    this.mana -= hc.getManaCost(this); // 正常减少法力值
                }
            }
            //         break;
            // }

        }

        /// <summary>
        /// 用血量支付法术牌的费用。
        /// </summary>
        private void PayHealthForSpell(Handmanager.Handcard hc)
        {
            this.minionGetDamageOrHeal(this.ownHero, hc.card.getManaCost(this, hc.manacost));
            doDmgTriggers();
            this.nextSpellThisTurnCostHealth = false;
        }



        /// <summary>
        /// 用血量支付鱼人牌的费用。
        /// </summary>
        private void PayHealthForMurloc(Handmanager.Handcard hc)
        {
            this.minionGetDamageOrHeal(this.ownHero, hc.card.getManaCost(this, hc.manacost));
            doDmgTriggers();
            this.nextMurlocThisTurnCostHealth = false;
        }

        /// <summary>
        /// 用敌方英雄血量支付任何牌的费用。
        /// </summary>
        private void PayEnemyHealthForAnyCard(Handmanager.Handcard hc)
        {
            this.minionGetDamageOrHeal(this.enemyHero, hc.card.getManaCost(this, hc.manacost));
            doDmgTriggers();
            this.nextAnyCardThisTurnCostEnemyHealth = false;
        }

        /// <summary>
        /// 移除其他发现的卡牌。
        /// </summary>
        private void RemoveOtherDiscoveredCards(Handmanager.Handcard hc)
        {
            foreach (Handmanager.Handcard hcc in this.owncards.ToArray())
            {
                if (hcc.discovered && hcc.entity != hc.entity)
                {
                    removeCard(hcc);
                }
            }
        }

        /// <summary>
        /// 更新元素和自然法术的标志位。
        /// </summary>
        private void UpdateElementalAndNatureFlags(CardDB.Card c)
        {
            UpdateElementalFlag();
            UpdateNatureSpellFlag(c);
            this.anzOwnElementalsThisTurn = 0;
        }

        /// <summary>
        /// 更新元素生物的标志位。
        /// </summary>
        private void UpdateElementalFlag()
        {
            foreach (Minion elm in this.ownMinions)
            {
                if ((TAG_RACE)elm.handcard.card.race == TAG_RACE.ELEMENTAL)
                {
                    this.anzOwnElementalsThisTurn = 1;
                    this.anzOwnElementalsLastTurn = this.anzOwnElementalsThisTurn;
                    break;
                }
            }
        }


        /// <summary>
        /// 更新自然法术的标志位。
        /// </summary>
        private void UpdateNatureSpellFlag(CardDB.Card c)
        {
            if (c.SpellSchool == CardDB.SpellSchool.NATURE)
            {
                foreach (Handmanager.Handcard ohc in this.owncards)
                {
                    if (ohc.card.nameCN == CardDB.cardNameCN.自然使徒)
                    {
                        this.useNature = 1;
                        prozis.useNature = 1;
                        break;
                    }
                }
            }
            else
            {
                this.useNature = 0;
                prozis.useNature = 0;
            }
        }

        /// <summary>
        /// 处理法术卡牌的效果，包括目标选择和特殊触发。
        /// </summary>
        private void HandleSpellCardEffects(Handmanager.Handcard hc, Minion target, int choice)
        {
            this.playedPreparation = false; // 清除伺机待发标志
            this.spellsplayedSinceRecalc++;

            // 处理暗影布缝针的效果
            if (this.ownWeapon != null && this.ownWeapon.Durability > 0 && this.ownWeapon.card.nameCN == CardDB.cardNameCN.暗影布缝针 && hc.card.SpellSchool == CardDB.SpellSchool.SHADOW)
            {
                this.allCharsOfASideGetDamage(false, 1);
                this.evaluatePenality -= (this.enemyMinions.Count + 1) * 4;
                this.ownWeapon.Durability--;
            }

            // 处理法术目标的特殊效果
            HandleSpellTargetEffects(hc, target, choice);

            // 处理奥秘牌
            if (hc.card.Secret)
            {
                this.ownSecretsIDList.Add(hc.card.cardIDenum);
                this.nextSecretThisTurnCost0 = false;
                this.secretsplayedSinceRecalc++;
            }
        }

        /// <summary>
        /// 处理法术目标的特殊效果。
        /// </summary>
        private void HandleSpellTargetEffects(Handmanager.Handcard hc, Minion target, int choice)
        {
            //这卡没几个人用注释掉优化性能，有需要再自己取消注释
            // if (target != null && target.own)
            // {
            //     switch (target.name)
            //     {
            //         case CardDB.cardNameEN.dragonkinsorcerer:
            //             this.minionGetBuffed(target, 1, 1);
            //             break;
            //         case CardDB.cardNameEN.eydisdarkbane:
            //             Minion mTarget = this.getEnemyCharTargetForRandomSingleDamage(3);
            //             this.minionGetDamageOrHeal(mTarget, 3, true);
            //             break;
            //         case CardDB.cardNameEN.fjolalightbane:
            //             target.divineShield = true;
            //             break;
            //         default:
            //             break;
            //     }
            // }
        }

        /// <summary>
        /// 处理随从或法术牌的效果。
        /// 2025-08-10添加武器牌和英雄牌
        /// </summary>
        private void HandleMinionOrSpellPlay(Handmanager.Handcard hc, Minion target, int position, int choice)
        {
            switch (hc.card.type)
            {
                case CardDB.cardtype.MOB:
                    HandleMinionPlay(hc, position, choice);
                    break;
                case CardDB.cardtype.SPELL:
                    HandleSpellPlay(hc, target, choice);
                    break;

                case CardDB.cardtype.WEAPON:
                    HandleWeaponPlay(hc, target, choice);
                    break;

                case CardDB.cardtype.HERO:
                    HandleHeroPlay(hc, choice);
                    break;

                case CardDB.cardtype.LOCATION:
                    HandleLocationPlay(hc, position, choice);
                    break;

                default:
                    HandleSpellPlay(hc, target, choice);
                    break;
            }
        }

        /// <summary>
        /// 处理地标卡牌的效果。
        /// </summary>
        /// <param name="hc"></param>
        /// <param name="position"></param>
        /// <param name="choice"></param>
        private void HandleLocationPlay(Handmanager.Handcard hc, int position, int choice)
        {
            if (this.ownMinions.Count < 7)
            {
                // 创建一个新的随从并设置其为手牌中打出
                Minion m = createNewMinion(hc, position, true);
                m.playedFromHand = true;

                // 将随从添加到战场
                addMinionToBattlefield(m);
                // 记录日志信息
                if (logging) Helpfunctions.Instance.logg("added " + m.handcard.card.nameEN);
                this.mobsplayedThisTurn++;
            }
        }
        /// <summary>
        ///  处理武器牌的效果。
        /// </summary>
        /// <param name="hc"></param>
        /// <param name="target"></param>
        /// <param name="choice"></param>
        private void HandleWeaponPlay(Handmanager.Handcard hc, Minion target, int choice)
        {
            this.equipWeapon(hc.card, true);
            hc.card.sim_card.onCardPlay(this, true, hc.target, choice, hc);
            hc.card.sim_card.getBattlecryEffect(this, this.ownWeapon, hc.target, choice);
            if (this.ownBrannBronzebeard > 0)
            {
                for (int i = 0; i < this.ownBrannBronzebeard; i++)
                {
                    hc.card.sim_card.getBattlecryEffect(this, this.ownWeapon, hc.target, choice);
                }
            }

        }
        /// <summary>
        /// 处理英雄牌的效果。
        /// </summary>
        /// <param name="hc"></param>
        /// <param name="choice"></param>
        private void HandleHeroPlay(Handmanager.Handcard hc, int choice)
        {

            hc.card.sim_card.getBattlecryEffect(this, this.ownHero, hc.target, choice);
            if (this.ownBrannBronzebeard > 0)
            {
                for (int i = 0; i < this.ownBrannBronzebeard; i++)
                {
                    hc.card.sim_card.getBattlecryEffect(this, this.ownHero, hc.target, choice);
                }
            }
            hc.card.sim_card.onCardPlay(this, this.ownHero, hc.target, choice, hc);
            this.setNewHeroPower(CardDB.Instance.getCardDataFromDbfID(hc.card.heroPower.ToString()).cardIDenum, true);
            this.minionGetArmor(this.ownHero, hc.card.armor);
            Minion hero = new Minion
            {
                Hp = ownHero.Hp,
                maxHp = ownHero.maxHp,
                Angr = ownHero.Angr,
                handcard = new Handmanager.Handcard(hc),
                cardClass = hc.card.KeepHeroClass == 1 ? this.ownHeroStartClass : (TAG_CLASS)hc.card.Class,
                own = true,
                isHero = true,
                entityID = hc.entity,
                // playedThisTurn = true,
                // numAttacksThisTurn = 0,
                divineShield = ownHero.divineShield,
                lifesteal = ownHero.lifesteal,
                // stealth = ownHero.stealth,
                name = hc.card.nameEN,
                nameCN = hc.card.nameCN,
            };
            ownHero = hero;
            ownHeroStartClass = hero.cardClass;

        }



        /// <summary>
        /// 处理随从卡牌的效果，包括召唤、群兽奔腾等。
        /// </summary>
        private void HandleMinionPlay(Handmanager.Handcard hc, int position, int choice)
        {
            if (this.ownMinions.Count < 7)
            {
                this.placeAmobSomewhere(hc, choice, position); // 放置随从到战场
                this.mobsplayedThisTurn++;
            }

            // 处理奔踏效果
            if (this.stampede > 0 && (TAG_RACE)hc.card.race == TAG_RACE.PET)
            {
                for (int i = 1; i <= stampede; i++)
                {
                    this.drawACard(CardDB.cardNameEN.unknown, true, true); // 随机获取一张野兽
                }
            }

            // 处理流放效果
            // HandleOutcastEffect(hc, hc.card.Outcast);
        }

        /// <summary>
        /// 处理法术卡牌的效果。
        /// </summary>
        private void HandleSpellPlay(Handmanager.Handcard hc, Minion target, int choice)
        {
            // 处理锁定装载特效
            // if (this.lockandload > 0 && hc.card.type == CardDB.cardtype.SPELL)
            // {
            //     for (int i = 1; i <= lockandload; i++)
            //     {
            //     this.drawACard(CardDB.cardNameEN.unknown, true, true);
            //     }
            // }

            // 处理流放效果
            HandleOutcastEffect(hc, hc.card.Outcast);

            // 处理日蚀的效果
            if (this.anzSolor && hc.card.type == CardDB.cardtype.SPELL)
            {
                if (hc.card.nameCN == CardDB.cardNameCN.日蚀)
                {
                    this.evaluatePenality += 1000;
                }
                hc.card.sim_card.onCardPlay(this, true, target, choice, hc);
                hc.card.sim_card.onCardPlay(this, true, target, choice, hc);
                this.anzSolor = false;
            }

            // 执行卡牌的主要效果
            hc.card.sim_card.onCardPlay(this, true, target, choice, hc);
            hc.card.sim_card.onCardPlay(this, true, target, choice, hc);
            this.ownQuest.trigger_SpellWasPlayed(hc, target, 0);
            // 处理法术迸发效果
            HandleSpellburstEffect(hc);
        }

        /// <summary>
        /// 处理流放效果。
        /// </summary>
        private void HandleOutcastEffect(Handmanager.Handcard hc, bool outcast)
        {
            if (outcast && (hc.position == 1 || hc.position == this.owncards.Count + 1))
            {
                hc.card.sim_card.onCardPlay(this, true, hc.target, hc.extraParam2, true);
                this.evaluatePenality--; // 优先打出右手边的流放
            }
        }

        /// <summary>
        /// 处理法术迸发效果。
        /// </summary>
        private void HandleSpellburstEffect(Handmanager.Handcard hc)
        {
            if (hc.card.type == CardDB.cardtype.SPELL)
            {
                foreach (Minion m in this.ownMinions.ToArray())
                {
                    if (m.Spellburst && !m.silenced)
                    {
                        m.handcard.card.sim_card.OnSpellburst(this, m, hc);
                        m.Spellburst = false;
                    }
                }
                if (this.ownWeapon.Spellburst)
                {
                    this.ownWeapon.card.sim_card.OnSpellburst(this, this.ownWeapon, hc);
                    this.ownWeapon.Spellburst = false;
                }
            }
        }

        /// <summary>
        /// 应用卡牌的增益效果。
        /// </summary>
        private void ApplyBuffEffect(Handmanager.Handcard hc)
        {
            if (hc.target != null && prozis.penman.healthBuffDatabase.ContainsKey(hc.card.nameEN))
            {
                hc.target.justBuffed += prozis.penman.healthBuffDatabase[hc.card.nameEN];
            }
        }

        /// <summary>
        /// 执行英雄技能的逻辑，包括处理技能使用次数、法力消耗、目标选择、触发效果等。
        /// </summary>
        /// <param name="target">技能的目标随从或英雄。</param>
        /// <param name="penality">执行该操作的惩罚值。</param>
        /// <param name="ownturn">标识是否为己方回合。</param>
        /// <param name="choice">技能的选择参数（用于二选一或类似效果）。</param>
        public void PlayHeroPower(Minion target, int penality, bool ownturn, int choice)
        {
            // 获取当前回合使用的英雄技能卡牌
            CardDB.Card c = ownturn ? this.ownHeroAblility.card : this.enemyHeroAblility.card;

            // 记录敌方英雄当前的生命值
            int enemyHeroHpBefore = this.enemyHero.Hp;

            // 处理技能使用次数和状态更新
            if (ownturn)
            {
                this.anzUsedOwnHeroPower++; // 更新己方英雄技能使用次数
                                            // 检查是否超过使用限制
                if (this.anzUsedOwnHeroPower >= this.ownHeroPowerAllowedQuantity)
                {
                    this.ownAbilityReady = false; // 超过限制后，标记技能未准备好
                }
            }
            else
            {
                this.anzUsedEnemyHeroPower++; // 更新敌方英雄技能使用次数
                if (this.anzUsedEnemyHeroPower >= this.enemyHeroPowerAllowedQuantity)
                {
                    this.enemyAbilityReady = false; // 超过限制后，标记技能未准备好
                }
            }

            // 累加惩罚值
            this.evaluatePenality += penality;

            // 处理特殊情况：如黑眼任务术中的法力消耗调整
            // if (this.ownHeroPowerCostLessOnce <= -2 && Ai.Instance.botBase.BehaviorName() == "黑眼任务术")
            // {
            //     this.evaluatePenality -= 20; // 减少惩罚值
            // }

            // 计算并减少法力值，确保消耗至少为0
            int manaCost = Math.Max(0, this.ownHeroAblility.manacost + this.ownHeroPowerCostLessOnce);
            this.mana -= manaCost;

            if (logging) Helpfunctions.Instance.logg("play hero power " + c.nameEN + " on target " + target);

            // 执行英雄技能的主要效果
            c.sim_card.onCardPlay(this, ownturn, target, choice, this.ownHeroAblility);

            // 计算英雄技能执行后敌方英雄的生命值差
            int damageDealt = enemyHeroHpBefore - this.enemyHero.Hp;
            if (damageDealt > 0)
            {
                this.damageDealtToEnemyHeroThisTurn += damageDealt;
            }

            // 重置技能法力消耗的临时减少效果
            this.ownHeroPowerCostLessOnce = 0;

            // 处理目标被冻结的效果
            if (target != null && (ownturn ? this.ownAbilityFreezesTarget > 0 : this.enemyAbilityFreezesTarget > 0))
            {
                minionGetFrozen(target); // 冻结目标
            }

            // 触发激励效果
            this.triggerInspire(ownturn);

            // 触发奥秘：英雄技能使用
            this.secretTrigger_HeroPowerUsed();

            // 触发所有的伤害效果
            this.doDmgTriggers();
        }

        public void drawACard(CardDB.cardNameEN ss, bool own, bool nopen = false)
        {
            CardDB.cardNameEN s = ss;

            // 检查是否为己方玩家抽卡
            if (own)
            {
                // 处理未知卡牌（即从牌库顶抽取的卡牌）
                if (s == CardDB.cardNameEN.unknown && !nopen)
                {
                    // 牌库为空，触发疲劳伤害
                    if (ownDeckSize == 0)
                    {
                        this.ownHeroFatigue++;
                        this.ownHero.getDamageOrHeal(this.ownHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        // 从牌库中抽卡
                        this.ownDeckSize--;
                        // 手牌已满（10张），无法再抽牌
                        if (this.owncards.Count >= 10)
                        {
                            return;
                        }
                        this.owncarddraw++;
                    }
                }
                else
                {
                    // 手牌已满（10张），无法再抽牌
                    if (this.owncards.Count >= 10)
                    {
                        return;
                    }
                    this.owncarddraw++;
                }
            }
            else // 处理敌方玩家抽卡
            {
                if (s == CardDB.cardNameEN.unknown && !nopen)
                {
                    // 牌库为空，触发疲劳伤害
                    if (enemyDeckSize == 0)
                    {
                        this.enemyHeroFatigue++;
                        this.enemyHero.getDamageOrHeal(this.enemyHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        // 从牌库中抽卡
                        this.enemyDeckSize--;
                        // 手牌已满（10张），无法再抽牌
                        if (this.enemyAnzCards >= 10)
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                    }
                }
                else
                {
                    // 手牌已满（10张），无法再抽牌
                    if (this.enemyAnzCards >= 10)
                    {
                        return;
                    }
                    this.enemycarddraw++;
                    this.enemyAnzCards++;
                }
                this.triggerCardsChanged(false);

                // 处理敌方的Chromaggus（克洛玛古斯）效果，额外抽卡
                if (anzEnemyChromaggus > 0 && s == CardDB.cardNameEN.unknown && !nopen)
                {
                    for (int i = 1; i <= anzEnemyChromaggus; i++)
                    {
                        if (this.enemyAnzCards >= 10)
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                        this.triggerCardsChanged(false);
                    }
                }
                return;
            }

            // 处理未知卡牌的抽取和添加到手牌中
            if (s == CardDB.cardNameEN.unknown)
            {
                CardDB.Card c = CardDB.Instance.getCardData(s);
                Handmanager.Handcard hc = new Handmanager.Handcard
                {
                    card = c,
                    position = this.owncards.Count + 1,
                    manacost = 1000,
                    entity = this.getNextEntity()
                };
                this.owncards.Add(hc);
                this.triggerCardsChanged(true);
            }
            else // 处理指定卡牌的抽取
            {
                CardDB.Card c = CardDB.Instance.getCardData(s);
                Handmanager.Handcard hc = new Handmanager.Handcard
                {
                    card = c,
                    position = this.owncards.Count + 1,
                    manacost = c.calculateManaCost(this),
                    entity = this.getNextEntity()
                };
                this.owncards.Add(hc);
                this.triggerCardsChanged(true);
            }

            // 处理己方的Chromaggus（克洛玛古斯）效果，额外抽卡
            if (anzOwnChromaggus > 0 && s == CardDB.cardNameEN.unknown && !nopen)
            {
                CardDB.Card c = CardDB.Instance.getCardData(s);
                for (int i = 1; i <= anzOwnChromaggus; i++)
                {
                    if (this.owncards.Count >= 10)
                    {
                        return;
                    }
                    this.owncarddraw++;

                    Handmanager.Handcard hc = new Handmanager.Handcard
                    {
                        card = c,
                        position = this.owncards.Count + 1,
                        manacost = 1000,
                        entity = this.getNextEntity()
                    };
                    this.owncards.Add(hc);
                    this.triggerCardsChanged(true);
                }
            }
        }

        /// <summary>
        /// 抽取一张卡牌并添加到玩家手牌中。如果手牌已满或牌库为空，会触发相应的惩罚或疲劳伤害。
        /// </summary>
        /// <param name="ss">要抽取的卡牌的枚举ID。</param>
        /// <param name="own">如果为 true，则表示为己方玩家抽卡；否则为敌方玩家抽卡。</param>
        /// <param name="nopen">如果为 true，则不进行抽卡操作。</param>
        public void drawACard(CardDB.cardIDEnum ss, bool own, bool nopen = false)
        {
            CardDB.cardIDEnum s = ss;

            // 处理己方玩家的抽卡逻辑
            if (own)
            {
                // 处理从牌库顶抽取的未知卡牌
                if (s == CardDB.cardIDEnum.None && !nopen)
                {
                    if (ownDeckSize == 0) // 牌库为空，触发疲劳伤害
                    {
                        this.ownHeroFatigue++;
                        this.ownHero.getDamageOrHeal(this.ownHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        // 从牌库中抽卡
                        this.ownDeckSize--;
                        if (this.owncards.Count >= 10) // 手牌已满（10张），无法再抽牌
                        {
                            return;
                        }
                        this.owncarddraw++;

                        // 符文秘银杖效果，每当抽一张牌时增加其计数
                        if (this.ownWeapon != null && this.ownWeapon.card.nameCN == CardDB.cardNameCN.符文秘银杖)
                        {
                            this.ownWeapon.scriptNum1++;
                        }
                    }
                }
                else
                {
                    if (this.owncards.Count >= 10) // 手牌已满（10张），无法再抽牌
                    {
                        return;
                    }
                    this.owncarddraw++;
                }
            }
            else // 处理敌方玩家的抽卡逻辑
            {
                if (s == CardDB.cardIDEnum.None && !nopen)
                {
                    if (enemyDeckSize == 0) // 牌库为空，触发疲劳伤害
                    {
                        this.enemyHeroFatigue++;
                        this.enemyHero.getDamageOrHeal(this.enemyHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        // 从牌库中抽卡
                        this.enemyDeckSize--;
                        if (this.enemyAnzCards >= 10) // 手牌已满（10张），无法再抽牌
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                    }
                }
                else
                {
                    if (this.enemyAnzCards >= 10) // 手牌已满（10张），无法再抽牌
                    {
                        return;
                    }
                    this.enemycarddraw++;
                    this.enemyAnzCards++;
                }
                this.triggerCardsChanged(false);

                // 处理敌方的Chromaggus（克洛玛古斯）效果，额外抽卡
                if (anzEnemyChromaggus > 0 && s == CardDB.cardIDEnum.None && !nopen)
                {
                    for (int i = 1; i <= anzEnemyChromaggus; i++)
                    {
                        if (this.enemyAnzCards >= 10) // 手牌已满（10张），无法再抽牌
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                        this.triggerCardsChanged(false);
                    }
                }
                return;
            }

            // 处理未知卡牌的抽取和添加到手牌中
            if (s == CardDB.cardIDEnum.None)
            {
                CardDB.Card c = CardDB.Instance.getCardDataFromID(s);
                Handmanager.Handcard hc = new Handmanager.Handcard
                {
                    card = c,
                    position = this.owncards.Count + 1,
                    manacost = 1000,
                    entity = this.getNextEntity()
                };
                this.owncards.Add(hc);
                this.triggerCardsChanged(true);
            }
            else // 处理指定卡牌的抽取
            {
                CardDB.Card c = CardDB.Instance.getCardDataFromID(s);
                Handmanager.Handcard hc = new Handmanager.Handcard
                {
                    card = c,
                    position = this.owncards.Count + 1,
                    manacost = c.calculateManaCost(this),
                    entity = this.getNextEntity()
                };
                //TODO:抽到时释放，兄弟抽未知的牌时,不知道这牌是什么,不好做这个效果
                // if (c.CastsWhenDrawn == 1)
                // {
                //     c.sim_card.castsWhenDrawn(this, hc, own);
                // }
                this.owncards.Add(hc);
                this.triggerCardsChanged(true);
            }

            // 处理己方的Chromaggus（克洛玛古斯）效果，额外抽卡
            if (anzOwnChromaggus > 0 && s == CardDB.cardIDEnum.None && !nopen)
            {
                CardDB.Card c = CardDB.Instance.getCardDataFromID(s);
                for (int i = 1; i <= anzOwnChromaggus; i++)
                {
                    if (this.owncards.Count >= 10) // 手牌已满（10张），无法再抽牌
                    {
                        return;
                    }
                    this.owncarddraw++;

                    Handmanager.Handcard hc = new Handmanager.Handcard
                    {
                        card = c,
                        position = this.owncards.Count + 1,
                        manacost = 1000,
                        entity = this.getNextEntity()
                    };
                    this.owncards.Add(hc);
                    this.triggerCardsChanged(true);
                }
            }
        }

        /// <summary>
        /// 从手牌中移除一张指定的卡牌，并重新编号手牌位置。
        /// </summary>
        /// <param name="hcc">要移除的卡牌。</param>
        public void removeCard(Handmanager.Handcard hcc)
        {
            int cardPos = 1; // 用于更新卡牌位置的计数器
            Handmanager.Handcard hcToRemove = null;

            // 遍历手牌，查找要移除的卡牌，并重新编号其余卡牌的位置
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                if (hc.entity == hcc.entity) // 找到要移除的卡牌
                {
                    hcToRemove = hc;
                    continue; // 跳过要移除的卡牌，不更新其位置
                }
                hc.position = cardPos; // 更新卡牌位置
                cardPos++;
            }

            // 如果找到了要移除的卡牌，从手牌中移除
            if (hcToRemove != null)
            {
                this.owncards.Remove(hcToRemove);
            }
        }

        /// <summary>
        /// 重新编号手牌中所有卡牌的位置，使它们的顺序与实际手牌中的顺序一致。
        /// </summary>
        /// <param name="list">要重新编号的手牌列表。</param>
        public void renumHandCards(List<Handmanager.Handcard> list)
        {
            int count = list.Count;

            // 重新为手牌中的每张卡牌分配位置
            for (int i = 0; i < count; i++)
            {
                list[i].position = i + 1;
            }
        }

        public void discardCards(int num, bool own)
        {
            if (own)
            {
                int anz = Math.Min(num, this.owncards.Count);
                if (anz < 1) return; // 如果没有需要弃掉的牌，直接返回

                int numDiscardedCards = anz;
                List<Handmanager.Handcard> discardedCardsBonusList = new List<Handmanager.Handcard>();
                Dictionary<int, int> cardsValDict = new Dictionary<int, int>();

                int bestCardValue = -1, bestCardPos = -1;
                int bestSecondCardValue = -1, bestSecondCardPos = -1;

                // 计算每张牌的价值并找出价值最高的两张牌
                for (int i = 0; i < this.owncards.Count; i++)
                {
                    Handmanager.Handcard hc = this.owncards[i];
                    CardDB.Card c = hc.card;
                    int cardValue = calculateCardValue(hc, c);
                    int ratio = getCardValueRatio(hc.manacost);

                    // 保存卡牌价值的最终计算值
                    cardsValDict.Add(hc.entity, cardValue * ratio / 100);

                    // 更新最有价值的两张牌的位置和价值
                    if (bestCardValue <= cardValue)
                    {
                        bestSecondCardValue = bestCardValue;
                        bestSecondCardPos = bestCardPos;
                        bestCardValue = cardValue;
                        bestCardPos = i;
                    }
                    else if (bestSecondCardValue <= cardValue)
                    {
                        bestSecondCardValue = cardValue;
                        bestSecondCardPos = i;
                    }
                }

                // 处理前两张最有价值的牌的弃牌逻辑
                handleCardDiscard(ref bestCardPos, ref bestCardValue, ref bestSecondCardPos, ref bestSecondCardValue, ref numDiscardedCards, discardedCardsBonusList);

                // 处理剩余的需要弃掉的牌
                handleRemainingCards(anz, ref numDiscardedCards, cardsValDict, discardedCardsBonusList);

                // 触发弃牌任务的进度
                if (this.ownQuest.Id != CardDB.cardIDEnum.None) this.ownQuest.trigger_WasDiscard(numDiscardedCards);

                // 处理玛克扎尔的小鬼等随从的弃牌触发效果
                handleMinionDiscardTriggers(numDiscardedCards, discardedCardsBonusList);

            }
            else
            {
                // 敌方弃牌逻辑
                int anz = Math.Min(num, this.enemyAnzCards);
                if (anz < 1) return;
                this.enemycarddraw -= anz;
                this.enemyAnzCards -= anz;
            }
            this.triggerCardsChanged(own);
        }

        /// <summary>
        /// 计算每张牌的价值，依据随从、武器、法术不同类型分别计算。
        /// </summary>
        /// <param name="hc">手牌。</param>
        /// <param name="c">卡牌信息。</param>
        /// <returns>返回卡牌的计算价值。</returns>
        private int calculateCardValue(Handmanager.Handcard hc, CardDB.Card c)
        {
            int cardValue = 0;
            switch (c.type)
            {
                case CardDB.cardtype.MOB:
                    cardValue = (c.Health + hc.addHp) * 2 + (c.Attack + hc.addattack) * 2 + c.rarity + hc.poweredUp * 2;
                    if (c.windfury) cardValue += c.Attack + hc.addattack;
                    if (c.tank) cardValue += 2;
                    if (c.Shield) cardValue += 2;
                    if (c.Charge) cardValue += 3;
                    if (c.Stealth) cardValue += 1;
                    if (c.isSpecialMinion) cardValue += 10;
                    if (c.nameEN == CardDB.cardNameEN.direwolfalpha && this.ownMinions.Count > 2) cardValue += 10;
                    if (c.nameEN == CardDB.cardNameEN.flametonguetotem && this.ownMinions.Count > 2) cardValue += 10;
                    if (c.nameEN == CardDB.cardNameEN.silverwaregolem || c.nameEN == CardDB.cardNameEN.clutchmotherzavas)
                        cardValue = (c.Health + hc.addHp) * 2 + c.rarity;
                    break;
                case CardDB.cardtype.WEAPON:
                    cardValue = c.Attack * c.Durability * 2;
                    if (c.battlecry || c.deathrattle) cardValue += 7;
                    break;
                case CardDB.cardtype.SPELL:
                    cardValue = 15;
                    break;
            }
            return cardValue;
        }

        /// <summary>
        /// 根据卡牌费用计算价值比率。
        /// </summary>
        /// <param name="manaCost">卡牌的法力消耗。</param>
        /// <returns>返回计算后的比率。</returns>
        private int getCardValueRatio(int manaCost)
        {
            if (manaCost > 1)
            {
                if (manaCost == this.mana) return 80;
                else return 60;
            }
            return 100;
        }

        /// <summary>
        /// 处理最有价值的两张卡牌的弃牌逻辑。
        /// </summary>
        /// <param name="bestCardPos">最有价值卡牌的位置。</param>
        /// <param name="bestCardValue">最有价值卡牌的价值。</param>
        /// <param name="bestSecondCardPos">第二有价值卡牌的位置。</param>
        /// <param name="bestSecondCardValue">第二有价值卡牌的价值。</param>
        /// <param name="numDiscardedCards">需要弃掉的卡牌数量。</param>
        /// <param name="discardedCardsBonusList">弃牌过程中获得奖励的卡牌列表。</param>
        private void handleCardDiscard(ref int bestCardPos, ref int bestCardValue, ref int bestSecondCardPos, ref int bestSecondCardValue, ref int numDiscardedCards, List<Handmanager.Handcard> discardedCardsBonusList)
        {
            Handmanager.Handcard removedhc;
            if (bestSecondCardPos > bestCardPos)
            {
                int tempPos = bestSecondCardPos;
                int tempVal = bestSecondCardValue;
                bestSecondCardPos = bestCardPos;
                bestSecondCardValue = bestCardValue;
                bestCardPos = tempPos;
                bestCardValue = tempVal;
            }

            for (int i = 0; i < numDiscardedCards; i++)
            {
                int cPos = i == 0 ? bestCardPos : bestSecondCardPos;
                int cVal = i == 0 ? bestCardValue : bestSecondCardValue;

                removedhc = this.owncards[cPos];
                this.owncards.RemoveAt(cPos);
                if (removedhc.card.sim_card != null && removedhc.card.sim_card.onCardDicscard(this, removedhc, null, 0, true))
                {
                    discardedCardsBonusList.Add(removedhc);
                    cVal = -6;
                }
                else this.owncarddraw--;
            }
        }

        /// <summary>
        /// 处理剩余需要弃掉的卡牌。
        /// </summary>
        /// <param name="anz">剩余需要弃掉的卡牌数量。</param>
        /// <param name="numDiscardedCards">实际弃掉的卡牌数量。</param>
        /// <param name="cardsValDict">卡牌价值字典。</param>
        /// <param name="discardedCardsBonusList">弃牌过程中获得奖励的卡牌列表。</param>
        private void handleRemainingCards(int anz, ref int numDiscardedCards, Dictionary<int, int> cardsValDict, List<Handmanager.Handcard> discardedCardsBonusList)
        {
            for (int i = 0; i < anz; i++)
            {
                Handmanager.Handcard removedhc = this.owncards[i];
                int bestCardValue = cardsValDict[this.owncards[i].entity];
                if (removedhc.card.sim_card.onCardDicscard(this, removedhc, null, 0, true))
                {
                    discardedCardsBonusList.Add(removedhc);
                    bestCardValue = 0;
                }
                else this.owncarddraw--;
            }
            this.owncards.RemoveRange(0, anz);
        }

        /// <summary>
        /// 处理弃牌相关的随从效果触发，例如玛克扎尔的小鬼。
        /// </summary>
        /// <param name="numDiscardedCards">实际弃掉的卡牌数量。</param>
        /// <param name="discardedCardsBonusList">弃牌过程中获得奖励的卡牌列表。</param>
        private void handleMinionDiscardTriggers(int numDiscardedCards, List<Handmanager.Handcard> discardedCardsBonusList)
        {
            int malchezaarsimpCount = 0;
            foreach (Minion m in this.ownMinions)
            {
                if (m.Hp > 0 && !m.silenced)
                {
                    if (m.name == CardDB.cardNameEN.malchezaarsimp) malchezaarsimpCount++;
                    m.handcard.card.sim_card.onCardDicscard(this, m.handcard, m, numDiscardedCards);
                }
            }

            if (malchezaarsimpCount > 0)
            {
                // 适当调整弃牌后的惩罚值（已删除打分逻辑）
            }

            foreach (Handmanager.Handcard dc in discardedCardsBonusList)
            {
                dc.card.sim_card.onCardDicscard(this, dc, null, 0);
            }
        }

        public void drawTemporaryCard(CardDB.cardIDEnum ss, bool own)
        {
            CardDB.cardIDEnum s = ss;

            // cant hold more than 10 cards
            if (own)
            {
                if (s == CardDB.cardIDEnum.None)
                {
                    if (ownDeckSize == 0)
                    {
                        this.ownHeroFatigue++;
                        this.ownHero.getDamageOrHeal(this.ownHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        this.ownDeckSize--;
                        if (this.owncards.Count >= 10)
                        {
                            return;
                        }
                        this.owncarddraw++;
                    }

                    // 符文秘银杖
                    if (this.ownWeapon != null && this.ownWeapon.card.nameCN == CardDB.cardNameCN.符文秘银杖)
                    {
                        this.ownWeapon.scriptNum1++;
                    }
                }
                else
                {
                    if (this.owncards.Count >= 10)
                    {
                        return;
                    }
                    this.owncarddraw++;
                }
            }
            else
            {
                if (s == CardDB.cardIDEnum.None)
                {
                    if (enemyDeckSize == 0)
                    {
                        this.enemyHeroFatigue++;
                        this.enemyHero.getDamageOrHeal(this.enemyHeroFatigue, this, false, true);
                        return;
                    }
                    else
                    {
                        this.enemyDeckSize--;
                        if (this.enemyAnzCards >= 10)
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                    }
                }
                else
                {
                    if (this.enemyAnzCards >= 10)
                    {
                        return;
                    }
                    this.enemycarddraw++;
                    this.enemyAnzCards++;
                }
                this.triggerCardsChanged(false);

                if (anzEnemyChromaggus > 0 && s == CardDB.cardIDEnum.None)
                {
                    for (int i = 1; i <= anzEnemyChromaggus; i++)
                    {
                        if (this.enemyAnzCards >= 10)
                        {
                            return;
                        }
                        this.enemycarddraw++;
                        this.enemyAnzCards++;
                        this.triggerCardsChanged(false);
                    }
                }
                return;
            }

            CardDB.Card c = CardDB.Instance.getCardDataFromID(s);
            Handmanager.Handcard hc = new Handmanager.Handcard
            {
                card = c,
                position = this.owncards.Count + 1,
                manacost = 0, // 临时卡牌的费用通常为0或特殊处理
                entity = this.getNextEntity(),
                temporary = true // 设置临时标志
            };
            this.owncards.Add(hc);
            this.triggerCardsChanged(true);

            if (anzOwnChromaggus > 0 && s == CardDB.cardIDEnum.None)
            {
                for (int i = 1; i <= anzOwnChromaggus; i++)
                {
                    if (this.owncards.Count >= 10)
                    {
                        return;
                    }
                    this.owncarddraw++;
                    hc = new Handmanager.Handcard
                    {
                        card = c,
                        position = this.owncards.Count + 1,
                        manacost = 0,
                        entity = this.getNextEntity(),
                        temporary = true // 设置临时标志
                    };
                    this.owncards.Add(hc);
                    this.triggerCardsChanged(true);
                }
            }
        }

        /// <summary>
        /// 移除临时卡牌
        /// </summary>
        /// <param name="own"></param>
        public void removeTemporaryCards(bool own)
        {
            if (own)
            {
                this.owncards = owncards.Where(hc => !hc.temporary).ToList();
            }
            this.triggerCardsChanged(own);
        }
    }
}
