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
    /// <summary>
    /// todo save "started" variables outside (they doesnt change)
    /// 记录战场数据 标记
    /// </summary>
    public partial class Playfield
    {
        private static readonly ILog ilog_0 = Logger.GetLoggerInstanceForType();

        public bool loatheb = false; //标志洛欧塞布效果是否生效
        public int loathebEffect = 0; //洛欧塞布效果的法力值增加量
        public List<Handmanager.Handcard> enemyHand = new List<Handmanager.Handcard>();//用于存储敌方手牌的字段，这是一个 List 类型的字段，用于存储敌方当前手牌中的所有卡牌对象
        public List<CardDB.Card> ownDeck = new List<CardDB.Card>();//新增ownDeck字段，用于记录玩家的牌库
        public Dictionary<CardDB.cardIDEnum, int> ownGraveyard = new Dictionary<CardDB.cardIDEnum, int>();//坟场，已死亡随从，不包括弃牌
        public List<CardDB.cardIDEnum> ownSecretsIDList = new List<CardDB.cardIDEnum>();
        public List<SecretItem> enemySecretList = new List<SecretItem>();
        public Dictionary<CardDB.cardIDEnum, int> enemyCardsOut = null;
        public List<Playfield> nextPlayfields = new List<Playfield>();
        public List<Minion> ownMinions = new List<Minion>();
        public List<Minion> enemyMinions = new List<Minion>();
        public List<GraveYardItem> diedMinions = null;
        public Dictionary<int, IDEnumOwner> LurkersDB = new Dictionary<int, IDEnumOwner>();
        public List<Handmanager.Handcard> owncards = new List<Handmanager.Handcard>();//玩家手牌
        public List<Action> playactions = new List<Action>();
        public List<int> pIdHistory = new List<int>();

        public string name = "";
        public string enemyGuessDeck = "";
        public bool logging = false;
        public bool complete = false;
        public int bestBoard = 0;
        public bool dirtyTwoTurnSim = false;
        public bool checkLostAct = false;
        public int totalAngr = -1;//场攻
        public int enemyTotalAngr = -1;
        public bool patchesInDeck = true;//可能有帕奇斯
        public int ownMinionsDied = 0;
        public bool anzSolor = false;//日蚀
        public int ownMinionStartCount = 0;//回合开始时己方随从数
        public int enemyMinionStartCount = 0;//回合开始时敌方随从数
        public int healOrDamageTimes = 0;//本回合治疗或受伤次数，巨人用
        public int healTimes = 0;
        public bool setMankrik = false;//曼科里克
        public int deckAngrBuff = 0;//牌库攻击BUFF
        public int deckHpBuff = 0;//牌库生命值BUFF
        public int nextEntity = 70;
        //第几个操作的场面
        public int pID = 0;
        public bool isLethalCheck = false;
        public int enemyTurnsCount = 0;
        public triggerCounter tempTrigger = new triggerCounter();
        public Hrtprozis prozis = Hrtprozis.Instance;//对局信息
        public int gTurn = 0;
        public int gTurnStep = 0;
        //aura minions########################## 光环随从
        //anz开头的都是某某随从的数量
        public int AnzSoulFragments = 0;// 灵魂残片
        public int anzOwnRaidleader = 0;//团队领袖
        public int anzEnemyRaidleader = 0;
        public int anzOwnVessina = 0;//维西纳
        public int anzEnemyVessina = 0;
        public int anzOwnStormwindChamps = 0;//暴风城勇士
        public int anzEnemyStormwindChamps = 0;
        public int anzOwnWarhorseTrainer = 0;//战马训练师
        public int anzEnemyWarhorseTrainer = 0;
        public int anzOwnTundrarhino = 0;//苔原犀牛
        public int anzEnemyTundrarhino = 0;
        public int anzOwnMrSmite = 0;//重拳先生
        public int anzEnemyMrSmite = 0;
        public int anzOwnTimberWolfs = 0;//森林狼
        public int anzEnemyTimberWolfs = 0;
        public int anzOwnMurlocWarleader = 0;//鱼人领军
        public int anzEnemyMurlocWarleader = 0;
        public int anzAcidmaw = 0;//酸喉
        public int anzOwnGrimscaleOracle = 0;//暗鳞先知
        public int anzEnemyGrimscaleOracle = 0;
        public int anzOwnShadowfiend = 0;//暗影魔
        public int anzOwnAuchenaiSoulpriest = 0;//奥金尼灵魂祭司
        public int anzEnemyAuchenaiSoulpriest = 0;
        public int anzOwnSouthseacaptain = 0;//南海船长
        public int anzEnemySouthseacaptain = 0;
        public int anzOwnMalGanis = 0;//玛尔加尼斯
        public int anzEnemyMalGanis = 0;
        public int anzOwnPiratesStarted = 0;//回合开始海盗数
        public int anzOwnMurlocStarted = 0;//回合开始鱼人数
        public int anzOwnElementStarted = 0;//回合开始元素数
        public int anzOwnDraeneiStarted = 0;//回合开始德莱尼数
        public int anzOwnTreantStarted = 0;//回合开始树人数
        public int anzOwnChromaggus = 0;//克洛玛古斯
        public int anzEnemyChromaggus = 0;
        public int anzOwnDragonConsort = 0;//龙王配偶
        public int anzOwnMurlocConsort = 0;
        public int anzOwnDragonConsortStarted = 0;
        public int ownElementCost = 0;//火光元素
        public int ownElementCostStarted = 0;
        public int ownBeastCostLessOnce = 0;//雷矛军用山羊
        public int ownBeastCostLessOnceStarted = 0;
        public int anzOwnBolfRamshield = 0;//博尔夫·碎盾
        public int anzEnemyBolfRamshield = 0;
        public int anzOwnHorsemen = 0;
        public int anzEnemyHorsemen = 0;
        public int anzOwnAnimatedArmor = 0;//复活的铠甲
        public int anzEnemyAnimatedArmor = 0;
        public int anzMoorabi = 0;//莫拉比
        public int anzDark = 0;//黑眼
        public int anzTamsinroame = 0;//塔姆辛罗姆
        public int anzOldWoman = 0;//虚触侍从
        public bool anzTamsin = false;//术士任务线 SW_091
        public int anzUsedOwnHeroPower = 0;
        public int anzUsedEnemyHeroPower = 0;
        public int anzOwnExtraAngrHp = 0;
        public int anzEnemyExtraAngrHp = 0;
        public int anzOwnMechwarper = 0;//机械跃迁者
        public int anzOwnMechwarperStarted = 0;
        public int anzEnemyMechwarper = 0;
        public int anzEnemyMechwarperStarted = 0;
        public int anzOgOwnCThun = 0; //克苏恩
        public int anzOgOwnCThunHpBonus = 0;
        public int anzOgOwnCThunAngrBonus = 0;
        public int anzOgOwnCThunTaunt = 0;
        public int anzOwnJadeGolem = 0;//青玉魔像
        public int anzEnemyJadeGolem = 0;
        public int anzOwnElementalsThisTurn = 0;  //这回合使用过元素
        public int anzOwnElementalsLastTurn = 0;  //上回合使用过元素
        public int anzEnemyTaunt = 0;
        public int anzOwnTaunt = 0;
        //#########################################

        public int usedUndeadAllies = 0;//不死盟军
        public int ownAbilityFreezesTarget = 0;
        public int enemyAbilityFreezesTarget = 0;
        public int ownHeroPowerCostLessOnce = 0;//英雄技能消耗更小一次
        public int enemyHeroPowerCostLessOnce = 0;
        public int ownHeroPowerCostLessTwice = 0;//英雄技能消耗更小两次
        public int ownDemonCostLessOnce = 0;//恶魔牌的法力值消耗减少
        public int ownHeroPowerExtraDamage = 0;//英雄技能会额外造成伤害
        public int enemyHeroPowerExtraDamage = 0;
        public int ownHeroPowerAllowedQuantity = 1;//英雄技能可以使用次数价值
        public int enemyHeroPowerAllowedQuantity = 1;
        public int useNature = 0;//特定牌在手中时触发是否使用过自然法术
        public int blackwaterpirate = 0;//黑水海盗
        public int blackwaterpirateStarted = 0;
        public int embracetheshadow = 0;//暗影之握
        public int ownCrystalCore = 0;//水晶核心
        public bool ownMinionsInDeckCost0 = false;//“践踏者”班纳布斯，牌库中所有随从的法力值消耗变为（0）点
        public bool LothraxionsPower = false;//使白银之手新兵获得圣盾标志位
        public int ownMinionsDiedTurn = 0;//本回合己方随从死亡的数量
        public int enemyMinionsDiedTurn = 0;//本回合敌方随从死亡的数量
        public bool feugenDead = false;//费尔根
        public bool stalaggDead = false;//斯塔拉格
        public bool weHavePlayedMillhouseManastorm = false;//米尔豪斯·法力风暴
        public bool weHaveSteamwheedleSniper = false;//热砂港狙击手
        public bool enemyHaveSteamwheedleSniper = false;
        public bool ownSpiritclaws = false;//幽灵之爪
        public bool enemySpiritclaws = false;
        public bool needGraveyard = false;//随从死亡
        public int doublepriest = 0;//先知维伦
        public int enemydoublepriest = 0;
        public int ownMistcaller = 0;//唤雾者伊戈瓦尔
        public int lockandload = 0;//子弹上膛
        public int stampede = 0;//奔踏
        public int ownBaronRivendare = 0;//瑞文戴尔男爵
        public int enemyBaronRivendare = 0;
        public int ownBrannBronzebeard = 0;//友方战吼额外触发次数
        public int enemyBrannBronzebeard = 0;//敌方战吼额外触发次数
        public int ownTurnEndEffectsTriggerTwice = 0;//己方达卡莱附魔师，回合结束效果会生效两次
        public int enemyTurnEndEffectsTriggerTwice = 0; //敌方达卡莱附魔师，回合结束效果会生效两次
        public int ownFandralStaghelm = 0;//范达尔·鹿盔
        public int tempanzOwnCards = 0; //手牌数 地精工兵
        public int tempanzEnemyCards = 0;//手牌数 地精工兵
        public bool isOwnTurn = true; //是我的回合
        public int turnCounter = 0;
        public bool attacked = false;// 标记已执行攻击
        public int attackFaceHP = 15;//打脸血量
        public int ownController = 0;
        public int evaluatePenality = 0;
        public int ruleWeight = 0;
        public string rulesUsed = "";
        public bool useSecretsPlayAround = true;
        public bool print = false;
        public Int64 hashcode = 0;
        public float value = Int32.MinValue;
        public int guessingHeroHP = 30;
        public float doDirtyTts = 100000;
        public int mana = 0;
        public int manaTurnEnd = 0;
        public int enemySecretCount = 0;
        public Minion ownHero;
        public Minion enemyHero;
        public HeroEnum ownHeroName = HeroEnum.None;
        public HeroEnum enemyHeroName = HeroEnum.None;
        public TAG_CLASS ownHeroStartClass = TAG_CLASS.INVALID;
        public TAG_CLASS enemyHeroStartClass = TAG_CLASS.INVALID;
        public Weapon ownWeapon = new Weapon();// 玩家当前装备的武器
        public Weapon enemyWeapon = new Weapon();// 玩家当前装备的武器
        public Questmanager.QuestItem ownQuest = new Questmanager.QuestItem();
        public Questmanager.QuestItem enemyQuest = new Questmanager.QuestItem();
        public Questmanager.QuestItem sideQuest = new Questmanager.QuestItem();
        public int owncarddraw = 0;
        public int enemycarddraw = 0;
        public int enemyAnzCards = 0;
        public int libram = 0;//圣契指示物
        public int spellpower = 0;
        public int spellpowerStarted = 0;//回合开始时的法强
        public int enemyspellpower = 0;
        public int enemyspellpowerStarted = 0;
        public int wehaveCounterspell = 0;
        public int lethlMissing = 1000;
        public bool nextSecretThisTurnCost0 = false;
        public bool playedPreparation = false;
        public bool nextSpellThisTurnCost0 = false;
        public bool nextMurlocThisTurnCostHealth = false;
        public bool nextSpellThisTurnCostHealth = false;
        public bool nextAnyCardThisTurnCostEnemyHealth = false;
        public int winzigebeschwoererin = 0;
        public int startedWithWinzigebeschwoererin = 0;
        public int winRazormaneBattleguard = 0;//钢鬃卫兵
        public int startedRazormaneBattleguard = 0;
        public int managespenst = 0;
        public int startedWithManagespenst = 0;
        public int ownMinionsCostMore = 0;
        public int ownMinionsCostMoreAtStart = 0;
        public int ownSpelsCostMore = 0;
        public int ownSpelsCostMoreAtStart = 0;
        public int ownDRcardsCostMore = 0;
        public int ownDRcardsCostMoreAtStart = 0;
        public int beschwoerungsportal = 0;
        public int startedWithbeschwoerungsportal = 0;
        public int myCardsCostLess = 0;
        public int startedWithmyCardsCostLess = 0;
        public int anzOwnAviana = 0; //艾维娜
        public int anzOwnScargil = 0; //斯卡基尔
        public int anzOwnCloakedHuntress = 0;//神秘女猎手
        public int anzOwnRazorfenRockstar = 0; // 剃刀沼泽摇滚明星
        public int anzOwnXB931Housekeeper = 0; // XB-931型家政机
        public int anzOwnPopGarThePutrid = 0;//我方腐臭淤泥波普加
        public int anzEnemyPopGarThePutrid = 0;//敌方腐臭淤泥波普加

        public int nerubarweblord = 0;
        public int startedWithnerubarweblord = 0;
        public bool startedWithDamagedMinions = false; //重碾
        public int ownWeaponAttackStarted = 0;
        public int ownMobsCountStarted = 0;
        public int ownCardsCountStarted = 0;
        public int enemyMobsCountStarted = 0;
        public int enemyCardsCountStarted = 0;
        public int ownHeroHpStarted = 30;
        public int enemyHeroHpStarted = 30;
        public int mobsplayedThisTurn = 0;
        public int startedWithMobsPlayedThisTurn = 0;
        public int spellsplayedSinceRecalc = 0;
        public int secretsplayedSinceRecalc = 0;
        public int optionsPlayedThisTurn = 0;
        public int cardsPlayedThisTurn = 0;
        public int ueberladung = 0;
        public int lockedMana = 0;
        public int enemyOptionsDoneThisTurn = 0;

        /// <summary>我方法力水晶上限</summary>
        public int ownMaxResources = 10;

        /// <summary>我方手牌上限</summary>
        public int ownMaxHandSize = 10;

        /// <summary>敌方法力水晶上限</summary>
        public int enemyMaxResources = 10;

        /// <summary>敌方手牌上限</summary>
        public int enemyMaxHandSize = 10;

        /// <summary>我方最大水晶</summary>
        public int ownMaxMana = 0;
        public int enemyMaxMana = 0;
        public int lostDamage = 0;
        public int lostHeal = 0;
        public int lostWeaponDamage = 0;
        public int ownDeckSize = 30;
        public int enemyDeckSize = 30;
        public int ownHeroFatigue = 0;
        public int enemyHeroFatigue = 0;
        public bool ownAbilityReady = false;
        public Handmanager.Handcard ownHeroAblility;
        public bool enemyAbilityReady = false;
        public Handmanager.Handcard enemyHeroAblility;
        public Playfield bestEnemyPlay = null;
        public Playfield endTurnState = null;
        public CardDB.cardIDEnum revivingOwnMinion = CardDB.cardIDEnum.None;//只是为了保存用奥秘复活的随从
        public CardDB.cardIDEnum revivingEnemyMinion = CardDB.cardIDEnum.None;
        public CardDB.cardIDEnum OwnLastDiedMinion = CardDB.cardIDEnum.None;
        public int shadowmadnessed = 0; //这一回合随从更换了控制权
        public int enemyHeroTurnStartedHp = 0;
        public int ownHeroTurnStartedHp = 0;
        public List<CardDB.cardIDEnum> sigilsToTriggerOnOwnTurnStart = new List<CardDB.cardIDEnum>();

        //法术派系
        public Dictionary<TAG_SPELL_SCHOOL, int> ownSpellSchoolCounts;
        //伊丽扎·刺刃光环
        public int ownElizagoreblade = 0;
        //回合结束存放的标志
        public List<CardDB.cardIDEnum> cardsToReturnAtEndOfTurn = new List<CardDB.cardIDEnum>();
        //救生光环
        public int ownSunscreenTurns = 0;
        public int enemySunscreenTurns = 0;
        //使用过的亡语牌
        public List<CardDB.cardIDEnum> ownPlayedDeathrattleCards = new List<CardDB.cardIDEnum>();
        public List<CardDB.cardIDEnum> enemyPlayedDeathrattleCards = new List<CardDB.cardIDEnum>();
        //重封者拉兹
        public bool ownHeroPowerCanBeUsedMultipleTimes = false;
        //使用过的随从牌
        public List<CardDB.cardIDEnum> ownMinionsPlayedThisGame = new List<CardDB.cardIDEnum>();
        //当前发掘次数
        public int excavationCount = 0;
        //总发掘次数
        public int allExcavationCount = 0;
        //下个战吼触发次数
        public int nextBattlecryTriggers = 1;
        //用于跟踪己方圣物是否需要双重施放
        public bool ownRelicDoubleCast = false;
        //用于跟踪敌方圣物是否需要双重施放
        public bool enemyRelicDoubleCast = false;
        //标志：下一个召唤的随从是否应该变为5/5
        public bool nextMinionBecomesFiveFive = false;
        //临时的法强
        public int tempSpellPower = 0;
        //下一张抉择牌同时拥有两种效果
        public bool nextChooseOneHasBothEffects = false;
        //鹦鹉乐园，战吼随从牌的法力值消耗减少
        public int parrotSanctuaryCount = 0;
        //小玩物小屋，记录手牌中的最后一张牌的实体ID
        public int lastDrawnCardEntityID = -1;
        //军团进攻
        public bool ownLegionInvasion = false;
        public bool enemyLegionInvasion = false;
        //维和者阿米图斯
        public bool ownAmitusThePeacekeeper = false;
        public bool enemyAmitusThePeacekeeper = false;
        //本回合对敌方英雄造成的伤害
        public int damageDealtToEnemyHeroThisTurn = 0;
        //下一张元素随从牌的法力值消耗减少量
        public int nextElementalReduction = 0;
        //本回合下一张元素随从牌的法力值消耗减少量
        public int thisTurnNextElementalReduction = 0;
        //上次打出的卡牌的费用
        public int lastPlayedCardCost = 0;
        //在本回合是否打出了元素牌
        public bool playedElementalThisTurn = false;
        //本回合打出的元素随从数量
        public int ownElementalsPlayedThisTurn = 0;

        /// <summary>
        /// 当前回合第0步操作或重新计算时会读取场面获得初始的 p，光环效果要初始化
        /// </summary>
        public Playfield()
        {
            this.value = int.MinValue;
            this.deckAngrBuff = 0;
            this.deckHpBuff = 0;
            this.patchesInDeck = true;
            this.healOrDamageTimes = 0;
            this.healTimes = 0;
            this.totalAngr = -1;
            this.enemyTotalAngr = -1;
            this.ownMinionsDied = 0;
            this.setMankrik = false;
            this.anzSolor = false;
            this.enemyMinionStartCount = 0;
            this.pID = prozis.getPid();
            if (this.print)
            {
                this.pIdHistory.Add(pID);
            }
            this.nextEntity = 1000;
            this.isLethalCheck = false;
            this.ownController = prozis.getOwnController();
            this.libram = 0;
            this.gTurn = (prozis.gTurn + 1) / 2;
            this.gTurnStep = prozis.gTurnStep;
            this.mana = prozis.currentMana;
            this.manaTurnEnd = this.mana;
            this.ownMaxMana = prozis.ownMaxMana;
            this.enemyMaxMana = prozis.enemyMaxMana;
            this.evaluatePenality = 0;
            this.ruleWeight = 0;
            this.rulesUsed = "";
            this.ownSecretsIDList.AddRange(prozis.ownSecretList);
            this.enemySecretCount = prozis.enemySecretCount;
            this.anzOgOwnCThunAngrBonus = prozis.anzOgOwnCThunAngrBonus;
            this.anzOgOwnCThunHpBonus = prozis.anzOgOwnCThunHpBonus;
            this.anzOgOwnCThunTaunt = prozis.anzOgOwnCThunTaunt;
            this.anzOwnJadeGolem = prozis.anzOwnJadeGolem;
            this.anzEnemyJadeGolem = prozis.anzEnemyJadeGolem;
            this.OwnLastDiedMinion = prozis.OwnLastDiedMinion;
            this.anzOwnElementalsThisTurn = prozis.anzOwnElementalsThisTurn;
            this.anzOwnElementalsLastTurn = prozis.anzOwnElementalsLastTurn;
            this.useNature = prozis.useNature;
            this.attackFaceHP = prozis.attackFaceHp;
            this.complete = false;
            this.ownHero = new Minion(prozis.ownHero);
            this.enemyHero = new Minion(prozis.enemyHero);
            this.ownWeapon = new Weapon(prozis.ownWeapon);
            this.enemyWeapon = new Weapon(prozis.enemyWeapon);
            this.AnzSoulFragments = prozis.turnDeck.ContainsKey(CardDB.cardIDEnum.SCH_307t) ? prozis.turnDeck[CardDB.cardIDEnum.SCH_307t] : 0;
            this.anzTamsin = prozis.anzTamsin;

            foreach (var item in Probabilitymaker.Instance.ownGraveyard)
            {
                if (!this.ownGraveyard.ContainsKey(item.Key) && CardDB.Instance.getCardDataFromID(item.Key).type == CardDB.cardtype.MOB)
                {
                    this.ownGraveyard.Add(item.Key, item.Value);
                }
            }

            addMinionsReal(prozis.ownMinions, ownMinions);
            addMinionsReal(prozis.enemyMinions, enemyMinions);
            addCardsReal(Handmanager.Instance.handCards);
            this.LurkersDB = new Dictionary<int, IDEnumOwner>(prozis.LurkersDB);

            this.enemySecretList.Clear();
            this.useSecretsPlayAround = true;
            foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets) // 敌方可能的奥秘
            {
                this.enemySecretList.Add(new SecretItem(si));
            }

            this.ownHeroName = prozis.heroname;
            this.enemyHeroName = prozis.enemyHeroname;
            this.ownHeroStartClass = prozis.ownHeroStartClass;
            this.enemyHeroStartClass = prozis.enemyHeroStartClass;
            //####buffs#############################

            this.anzOwnRaidleader = 0;
            this.anzEnemyRaidleader = 0;
            this.anzOwnVessina = 0;
            this.anzEnemyVessina = 0;
            this.anzOwnStormwindChamps = 0;
            this.anzEnemyStormwindChamps = 0;
            this.anzOwnAnimatedArmor = 0;
            this.anzEnemyAnimatedArmor = 0;
            this.anzMoorabi = 0;
            this.anzDark = 0;
            this.anzTamsinroame = 0;
            this.anzOldWoman = 0;
            this.usedUndeadAllies = 0;
            this.anzOwnExtraAngrHp = 0;
            this.anzEnemyExtraAngrHp = 0;
            this.anzOwnWarhorseTrainer = 0;
            this.anzEnemyWarhorseTrainer = 0;
            this.anzOwnTundrarhino = 0;
            this.anzEnemyTundrarhino = 0;
            this.anzOwnMrSmite = 0;//重拳先生
            this.anzEnemyMrSmite = 0;
            this.anzOwnTimberWolfs = 0;//森林狼
            this.anzEnemyTimberWolfs = 0;
            this.anzOwnMurlocWarleader = 0;
            this.anzEnemyMurlocWarleader = 0;
            this.anzAcidmaw = 0;
            this.anzOwnGrimscaleOracle = 0;
            this.anzEnemyGrimscaleOracle = 0;
            this.anzOwnShadowfiend = 0;
            this.anzOwnAuchenaiSoulpriest = 0;
            this.anzEnemyAuchenaiSoulpriest = 0;
            this.anzOwnSouthseacaptain = 0;
            this.anzEnemySouthseacaptain = 0;
            this.anzOwnDragonConsortStarted = 0;
            this.ownElementCostStarted = 0;//火光元素
            this.ownBeastCostLessOnceStarted = 0;//雷矛军用山羊
            this.anzOwnPiratesStarted = 0;//回合开始海盗数
            this.anzOwnMurlocStarted = 0;//回合开始鱼人数
            this.anzOwnElementStarted = 0;//回合开始元素数
            this.anzOwnDraeneiStarted = 0;//回合开始德莱尼数
            this.anzOwnTreantStarted = 0;//回合开始树人数
            this.ownAbilityFreezesTarget = 0;
            this.enemyAbilityFreezesTarget = 0;
            this.ownDemonCostLessOnce = 0;
            this.ownHeroPowerCostLessOnce = 0;
            this.ownHeroPowerCostLessTwice = 0;
            this.enemyHeroPowerCostLessOnce = 0;
            this.ownHeroPowerExtraDamage = 0;
            this.enemyHeroPowerExtraDamage = 0;
            this.ownHeroPowerAllowedQuantity = 1;
            this.enemyHeroPowerAllowedQuantity = 1;
            this.anzUsedOwnHeroPower = 0;
            this.anzUsedEnemyHeroPower = 0;
            this.anzEnemyTaunt = 0;
            this.anzOwnTaunt = 0;
            this.ownMinionsDiedTurn = 0;
            this.enemyMinionsDiedTurn = 0;
            this.feugenDead = Probabilitymaker.Instance.feugenDead;
            this.stalaggDead = Probabilitymaker.Instance.stalaggDead;
            this.weHavePlayedMillhouseManastorm = false;
            this.doublepriest = 0;
            this.enemydoublepriest = 0;
            this.ownMistcaller = 0;
            this.lockandload = 0;//子弹上膛标记
            this.stampede = 0;
            this.ownBaronRivendare = 0;
            this.enemyBaronRivendare = 0;
            this.ownBrannBronzebeard = 0;//友方战吼额外触发次数
            this.enemyBrannBronzebeard = 0;//敌方战吼额外触发次数
            this.ownTurnEndEffectsTriggerTwice = 0;
            this.enemyTurnEndEffectsTriggerTwice = 0;
            this.ownFandralStaghelm = 0;//范达尔·鹿盔
            this.anzOwnRazorfenRockstar = 0; // 剃刀沼泽摇滚明星
            this.anzOwnXB931Housekeeper = 0; // XB-931型家政机
            //#########################################
            this.enemycarddraw = 0;
            this.owncarddraw = 0;
            this.enemyAnzCards = Handmanager.Instance.enemyAnzCards;
            this.ownAbilityReady = prozis.ownAbilityisReady;
            this.ownHeroAblility = new Handmanager.Handcard { card = prozis.heroAbility, manacost = prozis.ownHeroPowerCost };
            this.enemyHeroAblility = new Handmanager.Handcard { card = prozis.enemyAbility, manacost = prozis.enemyHeroPowerCost };
            this.enemyAbilityReady = false;
            this.bestEnemyPlay = null;
            this.ownQuest.Copy(Questmanager.Instance.ownQuest);
            this.enemyQuest.Copy(Questmanager.Instance.enemyQuest);
            this.sideQuest.Copy(Questmanager.Instance.sideQuest);
            this.mobsplayedThisTurn = prozis.numMinionsPlayedThisTurn;
            this.startedWithMobsPlayedThisTurn = prozis.numMinionsPlayedThisTurn;// only change mobsplayedthisturm
            this.cardsPlayedThisTurn = prozis.cardsPlayedThisTurn;
            this.spellsplayedSinceRecalc = 0;
            this.secretsplayedSinceRecalc = 0;
            this.optionsPlayedThisTurn = prozis.numOptionsPlayedThisTurn;
            this.ueberladung = prozis.ueberladung;
            this.lockedMana = prozis.lockedMana;
            this.ownHeroFatigue = prozis.ownHeroFatigue;
            this.enemyHeroFatigue = prozis.enemyHeroFatigue;
            this.ownDeckSize = prozis.ownDeckSize;
            this.enemyDeckSize = prozis.enemyDeckSize;
            //需要以下内容来计算法力值
            this.ownHeroHpStarted = this.ownHero.Hp;
            this.enemyHeroHpStarted = this.enemyHero.Hp;
            this.ownWeaponAttackStarted = this.ownWeapon.Angr;
            this.ownCardsCountStarted = this.owncards.Count;
            this.enemyCardsCountStarted = this.enemyAnzCards;
            this.ownMobsCountStarted = this.ownMinions.Count;
            this.enemyMobsCountStarted = this.enemyMinions.Count;
            this.nextSecretThisTurnCost0 = false;
            this.playedPreparation = false;
            this.nextSpellThisTurnCost0 = false;
            this.nextMurlocThisTurnCostHealth = false;
            this.nextSpellThisTurnCostHealth = false;
            this.nextAnyCardThisTurnCostEnemyHealth = false;
            this.winzigebeschwoererin = 0;
            this.winRazormaneBattleguard = 0;
            this.managespenst = 0;
            this.beschwoerungsportal = 0;
            this.anzOwnAviana = 0; //艾维娜
            this.anzOwnScargil = 0; //斯卡基尔
            this.anzOwnCloakedHuntress = 0; //神秘女猎手
            this.anzOwnPopGarThePutrid = 0;//我方腐臭淤泥波普加

            this.nerubarweblord = 0;
            this.myCardsCostLess = 0;
            this.startedWithmyCardsCostLess = 0;
            this.startedWithnerubarweblord = 0;
            this.startedWithbeschwoerungsportal = 0;
            this.startedWithManagespenst = 0;
            this.startedWithWinzigebeschwoererin = 0;
            this.startedRazormaneBattleguard = 0;
            this.blackwaterpirate = 0;
            this.blackwaterpirateStarted = 0;
            this.embracetheshadow = 0;
            this.ownCrystalCore = prozis.ownCrystalCore;
            this.ownMinionsInDeckCost0 = prozis.ownMinionsInDeckCost0;
            this.LothraxionsPower = prozis.LothraxionsPower;
            this.needGraveyard = true;
            this.loatheb = false;
            this.spellpower = 0;
            this.spellpowerStarted = 0;
            this.enemyspellpower = 0;
            this.enemyspellpowerStarted = 0;
            this.startedWithDamagedMinions = false;
            this.enemyHeroTurnStartedHp = this.enemyHero.Hp;
            this.ownHeroTurnStartedHp = this.ownHero.Hp;
            this.tempanzOwnCards = this.owncards.Count;//手牌数
            this.tempanzEnemyCards = this.enemyAnzCards;
            this.value = int.MinValue;

            //法术派系
            this.ownSpellSchoolCounts = new Dictionary<TAG_SPELL_SCHOOL, int>();
            //伊丽扎·刺刃光环
            this.ownElizagoreblade = 0;
            //回合结束存放的标志
            this.cardsToReturnAtEndOfTurn = new List<CardDB.cardIDEnum>();
            //救生光环
            this.ownSunscreenTurns = 0;
            this.enemySunscreenTurns = 0;
            //重封者拉兹
            this.ownHeroPowerCanBeUsedMultipleTimes = false;
            //当前发掘次数
            this.excavationCount = 0;
            //总发掘次数
            this.allExcavationCount = 0;
            //下个战吼触发次数
            this.nextBattlecryTriggers = 1;
            //军团进攻
            this.ownLegionInvasion = false;
            this.enemyLegionInvasion = false;
            //维和者阿米图斯
            this.ownAmitusThePeacekeeper = false;
            this.enemyAmitusThePeacekeeper = false;
            //下一张元素随从牌的法力值消耗减少量
            this.nextElementalReduction = 0;
            //本回合下一张元素随从牌的法力值消耗减少量
            this.thisTurnNextElementalReduction = 0;
            //上次打出的卡牌的费用
            this.lastPlayedCardCost = 0;
            //在本回合是否打出了元素牌
            this.playedElementalThisTurn = false;
            foreach (TAG_SPELL_SCHOOL school in Enum.GetValues(typeof(TAG_SPELL_SCHOOL)))
            {
                this.ownSpellSchoolCounts[school] = 0;
            }

            //我方特殊随从的效果标志位 站场效果
            foreach (Minion m in this.ownMinions)
            {
                // 计算鱼人恩典
                if (!m.untouchable && (m.handcard.card.race == CardDB.Race.MURLOC || m.handcard.card.race == CardDB.Race.ALL)) this.anzOwnMurlocStarted++; //鱼人
                if (!m.untouchable && (m.handcard.card.race == CardDB.Race.PIRATE || m.handcard.card.race == CardDB.Race.ALL)) this.anzOwnPiratesStarted++;//Pirates海盗
                if (!m.untouchable && (m.handcard.card.race == CardDB.Race.ELEMENTAL || m.handcard.card.race == CardDB.Race.ALL)) this.anzOwnElementStarted++;//元素
                if (!m.untouchable && (m.handcard.card.race == CardDB.Race.DRAENEI || m.handcard.card.race == CardDB.Race.ALL)) this.anzOwnDraeneiStarted++;//德莱尼
                if (!m.untouchable && m.handcard.card.Treant) this.anzOwnTreantStarted++;//树人


                if (m.Hp < m.maxHp && m.Hp >= 1) this.startedWithDamagedMinions = true;

                this.spellpowerStarted += m.spellpower;//法强
                if (m.silenced) continue;
                this.spellpowerStarted += m.handcard.card.spellpowervalue;
                this.spellpower = this.spellpowerStarted;
                if (m.taunt) anzOwnTaunt++;

                //用来写 触发标记的随从
                switch (m.name)
                {
                    case CardDB.cardNameEN.blackwaterpirate://黑水海盗
                        this.blackwaterpirate++;
                        this.blackwaterpirateStarted++;
                        continue;
                    case CardDB.cardNameEN.chogall://古加尔
                        if (m.playedThisTurn && this.cardsPlayedThisTurn == this.mobsplayedThisTurn) this.nextSpellThisTurnCostHealth = true;
                        continue;
                    case CardDB.cardNameEN.prophetvelen://先知维纶
                        this.doublepriest++;//双倍牧师
                        continue;
                    case CardDB.cardNameEN.themistcaller:
                        this.ownMistcaller++;//随从+1+1
                        continue;
                    case CardDB.cardNameEN.pintsizedsummoner://小个子召唤师
                        this.winzigebeschwoererin++;
                        this.startedWithWinzigebeschwoererin++;
                        continue;
                    case CardDB.cardNameEN.razormanebattleguard://卫兵
                        this.winRazormaneBattleguard++;
                        this.startedRazormaneBattleguard++;
                        continue;
                    case CardDB.cardNameEN.manawraith:
                        this.managespenst++;//法力怨魂
                        this.startedWithManagespenst++;
                        continue;
                    case CardDB.cardNameEN.nerubarweblord://尼鲁巴蛛网领主
                        this.nerubarweblord++;
                        this.startedWithnerubarweblord++;
                        continue;
                    case CardDB.cardNameEN.venturecomercenary:  //风险投资公司雇佣兵                      
                        this.ownMinionsCostMore += 3;
                        this.ownMinionsCostMoreAtStart += 3;
                        continue;
                    case CardDB.cardNameEN.corpsewidow://巨型尸蛛
                        this.ownDRcardsCostMore -= 2;
                        this.ownDRcardsCostMoreAtStart -= 2;
                        continue;
                    case CardDB.cardNameEN.summoningportal://召唤传送门
                        this.beschwoerungsportal++;
                        this.startedWithbeschwoerungsportal++;
                        continue;
                    case CardDB.cardNameEN.vaelastrasz://瓦拉斯塔兹
                        this.myCardsCostLess += 3;//卡牌法力值消耗减少3
                        this.startedWithmyCardsCostLess += 3;
                        continue;
                    case CardDB.cardNameEN.scargil: // 斯卡基尔
                        this.anzOwnScargil++;
                        continue;
                    case CardDB.cardNameEN.aviana://艾维娜
                        this.anzOwnAviana++;
                        continue;
                    case CardDB.cardNameEN.cloakedhuntress://神秘女猎手
                        this.anzOwnCloakedHuntress++;
                        continue;
                    case CardDB.cardNameEN.razorfenrockstar: //剃刀沼泽摇滚明星
                        this.anzOwnRazorfenRockstar++;
                        continue;
                    case CardDB.cardNameEN.xb931housekeeper: //XB-931型家政机
                        this.anzOwnXB931Housekeeper++;
                        continue;
                    case CardDB.cardNameEN.popgartheputrid://腐臭淤泥波普加

                        this.anzOwnPopGarThePutrid++;
                        continue;
                    case CardDB.cardNameEN.baronrivendare://瑞文戴尔男爵
                        this.ownBaronRivendare++;
                        continue;
                    case CardDB.cardNameEN.brannbronzebeard://布莱恩·铜须
                        this.ownBrannBronzebeard++;
                        continue;
                    case CardDB.cardNameEN.drakkarienchanter://达卡莱附魔师
                        this.ownTurnEndEffectsTriggerTwice++;
                        continue;
                    case CardDB.cardNameEN.fandralstaghelm://范达尔·鹿盔
                        this.ownFandralStaghelm++;
                        continue;
                    case CardDB.cardNameEN.kelthuzad://克尔苏加德
                        this.needGraveyard = true;
                        continue;
                    case CardDB.cardNameEN.leokk://雷欧克
                        this.anzOwnRaidleader++;
                        continue;
                    case CardDB.cardNameEN.raidleader://团队领袖
                        this.anzOwnRaidleader++;
                        continue;
                    case CardDB.cardNameEN.vessina://维西纳
                        this.anzOwnVessina++;
                        continue;
                    case CardDB.cardNameEN.warhorsetrainer://战马训练师
                        this.anzOwnWarhorseTrainer++;
                        continue;
                    case CardDB.cardNameEN.fallenhero://英雄之魂
                        this.ownHeroPowerExtraDamage++;
                        continue;
                    case CardDB.cardNameEN.garrisoncommander://要塞指挥官
                        bool another = false;
                        foreach (Minion mnn in this.ownMinions)
                        {
                            if (mnn.name == CardDB.cardNameEN.garrisoncommander && mnn.entityID != m.entityID) another = true;
                        }
                        if (!another) this.ownHeroPowerAllowedQuantity++;
                        continue;
                    case CardDB.cardNameEN.coldarradrake://考达拉幼龙
                        this.ownHeroPowerAllowedQuantity += 100;
                        continue;
                    case CardDB.cardNameEN.mindbreaker://摧心者
                        this.ownHeroAblility.manacost = 100;
                        this.enemyHeroAblility.manacost = 100;
                        this.ownAbilityReady = false;
                        this.ownAbilityReady = false;
                        continue;
                    case CardDB.cardNameEN.malganis://玛尔加尼斯
                        this.anzOwnMalGanis++;
                        continue;
                    case CardDB.cardNameEN.bolframshield://博尔夫·碎盾
                        this.anzOwnBolfRamshield++;
                        continue;
                    case CardDB.cardNameEN.ladyblaumeux://女公爵布劳缪克丝 冒险卡
                        this.anzOwnHorsemen++;
                        continue;
                    case CardDB.cardNameEN.thanekorthazz://库尔塔兹领主 冒险
                        this.anzOwnHorsemen++;
                        continue;
                    case CardDB.cardNameEN.sirzeliek://瑟里耶克爵士 冒险
                        this.anzOwnHorsemen++;
                        continue;
                    case CardDB.cardNameEN.stormwindchampion://暴风城勇士
                        this.anzOwnStormwindChamps++;
                        continue;
                    case CardDB.cardNameEN.animatedarmor://复活的铠甲
                        this.anzOwnAnimatedArmor++;
                        continue;
                    case CardDB.cardNameEN.moorabi://莫拉比
                        this.anzMoorabi++;
                        continue;
                    case CardDB.cardNameEN.darkglare://黑眼
                        this.anzDark++;
                        continue;
                    case CardDB.cardNameEN.tamsinroame: //塔姆辛罗姆
                        this.anzTamsinroame++;
                        continue;
                    case CardDB.cardNameEN.voidtouchedattendant: //虚触侍从
                        this.anzOldWoman++;
                        continue;
                    case CardDB.cardNameEN.tundrarhino://苔原犀牛
                        this.anzOwnTundrarhino++;
                        continue;
                    case CardDB.cardNameEN.mrsmite://重拳先生
                        this.anzOwnMrSmite++;
                        continue;
                    case CardDB.cardNameEN.timberwolf://森林狼
                        this.anzOwnTimberWolfs++;
                        continue;
                    case CardDB.cardNameEN.murlocwarleader://鱼人领军
                        this.anzOwnMurlocWarleader++;
                        continue;
                    case CardDB.cardNameEN.acidmaw://酸喉
                        this.anzAcidmaw++;
                        continue;
                    case CardDB.cardNameEN.grimscaleoracle://暗鳞先知
                        this.anzOwnGrimscaleOracle++;
                        continue;
                    case CardDB.cardNameEN.shadowfiend://暗影魔
                        this.anzOwnShadowfiend++;
                        continue;
                    case CardDB.cardNameEN.auchenaisoulpriest://奥金尼灵魂祭司
                        this.anzOwnAuchenaiSoulpriest++;
                        continue;
                    case CardDB.cardNameEN.radiantelemental: goto case CardDB.cardNameEN.sorcerersapprentice;//光照元素
                    case CardDB.cardNameEN.sorcerersapprentice://巫师学徒
                        this.ownSpelsCostMore--;
                        this.ownSpelsCostMoreAtStart--;
                        continue;
                    case CardDB.cardNameEN.nerubianunraveler:  //     蛛魔拆解者                 
                        this.ownSpelsCostMore += 2;
                        this.ownSpelsCostMoreAtStart += 2;
                        continue;
                    case CardDB.cardNameEN.electron://电荷金刚
                        this.ownSpelsCostMore -= 3;
                        this.ownSpelsCostMoreAtStart -= 3;
                        continue;
                    case CardDB.cardNameEN.icewalker://寒冰行者
                        this.ownAbilityFreezesTarget++;
                        continue;
                    case CardDB.cardNameEN.southseacaptain://南海船长
                        this.anzOwnSouthseacaptain++;
                        continue;
                    case CardDB.cardNameEN.chromaggus://克洛玛古斯
                        this.anzOwnChromaggus++;
                        continue;
                    case CardDB.cardNameEN.mechwarper://机械跃迁者
                        this.anzOwnMechwarper++;
                        this.anzOwnMechwarperStarted++;
                        continue;
                    case CardDB.cardNameEN.steamwheedlesniper://热砂港狙击手
                        this.weHaveSteamwheedleSniper = true;
                        continue;
                    default:
                        break;
                }
            }

            foreach (Handmanager.Handcard hc in this.owncards)
            {

                if (hc.card.nameEN == CardDB.cardNameEN.kelthuzad)//克总
                {
                    this.needGraveyard = true;
                }
            }

            //敌方特殊随从的效果标志位
            foreach (Minion m in this.enemyMinions)
            {
                this.enemyMinionStartCount++;
                this.enemyspellpowerStarted += m.spellpower;
                if (m.silenced) continue;
                this.enemyspellpowerStarted += m.handcard.card.spellpowervalue;
                this.enemyspellpower = this.enemyspellpowerStarted;
                if (m.taunt) anzEnemyTaunt++;

                switch (m.name)
                {
                    case CardDB.cardNameEN.voidtouchedattendant:
                        this.anzOldWoman++;
                        continue;
                    case CardDB.cardNameEN.baronrivendare:
                        this.enemyBaronRivendare++;
                        continue;
                    case CardDB.cardNameEN.brannbronzebeard:
                        this.enemyBrannBronzebeard++;
                        continue;
                    case CardDB.cardNameEN.drakkarienchanter:
                        this.enemyTurnEndEffectsTriggerTwice++;
                        continue;
                    case CardDB.cardNameEN.kelthuzad:
                        this.needGraveyard = true;
                        continue;
                    case CardDB.cardNameEN.prophetvelen:
                        this.enemydoublepriest++;
                        continue;
                    case CardDB.cardNameEN.manawraith:
                        this.managespenst++;
                        this.startedWithManagespenst++;
                        continue;
                    case CardDB.cardNameEN.electron:
                        this.ownSpelsCostMore -= 3;
                        this.ownSpelsCostMoreAtStart -= 3;
                        continue;
                    case CardDB.cardNameEN.doomedapprentice:
                        this.ownSpelsCostMore++;
                        this.ownSpelsCostMoreAtStart++;
                        continue;
                    case CardDB.cardNameEN.nerubarweblord:
                        this.nerubarweblord++;
                        this.startedWithnerubarweblord++;
                        continue;
                    case CardDB.cardNameEN.garrisoncommander:
                        bool another = false;
                        foreach (Minion mnn in this.enemyMinions)
                        {
                            if (mnn.name == CardDB.cardNameEN.garrisoncommander && mnn.entityID != m.entityID) another = true;
                        }
                        if (!another) this.enemyHeroPowerAllowedQuantity++;
                        continue;
                    case CardDB.cardNameEN.coldarradrake:
                        this.enemyHeroPowerAllowedQuantity += 100;
                        continue;
                    case CardDB.cardNameEN.mindbreaker:
                        this.ownHeroAblility.manacost = 100;
                        this.enemyHeroAblility.manacost = 100;
                        this.ownAbilityReady = false;
                        this.ownAbilityReady = false;
                        continue;
                    case CardDB.cardNameEN.fallenhero:
                        this.enemyHeroPowerExtraDamage++;
                        continue;
                    case CardDB.cardNameEN.leokk:
                        this.anzEnemyRaidleader++;
                        continue;
                    case CardDB.cardNameEN.raidleader:
                        this.anzEnemyRaidleader++;
                        continue;
                    case CardDB.cardNameEN.vessina:
                        this.anzEnemyVessina++;
                        continue;
                    case CardDB.cardNameEN.warhorsetrainer:
                        this.anzEnemyWarhorseTrainer++;
                        continue;
                    case CardDB.cardNameEN.malganis:
                        this.anzEnemyMalGanis++;
                        continue;
                    case CardDB.cardNameEN.bolframshield:
                        this.anzEnemyBolfRamshield++;
                        continue;
                    case CardDB.cardNameEN.ladyblaumeux:
                        this.anzEnemyHorsemen++;
                        continue;
                    case CardDB.cardNameEN.thanekorthazz:
                        this.anzEnemyHorsemen++;
                        continue;
                    case CardDB.cardNameEN.sirzeliek:
                        this.anzEnemyHorsemen++;
                        continue;
                    case CardDB.cardNameEN.stormwindchampion:
                        this.anzEnemyStormwindChamps++;
                        continue;
                    case CardDB.cardNameEN.animatedarmor:
                        this.anzEnemyAnimatedArmor++;
                        continue;
                    case CardDB.cardNameEN.moorabi:
                        this.anzMoorabi++;
                        continue;
                    case CardDB.cardNameEN.tundrarhino:
                        this.anzEnemyTundrarhino++;
                        continue;
                    case CardDB.cardNameEN.mrsmite:
                        this.anzEnemyMrSmite++;
                        continue;
                    case CardDB.cardNameEN.timberwolf:
                        this.anzEnemyTimberWolfs++;
                        continue;
                    case CardDB.cardNameEN.murlocwarleader:
                        this.anzEnemyMurlocWarleader++;
                        continue;
                    case CardDB.cardNameEN.acidmaw:
                        this.anzAcidmaw++;
                        continue;
                    case CardDB.cardNameEN.grimscaleoracle:
                        this.anzEnemyGrimscaleOracle++;
                        continue;
                    case CardDB.cardNameEN.auchenaisoulpriest:
                        this.anzEnemyAuchenaiSoulpriest++;
                        continue;
                    case CardDB.cardNameEN.steamwheedlesniper:
                        this.enemyHaveSteamwheedleSniper = true;
                        continue;
                    case CardDB.cardNameEN.icewalker:
                        this.enemyAbilityFreezesTarget++;
                        continue;
                    case CardDB.cardNameEN.southseacaptain:
                        this.anzEnemySouthseacaptain++;
                        continue;
                    case CardDB.cardNameEN.chromaggus:
                        this.anzEnemyChromaggus++;
                        continue;
                    case CardDB.cardNameEN.mechwarper:
                        this.anzEnemyMechwarper++;
                        this.anzEnemyMechwarperStarted++;
                        continue;
                }
            }

            if (this.spellpowerStarted > 0) this.ownSpiritclaws = true;//幽灵爪加攻
            if (this.enemyspellpowerStarted > 0) this.enemySpiritclaws = true;

            if (this.enemySecretCount >= 1) this.needGraveyard = true;
            if (this.needGraveyard) this.diedMinions = new List<GraveYardItem>(Probabilitymaker.Instance.turngraveyard);//墓地

            deckGuess.guessEnemyDeck(this);
            this.enemyGuessDeck = Hrtprozis.Instance.enemyDeckName;
        }

        /// <summary>
        /// 后续计算从之前的 p 继承数据，光环效果从之前继承，因此 sim 中需要对光环做相应处理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="transToEnemy">改为用对手的视角来看</param>
        public Playfield(Playfield p, bool transToEnemy = false)
        {
            this.value = int.MinValue;
            this.enemyGuessDeck = p.enemyGuessDeck;
            this.enemyHeroTurnStartedHp = p.enemyHeroTurnStartedHp;
            this.ownHeroTurnStartedHp = p.ownHeroTurnStartedHp;
            this.pID = prozis.getPid();
            if (p.print)
            {
                this.print = true;
                this.pIdHistory.AddRange(p.pIdHistory);
                this.pIdHistory.Add(pID);
                this.doDirtyTts = p.doDirtyTts;
                this.dirtyTwoTurnSim = p.dirtyTwoTurnSim;
                this.checkLostAct = p.checkLostAct;
                this.enemyTurnsCount = p.enemyTurnsCount;
            }
            this.isLethalCheck = p.isLethalCheck;
            this.nextEntity = p.nextEntity;
            this.patchesInDeck = p.patchesInDeck;
            this.anzDark = p.anzDark;
            this.anzTamsinroame = p.anzTamsinroame;
            this.healOrDamageTimes = p.healOrDamageTimes;
            this.healTimes = p.healTimes;
            this.totalAngr = -1;
            this.enemyTotalAngr = -1;
            this.ownMinionsDied = p.ownMinionsDied;
            this.setMankrik = p.setMankrik;
            this.anzSolor = p.anzSolor;
            this.enemyMinionStartCount = p.enemyMinionStartCount;
            this.deckAngrBuff = p.deckAngrBuff;
            this.deckHpBuff = p.deckHpBuff;
            this.isOwnTurn = p.isOwnTurn;
            this.turnCounter = p.turnCounter;
            this.gTurn = p.gTurn;
            this.gTurnStep = p.gTurnStep;
            this.AnzSoulFragments = p.AnzSoulFragments;
            this.anzOldWoman = p.anzOldWoman;
            this.usedUndeadAllies = p.usedUndeadAllies;
            this.anzTamsin = p.anzTamsin;

            foreach (var item in p.ownGraveyard)
            {
                if (!this.ownGraveyard.ContainsKey(item.Key) && CardDB.Instance.getCardDataFromID(item.Key).type == CardDB.cardtype.MOB)
                {
                    this.ownGraveyard.Add(item.Key, item.Value);
                }
            }

            this.anzOgOwnCThunAngrBonus = p.anzOgOwnCThunAngrBonus;
            this.anzOgOwnCThunHpBonus = p.anzOgOwnCThunHpBonus;
            this.anzOgOwnCThunTaunt = p.anzOgOwnCThunTaunt;
            this.anzOwnJadeGolem = p.anzOwnJadeGolem;
            this.anzEnemyJadeGolem = p.anzEnemyJadeGolem;
            this.anzOwnElementalsThisTurn = p.anzOwnElementalsThisTurn;
            this.anzOwnElementalsLastTurn = p.anzOwnElementalsLastTurn;
            this.attacked = p.attacked;
            this.ownController = p.ownController;
            this.bestEnemyPlay = p.bestEnemyPlay;
            this.endTurnState = p.endTurnState;
            this.ownSecretsIDList.AddRange(p.ownSecretsIDList);
            this.evaluatePenality = p.evaluatePenality;
            this.ruleWeight = p.ruleWeight;
            this.rulesUsed = p.rulesUsed;
            this.enemySecretList.Clear();
            if (p.useSecretsPlayAround)
            {
                this.useSecretsPlayAround = true;
                foreach (SecretItem si in p.enemySecretList)
                {
                    this.enemySecretList.Add(new SecretItem(si));
                }
            }
            this.mana = p.mana;
            this.manaTurnEnd = p.manaTurnEnd;

            if (p.LurkersDB.Count > 0) this.LurkersDB = new Dictionary<int, IDEnumOwner>(p.LurkersDB);

            if (transToEnemy)
            {
                this.enemyMaxMana = p.ownMaxMana;
                this.ownMaxMana = p.enemyMaxMana;
                this.mana = p.enemyMaxMana;

                addMinionsReal(prozis.ownMinions, enemyMinions);
                addMinionsReal(prozis.enemyMinions, ownMinions);

                this.ownHero = new Minion(prozis.enemyHero);
                this.enemyHero = new Minion(prozis.ownHero);
                this.ownWeapon = new Weapon(prozis.enemyWeapon);
                this.enemyWeapon = new Weapon(prozis.ownWeapon);

                this.ownHeroName = prozis.enemyHeroname;
                this.enemyHeroName = prozis.heroname;
                this.ownHeroStartClass = prozis.enemyHeroStartClass;
                this.enemyHeroStartClass = prozis.ownHeroStartClass;

                this.anzTamsin = this.ownHero.enchs.Contains(CardDB.cardIDEnum.SW_091t5);

                this.enemyAnzCards = Handmanager.Instance.anzcards;
                this.owncarddraw = p.enemycarddraw;
                this.enemycarddraw = p.owncarddraw;

                this.ownAbilityReady = true;
                this.enemyHeroAblility = new Handmanager.Handcard { card = prozis.heroAbility, manacost = prozis.ownHeroPowerCost };
                this.ownHeroAblility = new Handmanager.Handcard { card = prozis.enemyAbility, manacost = prozis.enemyHeroPowerCost };
                this.enemyAbilityReady = false;
                this.bestEnemyPlay = null;

                this.enemyQuest.Copy(p.ownQuest);
                this.ownQuest.Copy(p.enemyQuest);

                this.anzOwnTaunt = p.anzEnemyTaunt;
                this.anzEnemyTaunt = p.anzOwnTaunt;

                this.enemyspellpower = p.spellpower;
                this.enemyspellpowerStarted = p.spellpowerStarted;
                this.spellpower = p.enemyspellpower;
                this.spellpowerStarted = p.enemyspellpowerStarted;

                if (this.ownHero.enchs.Contains(CardDB.cardIDEnum.SW_450t4e))
                {
                    this.spellpower += 3;
                }

                // TODO 对手的光环随从
                this.anzDark = 0;
                this.anzTamsinroame = 0;
                this.ownHeroPowerExtraDamage = 0;
                this.ownHero.numAttacksThisTurn = 0;

                this.enemySecretCount = p.ownSecretsIDList.Count;

                foreach (Minion m in p.ownMinions)
                {
                    switch (m.handcard.card.nameCN)
                    {
                        case CardDB.cardNameCN.黑眼:
                            anzDark++;
                            break;
                        case CardDB.cardNameCN.塔姆辛罗姆:
                            anzTamsinroame++;
                            break;
                    }
                }

            }
            else
            {
                this.ownMaxMana = p.ownMaxMana;
                this.enemyMaxMana = p.enemyMaxMana;

                addMinionsReal(p.ownMinions, ownMinions);
                addMinionsReal(p.enemyMinions, enemyMinions);
                addCardsReal(p.owncards);

                this.ownHero = new Minion(p.ownHero);
                this.enemyHero = new Minion(p.enemyHero);
                this.ownWeapon = new Weapon(p.ownWeapon);
                this.enemyWeapon = new Weapon(p.enemyWeapon);
                this.ownHeroName = p.ownHeroName;
                this.enemyHeroName = p.enemyHeroName;

                this.ownHeroStartClass = p.ownHeroStartClass;
                this.enemyHeroStartClass = p.enemyHeroStartClass;

                this.owncarddraw = p.owncarddraw;
                this.enemycarddraw = p.enemycarddraw;
                this.enemyAnzCards = p.enemyAnzCards;

                this.ownAbilityReady = p.ownAbilityReady;
                this.enemyAbilityReady = p.enemyAbilityReady;
                this.ownHeroAblility = new Handmanager.Handcard(p.ownHeroAblility);
                this.enemyHeroAblility = new Handmanager.Handcard(p.enemyHeroAblility);

                this.ownQuest.Copy(p.ownQuest);
                this.enemyQuest.Copy(p.enemyQuest);
                this.sideQuest.Copy(p.sideQuest);

                this.anzEnemyTaunt = p.anzEnemyTaunt;
                this.anzOwnTaunt = p.anzOwnTaunt;

                this.spellpower = p.spellpower;
                this.spellpowerStarted = p.spellpowerStarted;
                this.enemyspellpower = p.enemyspellpower;
                this.enemyspellpowerStarted = p.enemyspellpowerStarted;

                this.anzDark = p.anzDark;
                this.anzTamsinroame = p.anzTamsinroame;
                this.ownHeroPowerExtraDamage = p.ownHeroPowerExtraDamage;
                this.enemySecretCount = p.enemySecretCount;

            }

            this.playactions.AddRange(p.playactions);
            this.complete = false;

            this.attackFaceHP = p.attackFaceHP;


            this.lostDamage = p.lostDamage;
            this.lostWeaponDamage = p.lostWeaponDamage;
            this.lostHeal = p.lostHeal;

            this.mobsplayedThisTurn = p.mobsplayedThisTurn;
            this.startedWithMobsPlayedThisTurn = p.startedWithMobsPlayedThisTurn;
            this.spellsplayedSinceRecalc = p.spellsplayedSinceRecalc;
            this.secretsplayedSinceRecalc = p.secretsplayedSinceRecalc;
            this.optionsPlayedThisTurn = p.optionsPlayedThisTurn;
            this.cardsPlayedThisTurn = p.cardsPlayedThisTurn;
            this.enemyOptionsDoneThisTurn = p.enemyOptionsDoneThisTurn;
            this.ueberladung = p.ueberladung;
            this.lockedMana = p.lockedMana;
            //圣契
            this.libram = p.libram;
            this.ownDeckSize = p.ownDeckSize;
            this.enemyDeckSize = p.enemyDeckSize;
            this.ownHeroFatigue = p.ownHeroFatigue;
            this.enemyHeroFatigue = p.enemyHeroFatigue;

            //白银之手新兵
            this.LothraxionsPower = p.LothraxionsPower;

            //need the following for manacost-calculation
            this.ownHeroHpStarted = p.ownHeroHpStarted;
            this.ownWeaponAttackStarted = p.ownWeaponAttackStarted;
            this.ownCardsCountStarted = p.ownCardsCountStarted;
            this.enemyCardsCountStarted = p.enemyCardsCountStarted;
            this.ownMobsCountStarted = p.ownMobsCountStarted;
            this.enemyMobsCountStarted = p.enemyMobsCountStarted;
            this.nextSecretThisTurnCost0 = p.nextSecretThisTurnCost0;
            this.nextSpellThisTurnCost0 = p.nextSpellThisTurnCost0;
            this.nextMurlocThisTurnCostHealth = p.nextMurlocThisTurnCostHealth;
            this.nextAnyCardThisTurnCostEnemyHealth = p.nextAnyCardThisTurnCostEnemyHealth;


            this.blackwaterpirate = p.blackwaterpirate;
            this.blackwaterpirateStarted = p.blackwaterpirateStarted;
            this.nextSpellThisTurnCostHealth = p.nextSpellThisTurnCostHealth;
            this.embracetheshadow = p.embracetheshadow;
            this.ownCrystalCore = p.ownCrystalCore;
            this.ownMinionsInDeckCost0 = p.ownMinionsInDeckCost0;

            this.playedPreparation = p.playedPreparation;

            this.winzigebeschwoererin = p.winzigebeschwoererin;
            this.startedWithWinzigebeschwoererin = p.startedWithWinzigebeschwoererin;
            this.winRazormaneBattleguard = p.winRazormaneBattleguard;
            this.startedRazormaneBattleguard = p.startedRazormaneBattleguard;
            this.managespenst = p.managespenst;
            this.startedWithManagespenst = p.startedWithManagespenst;
            this.ownSpelsCostMore = p.ownSpelsCostMore;
            this.ownSpelsCostMoreAtStart = p.ownSpelsCostMoreAtStart;
            this.ownMinionsCostMore = p.ownMinionsCostMore;
            this.ownMinionsCostMoreAtStart = p.ownMinionsCostMoreAtStart;
            this.ownDRcardsCostMore = p.ownDRcardsCostMore;
            this.ownDRcardsCostMoreAtStart = p.ownDRcardsCostMoreAtStart;
            this.beschwoerungsportal = p.beschwoerungsportal;
            this.startedWithbeschwoerungsportal = p.startedWithbeschwoerungsportal;
            this.myCardsCostLess = p.myCardsCostLess;
            this.startedWithmyCardsCostLess = p.startedWithmyCardsCostLess;
            this.anzOwnScargil = p.anzOwnScargil;//斯卡基尔
            this.anzOwnAviana = p.anzOwnAviana; //艾维娜
            this.anzOwnCloakedHuntress = p.anzOwnCloakedHuntress;//神秘女猎手
            this.anzOwnRazorfenRockstar = p.anzOwnRazorfenRockstar; // 剃刀沼泽摇滚明星
            this.anzOwnXB931Housekeeper = p.anzOwnXB931Housekeeper; // XB-931型家政机
            this.anzOwnPopGarThePutrid = p.anzOwnPopGarThePutrid;//我方腐臭淤泥波普加

            this.nerubarweblord = p.nerubarweblord;
            this.startedWithnerubarweblord = p.startedWithnerubarweblord;
            this.startedWithDamagedMinions = p.startedWithDamagedMinions;
            this.loatheb = p.loatheb;
            this.needGraveyard = p.needGraveyard;
            if (p.needGraveyard) this.diedMinions = new List<GraveYardItem>(p.diedMinions);
            this.OwnLastDiedMinion = p.OwnLastDiedMinion;

            //####buffs#############################

            this.anzOwnRaidleader = p.anzOwnRaidleader;
            this.anzEnemyRaidleader = p.anzEnemyRaidleader;
            this.anzOwnVessina = p.anzOwnVessina;
            this.anzEnemyVessina = p.anzEnemyVessina;
            this.anzOwnWarhorseTrainer = p.anzOwnWarhorseTrainer;
            this.anzEnemyWarhorseTrainer = p.anzEnemyWarhorseTrainer;
            this.anzOwnMalGanis = p.anzOwnMalGanis;
            this.anzEnemyMalGanis = p.anzEnemyMalGanis;
            this.anzOwnBolfRamshield = p.anzOwnBolfRamshield;
            this.anzEnemyBolfRamshield = p.anzEnemyBolfRamshield;
            this.anzOwnHorsemen = p.anzOwnHorsemen;
            this.anzEnemyHorsemen = p.anzEnemyHorsemen;
            this.anzOwnAnimatedArmor = p.anzOwnAnimatedArmor;
            this.anzOwnExtraAngrHp = p.anzOwnExtraAngrHp;
            this.anzEnemyExtraAngrHp = p.anzEnemyExtraAngrHp;
            this.anzEnemyAnimatedArmor = p.anzEnemyAnimatedArmor;
            this.anzMoorabi = p.anzMoorabi;
            this.anzOwnPiratesStarted = p.anzOwnPiratesStarted;
            this.anzOwnMurlocStarted = p.anzOwnMurlocStarted;
            this.anzOwnElementStarted = p.anzOwnElementStarted;
            this.anzOwnDraeneiStarted = p.anzOwnDraeneiStarted;
            this.anzOwnTreantStarted = p.anzOwnTreantStarted;
            this.anzOwnStormwindChamps = p.anzOwnStormwindChamps;
            this.anzEnemyStormwindChamps = p.anzEnemyStormwindChamps;
            this.anzOwnTundrarhino = p.anzOwnTundrarhino;
            this.anzEnemyTundrarhino = p.anzEnemyTundrarhino;
            //重拳先生
            this.anzOwnMrSmite = p.anzOwnMrSmite;
            this.anzEnemyMrSmite = p.anzEnemyMrSmite;
            this.anzOwnTimberWolfs = p.anzOwnTimberWolfs;
            this.anzEnemyTimberWolfs = p.anzEnemyTimberWolfs;
            this.anzOwnMurlocWarleader = p.anzOwnMurlocWarleader;
            this.anzEnemyMurlocWarleader = p.anzEnemyMurlocWarleader;
            this.anzAcidmaw = p.anzAcidmaw;
            this.anzOwnGrimscaleOracle = p.anzOwnGrimscaleOracle;
            this.anzEnemyGrimscaleOracle = p.anzEnemyGrimscaleOracle;
            this.anzOwnShadowfiend = p.anzOwnShadowfiend;
            this.anzOwnAuchenaiSoulpriest = p.anzOwnAuchenaiSoulpriest;
            this.anzEnemyAuchenaiSoulpriest = p.anzEnemyAuchenaiSoulpriest;
            this.anzOwnSouthseacaptain = p.anzOwnSouthseacaptain;
            this.anzEnemySouthseacaptain = p.anzEnemySouthseacaptain;
            this.anzOwnMechwarper = p.anzOwnMechwarper;
            this.anzOwnMechwarperStarted = p.anzOwnMechwarperStarted;
            this.anzEnemyMechwarper = p.anzEnemyMechwarper;
            this.anzEnemyMechwarperStarted = p.anzEnemyMechwarperStarted;
            this.anzOwnChromaggus = p.anzOwnChromaggus;
            this.anzEnemyChromaggus = p.anzEnemyChromaggus;
            this.anzOwnDragonConsort = p.anzOwnDragonConsort;
            this.anzOwnMurlocConsort = p.anzOwnMurlocConsort;   //酸小明
            this.anzOwnDragonConsortStarted = p.anzOwnDragonConsortStarted;
            //火光元素
            this.ownElementCost = p.ownElementCost;
            this.ownElementCostStarted = p.ownElementCostStarted;
            //雷矛军用山羊
            this.ownBeastCostLessOnce = p.ownBeastCostLessOnce;
            this.ownBeastCostLessOnceStarted = p.ownBeastCostLessOnceStarted;
            this.ownAbilityFreezesTarget = p.ownAbilityFreezesTarget;
            this.enemyAbilityFreezesTarget = p.enemyAbilityFreezesTarget;
            this.ownDemonCostLessOnce = p.ownDemonCostLessOnce;
            this.ownHeroPowerCostLessOnce = p.ownHeroPowerCostLessOnce;
            this.ownHeroPowerCostLessTwice = p.ownHeroPowerCostLessTwice;
            this.enemyHeroPowerCostLessOnce = p.enemyHeroPowerCostLessOnce;
            this.enemyHeroPowerExtraDamage = p.enemyHeroPowerExtraDamage;
            this.ownHeroPowerAllowedQuantity = p.ownHeroPowerAllowedQuantity;
            this.enemyHeroPowerAllowedQuantity = p.enemyHeroPowerAllowedQuantity;
            this.anzUsedOwnHeroPower = p.anzUsedOwnHeroPower;
            this.anzUsedEnemyHeroPower = p.anzUsedEnemyHeroPower;
            this.ownMinionsDiedTurn = p.ownMinionsDiedTurn;
            this.enemyMinionsDiedTurn = p.enemyMinionsDiedTurn;
            this.feugenDead = p.feugenDead;
            this.stalaggDead = p.stalaggDead;
            this.weHavePlayedMillhouseManastorm = p.weHavePlayedMillhouseManastorm;
            this.ownSpiritclaws = p.ownSpiritclaws;
            this.enemySpiritclaws = p.enemySpiritclaws;
            this.doublepriest = p.doublepriest;
            this.enemydoublepriest = p.enemydoublepriest;
            this.ownMistcaller = p.ownMistcaller;
            this.lockandload = p.lockandload;
            this.stampede = p.stampede;
            this.ownBaronRivendare = p.ownBaronRivendare;
            this.enemyBaronRivendare = p.enemyBaronRivendare;
            this.ownBrannBronzebeard = p.ownBrannBronzebeard;
            this.enemyBrannBronzebeard = p.enemyBrannBronzebeard;
            this.ownTurnEndEffectsTriggerTwice = p.ownTurnEndEffectsTriggerTwice;
            this.enemyTurnEndEffectsTriggerTwice = p.enemyTurnEndEffectsTriggerTwice;
            this.ownFandralStaghelm = p.ownFandralStaghelm;
            this.weHaveSteamwheedleSniper = p.weHaveSteamwheedleSniper;
            this.enemyHaveSteamwheedleSniper = p.enemyHaveSteamwheedleSniper;
            //#########################################
            this.tempanzOwnCards = this.owncards.Count;
            this.tempanzEnemyCards = this.enemyAnzCards;
            this.value = int.MinValue;


            //法术派系
            this.ownSpellSchoolCounts = p.ownSpellSchoolCounts;
            //伊丽扎·刺刃光环
            this.ownElizagoreblade = p.ownElizagoreblade;
            //救生光环
            this.ownSunscreenTurns = p.ownSunscreenTurns;
            this.enemySunscreenTurns = p.enemySunscreenTurns;
            //重封者拉兹
            this.ownHeroPowerCanBeUsedMultipleTimes = p.ownHeroPowerCanBeUsedMultipleTimes;
            //当前发掘次数
            this.excavationCount = p.excavationCount;
            //总发掘次数
            this.allExcavationCount = p.allExcavationCount;
            //下个战吼触发次数
            this.nextBattlecryTriggers = p.nextBattlecryTriggers;
            //军团进攻
            this.ownLegionInvasion = p.ownLegionInvasion;
            this.enemyLegionInvasion = p.enemyLegionInvasion;
            //维和者阿米图斯
            this.ownAmitusThePeacekeeper = p.ownAmitusThePeacekeeper;
            this.enemyAmitusThePeacekeeper = p.enemyAmitusThePeacekeeper;
            //元素牌费用减少
            this.nextElementalReduction = p.nextElementalReduction;
            this.thisTurnNextElementalReduction = p.thisTurnNextElementalReduction;
            //上次打出的卡牌的费用
            this.lastPlayedCardCost = p.lastPlayedCardCost;
            //在本回合是否打出了元素牌
            this.playedElementalThisTurn = p.playedElementalThisTurn;
        }

        /// <summary>
        /// 从源 Playfield 复制所有字段，复用现有的 List/Dictionary 对象以避免分配。
        /// 逻辑与拷贝构造函数相同，但不会创建新的 List/Dictionary 实例。
        /// 此方法供 PlayfieldPool.Rent() 使用。
        /// </summary>
        public void CopyFrom(Playfield p, bool transToEnemy = false)
        {
            this.value = int.MinValue;
            this.enemyGuessDeck = p.enemyGuessDeck;
            this.enemyHeroTurnStartedHp = p.enemyHeroTurnStartedHp;
            this.ownHeroTurnStartedHp = p.ownHeroTurnStartedHp;
            this.pID = prozis.getPid();
            if (p.print)
            {
                this.print = true;
                this.pIdHistory.Clear();
                this.pIdHistory.AddRange(p.pIdHistory);
                this.pIdHistory.Add(pID);
                this.doDirtyTts = p.doDirtyTts;
                this.dirtyTwoTurnSim = p.dirtyTwoTurnSim;
                this.checkLostAct = p.checkLostAct;
                this.enemyTurnsCount = p.enemyTurnsCount;
            }
            else
            {
                this.print = false;
                this.pIdHistory.Clear();
            }
            this.isLethalCheck = p.isLethalCheck;
            this.nextEntity = p.nextEntity;
            this.patchesInDeck = p.patchesInDeck;
            this.anzDark = p.anzDark;
            this.anzTamsinroame = p.anzTamsinroame;
            this.healOrDamageTimes = p.healOrDamageTimes;
            this.healTimes = p.healTimes;
            this.totalAngr = -1;
            this.enemyTotalAngr = -1;
            this.ownMinionsDied = p.ownMinionsDied;
            this.setMankrik = p.setMankrik;
            this.anzSolor = p.anzSolor;
            this.enemyMinionStartCount = p.enemyMinionStartCount;
            this.deckAngrBuff = p.deckAngrBuff;
            this.deckHpBuff = p.deckHpBuff;
            this.isOwnTurn = p.isOwnTurn;
            this.turnCounter = p.turnCounter;
            this.gTurn = p.gTurn;
            this.gTurnStep = p.gTurnStep;
            this.AnzSoulFragments = p.AnzSoulFragments;
            this.anzOldWoman = p.anzOldWoman;
            this.usedUndeadAllies = p.usedUndeadAllies;
            this.anzTamsin = p.anzTamsin;

            // ownGraveyard: reuse existing Dictionary
            this.ownGraveyard.Clear();
            foreach (var item in p.ownGraveyard)
            {
                if (!this.ownGraveyard.ContainsKey(item.Key) && CardDB.Instance.getCardDataFromID(item.Key).type == CardDB.cardtype.MOB)
                {
                    this.ownGraveyard.Add(item.Key, item.Value);
                }
            }

            this.anzOgOwnCThunAngrBonus = p.anzOgOwnCThunAngrBonus;
            this.anzOgOwnCThunHpBonus = p.anzOgOwnCThunHpBonus;
            this.anzOgOwnCThunTaunt = p.anzOgOwnCThunTaunt;
            this.anzOwnJadeGolem = p.anzOwnJadeGolem;
            this.anzEnemyJadeGolem = p.anzEnemyJadeGolem;
            this.anzOwnElementalsThisTurn = p.anzOwnElementalsThisTurn;
            this.anzOwnElementalsLastTurn = p.anzOwnElementalsLastTurn;
            this.attacked = p.attacked;
            this.ownController = p.ownController;
            this.bestEnemyPlay = p.bestEnemyPlay;
            this.endTurnState = p.endTurnState;

            // ownSecretsIDList: reuse list
            this.ownSecretsIDList.Clear();
            this.ownSecretsIDList.AddRange(p.ownSecretsIDList);

            this.evaluatePenality = p.evaluatePenality;
            this.ruleWeight = p.ruleWeight;
            this.rulesUsed = p.rulesUsed;

            // enemySecretList: reuse list
            this.enemySecretList.Clear();
            if (p.useSecretsPlayAround)
            {
                this.useSecretsPlayAround = true;
                foreach (SecretItem si in p.enemySecretList)
                {
                    this.enemySecretList.Add(new SecretItem(si));
                }
            }
            else
            {
                this.useSecretsPlayAround = false;
            }

            this.mana = p.mana;
            this.manaTurnEnd = p.manaTurnEnd;

            // LurkersDB: reuse Dictionary
            this.LurkersDB.Clear();
            if (p.LurkersDB.Count > 0)
            {
                foreach (var kvp in p.LurkersDB)
                {
                    this.LurkersDB.Add(kvp.Key, kvp.Value);
                }
            }

            if (transToEnemy)
            {
                this.enemyMaxMana = p.ownMaxMana;
                this.ownMaxMana = p.enemyMaxMana;
                this.mana = p.enemyMaxMana;

                this.ownMinions.Clear();
                foreach (var m in prozis.ownMinions)
                    this.ownMinions.Add(MinionPool.Rent(m));
                this.enemyMinions.Clear();
                foreach (var m in prozis.enemyMinions)
                    this.enemyMinions.Add(MinionPool.Rent(m));

                this.ownHero = new Minion(prozis.enemyHero);
                this.enemyHero = new Minion(prozis.ownHero);
                this.ownWeapon = new Weapon(prozis.enemyWeapon);
                this.enemyWeapon = new Weapon(prozis.ownWeapon);

                this.ownHeroName = prozis.enemyHeroname;
                this.enemyHeroName = prozis.heroname;
                this.ownHeroStartClass = prozis.enemyHeroStartClass;
                this.enemyHeroStartClass = prozis.ownHeroStartClass;

                this.anzTamsin = this.ownHero.enchs.Contains(CardDB.cardIDEnum.SW_091t5);

                this.enemyAnzCards = Handmanager.Instance.anzcards;
                this.owncarddraw = p.enemycarddraw;
                this.enemycarddraw = p.owncarddraw;

                this.ownAbilityReady = true;
                this.enemyHeroAblility = new Handmanager.Handcard { card = prozis.heroAbility, manacost = prozis.ownHeroPowerCost };
                this.ownHeroAblility = new Handmanager.Handcard { card = prozis.enemyAbility, manacost = prozis.enemyHeroPowerCost };
                this.enemyAbilityReady = false;
                this.bestEnemyPlay = null;

                this.enemyQuest.Copy(p.ownQuest);
                this.ownQuest.Copy(p.enemyQuest);

                this.anzOwnTaunt = p.anzEnemyTaunt;
                this.anzEnemyTaunt = p.anzOwnTaunt;

                this.enemyspellpower = p.spellpower;
                this.enemyspellpowerStarted = p.spellpowerStarted;
                this.spellpower = p.enemyspellpower;
                this.spellpowerStarted = p.enemyspellpowerStarted;

                if (this.ownHero.enchs.Contains(CardDB.cardIDEnum.SW_450t4e))
                {
                    this.spellpower += 3;
                }

                this.anzDark = 0;
                this.anzTamsinroame = 0;
                this.ownHeroPowerExtraDamage = 0;
                this.ownHero.numAttacksThisTurn = 0;

                this.enemySecretCount = p.ownSecretsIDList.Count;

                // Recalculate own auras from transposed minions
                foreach (Minion m in p.ownMinions)
                {
                    switch (m.handcard.card.nameCN)
                    {
                        case CardDB.cardNameCN.黑眼:
                            anzDark++;
                            break;
                        case CardDB.cardNameCN.塔姆辛罗姆:
                            anzTamsinroame++;
                            break;
                    }
                }
            }
            else
            {
                this.ownMaxMana = p.ownMaxMana;
                this.enemyMaxMana = p.enemyMaxMana;

                // Reuse ownMinions list: clear and repopulate from pool
                this.ownMinions.Clear();
                foreach (var m in p.ownMinions)
                    this.ownMinions.Add(MinionPool.Rent(m));
                this.enemyMinions.Clear();
                foreach (var m in p.enemyMinions)
                    this.enemyMinions.Add(MinionPool.Rent(m));

                // Reuse owncards list: repopulate from pool
                this.owncards.Clear();
                foreach (var hc in p.owncards)
                    this.owncards.Add(HandcardPool.Rent(hc));

                this.ownHero = new Minion(p.ownHero);
                this.enemyHero = new Minion(p.enemyHero);
                this.ownWeapon = new Weapon(p.ownWeapon);
                this.enemyWeapon = new Weapon(p.enemyWeapon);

                this.ownHeroName = p.ownHeroName;
                this.enemyHeroName = p.enemyHeroName;
                this.ownHeroStartClass = p.ownHeroStartClass;
                this.enemyHeroStartClass = p.enemyHeroStartClass;

                this.owncarddraw = p.owncarddraw;
                this.enemycarddraw = p.enemycarddraw;
                this.enemyAnzCards = p.enemyAnzCards;

                this.ownAbilityReady = p.ownAbilityReady;
                this.enemyAbilityReady = p.enemyAbilityReady;
                this.ownHeroAblility = new Handmanager.Handcard(p.ownHeroAblility);
                this.enemyHeroAblility = new Handmanager.Handcard(p.enemyHeroAblility);

                this.ownQuest.Copy(p.ownQuest);
                this.enemyQuest.Copy(p.enemyQuest);
                this.sideQuest.Copy(p.sideQuest);

                this.anzEnemyTaunt = p.anzEnemyTaunt;
                this.anzOwnTaunt = p.anzOwnTaunt;

                this.spellpower = p.spellpower;
                this.spellpowerStarted = p.spellpowerStarted;
                this.enemyspellpower = p.enemyspellpower;
                this.enemyspellpowerStarted = p.enemyspellpowerStarted;

                this.anzDark = p.anzDark;
                this.anzTamsinroame = p.anzTamsinroame;
                this.ownHeroPowerExtraDamage = p.ownHeroPowerExtraDamage;
                this.enemySecretCount = p.enemySecretCount;
            }

            // playactions: reuse list
            this.playactions.Clear();
            this.playactions.AddRange(p.playactions);
            this.complete = false;

            this.attackFaceHP = p.attackFaceHP;
            this.lostDamage = p.lostDamage;
            this.lostWeaponDamage = p.lostWeaponDamage;
            this.lostHeal = p.lostHeal;

            this.mobsplayedThisTurn = p.mobsplayedThisTurn;
            this.startedWithMobsPlayedThisTurn = p.startedWithMobsPlayedThisTurn;
            this.spellsplayedSinceRecalc = p.spellsplayedSinceRecalc;
            this.secretsplayedSinceRecalc = p.secretsplayedSinceRecalc;
            this.optionsPlayedThisTurn = p.optionsPlayedThisTurn;
            this.cardsPlayedThisTurn = p.cardsPlayedThisTurn;
            this.enemyOptionsDoneThisTurn = p.enemyOptionsDoneThisTurn;
            this.ueberladung = p.ueberladung;
            this.lockedMana = p.lockedMana;
            this.libram = p.libram;
            this.ownDeckSize = p.ownDeckSize;
            this.enemyDeckSize = p.enemyDeckSize;
            this.ownHeroFatigue = p.ownHeroFatigue;
            this.enemyHeroFatigue = p.enemyHeroFatigue;

            this.LothraxionsPower = p.LothraxionsPower;

            this.ownHeroHpStarted = p.ownHeroHpStarted;
            this.ownWeaponAttackStarted = p.ownWeaponAttackStarted;
            this.ownCardsCountStarted = p.ownCardsCountStarted;
            this.enemyCardsCountStarted = p.enemyCardsCountStarted;
            this.ownMobsCountStarted = p.ownMobsCountStarted;
            this.enemyMobsCountStarted = p.enemyMobsCountStarted;
            this.nextSecretThisTurnCost0 = p.nextSecretThisTurnCost0;
            this.nextSpellThisTurnCost0 = p.nextSpellThisTurnCost0;
            this.nextMurlocThisTurnCostHealth = p.nextMurlocThisTurnCostHealth;
            this.nextAnyCardThisTurnCostEnemyHealth = p.nextAnyCardThisTurnCostEnemyHealth;

            this.blackwaterpirate = p.blackwaterpirate;
            this.blackwaterpirateStarted = p.blackwaterpirateStarted;
            this.nextSpellThisTurnCostHealth = p.nextSpellThisTurnCostHealth;
            this.embracetheshadow = p.embracetheshadow;
            this.ownCrystalCore = p.ownCrystalCore;
            this.ownMinionsInDeckCost0 = p.ownMinionsInDeckCost0;
            this.playedPreparation = p.playedPreparation;

            this.winzigebeschwoererin = p.winzigebeschwoererin;
            this.startedWithWinzigebeschwoererin = p.startedWithWinzigebeschwoererin;
            this.winRazormaneBattleguard = p.winRazormaneBattleguard;
            this.startedRazormaneBattleguard = p.startedRazormaneBattleguard;
            this.managespenst = p.managespenst;
            this.startedWithManagespenst = p.startedWithManagespenst;
            this.ownSpelsCostMore = p.ownSpelsCostMore;
            this.ownSpelsCostMoreAtStart = p.ownSpelsCostMoreAtStart;
            this.ownMinionsCostMore = p.ownMinionsCostMore;
            this.ownMinionsCostMoreAtStart = p.ownMinionsCostMoreAtStart;
            this.ownDRcardsCostMore = p.ownDRcardsCostMore;
            this.ownDRcardsCostMoreAtStart = p.ownDRcardsCostMoreAtStart;
            this.beschwoerungsportal = p.beschwoerungsportal;
            this.startedWithbeschwoerungsportal = p.startedWithbeschwoerungsportal;
            this.myCardsCostLess = p.myCardsCostLess;
            this.startedWithmyCardsCostLess = p.startedWithmyCardsCostLess;
            this.anzOwnScargil = p.anzOwnScargil;
            this.anzOwnAviana = p.anzOwnAviana;
            this.anzOwnCloakedHuntress = p.anzOwnCloakedHuntress;
            this.anzOwnRazorfenRockstar = p.anzOwnRazorfenRockstar;
            this.anzOwnXB931Housekeeper = p.anzOwnXB931Housekeeper;
            this.anzOwnPopGarThePutrid = p.anzOwnPopGarThePutrid;

            this.nerubarweblord = p.nerubarweblord;
            this.startedWithnerubarweblord = p.startedWithnerubarweblord;
            this.startedWithDamagedMinions = p.startedWithDamagedMinions;
            this.loatheb = p.loatheb;
            this.needGraveyard = p.needGraveyard;

            // diedMinions: only allocate if needed
            if (p.needGraveyard && p.diedMinions != null)
            {
                if (this.diedMinions == null)
                    this.diedMinions = new List<GraveYardItem>(p.diedMinions);
                else
                {
                    this.diedMinions.Clear();
                    this.diedMinions.AddRange(p.diedMinions);
                }
            }
            else
            {
                this.diedMinions = null;
            }

            this.OwnLastDiedMinion = p.OwnLastDiedMinion;

            // aura counters
            this.anzOwnRaidleader = p.anzOwnRaidleader;
            this.anzEnemyRaidleader = p.anzEnemyRaidleader;
            this.anzOwnVessina = p.anzOwnVessina;
            this.anzEnemyVessina = p.anzEnemyVessina;
            this.anzOwnWarhorseTrainer = p.anzOwnWarhorseTrainer;
            this.anzEnemyWarhorseTrainer = p.anzEnemyWarhorseTrainer;
            this.anzOwnMalGanis = p.anzOwnMalGanis;
            this.anzEnemyMalGanis = p.anzEnemyMalGanis;
            this.anzOwnBolfRamshield = p.anzOwnBolfRamshield;
            this.anzEnemyBolfRamshield = p.anzEnemyBolfRamshield;
            this.anzOwnHorsemen = p.anzOwnHorsemen;
            this.anzEnemyHorsemen = p.anzEnemyHorsemen;
            this.anzOwnAnimatedArmor = p.anzOwnAnimatedArmor;
            this.anzOwnExtraAngrHp = p.anzOwnExtraAngrHp;
            this.anzEnemyExtraAngrHp = p.anzEnemyExtraAngrHp;
            this.anzEnemyAnimatedArmor = p.anzEnemyAnimatedArmor;
            this.anzMoorabi = p.anzMoorabi;
            this.anzOwnPiratesStarted = p.anzOwnPiratesStarted;
            this.anzOwnMurlocStarted = p.anzOwnMurlocStarted;
            this.anzOwnElementStarted = p.anzOwnElementStarted;
            this.anzOwnDraeneiStarted = p.anzOwnDraeneiStarted;
            this.anzOwnTreantStarted = p.anzOwnTreantStarted;
            this.anzOwnStormwindChamps = p.anzOwnStormwindChamps;
            this.anzEnemyStormwindChamps = p.anzEnemyStormwindChamps;
            this.anzOwnTundrarhino = p.anzOwnTundrarhino;
            this.anzEnemyTundrarhino = p.anzEnemyTundrarhino;
            this.anzOwnMrSmite = p.anzOwnMrSmite;
            this.anzEnemyMrSmite = p.anzEnemyMrSmite;
            this.anzOwnTimberWolfs = p.anzOwnTimberWolfs;
            this.anzEnemyTimberWolfs = p.anzEnemyTimberWolfs;
            this.anzOwnMurlocWarleader = p.anzOwnMurlocWarleader;
            this.anzEnemyMurlocWarleader = p.anzEnemyMurlocWarleader;
            this.anzAcidmaw = p.anzAcidmaw;
            this.anzOwnGrimscaleOracle = p.anzOwnGrimscaleOracle;
            this.anzEnemyGrimscaleOracle = p.anzEnemyGrimscaleOracle;
            this.anzOwnShadowfiend = p.anzOwnShadowfiend;
            this.anzOwnAuchenaiSoulpriest = p.anzOwnAuchenaiSoulpriest;
            this.anzEnemyAuchenaiSoulpriest = p.anzEnemyAuchenaiSoulpriest;
            this.anzOwnSouthseacaptain = p.anzOwnSouthseacaptain;
            this.anzEnemySouthseacaptain = p.anzEnemySouthseacaptain;
            this.anzOwnMechwarper = p.anzOwnMechwarper;
            this.anzOwnMechwarperStarted = p.anzOwnMechwarperStarted;
            this.anzEnemyMechwarper = p.anzEnemyMechwarper;
            this.anzEnemyMechwarperStarted = p.anzEnemyMechwarperStarted;
            this.anzOwnChromaggus = p.anzOwnChromaggus;
            this.anzEnemyChromaggus = p.anzEnemyChromaggus;
            this.anzOwnDragonConsort = p.anzOwnDragonConsort;
            this.anzOwnMurlocConsort = p.anzOwnMurlocConsort;
            this.anzOwnDragonConsortStarted = p.anzOwnDragonConsortStarted;
            this.ownElementCost = p.ownElementCost;
            this.ownElementCostStarted = p.ownElementCostStarted;
            this.ownBeastCostLessOnce = p.ownBeastCostLessOnce;
            this.ownBeastCostLessOnceStarted = p.ownBeastCostLessOnceStarted;
            this.ownAbilityFreezesTarget = p.ownAbilityFreezesTarget;
            this.enemyAbilityFreezesTarget = p.enemyAbilityFreezesTarget;
            this.ownDemonCostLessOnce = p.ownDemonCostLessOnce;
            this.ownHeroPowerCostLessOnce = p.ownHeroPowerCostLessOnce;
            this.ownHeroPowerCostLessTwice = p.ownHeroPowerCostLessTwice;
            this.enemyHeroPowerCostLessOnce = p.enemyHeroPowerCostLessOnce;
            this.enemyHeroPowerExtraDamage = p.enemyHeroPowerExtraDamage;
            this.ownHeroPowerAllowedQuantity = p.ownHeroPowerAllowedQuantity;
            this.enemyHeroPowerAllowedQuantity = p.enemyHeroPowerAllowedQuantity;
            this.anzUsedOwnHeroPower = p.anzUsedOwnHeroPower;
            this.anzUsedEnemyHeroPower = p.anzUsedEnemyHeroPower;
            this.ownMinionsDiedTurn = p.ownMinionsDiedTurn;
            this.enemyMinionsDiedTurn = p.enemyMinionsDiedTurn;
            this.feugenDead = p.feugenDead;
            this.stalaggDead = p.stalaggDead;
            this.weHavePlayedMillhouseManastorm = p.weHavePlayedMillhouseManastorm;
            this.ownSpiritclaws = p.ownSpiritclaws;
            this.enemySpiritclaws = p.enemySpiritclaws;
            this.doublepriest = p.doublepriest;
            this.enemydoublepriest = p.enemydoublepriest;
            this.ownMistcaller = p.ownMistcaller;
            this.lockandload = p.lockandload;
            this.stampede = p.stampede;
            this.ownBaronRivendare = p.ownBaronRivendare;
            this.enemyBaronRivendare = p.enemyBaronRivendare;
            this.ownBrannBronzebeard = p.ownBrannBronzebeard;
            this.enemyBrannBronzebeard = p.enemyBrannBronzebeard;
            this.ownTurnEndEffectsTriggerTwice = p.ownTurnEndEffectsTriggerTwice;
            this.enemyTurnEndEffectsTriggerTwice = p.enemyTurnEndEffectsTriggerTwice;
            this.ownFandralStaghelm = p.ownFandralStaghelm;
            this.weHaveSteamwheedleSniper = p.weHaveSteamwheedleSniper;
            this.enemyHaveSteamwheedleSniper = p.enemyHaveSteamwheedleSniper;

            this.tempanzOwnCards = this.owncards.Count;
            this.tempanzEnemyCards = this.enemyAnzCards;
            this.value = int.MinValue;

            // 法术派系 (reuse dictionary)
            this.ownSpellSchoolCounts.Clear();
            foreach (var kvp in p.ownSpellSchoolCounts)
            {
                this.ownSpellSchoolCounts[kvp.Key] = kvp.Value;
            }

            this.ownElizagoreblade = p.ownElizagoreblade;
            this.ownSunscreenTurns = p.ownSunscreenTurns;
            this.enemySunscreenTurns = p.enemySunscreenTurns;
            this.ownHeroPowerCanBeUsedMultipleTimes = p.ownHeroPowerCanBeUsedMultipleTimes;
            this.excavationCount = p.excavationCount;
            this.allExcavationCount = p.allExcavationCount;
            this.nextBattlecryTriggers = p.nextBattlecryTriggers;
            this.ownLegionInvasion = p.ownLegionInvasion;
            this.enemyLegionInvasion = p.enemyLegionInvasion;
            this.ownAmitusThePeacekeeper = p.ownAmitusThePeacekeeper;
            this.enemyAmitusThePeacekeeper = p.enemyAmitusThePeacekeeper;
            this.nextElementalReduction = p.nextElementalReduction;
            this.thisTurnNextElementalReduction = p.thisTurnNextElementalReduction;
            this.lastPlayedCardCost = p.lastPlayedCardCost;
            this.playedElementalThisTurn = p.playedElementalThisTurn;

            this.name = p.name;
        }

        /// <summary>
        /// 将 Playfield 实例重置为可复用的空状态。
        /// 清除所有列表和对象引用以便安全地归还对象池。
        /// 注意：此方法不会将列表置为 null，只会 Clear()，以便 CopyFrom 可以复用它。
        /// </summary>
        public void ClearForPool()
        {
            // 将 Minion 对象归还 MinionPool，再清空列表
            foreach (var m in this.ownMinions) MinionPool.Return(m);
            this.ownMinions.Clear();
            foreach (var m in this.enemyMinions) MinionPool.Return(m);
            this.enemyMinions.Clear();

            // 将 Handcard 对象归还 HandcardPool，再清空列表
            foreach (var hc in this.owncards) HandcardPool.Return(hc);
            this.owncards.Clear();

            // 清空其余列表（保留分配的对象）
            this.nextPlayfields.Clear();
            this.playactions.Clear();
            this.ownSecretsIDList.Clear();
            this.enemySecretList.Clear();
            this.pIdHistory.Clear();
            this.ownGraveyard.Clear();
            this.LurkersDB.Clear();
            this.enemyHand.Clear();
            this.ownDeck.Clear();
            this.cardsToReturnAtEndOfTurn.Clear();
            this.ownPlayedDeathrattleCards.Clear();
            this.enemyPlayedDeathrattleCards.Clear();
            this.ownMinionsPlayedThisGame.Clear();
            this.sigilsToTriggerOnOwnTurnStart.Clear();

            // 重置字典
            if (this.ownSpellSchoolCounts != null)
                this.ownSpellSchoolCounts.Clear();

            // 释放对象引用
            this.bestEnemyPlay = null;
            this.endTurnState = null;
            this.diedMinions = null;
            this.enemyCardsOut = null;
            this.tempTrigger = new triggerCounter();

            // 重置数值字段
            this.value = int.MinValue;
            this.hashcode = 0;
            this.complete = false;
            this.totalAngr = -1;
            this.enemyTotalAngr = -1;
            this.evaluatePenality = 0;
            this.ruleWeight = 0;
            this.rulesUsed = "";
            this.name = "";
            this.enemyGuessDeck = "";
            this.print = false;
            this.logging = false;

            // 重置光环计数
            this.anzOwnRaidleader = 0;
            this.anzEnemyRaidleader = 0;
            this.anzOwnVessina = 0;
            this.anzEnemyVessina = 0;
            this.anzOwnStormwindChamps = 0;
            this.anzEnemyStormwindChamps = 0;
            this.anzOwnWarhorseTrainer = 0;
            this.anzEnemyWarhorseTrainer = 0;
            this.anzOwnTundrarhino = 0;
            this.anzEnemyTundrarhino = 0;
            this.anzOwnMrSmite = 0;
            this.anzEnemyMrSmite = 0;
            this.anzOwnTimberWolfs = 0;
            this.anzEnemyTimberWolfs = 0;
            this.anzOwnMurlocWarleader = 0;
            this.anzEnemyMurlocWarleader = 0;
            this.anzAcidmaw = 0;
            this.anzOwnGrimscaleOracle = 0;
            this.anzEnemyGrimscaleOracle = 0;
            this.anzOwnShadowfiend = 0;
            this.anzOwnAuchenaiSoulpriest = 0;
            this.anzEnemyAuchenaiSoulpriest = 0;
            this.anzOwnSouthseacaptain = 0;
            this.anzEnemySouthseacaptain = 0;
            this.anzOwnMalGanis = 0;
            this.anzEnemyMalGanis = 0;
            this.anzOwnPiratesStarted = 0;
            this.anzOwnMurlocStarted = 0;
            this.anzOwnElementStarted = 0;
            this.anzOwnDraeneiStarted = 0;
            this.anzOwnTreantStarted = 0;
            this.anzOwnChromaggus = 0;
            this.anzEnemyChromaggus = 0;
            this.anzOwnDragonConsort = 0;
            this.anzOwnMurlocConsort = 0;
            this.anzOwnDragonConsortStarted = 0;
            this.ownElementCost = 0;
            this.ownElementCostStarted = 0;
            this.ownBeastCostLessOnce = 0;
            this.ownBeastCostLessOnceStarted = 0;
            this.anzOwnBolfRamshield = 0;
            this.anzEnemyBolfRamshield = 0;
            this.anzOwnHorsemen = 0;
            this.anzEnemyHorsemen = 0;
            this.anzOwnAnimatedArmor = 0;
            this.anzEnemyAnimatedArmor = 0;
            this.anzMoorabi = 0;
            this.anzDark = 0;
            this.anzTamsinroame = 0;
            this.anzOldWoman = 0;
            this.anzTamsin = false;
            this.anzUsedOwnHeroPower = 0;
            this.anzUsedEnemyHeroPower = 0;
            this.anzOwnExtraAngrHp = 0;
            this.anzEnemyExtraAngrHp = 0;
            this.anzOwnMechwarper = 0;
            this.anzOwnMechwarperStarted = 0;
            this.anzEnemyMechwarper = 0;
            this.anzEnemyMechwarperStarted = 0;
            this.anzOgOwnCThun = 0;
            this.anzOgOwnCThunHpBonus = 0;
            this.anzOgOwnCThunAngrBonus = 0;
            this.anzOgOwnCThunTaunt = 0;
            this.anzOwnJadeGolem = 0;
            this.anzEnemyJadeGolem = 0;
            this.anzOwnElementalsThisTurn = 0;
            this.anzOwnElementalsLastTurn = 0;
            this.anzEnemyTaunt = 0;
            this.anzOwnTaunt = 0;
            this.usedUndeadAllies = 0;
            this.ownAbilityFreezesTarget = 0;
            this.enemyAbilityFreezesTarget = 0;
            this.ownHeroPowerCostLessOnce = 0;
            this.enemyHeroPowerCostLessOnce = 0;
            this.ownHeroPowerCostLessTwice = 0;
            this.ownDemonCostLessOnce = 0;
            this.ownHeroPowerExtraDamage = 0;
            this.enemyHeroPowerExtraDamage = 0;
            this.ownHeroPowerAllowedQuantity = 1;
            this.enemyHeroPowerAllowedQuantity = 1;
            this.useNature = 0;
            this.blackwaterpirate = 0;
            this.blackwaterpirateStarted = 0;
            this.embracetheshadow = 0;
            this.ownCrystalCore = 0;
            this.ownMinionsInDeckCost0 = false;
            this.LothraxionsPower = false;
            this.ownMinionsDiedTurn = 0;
            this.enemyMinionsDiedTurn = 0;
            this.feugenDead = false;
            this.stalaggDead = false;
            this.weHavePlayedMillhouseManastorm = false;
            this.weHaveSteamwheedleSniper = false;
            this.enemyHaveSteamwheedleSniper = false;
            this.ownSpiritclaws = false;
            this.enemySpiritclaws = false;
            this.needGraveyard = false;
            this.doublepriest = 0;
            this.enemydoublepriest = 0;
            this.ownMistcaller = 0;
            this.lockandload = 0;
            this.stampede = 0;
            this.ownBaronRivendare = 0;
            this.enemyBaronRivendare = 0;
            this.ownBrannBronzebeard = 0;
            this.enemyBrannBronzebeard = 0;
            this.ownTurnEndEffectsTriggerTwice = 0;
            this.enemyTurnEndEffectsTriggerTwice = 0;
            this.ownFandralStaghelm = 0;
            this.tempanzOwnCards = 0;
            this.tempanzEnemyCards = 0;
            this.isOwnTurn = true;
            this.turnCounter = 0;
            this.attacked = false;
            this.attackFaceHP = 15;
            this.ownController = 0;
            this.guessingHeroHP = 30;
            this.doDirtyTts = 100000;
            this.mana = 0;
            this.manaTurnEnd = 0;
            this.enemySecretCount = 0;
            this.owncarddraw = 0;
            this.enemycarddraw = 0;
            this.enemyAnzCards = 0;
            this.libram = 0;
            this.spellpower = 0;
            this.spellpowerStarted = 0;
            this.enemyspellpower = 0;
            this.enemyspellpowerStarted = 0;
            this.wehaveCounterspell = 0;
            this.lethlMissing = 1000;
            this.nextSecretThisTurnCost0 = false;
            this.playedPreparation = false;
            this.nextSpellThisTurnCost0 = false;
            this.nextMurlocThisTurnCostHealth = false;
            this.nextSpellThisTurnCostHealth = false;
            this.nextAnyCardThisTurnCostEnemyHealth = false;
            this.winzigebeschwoererin = 0;
            this.startedWithWinzigebeschwoererin = 0;
            this.winRazormaneBattleguard = 0;
            this.startedRazormaneBattleguard = 0;
            this.managespenst = 0;
            this.startedWithManagespenst = 0;
            this.ownMinionsCostMore = 0;
            this.ownMinionsCostMoreAtStart = 0;
            this.ownSpelsCostMore = 0;
            this.ownSpelsCostMoreAtStart = 0;
            this.ownDRcardsCostMore = 0;
            this.ownDRcardsCostMoreAtStart = 0;
            this.beschwoerungsportal = 0;
            this.startedWithbeschwoerungsportal = 0;
            this.myCardsCostLess = 0;
            this.startedWithmyCardsCostLess = 0;
            this.anzOwnAviana = 0;
            this.anzOwnScargil = 0;
            this.anzOwnCloakedHuntress = 0;
            this.anzOwnRazorfenRockstar = 0;
            this.anzOwnXB931Housekeeper = 0;
            this.anzOwnPopGarThePutrid = 0;
            this.anzEnemyPopGarThePutrid = 0;
            this.nerubarweblord = 0;
            this.startedWithnerubarweblord = 0;
            this.startedWithDamagedMinions = false;
            this.ownWeaponAttackStarted = 0;
            this.ownMobsCountStarted = 0;
            this.ownCardsCountStarted = 0;
            this.enemyMobsCountStarted = 0;
            this.enemyCardsCountStarted = 0;
            this.ownHeroHpStarted = 30;
            this.enemyHeroHpStarted = 30;
            this.mobsplayedThisTurn = 0;
            this.startedWithMobsPlayedThisTurn = 0;
            this.spellsplayedSinceRecalc = 0;
            this.secretsplayedSinceRecalc = 0;
            this.optionsPlayedThisTurn = 0;
            this.cardsPlayedThisTurn = 0;
            this.ueberladung = 0;
            this.lockedMana = 0;
            this.enemyOptionsDoneThisTurn = 0;
            this.ownMaxResources = 10;
            this.ownMaxHandSize = 10;
            this.enemyMaxResources = 10;
            this.enemyMaxHandSize = 10;
            this.ownMaxMana = 0;
            this.enemyMaxMana = 0;
            this.lostDamage = 0;
            this.lostHeal = 0;
            this.lostWeaponDamage = 0;
            this.ownDeckSize = 30;
            this.enemyDeckSize = 30;
            this.ownHeroFatigue = 0;
            this.enemyHeroFatigue = 0;
            this.ownAbilityReady = false;
            this.enemyAbilityReady = false;
            this.shadowmadnessed = 0;
            this.enemyHeroTurnStartedHp = 0;
            this.ownHeroTurnStartedHp = 0;
            this.ownElizagoreblade = 0;
            this.ownSunscreenTurns = 0;
            this.enemySunscreenTurns = 0;
            this.ownHeroPowerCanBeUsedMultipleTimes = false;
            this.excavationCount = 0;
            this.allExcavationCount = 0;
            this.nextBattlecryTriggers = 1;
            this.ownRelicDoubleCast = false;
            this.enemyRelicDoubleCast = false;
            this.nextMinionBecomesFiveFive = false;
            this.tempSpellPower = 0;
            this.nextChooseOneHasBothEffects = false;
            this.parrotSanctuaryCount = 0;
            this.lastDrawnCardEntityID = -1;
            this.ownLegionInvasion = false;
            this.enemyLegionInvasion = false;
            this.ownAmitusThePeacekeeper = false;
            this.enemyAmitusThePeacekeeper = false;
            this.damageDealtToEnemyHeroThisTurn = 0;
            this.nextElementalReduction = 0;
            this.thisTurnNextElementalReduction = 0;
            this.lastPlayedCardCost = 0;
            this.playedElementalThisTurn = false;
            this.ownMinionsPlayedThisGame.Clear();
            this.ownElementalsPlayedThisTurn = 0;
            this.ownMinionStartCount = 0;
            this.enemyMinionStartCount = 0;
            this.healOrDamageTimes = 0;
            this.healTimes = 0;
            this.setMankrik = false;
            this.anzSolor = false;
            this.nextEntity = 70;
            this.pID = 0;
            this.isLethalCheck = false;
            this.enemyTurnsCount = 0;
            this.patchesInDeck = true;
            this.AnzSoulFragments = 0;
            this.ownSecretsIDList.Clear();
            this.enemySecretList.Clear();
            this.useSecretsPlayAround = true;
            this.loatheb = false;
            this.loathebEffect = 0;
        }


    }

    /// <summary>
    /// 扳机触发计数器
    /// </summary>
    public struct triggerCounter
    {
        /// <summary>
        /// 随从受到治疗扳机
        /// </summary>
        public int minionsGotHealed;
        /// <summary>
        /// 角色受到治疗扳机
        /// </summary>
        public int charsGotHealed;
        /// <summary>
        /// 随从受到伤害扳机
        /// </summary>
        public int ownMinionsGotDmg;
        /// <summary>
        /// 敌方随从受到伤害扳机
        /// </summary>
        public int enemyMinionsGotDmg;
        /// <summary>
        /// 我方英雄受到伤害扳机
        /// </summary>
        public int ownHeroGotDmg;
        /// <summary>
        /// 敌方英雄受到伤害扳机
        /// </summary>
        public int enemyHeroGotDmg;
        /// <summary>
        /// 我方随从死亡扳机
        /// </summary>
        public int ownMinionsDied;
        /// <summary>
        /// 敌方随从死亡扳机
        /// </summary>
        public int enemyMinionsDied;
        /// <summary>
        /// 我方野兽随从召唤扳机
        /// </summary>
        public int ownBeastSummoned;
        /// <summary>
        /// 我方龙随从召唤扳机
        /// </summary>
        public int ownDragonSummoned;
        /// <summary>
        /// 我方野兽随从死亡扳机
        /// </summary>
        public int ownBeastDied;
        /// <summary>
        /// 敌方野兽随从死亡扳机
        /// </summary>
        public int enemyBeastDied;
        /// <summary>
        /// 我方机械随从死亡扳机
        /// </summary>
        public int ownMechanicDied;
        /// <summary>
        /// 敌方机械随从死亡扳机
        /// </summary>
        public int enemyMechanicDied;
        /// <summary>
        /// 我方鱼人随从死亡扳机
        /// </summary>
        public int ownMurlocDied;
        /// <summary>
        /// 敌方鱼人随从死亡扳机
        /// </summary>
        public int enemyMurlocDied;
        /// <summary>
        /// 我方随从失去圣盾扳机
        /// </summary>
        public int ownMinionLosesDivineShield;
        /// <summary>
        /// 敌方随从失去圣盾扳机
        /// </summary>
        public int enemyMinionLosesDivineShield;
        /// <summary>
        /// 我方随从变化扳机
        /// </summary>
        public bool ownMinionsChanged;
        /// <summary>
        /// 敌方随从变化扳机
        /// </summary>
        public bool enemyMininsChanged;
        /// <summary>
        /// 德莱尼扳机提示
        /// </summary>
        public bool draeneiTriggerHint;
        /// <summary>
        /// 星舰发射时扳机
        /// </summary>
        public bool starshipLaunchTrigger;
        /// <summary>
        /// 我方亡灵随从死亡扳机
        /// </summary>
        public int ownUndeadDied;
        /// <summary>
        /// 敌方亡灵随从死亡扳机
        /// </summary>
        public int enemyUndeadDied;
        /// <summary>
        /// 我方树人随从召唤扳机
        /// </summary>
        public int ownTreantSummoned;
        /// <summary>
        /// 我方树人随从死亡扳机
        /// </summary>
        public int ownTreantDied;
    }

    public struct IDEnumOwner
    {
        public CardDB.cardIDEnum IDEnum;
        public bool own;
    }

    public static class RaceUtils
    {
        /// <summary>
        /// 辅助方法，用于检查随从是否为指定的种族或融合怪
        /// </summary>
        /// <param name="race"></param>
        /// <param name="targetRace"></param>
        /// <returns></returns>
        public static bool IsRaceOrAll(CardDB.Race race, CardDB.Race targetRace)
        {
            return race == targetRace || race == CardDB.Race.ALL;
        }

        public static bool MinionBelongsToRace(List<CardDB.Race> races, List<CardDB.Race> targetRace)
        {
            if (races.Count == 0) return false;

            if (targetRace.Contains(CardDB.Race.ALL))
            {
                return true;
            }

            foreach (CardDB.Race race in races)
            {
                if (targetRace.Contains(race) || race == CardDB.Race.ALL)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool MinionBelongsToRace(List<CardDB.Race> races, CardDB.Race targetRace)
        {
            if (races.Count == 0) return false;

            if (targetRace == CardDB.Race.ALL)
            {
                return true;
            }

            foreach (CardDB.Race race in races)
            {
                if (targetRace == race || race == CardDB.Race.ALL)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
