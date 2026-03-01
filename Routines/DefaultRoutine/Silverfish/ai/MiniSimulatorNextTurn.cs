namespace HREngine.Bots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 下一回合模拟器
    /// 负责模拟计算下一回合的最佳行动方案
    /// </summary>
    public class MiniSimulatorNextTurn
    {
        //#####################################################################################################################
        //public int maxdeep = 6;      // 最大搜索深度
        //public int maxwide = 10;      // 每步最大保留场面数
        //public int totalboards = 50;  // 总计算场面数

        /// <summary>
        /// 线程ID，用于多线程计算时标识不同的模拟器实例
        /// </summary>
        public int thread = 0;

        /// <summary>
        /// 是否使用惩罚管理器评估动作
        /// </summary>
        private bool usePenalityManager = true;

        /// <summary>
        /// 是否对攻击目标进行剪枝
        /// </summary>
        private bool useCutingTargets = true;

        /// <summary>
        /// 是否不重新计算
        /// </summary>
        private bool dontRecalc = true;

        /// <summary>
        /// 是否进行斩杀检查
        /// </summary>
        private bool useLethalCheck = false;

        /// <summary>
        /// 是否使用比较模式（去重）
        /// </summary>
        private bool useComparison = true;

        /// <summary>
        /// 存储可能的游戏状态
        /// </summary>
        List<Playfield> posmoves = new List<Playfield>(7000);

        /// <summary>
        /// 最佳行动
        /// </summary>
        public Action bestmove = null;

        /// <summary>
        /// 最佳行动的价值
        /// </summary>
        public float bestmoveValue = 0;

        /// <summary>
        /// 最佳游戏状态
        /// </summary>
        public Playfield bestboard = new Playfield();

        /// <summary>
        /// 行为策略
        /// </summary>
        public Behavior botBase = null;

        /// <summary>
        /// 已计算的场面数
        /// </summary>
        private int calculated = 0;

        /// <summary>
        /// 是否模拟第二回合
        /// </summary>
        private bool simulateSecondTurn = false;

        /// <summary>
        /// 动作生成器
        /// </summary>
        Movegenerator movegen = Movegenerator.Instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MiniSimulatorNextTurn()
        {
        }

        /// <summary>
        /// 开始模拟敌方回合
        /// </summary>
        /// <param name="p">游戏状态</param>
        /// <param name="simulateTwoTurns">是否模拟两回合</param>
        /// <param name="print">是否打印日志</param>
        /// <param name="playaround">是否防AOE</param>
        /// <param name="playaroundprob">防AOE概率1</param>
        /// <param name="playaroundprob2">防AOE概率2</param>
        private void startEnemyTurnSim(Playfield p, bool simulateTwoTurns, bool print, bool playaround, int playaroundprob, int playaroundprob2)
        {
            // 只有当猜测的英雄血量大于等于1时才模拟
            if (p.guessingHeroHP >= 1)
            {
                // 使用对应的线程模拟敌方回合
                Ai.Instance.enemySecondTurnSim[this.thread].simulateEnemysTurn(p, simulateTwoTurns, playaround, print, playaroundprob, playaroundprob2);
            }
            // 标记场面计算完成
            p.complete = true;
        }

        /// <summary>
        /// 执行所有可能的动作模拟
        /// </summary>
        /// <param name="playf">初始游戏状态</param>
        /// <param name="print">是否打印日志</param>
        /// <returns>最佳行动的价值</returns>
        public float doallmoves(Playfield playf, bool print = false)
        {
            // 标记是否为斩杀检查
            bool isLethalCheck = playf.isLethalCheck;

            // 从设置中获取参数
            int totalboards = Settings.Instance.nextTurnTotalBoards;  // 总计算场面数
            int maxwide = Settings.Instance.nextTurnMaxWide;          // 每步最大保留场面数
            int maxdeep = Settings.Instance.nextTurnDeep;            // 最大搜索深度
            bool playaround = Settings.Instance.playaround;          // 是否防AOE
            int playaroundprob = Settings.Instance.playaroundprob;    // 防AOE概率1
            int playaroundprob2 = Settings.Instance.playaroundprob2;  // 防AOE概率2

            // 设置行为策略
            botBase = Ai.Instance.botBase;

            // 清空可能的游戏状态列表，添加初始状态
            this.posmoves.Clear();
            this.posmoves.Add(new Playfield(playf));

            // 标记是否还有操作可做
            bool havedonesomething = true;
            // 临时存储当前层的游戏状态
            List<Playfield> temp = new List<Playfield>();
            // 当前搜索深度
            int deep = 0;
            // 重置已计算场面数
            this.calculated = 0;
            // 记录最佳状态
            Playfield bestold = null;
            float bestoldval = -20000000;

            // 循环模拟，直到没有操作可做或达到搜索深度
            while (havedonesomething)
            {
                //GC.Collect();
                // 清空临时列表，添加当前层的所有状态
                temp.Clear();
                temp.AddRange(this.posmoves);
                // 重置操作标记
                havedonesomething = false;

                // 遍历当前层的每个状态
                foreach (Playfield p in temp)
                {
                    // 如果状态已完成或我方英雄已死亡，跳过
                    if (p.complete || p.ownHero.Hp <= 0)
                    {
                        continue;
                    }

                    // 生成所有可能的动作
                    List<Action> actions = movegen.getMoveList(p, usePenalityManager, useCutingTargets, true);

                    // 遍历每个可能的动作
                    foreach (Action a in actions)
                    {
                        // 标记有操作可做
                        havedonesomething = true;
                        // 创建新的游戏状态并执行动作
                        Playfield pf = new Playfield(p);
                        pf.doAction(a);
                        // 如果我方英雄存活，添加到可能的状态列表
                        if (pf.ownHero.Hp > 0) this.posmoves.Add(pf);
                        // 增加已计算场面数
                        if (totalboards > 0) this.calculated++;
                    }

                    // 结束当前回合
                    p.endTurn();

                    // 如果不是斩杀检查，模拟敌方回合
                    if (!isLethalCheck) this.startEnemyTurnSim(p, this.simulateSecondTurn, false, playaround, playaroundprob, playaroundprob2);

                    // 评估状态价值，更新最佳状态
                    if (botBase.getPlayfieldValue(p) > bestoldval)
                    {
                        bestoldval = botBase.getPlayfieldValue(p);
                        bestold = p;
                    }

                    // 从可能的状态列表中移除当前状态
                    posmoves.Remove(p);

                    // 如果已计算场面数超过限制，跳出循环
                    if (this.calculated > totalboards) break;
                }

                // 对可能的状态进行剪枝
                cuttingposibilities(maxwide);

                // 增加搜索深度
                deep++;

                // 如果已计算场面数超过限制或达到最大搜索深度，跳出循环
                if (this.calculated > totalboards) break;
                if (deep >= maxdeep) break;
            }

            // 将最佳状态添加到可能的状态列表
            posmoves.Add(bestold);

            // 处理未完成的状态
            foreach (Playfield p in posmoves)
            {
                if (!p.complete)
                {
                    // 结束回合
                    p.endTurn();
                    // 模拟敌方回合
                    if (!isLethalCheck) this.startEnemyTurnSim(p, this.simulateSecondTurn, false, playaround, playaroundprob, playaroundprob2);
                }
            }

            // 寻找最佳状态
            if (posmoves.Count >= 1)
            {
                // 按价值排序
                posmoves.Sort((a, b) => botBase.getPlayfieldValue(b).CompareTo(botBase.getPlayfieldValue(a)));

                // 初始化最佳状态
                Playfield bestplay = posmoves[0];
                float bestval = botBase.getPlayfieldValue(bestplay);
                int pcount = posmoves.Count;

                // 遍历所有状态，寻找最佳状态
                for (int i = 1; i < pcount; i++)
                {
                    float val = botBase.getPlayfieldValue(posmoves[i]);
                    // 如果价值低于当前最佳，跳出循环
                    if (bestval > val) break;
                    // 如果动作数更多，跳过
                    if (bestplay.playactions.Count <= posmoves[i].playactions.Count) continue; //priority to the minimum acts
                    // 更新最佳状态
                    bestplay = posmoves[i];
                    bestval = val;
                }

                // 设置最佳行动和状态
                this.bestmove = bestplay.getNextAction();
                this.bestmoveValue = bestval;
                this.bestboard = new Playfield(bestplay);
                return bestval;
            }

            // 如果没有找到最佳状态，返回默认值
            this.bestmove = null;
            this.bestmoveValue = -2000000;
            this.bestboard = playf;
            return -2000000;
        }

        /// <summary>
        /// 剪枝可能的状态
        /// </summary>
        /// <param name="maxwide">每步最大保留场面数</param>
        public void cuttingposibilities(int maxwide)
        {
            // 临时存储剪枝后的状态
            List<Playfield> temp = new List<Playfield>();
            // 用于去重的字典
            Dictionary<Int64, Playfield> tempDict = new Dictionary<Int64, Playfield>();

            try
            {
                // 按价值排序，保留最佳状态
                posmoves.Sort((a, b) => -(botBase.getPlayfieldValue(a)).CompareTo(botBase.getPlayfieldValue(b)));//want to keep the best
            }
            catch (Exception e)
            {
                // 捕获异常并记录日志
                Helpfunctions.Instance.logg("异常:" + e.Message); //Todo: 待Fix 不应该有异常，猜测是因为相同场面因为牌序不同，得分不同，可以考虑先去重再排序
            }

            // 如果使用比较模式（去重）
            if (this.useComparison)
            {
                int i = 0;
                // 计算最大处理数量
                int max = Math.Min(posmoves.Count, maxwide);

                Playfield p = null;
                // 遍历状态，进行去重
                for (i = 0; i < max; i++)
                {
                    p = posmoves[i];
                    // 获取状态的哈希值
                    Int64 hash = p.GetPHash();
                    p.hashcode = hash;
                    // 如果字典中不包含该哈希值，添加到字典
                    if (!tempDict.ContainsKey(hash)) tempDict.Add(hash, p);
                }

                // 将字典中的状态添加到临时列表
                foreach (KeyValuePair<Int64, Playfield> d in tempDict)
                {
                    temp.Add(d.Value);
                }
            }
            else
            {
                // 如果不使用比较模式，直接添加所有状态
                temp.AddRange(posmoves);
            }

            // 清空原列表，添加剪枝后的状态
            posmoves.Clear();
            posmoves.AddRange(temp.GetRange(0, Math.Min(maxwide, temp.Count)));
        }

        /// <summary>
        /// 剪枝攻击目标
        /// </summary>
        /// <param name="oldlist">原始目标列表</param>
        /// <param name="p">游戏状态</param>
        /// <param name="own">是否为我方攻击</param>
        /// <returns>剪枝后的目标列表</returns>
        public List<targett> cutAttackTargets(List<targett> oldlist, Playfield p, bool own)
        {
            // 存储剪枝后的目标
            List<targett> retvalues = new List<targett>();
            // 存储已添加的随从
            List<Minion> addedmins = new List<Minion>(8);

            // 是否有优先目标
            bool priomins = false;
            // 优先目标列表
            List<targett> retvaluesPrio = new List<targett>();

            // 遍历原始目标列表
            foreach (targett t in oldlist)
            {
                // 如果目标是英雄
                if ((own && t.target == 200) || (!own && t.target == 100))
                {
                    retvalues.Add(t);
                    continue;
                }

                // 如果目标是随从
                if ((own && t.target >= 10 && t.target <= 19) || (!own && t.target >= 0 && t.target <= 9))
                {
                    Minion m = null;
                    // 获取目标随从
                    if (own) m = p.enemyMinions[t.target - 10];
                    if (!own) m = p.ownMinions[t.target];

                    /*if (penman.priorityDatabase.ContainsKey(m.name))
                    {
                        //retvalues.Add(t);
                        retvaluesPrio.Add(t);
                        priomins = true;
                        //help.logg(m.name + " is added to targetlist");
                        continue;
                    }*/

                    // 标记是否添加该目标
                    bool goingtoadd = true;
                    // 临时存储已添加的随从
                    List<Minion> temp = new List<Minion>(addedmins);
                    // 标记是否为特殊随从
                    bool isSpecial = m.handcard.card.isSpecialMinion;

                    // 遍历已添加的随从
                    foreach (Minion mnn in temp)
                    {
                        // special minions are allowed to attack in silended and unsilenced state!
                        //help.logg(mnn.silenced + " " + m.silenced + " " + mnn.name + " " + m.name + " " + penman.specialMinions.ContainsKey(m.name));

                        // 标记另一个随从是否为特殊随从
                        bool otherisSpecial = mnn.handcard.card.isSpecialMinion;

                        // 如果两个随从都不是特殊随从，或者都是被沉默的特殊随从
                        if ((!isSpecial || (isSpecial && m.silenced)) && (!otherisSpecial || (otherisSpecial && mnn.silenced))) // both are not special, if they are the same, dont add
                        {
                            // 如果属性相同，不添加
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous && mnn.lifesteal == m.lifesteal) goingtoadd = false;
                            continue;
                        }

                        // 如果两个随从都是特殊随从且未被沉默
                        if (isSpecial == otherisSpecial && !m.silenced && !mnn.silenced) // same are special
                        {
                            // 如果名称不同，添加
                            if (m.name != mnn.name) // different name -> take it
                            {
                                continue;
                            }
                            // 如果名称相同，检查属性是否相同
                            if (mnn.Angr == m.Angr && mnn.Hp == m.Hp && mnn.divineshild == m.divineshild && mnn.taunt == m.taunt && mnn.poisonous == m.poisonous && mnn.lifesteal == m.lifesteal) goingtoadd = false;
                            continue;
                        }
                    }

                    // 如果可以添加，添加到列表
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

            // 如果有优先目标，返回优先目标列表
            if (priomins) return retvaluesPrio;

            // 返回剪枝后的目标列表
            return retvalues;
        }

        /// <summary>
        /// 打印可能的游戏状态
        /// </summary>
        public void printPosmoves()
        {
            foreach (Playfield p in this.posmoves)
            {
                p.printBoard();
            }
        }

    }


}