namespace HREngine.Bots
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum HeroEnum
    {
        None,
        weizbang,//威兹班
        druid,		//德鲁伊
        hunter,		//猎人
        priest,		//牧师
        warlock,	//术士
        thief,		//贼
        pala,
        warrior,	//战士
        shaman,		//萨满
        mage,		//法师
        lordjaraxxus,//大王
        ragnarosthefirelord,//炎魔
        hogger,//霍格
        demonhunter,//恶魔猎手
        deathknight,//巫妖王
    }

    /// <summary>
    /// 对局信息类，记录游戏中的各种状态和数据
    /// Hrtprozis是兄弟中非常重要的一个类，记录着从兄弟内部数据中获取到的各种信息
    /// </summary>    
    public class Hrtprozis
    {
        /// <summary>
        /// 唯一ID，用于标识操作
        /// </summary>
        public int pId = 0;
        /// <summary>
        /// 打脸血量，用于判断是否应该直接攻击敌方英雄
        /// </summary>
        public int attackFaceHp = 15;
        /// <summary>
        /// 我方疲劳值
        /// </summary>
        public int ownHeroFatigue = 0;
        /// <summary>
        /// 我方牌库数量
        /// </summary>
        public int ownDeckSize = 30;  
        /// <summary>
        /// 敌方牌库数量
        /// </summary>
        public int enemyDeckSize = 30; 
        /// <summary>
        /// 敌方疲劳值
        /// </summary>
        public int enemyHeroFatigue = 0;      
        /// <summary>
        /// 第几个回合，敌方双方都算，我方先手，则是1,3,5,7....
        /// </summary>
        public int gTurn = 0;           
        /// <summary>
        /// 同一个回合内，第几个操作
        /// </summary>
        public int gTurnStep = 0;

        /// <summary>
        /// 我方英雄实体ID
        /// </summary>
        public int ownHeroEntity = -1;   
        /// <summary>
        /// 敌方英雄实体ID
        /// </summary>
        public int enemyHeroEntitiy = -1;
        /// <summary>
        /// 回合开始时间
        /// </summary>
        public DateTime roundstart = DateTime.Now;
        /// <summary>
        /// 当前法力值
        /// </summary>
        public int currentMana = 0;

        /// <summary>
        /// 我方英雄血量
        /// </summary>
        public int heroHp = 30;
        /// <summary>
        /// 敌方英雄血量
        /// </summary>
        public int enemyHp = 30;          
        /// <summary>
        /// 我方英雄攻击力
        /// </summary>
        public int heroAtk = 0;
        /// <summary>
        /// 敌方英雄攻击力
        /// </summary>
        public int enemyAtk = 0;          
        /// <summary>
        /// 我方英雄护甲
        /// </summary>
        public int heroDefence = 0;
        /// <summary>
        /// 敌方英雄护甲
        /// </summary>
        public int enemyDefence = 0;  
        /// <summary>
        /// 我方英雄是否可以攻击
        /// </summary>
        public bool ownheroisread = false;
        /// <summary>
        /// 我方英雄此回合攻击了几次
        /// </summary>
        public int ownHeroNumAttacksThisTurn = 0;
        /// <summary>
        /// 我方英雄是否有风怒
        /// </summary>
        public bool ownHeroWindfury = false; 
        /// <summary>
        /// 我方英雄是否冻结
        /// </summary>
        public bool herofrozen = false;
        /// <summary>
        /// 敌方英雄是否冻结
        /// </summary>
        public bool enemyfrozen = false;

        /// <summary>
        /// 我方奥秘列表
        /// </summary>
        public List<CardDB.cardIDEnum> ownSecretList = new List<CardDB.cardIDEnum>();
        /// <summary>
        /// 敌方奥秘数量
        /// </summary>
        public int enemySecretCount = 0;
        /// <summary>
        /// 发现的卡牌
        /// </summary>
        public Dictionary<int, CardDB.cardIDEnum> DiscoverCards = new Dictionary<int, CardDB.cardIDEnum>();
        /// <summary>
        /// 牌库的卡牌及其数量
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> turnDeck = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 指定费用的卡牌
        /// </summary>
        private Dictionary<int, CardDB.cardIDEnum> deckCardForCost = new Dictionary<int, CardDB.cardIDEnum>();
        /// <summary>
        /// 牌库无重复（宇宙）
        /// </summary>
        public bool noDuplicates = false;
        /// <summary>
        /// 牌库中是否有帕奇斯
        /// </summary>
        public bool patchesInDeck = false;

        /// <summary>
        /// 牌库中有几张嘲讽卡
        /// </summary>
        private int numTauntCards = -1;
        /// <summary>
        /// 牌库中有几张圣盾卡
        /// </summary>
        private int numDivineShieldCards = -1;  
        /// <summary>
        /// 牌库中有几张吸血卡
        /// </summary>
        private int numLifestealCards = -1;  
        /// <summary>
        /// 牌库中有几张风怒卡
        /// </summary>
        private int numWindfuryCards = -1;  
        /// <summary>
        /// 牌库中有几张奥秘卡，用于远古谜团等判断
        /// </summary>
        private int numSecretCards = -1;

        /// <summary>
        /// 是否设置了游戏规则
        /// </summary>
        public bool setGameRule = false;

        /// <summary>
        /// 我方英雄职业
        /// </summary>
        public HeroEnum heroname = HeroEnum.None;
        /// <summary>
        /// 敌方英雄职业
        /// </summary>
        public HeroEnum enemyHeroname = HeroEnum.None;
        /// <summary>
        /// 我方英雄名称
        /// </summary>
        public string heronameingame = "";
        /// <summary>
        /// 敌方英雄名称
        /// </summary>
        public string enemyHeronameingame = "";
        /// <summary>
        /// 我方英雄初始职业
        /// </summary>
        public TAG_CLASS ownHeroStartClass = TAG_CLASS.INVALID;
        /// <summary>
        /// 敌方英雄初始职业
        /// </summary>
        public TAG_CLASS enemyHeroStartClass = TAG_CLASS.INVALID;
        /// <summary>
        /// 我方英雄技能
        /// </summary>
        public CardDB.Card heroAbility;
        /// <summary>
        /// 我方英雄技能是否准备完成
        /// </summary>
        public bool ownAbilityisReady = false;
        /// <summary>
        /// 我方英雄技能费用
        /// </summary>
        public int ownHeroPowerCost = 2;
        /// <summary>
        /// 敌方英雄技能
        /// </summary>
        public CardDB.Card enemyAbility;
        /// <summary>
        /// 敌方英雄技能费用
        /// </summary>
        public int enemyHeroPowerCost = 2;
        /// <summary>
        /// 此回合中的操作数
        /// </summary>
        public int numOptionsPlayedThisTurn = 0;
        /// <summary>
        /// 此回合中的随从数
        /// </summary>
        public int numMinionsPlayedThisTurn = 0;
        /// <summary>
        /// 我方最后死亡的随从
        /// </summary>
        public CardDB.cardIDEnum OwnLastDiedMinion = CardDB.cardIDEnum.None;

        /// <summary>
        /// 此回合中的出牌数，可以用于判定连击
        /// </summary>
        public int cardsPlayedThisTurn = 0;
        /// <summary>
        /// 过载水晶
        /// </summary>
        public int ueberladung = 0; 
        /// <summary>
        /// 锁定水晶
        /// </summary>
        public int lockedMana = 0;

        /// <summary>
        /// 我方最大法力值
        /// </summary>
        public int ownMaxMana = 0;
        /// <summary>
        /// 敌方最大法力值
        /// </summary>
        public int enemyMaxMana = 0;

        /// <summary>
        /// 我方英雄
        /// </summary>
        public Minion ownHero = new Minion();
        /// <summary>
        /// 敌方英雄
        /// </summary>
        public Minion enemyHero = new Minion();
        /// <summary>
        /// 我方武器
        /// </summary>
        public Weapon ownWeapon = new Weapon();
        /// <summary>
        /// 敌方武器
        /// </summary>
        public Weapon enemyWeapon = new Weapon();
        /// <summary>
        /// 我方随从
        /// </summary>
        public List<Minion> ownMinions = new List<Minion>();
        /// <summary>
        /// 敌方随从
        /// </summary>
        public List<Minion> enemyMinions = new List<Minion>();
        /// <summary>
        /// 沟渠潜伏者消灭随从的实体id
        /// </summary>
        public Dictionary<int, IDEnumOwner> LurkersDB = new Dictionary<int, IDEnumOwner>();

        /// <summary>
        /// 克苏恩血量加成
        /// </summary>
        public int anzOgOwnCThunHpBonus = 0;
        /// <summary>
        /// 克苏恩攻击力加成
        /// </summary>
        public int anzOgOwnCThunAngrBonus = 0;
        /// <summary>
        /// 克苏恩嘲讽状态
        /// </summary>
        public int anzOgOwnCThunTaunt = 0;
        /// <summary>
        /// 我方魔像计数器
        /// </summary>
        public int anzOwnJadeGolem = 0;
        /// <summary>
        /// 敌方魔像计数器
        /// </summary>
        public int anzEnemyJadeGolem = 0;
        /// <summary>
        /// 魔王的骑士：水晶核心，任务贼的任务奖励，在本局对战的剩余时间内，你的所有随从变为 4/4
        /// </summary>
        public int ownCrystalCore = 0;
        /// <summary>
        /// 我方牌库中是否有0费随从
        /// </summary>
        public bool ownMinionsInDeckCost0 = false;
        /// <summary>
        /// 在此回合中使用元素牌的数量
        /// </summary>
        public int anzOwnElementalsThisTurn = 0;
        /// <summary>
        /// 上一回合使用元素牌的数量
        /// </summary>
        public int anzOwnElementalsLastTurn = 0;
        /// <summary>
        /// 是否有自然使徒在手上的时候使用过自然法术
        /// </summary>
        public int useNature = 0;
        /// <summary>
        /// 我方元素牌具有吸血的数量
        /// </summary>
        public int ownElementalsHaveLifesteal = 0;
        /// <summary>
        /// 我方玩家控制器ID
        /// </summary>
        private int ownPlayerController = 0;
        /// <summary>
        /// 使白银之手新兵获得圣盾
        /// </summary>
        public bool LothraxionsPower = false;
        /// <summary>
        /// 术士任务线 SW_091
        /// </summary>
        public bool anzTamsin = false;

        /// <summary>
        /// 猜测对手的套牌名称
        /// </summary>
        public string enemyDeckName = ""; 
        /// <summary>
        /// 猜测对手的套牌代码
        /// </summary>
        public string enemyDeckCode = ""; 
        /// <summary>
        /// 猜测对手剩余卡牌及其数量
        /// </summary>
        public Dictionary<string, int> guessEnemyDeck = new Dictionary<string, int>();
        /// <summary>
        /// 预计对手斩杀线
        /// </summary>
        public int enemyDirectDmg = 0; 
        /// <summary>
        /// 套牌相似度
        /// </summary>
        public int similarity = 66;

        /// <summary>
        /// 连续使用元素牌的回合数
        /// </summary>
        public int ownConsecutiveElementalTurns = 0;
        /// <summary>
        /// 上个回合玩家使用的元素牌数量
        /// </summary>
        public int ownElementalsPlayedLastTurn = 0;

        /// <summary>
        /// 附魔列表
        /// </summary>
        public List<CardDB.cardIDEnum> enchs = new List<CardDB.cardIDEnum>();

        /// <summary>
        /// 惩罚管理器
        /// </summary>
        public PenalityManager penman;
        /// <summary>
        /// 设置管理器
        /// </summary>
        public Settings settings;
        /// <summary>
        /// 调试输出工具
        /// </summary>
        Helpfunctions help;
        /// <summary>
        /// 卡牌数据库
        /// </summary>
        CardDB cdb;

        /// <summary>
        /// 单例实例
        /// </summary>
        private static Hrtprozis instance;

        /// <summary>
        /// 获取 Hrtprozis 单例实例
        /// </summary>
        public static Hrtprozis Instance
        {
            get
            {
                return instance ?? (instance = new Hrtprozis());
            }
        }

        /// <summary>
        /// 设置实例引用，初始化各种管理器和工具
        /// </summary>
        public void setInstances()
        {
            help = Helpfunctions.Instance;
            penman = PenalityManager.Instance;
            settings = Settings.Instance;
            cdb = CardDB.Instance;
        }

        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private Hrtprozis()
        {
        }

        /// <summary>
        /// 设置打脸血量阈值
        /// </summary>
        /// <param name="hp">血量阈值</param>
        public void setAttackFaceHP(int hp)
        {
            this.attackFaceHp = hp;
        }

        /// <summary>
        /// 获取并递增唯一ID
        /// </summary>
        /// <returns>唯一ID</returns>
        public int getPid()
        {
            return pId++;
        }

        /// <summary>
        /// 新游戏开始时重置所有数据
        /// </summary>
        public void clearAllNewGame()
        {
            this.ownHeroStartClass = TAG_CLASS.INVALID;
            this.enemyHeroStartClass = TAG_CLASS.INVALID;
            this.setGameRule = false;
            enchs = new List<CardDB.cardIDEnum>();
            this.clearAllRecalc();
        }

        /// <summary>
        /// 重置所有需要重新计算的数据
        /// </summary>
        public void clearAllRecalc()
        {
            pId = 0;
            ownHeroEntity = -1;
            enemyHeroEntitiy = -1;
            numSecretCards = -1; //初始化
            currentMana = 0;
            heroHp = 30;
            enemyHp = 30;
            heroAtk = 0;
            enemyAtk = 0;
            heroDefence = 0; enemyDefence = 0;
            ownheroisread = false;
            ownAbilityisReady = false;
            ownHeroNumAttacksThisTurn = 0;
            ownHeroWindfury = false;
            ownSecretList.Clear();
            enemySecretCount = 0;
            heroname = HeroEnum.None;
            enemyHeroname = HeroEnum.None;
            heronameingame = "";
            enemyHeronameingame = "";
            heroAbility = new CardDB.Card();
            enemyAbility = new CardDB.Card();
            herofrozen = false;
            enemyfrozen = false;
            numMinionsPlayedThisTurn = 0;
            cardsPlayedThisTurn = 0;
            ueberladung = 0;
            lockedMana = 0;
            ownMaxMana = 0;
            enemyMaxMana = 0;
            ownWeapon = new Weapon();
            enemyWeapon = new Weapon();
            ownMinions.Clear();
            enemyMinions.Clear();
            noDuplicates = false;
            deckCardForCost.Clear();
            turnDeck.Clear();
            LothraxionsPower = false;
            patchesInDeck = false;
            anzTamsin = false;
            enemyDeckCode = "";
            enemyDirectDmg = 0;
            guessEnemyDeck = new Dictionary<string, int>();
            enemyDeckName = "";
            similarity = 50;
        }


        /// <summary>
        /// 设置我方玩家控制器ID
        /// </summary>
        /// <param name="player">玩家控制器ID</param>
        public void setOwnPlayer(int player)
        {
            this.ownPlayerController = player;
        }

        /// <summary>
        /// 获取我方玩家控制器ID
        /// </summary>
        /// <returns>玩家控制器ID</returns>
        public int getOwnController()
        {
            return this.ownPlayerController;
        }
        /// <summary>
        /// 更新玩家附件状态，如白银之手新兵获得圣盾
        /// </summary>
        /// <param name="LothraxionsPower">是否使白银之手新兵获得圣盾</param>
        public void updatePlayerAttachments(bool LothraxionsPower)
        {
            this.LothraxionsPower = LothraxionsPower;
        }

        /// <summary>
        /// 更新青玉魔像计数器
        /// </summary>
        /// <param name="anzOwnJG">我方魔像计数器</param>
        /// <param name="anzEmemyJG">敌方魔像计数器</param>
        public void updateJadeGolemsInfo(int anzOwnJG, int anzEmemyJG)
        {
            anzOwnJadeGolem = anzOwnJG;
            anzEnemyJadeGolem = anzEmemyJG;
        }

        /// <summary>
        /// 更新水晶核心状态（任务贼奖励）
        /// </summary>
        /// <param name="num">水晶核心状态值</param>
        public void updateCrystalCore(int num)
        {
            ownCrystalCore = num;
        }

        /// <summary>
        /// 更新我方牌库中是否有0费随从的状态
        /// </summary>
        /// <param name="tmp">是否有0费随从</param>
        public void updateOwnMinionsInDeckCost0(bool tmp)
        {
            ownMinionsInDeckCost0 = tmp;
        }

        /// <summary>
        /// 更新元素牌相关信息
        /// </summary>
        /// <param name="anzOwnElemTT">本回合使用元素牌的数量</param>
        /// <param name="anzOwnElemLT">上一回合使用元素牌的数量</param>
        /// <param name="ownElementalsHaveLS">我方元素牌具有吸血的数量</param>
        public void updateElementals(int anzOwnElemTT, int anzOwnElemLT, int ownElementalsHaveLS)
        {
            anzOwnElementalsThisTurn = anzOwnElemTT;
            anzOwnElementalsLastTurn = anzOwnElemLT;
            ownElementalsHaveLifesteal = ownElementalsHaveLS;
        }


        /// <summary>
        /// 将英雄ID转换为英雄名称
        /// </summary>
        /// <param name="s">英雄ID</param>
        /// <returns>英雄名称</returns>
        public string heroIDtoName(string s)
        {
            if (s.StartsWith("HERO_01")) return "warrior";
            else if (s.StartsWith("HERO_02")) return "shaman";
            else if (s.StartsWith("HERO_03")) return "thief";
            else if (s.StartsWith("HERO_04")) return "pala";
            else if (s.StartsWith("HERO_05")) return "hunter";
            else if (s.StartsWith("HERO_06")) return "druid";
            else if (s.StartsWith("HERO_07")) return "warlock";
            else if (s.StartsWith("HERO_08")) return "mage";
            else if (s.StartsWith("HERO_09")) return "priest";
            else if (s.StartsWith("HERO_10")) return "demonhunter";
            else if (s.StartsWith("HERO_11")) return "deathknight";

            switch (s)
            {//添加英雄sim编号到名字的转换
                case "HERO_01": return "warrior";
                case "HERO_01a": return "warrior";
                case "ICC_834": return "warrior";
                case "HERO_02": return "shaman";
                case "HERO_02a": return "shaman";
                case "ICC_481": return "shaman";

                case "HERO_03": return "thief";
                case "HERO_03a": return "thief";
                case "ICC_827": return "thief";
                case "HERO_04": return "pala";
                case "HERO_04a": return "pala";
                case "HERO_04b": return "pala";

                case "ICC_829": return "pala";
                case "HERO_05": return "hunter";
                case "HERO_05a": return "hunter";
                case "ICC_828": return "hunter";

                case "HERO_06": return "druid";
                case "ICC_832": return "druid";

                case "HERO_07": return "warlock";
                case "HERO_07a": return "warlock";
                case "ICC_831": return "warlock";

                case "HERO_08": return "mage";
                case "HERO_08a": return "mage";
                case "HERO_08b": return "mage";
                case "ICC_833": return "mage";

                case "HERO_09": return "priest";
                case "HERO_09a": return "priest";
                case "HERO_10": return "demonhunter";
                case "HERO_10a": return "demonhunter";
                case "ICC_830": return "priest";
                case "EX1_323h": return "lordjaraxxus";
                case "BRM_027h": return "ragnarosthefirelord";
                default:
                    string retval = cdb.getCardDataFromID(cdb.cardIdstringToEnum(s)).nameCN.ToString();
                    return retval;
            }
        }

        /// <summary>
        /// 将英雄技能ID转换为英雄名称
        /// </summary>
        /// <param name="s">英雄技能ID</param>
        /// <returns>英雄名称</returns>
        public string heroPowerToName(string s)
        {
            if (s.StartsWith("HERO_01")) return "warrior";
            else if (s.StartsWith("HERO_02")) return "shaman";
            else if (s.StartsWith("HERO_03")) return "thief";
            else if (s.StartsWith("HERO_04")) return "pala";
            else if (s.StartsWith("HERO_05")) return "hunter";
            else if (s.StartsWith("HERO_06")) return "druid";
            else if (s.StartsWith("HERO_07")) return "warlock";
            else if (s.StartsWith("HERO_08")) return "mage";
            else if (s.StartsWith("HERO_09")) return "perist";
            else if (s.StartsWith("HERO_10")) return "demonhunter";
            else if (s.StartsWith("HERO_11")) return "deathknight";

            switch (s)
            {//添加英雄sim编号到名字的转换
                case "HERO_01": return "warrior";
                case "HERO_01a": return "warrior";
                case "ICC_834": return "warrior";
                case "HERO_02": return "shaman";
                case "HERO_02a": return "shaman";
                case "ICC_481": return "shaman";
                case "HERO_03": return "thief";
                case "HERO_03a": return "thief";
                case "ICC_827": return "thief";
                case "HERO_04": return "pala";
                case "HERO_04a": return "pala";
                case "HERO_04b": return "pala";
                case "ICC_829": return "pala";
                case "HERO_05": return "hunter";
                case "HERO_05a": return "hunter";
                case "ICC_828": return "hunter";
                case "HERO_06": return "druid";
                case "ICC_832": return "druid";
                case "HERO_07": return "warlock";
                case "HERO_07a": return "warlock";
                case "ICC_831": return "warlock";
                case "HERO_08": return "mage";
                case "HERO_08a": return "mage";
                case "HERO_08b": return "mage";
                case "ICC_833": return "mage";
                case "HERO_09": return "priest";
                case "HERO_09a": return "priest";
                case "HERO_10": return "demonhunter";
                case "HERO_10a": return "demonhunter";
                case "ICC_830": return "priest";
                case "EX1_323h": return "lordjaraxxus";
                case "BRM_027h": return "ragnarosthefirelord";
                default:
                    string retval = cdb.getCardDataFromID(cdb.cardIdstringToEnum(s)).nameCN.ToString();
                    return retval;
            }
        }
        /// <summary>
        /// 将英雄名称转换为英雄枚举
        /// 留牌文件就是用这个函数进行转换的，由此可见圣骑士的名称用pala和paladin是一样的
        /// </summary>
        /// <param name="s">英雄名称</param>
        /// <returns>英雄枚举</returns>
        public HeroEnum heroNametoEnum(string s)
        {
            switch (s)
            {
                case "weizbang": return HeroEnum.weizbang;
                case "druid": return HeroEnum.druid;
                case "hunter": return HeroEnum.hunter;
                case "mage": return HeroEnum.mage;
                case "法师": return HeroEnum.mage;
                case "pala": return HeroEnum.pala;
                case "paladin": return HeroEnum.pala;
                case "priest": return HeroEnum.priest;
                case "shaman": return HeroEnum.shaman;
                case "thief": return HeroEnum.thief;
                case "rogue": return HeroEnum.thief;
                case "maievshadowsong": return HeroEnum.thief;
                case "warlock": return HeroEnum.warlock;
                case "warrior": return HeroEnum.warrior;
                case "demonhunter": return HeroEnum.demonhunter;
                case "Illidanstormrage": return HeroEnum.demonhunter;
                case "lordjaraxxus": return HeroEnum.lordjaraxxus;
                case "ragnarosthefirelord": return HeroEnum.ragnarosthefirelord;
                case "deathknight": return HeroEnum.deathknight;
                default:
                    if (s.EndsWith("吉安娜"))
                        return HeroEnum.mage;
                    else if (s.EndsWith("雷克萨"))
                        return HeroEnum.hunter;
                    Helpfunctions.Instance.logg("异常，不认识敌方英雄:" + s);
                    return HeroEnum.None;
            }
        }
        /// <summary>
        /// 将英雄枚举转换为英雄职业
        /// </summary>
        /// <param name="he">英雄枚举</param>
        /// <returns>英雄职业</returns>
        public TAG_CLASS heroEnumtoTagClass(HeroEnum he)
        {
            switch (he)
            {
                case HeroEnum.druid: return TAG_CLASS.DRUID;
                case HeroEnum.hunter: return TAG_CLASS.HUNTER;
                case HeroEnum.mage: return TAG_CLASS.MAGE;
                case HeroEnum.pala: return TAG_CLASS.PALADIN;
                case HeroEnum.priest: return TAG_CLASS.PRIEST;
                case HeroEnum.shaman: return TAG_CLASS.SHAMAN;
                case HeroEnum.thief: return TAG_CLASS.ROGUE;
                case HeroEnum.warlock: return TAG_CLASS.WARLOCK;
                case HeroEnum.warrior: return TAG_CLASS.WARRIOR;
                case HeroEnum.demonhunter: return TAG_CLASS.DEMONHUNTER;
                case HeroEnum.deathknight: return TAG_CLASS.DEATHKNIGHT;
                case HeroEnum.weizbang: return TAG_CLASS.WHIZBANG;
                default: return TAG_CLASS.INVALID;
            }
        }
        /// <summary>
        /// 将英雄职业字符串转换为英雄枚举
        /// </summary>
        /// <param name="s">英雄职业字符串</param>
        /// <returns>英雄枚举</returns>
        public HeroEnum heroTAG_CLASSstringToEnum(string s)
        {
            switch (s)
            {
                case "DRUID": return HeroEnum.druid;
                case "HUNTER": return HeroEnum.hunter;
                case "MAGE": return HeroEnum.mage;
                case "PALADIN": return HeroEnum.pala;
                case "PRIEST": return HeroEnum.priest;
                case "SHAMAN": return HeroEnum.shaman;
                case "ROGUE": return HeroEnum.thief;
                case "WARLOCK": return HeroEnum.warlock;
                case "WARRIOR": return HeroEnum.warrior;
                default: return HeroEnum.None;
            }
        }

        /// <summary>
        /// 更新随从信息
        /// </summary>
        /// <param name="om">我方随从列表</param>
        /// <param name="em">敌方随从列表</param>
        public void updateMinions(List<Minion> om, List<Minion> em)
        {
            this.ownMinions.Clear();
            this.enemyMinions.Clear();
            foreach (Minion item in om)
            {
                this.ownMinions.Add(new Minion(item));
            }
            //this.ownMinions.AddRange(om);
            foreach (Minion item in em)
            {
                this.enemyMinions.Add(new Minion(item));
            }
            //this.enemyMinions.AddRange(em);

            //sort them 
            updatePositions();
        }

        /// <summary>
        /// 更新潜行随从数据库
        /// </summary>
        /// <param name="ldb">潜行随从数据</param>
        public void updateLurkersDB(Dictionary<int, IDEnumOwner> ldb)
        {
            this.LurkersDB.Clear();
            foreach (KeyValuePair<int, IDEnumOwner> lt in ldb)
            {
                this.LurkersDB.Add(lt.Key, lt.Value);
            }
        }

        /// <summary>
        /// 更新奥秘状态
        /// </summary>
        /// <param name="ownsecs">我方奥秘列表</param>
        /// <param name="numEnemSec">敌方奥秘数量</param>
        public void updateSecretStuff(List<string> ownsecs, int numEnemSec)
        {
            this.ownSecretList.Clear();
            foreach (string s in ownsecs)
            {
                this.ownSecretList.Add(cdb.cardIdstringToEnum(s));
            }
            this.enemySecretCount = numEnemSec;
        }

        /// <summary>
        /// 更新当前牌库状态
        /// </summary>
        /// <param name="deck">牌库卡牌及其数量</param>
        public void updateTurnDeck(Dictionary<CardDB.cardIDEnum, int> deck)
        {
            this.turnDeck.Clear();
            bool patchesidsk = false;
            bool noDupl = true;
            foreach (KeyValuePair<CardDB.cardIDEnum, int> c in deck)
            {
                this.turnDeck.Add(c.Key, c.Value);
                if (!patchesidsk && c.Key == CardDB.cardIDEnum.CFM_637)
                    patchesidsk = true;
                if (c.Value > 1)
                    noDupl = false;
            }
            this.noDuplicates = noDupl;
            this.patchesInDeck = patchesidsk;
            deckCardForCost.Clear();
        }
        /// <summary>
        /// 读取牌库中指定费用的卡牌
        /// </summary>
        /// <param name="cost">费用</param>
        /// <returns>卡牌ID枚举</returns>
        public CardDB.cardIDEnum getDeckCardsForCost(int cost)
        {
            if (deckCardForCost.Count == 0)
            {
                CardDB.Card c;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> cn in turnDeck)
                {
                    c = CardDB.Instance.getCardDataFromID(cn.Key);
                    if (!deckCardForCost.ContainsKey(c.cost)) deckCardForCost.Add(c.cost, c.cardIDenum);
                }
            }
            if (deckCardForCost.ContainsKey(cost)) return deckCardForCost[cost];
            else return CardDB.cardIDEnum.None;
        }
        /// <summary>
        /// 读取牌库中指定特性的卡牌数量
        /// 支持的特性：GAME_TAGs.TAUNT, GAME_TAGs.DIVINE_SHIELD, GAME_TAGs.LIFESTEAL, GAME_TAGs.WINDFURY, GAME_TAGs.SECRET
        /// </summary>
        /// <param name="tag">卡牌特性标签</param>
        /// <returns>具有该特性的卡牌数量</returns>
        public int numDeckCardsByTag(GAME_TAGs tag)
        {
            switch (tag)
            {
                case GAME_TAGs.TAUNT: if (numTauntCards > -1) return numTauntCards; break;
                case GAME_TAGs.DIVINE_SHIELD: if (numDivineShieldCards > -1) return numDivineShieldCards; break;
                case GAME_TAGs.LIFESTEAL: if (numLifestealCards > -1) return numLifestealCards; break;
                case GAME_TAGs.WINDFURY: if (numWindfuryCards > -1) return numWindfuryCards; break;
                case GAME_TAGs.SECRET: if (numSecretCards > -1) return numSecretCards; break;
            }
            numTauntCards = 0;
            numDivineShieldCards = 0;
            numLifestealCards = 0;
            numWindfuryCards = 0;
            numSecretCards = 0;

            CardDB.Card c;
            foreach (KeyValuePair<CardDB.cardIDEnum, int> cn in turnDeck)
            {
                c = CardDB.Instance.getCardDataFromID(cn.Key);
                if (c.tank) numTauntCards += cn.Value;
                if (c.Shield) numDivineShieldCards += cn.Value;
                if (c.lifesteal) numLifestealCards += cn.Value;
                if (c.windfury) numWindfuryCards += cn.Value;
                if (c.Secret) numSecretCards += cn.Value;
            }

            switch (tag)
            {
                case GAME_TAGs.TAUNT: return numTauntCards;
                case GAME_TAGs.DIVINE_SHIELD: return numDivineShieldCards;
                case GAME_TAGs.LIFESTEAL: return numLifestealCards;
                case GAME_TAGs.WINDFURY: return numWindfuryCards;
                case GAME_TAGs.SECRET: return numSecretCards;
            }
            return -1;
        }

        /// <summary>
        /// 更新玩家状态信息
        /// </summary>
        /// <param name="maxmana">最大法力值</param>
        /// <param name="currentmana">当前法力值</param>
        /// <param name="cardsplayedthisturn">本回合出牌数</param>
        /// <param name="numMinionsplayed">本回合出随从数</param>
        /// <param name="optionsPlayedThisTurn">本回合操作数</param>
        /// <param name="overload">过载水晶</param>
        /// <param name="lockedmana">锁定水晶</param>
        /// <param name="heroentity">我方英雄实体ID</param>
        /// <param name="enemyentity">敌方英雄实体ID</param>
        public void updatePlayer(int maxmana, int currentmana, int cardsplayedthisturn, int numMinionsplayed, int optionsPlayedThisTurn, int overload, int lockedmana, int heroentity, int enemyentity)
        {
            this.currentMana = currentmana;
            this.ownMaxMana = maxmana;
            this.cardsPlayedThisTurn = cardsplayedthisturn;
            this.numMinionsPlayedThisTurn = numMinionsplayed;
            this.ueberladung = overload;
            this.lockedMana = lockedmana;
            this.ownHeroEntity = heroentity;
            this.enemyHeroEntitiy = enemyentity;
            this.numOptionsPlayedThisTurn = optionsPlayedThisTurn;
        }

        /// <summary>
        /// 更新英雄初始职业
        /// </summary>
        /// <param name="ownHSClass">我方英雄初始职业</param>
        /// <param name="enemyHSClass">敌方英雄初始职业</param>
        public void updateHeroStartClass(TAG_CLASS ownHSClass, TAG_CLASS enemyHSClass)
        {
            this.ownHeroStartClass = ownHSClass;
            this.enemyHeroStartClass = enemyHSClass;
        }


        /// <summary>
        /// 更新英雄信息
        /// </summary>
        /// <param name="w">武器</param>
        /// <param name="heron">英雄名称</param>
        /// <param name="ability">英雄技能</param>
        /// <param name="abrdy">技能是否准备就绪</param>
        /// <param name="abCost">技能费用</param>
        /// <param name="hero">英雄实体</param>
        /// <param name="enMaxMana">敌方最大法力值（默认10）</param>
        public void updateHero(Weapon w, string heron, CardDB.Card ability, bool abrdy, int abCost, Minion hero, int enMaxMana = 10)
        {
            if (w.name == CardDB.cardNameEN.foolsbane) w.cantAttackHeroes = true;

            if (hero.own)
            {
                this.ownWeapon = new Weapon(w);

                this.ownHero = new Minion(hero);
                this.heroname = this.heroNametoEnum(heron);
                this.heronameingame = heron;
                if (this.ownHeroStartClass == TAG_CLASS.INVALID) this.ownHeroStartClass = hero.cardClass;
                this.ownHero.poisonous = this.ownWeapon.poisonous;
                this.ownHero.lifesteal = this.ownWeapon.lifesteal;
                if (this.ownWeapon.name == CardDB.cardNameEN.gladiatorslongbow) this.ownHero.immuneWhileAttacking = true;

                this.heroAbility = ability;
                this.ownHeroPowerCost = abCost;
                this.ownAbilityisReady = abrdy;
            }
            else
            {
                this.enemyWeapon = new Weapon(w);
                this.enemyHero = new Minion(hero); ;

                this.enemyHeroname = this.heroNametoEnum(heron);
                this.enemyHeronameingame = heron;
                if (this.enemyHeroStartClass == TAG_CLASS.INVALID) this.enemyHeroStartClass = enemyHero.cardClass;
                this.enemyHero.poisonous = this.enemyWeapon.poisonous;
                this.enemyHero.lifesteal = this.enemyWeapon.lifesteal;
                if (this.enemyWeapon.name == CardDB.cardNameEN.gladiatorslongbow) this.enemyHero.immuneWhileAttacking = true;

                this.enemyAbility = ability;
                this.enemyHeroPowerCost = abCost;

                this.enemyMaxMana = enMaxMana;
            }
        }

        /// <summary>
        /// 更新回合信息
        /// </summary>
        /// <param name="turn">回合数</param>
        /// <param name="step">回合内步骤</param>
        public void updateTurnInfo(int turn, int step)
        {
            this.gTurn = turn;
            this.gTurnStep = step;
        }

        /// <summary>
        /// 更新克苏恩信息
        /// </summary>
        /// <param name="OgOwnCThunAngrBonus">克苏恩攻击力加成</param>
        /// <param name="OgOwnCThunHpBonus">克苏恩血量加成</param>
        /// <param name="OgOwnCThunTaunt">克苏恩嘲讽状态</param>
        public void updateCThunInfo(int OgOwnCThunAngrBonus, int OgOwnCThunHpBonus, int OgOwnCThunTaunt)
        {
            this.anzOgOwnCThunAngrBonus = OgOwnCThunAngrBonus;
            this.anzOgOwnCThunHpBonus = OgOwnCThunHpBonus;
            this.anzOgOwnCThunTaunt = OgOwnCThunTaunt;
        }

        /// <summary>
        /// 更新疲劳状态
        /// </summary>
        /// <param name="ods">我方牌库大小</param>
        /// <param name="ohf">我方疲劳值</param>
        /// <param name="eds">敌方牌库大小</param>
        /// <param name="ehf">敌方疲劳值</param>
        public void updateFatigueStats(int ods, int ohf, int eds, int ehf)
        {
            this.ownDeckSize = ods;
            this.ownHeroFatigue = ohf;
            this.enemyDeckSize = eds;
            this.enemyHeroFatigue = ehf;
        }

        /// <summary>
        /// 更新随从位置
        /// </summary>
        public void updatePositions()
        {
            this.ownMinions.Sort((a, b) => a.zonepos.CompareTo(b.zonepos));
            this.enemyMinions.Sort((a, b) => a.zonepos.CompareTo(b.zonepos));
            int i = 0;
            foreach (Minion m in this.ownMinions)
            {
                i++;
                m.zonepos = i;

            }
            i = 0;
            foreach (Minion m in this.enemyMinions)
            {
                i++;
                m.zonepos = i;
            }

        }

        /// <summary>
        /// 更新发现的卡牌
        /// </summary>
        /// <param name="discoverCardsList">发现的卡牌列表</param>
        public void updateDiscoverCards(List<string> discoverCardsList)
        {
            if (discoverCardsList.Count == 4)
            {
                this.DiscoverCards.Clear();
                for (int i = 0; i < 3; i++)
                {
                    this.DiscoverCards.Add(i, cdb.cardIdstringToEnum(discoverCardsList[i + 1]));
                }
            }
        }

        /// <summary>
        /// 更新我方最后死亡的随从
        /// </summary>
        /// <param name="cid">最后死亡的随从卡牌ID</param>
        public void updateOwnLastDiedMinion(CardDB.cardIDEnum cid)
        {
            this.OwnLastDiedMinion = cid;
        }

        /// <summary>
        /// 创建新的随从实例
        /// </summary>
        /// <param name="hc">手牌</param>
        /// <param name="id">位置ID</param>
        /// <returns>新创建的随从</returns>
        private Minion createNewMinion(Handmanager.Handcard hc, int id)
        {
            Minion m = new Minion
            {
                handcard = new Handmanager.Handcard(hc),
                zonepos = id + 1,
                entitiyID = hc.entity,
                Angr = hc.card.Attack,
                Hp = hc.card.Health,
                maxHp = hc.card.Health,
                name = hc.card.nameEN,
                nameCN = hc.card.nameCN,
                playedThisTurn = true,
                numAttacksThisTurn = 0,
                CooldownTurn = hc.card.CooldownTurn
            };


            if (hc.card.windfury) m.windfury = true;
            if (hc.card.tank) m.taunt = true;
            if (hc.card.Charge)
            {
                m.Ready = true;
                m.charge = 1;
            }
            if (hc.card.Shield) m.divineshild = true;
            if (hc.card.poisonous) m.poisonous = true;
            if (hc.card.lifesteal) m.lifesteal = true;
            if (hc.card.reborn) m.reborn = true;
            if (hc.card.Rush) m.rush = 1;

            if (hc.card.Stealth) m.stealth = true;

            if (m.name == CardDB.cardNameEN.lightspawn && !m.silenced)
            {
                m.Angr = m.Hp;
            }


            return m;
        }
        /// <summary>
        /// 输出当前场面上的信息到日志
        /// </summary>
        public void printHero()
        {
            help.logg("player:");
            help.logg(this.numMinionsPlayedThisTurn + " " + this.cardsPlayedThisTurn + " " + this.ueberladung + " " + this.lockedMana + " " + this.ownPlayerController);

            help.logg("ownhero:");
            StringBuilder ownHeroEnchs = new StringBuilder(this.ownHero.enchs.Count > 0 ? " 附魔:" : "", 50);
            foreach (CardDB.cardIDEnum cardIDEnum in this.ownHero.enchs)
            {
                ownHeroEnchs.AppendFormat(" {0}", cardIDEnum.ToString());
            }
            help.logg((this.heroname == HeroEnum.None ? this.heronameingame : this.heroname.ToString()) + " " + this.ownHero.Hp + " " + this.ownHero.maxHp + " " + this.ownHero.armor + " " + this.ownHero.immuneWhileAttacking + " " + this.ownHero.immune + " " + this.ownHero.entitiyID + " " + this.ownHero.Ready + " " + this.ownHero.numAttacksThisTurn + " " + this.ownHero.frozen + " " + this.ownHero.Angr + " " + this.ownHero.tempAttack + " " + this.enemyHero.stealth
                + ownHeroEnchs.ToString()
                );
            help.logg("weapon: " + ownWeapon.Angr + " " + ownWeapon.Durability + " " + this.ownWeapon.card.nameCN + " " + this.ownWeapon.card.cardIDenum + " " + (this.ownWeapon.poisonous ? 1 : 0) + " " + (this.ownWeapon.lifesteal ? 1 : 0) + " " + this.ownWeapon.scriptNum1);
            help.logg("ability: " + this.ownAbilityisReady + " " + this.heroAbility.cardIDenum);
            string secs = "";
            foreach (CardDB.cardIDEnum sec in this.ownSecretList)
            {
                secs += sec + " ";
            }
            help.logg("osecrets: " + secs);
            help.logg("cthunbonus: " + this.anzOgOwnCThunAngrBonus + " " + this.anzOgOwnCThunHpBonus + " " + this.anzOgOwnCThunTaunt);
            help.logg("jadegolems: " + this.anzOwnJadeGolem + " " + this.anzEnemyJadeGolem);
            help.logg("elementals: " + this.anzOwnElementalsThisTurn + " " + this.anzOwnElementalsLastTurn + " " + this.ownElementalsHaveLifesteal);
            help.logg(Questmanager.Instance.getQuestsString());
            help.logg("advanced: " + this.ownCrystalCore + " " + (this.ownMinionsInDeckCost0 ? 1 : 0));
            help.logg("enemyhero:");
            StringBuilder enemyHeroEnchs = new StringBuilder(this.enemyHero.enchs.Count > 0 ? " 附魔:" : "", 50);
            foreach (CardDB.cardIDEnum cardIDEnum in this.enemyHero.enchs)
            {
                enemyHeroEnchs.AppendFormat(" {0}", cardIDEnum.ToString());
            }
            help.logg((this.enemyHeroname == HeroEnum.None ? this.enemyHeronameingame : this.enemyHeroname.ToString()) + " " + this.enemyHero.Hp + " " + this.enemyHero.maxHp + " " + this.enemyHero.armor + " " + this.enemyHero.frozen + " " + this.enemyHero.immune + " " + this.enemyHero.entitiyID + " " + this.enemyHero.stealth + (enemyHeroEnchs.ToString()));
            help.logg("weapon: " + this.enemyWeapon.Angr + " " + this.enemyWeapon.Durability + " " + this.enemyWeapon.card.nameCN + " " + this.enemyWeapon.card.cardIDenum + " " + (this.enemyWeapon.poisonous ? 1 : 0) + " " + (this.enemyWeapon.lifesteal ? 1 : 0) + " " + this.enemyWeapon.scriptNum1);
            help.logg("ability: " + "True" + " " + this.enemyAbility.cardIDenum);
            help.logg("fatigue: " + this.ownDeckSize + " " + this.ownHeroFatigue + " " + this.enemyDeckSize + " " + this.enemyHeroFatigue);
        }
        /// <summary>
        /// 输出单个随从信息到日志
        /// </summary>
        /// <param name="stringBuilder">字符串构建器</param>
        /// <param name="minion">随从</param>
        public void printOwnMinions(StringBuilder stringBuilder, Minion minion)
        {
            stringBuilder.AppendFormat("{0} ({1}/{2}) {3} zp: e: {4} A:{1} H:{2} mH:{5} rey:{6} natt:{7}", minion.handcard.card.nameCN, minion.Angr, minion.Hp, minion.zonepos, minion.entitiyID, minion.maxHp, minion.Ready, minion.numAttacksThisTurn);

            if (minion.exhausted) stringBuilder.Append(" ex");
            if (minion.taunt) stringBuilder.Append(" tnt");
            if (minion.frozen) stringBuilder.Append(" frz");
            if (minion.silenced) stringBuilder.Append(" silenced");
            if (minion.divineshild) stringBuilder.Append(" divshield");
            if (minion.Spellburst) stringBuilder.Append(" 法术迸发");
            if (minion.Frenzy) stringBuilder.Append(" 暴怒");
            if (minion.playedThisTurn) stringBuilder.Append(" ptt");
            if (minion.windfury) stringBuilder.Append(" wndfr");
            if (minion.stealth) stringBuilder.Append(" stlth");
            if (minion.poisonous) stringBuilder.Append(" poi");
            if (minion.lifesteal) stringBuilder.Append(" lfst");
            if (minion.immune) stringBuilder.Append(" imm");
            if (minion.untouchable) stringBuilder.Append(" untch");
            if (minion.conceal) stringBuilder.Append(" cncdl");
            if (minion.destroyOnOwnTurnStart) stringBuilder.Append(" dstrOwnTrnStrt");
            if (minion.destroyOnOwnTurnEnd) stringBuilder.Append(" dstrOwnTrnnd");
            if (minion.destroyOnEnemyTurnStart) stringBuilder.Append(" dstrEnmTrnStrt");
            if (minion.destroyOnEnemyTurnEnd) stringBuilder.Append(" dstrEnmTrnnd");
            if (minion.shadowmadnessed) stringBuilder.Append(" shdwmdnssd");
            if (minion.Elusive) stringBuilder.Append("elus");
            if (minion.cantLowerHPbelowONE) stringBuilder.Append(" cantLowerHpBelowOne");
            // if (m.cantBeTargetedBySpellsOrHeroPowers) mini += " cbtBySpells";

            if (minion.charge >= 1) stringBuilder.AppendFormat(" chrg({0})", minion.charge);
            if (minion.hChoice >= 1) stringBuilder.AppendFormat(" hChoice({0})", minion.hChoice);
            if (minion.AdjacentAngr >= 1) stringBuilder.AppendFormat(" adjaattk({0})", minion.AdjacentAngr);
            if (minion.tempAttack != 0) stringBuilder.AppendFormat(" tmpattck({0})", minion.tempAttack);
            if (minion.spellpower != 0) stringBuilder.AppendFormat(" spllpwr({0})", minion.spellpower);
            if (minion.ancestralspirit >= 1) stringBuilder.AppendFormat(" ancstrl({0})", minion.ancestralspirit);
            if (minion.desperatestand >= 1) stringBuilder.AppendFormat(" despStand({0})", minion.desperatestand);
            if (minion.ownBlessingOfWisdom >= 1) stringBuilder.AppendFormat(" ownBlssng({0})", minion.ownBlessingOfWisdom);
            if (minion.enemyBlessingOfWisdom >= 1) stringBuilder.AppendFormat(" enemyBlssng({0})", minion.enemyBlessingOfWisdom);
            if (minion.ownPowerWordGlory >= 1) stringBuilder.AppendFormat(" ownGlory({0})", minion.ownPowerWordGlory);
            if (minion.enemyPowerWordGlory >= 1) stringBuilder.AppendFormat(" enemyGlory({0})", minion.enemyPowerWordGlory);
            if (minion.souloftheforest >= 1) stringBuilder.AppendFormat(" souloffrst({0})", minion.souloftheforest);
            if (minion.stegodon >= 1) stringBuilder.AppendFormat(" stegodon({0})", minion.stegodon);
            if (minion.livingspores >= 1) stringBuilder.AppendFormat(" lspores({0})", minion.livingspores);
            if (minion.explorershat >= 1) stringBuilder.AppendFormat(" explHat({0})", minion.explorershat);
            if (minion.returnToHand >= 1) stringBuilder.AppendFormat(" retHand({0})", minion.returnToHand);
            if (minion.infest >= 1) stringBuilder.AppendFormat(" infest({0})", minion.infest);
            if (minion.deathrattle2 != null) stringBuilder.AppendFormat(" dethrl({0})", minion.deathrattle2.cardIDenum);
            if (minion.name == CardDB.cardNameEN.moatlurker && this.LurkersDB.ContainsKey(minion.entitiyID))
            {
                stringBuilder.AppendFormat(" respawn:{0}:{1}", this.LurkersDB[minion.entitiyID].IDEnum, this.LurkersDB[minion.entitiyID].own);
            }
            if (minion.handcard.card.type == CardDB.cardtype.LOCATION) stringBuilder.AppendFormat(" cooldownTurn:{0}", minion.CooldownTurn);
            if (minion.enchs.Count > 0)
            {
                StringBuilder miniEnchs = new StringBuilder(" 附魔:", 50);
                foreach (CardDB.cardIDEnum cardIDEnum in minion.enchs)
                {
                    miniEnchs.AppendFormat(" {0}", cardIDEnum.ToString());
                }
                stringBuilder.Append(miniEnchs.ToString());
            }
            help.logg(stringBuilder.ToString());
        }
        /// <summary>
        /// 输出我方所有随从信息到日志
        /// </summary>
        public void printOwnMinions()
        {
            help.logg("OwnMinions:");
            foreach (Minion m in this.ownMinions)
            {
                StringBuilder minionStringBuilder = new StringBuilder(120);
                printOwnMinions(minionStringBuilder, m);
                // string mini = m.handcard.card.nameCN + "(" + m.Angr + "/" + m.Hp + ")" + " " + m.handcard.card.cardIDenum + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready + " natt:" + m.numAttacksThisTurn;
                // if (m.exhausted) mini += " ex";
                // if (m.taunt) mini += " tnt";
                // if (m.frozen) mini += " frz";
                // if (m.silenced) mini += " silenced";
                // if (m.divineshild) mini += " divshield";
                // if (m.Spellburst) mini += " 法术迸发";
                // if (m.Frenzy) mini += " 暴怒";
                // if (m.playedThisTurn) mini += " ptt";
                // if (m.windfury) mini += " wndfr";
                // if (m.stealth) mini += " stlth";
                // if (m.poisonous) mini += " poi";
                // if (m.lifesteal) mini += " lfst";
                // if (m.immune) mini += " imm";
                // if (m.untouchable) mini += " untch";
                // if (m.conceal) mini += " cncdl";
                // if (m.destroyOnOwnTurnStart) mini += " dstrOwnTrnStrt";
                // if (m.destroyOnOwnTurnEnd) mini += " dstrOwnTrnnd";
                // if (m.destroyOnEnemyTurnStart) mini += " dstrEnmTrnStrt";
                // if (m.destroyOnEnemyTurnEnd) mini += " dstrEnmTrnnd";
                // if (m.shadowmadnessed) mini += " shdwmdnssd";
                // if (m.cantLowerHPbelowONE) mini += " cantLowerHpBelowOne";
                // // if (m.cantBeTargetedBySpellsOrHeroPowers) mini += " cbtBySpells";
                // if (m.Elusive) mini += "elus";
                // if (m.charge >= 1) mini += " chrg(" + m.charge + ")";
                // if (m.hChoice >= 1) mini += " hChoice(" + m.hChoice + ")";
                // if (m.AdjacentAngr >= 1) mini += " adjaattk(" + m.AdjacentAngr + ")";
                // if (m.tempAttack != 0) mini += " tmpattck(" + m.tempAttack + ")";
                // if (m.spellpower != 0) mini += " spllpwr(" + m.spellpower + ")";

                // if (m.ancestralspirit >= 1) mini += " ancstrl(" + m.ancestralspirit + ")";
                // if (m.desperatestand >= 1) mini += " despStand(" + m.desperatestand + ")";
                // if (m.ownBlessingOfWisdom >= 1) mini += " ownBlssng(" + m.ownBlessingOfWisdom + ")";
                // if (m.enemyBlessingOfWisdom >= 1) mini += " enemyBlssng(" + m.enemyBlessingOfWisdom + ")";
                // if (m.ownPowerWordGlory >= 1) mini += " ownGlory(" + m.ownPowerWordGlory + ")";
                // if (m.enemyPowerWordGlory >= 1) mini += " enemyGlory(" + m.enemyPowerWordGlory + ")";
                // if (m.souloftheforest >= 1) mini += " souloffrst(" + m.souloftheforest + ")";
                // if (m.stegodon >= 1) mini += " stegodon(" + m.stegodon + ")";
                // if (m.livingspores >= 1) mini += " lspores(" + m.livingspores + ")";
                // if (m.explorershat >= 1) mini += " explHat(" + m.explorershat + ")";
                // if (m.returnToHand >= 1) mini += " retHand(" + m.returnToHand + ")";
                // if (m.infest >= 1) mini += " infest(" + m.infest + ")";
                // if (m.deathrattle2 != null) mini += " dethrl(" + m.deathrattle2.cardIDenum + ")";
                // if (m.name == CardDB.cardNameEN.moatlurker && this.LurkersDB.ContainsKey(m.entitiyID))
                // {
                //     mini += " respawn:" + this.LurkersDB[m.entitiyID].IDEnum + ":" + this.LurkersDB[m.entitiyID].own;
                // }
                // if (m.handcard.card.type == CardDB.cardtype.LOCATION) mini += " cooldownTurn:" + m.CooldownTurn;
                // if (m.enchs.Count > 0)
                // {
                //     StringBuilder miniEnchs = new StringBuilder(" 附魔:", 50);
                //     foreach (CardDB.cardIDEnum cardIDEnum in m.enchs)
                //     {
                //         miniEnchs.AppendFormat(" {0}", cardIDEnum.ToString());
                //     }
                //     mini += miniEnchs.ToString();
                // }
                // help.logg(mini);
            }

        }
        /// <summary>
        /// 输出敌方所有随从信息到日志
        /// </summary>
        public void printEnemyMinions()
        {
            help.logg("EnemyMinions:");
            foreach (Minion m in this.enemyMinions)
            {
                StringBuilder minionStringBuilder = new StringBuilder(120);
                printOwnMinions(minionStringBuilder, m);
                // string mini = m.handcard.card.nameCN + "(" + m.Angr + "/" + m.Hp + ")" + " " + m.handcard.card.cardIDenum + " zp:" + m.zonepos + " e:" + m.entitiyID + " A:" + m.Angr + " H:" + m.Hp + " mH:" + m.maxHp + " rdy:" + m.Ready;// +" natt:" + m.numAttacksThisTurn;
                // if (m.exhausted) mini += " ex";
                // if (m.taunt) mini += " tnt";
                // if (m.frozen) mini += " frz";
                // if (m.silenced) mini += " silenced";
                // if (m.divineshild) mini += " divshield";
                // if (m.Spellburst) mini += " 法术迸发";
                // if (m.Frenzy) mini += " 暴怒";
                // if (m.playedThisTurn) mini += " ptt";
                // if (m.windfury) mini += " wndfr";
                // if (m.stealth) mini += " stlth";
                // if (m.poisonous) mini += " poi";
                // if (m.lifesteal) mini += " lfst";
                // if (m.immune) mini += " imm";
                // if (m.untouchable) mini += " untch";
                // if (m.conceal) mini += " cncdl";
                // if (m.destroyOnOwnTurnStart) mini += " dstrOwnTrnStrt";
                // if (m.destroyOnOwnTurnEnd) mini += " dstrOwnTrnnd";
                // if (m.destroyOnEnemyTurnStart) mini += " dstrEnmTrnStrt";
                // if (m.destroyOnEnemyTurnEnd) mini += " dstrEnmTrnnd";
                // if (m.shadowmadnessed) mini += " shdwmdnssd";
                // if (m.cantLowerHPbelowONE) mini += " cantLowerHpBelowOne";
                // // if (m.cantBeTargetedBySpellsOrHeroPowers) mini += " cbtBySpells";
                // if (m.Elusive) mini += "elus";

                // if (m.charge >= 1) mini += " chrg(" + m.charge + ")";
                // if (m.hChoice >= 1) mini += " hChoice(" + m.hChoice + ")";
                // if (m.AdjacentAngr >= 1) mini += " adjaattk(" + m.AdjacentAngr + ")";
                // if (m.tempAttack != 0) mini += " tmpattck(" + m.tempAttack + ")";
                // if (m.spellpower != 0) mini += " spllpwr(" + m.spellpower + ")";

                // if (m.ancestralspirit >= 1) mini += " ancstrl(" + m.ancestralspirit + ")";
                // if (m.desperatestand >= 1) mini += " despStand(" + m.desperatestand + ")";
                // if (m.ownBlessingOfWisdom >= 1) mini += " ownBlssng(" + m.ownBlessingOfWisdom + ")";
                // if (m.enemyBlessingOfWisdom >= 1) mini += " enemyBlssng(" + m.enemyBlessingOfWisdom + ")";
                // if (m.ownPowerWordGlory >= 1) mini += " ownGlory(" + m.ownPowerWordGlory + ")";
                // if (m.enemyPowerWordGlory >= 1) mini += " enemyGlory(" + m.enemyPowerWordGlory + ")";
                // if (m.souloftheforest >= 1) mini += " souloffrst(" + m.souloftheforest + ")";
                // if (m.stegodon >= 1) mini += " stegodon(" + m.stegodon + ")";
                // if (m.livingspores >= 1) mini += " lspores(" + m.livingspores + ")";
                // if (m.explorershat >= 1) mini += " explHat(" + m.explorershat + ")";
                // if (m.returnToHand >= 1) mini += " retHand(" + m.returnToHand + ")";
                // if (m.infest >= 1) mini += " infest(" + m.infest + ")";
                // if (m.deathrattle2 != null) mini += " dethrl(" + m.deathrattle2.cardIDenum + ")";
                // if (m.name == CardDB.cardNameEN.moatlurker && this.LurkersDB.ContainsKey(m.entitiyID))
                // {
                //     mini += " respawn:" + this.LurkersDB[m.entitiyID].IDEnum + ":" + this.LurkersDB[m.entitiyID].own;
                // }
                // if (m.handcard.card.type == CardDB.cardtype.LOCATION) mini += " cooldownTurn:" + m.CooldownTurn;

                // if (m.enchs.Count > 0)
                // {
                //     StringBuilder miniEnchs = new StringBuilder(" 附魔:", 50);
                //     foreach (CardDB.cardIDEnum cardIDEnum in m.enchs)
                //     {
                //         miniEnchs.AppendFormat(" {0}", cardIDEnum.ToString());
                //     }
                //     mini += miniEnchs.ToString();
                // }
                // help.logg(mini);
            }

        }
        /// <summary>
        /// 输出我方牌库信息到日志（兄弟的牌库读取时好时坏）
        /// </summary>
        public void printOwnDeck()
        {
            string od = "od: ";
            foreach (KeyValuePair<CardDB.cardIDEnum, int> e in this.turnDeck)
            {
                od += e.Key + "," + e.Value + ";";
            }
            Helpfunctions.Instance.logg(od);
        }

    }

}