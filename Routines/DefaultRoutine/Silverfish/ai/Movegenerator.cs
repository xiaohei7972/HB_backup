namespace HREngine.Bots
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Movegenerator
    {
        PenalityManager pen = PenalityManager.Instance;

        private static Movegenerator instance;

        public static Movegenerator Instance
        {
            get
            {
                return instance ?? (instance = new Movegenerator());
            }
        }

        private Movegenerator()
        {
        }

        /// <summary>
        /// 生成潜在的动作列表，并对每个动作进行打分。
        /// </summary>
        /// <param name="p">当前的游戏状态。</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器。</param>
        /// <param name="useCutingTargets">是否使用目标剪枝。</param>
        /// <param name="own">是否为己方回合。</param>
        /// <returns>返回动作列表。</returns>
        public List<Action> getMoveList(Playfield p, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            List<Action> resultAction = new List<Action>();
            // 游戏终止条件
            if (p.complete || p.ownHero.Hp <= 0)
                return resultAction;

            List<Minion> targets = new List<Minion>();
            List<Minion> minions = own ? p.ownMinions : p.enemyMinions;
            if (own)
            {
                //使用卡牌动作
                resultAction.AddRange(this.getPlayCardActions(p, targets, usePenalityManager, useCutingTargets, own));


                if (p.owncards.Count > 0)
                {

                    // 检查是否有可交易的卡牌
                    if (p.owncards.Any(hc => hc.card.Tradeable))
                        //交易动作
                        resultAction.AddRange(this.getTradeActions(p));
                    // 检查是否有可锻造的卡牌

                    if (p.owncards.Any(hc => hc.card.Forge))
                        //锻造动作
                        resultAction.AddRange(this.getForgeActions(p));
                }
            }

            // 获取英雄武器和随从的攻击目标
            targets = p.GetAttackTargets(own, p.isLethalCheck);
            if (!p.isLethalCheck) targets = this.cutAttackList(targets);
            //获取随从攻击动作
            resultAction.AddRange(this.getMinionAttackActions(p, targets, usePenalityManager));

            //获取英雄攻击动作
            resultAction.AddRange(this.getHeroAttackActions(p, targets, usePenalityManager, own));

            // 使用己方英雄技能
            resultAction.AddRange(this.getHeroPowerActions(p, targets, usePenalityManager, own));

            // 检查是否有地标
            if (minions.Any(m => m.handcard.card.type == CardDB.cardtype.LOCATION))
                //获取使用地标动作
                resultAction.AddRange(this.getLocationActions(p, targets, usePenalityManager, useCutingTargets, own));

            //检查是否有使用泰坦随从
            if (minions.Any(m => m.Titan))
                //获取使用泰坦技能动作
                resultAction.AddRange(this.getTitanActions(p, targets, usePenalityManager, useCutingTargets, own));


            return resultAction;
        }
        /// <summary>
        /// 获取使用卡牌动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <param name="useCutingTargets"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public List<Action> getPlayCardActions(Playfield p, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            List<Action> resultAction = new List<Action>();
            HashSet<string> playedcards = new HashSet<string>();
            StringBuilder cardNcost = new StringBuilder(20);

            foreach (Handmanager.Handcard hc in p.owncards)
            {
                //如果隐藏费用跳过此次循环
                if (hc.card.nameEN == CardDB.cardNameEN.unknown || hc.card.HideCost) continue;

                int cardCost = hc.card.getManaCost(p, hc.manacost);

                // 检查卡牌的打出条件
                if ((p.nextSpellThisTurnCostHealth && hc.card.type == CardDB.cardtype.SPELL) ||
                    (p.nextMurlocThisTurnCostHealth && hc.card.race == CardDB.Race.MURLOC))
                {
                    if (p.ownHero.Hp <= cardCost && !p.ownHero.immune) continue;
                }
                else if (p.mana < cardCost) continue;

                // 检查是否在此回合内打出了相同的卡牌
                cardNcost.Clear();
                cardNcost.Append(hc.card.cardIDenum).Append(hc.manacost);
                if (playedcards.Contains(cardNcost.ToString()) && !hc.card.Outcast && hc.enchs.Count == 0) continue;
                playedcards.Add(cardNcost.ToString());

                bool isChoice = hc.card.choice;
                CardDB.Card c = hc.card;

                for (int choice = isChoice ? 1 : 0; choice <= (isChoice ? 2 : 1); choice++)
                {
                    if (isChoice)
                    {
                        c = pen.getChooseCard(hc.card, choice);
                        if (p.ownFandralStaghelm > 0)
                        {
                            // 找到包含伤害或治疗效果的选择项
                            for (int i = 1; i <= 2; i++)
                            {
                                CardDB.Card cTmp = pen.getChooseCard(hc.card, i);
                                if (pen.DamageTargetDatabase.ContainsKey(cTmp.nameEN) ||
                                    (p.anzOwnAuchenaiSoulpriest > 0 && pen.HealTargetDatabase.ContainsKey(cTmp.nameEN)))
                                {
                                    choice = i;
                                    c = cTmp;
                                    break;
                                }
                            }
                        }
                    }
                    if (p.ownMinions.Count >= 7)
                        if (hc.card.type == CardDB.cardtype.MOB || hc.card.type == CardDB.cardtype.LOCATION)
                            continue;
                    targets = c.getTargetsForCard(p, p.isLethalCheck, true);
                    if (targets.Count == 0) continue;


                    int bestplace = p.getBestPlace(c, p.isLethalCheck);

                    foreach (Minion target in targets)
                    {
                        int cardplayPenality = usePenalityManager ? pen.getPlayCardPenality(c, target, p, hc) : 0;
                        if (cardplayPenality <= 499)
                        {
                            resultAction.Add(new Action(actionEnum.playcard, hc, null, bestplace, target, cardplayPenality, choice));
                        }
                    }
                }
            }
            return resultAction;
        }


        /// <summary>
        /// 获取使用随从攻击动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <returns></returns>
        public List<Action> getMinionAttackActions(Playfield p, List<Minion> targets, bool usePenalityManager)
        {
            List<Action> resultAction = new List<Action>();
            // 处理随从攻击
            List<Minion> attackingMinions = new List<Minion>();
            foreach (Minion m in p.ownMinions)
            {
                if (m.handcard.card.type == CardDB.cardtype.LOCATION) continue;
                m.updateReadyness();
                if (m.Ready)
                {
                    attackingMinions.Add(m);
                }
            }

            attackingMinions = this.cutAttackList(attackingMinions);

            // 计算随从攻击的惩罚值
            foreach (Minion m in attackingMinions)
            {
                foreach (Minion target in targets)
                {
                    if (target == null || target.own) continue;
                    if (target.untouchable || (m.cantAttackHeroes && target.isHero)) continue;

                    int attackPenality = usePenalityManager ? pen.getAttackWithMininonPenality(m, p, target) : 0;
                    if (attackPenality <= 499)
                    {
                        resultAction.Add(new Action(actionEnum.attackWithMinion, null, m, 0, target, attackPenality, 0));
                    }
                }
            }
            return resultAction;
        }
        /// <summary>
        /// 获取英雄攻击动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public List<Action> getHeroAttackActions(Playfield p, List<Minion> targets, bool usePenalityManager, bool own)
        {
            List<Action> resultAction = new List<Action>();
            Minion hero = own ? p.ownHero : p.enemyHero;
            hero.updateReadyness();

            // 处理英雄攻击（武器）
            if (hero.Ready)
            {
                foreach (Minion target in targets)
                {
                    if ((own ? p.ownWeapon.cantAttackHeroes : p.enemyWeapon.cantAttackHeroes) && target.isHero) continue;

                    int heroAttackPen = usePenalityManager ? pen.getAttackWithHeroPenality(target, p) : 0;
                    if (heroAttackPen <= 499)
                    {
                        resultAction.Add(new Action(actionEnum.attackWithHero, null, hero, 0, target, heroAttackPen, 0));
                    }
                }
            }
            return resultAction;

        }
        /// <summary>
        /// 获取使用英雄技能动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <param name="useCutingTargets"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public List<Action> getHeroPowerActions(Playfield p, List<Minion> targets, bool usePenalityManager, bool own)
        {
            List<Action> resultAction = new List<Action>();
            if (own && p.ownAbilityReady && p.mana >= p.ownHeroAblility.card.getManaCost(p, p.ownHeroAblility.manacost))
            {
                CardDB.Card c = p.ownHeroAblility.card;
                int choiceCount = c.choice ? 2 : 1;  // 如果是抉择卡牌，choiceCount为2，否则为1
                targets = p.ownHeroAblility.card.getTargetsForHeroPower(p, true);
                for (int choice = 1; choice <= choiceCount; choice++)
                {
                    CardDB.Card chosenCard = c;

                    // 如果是抉择卡牌，根据choice获取不同的卡牌
                    if (c.choice)
                    {
                        chosenCard = pen.getChooseCard(p.ownHeroAblility.card, choice);
                    }

                    int playCardPenalty = 0;
                    int place = p.ownMinions.Count + 1;

                    foreach (Minion target in targets)
                    {
                        if (p.ownHeroAblility.card.nameCN == CardDB.cardNameCN.未知 && p.ownHeroName == HeroEnum.thief)
                        {
                            p.ownHeroAblility.card.nameCN = CardDB.cardNameCN.匕首精通;
                        }

                        if (usePenalityManager)
                        {
                            playCardPenalty = pen.getPlayCardPenality(chosenCard, target, p, new Handmanager.Handcard());
                        }

                        if (playCardPenalty > 499) continue;
                        Action a = new Action(actionEnum.useHeroPower, p.ownHeroAblility, null, place, target, playCardPenalty, choice);
                        resultAction.Add(a);
                    }
                }
            }
            return resultAction;

        }
        /// <summary>
        /// 获取使用交易动作
        /// </summary>
        /// <param name="p"></param>
        /// <returns>返回交易动作列表</returns>
        public List<Action> getTradeActions(Playfield p)
        {
            List<Action> resultAction = new List<Action>();
            // 处理可交易的卡牌
            if (p.ownDeckSize > 0)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.nameEN == CardDB.cardNameEN.unknown) continue;
                    if (hc.card.Tradeable && p.mana >= hc.card.TradeCost)
                    {
                        resultAction.Add(new Action(actionEnum.trade, hc, null, 0, null, 0, 0));
                    }
                }
            }
            return resultAction;
        }
        /// <summary>
        /// 获取使用锻造动作
        /// </summary>
        /// <param name="p"></param>
        /// <returns>返回锻造动作列表</returns>
        public List<Action> getForgeActions(Playfield p)
        {
            List<Action> resultAction = new List<Action>();
            // 处理可锻造的卡牌
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.nameEN == CardDB.cardNameEN.unknown) continue;
                if (hc.card.Forge && p.mana >= hc.card.ForgeCost)
                {
                    resultAction.Add(new Action(actionEnum.forge, hc, null, 0, null, 0, 0));
                }
            }
            return resultAction;
        }
        /// <summary>
        /// 获取使用地标动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <param name="useCutingTargets"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public List<Action> getLocationActions(Playfield p, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            List<Action> resultAction = new List<Action>();
            // 使用地标逻辑
            List<Minion> usingMinions = (own ? p.ownMinions : p.enemyMinions)
                .Where(m => m.handcard.card.type == CardDB.cardtype.LOCATION && m.CooldownTurn == 0 && m.Ready)
                .ToList();
            foreach (Minion minion in usingMinions)
            {
                targets = minion.handcard.card.getTargetsForLocation(p, p.isLethalCheck, true);

                if (targets.Count > 0)
                {
                    foreach (Minion target in targets)
                    {
                        //有目标的地标
                        int useLocationPenalty = usePenalityManager ? pen.getUseLocationPenality(minion, target, p) : -100;
                        if (useLocationPenalty <= 499)
                        {
                            resultAction.Add(new Action(actionEnum.useLocation, null, minion, 0, target, useLocationPenalty, 0));
                        }
                    }
                }
                ///无目标的地标
                else if (targets.Count == 0)
                {
                    int useLocationPenalty = usePenalityManager ? pen.getUseLocationPenality(minion, null, p) : -100;
                    resultAction.Add(new Action(actionEnum.useLocation, null, minion, 0, null, useLocationPenalty, 0));
                }
            }
            return resultAction;

        }
        /// <summary>
        /// 获取使用泰坦技能动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="targets"></param>
        /// <param name="usePenalityManager"></param>
        /// <param name="useCutingTargets"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public List<Action> getTitanActions(Playfield p, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            List<Action> resultAction = new List<Action>();
            // 使用泰坦技能逻辑
            List<Minion> titans = (own ? p.ownMinions : p.enemyMinions).Where(m => m.Titan).ToList();
            foreach (Minion titan in titans)
            {
                //初始化技能列表
                titan.handcard.card.TitanAbility = titan.handcard.card.GetTitanAbility();
                // 遍历每个技能
                for (int i = 0; i < 3; i++)
                {
                    if ((i == 0 && titan.TitanAbilityUsed1) ||
                        (i == 1 && titan.TitanAbilityUsed2) ||
                        (i == 2 && titan.TitanAbilityUsed3))
                    {
                        continue; // 如果技能已经使用过，跳过
                    }

                    CardDB.Card ability = titan.handcard.card.TitanAbility[i];
                    targets = ability.getTargetsForCard(p, p.isLethalCheck, true);

                    // 如果技能不需要目标，直接添加动作
                    if (targets.Count == 0)
                    {
                        int titanAbilityPenalty = usePenalityManager ? pen.getUseTitanAbilityPenality(titan, null, p) : -100;

                        resultAction.Add(new Action(actionEnum.useTitanAbility, null, titan, 0, null, titanAbilityPenalty, 0, i + 1));
                        continue;
                    }

                    // 如果技能需要一个目标，生成对应的动作
                    foreach (Minion target in targets)
                    {
                        int titanAbilityPenalty = usePenalityManager ? pen.getUseTitanAbilityPenality(titan, target, p) : -100;
                        if (titanAbilityPenalty <= 499)
                        {
                            resultAction.Add(new Action(actionEnum.useTitanAbility, null, titan, 0, target, titanAbilityPenalty - 100, 0, i + 1));
                        }
                    }
                }
            }
            return resultAction;
        }

        /// <summary>
        /// 剪枝，排除相同的目标
        /// </summary>
        /// <param name="oldlist"></param>
        /// <returns></returns>
        public List<Minion> cutAttackList(List<Minion> oldlist)
        {
            List<Minion> retvalues = new List<Minion>(oldlist.Count);
            List<Minion> addedmins = new List<Minion>(oldlist.Count);

            foreach (Minion m in oldlist)
            {
                if (m.isHero)
                {
                    retvalues.Add(m);
                    continue;
                }

                bool goingtoadd = true;
                bool isSpecial = m.handcard.card.isSpecialMinion;
                foreach (Minion mnn in addedmins)
                {
                    bool otherisSpecial = mnn.handcard.card.isSpecialMinion;
                    bool onlySpecial = isSpecial && otherisSpecial && !m.silenced && !mnn.silenced;
                    bool onlyNotSpecial = (!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced));

                    if (onlySpecial && (m.name != mnn.name)) continue; // different name -> take it
                    // if ((onlySpecial || onlyNotSpecial) && (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous && mnn.lifesteal == m.lifesteal && m.handcard.card.isToken == mnn.handcard.card.isToken && mnn.handcard.card.race == m.handcard.card.race && mnn.Spellburst == m.Spellburst && mnn.cantAttackHeroes == m.cantAttackHeroes))
                    if ((onlySpecial || onlyNotSpecial) && (mnn == m))
                    {
                        goingtoadd = false;
                        break;
                    }
                }

                if (goingtoadd)
                {
                    addedmins.Add(m);
                    retvalues.Add(m);
                }
                else
                {
                    continue;
                }
            }
            return retvalues;
        }


        /*        public List<Minion> cutAttackList(List<Minion> oldlist)
       {
           // var uniqueMinions = new HashSet<string>();
           // var result = new List<Minion>();

           // foreach (var m in oldlist)
           // {
           //     if (m.isHero)
           //     {
           //         result.Add(m);
           //         continue;
           //     }

           //     string key = $"{m.nameCN}_{m.Angr}_{m.Hp}"; // 构造唯一标识
           //     if (!uniqueMinions.Contains(key))
           //     {
           //         uniqueMinions.Add(key);
           //         result.Add(m);
           //     }
           // }

           // return result;
           List<Minion> result = new List<Minion>(oldlist.Count);
           // List<Minion> addedmins = new List<Minion>(oldlist.Count);
           HashSet<int> uniqueMinions = new HashSet<int>();

           foreach (Minion m in oldlist)
           {
               if (m.isHero)
               {
                   result.Add(m);
                   continue;
               }
               if (uniqueMinions.Contains(m.entitiyID)) continue;
               uniqueMinions.Add(m.entitiyID);
               result.Add(m);
               // bool goingtoadd = true;
               // bool isSpecial = m.handcard.card.isSpecialMinion;
               // foreach (Minion otherMinion in addedmins)
               // {
               //     bool otherisSpecial = otherMinion.handcard.card.isSpecialMinion;
               //     bool onlySpecial = isSpecial && otherisSpecial && !m.silenced && !otherMinion.silenced;
               //     bool onlyNotSpecial = (!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && otherMinion.silenced));

               //     if (onlySpecial && (m.name != otherMinion.name)) continue; // different name -> take it
               //     // if ((onlySpecial || onlyNotSpecial) && (otherMinion.Angr == m.Angr && otherMinion.Hp == m.Hp && otherMinion.divineshild == m.divineshild && otherMinion.taunt == m.taunt && otherMinion.poisonous == m.poisonous && otherMinion.lifesteal == m.lifesteal && m.handcard.card.isToken == otherMinion.handcard.card.isToken && otherMinion.handcard.card.race == m.handcard.card.race && otherMinion.Spellburst == m.Spellburst && otherMinion.cantAttackHeroes == m.cantAttackHeroes))
               //     if ((onlySpecial || onlyNotSpecial) && (otherMinion == m))
               //     {
               //         goingtoadd = false;
               //         break;
               //     }
               // }

               // if (goingtoadd)
               // {
               //     addedmins.Add(m);
               //     returnValues.Add(m);
               // }
               // else
               // {
               //     continue;
               // }
           }
           return result;
       } */


        /// <summary>
        /// 攻击指令事项
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns> 
        public bool didAttackOrderMatters(Playfield p)
        {
            //return true;
            if (p.isOwnTurn)
            {
                if (p.enemySecretCount >= 1) return true;
                if (p.enemyHero.immune) return true;

            }
            else
            {
                if (p.ownHero.immune) return true;
            }
            List<Minion> enemym = (p.isOwnTurn) ? p.enemyMinions : p.ownMinions;
            List<Minion> ownm = (p.isOwnTurn) ? p.ownMinions : p.enemyMinions;

            int strongestAttack = 0;
            foreach (Minion m in enemym)
            {
                if (m.Angr > strongestAttack) strongestAttack = m.Angr;
                if (m.taunt) return true;
                if (m.name == CardDB.cardNameEN.dancingswords || m.name == CardDB.cardNameEN.deathlord) return true;
            }

            int haspets = 0;
            bool hashyena = false;
            bool hasJuggler = false;
            bool spawnminions = false;
            foreach (Minion m in ownm)
            {
                if (m.name == CardDB.cardNameEN.cultmaster) return true;
                if (m.name == CardDB.cardNameEN.knifejuggler) hasJuggler = true;
                if (m.Ready && m.Angr >= 1)
                {
                    if (m.AdjacentAngr >= 1) return true;//wolphalfa or flametongue is in play
                    if (m.name == CardDB.cardNameEN.northshirecleric) return true;
                    if (m.name == CardDB.cardNameEN.armorsmith) return true;
                    if (m.name == CardDB.cardNameEN.loothoarder) return true;
                    //if (m.name == CardDB.cardName.madscientist) return true; // dont change the tactic
                    if (m.name == CardDB.cardNameEN.sylvanaswindrunner) return true;
                    if (m.name == CardDB.cardNameEN.darkcultist) return true;
                    if (m.ownBlessingOfWisdom >= 1) return true;
                    if (m.ownPowerWordGlory >= 1) return true;
                    if (m.name == CardDB.cardNameEN.acolyteofpain) return true;
                    if (m.name == CardDB.cardNameEN.frothingberserker) return true;
                    if (m.name == CardDB.cardNameEN.flesheatingghoul) return true;
                    if (m.name == CardDB.cardNameEN.bloodmagethalnos) return true;
                    if (m.name == CardDB.cardNameEN.webspinner) return true;
                    if (m.name == CardDB.cardNameEN.tirionfordring) return true;
                    if (m.name == CardDB.cardNameEN.baronrivendare) return true;


                    //if (m.name == CardDB.cardName.manawraith) return true;
                    //buffing minions (attack with them last)
                    if (m.name == CardDB.cardNameEN.raidleader || m.name == CardDB.cardNameEN.stormwindchampion || m.name == CardDB.cardNameEN.timberwolf || m.name == CardDB.cardNameEN.southseacaptain || m.name == CardDB.cardNameEN.murlocwarleader || m.name == CardDB.cardNameEN.grimscaleoracle || m.name == CardDB.cardNameEN.leokk || m.name == CardDB.cardNameEN.fallenhero || m.name == CardDB.cardNameEN.warhorsetrainer) return true;


                    if (m.name == CardDB.cardNameEN.scavenginghyena) hashyena = true;
                    if (m.handcard.card.race == CardDB.Race.PET) haspets++;
                    if (m.name == CardDB.cardNameEN.harvestgolem || m.name == CardDB.cardNameEN.hauntedcreeper || m.souloftheforest >= 1 || m.stegodon >= 1 || m.livingspores >= 1 || m.infest >= 1 || m.ancestralspirit >= 1 || m.desperatestand >= 1 || m.explorershat >= 1 || m.returnToHand >= 1 || m.name == CardDB.cardNameEN.nerubianegg || m.name == CardDB.cardNameEN.savannahhighmane || m.name == CardDB.cardNameEN.sludgebelcher || m.name == CardDB.cardNameEN.cairnebloodhoof || m.name == CardDB.cardNameEN.feugen || m.name == CardDB.cardNameEN.stalagg || m.name == CardDB.cardNameEN.thebeast) spawnminions = true;

                }
            }

            if (haspets >= 1 && hashyena) return true;
            if (hasJuggler && spawnminions) return true;




            return false;
        }
    }

}