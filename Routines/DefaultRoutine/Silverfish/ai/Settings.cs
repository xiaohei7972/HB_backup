

using System;
using System.IO;

namespace HREngine.Bots
{
    /// <summary>
    /// Settings 类 - 管理 Silverfish AI 的各种配置参数
    /// 此类采用单例模式，确保全局只有一个配置实例
    /// </summary>
    public class Settings
    {
        // 核心战斗设置区域###################################
        /// <summary>
        /// 敌方英雄血量阈值 - 当敌方血量低于此值时，允许英雄使用武器攻击敌方英雄
        /// 单位：生命值
        /// </summary>
        public int enfacehp = 15;  
        
        /// <summary>
        /// 武器攻击模式设置 - 当敌方血量高于 enfacehp 时，武器的攻击行为模式
        /// 0 - 不攻击敌方英雄，除非武器攻击力为1
        /// 1 - 当武器耐久度为1时不攻击敌方英雄，耐久度大于1时允许攻击（除非武器攻击力为1）
        /// 2 - 不攻击敌方英雄（任何武器）
        /// 3 - 当武器耐久度为1时不攻击敌方英雄，耐久度大于1时允许攻击（任何武器）
        /// 4 - 不攻击敌方英雄，除非手牌中有武器生成卡牌（除了 Upgrade!）
        /// 5 - 不攻击敌方英雄，除非手牌中有攻击力大于1的武器生成卡牌（或两者攻击力都为1）（除了 Upgrade!）
        /// </summary>
        public int weaponOnlyAttackMobsUntilEnfacehp = 5;
        
        /// <summary>
        /// 每深度级别考虑的棋盘数量 - 影响AI计算的广度
        /// 值越大，AI考虑的可能性越多，但计算时间也会增加
        /// </summary>
        public int maxwide = 3000;   

        /// <summary>
        /// 是否开启防AOE模式 - 使AI考虑敌方可能的AOE法术
        /// </summary>
        public bool playaround = false;  
        
        /// <summary>
        /// 防AOE概率参数1 - 敌方不使用AOE法术的概率（0-100）
        /// 100 = 敌方永远不使用AOE法术，0 = 敌方总是使用AOE法术
        /// </summary>
        public int playaroundprob = 50;    
        
        /// <summary>
        /// 防AOE概率参数2 - 敌方使用AOE法术后我方随从存活的概率（0-100）
        /// 100 = 我方随从总是存活，0 = 我方随从总是死亡（存活与否取决于实际生命值）
        /// </summary>
        public int playaroundprob2 = 80;   

        /// <summary>
        /// 第二AI步骤模拟的棋盘数量 - 影响AI对未来回合的预测深度
        /// </summary>
        public int twotsamount = 256; 
        
        /// <summary>
        /// 敌方第一回合第一步AI计算的最大棋盘数量
        /// 此值应低于 enemyTurnMaxWideSecondStep
        /// </summary>
        public int enemyTurnMaxWide = 40; 
        
        /// <summary>
        /// 敌方第一回合第二步AI计算的最大棋盘数量
        /// 此值应高于 enemyTurnMaxWide
        /// </summary>
        public int enemyTurnMaxWideSecondStep = 200; 

        /// <summary>
        /// 第二回合的最大 combo 深度 - 影响AI计算的深度
        /// 注意：请勿修改此值！
        /// </summary>
        public int nextTurnDeep = 6; 
        
        /// <summary>
        /// 第二回合每步计算的最大最佳棋盘数
        /// </summary>
        public int nextTurnMaxWide = 20; 
        
        /// <summary>
        /// 第二回合模拟计算的最大棋盘总数
        /// </summary>
        public int nextTurnTotalBoards = 200; 
        
        /// <summary>
        /// 狂暴模式设置 - 如果有机会在下一回合杀死敌人，所有攻击都会朝向敌方英雄
        /// 0 - 关闭，1 - 开启
        /// </summary>
        public int berserkIfCanFinishNextTour = 0; 

        /// <summary>
        /// 第二回合权重系数 - 影响AI对当前回合和下一回合的重视程度（0-100）
        /// 值越高，AI越重视下一回合的结果
        /// </summary>
        public int alpha = 50; 
        
        /// <summary>
        /// 是否开启防奥秘模式 - 使AI考虑敌方可能的奥秘
        /// </summary>
        public bool useSecretsPlayAround = true; 
        
        /// <summary>
        /// 随从站位模式设置
        /// 0 - 随从按价值交错排列（低值-高值-低值...）
        /// 1 - 高价值随从站在边缘，低价值随从站在中间
        /// </summary>
        public int placement = 0;  

        //外部进程设置（仅在 useExternalProcess = true 时使用）
        /// <summary>
        /// 是否使用外部进程进行计算
        /// 注意：仅当 useExternalProcess = true 时才能使用 passiveWaiting
        /// </summary>
        public bool useExternalProcess = false; 
        
        /// <summary>
        /// 是否被动等待外部进程完成计算
        /// </summary>
        public bool passiveWaiting = false; 

        /// <summary>
        /// 速度提升级别
        /// 0 - 正常速度（最安全 - 等待动画）
        /// 1 - 加速脸攻击
        /// 2 - 加速脸攻击和随从攻击
        /// </summary>
        public int speedupLevel = 0; 
        
        /// <summary>
        /// 行动调整模式 - 计算后重新排序行动
        /// 0 - 按计算顺序（默认）
        /// 1 - 先使用AOE
        /// 注意：测试功能！
        /// </summary>
        public int adjustActions = 0; 
        
        /// <summary>
        /// 规则打印模式
        /// 0 - 关闭
        /// 1 - 开启
        /// </summary>
        public int printRules = 0; 
        
        /// <summary>
        /// 认输模式
        /// 0 - 直接失败时认输
        /// 1 - 当 Playfield 值 <= -10000 时认输
        /// </summary>
        public int concedeMode = 0; 
        //###########################################################

        /// <summary>
        /// 当前回合权重 - 由 alpha 计算得出
        /// </summary>
        public float firstweight = 0.5f;
        
        /// <summary>
        /// 下一回合权重 - 由 alpha 计算得出
        /// </summary>
        public float secondweight = 0.5f;
        
        /// <summary>
        /// 线程数量 - 用于并行计算
        /// </summary>
        public int numberOfThreads = 32;
        
        /// <summary>
        /// 是否模拟敌方回合
        /// </summary>
        public bool simulateEnemysTurn = true;
        
        /// <summary>
        /// 第二回合模拟数量
        /// </summary>
        public int secondTurnAmount = 256;
        
        /// <summary>
        /// 主路径
        /// </summary>
        public string mainPath = "";
        
        /// <summary>
        /// 当前路径
        /// </summary>
        public string path = "";
        
        /// <summary>
        /// 日志路径
        /// </summary>
        public string logpath = "";
        
        /// <summary>
        /// 日志文件名
        /// </summary>
        public string logfile = "Logg.txt";
        
        /// <summary>
        /// 是否认输
        /// </summary>
        public bool concede = false;
        
        /// <summary>
        /// 敌方是否认输
        /// </summary>
        public bool enemyConcede = false;
        
        /// <summary>
        /// 是否将日志写入单个文件
        /// </summary>
        public bool writeToSingleFile = false;
        
        /// <summary>
        /// 是否开启学习模式
        /// </summary>
        public bool learnmode = true;
        
        /// <summary>
        /// 是否打印学习模式信息
        /// </summary>
        public bool printlearnmode = true;
        
        /// <summary>
        /// 是否处于测试模式
        /// </summary>
        public bool test = false;

        /// <summary>
        /// Settings 单例实例
        /// </summary>
        private static Settings instance;

        /// <summary>
        /// 获取 Settings 单例实例
        /// 使用懒加载模式，首次访问时创建实例
        /// </summary>
        public static Settings Instance
        {
            get
            {
                return instance ?? (instance = new Settings());
            }
        }

        /// <summary>
        /// 私有构造函数 - 防止外部实例化
        /// </summary>
        private Settings()
        {
            this.writeToSingleFile = false;
        }

        /// <summary>
        /// 从配置文件加载设置
        /// </summary>
        /// <param name="behavName">行为名称或配置文件路径</param>
        /// <param name="nameIsPath">behavName 是否为路径</param>
        public void setSettings(string behavName, bool nameIsPath = false)
        {
            if (test) return;
            string pathToSettings = behavName;
            if (test)
            {
                pathToSettings = mainPath + "behavior\\" + behavName + "\\_settings.txt";
            }
            else if (!nameIsPath)
            {
                if (!Silverfish.Instance.BehaviorPath.ContainsKey(behavName))
                {
                    Helpfunctions.Instance.ErrorLog(behavName + ": 本策略没有配置文件.");
                    endOfSetSettings();
                    return;
                }
                pathToSettings = Path.Combine(Silverfish.Instance.BehaviorPath[behavName], "_settings_custom.txt");
                if (System.IO.File.Exists(pathToSettings)) Helpfunctions.Instance.ErrorLog(behavName + ": 应用自定义配置.");
                else pathToSettings = Path.Combine(Silverfish.Instance.BehaviorPath[behavName], "_settings.txt");
            }

            if (!System.IO.File.Exists(pathToSettings))
            {
                Helpfunctions.Instance.ErrorLog(behavName + ": 没有设置.");
                endOfSetSettings();
                return;
            }
            try
            {
                Helpfunctions.Instance.ErrorLog("读取战场设置 " + behavName);
                string[] lines = System.IO.File.ReadAllLines(pathToSettings);
                String[] tmp;
                int valueInt;
                bool valueBool = false;
                foreach (string s in lines)
                {
                    if (s == "" || s == null) continue;
                    if (s.StartsWith("//")) continue;
                    tmp = s.Split(';')[0].Split(' ');
                    if (tmp.Length != 3) continue;

                    if (!Int32.TryParse(tmp[2], out valueInt))
                    {
                        switch (tmp[2])
                        {
                            case "true": valueBool = true; break;
                            case "false": valueBool = false; break;
                        }
                    }

                    switch (tmp[0])
                    {
                        case "enfacehp": this.enfacehp = valueInt; break;
                        case "weaponOnlyAttackMobsUntilEnfacehp": this.weaponOnlyAttackMobsUntilEnfacehp = valueInt; break;
                        // case "maxwide": this.maxwide = valueInt; break;
                        case "playaround": this.playaround = valueBool; break;
                        case "playaroundprob": this.playaroundprob = valueInt; break;
                        case "playaroundprob2": this.playaroundprob2 = valueInt; break;
                        case "twotsamount": this.twotsamount = valueInt; break;
                        case "enemyTurnMaxWide": this.enemyTurnMaxWide = valueInt; break;
                        case "enemyTurnMaxWideSecondStep": this.enemyTurnMaxWideSecondStep = valueInt; break;
                        case "berserkIfCanFinishNextTour": this.berserkIfCanFinishNextTour = valueInt; break;
                        case "concedeMode": this.concedeMode = valueInt; break;                            
                        case "printRules": this.printRules = valueInt; break;
                        case "nextTurnDeep": this.nextTurnDeep = valueInt; break;
                        case "nextTurnMaxWide": this.nextTurnMaxWide = valueInt; break;
                        case "nextTurnTotalBoards": this.nextTurnTotalBoards = valueInt; break;
                        case "useSecretsPlayAround": this.useSecretsPlayAround = valueBool; break;
                        case "alpha": this.alpha = valueInt; break;
                        case "placement": this.placement = valueInt; break;
                        case "useExternalProcess": this.useExternalProcess = valueBool; break;
                        case "passiveWaiting": this.passiveWaiting = valueBool; break;
                        case "speedupLevel": this.speedupLevel = valueInt; break;
                        case "adjustActions": this.adjustActions = valueInt; break;
                        default:
                            Helpfunctions.Instance.ErrorLog(tmp[2] + " is not supported!!!");
                            break;
                    }
                }
                endOfSetSettings();
            }
            catch (Exception e)
            {
                Helpfunctions.Instance.ErrorLog(behavName + " _settings.txt - 读取错误. 我们将使用默认配置. 异常: " + e.Message);
                endOfSetSettings();
                return;
            }
            Helpfunctions.Instance.ErrorLog(behavName + " 配置文件读取成功.");
            printSettings();
        }

        /// <summary>
        /// 设置完成后的处理方法
        /// 应用配置并记录日志
        /// </summary>
        private void endOfSetSettings()
        {
            setWeights(this.alpha);

            Helpfunctions.Instance.ErrorLog("设置怼脸血线值为: " + this.enfacehp);
            Helpfunctions.Instance.ErrorLog("设置武器怼脸血线值: " + this.weaponOnlyAttackMobsUntilEnfacehp);
            ComboBreaker.Instance.attackFaceHP = this.enfacehp;

            Ai.Instance.setMaxWide(DefaultRoutineSettings.Instance.MaxWide);
            Helpfunctions.Instance.ErrorLog("设置AI值（maxwide）: " + DefaultRoutineSettings.Instance.MaxWide);
            Ai.Instance.setTwoTurnSimulation(false, this.twotsamount);
            Helpfunctions.Instance.ErrorLog("计算下个回合 " + this.twotsamount + " 线程");
            if (this.twotsamount > 0) Helpfunctions.Instance.ErrorLog("推算下回合敌方行动");
            if (this.useSecretsPlayAround) Helpfunctions.Instance.ErrorLog("开启防奥秘");
            if (this.playaround)
            {
                Ai.Instance.setPlayAround();
                Helpfunctions.Instance.ErrorLog("开启防AOE");
            }
            if (this.writeToSingleFile) Helpfunctions.Instance.ErrorLog("write log to single file");

        }

        /// <summary>
        /// 根据 alpha 值计算权重
        /// </summary>
        /// <param name="alpha">第二回合权重系数（0-100）</param>
        public void setWeights(int alpha)
        {
            float a = ((float)alpha) / 100f;
            this.firstweight = 1f - a;
            this.secondweight = a;
            Helpfunctions.Instance.ErrorLog("目前的AI值（alpha）是 " + this.secondweight);
        }

        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        public void setFilePath(string path)
        {
            this.path = path;
        }
        
        /// <summary>
        /// 设置日志路径
        /// </summary>
        /// <param name="path">日志路径</param>
        public void setLoggPath(string path)
        {
            this.logpath = path;
        }

        /// <summary>
        /// 设置日志文件名
        /// </summary>
        /// <param name="path">日志文件名</param>
        public void setLoggFile(string path)
        {
            this.logfile = path;
        }

        /// <summary>
        /// 打印所有设置到日志
        /// </summary>
        public void printSettings()
        {
            Helpfunctions.Instance.logg("#################### Settings #########################################");
            Helpfunctions.Instance.logg("path = " + Settings.Instance.path);
            Helpfunctions.Instance.logg("logpath = " + Settings.Instance.logpath);
            Helpfunctions.Instance.logg("logfile = " + Settings.Instance.logfile);
            Helpfunctions.Instance.logg("twotsamount = " + Settings.Instance.twotsamount);
            Helpfunctions.Instance.logg("secondTurnAmount = " + Settings.Instance.secondTurnAmount);
            Helpfunctions.Instance.logg("playaroundprob2 = " + Settings.Instance.playaroundprob2);
            Helpfunctions.Instance.logg("playaroundprob = " + Settings.Instance.playaroundprob);
            Helpfunctions.Instance.logg("nextTurnTotalBoards = " + Settings.Instance.nextTurnTotalBoards);
            Helpfunctions.Instance.logg("nextTurnMaxWide = " + Settings.Instance.nextTurnMaxWide);
            Helpfunctions.Instance.logg("nextTurnDeep = " + Settings.Instance.nextTurnDeep);
            Helpfunctions.Instance.logg("maxwide = " + Settings.Instance.maxwide);
            Helpfunctions.Instance.logg("enfacehp = " + Settings.Instance.enfacehp);
            Helpfunctions.Instance.logg("weaponOnlyAttackMobsUntilEnfacehp = " + Settings.Instance.weaponOnlyAttackMobsUntilEnfacehp);
            Helpfunctions.Instance.logg("enemyTurnMaxWideSecondStep = " + Settings.Instance.enemyTurnMaxWideSecondStep);
            Helpfunctions.Instance.logg("enemyTurnMaxWide = " + Settings.Instance.enemyTurnMaxWide);
            Helpfunctions.Instance.logg("alpha = " + Settings.Instance.alpha);
            Helpfunctions.Instance.logg("secondweight = " + Settings.Instance.secondweight);
            Helpfunctions.Instance.logg("firstweight = " + Settings.Instance.firstweight);
            Helpfunctions.Instance.logg("writeToSingleFile = " + Settings.Instance.writeToSingleFile);
            Helpfunctions.Instance.logg("useSecretsPlayAround = " + Settings.Instance.useSecretsPlayAround);
            Helpfunctions.Instance.logg("useExternalProcess = " + Settings.Instance.useExternalProcess);
            Helpfunctions.Instance.logg("placement = " + Settings.Instance.placement);
            Helpfunctions.Instance.logg("simulateEnemysTurn = " + Settings.Instance.simulateEnemysTurn);
            Helpfunctions.Instance.logg("printlearnmode = " + Settings.Instance.printlearnmode);
            Helpfunctions.Instance.logg("playaround = " + Settings.Instance.playaround);
            Helpfunctions.Instance.logg("passiveWaiting = " + Settings.Instance.passiveWaiting);
            Helpfunctions.Instance.logg("learnmode = " + Settings.Instance.learnmode);
            Helpfunctions.Instance.logg("enemyConcede = " + Settings.Instance.enemyConcede);
            Helpfunctions.Instance.logg("concede = " + Settings.Instance.concede);
            Helpfunctions.Instance.logg("speedupLevel = " + Settings.Instance.speedupLevel);
            Helpfunctions.Instance.logg("adjustActions = " + Settings.Instance.adjustActions);
            Helpfunctions.Instance.logg("#################### Settings End #####################################");
        }
    }
}