using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

#if APPLICATION_MODE
    using RoutineHelper;
#endif

namespace HREngine.Bots
{
    /// <summary>
    /// AI类，负责处理游戏中的决策逻辑
    /// 包括搜索最优行动、模拟游戏状态、执行AI决策等功能
    /// </summary>
    public class Ai
    {
        /// <summary>
        /// 最大计算量
        /// </summary>
        public int maxdeep = 12;
        /// <summary>
        /// 最大考虑步数
        /// </summary>
        public int maxwide = 3000;
        /// <summary>
        /// 每步最大保留场面数
        /// </summary>
        public int maxCal = 60;
        //public int playaroundprob = 40;
        /// <summary>
        /// 防奥秘概率2
        /// </summary>
        public int playaroundprob2 = 80;
        /// <summary>
        /// 最大线程数
        /// </summary>
        public int maxNumberOfThreads = 100; //don't change manually, because it changes automatically

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
        private bool useLethalCheck = false;  // 原本是true，现在我们应该能做到正常计算，斩杀的场景下得分最高，所以无需计算多次
        /// <summary>
        /// 是否使用比较
        /// </summary>
        private bool useComparison = true;
        /// <summary>
        /// 最优场面
        /// </summary>
        public Playfield bestplay = new Playfield();

        /// <summary>
        /// 斩杀缺失伤害
        /// </summary>
        public int lethalMissing = 30; //RR

        /// <summary>
        /// 主回合模拟器
        /// </summary>
        public MiniSimulator mainTurnSimulator;
        /// <summary>
        /// 敌方回合模拟器列表
        /// </summary>
        public List<EnemyTurnSimulator> enemyTurnSim = new List<EnemyTurnSimulator>();
        /// <summary>
        /// 下一回合模拟器列表
        /// </summary>
        public List<MiniSimulatorNextTurn> nextTurnSimulator = new List<MiniSimulatorNextTurn>();
        /// <summary>
        /// 敌方第二回合模拟器列表
        /// </summary>
        public List<EnemyTurnSimulator> enemySecondTurnSim = new List<EnemyTurnSimulator>();

        /// <summary>
        /// 当前计算的牌面
        /// </summary>
        public string currentCalculatedBoard = "1";

        /// <summary>
        /// 惩罚管理器实例
        /// </summary>
        PenalityManager penman = PenalityManager.Instance;
        /// <summary>
        /// 设置实例
        /// </summary>
        Settings settings = Settings.Instance;

        /// <summary>
        /// 可能的移动列表
        /// </summary>
        List<Playfield> posmoves = new List<Playfield>(7000);

        /// <summary>
        /// 进程实例
        /// </summary>
        Hrtprozis hp = Hrtprozis.Instance;
        /// <summary>
        /// 手牌管理器实例
        /// </summary>
        Handmanager hm = Handmanager.Instance;
        /// <summary>
        /// 帮助函数实例
        /// </summary>
        Helpfunctions help = Helpfunctions.Instance;

        /// <summary>
        /// 最优移动
        /// </summary>
        public Action bestmove = null;
        /// <summary>
        /// 最优移动值
        /// </summary>
        public float bestmoveValue = 0;
        /// <summary>
        /// 下一移动猜测
        /// </summary>
        public Playfield nextMoveGuess = new Playfield();
        /// <summary>
        /// 机器人基础行为
        /// </summary>
        public Behavior botBase = null;

        /// <summary>
        /// 最优行动列表
        /// </summary>
        public List<Action> bestActions = new List<Action>();

        /// <summary>
        /// 是否进行第二回合模拟
        /// </summary>
        public bool secondturnsim = false;
        // public int secondTurnAmount = 256;
        /// <summary>
        /// 是否防奥秘
        /// </summary>
        public bool playaround = false;

        /// <summary>
        /// 单例实例
        /// </summary>
        private static Ai instance;

        /// <summary>
        /// 获取Ai的单例实例
        /// </summary>
        public static Ai Instance
        {
            get
            {
                return instance ?? (instance = new Ai());
            }
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private Ai()
        {
            this.nextMoveGuess = new Playfield { mana = -100 };

            this.mainTurnSimulator = new MiniSimulator(maxdeep, maxwide, 0); // 0 for unlimited 对每层搜索数量没有限制
            this.mainTurnSimulator.setPrintingstuff(true);

            for (int i = 0; i < maxNumberOfThreads; i++)
            {
                this.nextTurnSimulator.Add(new MiniSimulatorNextTurn());
                this.enemyTurnSim.Add(new EnemyTurnSimulator());
                this.enemySecondTurnSim.Add(new EnemyTurnSimulator());

                this.nextTurnSimulator[i].thread = i;
                this.enemyTurnSim[i].thread = i;
                this.enemySecondTurnSim[i].thread = i;
            }
        }

        /// <summary>
        /// 设置最大搜索宽度
        /// </summary>
        /// <param name="mw">最大宽度</param>
        public void setMaxWide(int mw)
        {
            this.maxwide = mw;
            if (maxwide <= 0) this.maxwide = 3000;
            if (maxwide <= 100) this.maxwide = 100;
            this.mainTurnSimulator.updateParams(maxdeep, maxwide, 0);
        }

        /// <summary>
        /// 设置最大搜索深度
        /// </summary>
        /// <param name="mw">最大深度</param>
        public void setMaxDeep(int mw)
        {
            this.maxdeep = mw;
            if (maxdeep <= 0) this.maxdeep = 12;
            if (maxdeep <= 2) this.maxdeep = 2;
            this.mainTurnSimulator.updateParams(maxdeep, maxwide, 0);
        }

        /// <summary>
        /// 设置最大计算时间
        /// </summary>
        /// <param name="mw">最大计算时间</param>
        public void setMaxCal(int mw)
        {
            this.maxCal = mw;
            if (maxCal <= 0) this.maxCal = 60;
            if (maxCal <= 1) this.maxCal = 1;
        }

        /// <summary>
        /// 设置两回合模拟
        /// </summary>
        /// <param name="stts">是否启用</param>
        /// <param name="amount">模拟数量</param>
        public void setTwoTurnSimulation(bool EnableTwoTurnSimulation, int amount)
        {
            this.mainTurnSimulator.setSecondTurnSimu(EnableTwoTurnSimulation, amount);
            this.secondturnsim = EnableTwoTurnSimulation;
            settings.simulateEnemysTurn = EnableTwoTurnSimulation;
            settings.secondTurnAmount = amount;
        }
        /// <summary>
        /// 更新两回合测试
        /// </summary>
        public void updateTwoTurnSim()
        {
            this.mainTurnSimulator.setSecondTurnSimu(settings.simulateEnemysTurn, settings.secondTurnAmount);
        }

        /// <summary>
        /// 设置防奥秘
        /// </summary>
        public void setPlayAround()
        {
            this.mainTurnSimulator.setPlayAround(settings.playaround, settings.playaroundprob, settings.playaroundprob2);
        }

        /// <summary>
        /// 执行所有可能的移动
        /// 计算最优行动并更新相关状态
        /// test：调试的时候为true,不过发现下面没用:(  
        /// isLethalCheck：斩杀判定，当为true时，程序接下来只会模拟一些有可能造成伤害的操作，来判定是否会斩杀，如果不能造成斩杀，
        /// 则再次进行isLethalCheck = false的运算。如果看到兄弟错斩的情况，就可以从这里入手来找解决的办法，具体解决办法在PenalityManager.cs处说明
        /// </summary>
        /// <param name="test">是否为测试模式</param>
        /// <param name="isLethalCheck">是否进行斩杀检验</param>
        private void doAllMoves(bool test, bool isLethalCheck)
        {
            // 如果是斩杀检验，打印开始信息
            if (isLethalCheck)
                help.logg("斩杀检验开始:");
            // 设置敌方回合模拟器的最大搜索宽度
            foreach (EnemyTurnSimulator ets in enemyTurnSim)
            {
                ets.setMaxwide(true);
            }
            foreach (EnemyTurnSimulator ets in enemySecondTurnSim)
            {
                ets.setMaxwide(true);
            }

            // 设置当前游戏状态的斩杀检验标志
            this.posmoves[0].isLethalCheck = isLethalCheck;
            // 执行核心计算，模拟所有可能的移动
            this.mainTurnSimulator.doallmoves(this.posmoves[0]); //核心计算

            // 获取最优场面和得分
            bestplay = this.mainTurnSimulator.bestboard;  // 找到最优场面
            float bestval = this.mainTurnSimulator.bestmoveValue; //最优场面得分

            // 开启日志记录
            help.loggonoff(true);
            // 打印分隔线
            help.logg("-------------------------------------");
            // 打印规则权重（如果不为0）
            if (bestplay.ruleWeight != 0) help.logg("ruleWeight " + bestplay.ruleWeight * -1);
            // 如果设置了打印规则，打印使用的规则
            if (settings.printRules > 0)
            {
                String[] rulesStr = bestplay.rulesUsed.Split('@');
                foreach (string rs in rulesStr)
                {
                    if (rs == "") continue;
                    help.logg("rule: " + rs);
                }
            }
            // 打印最优场面的得分
            help.logg("value of best board " + bestval);

#if APPLICATION_MODE
            // 在UI中更新最优得分
            (Application.Current.MainWindow as MainWindow).BestScore = bestval;
#endif

            // 清空最优行动列表，清除上次回合的最优操作
            this.bestActions.Clear();  // 初始化，清除上次回合的最优操作  这是List<Action>
            // 重置最优移动
            this.bestmove = null;

            // 创建动作归一化器，用于重排操作顺序
            ActionNormalizer an = new ActionNormalizer(); //对操作重排使得其先打出AOE伤害，在_setting.txt中设定，默认关闭，实际使用效果并不好
            // 如果设置了调整动作，执行动作重排
            if (settings.adjustActions > 0) an.adjustActions(bestplay, isLethalCheck);
            // 打印最优行动列表开始
            Helpfunctions.Instance.logg("bestplay.playactions ################################# START");
            // 遍历最优行动，添加到最优行动列表并打印
            foreach (Action a in bestplay.playactions)
            {
                this.bestActions.Add(new Action(a));
                a.print();
            }
            // 打印最优行动列表结束
            Helpfunctions.Instance.logg("bestplay.playactions ################################# END");

#if APPLICATION_MODE
            // 在UI中更新最优行动列表
            (Application.Current.MainWindow as MainWindow).UpdateBestActionList(bestplay.playactions);
#endif

            // 如果有最优行动，设置第一个为当前最优移动，并从列表中移除
            if (this.bestActions.Count >= 1)
            {
                this.bestmove = this.bestActions[0];
                this.bestActions.RemoveAt(0);
            }
            // 更新最优移动值
            this.bestmoveValue = bestval;

            // 如果最优移动不为空且不是结束回合，保存猜测的移动
            if (bestmove != null && bestmove.actionType != actionEnum.endturn) // save the guessed move, so we doesnt need to recalc!
            {
                // 创建新的游戏状态
                this.nextMoveGuess = new Playfield();
                // 执行最优移动
                this.nextMoveGuess.doAction(bestmove);
            }
            else
            {
                // 设置水晶为-100，表示结束回合
                nextMoveGuess.mana = -100;
            }

            // 如果是斩杀检验，计算并打印还差的伤害
            if (isLethalCheck)
            {
                this.lethalMissing = bestplay.enemyHero.armor + bestplay.enemyHero.Hp;//RR
                help.logg("斩杀检验结束：还差伤害:" + this.lethalMissing);
            }
        }

        /// <summary>
        /// 执行下一个计算好的移动
        /// 从最优行动列表中获取下一个行动并执行
        /// </summary>
        public void doNextCalcedMove()
        {
            //help.logg("noRecalcNeeded!!!-----------------------------------");
            // help.logg("无需要重新计算!!!-----------------------------------");

            this.bestmove = null;
            if (this.bestActions.Count >= 1)
            {
                //下一步操作
                this.bestmove = this.bestActions[0];
                this.bestActions.RemoveAt(0);
            }
            if (this.nextMoveGuess == null) this.nextMoveGuess = new Playfield();
            else
            {
                //更新克苏恩
                //Silverfish.Instance.updateCThunInfo(nextMoveGuess.anzOgOwnCThunAngrBonus, nextMoveGuess.anzOgOwnCThunHpBonus, nextMoveGuess.anzOgOwnCThunTaunt);
            }

            if (bestmove != null && bestmove.actionType != actionEnum.endturn)  // save the guessed move, so we doesnt need to recalc!
            {
                //this.nextMoveGuess = new Playfield();
                Helpfunctions.Instance.logg("nmgsim-");
                try
                {
                    //做下一步操作测试
                    this.nextMoveGuess.doAction(bestmove);
                    this.bestmove = this.nextMoveGuess.playactions[this.nextMoveGuess.playactions.Count - 1];
                }
                catch (Exception ex)
                {
                    Helpfunctions.Instance.logg("Message ---");
                    Helpfunctions.Instance.logg("Message ---" + ex.Message);
                    Helpfunctions.Instance.logg("Source ---" + ex.Source);
                    Helpfunctions.Instance.logg("StackTrace ---" + ex.StackTrace);
                    Helpfunctions.Instance.logg("TargetSite ---\n{0}" + ex.TargetSite);

                }
                Helpfunctions.Instance.logg("nmgsime-");

            }
            else
            {
                //Helpfunctions.Instance.logg("nd trn");
                nextMoveGuess.mana = -100;
                //int twilightelderBonus = 0;
                /*foreach (Minion m in this.nextMoveGuess.ownMinions)
                {
                    //暮光尊者
                    if (m.name == CardDB.cardIDEnum.OG_286 && !m.silenced) twilightelderBonus++;
                }
                if (twilightelderBonus > 0)
                {
                    Silverfish.Instance.updateCThunInfo(nextMoveGuess.anzOgOwnCThunAngrBonus + twilightelderBonus, nextMoveGuess.anzOgOwnCThunHpBonus + twilightelderBonus, nextMoveGuess.anzOgOwnCThunTaunt);
                }*/
            }

        }

        /// <summary>
        /// 执行智能决策
        /// 检查是否需要重新计算，执行斩杀检验，计算最优行动
        /// </summary>
        /// <param name="bbase">机器人基础行为</param>
        public void dosomethingclever(Behavior bbase)
        {
            // 保存机器人基础行为
            this.botBase = bbase;
            // 更新游戏中各实体的位置信息
            hp.updatePositions();
            
            // 清空可能的移动列表
            posmoves.Clear();
            // 创建新的游戏状态并添加到列表中
            posmoves.Add(new Playfield());

            // 关闭日志记录功能
            help.loggonoff(false);
            // 记录重新计算检查的开始
            help.logg("recalc-check###########");
            // 检查是否不需要重新计算：
            // 1. dontRecalc 为 true
            // 2. 当前游戏状态与下一个移动猜测相同
            if (this.dontRecalc && posmoves[0].isEqual(this.nextMoveGuess, false))
            {
                // 执行下一个计算好的移动
                doNextCalcedMove();
            }
            else
            {
                // 初始化最优移动值为一个很小的值
                bestmoveValue = -1000000;
                // 记录当前时间，用于计算执行时间
                DateTime strt = DateTime.Now;
                // 检查是否启用斩杀检验
                if (useLethalCheck)  //如果斩杀检验，默认不做斩杀检验
                {
                    // 重新记录当前时间
                    strt = DateTime.Now;
                    // 执行所有可能的移动，启用斩杀检验
                    doAllMoves(false, true);
                    // 记录斩杀检验的计算时间
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);
                }

                // 检查是否没有达到斩杀条件（bestmoveValue < 10000）
                if (bestmoveValue < 10000)  //如果没有斩杀，再试试
                {
                    // 清空可能的移动列表
                    posmoves.Clear();
                    // 创建新的游戏状态并添加到列表中
                    posmoves.Add(new Playfield());
                    // 重新记录当前时间
                    strt = DateTime.Now;
                    // 执行所有可能的移动，不启用斩杀检验
                    doAllMoves(false, false);
                    // 记录正常出牌的计算时间
                    help.logg("calculated " + (DateTime.Now - strt).TotalSeconds);

                }
            }
        }



        /// <summary>
        /// 自动测试函数
        /// 用于测试AI决策，返回每一步的计算时间
        /// </summary>
        /// <param name="printstuff">是否打印信息</param>
        /// <param name="data">测试数据</param>
        /// <param name="mode">测试模式：0 先斩杀检验，再正常出牌 1：斩杀检验 2:正常</param>
        /// <returns>每一步的计算时间列表</returns>
        public List<double> autoTester(bool printstuff, string data = "", int mode = 0) //测试用函数  返回值是每一步的计算时间
                                                                                        //mode: 0 先斩杀检验，再正常出牌 1：斩杀检验 2:正常
        {
            // 初始化返回值列表，用于存储计算时间
            List<double> retval = new List<double>();
            // 初始化计算时间变量
            double calcTime = 0;
            // 创建BoardTester对象，用于测试牌面
            BoardTester bt = new BoardTester(data);
            // 如果数据读取失败，返回空列表
            if (!bt.datareaded) return retval;
            // 打印初始局面信息
            help.logg("打印初始局面：");
            //hp.printHero();
            //hp.printOwnMinions();
            //hp.printEnemyMinions();
            //hm.printcards();
            // 清空可能的移动列表
            posmoves.Clear();
            // 创建新的游戏状态
            Playfield pMain = new Playfield();  // 为什么new出来的有牌面信息，因为public Hrtprozis prozis = Hrtprozis.Instance存储了对局信息
            // 设置回合牌组
            pMain.prozis.turnDeck = bt.od;

            // 打印牌面信息
            pMain.printBoard();
            // 设置是否打印信息
            pMain.print = printstuff;
            // 将游戏状态添加到可能的移动列表
            posmoves.Add(pMain);
            //pMain.printBoard();
            //help.logg("ownminionscount " + posmoves[0].ownMinions.Count);
            //help.logg("owncardscount " + posmoves[0].owncards.Count);

            //foreach (var item in this.posmoves[0].owncards)
            //{
            //    help.logg("card " + item.card.name + " is playable :" + item.canplayCard(posmoves[0], true) + " cost/mana: " + item.manacost + "/" + posmoves[0].mana);
            //}
            //help.logg("ability " + posmoves[0].ownHeroAblility.card.name + " is playable :" + posmoves[0].ownHeroAblility.card.canplayCard(posmoves[0], 2, true) + " cost/mana: " + posmoves[0].ownHeroAblility.card.getManaCost(posmoves[0], 2) + "/" + posmoves[0].mana);

            // 记录当前时间，用于计算执行时间
            DateTime strt = DateTime.Now;

            //默认不再做斩杀检验，斩杀检查,所有动作都是为了制造伤害，看能否致死
            //if (mode == 0 || mode == 1)
            //{
            //    doallmoves(false, true);
            //    calcTime = (DateTime.Now - strt).TotalSeconds;
            //    help.logg("calculated " + calcTime);
            //    retval.Add(calcTime);
            //}

            // 这里要补充一下，如果兄弟判定对面血量为负数(已斩杀)，则会将场面评分提升到10000以上。
            // 而这里的bestmoveValue > 5000，则说明场面已经大概可以斩杀了。
            // 但是为什么是五千不是一万？
            // 这个问题的原因在于兄弟是会对当前回合和我方下一回合进行模拟的，而默认的权重分配是0.5
            // 如果这个回合斩杀了，兄弟可能就不会对下个回合进行模拟，所以如果这个回合评分为10000 * 0.5 + 0 * 0.5 = 5000
            // 以上只是个人猜测，有少量的代码可证明，知道原因的可以在下面评论哦
            // FIXME 暂时不考虑斩杀，继续进行计算，获得最高的评分后行动！
            //if (bestmoveValue >= 5000)
            //{
            //    //可以斩杀
            //}
            //else //如果不足以斩杀

            // 检查测试模式，0或2表示需要进行正常出牌计算
            if (mode == 0 || mode == 2)
            {
                // 清空可能的移动列表
                posmoves.Clear();
                // 创建新的游戏状态
                pMain = new Playfield();
                // 设置是否打印信息
                pMain.print = printstuff;
                // 将游戏状态添加到可能的移动列表
                posmoves.Add(pMain);
                // 重新记录当前时间
                strt = DateTime.Now;
                // 执行所有可能的移动，不启用斩杀检验
                doAllMoves(false, false);
                // 计算执行时间
                calcTime = (DateTime.Now - strt).TotalSeconds;
                // 记录计算时间
                help.logg("calculated " + calcTime);
                // 将计算时间添加到返回列表
                retval.Add(calcTime);
            }

            // 如果需要打印信息
            if (printstuff)
            {
                //this.mainTurnSimulator.printPosmoves();
                // 模拟全程并打印详细信息
                simmulateWholeTurn(bt);
                // 记录计算时间
                help.logg("calculated " + calcTime);
            }

            return retval;
        }

        /// <summary>
        /// 模拟全程,用于测试
        /// 模拟最优行动并打印详细的游戏状态信息
        /// </summary>
        /// <param name="bt">牌面测试器</param>
        public void simmulateWholeTurn(BoardTester bt)  // 这里影响输出.result文件
        {
#if APPLICATION_MODE
            // 清除UI中的行动列表
            (Application.Current.MainWindow as MainWindow).playfieldActionList.Clear();
#endif

            // 打印结果设置
            printUtils.printResult = true;
            // 打印分隔线
            help.logg("########################################################################################################");
            // 输出初始场面
            Playfield p = new Playfield();
            // 设置回合牌组
            p.prozis.turnDeck = bt.od;

#if APPLICATION_MODE
            // 在UI中添加回合开始的场面
            (Application.Current.MainWindow as MainWindow).playfieldActionList.Add("回合开始", new Playfield(p));
#endif

            // 初始化字符串构建器，用于构建各种信息
            StringBuilder normalInfo = new StringBuilder("", 100);
            StringBuilder enemyVal = new StringBuilder("[敌方场面] ", 20);
            StringBuilder myVal = new StringBuilder("[我方场面] ", 20);
            StringBuilder handCard = new StringBuilder("[我方手牌] ", 20);

            // 构建基本信息：水晶、英雄状态、任务进度
            normalInfo.AppendFormat("水晶： {0} / {1}", p.mana, p.ownMaxMana);
            normalInfo.AppendFormat(" [我方英雄] {0}（生命: {1} + {2} 奥秘数: {3} )", p.ownHeroName, p.ownHero.Hp, p.ownHero.armor, p.ownSecretsIDList.Count);
            normalInfo.AppendFormat(" [敌方英雄] {0}（生命: {1} + {2} 奥秘数: {3}{4})", p.enemyHeroName, p.enemyHero.Hp, p.enemyHero.armor, p.enemySecretCount, (p.enemyHero.immune ? " 免疫" : ""));
            normalInfo.AppendFormat(" [任务] quests: {0} {1} {2}", p.ownQuest.Id, p.ownQuest.questProgress, p.ownQuest.maxProgress);
            normalInfo.AppendFormat(" {0} {1} {2}", p.enemyQuest.Id, p.enemyQuest.questProgress, p.enemyQuest.maxProgress);

            // 构建敌方场面信息
            foreach (Minion m in p.enemyMinions)
            {
                enemyVal.AppendFormat("{0} ({1}/{2}) ", m.handcard.card.nameCN, m.Angr, m.Hp);
                enemyVal.Append(m.frozen ? "[冻结]" : "");
                enemyVal.Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
                enemyVal.Append(m.windfury ? "[风怒]" : "");
                enemyVal.Append(m.megaWindfury ? "[超级风怒]" : "");
                enemyVal.Append(m.taunt ? "[嘲讽]" : "");
                enemyVal.Append(m.rush > 0 ? "[突袭]" : "");
                enemyVal.Append(m.divineshild ? "[圣盾]" : "");
                enemyVal.Append(m.lifesteal ? "[吸血]" : "");
                enemyVal.Append(m.poisonous ? "[剧毒]" : "");
                enemyVal.Append(m.reborn ? "[复生]" : "");
                enemyVal.Append(m.stealth ? "[潜行]" : "");
                enemyVal.Append(m.immune ? "[免疫]" : "");
            }
            // 构建我方场面信息
            foreach (Minion m in p.ownMinions)
            {
                myVal.AppendFormat("{0} ({1}/{2}) ", m.handcard.card.nameCN, m.Angr, m.Hp);
                myVal.Append(m.frozen ? "[冻结]" : "");
                myVal.Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
                myVal.Append(m.windfury ? "[风怒]" : "");
                myVal.Append(m.megaWindfury ? "[超级风怒]" : "");
                myVal.Append(m.taunt ? "[嘲讽]" : "");
                myVal.Append(m.rush > 0 ? "[突袭]" : "");
                myVal.Append(m.divineshild ? "[圣盾]" : "");
                myVal.Append(m.lifesteal ? "[吸血]" : "");
                myVal.Append(m.poisonous ? "[剧毒]" : "");
                myVal.Append(m.reborn ? "[复生]" : "");
                myVal.Append(m.stealth ? "[潜行]" : "");
                myVal.Append(m.immune ? "[免疫]" : "");
            }
            // 构建我方手牌信息
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                handCard.AppendFormat("{0}(费用：{1} ; {2} / {3} ) ", hc.card.nameCN, hc.manacost, (hc.addattack + hc.card.Attack), (hc.addHp + hc.card.Health));
            }

            // 打印初始场面信息
            help.logg(normalInfo.ToString() + (p.enemyGuessDeck == "" ? "" : "(猜测对手构筑为:" + p.enemyGuessDeck + " 套牌代码：" + Hrtprozis.Instance.enemyDeckCode + " 预计直伤： " + Hrtprozis.Instance.enemyDirectDmg + " 加上场攻一共 " + (Hrtprozis.Instance.enemyDirectDmg + p.calEnemyTotalAngr()) + " )"));
            help.logg(enemyVal.ToString());
            help.logg(myVal.ToString());
            help.logg(handCard.ToString());
            help.logg("########################################################################################################");
            help.logg("simulate best board，最终结果如下：");
            help.logg("########################################################################################################");
            //this.bestboard.printActions();

            // 创建临时游戏状态，用于模拟最优行动
            Playfield tempbestboard = new Playfield();
            // 初始化步骤计数器
            int step = 0;
            // 检查是否有最优移动且不是结束回合
            if (bestmove != null && bestmove.actionType != actionEnum.endturn)  // save the guessed move, so we doesnt need to recalc!
            {
                // 步骤加1
                step++;
                // 打印步骤信息
                help.logg("第" + step + "步:");
                // 打印最优移动
                bestmove.print(false);
                // 执行最优移动
                tempbestboard.doAction(bestmove);

#if APPLICATION_MODE
                // 在UI中添加当前步骤的场面
                (Application.Current.MainWindow as MainWindow).playfieldActionList.Add("第" + step + "步", new Playfield(tempbestboard));
#endif

                // 如果没有更多行动，打印结束回合
                if (this.bestActions.Count == 0)
                {
                    help.ErrorLog("end turn");
                }
            }
            else
            {
                // 设置临时游戏状态的水晶为-100，表示结束回合
                tempbestboard.mana = -100;
                // 打印结束回合
                help.ErrorLog("end turn");
            }

            // 遍历剩余的最优行动
            foreach (Action imove in this.bestActions)  // imove:每一步动作  bestmove + bestActions 才是整体，因为有个RemoveAt的操作
            {
                // 步骤加1
                step++;
                // 打印步骤信息
                help.logg("第" + step + "步:");
                // 检查行动是否有效且不是结束回合
                if (imove != null && imove.actionType != actionEnum.endturn)  // save the guessed move, so we doesnt need to recalc!
                {
                    // 打印行动
                    imove.print();
                    // 执行行动
                    tempbestboard.doAction(imove);
                }
                else
                {
                    // 设置临时游戏状态的水晶为-100，表示结束回合
                    tempbestboard.mana = -100;
                }
#if APPLICATION_MODE
                // 在UI中添加当前步骤的场面
                (Application.Current.MainWindow as MainWindow).playfieldActionList.Add("第" + step + "步", new Playfield(tempbestboard));
#endif
            }

#if APPLICATION_MODE
            // 更新UI中的行动列表
            (Application.Current.MainWindow as MainWindow).UpdatePlayfieldActionList();
#endif

            // 重新初始化字符串构建器，用于构建最终场面信息
            normalInfo = new StringBuilder("", 100);
            normalInfo.AppendFormat("水晶： {0} / {1}", p.mana, p.ownMaxMana);
            normalInfo.AppendFormat(" [我方英雄] {0}（生命: {1} + {2} 奥秘数: {3} )", p.ownHeroName, p.ownHero.Hp, p.ownHero.armor, p.ownSecretsIDList.Count);
            normalInfo.AppendFormat(" [敌方英雄] {0}（生命: {1} + {2} 奥秘数: {3}{4})", p.enemyHeroName, p.enemyHero.Hp, p.enemyHero.armor, p.enemySecretCount, (p.enemyHero.immune ? " 免疫" : ""));
            normalInfo.AppendFormat(" [任务] quests: {0} {1} {2}", p.ownQuest.Id, p.ownQuest.questProgress, p.ownQuest.maxProgress);
            normalInfo.AppendFormat(" {0} {1} {2}", p.enemyQuest.Id, p.enemyQuest.questProgress, p.enemyQuest.maxProgress);
            enemyVal = new StringBuilder("[敌方场面] ", 60);
            myVal = new StringBuilder("[我方场面] ", 60);
            handCard = new StringBuilder("[我方手牌] ", 60);

            // 构建最终敌方场面信息
            foreach (Minion m in tempbestboard.enemyMinions)
            {
                enemyVal.AppendFormat("{0} ({1}/{2}) ", m.handcard.card.nameCN, m.Angr, m.Hp);
                enemyVal.Append(m.frozen ? "[冻结]" : "");
                enemyVal.Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
                enemyVal.Append(m.windfury ? "[风怒]" : "");
                enemyVal.Append(m.megaWindfury ? "[超级风怒]" : "");
                enemyVal.Append(m.taunt ? "[嘲讽]" : "");
                enemyVal.Append(m.rush > 0 ? "[突袭]" : "");
                enemyVal.Append(m.divineshild ? "[圣盾]" : "");
                enemyVal.Append(m.lifesteal ? "[吸血]" : "");
                enemyVal.Append(m.poisonous ? "[剧毒]" : "");
                enemyVal.Append(m.reborn ? "[复生]" : "");
                enemyVal.Append(m.stealth ? "[潜行]" : "");
                enemyVal.Append(m.immune ? "[免疫]" : "");
            }
            // 构建最终我方场面信息
            foreach (Minion m in tempbestboard.ownMinions)
            {
                myVal.AppendFormat("{0} ({1}/{2}) ", m.handcard.card.nameCN, m.Angr, m.Hp);
                myVal.Append(m.frozen ? "[冻结]" : "");
                myVal.Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
                myVal.Append(m.windfury ? "[风怒]" : "");
                myVal.Append(m.megaWindfury ? "[超级风怒]" : "");
                myVal.Append(m.taunt ? "[嘲讽]" : "");
                myVal.Append(m.rush > 0 ? "[突袭]" : "");
                myVal.Append(m.divineshild ? "[圣盾]" : "");
                myVal.Append(m.lifesteal ? "[吸血]" : "");
                myVal.Append(m.poisonous ? "[剧毒]" : "");
                myVal.Append(m.reborn ? "[复生]" : "");
                myVal.Append(m.stealth ? "[潜行]" : "");
                myVal.Append(m.immune ? "[免疫]" : "");
            }
            // 构建最终我方手牌信息
            foreach (Handmanager.Handcard hc in tempbestboard.owncards)
            {
                handCard.AppendFormat("{0}(费用：{1} ； {2} / {3} ) ", hc.card.nameCN, hc.manacost, (hc.addattack + hc.card.Attack), (hc.addHp + hc.card.Health));
            }

            // 打印最终场面信息
            help.logg("########################################################################################################");
            help.logg("最终场面：");
            help.logg(normalInfo.ToString());
            help.logg(enemyVal.ToString());
            help.logg(myVal.ToString());
            help.logg(handCard.ToString());
            help.logg("########################################################################################################");

            // 打印结果设置
            printUtils.printResult = false;

            // 打印分隔线
            help.logg("----------------------------");
        }

        /// <summary>
        /// 模拟全程并打印
        /// 模拟最优行动并打印详细信息
        /// </summary>
        public void simmulateWholeTurnandPrint()
        {
            // 打印标题和分隔线
            help.ErrorLog("###################################");
            help.ErrorLog("what would silverfish do?---------");
            help.ErrorLog("###################################");
            // 如果检测到斩杀，打印斩杀信息
            if (this.bestmoveValue >= 10000) help.ErrorLog("DETECTED LETHAL ###################################");
            //this.bestboard.printActions();

            // 创建临时游戏状态，用于模拟最优行动
            Playfield tempbestboard = new Playfield();

            // 检查是否有最优移动且不是结束回合
            if (bestmove != null && bestmove.actionType != actionEnum.endturn)  // save the guessed move, so we doesnt need to recalc!
            {
                // 执行最优移动
                tempbestboard.doAction(bestmove);
                // 打印行动的详细信息（适合新手理解的格式）
                tempbestboard.printActionforDummies(tempbestboard.playactions[tempbestboard.playactions.Count - 1]);

                // 如果没有更多行动，打印结束回合
                if (this.bestActions.Count == 0)
                {
                    help.ErrorLog("end turn");
                }
            }
            else
            {
                // 设置临时游戏状态的水晶为-100，表示结束回合
                tempbestboard.mana = -100;
                // 打印结束回合
                help.ErrorLog("end turn");
            }

            // 遍历剩余的最优行动
            foreach (Action bestmovee in this.bestActions)
            {
                // 检查行动是否有效且不是结束回合
                if (bestmovee != null && bestmove.actionType != actionEnum.endturn)  // save the guessed move, so we doesnt need to recalc!
                {
                    // 执行行动
                    tempbestboard.doAction(bestmovee);
                    // 打印行动的详细信息（适合新手理解的格式）
                    tempbestboard.printActionforDummies(tempbestboard.playactions[tempbestboard.playactions.Count - 1]);
                }
                else
                {
                    // 设置临时游戏状态的水晶为-100，表示结束回合
                    tempbestboard.mana = -100;
                    // 打印结束回合
                    help.ErrorLog("end turn");
                }
            }
        }

        /// <summary>
        /// 更新实体ID
        /// 当实体ID发生变化时，更新相关对象中的实体ID
        /// </summary>
        /// <param name="old">旧实体ID</param>
        /// <param name="newone">新实体ID</param>
        public void updateEntitiy(int old, int newone)
        {
            // 记录实体ID更新信息
            Helpfunctions.Instance.logg("entityupdate! " + old + " to " + newone);
            // 检查下一个移动猜测是否存在
            if (this.nextMoveGuess != null)
            {
                // 更新我方随从的实体ID
                foreach (Minion m in this.nextMoveGuess.ownMinions)
                {
                    if (m.entitiyID == old) m.entitiyID = newone;
                }
                // 更新敌方随从的实体ID
                foreach (Minion m in this.nextMoveGuess.enemyMinions)
                {
                    if (m.entitiyID == old) m.entitiyID = newone;
                }
            }
            // 更新最优行动列表中行动的实体ID
            foreach (Action a in this.bestActions)
            {
                // 更新行动中的我方实体ID
                if (a.own != null && a.own.entitiyID == old) a.own.entitiyID = newone;
                // 更新行动中的目标实体ID
                if (a.target != null && a.target.entitiyID == old) a.target.entitiyID = newone;
                // 更新行动中的卡牌实体ID
                if (a.card != null && a.card.entity == old) a.card.entity = newone;
            }

        }

    }


}