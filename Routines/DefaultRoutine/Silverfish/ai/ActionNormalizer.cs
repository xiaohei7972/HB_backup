using System.Collections.Generic;
using System.Linq;

namespace HREngine.Bots
{
    /// <summary>
    /// ActionNormalizer 类 - 负责调整动作顺序以提高效率和伤害输出
    /// 特别是在致命伤害检查中，重新排列动作顺序以获得最大伤害
    /// </summary>
    public class ActionNormalizer
    {
        /// <summary>
        /// 惩罚管理器实例 - 用于获取伤害相关的数据库
        /// </summary>
        PenalityManager penman = PenalityManager.Instance;
        
        /// <summary>
        /// 辅助功能实例 - 用于日志记录等操作
        /// </summary>
        Helpfunctions help = Helpfunctions.Instance;
        
        /// <summary>
        /// 设置实例 - 用于获取AI设置参数
        /// </summary>
        Settings settings = Settings.Instance;

        /// <summary>
        /// 目标伤害结构体 - 存储目标实体ID和受到的伤害
        /// </summary>
        public struct targetNdamage
        {
            /// <summary>
            /// 目标实体ID
            /// </summary>
            public int targetEntity;
            
            /// <summary>
            /// 受到的伤害值
            /// </summary>
            public int receivedDamage;

            /// <summary>
            /// 构造函数 - 初始化目标伤害结构体
            /// </summary>
            /// <param name="ent">目标实体ID</param>
            /// <param name="dmg">伤害值</param>
            public targetNdamage(int ent, int dmg)
            {
                // 设置目标实体ID
                this.targetEntity = ent;
                // 设置受到的伤害值
                this.receivedDamage = dmg;
            }
        }

        /// <summary>
        /// 调整动作的顺序，以提高效率和潜在的伤害输出
        /// 特别是在致命伤害检查中，此方法尝试重新排列动作顺序以获得最大伤害
        /// </summary>
        /// <param name="p">当前的游戏状态</param>
        /// <param name="isLethalCheck">是否正在进行致命伤害检查</param>
        public void adjustActions(Playfield p, bool isLethalCheck)
        {
            // 如果敌方有秘密或动作数小于2，不调整动作顺序
            // 敌方有秘密时调整动作可能会触发不利效果，动作数小于2时调整意义不大
            if (p.enemySecretCount > 0 || p.playactions.Count < 2) return;

            // 定义重排序后的动作列表
            List<Action> reorderedActions = new List<Action>();
            // 存储随机伤害的动作ID及其对应的伤害
            Dictionary<int, Dictionary<int, int>> rndActIdsDmg = new Dictionary<int, Dictionary<int, int>>();
            // 用于模拟原始动作序列执行结果的游戏状态
            Playfield tmpPlOld = new Playfield();

            // 致命伤害检查逻辑
            if (isLethalCheck)
            {
                // 如果当前场地值低于10000，说明不是致命伤害场景，不调整
                if (Ai.Instance.botBase.getPlayfieldValue(p) < 10000) return;
                
                // 创建临时游戏状态用于模拟
                Playfield tmpPf = new Playfield();
                
                // 如果敌方有嘲讽随从，无法直接攻击敌方英雄，不调整
                if (tmpPf.anzEnemyTaunt > 0) return;

                // 存储每个动作造成的伤害
                Dictionary<Action, int> actDmgDict = new Dictionary<Action, int>();
                // 初始化敌方英雄血量为30
                tmpPf.enemyHero.Hp = 30;
                
                try
                {
                    // 记录英雄技能和英雄攻击的使用次数
                    int useability = 0;
                    
                    // 遍历所有动作，计算每个动作对敌方英雄造成的伤害
                    foreach (Action a in p.playactions)
                    {
                        // 如果使用英雄技能，记录为1
                        if (a.actionType == actionEnum.useHeroPower) useability = 1;
                        // 如果使用英雄攻击，次数加1
                        if (a.actionType == actionEnum.attackWithHero) useability++;
                        
                        // 记录当前敌方英雄的总生命值（血量+护甲）
                        int actDmd = tmpPf.enemyHero.Hp + tmpPf.enemyHero.armor;
                        // 执行动作
                        tmpPf.doAction(a);
                        // 计算动作造成的伤害（执行前总生命值 - 执行后总生命值）
                        actDmd -= (tmpPf.enemyHero.Hp + tmpPf.enemyHero.armor);
                        // 将动作和对应的伤害添加到字典中
                        actDmgDict.Add(a, actDmd);
                    }
                    
                    // 如果英雄技能和英雄攻击的使用次数超过1，不调整
                    if (useability > 1) return;
                }
                catch
                {
                    // 如果执行过程中出错，不调整
                    return;
                }

                // 根据动作造成的伤害从高到低进行排序
                foreach (var pair in actDmgDict.OrderByDescending(pair => pair.Value))
                {
                    // 将排序后的动作添加到新列表
                    reorderedActions.Add(pair.Key);
                }

                // 重新创建临时游戏状态，验证排序后的动作序列是否可行
                tmpPf = new Playfield();
                foreach (Action a in reorderedActions)
                {
                    // 检查动作是否可行
                    if (!isActionPossible(tmpPf, a)) return;
                    try
                    {
                        // 执行动作
                        tmpPf.doAction(a);
                    }
                    catch
                    {
                        // 如果执行出错，打印错误信息并返回
                        this.printError(p.playactions, reorderedActions, a);
                        return;
                    }
                }

                // 如果调整后的场地值低于10000，说明调整后不是致命伤害场景，不进行调整
                if (Ai.Instance.botBase.getPlayfieldValue(tmpPf) < 10000) return;
            }
            else
            {
                // 非致命伤害检查逻辑
                // 标记是否有随机伤害
                bool damageRandom = false;
                // 标记是否在AOE伤害前有随机伤害
                bool rndBeforeDamageAll = false;
                Action aa;
                
                // 第一次遍历，检查是否有随机伤害和AOE伤害的组合
                for (int i = 0; i < p.playactions.Count; i++)
                {
                    aa = p.playactions[i];
                    switch (aa.actionType)
                    {
                        case actionEnum.playcard:
                            // 如果已经有随机伤害，且当前是AOE伤害，则标记为随机伤害在AOE伤害前
                            if (damageRandom && penman.DamageAllEnemysDatabase.ContainsKey(aa.card.card.nameEN)) rndBeforeDamageAll = true;
                            // 如果当前是随机伤害，标记为有随机伤害
                            else if (penman.DamageRandomDatabase.ContainsKey(aa.card.card.nameEN)) damageRandom = true;
                            break;
                    }
                }

                // AOE法术的索引位置，用于将AOE法术移到前面
                int aoeEnNum = 0;
                // 位置不正确的动作数量
                int outOfPlace = 0;
                // 是否使用了图腾召唤英雄技能
                bool totemiccall = false;
                // 存储随机伤害动作的列表
                List<Action> rndAct = new List<Action>();
                List<Action> rndActTmp = new List<Action>();
                
                // 第二次遍历，调整动作顺序
                for (int i = 0; i < p.playactions.Count; i++)
                {
                    // 重置随机伤害标记
                    damageRandom = false;
                    aa = p.playactions[i];
                    // 将当前动作添加到重排序列表
                    reorderedActions.Add(aa);
                    
                    switch (aa.actionType)
                    {
                        case actionEnum.useHeroPower:
                            // 如果使用的是图腾召唤英雄技能，标记为使用了图腾召唤
                            if (aa.card.card.nameEN == CardDB.cardNameEN.totemiccall) totemiccall = true;
                            break;
                        case actionEnum.playcard:
                            // 如果当前是AOE伤害法术
                            if (penman.DamageAllEnemysDatabase.ContainsKey(aa.card.card.nameEN))
                            {
                                // 如果AOE法术不在正确的位置
                                if (i != aoeEnNum)
                                {
                                    // 如果使用了图腾召唤且当前是法术，不调整
                                    if (totemiccall && aa.card.card.type == CardDB.cardtype.SPELL) return;
                                    // 从当前位置移除AOE法术
                                    reorderedActions.RemoveAt(i);
                                    // 将AOE法术插入到正确的位置
                                    reorderedActions.Insert(aoeEnNum, aa);
                                    // 位置不正确的动作数量加1
                                    outOfPlace++;
                                }
                                // AOE法术的索引位置加1
                                aoeEnNum++;
                            }
                            // 如果有随机伤害在AOE伤害前，且当前是随机伤害法术
                            else if (rndBeforeDamageAll && aa.card.card.type == CardDB.cardtype.SPELL && penman.DamageRandomDatabase.ContainsKey(aa.card.card.nameEN))
                            {
                                // 标记为随机伤害
                                damageRandom = true;
                                // 创建临时游戏状态，用于记录随机伤害的结果
                                Playfield tmp = new Playfield(tmpPlOld);
                                // 执行动作，记录结果到tmpPlOld
                                tmpPlOld.doAction(aa);

                                // 存储随机伤害的目标和伤害值
                                Dictionary<int, int> actIdDmg = new Dictionary<int, int>();
                                
                                // 检查敌方英雄是否受到伤害
                                if (tmp.enemyHero.Hp != tmpPlOld.enemyHero.Hp) 
                                    actIdDmg.Add(tmpPlOld.enemyHero.entitiyID, tmp.enemyHero.Hp - tmpPlOld.enemyHero.Hp);
                                // 检查己方英雄是否受到伤害
                                if (tmp.ownHero.Hp != tmpPlOld.ownHero.Hp) 
                                    actIdDmg.Add(tmpPlOld.ownHero.entitiyID, tmp.ownHero.Hp - tmpPlOld.ownHero.Hp);
                                
                                bool found = false;
                                // 检查敌方随从是否受到伤害
                                foreach (Minion m in tmp.enemyMinions)
                                {
                                    found = false;
                                    foreach (Minion nm in tmpPlOld.enemyMinions)
                                    {
                                        if (m.entitiyID == nm.entitiyID)
                                        {
                                            found = true;
                                            // 如果随从受到伤害，记录伤害值
                                            if (m.Hp != nm.Hp) actIdDmg.Add(m.entitiyID, m.Hp - nm.Hp);
                                            break;
                                        }
                                    }
                                    // 如果是新出现的随从，记录其当前生命值
                                    if (!found) actIdDmg.Add(m.entitiyID, m.Hp);
                                }
                                // 检查己方随从是否受到伤害
                                foreach (Minion m in tmp.ownMinions)
                                {
                                    found = false;
                                    foreach (Minion nm in tmpPlOld.ownMinions)
                                    {
                                        if (m.entitiyID == nm.entitiyID)
                                        {
                                            found = true;
                                            // 如果随从受到伤害，记录伤害值
                                            if (m.Hp != nm.Hp) actIdDmg.Add(m.entitiyID, m.Hp - nm.Hp);
                                            break;
                                        }
                                    }
                                    // 如果是新出现的随从，记录其当前生命值
                                    if (!found) actIdDmg.Add(m.entitiyID, m.Hp);
                                }
                                // 将随机伤害的结果添加到字典中
                                rndActIdsDmg.Add(aa.card.entity, actIdDmg);
                            }
                            break;
                    }
                    // 如果不是随机伤害，执行动作到tmpPlOld
                    if (!damageRandom) tmpPlOld.doAction(aa);
                }

                // 如果没有位置不正确的动作，不调整
                if (outOfPlace == 0) return;

                // 创建临时游戏状态，验证调整后的动作序列
                Playfield tmpPf = new Playfield();
                foreach (Action a in reorderedActions)
                {
                    // 检查动作是否可行
                    if (!isActionPossible(tmpPf, a)) return;
                    try
                    {
                        // 如果不是随机伤害法术，直接执行动作
                        if (!(a.actionType == actionEnum.playcard && rndActIdsDmg.ContainsKey(a.card.entity)))
                            tmpPf.doAction(a);
                        else
                        {
                            // 如果是随机伤害法术，手动处理伤害
                            tmpPf.playactions.Add(a);
                            // 获取随机伤害的结果
                            Dictionary<int, int> actIdDmg = rndActIdsDmg[a.card.entity];
                            // 处理敌方英雄的伤害
                            if (actIdDmg.ContainsKey(tmpPf.enemyHero.entitiyID))
                                tmpPf.minionGetDamageOrHeal(tmpPf.enemyHero, actIdDmg[tmpPf.enemyHero.entitiyID]);
                            // 处理己方英雄的伤害
                            if (actIdDmg.ContainsKey(tmpPf.ownHero.entitiyID))
                                tmpPf.minionGetDamageOrHeal(tmpPf.ownHero, actIdDmg[tmpPf.ownHero.entitiyID]);
                            // 处理敌方随从的伤害
                            foreach (Minion m in tmpPf.enemyMinions)
                            {
                                if (actIdDmg.ContainsKey(m.entitiyID))
                                    tmpPf.minionGetDamageOrHeal(m, actIdDmg[m.entitiyID]);
                            }
                            // 处理己方随从的伤害
                            foreach (Minion m in tmpPf.ownMinions)
                            {
                                if (actIdDmg.ContainsKey(m.entitiyID))
                                    tmpPf.minionGetDamageOrHeal(m, actIdDmg[m.entitiyID]);
                            }
                            // 触发伤害相关的效果
                            tmpPf.doDmgTriggers();
                        }
                    }
                    catch
                    {
                        // 如果执行出错，打印错误信息并返回
                        printError(p.playactions, reorderedActions, a);
                        return;
                    }
                }

                // 复制丢失的伤害信息
                tmpPf.lostDamage = tmpPlOld.lostDamage;
                // 计算调整后的场地值
                float newval = Ai.Instance.botBase.getPlayfieldValue(tmpPf);
                // 计算调整前的场地值
                float oldval = Ai.Instance.botBase.getPlayfieldValue(tmpPlOld);

                // 如果调整后的场地值比原来低，不调整
                if (oldval > newval) return;
            }
            
            // 打印调整前的动作顺序
            help.logg("Old order of actions:");
            foreach (Action a in p.playactions) a.print();

            // 清空原动作列表
            p.playactions.Clear();
            // 添加调整后的动作列表
            p.playactions.AddRange(reorderedActions);

            // 打印调整后的动作顺序
            help.logg("New order of actions:");
        }

        /// <summary>
        /// 检查某个操作在当前的游戏状态下是否可行，包括使用地标的判断
        /// </summary>
        /// <param name="p">当前的游戏状态</param>
        /// <param name="a">要检查的操作</param>
        /// <returns>如果操作可行，则返回 true；否则返回 false</returns>
        private bool isActionPossible(Playfield p, Action a)
        {
            // 标记动作是否可行
            bool actionFound = false;

            // 根据操作类型进行不同的检查
            switch (a.actionType)
            {
                case actionEnum.playcard:
                    // 检查是否可以打出卡牌
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        // 找到对应的手牌
                        if (hc.entity == a.card.entity)
                        {
                            // 判断法力值是否足够
                            if (p.mana >= hc.card.getManaCost(p, hc.manacost))
                            {
                                // 如果随从已满且尝试打出随从或地标，则返回 false
                                if (p.ownMinions.Count >= 7)
                                    if (hc.card.type == CardDB.cardtype.MOB || hc.card.type == CardDB.cardtype.LOCATION)
                                        return actionFound;

                                // 动作可行
                                actionFound = true;
                            }
                            break;
                        }
                    }
                    break;

                case actionEnum.attackWithMinion:
                    // 检查随从是否可以攻击
                    // 不能攻击己方目标
                    if (a.target.own)
                    {
                        actionFound = false;
                        return actionFound;
                    }
                    // 查找要攻击的随从
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.entitiyID == a.own.entitiyID)
                        {
                            // 如果随从没有准备好，则返回 false
                            if (!m.Ready)
                                return false;

                            // 动作可行
                            actionFound = true;
                            break;
                        }
                    }
                    break;

                case actionEnum.attackWithHero:
                    // 检查英雄是否可以攻击
                    if (p.ownHero.Ready)
                        actionFound = true;
                    break;

                case actionEnum.useHeroPower:
                    // 检查英雄技能是否可以使用
                    if (p.ownAbilityReady && p.mana >= p.ownHeroAblility.card.getManaCost(p, p.ownHeroAblility.manacost))
                        actionFound = true;
                    break;

                case actionEnum.trade:
                    // 检查卡牌是否可以交易
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.entity == a.card.entity)
                        {
                            // 检查是否满足交易的条件
                            if (hc.card.Tradeable && p.mana >= hc.card.TradeCost && p.ownDeckSize > 0)
                            {
                                actionFound = true;
                            }
                            break;
                        }
                    }
                    break;

                case actionEnum.useLocation:
                    // 检查地标是否可以使用
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.entitiyID == a.own.entitiyID)
                        {
                            // 确认地标类型并检查其冷却时间是否已经结束
                            if (m.handcard.card.type == CardDB.cardtype.LOCATION && m.CooldownTurn == 0)
                            {
                                actionFound = true;
                            }
                            break;
                        }
                    }
                    break;

                case actionEnum.useTitanAbility:
                    // 检查泰坦是否可以使用
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.entitiyID == a.own.entitiyID)
                        {
                            // 确认泰坦技能是否可以使用（至少有一个技能未使用）
                            if (m.handcard.card.Titan && (!m.TitanAbilityUsed1 || !m.TitanAbilityUsed2 || !m.TitanAbilityUsed3))
                            {
                                actionFound = true;
                            }
                            break;
                        }
                    }
                    break;

                case actionEnum.forge:
                    // 检查卡牌是否可以锻造
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.entity == a.card.entity)
                        {
                            // 检查是否满足锻造的条件
                            if (hc.card.Forge && p.mana >= hc.card.ForgeCost && !hc.card.Forged)
                            {
                                actionFound = true;
                            }
                            break;
                        }
                    }
                    break;
                    /*                 case actionEnum.launchStarship:
                                        break;
                                    case actionEnum.useUnderfelRift:
                                        break;
                                    case actionEnum.rewind:
                                        break; */
            }

            // 如果在上面的检查中没有找到合适的操作，返回 false
            if (!actionFound)
                return false;

            // 如果操作有目标，则需要检查目标是否存在
            if (a.target != null)
            {
                // 重置动作可行标记
                actionFound = false;

                // 检查目标是否是敌方英雄或己方英雄
                if (p.enemyHero.entitiyID == a.target.entitiyID || p.ownHero.entitiyID == a.target.entitiyID)
                {
                    actionFound = true;
                }
                else
                {
                    // 检查目标是否是敌方随从
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.entitiyID == a.target.entitiyID)
                        {
                            actionFound = true;
                            break;
                        }
                    }
                    // 如果不是敌方随从，检查是否是己方随从
                    if (!actionFound)
                    {
                        foreach (Minion m in p.ownMinions)
                        {
                            if (m.entitiyID == a.target.entitiyID)
                            {
                                actionFound = true;
                                break;
                            }
                        }
                    }
                }
            }

            // 返回动作是否可行
            return actionFound;
        }

        /// <summary>
        /// 打印动作重排序错误信息
        /// </summary>
        /// <param name="mainActList">原始动作列表</param>
        /// <param name="newActList">新动作列表</param>
        /// <param name="aError">出错的动作</param>
        private void printError(List<Action> mainActList, List<Action> newActList, Action aError)
        {
            // 打印错误日志
            help.ErrorLog("Reordering actions error!");
            help.logg("Reordering actions error!\r\nError in action:");
            // 打印出错的动作
            aError.print();
            // 打印原始动作顺序
            help.logg("Main order of actions:");
            foreach (Action a in mainActList) a.print();
            // 打印新动作顺序
            help.logg("New order of actions:");
            foreach (Action a in newActList) a.print();
            return;
        }

        /// <summary>
        /// 检查是否有丢失的最佳动作
        /// 通过模拟执行当前动作，然后使用 MiniSimulator 重新计算最佳动作序列
        /// </summary>
        /// <param name="p">当前的游戏状态</param>
        public void checkLostActions(Playfield p)
        {
            // 创建临时游戏状态，用于模拟执行当前动作
            Playfield tmpPf = new Playfield();
            foreach (Action a in p.playactions)
            {
                // 调整动作的所有权标记，确保动作可以正确执行
                if (a.target != null && a.own != null) a.own.own = !a.target.own;
                // 执行动作
                tmpPf.doAction(a);
            }
            
            // 创建迷你模拟器，用于重新计算最佳动作序列
            MiniSimulator mainTurnSimulator = new MiniSimulator(6, 3000, 0);
            // 设置第二回合模拟参数
            mainTurnSimulator.setSecondTurnSimu(settings.simulateEnemysTurn, settings.secondTurnAmount);
            // 设置防AOE参数
            mainTurnSimulator.setPlayAround(settings.playaround, settings.playaroundprob, settings.playaroundprob2);

            // 标记为检查丢失的动作
            tmpPf.checkLostAct = true;
            // 继承致命伤害检查标记
            tmpPf.isLethalCheck = p.isLethalCheck;

            // 执行所有可能的动作，计算最佳结果
            float bestval = mainTurnSimulator.doallmoves(tmpPf);
            // 如果新的最佳值比当前值高，更新动作列表
            if (bestval > p.value)
            {
                // 清空原动作列表
                p.playactions.Clear();
                // 添加新的最佳动作列表
                p.playactions.AddRange(mainTurnSimulator.bestboard.playactions);
                // 更新场地值
                p.value = bestval;
            }
        }
    }


}