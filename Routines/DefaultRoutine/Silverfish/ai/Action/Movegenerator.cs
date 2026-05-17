namespace HREngine.Bots
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 动作生成器类，负责生成炉石传说中所有可能的行动动作
    /// 包括打出卡牌、随从攻击、英雄攻击、使用英雄技能、交易、锻造、使用地标、使用泰坦技能等
    /// 采用单例模式，全局只有一个实例
    /// </summary>
    public class Movegenerator
    {
        /// <summary>
        /// 惩罚值管理器实例，用于计算各种动作的惩罚值
        /// </summary>
        PenalityManager pen = PenalityManager.Instance;

        /// <summary>
        /// 单例实例
        /// </summary>
        private static Movegenerator instance;

        /// <summary>
        /// 获取动作生成器的单例实例
        /// </summary>
        public static Movegenerator Instance
        {
            get
            {
                return instance ?? (instance = new Movegenerator());
            }
        }

        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private Movegenerator()
        {
        }

        /// <summary>
        /// 生成潜在的动作列表，并对每个动作进行打分。
        /// 这是动作生成的主入口方法，会调用各个子方法来生成不同类型的动作。
        /// </summary>
        /// <param name="p">当前的游戏状态（场面）</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器对动作进行评分</param>
        /// <param name="useCutingTargets">是否使用目标剪枝（减少搜索空间）</param>
        /// <param name="own">是否为己方回合（true为己方，false为敌方）</param>
        /// <returns>返回所有可能的动作列表</returns>
        public List<Action> getMoveList(Playfield p, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            List<Action> resultAction = ActionListPool.Rent();

            // 游戏终止条件：游戏已结束或己方英雄死亡，返回空列表
            if (p.complete || p.ownHero.Hp <= 0)
                return resultAction;

            List<Minion> targets = new List<Minion>();
            List<Minion> minions = own ? p.ownMinions : p.enemyMinions;

            // 单次扫描，计算所有需要的标志位
            bool hasTradeable = false;
            bool hasForge = false;
            bool hasLocation = false;
            bool hasTitan = false;

            if (own && p.owncards.Count > 0)
            {
                int cardCount = p.owncards.Count;
                for (int i = 0; i < cardCount; i++)
                {
                    var hc = p.owncards[i];
                    if (hc.card.Tradeable) hasTradeable = true;
                    if (hc.card.Forge) hasForge = true;
                }
            }

            int minionCount = minions.Count;
            for (int i = 0; i < minionCount; i++)
            {
                var m = minions[i];
                if (m.handcard.card.type == CardDB.cardtype.LOCATION) hasLocation = true;
                if (m.Titan) hasTitan = true;
            }

            // 己方回合才有的动作：打出卡牌、交易、锻造
            if (own)
            {
                // 生成打出卡牌动作
                this.getPlayCardActions(p, ref resultAction, targets, usePenalityManager, useCutingTargets, own);

                if (p.owncards.Count > 0)
                {
                    // 检查是否有可交易的卡牌，生成交易动作
                    if (hasTradeable)
                        this.getTradeActions(p, ref resultAction);

                    // 检查是否有可锻造的卡牌，生成锻造动作
                    if (hasForge)
                        this.getForgeActions(p, ref resultAction);
                }
            }

            // 获取英雄武器和随从的攻击目标
            targets = p.GetAttackTargets(own, p.isLethalCheck);
            // 非斩杀检测时，对目标列表进行剪枝以减少搜索空间
            if (!p.isLethalCheck) targets = this.cutAttackList(targets);

            // 生成随从攻击动作
            this.getMinionAttackActions(p, ref resultAction, targets, usePenalityManager);

            // 生成英雄攻击动作（武器攻击）
            this.getHeroAttackActions(p, ref resultAction, targets, usePenalityManager, own);

            // 生成使用英雄技能动作
            this.getHeroPowerActions(p, ref resultAction, targets, usePenalityManager, own);

            // 检查是否有地标，生成使用地标动作
            if (hasLocation)
                this.getLocationActions(p, ref resultAction, targets, usePenalityManager, useCutingTargets, own);

            // 检查是否有泰坦随从，生成使用泰坦技能动作
            if (hasTitan)
                this.getTitanActions(p, ref resultAction, targets, usePenalityManager, useCutingTargets, own);

            return resultAction;
        }

        /// <summary>
        /// 获取打出卡牌动作
        /// 遍历手牌中的每张卡牌，检查打出条件，生成对应的动作
        /// 支持抉择卡牌的处理，以及范达尔·鹿盔的特殊效果
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递，避免创建临时列表）</param>
        /// <param name="targets">目标列表（此参数在方法内会被重新赋值，用于存储卡牌的目标）</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        /// <param name="useCutingTargets">是否使用目标剪枝</param>
        /// <param name="own">是否为己方回合</param>
        public void getPlayCardActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            // 记录本回合已打出的卡牌（卡牌ID+费用），用于去重
            HashSet<string> playedcards = new HashSet<string>();
            // 用于构建卡牌唯一标识的字符串构建器，预分配20字符容量
            StringBuilder cardNcost = new StringBuilder(20);

            foreach (Handmanager.Handcard hc in p.owncards)
            {
                // 跳过未知卡牌和隐藏费用的卡牌
                if (hc.card.nameEN == CardDB.cardNameEN.unknown || hc.card.HideCost || hc.literallyUnplayable) continue;

                // 随从和地标卡牌需要检查场上随从数量上限（7个）
                if (hc.card.type == CardDB.cardtype.MOB || hc.card.type == CardDB.cardtype.LOCATION)
                    if (p.ownMinions.Count >= 7)
                        continue;

                // 计算卡牌的实际费用
                int cardCost = hc.card.getManaCost(p, hc.manacost);
                
                // 检查特殊打出条件：本回合法术消耗生命值或鱼人消耗生命值
                if ((p.nextSpellThisTurnCostHealth && hc.card.type == CardDB.cardtype.SPELL) ||
                    (p.nextMurlocThisTurnCostHealth && hc.card.race == CardDB.Race.MURLOC))
                {
                    // 如果英雄生命值不足以支付费用且不免疫，跳过
                    if (p.ownHero.Hp <= cardCost && !p.ownHero.immune) continue;
                }
                else if (p.mana < cardCost) continue;  // 法力值不足，跳过

                // 检查是否在本回合已打出过相同的卡牌（用于去重优化）
                cardNcost.Clear();
                cardNcost.Append(hc.card.cardIDenum).Append(hc.manacost);
                if (playedcards.Contains(cardNcost.ToString()) && !hc.card.Outcast && hc.enchs.Count == 0) continue;
                playedcards.Add(cardNcost.ToString());

                // 处理抉择卡牌
                bool isChoice = hc.card.choice;
                CardDB.Card c = hc.card;

               // 获取卡牌的抉择数
                int chooseCount = isChoice ? pen.getChooseCount(hc.card.cardIDenum) : 0;

                // 抉择卡牌需要遍历所有选择，普通卡牌只处理一次（choice=0）
                for (int choice = isChoice ? 1 : 0; choice <= (isChoice ? chooseCount : 0); choice++)
                {
                    if (isChoice)
                    {
                        // 获取抉择后的卡牌
                        c = pen.getChooseCard(hc.card, choice);

                        // 如果场上有范达尔·鹿盔，优先选择伤害或治疗效果的选项
                        if (p.ownFandralStaghelm > 0)
                        {
                            for (int i = 1; i <= 2; i++)
                            {
                                CardDB.Card cTmp = pen.getChooseCard(hc.card, i);
                                // 检查是否为伤害或治疗类卡牌
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

                    // 获取卡牌的有效目标列表
                    targets = c.getTargetsForCard(p, p.isLethalCheck, true);
                    // 没有可用目标时跳过（注意：targets[0]==null表示不需要目标）
                    if (targets.Count == 0) continue;

                    // 获取打出随从的最佳位置
                    int bestplace = p.getBestPlace(c, p.isLethalCheck);

                    // 为每个目标生成一个动作
                    foreach (Minion target in targets)
                    {
                        // 计算打出卡牌的惩罚值
                        int cardplayPenality = usePenalityManager ? pen.getPlayCardPenality(c, target, p, hc) : 0;
                        // 惩罚值<=499的动作才添加（>=500表示不可接受的动作）
                        if (cardplayPenality <= 499)
                        {
                            resultAction.Add(new Action(actionEnum.playcard, hc, null, bestplace, target, cardplayPenality, choice));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 获取随从攻击动作
        /// 遍历己方所有可攻击的随从，为每个随从生成对有效目标的攻击动作
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        /// <param name="targets">可攻击的目标列表</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        public void getMinionAttackActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager)
        {
            // 收集所有可攻击的随从
            List<Minion> attackingMinions = new List<Minion>();
            foreach (Minion m in p.ownMinions)
            {
                // 跳过地标（地标不能攻击）
                if (m.handcard.card.type == CardDB.cardtype.LOCATION) continue;
                // 更新随从的就绪状态
                m.updateReadyness();
                // 只收集已就绪（可攻击）的随从
                if (m.Ready)
                {
                    attackingMinions.Add(m);
                }
            }

            // 对攻击随从列表进行剪枝，减少搜索空间
            attackingMinions = this.cutAttackList(attackingMinions);

            // 为每个攻击随从和每个目标生成攻击动作
            foreach (Minion m in attackingMinions)
            {
                foreach (Minion target in targets)
                {
                    // 跳过空目标
                    if (target == null) continue;
                    // 跳过不可接触的目标和不能攻击英雄的随从攻击英雄的情况
                    if (target.untouchable || (m.cantAttackHeroes && target.isHero)) continue;

                    // 计算攻击惩罚值
                    int attackPenality = usePenalityManager ? pen.getAttackWithMininonPenality(m, p, target) : 0;
                    // 惩罚值<=499的动作才添加
                    if (attackPenality <= 499)
                    {
                        resultAction.Add(new Action(actionEnum.attackWithMinion, null, m, 0, target, attackPenality, 0));
                    }
                }
            }
        }

        /// <summary>
        /// 获取英雄攻击动作
        /// 当英雄装备武器且可以攻击时，生成英雄攻击动作
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        /// <param name="targets">可攻击的目标列表</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        /// <param name="own">是否为己方回合</param>
        public void getHeroAttackActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager, bool own)
        {
            // 获取当前回合的英雄
            Minion hero = own ? p.ownHero : p.enemyHero;
            // 更新英雄的就绪状态
            hero.updateReadyness();

            // 检查英雄是否可以攻击（已就绪且有攻击力）
            if (hero.Ready)
            {
                foreach (Minion target in targets)
                {
                    // 检查武器是否禁止攻击英雄
                    if ((own ? p.ownWeapon.cantAttackHeroes : p.enemyWeapon.cantAttackHeroes) && target.isHero) continue;

                    // 计算英雄攻击惩罚值
                    int heroAttackPen = usePenalityManager ? pen.getAttackWithHeroPenality(target, p) : 0;
                    // 惩罚值<=499的动作才添加
                    if (heroAttackPen <= 499)
                    {
                        resultAction.Add(new Action(actionEnum.attackWithHero, null, own ? p.ownHero : p.enemyHero, 0, target, heroAttackPen, 0));
                    }
                }
            }
        }

        /// <summary>
        /// 获取使用英雄技能动作
        /// 当英雄技能可用且有足够法力值时，生成使用英雄技能的动作
        /// 支持抉择类英雄技能
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        /// <param name="targets">目标列表（此参数在方法内会被重新赋值）</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        /// <param name="own">是否为己方回合</param>
        public void getHeroPowerActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager, bool own)
        {
            // 检查英雄技能是否可用：己方回合、技能已就绪、有足够法力值
            if (own && p.ownAbilityReady && p.mana >= p.ownHeroAblility.card.getManaCost(p, p.ownHeroAblility.manacost))
            {
                CardDB.Card card = p.ownHeroAblility.card;
                bool isChoice = card.choice;

                // 获取英雄技能的抉择数
                int chooseCount = isChoice ? pen.getChooseCount(p.ownHeroAblility.card.cardIDenum) : 0;

                // 抉择类英雄技能需要遍历所有选择，普通英雄技能只有一个
                for (int choice = isChoice ? 1 : 0; choice <= (isChoice ? chooseCount : 0); choice++)
                {
                    CardDB.Card chosenCard = card;

                    // 如果是抉择类英雄技能，根据choice获取对应的卡牌
                    if (isChoice)
                    {
                        chosenCard = pen.getChooseCard(p.ownHeroAblility.card, choice);
                    }

                    // 获取抉择后英雄技能的有效目标（仿照出牌抉择的处理逻辑）
                    targets = chosenCard.getTargetsForHeroPower(p, true);

                    // 没有可用目标时跳过
                    if (targets.Count == 0) continue;

                    int playCardPenalty = 0;
                    int place = p.ownMinions.Count + 1;

                    foreach (Minion target in targets)
                    {

                        // 计算使用英雄技能的惩罚值
                        if (usePenalityManager)
                        {
                            playCardPenalty = pen.getPlayCardPenality(chosenCard, target, p, new Handmanager.Handcard());
                        }

                        // 惩罚值<=499的动作才添加
                        if (playCardPenalty > 499) continue;
                        Action a = new Action(actionEnum.useHeroPower, p.ownHeroAblility, null, place, target, playCardPenalty, choice);
                        resultAction.Add(a);
                    }
                }
            }
        }

        /// <summary>
        /// 获取交易动作
        /// 交易是炉石传说中的一种卡牌机制，消耗法力值将手牌中的可交易卡牌换成牌库中的另一张牌
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        public void getTradeActions(Playfield p, ref List<Action> resultAction)
        {
            // 只有牌库中还有牌时才能交易
            if (p.ownDeckSize > 0)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    // 跳过未知卡牌
                    if (hc.card.nameEN == CardDB.cardNameEN.unknown) continue;
                    // 检查卡牌是否可交易且有足够法力值支付交易费用
                    if (hc.card.Tradeable && p.mana >= hc.card.TradeCost)
                    {
                        resultAction.Add(new Action(actionEnum.trade, hc, null, 0, null, 0, 0));
                    }
                }
            }
        }

        /// <summary>
        /// 获取锻造动作
        /// 锻造是炉石传说中的一种卡牌机制，消耗法力值强化手牌中的可锻造卡牌
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        public void getForgeActions(Playfield p, ref List<Action> resultAction)
        {
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                // 跳过未知卡牌
                if (hc.card.nameEN == CardDB.cardNameEN.unknown) continue;
                // 检查卡牌是否可锻造且有足够法力值支付锻造费用
                if (hc.card.Forge && p.mana >= hc.card.ForgeCost)
                {
                    resultAction.Add(new Action(actionEnum.forge, hc, null, 0, null, 0, 0));
                }
            }
        }

        /// <summary>
        /// 获取使用地标动作
        /// 地标是炉石传说中的一种卡牌类型，可以在场上使用其技能
        /// 地标使用后会有冷却时间，冷却完成后才能再次使用
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        /// <param name="targets">目标列表（此参数在方法内会被重新赋值）</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        /// <param name="useCutingTargets">是否使用目标剪枝</param>
        /// <param name="own">是否为己方回合</param>
        public void getLocationActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            // 获取所有可使用的地标：类型为地标、冷却时间为0、已就绪
            List<Minion> usingMinions = (own ? p.ownMinions : p.enemyMinions)
                .Where(m => m.handcard.card.type == CardDB.cardtype.LOCATION && m.CooldownTurn == 0 && m.Ready)
                .ToList();

            foreach (Minion minion in usingMinions)
            {
                // 获取地标的有效目标
                targets = minion.handcard.card.getTargetsForLocation(p, p.isLethalCheck, true);

                // 没有可用目标时跳过（无法使用）
                if (targets.Count == 0) continue;

                // targets[0] == null 表示地标不需要目标
                if (targets[0] == null)
                {
                    int useLocationPenalty = usePenalityManager ? pen.getUseLocationPenality(minion, null, p) : -100;
                    resultAction.Add(new Action(actionEnum.useLocation, null, minion, 0, null, useLocationPenalty, 0));
                }
                else
                {
                    // 地标需要选择目标
                    foreach (Minion target in targets)
                    {
                        int useLocationPenalty = usePenalityManager ? pen.getUseLocationPenality(minion, target, p) : -100;
                        if (useLocationPenalty <= 499)
                        {
                            resultAction.Add(new Action(actionEnum.useLocation, null, minion, 0, target, useLocationPenalty, 0));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取使用泰坦技能动作
        /// 泰坦是炉石传说中的一种特殊随从类型，拥有3个独立技能
        /// 每个技能只能使用一次，使用后标记为已使用
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <param name="resultAction">动作结果列表（引用传递）</param>
        /// <param name="targets">目标列表（此参数在方法内会被重新赋值）</param>
        /// <param name="usePenalityManager">是否使用惩罚值管理器</param>
        /// <param name="useCutingTargets">是否使用目标剪枝</param>
        /// <param name="own">是否为己方回合</param>
        public void getTitanActions(Playfield p, ref List<Action> resultAction, List<Minion> targets, bool usePenalityManager, bool useCutingTargets, bool own)
        {
            // 获取所有泰坦随从
            List<Minion> titans = (own ? p.ownMinions : p.enemyMinions).Where(m => m.Titan).ToList();

            foreach (Minion titan in titans)
            {
                // 初始化泰坦的技能列表
                titan.handcard.card.TitanAbility = titan.handcard.card.GetTitanAbility();

                // 遍历泰坦的3个技能
                for (int i = 0; i < 3; i++)
                {
                    // 检查技能是否已使用，已使用的跳过
                    if ((i == 0 && titan.TitanAbilityUsed1) ||
                        (i == 1 && titan.TitanAbilityUsed2) ||
                        (i == 2 && titan.TitanAbilityUsed3))
                    {
                        continue;
                    }

                    // 获取当前技能
                    CardDB.Card ability = titan.handcard.card.TitanAbility[i];
                    // 获取技能的有效目标
                    targets = ability.getTargetsForCard(p, p.isLethalCheck, true);

                    // 没有可用目标时跳过（无法使用）
                    if (targets.Count == 0) continue;

                    // targets[0] == null 表示技能不需要目标
                    if (targets[0] == null)
                    {
                        int titanAbilityPenalty = usePenalityManager ? pen.getUseTitanAbilityPenality(titan, null, p) : -100;
                        resultAction.Add(new Action(actionEnum.useTitanAbility, null, titan, 0, null, titanAbilityPenalty, 0, i + 1));
                    }
                    else
                    {
                        // 技能需要选择目标
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
            }
        }

        /// <summary>
        /// 目标剪枝方法，排除重复目标，并按威胁度排序限制数量
        /// 用于减少搜索空间，提高AI决策效率
        /// </summary>
        /// <param name="oldlist">原始目标列表</param>
        /// <param name="maxTargets">最大保留目标数量，默认10</param>
        /// <returns>裁剪后的目标列表</returns>
        public List<Minion> cutAttackList(List<Minion> oldlist, int maxTargets = 10)
        {
            // 数量未超限时，只需去重
            if (oldlist.Count <= maxTargets)
            {
                List<Minion> result = new List<Minion>(oldlist.Count);
                HashSet<Minion> seen = new HashSet<Minion>();
                foreach (Minion m in oldlist)
                {
                    // 英雄始终保留
                    if (m.isHero)
                    {
                        result.Add(m);
                        continue;
                    }
                    // 去重：只添加未见过的随从
                    if (!seen.Contains(m))
                    {
                        seen.Add(m);
                        result.Add(m);
                    }
                }
                return result;
            }

            // 数量超限时，需要按威胁度排序并裁剪
            // 分离英雄和随从
            Minion hero = null;
            List<Minion> minions = new List<Minion>(oldlist.Count);
            HashSet<Minion> seenMinions = new HashSet<Minion>();

            foreach (Minion m in oldlist)
            {
                if (m.isHero)
                {
                    // 只保留一个英雄引用
                    if (hero == null) hero = m;
                    continue;
                }
                // 去重
                if (!seenMinions.Contains(m))
                {
                    seenMinions.Add(m);
                    minions.Add(m);
                }
            }

            // 按威胁度排序：降序排列，高威胁在前
            minions.Sort((a, b) =>
            {
                int threatA = CalculateThreat(a);
                int threatB = CalculateThreat(b);
                return threatB.CompareTo(threatA);
            });

            // 构建结果列表
            List<Minion> finalResult = new List<Minion>(maxTargets);

            // 英雄优先添加
            if (hero != null)
            {
                finalResult.Add(hero);
                maxTargets--;
            }

            // 添加威胁度最高的随从
            int takeCount = Math.Min(maxTargets, minions.Count);
            for (int i = 0; i < takeCount; i++)
            {
                finalResult.Add(minions[i]);
            }

            return finalResult;
        }

        /// <summary>
        /// 计算随从威胁度
        /// 综合考虑攻击力、血量、关键词能力等因素
        /// 用于目标剪枝时的排序依据
        /// </summary>
        /// <param name="m">要计算威胁度的随从</param>
        /// <returns>威胁度分数，分数越高威胁越大</returns>
        private int CalculateThreat(Minion m)
        {
            int threat = 0;

            // 基础威胁：攻击力权重为2，血量权重为1
            threat += m.Angr * 2 + m.Hp;

            // 关键词威胁加成
            if (m.taunt) threat += 10;           // 嘲讽：必须处理，高优先级
            if (m.divineShield) threat += 8;      // 圣盾：需要两次攻击才能消灭
            if (m.poisonous) threat += 10;       // 剧毒：可以消灭任意随从
            if (m.windfury || m.megaWindfury) threat += 5;  // 风怒：可以攻击多次
            if (m.lifesteal) threat += 3;        // 吸血：有恢复能力
            if (m.stealth) threat += 5;          // 潜行：无法直接攻击
            if (m.spellpower > 0) threat += m.spellpower * 3; // 法强：增强法术伤害

            // 即时威胁：可以攻击的随从更危险
            if (m.Ready && !m.cantAttack)
            {
                threat += m.Angr;
                // 能攻击英雄的随从威胁更高
                if (!m.cantAttackHeroes) threat += m.Angr;
            }

            // 特殊机制威胁
            if (m.Deathrattle) threat += 3;      // 亡语：可能有后续效果
            if (m.divineShield) threat += 5;      // 圣盾额外加成

            return threat;
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
               if (uniqueMinions.Contains(m.entityID)) continue;
               uniqueMinions.Add(m.entityID);
               result.Add(m);
               // bool goingtoadd = true;
               // bool isSpecial = m.handcard.card.isSpecialMinion;
               // foreach (Minion otherMinion in addedmins)
               // {
               //     bool otherisSpecial = otherMinion.handcard.card.isSpecialMinion;
               //     bool onlySpecial = isSpecial && otherisSpecial && !m.silenced && !otherMinion.silenced;
               //     bool onlyNotSpecial = (!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && otherMinion.silenced));

               //     if (onlySpecial && (m.name != otherMinion.name)) continue; // different name -> take it
               //     // if ((onlySpecial || onlyNotSpecial) && (otherMinion.Angr == m.Angr && otherMinion.Hp == m.Hp && otherMinion.divineShield == m.divineShield && otherMinion.taunt == m.taunt && otherMinion.poisonous == m.poisonous && otherMinion.lifesteal == m.lifesteal && m.handcard.card.isToken == otherMinion.handcard.card.isToken && otherMinion.handcard.card.race == m.handcard.card.race && otherMinion.Spellburst == m.Spellburst && otherMinion.cantAttackHeroes == m.cantAttackHeroes))
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
        /// 判断攻击顺序是否重要
        /// 在某些情况下，攻击的顺序会影响最终结果，此时AI需要考虑不同的攻击顺序
        /// 例如：存在亡语随从、风怒随从、图腾师等
        /// </summary>
        /// <param name="p">当前游戏场面</param>
        /// <returns>如果攻击顺序重要返回true，否则返回false</returns>
        public bool didAttackOrderMatters(Playfield p)
        {
            // 己方回合的特殊情况
            if (p.isOwnTurn)
            {
                // 敌方有秘密时，攻击可能触发秘密，顺序重要
                if (p.enemySecretCount >= 1) return true;
                // 敌方英雄免疫时，需要考虑如何破除免疫
                if (p.enemyHero.immune) return true;
            }
            else
            {
                // 敌方回合时，己方英雄免疫需要特殊处理
                if (p.ownHero.immune) return true;
            }

            // 获取敌方和己方随从列表
            List<Minion> enemym = (p.isOwnTurn) ? p.enemyMinions : p.ownMinions;
            List<Minion> ownm = (p.isOwnTurn) ? p.ownMinions : p.enemyMinions;

            // 检查敌方随从
            int strongestAttack = 0;
            foreach (Minion m in enemym)
            {
                if (m.Angr > strongestAttack) strongestAttack = m.Angr;
                // 有嘲讽随从时，必须先处理嘲讽
                if (m.taunt) return true;
                // 舞动之剑和亡灵骑士有负面亡语，需要特殊处理
                if (m.name == CardDB.cardNameEN.dancingswords || m.name == CardDB.cardNameEN.deathlord) return true;
            }

            // 检查己方随从的特殊效果
            int haspets = 0;          // 宠物数量（猎人）
            bool hashyena = false;    // 是否有食腐土狼
            bool hasJuggler = false;  // 是否有飞刀杂耍者
            bool spawnminions = false; // 是否有召唤随从的效果

            foreach (Minion m in ownm)
            {
                // 邪恶祭祀师：受伤时抽牌
                if (m.name == CardDB.cardNameEN.cultmaster) return true;
                // 飞刀杂耍者：召唤随从时造成伤害
                if (m.name == CardDB.cardNameEN.knifejuggler) hasJuggler = true;

                if (m.Ready && m.Angr >= 1)
                {
                    // 相邻攻击力加成（火舌图腾等）
                    if (m.AdjacentAngr >= 1) return true;

                    // 以下随从攻击后会有特殊效果，攻击顺序重要
                    if (m.name == CardDB.cardNameEN.northshirecleric) return true;    // 北郡牧师：受伤抽牌
                    if (m.name == CardDB.cardNameEN.armorsmith) return true;          // 铸甲师：受伤获得护甲
                    if (m.name == CardDB.cardNameEN.loothoarder) return true;         // 战利品贮藏者：亡语抽牌
                    if (m.name == CardDB.cardNameEN.sylvanaswindrunner) return true;  // 希尔瓦娜斯：亡语控制
                    if (m.name == CardDB.cardNameEN.darkcultist) return true;         // 黑暗教徒：亡语加血
                    if (m.ownBlessingOfWisdom >= 1) return true;                      // 智慧祝福：攻击抽牌
                    if (m.ownPowerWordGlory >= 1) return true;                        // 荣耀之语：攻击回血
                    if (m.name == CardDB.cardNameEN.acolyteofpain) return true;       // 苦痛侍僧：受伤抽牌
                    if (m.name == CardDB.cardNameEN.frothingberserker) return true;   // 暴怒的狂战士：受伤加攻
                    if (m.name == CardDB.cardNameEN.flesheatingghoul) return true;    // 腐食食尸鬼：随从死亡加攻
                    if (m.name == CardDB.cardNameEN.bloodmagethalnos) return true;    // 血法师萨尔诺斯：亡语抽牌+法强
                    if (m.name == CardDB.cardNameEN.webspinner) return true;          // 织网蛛：亡语获得野兽
                    if (m.name == CardDB.cardNameEN.tirionfordring) return true;      // 提里奥·弗丁：亡语装备武器
                    if (m.name == CardDB.cardNameEN.baronrivendare) return true;      // 瑞文戴尔男爵：双亡语

                    // 增益型随从：应该最后攻击以保持增益效果
                    if (m.name == CardDB.cardNameEN.raidleader ||        // 团队领袖
                        m.name == CardDB.cardNameEN.stormwindchampion || // 暴风城勇士
                        m.name == CardDB.cardNameEN.timberwolf ||        // 森林狼
                        m.name == CardDB.cardNameEN.southseacaptain ||   // 南海船长
                        m.name == CardDB.cardNameEN.murlocwarleader ||   // 鱼人领军
                        m.name == CardDB.cardNameEN.grimscaleoracle ||   // 灰鳞先知
                        m.name == CardDB.cardNameEN.leokk ||             // 雷欧克
                        m.name == CardDB.cardNameEN.fallenhero ||        // 堕落英雄
                        m.name == CardDB.cardNameEN.warhorsetrainer)     // 战马训练师
                        return true;

                    // 食腐土狼：野兽死亡时获得增益
                    if (m.name == CardDB.cardNameEN.scavenginghyena) hashyena = true;
                    // 统计宠物数量
                    if (m.handcard.card.race == CardDB.Race.PET) haspets++;

                    // 检查是否有召唤随从的效果（亡语或其他）
                    if (m.name == CardDB.cardNameEN.harvestgolem ||      // 收割傀儡
                        m.name == CardDB.cardNameEN.hauntedcreeper ||    // 鬼灵爬虫
                        m.souloftheforest >= 1 ||                        // 森林之魂
                        m.stegodon >= 1 ||                               // 剑龙
                        m.livingspores >= 1 ||                           // 活体孢子
                        m.infest >= 1 ||                                 // 侵蚀
                        m.ancestralspirit >= 1 ||                        // 先祖之魂
                        m.desperatestand >= 1 ||                         // 绝望复生
                        m.explorershat >= 1 ||                           // 探险家帽子
                        m.returnToHand >= 1 ||                           // 返回手牌效果
                        m.name == CardDB.cardNameEN.nerubianegg ||       // 尼鲁布蛛蛋
                        m.name == CardDB.cardNameEN.savannahhighmane ||  // 长鬃草原狮
                        m.name == CardDB.cardNameEN.sludgebelcher ||     // 淤泥喷射者
                        m.name == CardDB.cardNameEN.cairnebloodhoof ||   // 凯恩·血蹄
                        m.name == CardDB.cardNameEN.feugen ||            // 费尔根
                        m.name == CardDB.cardNameEN.stalagg ||           // 斯塔拉格
                        m.name == CardDB.cardNameEN.thebeast)            // 野兽
                        spawnminions = true;
                }
            }

            // 食腐土狼配合宠物死亡
            if (haspets >= 1 && hashyena) return true;
            // 飞刀杂耍者配合召唤随从
            if (hasJuggler && spawnminions) return true;

            return false;
        }
    }
}
