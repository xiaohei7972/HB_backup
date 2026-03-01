using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
#if APPLICATION_MODE
    using RoutineHelper;
#endif

namespace HREngine.Bots
{
    /// <summary>
    /// 迷你模拟器类，用于模拟游戏状态和计算最优行动
    /// 负责搜索最优行动、模拟游戏状态、执行AI决策等功能
    /// </summary>
    public partial class MiniSimulator
    {
        //#####################################################################################################################
        /// <summary>
        /// 最大搜索深度
        /// </summary>
        private int maxdeep = 6;
        /// <summary>
        /// 最大搜索宽度
        /// </summary>
        private int maxwide = 10;
        /// <summary>
        /// 总牌面数，0表示没有限制每一层的大小
        /// </summary>
        private int totalboards = 0; // 0表示没有限制每一层的大小
        /// <summary>
        /// 是否使用惩罚管理器
        /// </summary>
        private bool usePenalityManager = true;
        /// <summary>
        /// 是否使用目标裁剪
        /// </summary>
        private bool useCutingTargets = true;
        /// <summary>
        /// 是否不重新计算
        /// </summary>
        private bool dontRecalc = true;
        /// <summary>
        /// 是否使用斩杀检查
        /// </summary>
        private bool useLethalCheck = false;
        /// <summary>
        /// 是否使用比较
        /// </summary>
        private bool useComparison = true;

        /// <summary>
        /// 是否打印普通信息
        /// </summary>
        private bool printNormalstuff = false;
        /// <summary>
        /// 是否打印信息，单线程多线程，Todo:借用这个变量来调试actions
        /// </summary>
        public bool print = false; //单线程多线程，Todo:借用这个变量来调试actions

        /// <summary>
        /// 可能的移动列表
        /// </summary>
        List<Playfield> posmoves = new List<Playfield>(7000);
        /// <summary>
        /// 最佳旧重复牌面列表
        /// </summary>
        List<Playfield> bestoldDuplicates = new List<Playfield>(7000);
        /// <summary>
        /// 两回合牌面列表
        /// </summary>
        List<Playfield> twoturnfields = new List<Playfield>(500);

        /// <summary>
        /// 线程结果列表
        /// </summary>
        List<List<Playfield>> threadresults = new List<List<Playfield>>(64);
        /// <summary>
        /// 两回合模拟数量
        /// </summary>
        private int dirtyTwoTurnSim = 256;

        /// <summary>
        /// 最佳移动
        /// </summary>
        public Action bestmove = null;
        /// <summary>
        /// 最佳移动值
        /// </summary>
        public float bestmoveValue = 0;
        /// <summary>
        /// 最佳旧值
        /// </summary>
        private float bestoldval = -20000000;
        /// <summary>
        /// 最佳牌面
        /// </summary>
        public Playfield bestboard = new Playfield();

        /// <summary>
        /// 机器人基础行为
        /// </summary>
        public Behavior botBase = null;
        /// <summary>
        /// 计算数量
        /// </summary>
        private int calculated = 0;
        /// <summary>
        /// 是否足够计算
        /// </summary>
        private bool enoughCalculations = false;

        /// <summary>
        /// 是否进行斩杀检查
        /// </summary>
        private bool isLethalCheck = false;
        /// <summary>
        /// 是否模拟第二回合
        /// </summary>
        private bool simulateSecondTurn = false;
        /// <summary>
        /// 是否防奥秘
        /// </summary>
        private bool playaround = false;
        /// <summary>
        /// 防奥秘概率
        /// </summary>
        private int playaroundprob = 50;
        /// <summary>
        /// 防奥秘概率2
        /// </summary>
        private int playaroundprob2 = 80;

        /// <summary>
        /// 线程号锁
        /// </summary>
        private static readonly object threadnumberLocker = new object();
        /// <summary>
        /// 全局线程号
        /// </summary>
        private int threadnumberGlobal = 0;

        /// <summary>
        /// 移动生成器实例
        /// </summary>
        Movegenerator movegen = Movegenerator.Instance;

        /// <summary>
        /// 惩罚管理器实例
        /// </summary>
        PenalityManager pen = PenalityManager.Instance;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MiniSimulator()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deep">最大搜索深度</param>
        /// <param name="wide">最大搜索宽度</param>
        /// <param name="ttlboards">总牌面数</param>
        public MiniSimulator(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        /// <summary>
        /// 更新参数
        /// </summary>
        /// <param name="deep">最大搜索深度</param>
        /// <param name="wide">最大搜索宽度</param>
        /// <param name="ttlboards">总牌面数</param>
        public void updateParams(int deep, int wide, int ttlboards)
        {
            this.maxdeep = deep;
            this.maxwide = wide;
            this.totalboards = ttlboards;
        }

        /// <summary>
        /// 设置打印信息
        /// </summary>
        /// <param name="sp">是否打印普通信息</param>
        public void setPrintingstuff(bool sp)
        {
            this.printNormalstuff = sp;
        }

        /// <summary>
        /// 设置第二回合模拟
        /// </summary>
        /// <param name="sts">是否模拟第二回合</param>
        /// <param name="amount">模拟数量</param>
        public void setSecondTurnSimu(bool sts, int amount)
        {
            this.simulateSecondTurn = sts;
            this.dirtyTwoTurnSim = amount;
        }

        /// <summary>
        /// 获取第二回合模拟数量
        /// </summary>
        /// <returns>模拟数量</returns>
        public int getSecondTurnSimu()
        {
            return this.dirtyTwoTurnSim;
        }
        public bool getSimulateSecondTurn()
        {
            return this.simulateSecondTurn;
        }

        /// <summary>
        /// 设置防奥秘
        /// </summary>
        /// <param name="spa">是否防奥秘</param>
        /// <param name="pprob">防奥秘概率</param>
        /// <param name="pprob2">防奥秘概率2</param>
        public void setPlayAround(bool spa, int pprob, int pprob2)
        {
            this.playaround = spa;
            this.playaroundprob = pprob;
            this.playaroundprob2 = pprob2;
        }

        /// <summary>
        /// 添加到可能的移动列表
        /// </summary>
        /// <param name="pf">游戏状态</param>
        private void addToPosmoves(Playfield pf)
        {
            if (pf.ownHero.Hp <= 0) return;
            this.posmoves.Add(pf);
            if (this.totalboards >= 1)
            {
                this.calculated++;
            }
        }

        /// <summary>
        /// 执行所有可能的移动
        /// 计算最优行动并更新相关状态
        /// </summary>
        /// <param name="playf">游戏状态</param>
        /// <returns>最优移动值</returns>
        public float doallmoves(Playfield playf)
        {
            // 设置打印模式
            print = playf.print;
            // 设置是否进行斩杀检查
            this.isLethalCheck = playf.isLethalCheck; // 是否做斩杀检验
            // 初始化计算状态
            enoughCalculations = false; //计算是否足够，深度大于最大深度，计算场面数大于最大宽度时为true
            // 获取机器人基础行为策略
            botBase = Ai.Instance.botBase;  // 用哪个策略，策略文件
            // 清空可能的移动列表
            this.posmoves.Clear();
            // 清空两回合牌面列表
            this.twoturnfields.Clear();
            // 将当前游戏状态加入可能的移动列表
            this.addToPosmoves(playf);  //将当前场面加入到状态队列
            // 初始化是否有可执行操作的标志
            bool havedonesomething = true;  //是否无步骤可出，例如：法力值不够出任何牌，随从全部已经攻击
            // 创建临时游戏状态列表
            List<Playfield> temp = new List<Playfield>(); //这一回合的状态队列
            // 初始化搜索深度
            int deep = 0;
            // 初始化计算计数
            this.calculated = 0;
            // 初始化最佳游戏状态
            Playfield bestold = null;
            // 初始化最佳游戏状态值
            bestoldval = -20000000;  //最小的val，用于标记是否已经计算过场面val

            // 循环直到没有可执行的操作
            while (havedonesomething)
            {
                // 每次循环是同一回合的一步，每多一步，deep加1
                // 执行垃圾回收
                GC.Collect();
                // 清空临时列表
                temp.Clear();
                // 将当前可能的移动列表复制到临时列表
                temp.AddRange(this.posmoves);

                // 清空可能的移动列表
                this.posmoves.Clear();
                // 重置是否有可执行操作的标志
                havedonesomething = false;
                // 重置线程号
                threadnumberGlobal = 0;

                // 根据是否打印决定单线程或多线程执行
                if (print)
                    // 单线程执行
                    startEnemyTurnSimThread(temp, 0, temp.Count);
                else
                {
                    // 多线程计算
                    Parallel.ForEach(Partitioner.Create(0, temp.Count),  // 多线程计算
                         range =>
                         {
                             // 为每个范围生成下一层游戏状态
                             startEnemyTurnSimThread(temp, range.Item1, range.Item2); //生成下一层，赋值temp[i].nextPlayfields
                         });
                }
                // 初始化索引和最佳索引
                int idx = 0;
                int best_idx = 0;

                // 遍历临时列表中的每个游戏状态
                foreach (Playfield p in temp)  // 到这里nextPlayfields 哪些牌面计算好了（动作也都模拟做完了），value还没算，evaluatePenality已经计算好了。
                {
                    // 增加索引
                    idx++;
                    // 计算游戏状态的价值
                    float pVal = botBase.getPlayfieldValue(p);
                    // 保存价值到游戏状态
                    p.value = pVal;
                    // 如果当前价值大于最佳价值
                    if (pVal > bestoldval)  // 找最优得分场面
                    {
                        // 更新最佳价值
                        bestoldval = pVal;
                        // 更新最佳游戏状态
                        bestold = p;
                        // 清空最佳旧重复牌面列表
                        bestoldDuplicates.Clear();
                        // 更新最佳索引
                        best_idx = idx;
                        // TODO 记录树层
                    }
                    // 如果当前价值等于最佳价值
                    else if (pVal == bestoldval)
                        // 将当前游戏状态加入最佳旧重复牌面列表
                        bestoldDuplicates.Add(p);  //如果val相等则加入bestoldDuplicates，方便之后调用

                    // 如果是测试模式
                    if (Settings.Instance.test)  // 重点调试，打印每一个step里面 每一个action带来的牌面，以及得分
                    {
                        // 打印动作
                        // Helpfunctions.Instance.logg(string.Format("树层:{0}，牌面{0}-{1}: actions {2}, 得分:{3}", deep, idx, p.playactions.Count, pVal));
                        p.printActions();
                        // Helpfunctions.Instance.logg("");


#if APPLICATION_MODE
                        // 更新动作视图树
                        (Application.Current.MainWindow as MainWindow).UpdateActionViewTree(deep, idx, pVal, p.playactions, p);
#endif

                    }
                }
                // 对临时列表按价值降序排序
                temp.Sort((a, b) => -a.value.CompareTo(b.value));

                // 根据计算量进行剪枝
                if (this.calculated > Ai.Instance.maxwide)
                {
                    // 加快计算，只保留一部分游戏状态
                    temp = temp.Take(Ai.Instance.maxCal / 6).ToList();
                }
                else if (this.calculated > Ai.Instance.maxwide * 2 / 3)
                {
                    // 加快计算，只保留一部分游戏状态
                    temp = temp.Take(Ai.Instance.maxCal / 2).ToList();
                }
                else
                {
                    // 保留指定数量的游戏状态
                    temp = temp.Take(Ai.Instance.maxCal).ToList();
                }

                // 处理每个游戏状态
                temp.ForEach(p =>
                {
                    // 如果计算量超过总牌面数
                    if (this.calculated > this.totalboards)
                    {
                        // Helpfunctions.Instance.logg(string.Format("触发剪枝,deep={0},已计算={1}>阈值{2},牌面{3}的所有子孙牌面被抛弃", deep + 1, calculated, totalboards, idx));
                    }
                    else
                    {
                        // 增加计算量
                        if (this.totalboards > 0) this.calculated += p.nextPlayfields.Count;
                        // 将下一层牌面加入可能的移动列表
                        this.posmoves.AddRange(p.nextPlayfields); //将下一层牌面进入队列，赋值posmoves下一层牌面
                        // 清空下一层牌面列表
                        p.nextPlayfields.Clear();
                    }
                });

                // 如果是测试模式且找到最佳游戏状态
                if (best_idx > 0 && Settings.Instance.test)
                {
                    // 打印最佳游戏状态信息
                    // Helpfunctions.Instance.logg(string.Format("树第{0}层全局最优牌面下标:{1},得分:{2}", deep, best_idx, bestoldval));
                    // Helpfunctions.Instance.logg("");
                }

                // 如果最佳价值大于等于10000（表示可以斩杀）且满足条件
                if (bestoldval >= 10000 && (Hrtprozis.Instance.enemySecretCount == 0 || Hrtprozis.Instance.enemyHeroStartClass != TAG_CLASS.MAGE))
                    // 清空可能的移动列表，因为已经找到斩杀方案
                    this.posmoves.Clear();
                //如果bestval大于等于10000则意味着兄弟计算出当前场面可以斩杀
                //对应的策略中的代码是 if (p.enemyHero.Hp <= 0) retval = 10000;

                // 如果还有其他状态可以模拟，则继续运算
                if (this.posmoves.Count > 0) havedonesomething = true;

                // 记录剪枝前的数量
                int num_before_cut = posmoves.Count;
                // 执行剪枝，去除重复的游戏状态
                cuttingposibilities(isLethalCheck); //去除重复的PlayField  这里面有排序，以及对posmoves的赋值
                // Helpfunctions.Instance.logg("cut to len 去重从" + num_before_cut + " 到 " + this.posmoves.Count);

                // 增加搜索深度
                deep++;
                // 清空临时列表
                temp.Clear();

                // 检查是否计算足够
                if (this.calculated > this.totalboards) enoughCalculations = true;
                if (deep >= this.maxdeep) enoughCalculations = true;
            }

            // 如果需要两回合模拟且最佳游戏状态不在两回合牌面列表中
            if (this.dirtyTwoTurnSim > 0 && !twoturnfields.Contains(bestold))
                // 将最佳游戏状态加入两回合牌面列表
                twoturnfields.Add(bestold);
            // 清空可能的移动列表
            this.posmoves.Clear();
            // 将最佳游戏状态加入可能的移动列表
            this.posmoves.Add(bestold);
            // 将最佳旧重复牌面加入可能的移动列表
            this.posmoves.AddRange(bestoldDuplicates);

            // 搜索最佳玩法...........................................................
            // 首先执行两回合模拟
            if (!isLethalCheck && bestoldval < 10000) doDirtyTwoTurnsim();  // 搜索2个回合

            // 如果有游戏状态
            if (posmoves.Count >= 1)  //所有场面根据得分排序，高分排在前面，优先搜索
            {
                // 按价值降序排序
                posmoves.Sort((a, b) => botBase.getPlayfieldValue(b).CompareTo(botBase.getPlayfieldValue(a)));
                // 初始化最佳游戏状态和价值
                Playfield bestplay = posmoves[0];
                float bestval = botBase.getPlayfieldValue(bestplay);
                int pcount = posmoves.Count;
                // 遍历所有游戏状态
                for (int i = 1; i < pcount; i++)
                {
                    // 计算当前游戏状态的价值
                    float val = botBase.getPlayfieldValue(posmoves[i]);
                    // 如果当前价值小于最佳价值，跳出循环
                    if (bestval > val) break;
                    // 如果当前游戏状态的出牌数量大于最佳游戏状态，跳过
                    if (posmoves[i].cardsPlayedThisTurn > bestplay.cardsPlayedThisTurn) continue;
                    // 如果出牌数量相同
                    else if (posmoves[i].cardsPlayedThisTurn == bestplay.cardsPlayedThisTurn)
                    {
                        // 如果最佳游戏状态的选项出牌数量大于当前游戏状态，跳过
                        if (bestplay.optionsPlayedThisTurn > posmoves[i].optionsPlayedThisTurn) continue;
                        // 如果选项出牌数量相同且最佳游戏状态的敌方英雄生命值小于等于当前游戏状态，跳过
                        else if (bestplay.optionsPlayedThisTurn == posmoves[i].optionsPlayedThisTurn && bestplay.enemyHero.Hp <= posmoves[i].enemyHero.Hp) continue;

                    }
                    // 更新最佳游戏状态和价值
                    bestplay = posmoves[i];
                    bestval = val;
                }
                // 获取最佳移动
                this.bestmove = bestplay.getNextAction();
                // 保存最佳移动价值
                this.bestmoveValue = bestval;
                // 创建最佳游戏状态的副本
                this.bestboard = new Playfield(bestplay);
                // 复制猜测的英雄生命值
                this.bestboard.guessingHeroHP = bestplay.guessingHeroHP;
                // 复制价值
                this.bestboard.value = bestplay.value;
                // 复制哈希码
                this.bestboard.hashcode = bestplay.hashcode;
                // 清空最佳旧重复牌面列表
                bestoldDuplicates.Clear();
                // 返回最佳价值
                return bestval;  // 正常退出值
            }
            // 如果没有游戏状态，设置默认值
            this.bestmove = null;
            this.bestmoveValue = -2000000;
            this.bestboard = playf;

            // 返回异常退出值
            return -2000000; //异常退出值
        }


        /// <summary>
        /// 启动敌方回合模拟线程
        /// 生成下一步所有可能的操作并模拟执行
        /// </summary>
        /// <param name="source">游戏状态列表</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="endIndex">结束索引</param>
        private void startEnemyTurnSimThread(List<Playfield> source, int startIndex, int endIndex)
        {
            // 初始化线程号
            int threadnumber = 0;
            // 线程号锁，确保线程号分配的原子性
            lock (threadnumberLocker)
            {
                // 获取并增加全局线程号
                threadnumber = threadnumberGlobal++;
                // 唤醒等待的线程
                System.Threading.Monitor.Pulse(threadnumberLocker);
            }
            // 检查线程号是否超过最大线程数
            if (threadnumber > Ai.Instance.maxNumberOfThreads - 2)
            {
                // 限制线程号在有效范围内
                threadnumber = Ai.Instance.maxNumberOfThreads - 2;
                // 记录错误日志
                Helpfunctions.Instance.ErrorLog("You need more threads!");
                // 退出方法
                return;
            }

            // 获取斩杀设置
            int berserk = Settings.Instance.berserkIfCanFinishNextTour;
            // 获取规则打印设置
            int printRules = Settings.Instance.printRules;
            // 遍历指定范围内的游戏状态
            for (int i = startIndex; i < endIndex; i++)  // 不同的index对应同一步的不同选择到达的不同牌面
            {
                // 获取当前游戏状态
                Playfield p = source[i];
                // 计算游戏状态的价值
                float pv = botBase.getPlayfieldValue(p);
                // 如果场面得分过低，弃掉这个场面以及这个场面的子序列，提前剪枝
                if (pv < -10000)
                    continue;
                // 如果计算不足
                if (!enoughCalculations)
                {
                    // 生成所有可能的移动
                    // //从这里进入调用getMoveList函数，这个函数返回的是兄弟下一步所有可以做的操作
                    //如果调试到这里，输出actions，发现没有你想要的Action就可以跟进去看看
                    //一般来说，法力值不够出的牌会直接省略，如果正常操作能够出的牌没出，
                    //则说明，惩罚值大于500，操作直接被省略
                    //建议直接跟进去看寻找原因，可以加深理解
                    List<Action> actions = movegen.getMoveList(p, usePenalityManager, useCutingTargets, true); // 这里面会算每个action的惩罚值
                    //读取到actions后接下来对每个步骤进行模拟
                    //从而得到操作之后的场面并且计算val值
                    // 如果需要打印规则，保存结束回合状态
                    if (printRules > 0) p.endTurnState = new Playfield(p);
                    // 记录已经模拟过的泰坦技能动作
                    var usedTitanSkills = "";
                    // 遍历每个动作
                    foreach (Action a in actions)  // 到这步 a.penalty已经计算好了
                    {
                        // 检查是否为泰坦技能动作，并检查是否已经模拟过
                        if (a.actionType == actionEnum.useTitanAbility)
                        {
                            // 生成泰坦技能键
                            string titanSkillKey = a.own.entitiyID + "-" + a.titanAbilityNO;
                            // 如果是第一个技能，记录
                            if (usedTitanSkills.Length == 0)
                            {
                                usedTitanSkills = titanSkillKey;
                            }
                            else
                            {
                                // 如果不是同一个技能，跳过
                                if (usedTitanSkills != titanSkillKey)
                                {
                                    continue; // 已使用其他技能
                                }
                            }
                        }
                        // 检查是否为地标使用动作
                        if (a.actionType == actionEnum.useLocation)
                        {
                            if (a.own != null)
                            {
                                // 检查是否已经使用过相同的地标
                                bool hasSameEntitiyID = p.playactions.Any(temp => temp.own != null && temp.own.entitiyID == a.own.entitiyID);
                                if (hasSameEntitiyID)
                                {
                                    continue; //当前地标已经模拟使用
                                }
                            }
                        }
                        // 创建游戏状态副本
                        Playfield pf = new Playfield(p);
                        // 执行动作
                        pf.doAction(a);
                        // 如果执行后我方英雄生命值大于0且惩罚值小于500，添加到下一层游戏状态
                        if (pf.ownHero.Hp > 0 && pf.evaluatePenality < 500) p.nextPlayfields.Add(pf);
                    }
                }

                // 如果是斩杀检查
                if (this.isLethalCheck)
                {
                    // 如果可以斩杀
                    if (berserk > 0) // 可以斩杀
                    {
                        // 结束回合
                        p.endTurn();  //结束回合，接下来模拟敌方下一回合的操作
                        // 如果敌方英雄生命值大于0
                        if (p.enemyHero.Hp > 0)
                        {
                            // 初始化是否需要敌方回合模拟的标志
                            bool needETS = true;
                            // 如果对面没有嘲讽且我方随从全部可以攻击则不模拟
                            if (p.anzEnemyTaunt < 1) foreach (Minion m in p.ownMinions) { if (m.Ready) { needETS = false; break; } }
                            else
                            {
                                // 如果我方没有嘲讽且我方随从全部可以攻击则不模拟
                                if (p.anzOwnTaunt < 1) foreach (Minion m in p.ownMinions) { if (m.Ready) { needETS = false; break; } }
                            }
                            // 如果需要，模拟敌方下一回合的操作
                            if (needETS) Ai.Instance.enemyTurnSim[threadnumber].simulateEnemysTurn(p, this.simulateSecondTurn, playaround, false, playaroundprob, playaroundprob2);
                        }
                    }

                    // 标记游戏状态为完成
                    p.complete = true;

                }
                else
                {
                    // 结束回合
                    p.endTurn();
                    // 如果敌方英雄生命值大于0
                    if (p.enemyHero.Hp > 0)
                    {
                        // 模拟敌方下一回合的操作
                        Ai.Instance.enemyTurnSim[threadnumber].simulateEnemysTurn(p, this.simulateSecondTurn, playaround, false, playaroundprob, playaroundprob2);
                        // 如果游戏状态价值过低
                        if (p.value <= -10000)
                        {
                            // 检查是否有战吼相关的牌
                            bool secondChance = false;
                            foreach (Action a in p.playactions)
                            {
                                if (a.actionType == actionEnum.playcard)
                                {
                                    // 检查是否出了战吼相关的牌
                                    if (pen.cardDrawBattleCryDatabase.ContainsKey(a.card.card.nameEN)) secondChance = true;
                                }
                            }
                            // 如果有战吼相关的牌，增加价值
                            if (secondChance) p.value += 1500;
                        }
                    }
                    // 标记游戏状态为完成
                    p.complete = true;
                }
            }
        }

        /// <summary>
        /// 执行两回合模拟
        /// 模拟接下来两个回合的游戏状态，以获取更长远的最优决策
        /// </summary>
        public void doDirtyTwoTurnsim()
        {
            // 如果两回合模拟数量为0，直接返回
            if (this.dirtyTwoTurnSim == 0) return;
            // 清空可能的移动列表
            this.posmoves.Clear();

            // 根据是否打印决定单线程或多线程执行
            if (print)
                // 单线程执行两回合模拟线程
                doDirtyTwoTurnsimThread(twoturnfields, 0, twoturnfields.Count);
            else
            {
                // 多线程执行两回合模拟线程
                Parallel.ForEach(Partitioner.Create(0, this.twoturnfields.Count),
                          range =>
                          {
                              // 为每个范围执行两回合模拟
                              doDirtyTwoTurnsimThread(twoturnfields, range.Item1, range.Item2);
                          });
            }
            // 将两回合牌面列表添加到可能的移动列表
            this.posmoves.AddRange(this.twoturnfields);
        }

        /// <summary>
        /// 执行两回合模拟线程
        /// 为两回合模拟的每个游戏状态执行详细的模拟计算
        /// </summary>
        /// <param name="source">游戏状态列表</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="endIndex">结束索引</param>
        public void doDirtyTwoTurnsimThread(List<Playfield> source, int startIndex, int endIndex)
        {
            int threadnumber = Ai.Instance.maxNumberOfThreads - 2;
            if (endIndex < source.Count) threadnumber = startIndex / (endIndex - startIndex);
            //set maxwide of enemyturnsimulator's to second step (this value is higher than the maxwide in first step) 
            Ai.Instance.enemyTurnSim[threadnumber].setMaxwide(false);

            for (int i = startIndex; i < endIndex; i++)
            {
                Playfield p = source[i];
                if (p.guessingHeroHP >= 1)
                {
                    p.doDirtyTts = p.value;
                    p.complete = false;
                    p.value = int.MinValue;
                    p.bestEnemyPlay = null;
                    Ai.Instance.enemyTurnSim[threadnumber].simulateEnemysTurn(p, true, playaround, false, this.playaroundprob, this.playaroundprob2);
                }
                else
                {
                    //p.value = -10000;
                }
                botBase.getPlayfieldValue(p);
            }
        }

        /// <summary>
        /// 剪枝和排序可能的游戏状态
        /// 去除重复的游戏状态，保留最优的游戏状态，以优化搜索过程
        /// </summary>
        /// <param name="isLethalCheck">是否进行斩杀检查</param>
        public void cuttingposibilities(bool isLethalCheck)
        {
            // 创建临时游戏状态列表
            List<Playfield> temp = new List<Playfield>();
            // 创建临时字典，用于去重
            Dictionary<Int64, Playfield> tempDict = new Dictionary<Int64, Playfield>();
            try
            {
                // 计算每个游戏状态的价值
                posmoves.ForEach(p => botBase.getPlayfieldValue(p));
                // 按价值降序排序，保留最佳状态
                posmoves.Sort((a, b) => b.value.CompareTo(a.value));//want to keep the best
            }
            catch (Exception ex) // Todo:待fix，不应该有这个异常，是处理了奥数、疯狂科学家牌序后才有的
            {
                // 记录异常信息
                Helpfunctions.Instance.logg("cuttingposibilities异常:" + ex.Message);
            }
            //useComparison = false;// 暂时调试用，打出所有重复的牌面，重点调试，用于寻找为啥不是相同牌面里的最佳牌序
            if (this.useComparison)
            {
                // 初始化索引和最大值
                int i = 0;
                int max = Math.Min(posmoves.Count, this.maxwide);

                Playfield p = null;
                // 遍历游戏状态
                for (i = 0; i < max; i++)
                {
                    // 获取当前游戏状态
                    p = posmoves[i];
                    // 计算游戏状态的哈希值
                    Int64 hash = p.GetPHash();
                    // 保存哈希值到游戏状态
                    p.hashcode = hash;
                    // 如果字典中不存在该哈希值，添加
                    if (!tempDict.ContainsKey(hash)) tempDict.Add(hash, p);
                    else
                    {
                        // 计算当前游戏状态的价值
                        float pvalue = botBase.getPlayfieldValue(p);
                        // 计算字典中对应哈希值的游戏状态的价值
                        float tempv = botBase.getPlayfieldValue(tempDict[hash]);
                        // 如果当前游戏状态的价值更高，替换
                        if (pvalue > tempv) //如果场面一样，取得分高的
                        {
                            tempDict[hash] = p;
                        }
                    }
                }
                // 将字典中的游戏状态添加到临时列表
                foreach (KeyValuePair<Int64, Playfield> d in tempDict)
                {
                    temp.Add(d.Value);
                }
            }
            else
            {
                // 如果不使用比较，直接添加所有游戏状态
                temp.AddRange(posmoves);
            }
            // 清空可能的移动列表
            posmoves.Clear();
            // 将临时列表中的游戏状态添加到可能的移动列表
            posmoves.AddRange(temp);




            // 处理两回合牌面列表
            if (this.dirtyTwoTurnSim == 0 || isLethalCheck) return;
            // 清空临时字典和列表
            tempDict.Clear();
            temp.Clear();
            // 如果最佳价值大于等于10000（表示可以斩杀），直接返回
            if (bestoldval >= 10000) return;
            // 将两回合牌面列表中的游戏状态添加到字典
            foreach (Playfield p in twoturnfields) tempDict.Add(p.hashcode, p);
            // 按价值降序排序可能的移动列表
            posmoves.Sort((a, b) => botBase.getPlayfieldValue(b).CompareTo(botBase.getPlayfieldValue(a)));

            // 计算两回合模拟的最大数量
            int maxTts = Math.Min(posmoves.Count, this.dirtyTwoTurnSim);
            // 遍历可能的移动列表
            for (int i = 0; i < maxTts; i++)
            {
                // 如果字典中不存在该哈希值，添加到临时列表
                if (!tempDict.ContainsKey(posmoves[i].hashcode)) temp.Add(posmoves[i]);
            }
            // 按价值降序排序两回合牌面列表
            twoturnfields.Sort((a, b) => botBase.getPlayfieldValue(b).CompareTo(botBase.getPlayfieldValue(a)));
            // 将两回合牌面列表中的前几个元素添加到临时列表
            temp.AddRange(twoturnfields.GetRange(0, Math.Min(this.dirtyTwoTurnSim, twoturnfields.Count)));
            // 清空两回合牌面列表
            twoturnfields.Clear();
            // 将临时列表中的游戏状态添加到两回合牌面列表
            twoturnfields.AddRange(temp);



        }

        /// <summary>
        /// 裁剪攻击目标列表
        /// 去除重复的攻击目标，优化攻击决策过程
        /// </summary>
        /// <param name="oldlist">原始目标列表</param>
        /// <param name="p">游戏状态</param>
        /// <param name="own">是否为我方</param>
        /// <returns>裁剪后的目标列表</returns>
        public List<targett> cutAttackTargets(List<targett> oldlist, Playfield p, bool own)
        {
            List<targett> retvalues = new List<targett>();
            List<Minion> addedmins = new List<Minion>(8);

            bool priomins = false;
            List<targett> retvaluesPrio = new List<targett>();
            foreach (targett t in oldlist)
            {
                if ((own && t.target == 200) || (!own && t.target == 100))
                {
                    retvalues.Add(t);
                    continue;
                }
                if ((own && t.target >= 10 && t.target <= 19) || (!own && t.target >= 0 && t.target <= 9))
                {
                    Minion m = null;
                    if (own) m = p.enemyMinions[t.target - 10];
                    if (!own) m = p.ownMinions[t.target];


                    bool goingtoadd = true;
                    List<Minion> temp = new List<Minion>(addedmins);
                    bool isSpecial = m.handcard.card.isSpecialMinion;
                    foreach (Minion mnn in temp)
                    {
                        // special minions are allowed to attack in silended and unsilenced state!
                        //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                        bool otherisSpecial = mnn.handcard.card.isSpecialMinion;

                        if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                        {
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous && mnn.lifesteal == m.lifesteal) goingtoadd = false;
                            continue;
                        }

                        if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                        {
                            if (m.name != mnn.name) // different name -> take it
                            {
                                continue;
                            }

                            // 同名随从不重复添加
                            // same name -> test whether they are equal
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous && mnn.lifesteal == m.lifesteal && mnn.Spellburst == m.Spellburst) goingtoadd = false;
                            continue;
                        }

                    }

                    if (goingtoadd)
                    {
                        addedmins.Add(m);
                        retvalues.Add(t);
                        //help.logg(m.name + " " + m.id +" is added to targetlist");
                    }
                    else
                    {
                        //help.logg(m.name + " is not needed to attack");
                        continue;
                    }

                }
            }
            //help.logg("end targetcutting");
            if (priomins) return retvaluesPrio;

            return retvalues;
        }

        /// <summary>
        /// 打印可能的移动列表
        /// 输出当前可能的游戏状态，用于调试和分析
        /// </summary>
        public void printPosmoves()
        {
            int i = 0;
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
                i++;
                if (i >= 200) break;
            }
        }

    }

}