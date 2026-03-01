using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace HREngine.Bots
{
    //!!!	Under test
    //v0.1.2
    /*
     SUMMARY
     1) Comparison operators (=, !=, >, <) !ONLY NUMERIC VALUE!
     (= - equal, != - not equal, > - greater than, < - less than)
     
     tm - turn mana (default mana at this turn);
     am - available mana (at this turn);
     t - turn;
     overload - overload, which can cause a card in the current round;
     owncarddraw - extra carddraw this turn
     ohhp - own hero health points;
     ehhp - enemy hero health points;
     owa - own weapon attack;
     ewa - enemy weapon attack;
     owd - own weapon durability;
     ewd - enemy weapon durability;
     ohc - own hand cards count (the number of cards in own hand);
     ehc - enemy hand cards count (the number of cards in enemy's hand);
     omc - own minions count (the number of own minions on the board);
     emc - enemy minions count (the number of enemy minions on the board);
     For "ohc", "omc" and "emc" you can use these extensions: 
        :Murlocs (the number of own/enemy Murlocs on the board/in own hand)
        :Demons (the number of own/enemy Demons on the board/in own hand)
        :Mechs (the number of own/enemy Mechs on the board/in own hand)
        :Beasts (the number of own/enemy Beasts on the board/in own hand)
        :Totems (the number of own/enemy Totems on the board/in own hand)
        :Pirates (the number of own/enemy Pirates on the board/in own hand)
        :Dragons (the number of own/enemy Dragons on the board/in own hand)
        :Elems (the number of own/enemy Elementals on the board/in own hand)
        :shields (the number of own/enemy shields on the board/in own hand)
        :taunts (the number of own/enemy taunts on the board/in own hand)
     For "ohc" only:
        :Minions (the number of Minions in own hand)
        :Spells (the number of Spells in own hand)
        :Secrets (the number of Secrets in own hand)
        :Weapons (the number of Weapons in own hand)
     For "omc" and "emc" only: 
        :SHR (the number of own/enemy Silver Hand Recruits on the board)
        :undamaged (the number of undamaged own/enemy minions on the board)
        :damaged (the number of damaged own/enemy minions on the board)
    Also you can compare "ohc" with "ehc" and "omc" with "emc"
     Example:   omc>3 - means that you must have more than 3 minions on the board
                omc:Murlocs>3 - means that you must have more than 3 murlocs on the board
                omc>emc - means that you must have more minions than your opponent
     
     2) Boolean operators (=, !=)
     (= - equal/contain; != - not equal/does't contain)
     
     ob - own board (own board must/must not contain this minion (CardID));
     eb - enemy board (enemy board must/must not contain this minion (CardID));
     oh - own hand (own hand must/must not contain this card (CardID));
     ow - own weapon (CardID);
     ew - enemy weapon (CardID);
     ohero - own hero class (ALL, DRUID, HUNTER, MAGE, PALADIN, PRIEST, ROGUE, SHAMAN, WARLOCK, WARRIOR);
     ehero - enemy hero class;
      
     3) Unique:
     coin - must be a coin in hand at turn start;
     !coin - must not be a coin in hand at turn start;
     noduplicates - if your deck contain no duplicates
     p= - play - card in hand that must be played (CardID);
     p2= - play 2 identical cards - 2 identical card in hand that must be played (CardID);
     a= - attacker - minion on board (CardID);
     For "p=" and "p2=" and "a=" you can use these extensions: 
        :pen= (after CardID) - penalty for playing/attacking this card outside of this rule;
        :tgt= - target - target for spell or for minion/weapon (CardID/hero/!hero);
        You can use comparison operators ( =, !=, >, < !ONLY NUMERIC VALUE!) for these parameters:
        :aAt - attacker's attack (mob/hero)
        :aHp - attacker's health points
        :tAt - target's attack
        :tHp - target's health points
     
     4) Condition binding:
     & - And (condition1&condition2 - true only if the condition 1 AND condition 2 are true);
     || - Or (condition1||condition2 - true if the condition 1 is true OR condition 2 is true);
     Example: cond_1 & cond_2||cond_3||cond_4 & cond_5 - true if condition_1 = true And (condition 2 or 3 or 4) = true And condition_5 = true;
     
     5) Extra info (written with a comma)
     bonus=X - if all conditions are TRUE then this Playfield gets this bonus;
         
     */

    /*
     摘要
     1) 比较运算符 (=, !=, >, <) !仅数值值!
     (= - 等于, != - 不等于, > - 大于, < - 小于)
     
     tm - 回合 mana (本回合的默认 mana);
     am - 可用 mana (本回合);
     t - 回合;
     overload - 过载，可能导致当前回合的卡牌;
     owncarddraw - 本回合额外抽牌
     ohhp - 己方英雄生命值;
     ehhp - 敌方英雄生命值;
     owa - 己方武器攻击力;
     ewa - 敌方武器攻击力;
     owd - 己方武器耐久度;
     ewd - 敌方武器耐久度;
     ohc - 己方手牌数量 (己方手牌中的卡牌数量);
     ehc - 敌方手牌数量 (敌方手牌中的卡牌数量);
     omc - 己方随从数量 (己方场上的随从数量);
     emc - 敌方随从数量 (敌方场上的随从数量);
     对于 "ohc", "omc" 和 "emc"，您可以使用这些扩展名：
        :Murlocs (己方/敌方场上/己方手牌中的鱼人数量)
        :Demons (己方/敌方场上/己方手牌中的恶魔数量)
        :Mechs (己方/敌方场上/己方手牌中的机械数量)
        :Beasts (己方/敌方场上/己方手牌中的野兽数量)
        :Totems (己方/敌方场上/己方手牌中的图腾数量)
        :Pirates (己方/敌方场上/己方手牌中的海盗数量)
        :Dragons (己方/敌方场上/己方手牌中的龙数量)
        :Elems (己方/敌方场上/己方手牌中的元素数量)
        :shields (己方/敌方场上/己方手牌中的圣盾数量)
        :taunts (己方/敌方场上/己方手牌中的嘲讽数量)
     仅对于 "ohc"：
        :Minions (己方手牌中的随从数量)
        :Spells (己方手牌中的法术数量)
        :Secrets (己方手牌中的奥秘数量)
        :Weapons (己方手牌中的武器数量)
     仅对于 "omc" 和 "emc"：
        :SHR (己方/敌方场上的白银之手新兵数量)
        :undamaged (己方/敌方场上未受伤的随从数量)
        :damaged (己方/敌方场上受伤的随从数量)
     您还可以比较 "ohc" 与 "ehc" 以及 "omc" 与 "emc"
     示例:   omc>3 - 表示您必须在场上有超过 3 个随从
            omc:Murlocs>3 - 表示您必须在场上有超过 3 个鱼人
            omc>emc - 表示您必须比对手有更多的随从
     
     2) 布尔运算符 (=, !=)
     (= - 等于/包含; != - 不等于/不包含)
     
     ob - 己方场 (己方场必须/不得包含此随从 (CardID));
     eb - 敌方场 (敌方场必须/不得包含此随从 (CardID));
     oh - 己方手牌 (己方手牌必须/不得包含此卡牌 (CardID));
     ow - 己方武器 (CardID);
     ew - 敌方武器 (CardID);
     ohero - 己方英雄职业 (ALL, DRUID, HUNTER, MAGE, PALADIN, PRIEST, ROGUE, SHAMAN, WARLOCK, WARRIOR);
     ehero - 敌方英雄职业;
      
     3) 特殊：
     coin - 回合开始时手牌中必须有硬币;
     !coin - 回合开始时手牌中不得有硬币;
     noduplicates - 如果您的牌组不包含重复牌
     p= - 播放 - 必须播放的手牌中的卡牌 (CardID);
     p2= - 播放 2 张相同的卡牌 - 必须播放的手牌中的 2 张相同卡牌 (CardID);
     a= - 攻击者 - 场上的随从 (CardID);
     对于 "p=", "p2=" 和 "a="，您可以使用这些扩展名：
        :pen= (CardID 之后) - 在本规则之外播放/攻击此卡牌的惩罚;
        :tgt= - 目标 - 法术或随从/武器的目标 (CardID/hero/!hero);
        您可以对这些参数使用比较运算符 ( =, !=, >, < !仅数值值!):
        :aAt - 攻击者的攻击力 (随从/英雄)
        :aHp - 攻击者的生命值
        :tAt - 目标的攻击力
        :tHp - 目标的生命值
     
     4) 条件绑定：
     & - 与 (condition1&condition2 - 仅当条件 1 和条件 2 都为真时为真);
     || - 或 (condition1||condition2 - 如果条件 1 为真或条件 2 为真则为真);
     示例: cond_1 & cond_2||cond_3||cond_4 & cond_5 - 如果 condition_1 = 真 且 (condition 2 或 3 或 4) = 真 且 condition_5 = 真 则为真;
     
     5) 额外信息 (用逗号书写)
     bonus=X - 如果所有条件都为 TRUE，则此 Playfield 获得此奖励;
         
     */

    /// <summary>
    /// 规则引擎类，用于管理和处理游戏中的规则系统
    /// 包括读取规则配置、检查规则条件、计算规则权重等功能
    /// </summary>
    public class RulesEngine
    {
        /// <summary>
        /// 规则堆，存储所有规则
        /// </summary>
        Dictionary<int, Rule> heapOfRules = new Dictionary<int, Rule>();
        /// <summary>
        /// 规则卡牌ID（播放）
        /// </summary>
        Dictionary<int, List<CardDB.cardIDEnum>> RuleCardIdsPlay = new Dictionary<int, List<CardDB.cardIDEnum>>(); 
        /// <summary>
        /// 规则卡牌ID（攻击）
        /// </summary>
        Dictionary<int, List<CardDB.cardIDEnum>> RuleCardIdsAttack = new Dictionary<int, List<CardDB.cardIDEnum>>(); 
        /// <summary>
        /// 规则卡牌ID（手牌）
        /// </summary>
        Dictionary<int, List<CardDB.cardIDEnum>> RuleCardIdsHand = new Dictionary<int, List<CardDB.cardIDEnum>>(); 
        /// <summary>
        /// 规则卡牌ID（己方场）
        /// </summary>
        Dictionary<int, List<CardDB.cardIDEnum>> RuleCardIdsOwnBoard = new Dictionary<int, List<CardDB.cardIDEnum>>(); 
        /// <summary>
        /// 规则卡牌ID（敌方场）
        /// </summary>
        Dictionary<int, List<CardDB.cardIDEnum>> RuleCardIdsEnemyBoard = new Dictionary<int, List<CardDB.cardIDEnum>>(); 
        /// <summary>
        /// 场状态规则
        /// </summary>
        Dictionary<int, int> BoardStateRules = new Dictionary<int, int>(); 
        /// <summary>
        /// 游戏场状态规则
        /// </summary>
        Dictionary<int, int> BoardStateRulesGame = new Dictionary<int, int>(); 
        /// <summary>
        /// 回合场状态规则
        /// </summary>
        Dictionary<int, int> BoardStateRulesTurn = new Dictionary<int, int>(); 
        /// <summary>
        /// 卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, List<int>> CardIdRules = new Dictionary<CardDB.cardIDEnum, List<int>>();
        /// <summary>
        /// 游戏卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> CardIdRulesGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 游戏播放卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> CardIdRulesPlayGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 游戏手牌卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> CardIdRulesHandGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 游戏己方场卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> CardIdRulesOwnBoardGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 游戏敌方场卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> CardIdRulesEnemyBoardGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 游戏攻击者ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> AttackerIdRulesGame = new Dictionary<CardDB.cardIDEnum, Dictionary<int, int>>(); 
        /// <summary>
        /// 回合播放卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, List<int>> CardIdRulesTurnPlay = new Dictionary<CardDB.cardIDEnum, List<int>>(); 
        /// <summary>
        /// 回合手牌卡牌ID规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, List<int>> CardIdRulesTurnHand = new Dictionary<CardDB.cardIDEnum, List<int>>();
        /// <summary>
        /// 游戏手牌种族规则
        /// </summary>
        Dictionary<TAG_RACE, List<int>> hcRaceRulesGame = new Dictionary<TAG_RACE, List<int>>();
        /// <summary>
        /// 回合手牌种族规则
        /// </summary>
        Dictionary<TAG_RACE, List<int>> hcRaceRulesTurn = new Dictionary<TAG_RACE, List<int>>();
        /// <summary>
        /// 游戏场状态规则列表
        /// </summary>
        List<int> pfStateRulesGame = new List<int>();
        /// <summary>
        /// 己方英雄职业规则
        /// </summary>
        Dictionary<TAG_CLASS, Dictionary<int, int>> RuleOwnClass = new Dictionary<TAG_CLASS, Dictionary<int, int>>();
        /// <summary>
        /// 敌方英雄职业规则
        /// </summary>
        Dictionary<TAG_CLASS, Dictionary<int, int>> RuleEnemyClass = new Dictionary<TAG_CLASS, Dictionary<int, int>>();
        /// <summary>
        /// 替换规则
        /// </summary>
        Dictionary<int, int> replacedRules = new Dictionary<int, int>();
        /// <summary>
        /// 规则路径
        /// </summary>
        string pathToRules = "";

        /// <summary>
        /// 是否加载了 mulligan 规则
        /// </summary>
        public bool mulliganRulesLoaded = false;
        /// <summary>
        /// Mulligan 规则
        /// </summary>
        Dictionary<string, string> MulliganRules = new Dictionary<string, string>();
        /// <summary>
        /// Mulligan 规则数据库
        /// </summary>
        Dictionary<string, Dictionary<string, string>> MulliganRulesDB = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 手动 Mulligan 规则
        /// </summary>
        Dictionary<CardDB.cardIDEnum, string> MulliganRulesManual = new Dictionary<CardDB.cardIDEnum, string>();
        /// <summary>
        /// 临时条件
        /// </summary>
        Condition condTmp;
        /// <summary>
        /// 条件错误
        /// </summary>
        string condErr;
        /// <summary>
        /// 目标
        /// </summary>
        Minion target;
        /// <summary>
        /// 临时计数器
        /// </summary>
        int tmp_counter;
        /// <summary>
        /// 打印规则
        /// </summary>
        int printRules = Settings.Instance.printRules;

        /// <summary>
        /// 进程实例
        /// </summary>
        Hrtprozis prozis = Hrtprozis.Instance;


        /// <summary>
        /// 参数枚举
        /// </summary>
        public enum param
        {
            /// <summary>无</summary>
            None,
            /// <summary>或条件</summary>
            orcond,
            /// <summary>回合 mana 等于</summary>
            tm_equal,
            /// <summary>回合 mana 不等于</summary>
            tm_notequal,
            /// <summary>回合 mana 大于</summary>
            tm_greater,
            /// <summary>回合 mana 小于</summary>
            tm_less,
            /// <summary>可用 mana 等于</summary>
            am_equal,
            /// <summary>可用 mana 不等于</summary>
            am_notequal,
            /// <summary>可用 mana 大于</summary>
            am_greater,
            /// <summary>可用 mana 小于</summary>
            am_less,
            /// <summary>己方武器攻击力等于</summary>
            owa_equal,
            /// <summary>己方武器攻击力不等于</summary>
            owa_notequal,
            /// <summary>己方武器攻击力大于</summary>
            owa_greater,
            /// <summary>己方武器攻击力小于</summary>
            owa_less,
            /// <summary>敌方武器攻击力等于</summary>
            ewa_equal,
            /// <summary>敌方武器攻击力不等于</summary>
            ewa_notequal,
            /// <summary>敌方武器攻击力大于</summary>
            ewa_greater,
            /// <summary>敌方武器攻击力小于</summary>
            ewa_less,
            /// <summary>己方武器耐久度等于</summary>
            owd_equal,
            /// <summary>己方武器耐久度不等于</summary>
            owd_notequal,
            /// <summary>己方武器耐久度大于</summary>
            owd_greater,
            /// <summary>己方武器耐久度小于</summary>
            owd_less,
            /// <summary>敌方武器耐久度等于</summary>
            ewd_equal,
            /// <summary>敌方武器耐久度不等于</summary>
            ewd_notequal,
            /// <summary>敌方武器耐久度大于</summary>
            ewd_greater,
            /// <summary>敌方武器耐久度小于</summary>
            ewd_less,
            /// <summary>己方随从数量等于</summary>
            omc_equal,
            /// <summary>己方随从数量不等于</summary>
            omc_notequal,
            /// <summary>己方随从数量大于</summary>
            omc_greater,
            /// <summary>己方随从数量小于</summary>
            omc_less,
            /// <summary>敌方随从数量等于</summary>
            emc_equal,
            /// <summary>敌方随从数量不等于</summary>
            emc_notequal,
            /// <summary>敌方随从数量大于</summary>
            emc_greater,
            /// <summary>敌方随从数量小于</summary>
            emc_less,
            /// <summary>己方随从数量等于敌方</summary>
            omc_equal_emc,
            /// <summary>己方随从数量不等于敌方</summary>
            omc_notequal_emc,
            /// <summary>己方随从数量大于敌方</summary>
            omc_greater_emc,
            /// <summary>己方随从数量小于敌方</summary>
            omc_less_emc,
            /// <summary>己方鱼人数量等于</summary>
            omc_murlocs_equal,
            /// <summary>己方鱼人数量不等于</summary>
            omc_murlocs_notequal,
            /// <summary>己方鱼人数量大于</summary>
            omc_murlocs_greater,
            /// <summary>己方鱼人数量小于</summary>
            omc_murlocs_less,
            /// <summary>敌方鱼人数量等于</summary>
            emc_murlocs_equal,
            /// <summary>敌方鱼人数量不等于</summary>
            emc_murlocs_notequal,
            /// <summary>敌方鱼人数量大于</summary>
            emc_murlocs_greater,
            /// <summary>敌方鱼人数量小于</summary>
            emc_murlocs_less,
            /// <summary>己方恶魔数量等于</summary>
            omc_demons_equal,
            /// <summary>己方恶魔数量不等于</summary>
            omc_demons_notequal,
            /// <summary>己方恶魔数量大于</summary>
            omc_demons_greater,
            /// <summary>己方恶魔数量小于</summary>
            omc_demons_less,
            /// <summary>敌方恶魔数量等于</summary>
            emc_demons_equal,
            /// <summary>敌方恶魔数量不等于</summary>
            emc_demons_notequal,
            /// <summary>敌方恶魔数量大于</summary>
            emc_demons_greater,
            /// <summary>敌方恶魔数量小于</summary>
            emc_demons_less,
            /// <summary>己方机械数量等于</summary>
            omc_mechs_equal,
            /// <summary>己方机械数量不等于</summary>
            omc_mechs_notequal,
            /// <summary>己方机械数量大于</summary>
            omc_mechs_greater,
            /// <summary>己方机械数量小于</summary>
            omc_mechs_less,
            /// <summary>敌方机械数量等于</summary>
            emc_mechs_equal,
            /// <summary>敌方机械数量不等于</summary>
            emc_mechs_notequal,
            /// <summary>敌方机械数量大于</summary>
            emc_mechs_greater,
            /// <summary>敌方机械数量小于</summary>
            emc_mechs_less,
            /// <summary>己方野兽数量等于</summary>
            omc_beasts_equal,
            /// <summary>己方野兽数量不等于</summary>
            omc_beasts_notequal,
            /// <summary>己方野兽数量大于</summary>
            omc_beasts_greater,
            /// <summary>己方野兽数量小于</summary>
            omc_beasts_less,
            /// <summary>敌方野兽数量等于</summary>
            emc_beasts_equal,
            /// <summary>敌方野兽数量不等于</summary>
            emc_beasts_notequal,
            /// <summary>敌方野兽数量大于</summary>
            emc_beasts_greater,
            /// <summary>敌方野兽数量小于</summary>
            emc_beasts_less,
            /// <summary>己方图腾数量等于</summary>
            omc_totems_equal,
            /// <summary>己方图腾数量不等于</summary>
            omc_totems_notequal,
            /// <summary>己方图腾数量大于</summary>
            omc_totems_greater,
            /// <summary>己方图腾数量小于</summary>
            omc_totems_less,
            /// <summary>敌方图腾数量等于</summary>
            emc_totems_equal,
            /// <summary>敌方图腾数量不等于</summary>
            emc_totems_notequal,
            /// <summary>敌方图腾数量大于</summary>
            emc_totems_greater,
            /// <summary>敌方图腾数量小于</summary>
            emc_totems_less,
            /// <summary>己方海盗数量等于</summary>
            omc_pirates_equal,
            /// <summary>己方海盗数量不等于</summary>
            omc_pirates_notequal,
            /// <summary>己方海盗数量大于</summary>
            omc_pirates_greater,
            /// <summary>己方海盗数量小于</summary>
            omc_pirates_less,
            /// <summary>敌方海盗数量等于</summary>
            emc_pirates_equal,
            /// <summary>敌方海盗数量不等于</summary>
            emc_pirates_notequal,
            /// <summary>敌方海盗数量大于</summary>
            emc_pirates_greater,
            /// <summary>敌方海盗数量小于</summary>
            emc_pirates_less,
            /// <summary>己方龙数量等于</summary>
            omc_Dragons_equal,
            /// <summary>己方龙数量不等于</summary>
            omc_Dragons_notequal,
            /// <summary>己方龙数量大于</summary>
            omc_Dragons_greater,
            /// <summary>己方龙数量小于</summary>
            omc_Dragons_less,
            /// <summary>敌方龙数量等于</summary>
            emc_Dragons_equal,
            /// <summary>敌方龙数量不等于</summary>
            emc_Dragons_notequal,
            /// <summary>敌方龙数量大于</summary>
            emc_Dragons_greater,
            /// <summary>敌方龙数量小于</summary>
            emc_Dragons_less,
            /// <summary>己方元素数量等于</summary>
            omc_elems_equal,
            /// <summary>己方元素数量不等于</summary>
            omc_elems_notequal,
            /// <summary>己方元素数量大于</summary>
            omc_elems_greater,
            /// <summary>己方元素数量小于</summary>
            omc_elems_less,
            /// <summary>敌方元素数量等于</summary>
            emc_elems_equal,
            /// <summary>敌方元素数量不等于</summary>
            emc_elems_notequal,
            /// <summary>敌方元素数量大于</summary>
            emc_elems_greater,
            /// <summary>敌方元素数量小于</summary>
            emc_elems_less,
            /// <summary>己方白银之手新兵数量等于</summary>
            omc_shr_equal,
            /// <summary>己方白银之手新兵数量不等于</summary>
            omc_shr_notequal,
            /// <summary>己方白银之手新兵数量大于</summary>
            omc_shr_greater,
            /// <summary>己方白银之手新兵数量小于</summary>
            omc_shr_less,
            /// <summary>敌方白银之手新兵数量等于</summary>
            emc_shr_equal,
            /// <summary>敌方白银之手新兵数量不等于</summary>
            emc_shr_notequal,
            /// <summary>敌方白银之手新兵数量大于</summary>
            emc_shr_greater,
            /// <summary>敌方白银之手新兵数量小于</summary>
            emc_shr_less,
            /// <summary>己方未受伤随从数量等于</summary>
            omc_undamaged_equal,
            /// <summary>己方未受伤随从数量不等于</summary>
            omc_undamaged_notequal,
            /// <summary>己方未受伤随从数量大于</summary>
            omc_undamaged_greater,
            /// <summary>己方未受伤随从数量小于</summary>
            omc_undamaged_less,
            /// <summary>敌方未受伤随从数量等于</summary>
            emc_undamaged_equal,
            /// <summary>敌方未受伤随从数量不等于</summary>
            emc_undamaged_notequal,
            /// <summary>敌方未受伤随从数量大于</summary>
            emc_undamaged_greater,
            /// <summary>敌方未受伤随从数量小于</summary>
            emc_undamaged_less,
            /// <summary>己方受伤随从数量等于</summary>
            omc_damaged_equal,
            /// <summary>己方受伤随从数量不等于</summary>
            omc_damaged_notequal,
            /// <summary>己方受伤随从数量大于</summary>
            omc_damaged_greater,
            /// <summary>己方受伤随从数量小于</summary>
            omc_damaged_less,
            /// <summary>敌方受伤随从数量等于</summary>
            emc_damaged_equal,
            /// <summary>敌方受伤随从数量不等于</summary>
            emc_damaged_notequal,
            /// <summary>敌方受伤随从数量大于</summary>
            emc_damaged_greater,
            /// <summary>敌方受伤随从数量小于</summary>
            emc_damaged_less,
            /// <summary>己方圣盾随从数量等于</summary>
            omc_shields_equal,
            /// <summary>己方圣盾随从数量不等于</summary>
            omc_shields_notequal,
            /// <summary>己方圣盾随从数量大于</summary>
            omc_shields_greater,
            /// <summary>己方圣盾随从数量小于</summary>
            omc_shields_less,
            /// <summary>敌方圣盾随从数量等于</summary>
            emc_shields_equal,
            /// <summary>敌方圣盾随从数量不等于</summary>
            emc_shields_notequal,
            /// <summary>敌方圣盾随从数量大于</summary>
            emc_shields_greater,
            /// <summary>敌方圣盾随从数量小于</summary>
            emc_shields_less,
            /// <summary>己方嘲讽随从数量等于</summary>
            omc_taunts_equal,
            /// <summary>己方嘲讽随从数量不等于</summary>
            omc_taunts_notequal,
            /// <summary>己方嘲讽随从数量大于</summary>
            omc_taunts_greater,
            /// <summary>己方嘲讽随从数量小于</summary>
            omc_taunts_less,
            /// <summary>敌方嘲讽随从数量等于</summary>
            emc_taunts_equal,
            /// <summary>敌方嘲讽随从数量不等于</summary>
            emc_taunts_notequal,
            /// <summary>敌方嘲讽随从数量大于</summary>
            emc_taunts_greater,
            /// <summary>敌方嘲讽随从数量小于</summary>
            emc_taunts_less,
            /// <summary>攻击者攻击力等于</summary>
            aAt_equal,
            /// <summary>攻击者攻击力不等于</summary>
            aAt_notequal,
            /// <summary>攻击者攻击力大于</summary>
            aAt_greater,
            /// <summary>攻击者攻击力小于</summary>
            aAt_less,
            /// <summary>攻击者生命值等于</summary>
            aHp_equal,
            /// <summary>攻击者生命值不等于</summary>
            aHp_notequal,
            /// <summary>攻击者生命值大于</summary>
            aHp_greater,
            /// <summary>攻击者生命值小于</summary>
            aHp_less,
            /// <summary>目标攻击力等于</summary>
            tAt_equal,
            /// <summary>目标攻击力不等于</summary>
            tAt_notequal,
            /// <summary>目标攻击力大于</summary>
            tAt_greater,
            /// <summary>目标攻击力小于</summary>
            tAt_less,
            /// <summary>目标生命值等于</summary>
            tHp_equal,
            /// <summary>目标生命值不等于</summary>
            tHp_notequal,
            /// <summary>目标生命值大于</summary>
            tHp_greater,
            /// <summary>目标生命值小于</summary>
            tHp_less,
            /// <summary>己方手牌数量等于</summary>
            ohc_equal,
            /// <summary>己方手牌数量不等于</summary>
            ohc_notequal,
            /// <summary>己方手牌数量大于</summary>
            ohc_greater,
            /// <summary>己方手牌数量小于</summary>
            ohc_less,
            /// <summary>敌方手牌数量等于</summary>
            ehc_equal,
            /// <summary>敌方手牌数量不等于</summary>
            ehc_notequal,
            /// <summary>敌方手牌数量大于</summary>
            ehc_greater,
            /// <summary>敌方手牌数量小于</summary>
            ehc_less,
            /// <summary>己方手牌数量等于敌方</summary>
            ohc_equal_ehc,
            /// <summary>己方手牌数量不等于敌方</summary>
            ohc_notequal_ehc,
            /// <summary>己方手牌数量大于敌方</summary>
            ohc_greater_ehc,
            /// <summary>己方手牌数量小于敌方</summary>
            ohc_less_ehc,
            /// <summary>己方手牌中随从数量等于</summary>
            ohc_minions_equal,
            /// <summary>己方手牌中随从数量不等于</summary>
            ohc_minions_notequal,
            /// <summary>己方手牌中随从数量大于</summary>
            ohc_minions_greater,
            /// <summary>己方手牌中随从数量小于</summary>
            ohc_minions_less,
            /// <summary>己方手牌中法术数量等于</summary>
            ohc_spells_equal,
            /// <summary>己方手牌中法术数量不等于</summary>
            ohc_spells_notequal,
            /// <summary>己方手牌中法术数量大于</summary>
            ohc_spells_greater,
            /// <summary>己方手牌中法术数量小于</summary>
            ohc_spells_less,
            /// <summary>己方手牌中奥秘数量等于</summary>
            ohc_secrets_equal,
            /// <summary>己方手牌中奥秘数量不等于</summary>
            ohc_secrets_notequal,
            /// <summary>己方手牌中奥秘数量大于</summary>
            ohc_secrets_greater,
            /// <summary>己方手牌中奥秘数量小于</summary>
            ohc_secrets_less,
            /// <summary>己方手牌中武器数量等于</summary>
            ohc_weapons_equal,
            /// <summary>己方手牌中武器数量不等于</summary>
            ohc_weapons_notequal,
            /// <summary>己方手牌中武器数量大于</summary>
            ohc_weapons_greater,
            /// <summary>己方手牌中武器数量小于</summary>
            ohc_weapons_less,
            /// <summary>己方手牌中鱼人数量等于</summary>
            ohc_murlocs_equal,
            /// <summary>己方手牌中鱼人数量不等于</summary>
            ohc_murlocs_notequal,
            /// <summary>己方手牌中鱼人数量大于</summary>
            ohc_murlocs_greater,
            /// <summary>己方手牌中鱼人数量小于</summary>
            ohc_murlocs_less,
            /// <summary>己方手牌中恶魔数量等于</summary>
            ohc_demons_equal,
            /// <summary>己方手牌中恶魔数量不等于</summary>
            ohc_demons_notequal,
            /// <summary>己方手牌中恶魔数量大于</summary>
            ohc_demons_greater,
            /// <summary>己方手牌中恶魔数量小于</summary>
            ohc_demons_less,
            /// <summary>己方手牌中机械数量等于</summary>
            ohc_mechs_equal,
            /// <summary>己方手牌中机械数量不等于</summary>
            ohc_mechs_notequal,
            /// <summary>己方手牌中机械数量大于</summary>
            ohc_mechs_greater,
            /// <summary>己方手牌中机械数量小于</summary>
            ohc_mechs_less,
            /// <summary>己方手牌中野兽数量等于</summary>
            ohc_beasts_equal,
            /// <summary>己方手牌中野兽数量不等于</summary>
            ohc_beasts_notequal,
            /// <summary>己方手牌中野兽数量大于</summary>
            ohc_beasts_greater,
            /// <summary>己方手牌中野兽数量小于</summary>
            ohc_beasts_less,
            /// <summary>己方手牌中图腾数量等于</summary>
            ohc_totems_equal,
            /// <summary>己方手牌中图腾数量不等于</summary>
            ohc_totems_notequal,
            /// <summary>己方手牌中图腾数量大于</summary>
            ohc_totems_greater,
            /// <summary>己方手牌中图腾数量小于</summary>
            ohc_totems_less,
            /// <summary>己方手牌中海盗数量等于</summary>
            ohc_pirates_equal,
            /// <summary>己方手牌中海盗数量不等于</summary>
            ohc_pirates_notequal,
            /// <summary>己方手牌中海盗数量大于</summary>
            ohc_pirates_greater,
            /// <summary>己方手牌中海盗数量小于</summary>
            ohc_pirates_less,
            /// <summary>己方手牌中龙数量等于</summary>
            ohc_Dragons_equal,
            /// <summary>己方手牌中龙数量不等于</summary>
            ohc_Dragons_notequal,
            /// <summary>己方手牌中龙数量大于</summary>
            ohc_Dragons_greater,
            /// <summary>己方手牌中龙数量小于</summary>
            ohc_Dragons_less,
            /// <summary>己方手牌中元素数量等于</summary>
            ohc_elems_equal,
            /// <summary>己方手牌中元素数量不等于</summary>
            ohc_elems_notequal,
            /// <summary>己方手牌中元素数量大于</summary>
            ohc_elems_greater,
            /// <summary>己方手牌中元素数量小于</summary>
            ohc_elems_less,
            /// <summary>己方手牌中圣盾数量等于</summary>
            ohc_shields_equal,
            /// <summary>己方手牌中圣盾数量不等于</summary>
            ohc_shields_notequal,
            /// <summary>己方手牌中圣盾数量大于</summary>
            ohc_shields_greater,
            /// <summary>己方手牌中圣盾数量小于</summary>
            ohc_shields_less,
            /// <summary>己方手牌中嘲讽数量等于</summary>
            ohc_taunts_equal,
            /// <summary>己方手牌中嘲讽数量不等于</summary>
            ohc_taunts_notequal,
            /// <summary>己方手牌中嘲讽数量大于</summary>
            ohc_taunts_greater,
            /// <summary>己方手牌中嘲讽数量小于</summary>
            ohc_taunts_less,
            /// <summary>回合数等于</summary>
            turn_equal,
            /// <summary>回合数不等于</summary>
            turn_notequal,
            /// <summary>回合数大于</summary>
            turn_greater,
            /// <summary>回合数小于</summary>
            turn_less,
            /// <summary>过载等于</summary>
            overload_equal,
            /// <summary>过载不等于</summary>
            overload_notequal,
            /// <summary>过载大于</summary>
            overload_greater,
            /// <summary>过载小于</summary>
            overload_less,
            /// <summary>额外抽牌等于</summary>
            owncarddraw_equal,
            /// <summary>额外抽牌不等于</summary>
            owncarddraw_notequal,
            /// <summary>额外抽牌大于</summary>
            owncarddraw_greater,
            /// <summary>额外抽牌小于</summary>
            owncarddraw_less,
            /// <summary>己方英雄生命值等于</summary>
            ohhp_equal,
            /// <summary>己方英雄生命值不等于</summary>
            ohhp_notequal,
            /// <summary>己方英雄生命值大于</summary>
            ohhp_greater,
            /// <summary>己方英雄生命值小于</summary>
            ohhp_less,
            /// <summary>敌方英雄生命值等于</summary>
            ehhp_equal,
            /// <summary>敌方英雄生命值不等于</summary>
            ehhp_notequal,
            /// <summary>敌方英雄生命值大于</summary>
            ehhp_greater,
            /// <summary>敌方英雄生命值小于</summary>
            ehhp_less,
            /// <summary>己方场包含</summary>
            ownboard_contain,
            /// <summary>己方场不包含</summary>
            ownboard_notcontain,
            /// <summary>敌方场包含</summary>
            enboard_contain,
            /// <summary>敌方场不包含</summary>
            enboard_notcontain,
            /// <summary>己方手牌包含</summary>
            ownhand_contain,
            /// <summary>己方手牌不包含</summary>
            ownhand_notcontain,
            /// <summary>己方武器等于</summary>
            ownweapon_equal,
            /// <summary>己方武器不等于</summary>
            ownweapon_notequal,
            /// <summary>敌方武器等于</summary>
            enweapon_equal,
            /// <summary>敌方武器不等于</summary>
            enweapon_notequal,
            /// <summary>己方英雄等于</summary>
            ownhero_equal,
            /// <summary>己方英雄不等于</summary>
            ownhero_notequal,
            /// <summary>敌方英雄等于</summary>
            enhero_equal,
            /// <summary>敌方英雄不等于</summary>
            enhero_notequal,
            /// <summary>目标等于</summary>
            tgt_equal,
            /// <summary>目标不等于</summary>
            tgt_notequal,
            /// <summary>无重复</summary>
            noduplicates,
            /// <summary>播放</summary>
            play,
            /// <summary>播放2</summary>
            play2,
            /// <summary>攻击者</summary>
            attacker,
            /// <summary>终极规则</summary>
            ur,
            /// <summary>规则编号</summary>
            rn,
            /// <summary>替换规则</summary>
            rr,
            /// <summary>奖励</summary>
            bonus
        }

        /// <summary>
        /// 条件类
        /// </summary>
        public class Condition
        {
            /// <summary>参数</summary>
            public param parameter = param.None;
            /// <summary>数值</summary>
            public int num = int.MinValue;
            /// <summary>英雄职业</summary>
            public TAG_CLASS hClass = TAG_CLASS.INVALID;
            /// <summary>卡牌ID</summary>
            public CardDB.cardIDEnum cardID = CardDB.cardIDEnum.None;
            /// <summary>卡牌数量</summary>
            public int numCards = 0;
            /// <summary>奖励</summary>
            public int bonus = 0;
            /// <summary>或条件编号</summary>
            public int orCondNum = -1;
            /// <summary>或条件列表</summary>
            public List<Condition> orConditions = new List<Condition>();
            /// <summary>额外条件列表</summary>
            public List<Condition> extraConditions = new List<Condition>();
            /// <summary>父规则</summary>
            public string parentRule = "";

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="paramtr">参数</param>
            /// <param name="pnum">数值</param>
            /// <param name="pRule">父规则</param>
            public Condition(param paramtr, int pnum, string pRule)
            {
                this.parameter = paramtr;
                this.num = pnum;
                this.parentRule = pRule;
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="paramtr">参数</param>
            /// <param name="cID">卡牌ID</param>
            /// <param name="pRule">父规则</param>
            public Condition(param paramtr, CardDB.cardIDEnum cID, string pRule)
            {
                this.parameter = paramtr;
                this.cardID = cID;
                this.parentRule = pRule;
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="paramtr">参数</param>
            /// <param name="hClas">英雄职业</param>
            /// <param name="pRule">父规则</param>
            public Condition(param paramtr, TAG_CLASS hClas, string pRule)
            {
                this.parameter = paramtr;
                this.hClass = hClas;
                this.parentRule = pRule;
            }
        }

        /// <summary>
        /// 规则类
        /// </summary>
        public class Rule
        {
            /// <summary>是否为终极规则</summary>
            public bool ultimateRule = false;
            /// <summary>规则编号</summary>
            public int ruleNumber = 0;
            /// <summary>替换规则</summary>
            public int replacedRule = 0;
            /// <summary>奖励</summary>
            public int bonus = 0;
            /// <summary>条件列表</summary>
            public List<Condition> conditions = new List<Condition>();
        }

        /// <summary>
        /// 动作单元类
        /// </summary>
        public class actUnit
        {
            /// <summary>卡牌ID</summary>
            public CardDB.cardIDEnum cardID = CardDB.cardIDEnum.None;
            /// <summary>动作</summary>
            public Action action = null;
            /// <summary>实体</summary>
            public int entity = -1;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="cid">卡牌ID</param>
            /// <param name="a">动作</param>
            /// <param name="ent">实体</param>
            public actUnit(CardDB.cardIDEnum cid, Action a, int ent)
            {
                this.cardID = cid;
                this.action = a;
                this.entity = ent;
            }
        }

        /// <summary>
        /// 单例实例
        /// </summary>
        private static RulesEngine instance;

        /// <summary>
        /// 获取RulesEngine的单例实例
        /// </summary>
        public static RulesEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RulesEngine();
                }
                return instance;
            }
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private RulesEngine()
        {
        }

        /// <summary>
        /// 获取规则权重
        /// </summary>
        /// <param name="p">游戏场状态</param>
        /// <returns>规则权重</returns>
        public int getRuleWeight(Playfield p)
        {
            int weight = 0;
            List<int> possibleRules = new List<int>();
            possibleRules.AddRange(this.BoardStateRulesTurn.Keys);
            Dictionary<CardDB.cardIDEnum, int> handCardsWRule = new Dictionary<CardDB.cardIDEnum, int>();
            Dictionary<CardDB.cardIDEnum, List<actUnit>> playedCardsWRule = new Dictionary<CardDB.cardIDEnum, List<actUnit>>();
            Dictionary<CardDB.cardIDEnum, int> playedCardsWRulePen = new Dictionary<CardDB.cardIDEnum, int>();
            Dictionary<CardDB.cardIDEnum, List<actUnit>> attackersWRule = new Dictionary<CardDB.cardIDEnum, List<actUnit>>();
            Dictionary<CardDB.cardIDEnum, int> attackersWRulePen = new Dictionary<CardDB.cardIDEnum, int>();
            foreach (Action a in p.playactions)
            {
                CardDB.cardIDEnum cardID = CardDB.cardIDEnum.None;
                switch (a.actionType)
                {
                    case actionEnum.playcard:
                        cardID = a.card.card.cardIDenum;
                        if (CardIdRulesGame.ContainsKey(cardID))
                        {
                            possibleRules.AddRange(CardIdRulesGame[cardID].Keys);
                            if (playedCardsWRule.ContainsKey(cardID))
                            {
                                playedCardsWRule[cardID].Add(new actUnit(cardID, a, a.card.entity));
                            }
                            else
                            {
                                playedCardsWRule.Add(cardID, new List<actUnit> { new actUnit(cardID, a, a.card.entity) });
                                playedCardsWRulePen.Add(cardID, 0);
                            }
                        }
                        break;
                    case actionEnum.attackWithMinion:
                        cardID = a.own.handcard.card.cardIDenum;
                        if (AttackerIdRulesGame.ContainsKey(cardID))
                        {
                            possibleRules.AddRange(AttackerIdRulesGame[cardID].Keys);
                            if (attackersWRule.ContainsKey(cardID))
                            {
                                attackersWRule[cardID].Add(new actUnit(cardID, a, a.own.entitiyID));
                            }
                            else
                            {
                                attackersWRule.Add(cardID, new List<actUnit> { new actUnit(cardID, a, a.own.entitiyID) });
                                attackersWRulePen.Add(cardID, 0);
                            }
                        }
                        break;
                    case actionEnum.attackWithHero: break;
                    case actionEnum.useHeroPower: break;
                }
            }
            if (possibleRules.Count > 0)
            {
                p.rulesUsed = "";
                possibleRules = possibleRules.Distinct().ToList();
                int count = possibleRules.Count;
                for (int i = 0; i < count; i++)
                {
                    int ruleNum = possibleRules[i];
                    bool ruleBroken = false;
                    List<Tuple<Condition, List<actUnit>>> tmpPen = new List<Tuple<Condition, List<actUnit>>>();
                    foreach (Condition cond in heapOfRules[ruleNum].conditions)
                    {
                        if (cond.orCondNum < 0)
                        {
                            switch (cond.parameter)
                            {
                                
                                case param.play:
                                    if (playedCardsWRule.ContainsKey(cond.cardID))
                                    {
                                        tmpPen.Add(new Tuple<Condition, List<actUnit>>(cond, playedCardsWRule[cond.cardID]));
                                        continue;
                                    }
                                    ruleBroken = true;
                                    continue;
                                case param.play2:
                                    if (playedCardsWRule.ContainsKey(cond.cardID))
                                    {
                                        tmpPen.Add(new Tuple<Condition, List<actUnit>>(cond, playedCardsWRule[cond.cardID]));
                                        if (playedCardsWRule[cond.cardID].Count >= 2) continue;
                                    }
                                    ruleBroken = true;
                                    continue;
                                case param.attacker:
                                    if (attackersWRule.ContainsKey(cond.cardID))
                                    {
                                        tmpPen.Add(new Tuple<Condition, List<actUnit>>(cond, attackersWRule[cond.cardID]));
                                        continue;
                                    }
                                    ruleBroken = true;
                                    continue;
                                default:
                                    if (!ruleBroken && checkCondition(cond, p)) continue;
                                    ruleBroken = true;
                                    continue;
                            }
                        }
                        else
                        {
                            bool orCondBroken = true;
                            foreach (Condition orCond in cond.orConditions)
                            {
                                switch (orCond.parameter)
                                {
                                    
                                    case param.play:
                                        if (playedCardsWRule.ContainsKey(orCond.cardID))
                                        {
                                            tmpPen.Add(new Tuple<Condition, List<actUnit>>(orCond, playedCardsWRule[orCond.cardID]));
                                            orCondBroken = false;
                                        }
                                        break;
                                    case param.play2:
                                        if (playedCardsWRule.ContainsKey(orCond.cardID))
                                        {
                                            tmpPen.Add(new Tuple<Condition, List<actUnit>>(orCond, playedCardsWRule[orCond.cardID]));
                                            if (playedCardsWRule[orCond.cardID].Count >= 2) orCondBroken = false;
                                        }
                                        break;
                                    case param.attacker:
                                        if (attackersWRule.ContainsKey(cond.cardID))
                                        {
                                            tmpPen.Add(new Tuple<Condition, List<actUnit>>(cond, attackersWRule[cond.cardID]));
                                            continue;
                                        }
                                        ruleBroken = true;
                                        continue;
                                    default:
                                        if (checkCondition(orCond, p)) orCondBroken = false;
                                        break;
                                }
                                if (!orCondBroken) break;
                            }
                            if (orCondBroken) ruleBroken = true;
                        }
                    }

                    if (ruleBroken)
                    {
                        foreach (Tuple<Condition, List<actUnit>> condPen in tmpPen)
                        {
                            weight += condPen.Item1.bonus;
                            if (this.printRules > 0) p.rulesUsed += -condPen.Item1.bonus + " broken rule:" + condPen.Item1.parentRule + "@";
                        }
                    }
                    else
                    {
                        int tmpPenBonus = 0;
                        foreach (Tuple<Condition, List<actUnit>> condPen in tmpPen)
                        {
                            foreach (actUnit au in condPen.Item2)
                            {
                                bool actRuleBroken = false;
                                if (condPen.Item1.extraConditions.Count > 0)
                                {
                                    foreach (Condition extraCond in condPen.Item1.extraConditions)
                                    {
                                        if (checkCondition(extraCond, p, au.action)) continue;
                                        actRuleBroken = true;
                                        tmpPenBonus -= condPen.Item1.bonus;
                                        if (this.printRules > 0) p.rulesUsed += -condPen.Item1.bonus + " broken extra condition:" + condPen.Item1.parentRule + "@"; 
                                        break;
                                    }
                                }
                                if (!actRuleBroken)
                                {
                                    tmpPenBonus += heapOfRules[ruleNum].bonus;
                                    if (this.printRules > 0) p.rulesUsed += heapOfRules[ruleNum].bonus + " " + condPen.Item1.parentRule + "@";
                                }
                            }
                        }
                        if (tmpPen.Count > 0) weight -= tmpPenBonus;
                        else
                        {
                            weight -= heapOfRules[ruleNum].bonus;
                            if (this.printRules > 0)
                            {
                                string ruleStr = "no conditions";
                                if (heapOfRules[ruleNum].conditions.Count > 0) ruleStr = heapOfRules[ruleNum].conditions[0].parentRule;
                                p.rulesUsed += heapOfRules[ruleNum].bonus + ruleStr + "@";
                            }
                        }
                    }
                }
            }
            p.ruleWeight = weight;
            return weight;
        }

        /// <summary>
        /// 设置游戏卡牌ID规则
        /// </summary>
        /// <param name="ohc">己方英雄职业</param>
        /// <param name="ehc">敌方英雄职业</param>
        public void setCardIdRulesGame(TAG_CLASS ohc, TAG_CLASS ehc)
        {
            CardIdRulesGame.Clear();
            CardIdRulesPlayGame.Clear();
            CardIdRulesHandGame.Clear();
            CardIdRulesOwnBoardGame.Clear();
            CardIdRulesEnemyBoardGame.Clear();
            AttackerIdRulesGame.Clear();
            var sdf = heapOfRules;
            if (RuleOwnClass.ContainsKey(ohc) && RuleEnemyClass.ContainsKey(ehc))
            {
                foreach (int ruleNum in RuleOwnClass[ohc].Keys)
                {
                    if (RuleEnemyClass[ehc].ContainsKey(ruleNum))
                    {
                        if (RuleCardIdsPlay.ContainsKey(ruleNum)) addCardIdRulesGame(RuleCardIdsPlay, CardIdRulesPlayGame, ruleNum);
                        if (RuleCardIdsAttack.ContainsKey(ruleNum)) addAttackerIdRulesGame(ruleNum);
                        if (BoardStateRules.ContainsKey(ruleNum) && !BoardStateRulesGame.ContainsKey(ruleNum)) BoardStateRulesGame.Add(ruleNum, 0);
                    }
                }
            }
            if (RuleOwnClass.ContainsKey(ohc) && RuleEnemyClass.ContainsKey(TAG_CLASS.WHIZBANG))
            {
                foreach (int ruleNum in RuleOwnClass[ohc].Keys)
                {
                    if (RuleEnemyClass[TAG_CLASS.WHIZBANG].ContainsKey(ruleNum))
                    {
                        if (RuleCardIdsPlay.ContainsKey(ruleNum)) addCardIdRulesGame(RuleCardIdsPlay, CardIdRulesPlayGame, ruleNum);
                        if (RuleCardIdsAttack.ContainsKey(ruleNum)) addAttackerIdRulesGame(ruleNum);
                        if (BoardStateRules.ContainsKey(ruleNum) && !BoardStateRulesGame.ContainsKey(ruleNum)) BoardStateRulesGame.Add(ruleNum, 0);
                    }
                }
            }
            if (RuleOwnClass.ContainsKey(TAG_CLASS.WHIZBANG) && RuleEnemyClass.ContainsKey(ehc))
            {
                foreach (int ruleNum in RuleEnemyClass[ehc].Keys)
                {
                    if (RuleOwnClass[TAG_CLASS.WHIZBANG].ContainsKey(ruleNum))
                    {
                        if (RuleCardIdsPlay.ContainsKey(ruleNum)) addCardIdRulesGame(RuleCardIdsPlay, CardIdRulesPlayGame, ruleNum);
                        if (RuleCardIdsAttack.ContainsKey(ruleNum)) addAttackerIdRulesGame(ruleNum);
                        if (BoardStateRules.ContainsKey(ruleNum) && !BoardStateRulesGame.ContainsKey(ruleNum)) BoardStateRulesGame.Add(ruleNum, 0);
                    }
                }
            }
            if (RuleOwnClass.ContainsKey(TAG_CLASS.WHIZBANG) && RuleEnemyClass.ContainsKey(TAG_CLASS.WHIZBANG))
            {
                foreach (int ruleNum in RuleOwnClass[TAG_CLASS.WHIZBANG].Keys)
                {
                    if (RuleEnemyClass[TAG_CLASS.WHIZBANG].ContainsKey(ruleNum))
                    {
                        if (RuleCardIdsPlay.ContainsKey(ruleNum)) addCardIdRulesGame(RuleCardIdsPlay, CardIdRulesPlayGame, ruleNum);
                        if (RuleCardIdsAttack.ContainsKey(ruleNum)) addAttackerIdRulesGame(ruleNum);
                        if (BoardStateRules.ContainsKey(ruleNum) && !BoardStateRulesGame.ContainsKey(ruleNum)) BoardStateRulesGame.Add(ruleNum, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 添加卡牌ID规则到游戏
        /// </summary>
        /// <param name="baseDct">基础字典</param>
        /// <param name="targetDct">目标字典</param>
        /// <param name="ruleNum">规则编号</param>
        private void addCardIdRulesGame(Dictionary<int, List<CardDB.cardIDEnum>> baseDct, Dictionary<CardDB.cardIDEnum, Dictionary<int, int>> targetDct, int ruleNum)
        {
            foreach (CardDB.cardIDEnum cid in baseDct[ruleNum])
            {
                if (targetDct.ContainsKey(cid))
                {
                    if (replacedRules.ContainsKey(ruleNum))
                    {
                        var oldRules = targetDct[cid];
                        if (oldRules.ContainsKey(replacedRules[ruleNum])) oldRules.Remove(replacedRules[ruleNum]);
                    }
                    if (!targetDct[cid].ContainsKey(ruleNum)) targetDct[cid].Add(ruleNum, 0);
                }
                else targetDct.Add(cid, new Dictionary<int, int>() { { ruleNum, 0 } });
            }

            foreach (CardDB.cardIDEnum cid in baseDct[ruleNum])
            {
                if (CardIdRulesGame.ContainsKey(cid))
                {
                    if (replacedRules.ContainsKey(ruleNum))
                    {
                        var oldRules = CardIdRulesGame[cid];
                        if (oldRules.ContainsKey(replacedRules[ruleNum])) oldRules.Remove(replacedRules[ruleNum]);
                    }
                    if (!CardIdRulesGame[cid].ContainsKey(ruleNum)) CardIdRulesGame[cid].Add(ruleNum, 0);
                }
                else CardIdRulesGame.Add(cid, new Dictionary<int, int>() { { ruleNum, 0 } });
            }
        }

        /// <summary>
        /// 添加攻击者ID规则到游戏
        /// </summary>
        /// <param name="ruleNum">规则编号</param>
        private void addAttackerIdRulesGame(int ruleNum)
        {
            foreach (CardDB.cardIDEnum cid in RuleCardIdsAttack[ruleNum])
            {
                if (AttackerIdRulesGame.ContainsKey(cid))
                {
                    if (replacedRules.ContainsKey(ruleNum))
                    {
                        var oldRules = AttackerIdRulesGame[cid];
                        if (oldRules.ContainsKey(replacedRules[ruleNum])) oldRules.Remove(replacedRules[ruleNum]);
                    }
                    if (!AttackerIdRulesGame[cid].ContainsKey(ruleNum)) AttackerIdRulesGame[cid].Add(ruleNum, 0);
                }
                else AttackerIdRulesGame.Add(cid, new Dictionary<int, int>() { { ruleNum, 0 } });
            }
        }

        /// <summary>
        /// 设置回合规则
        /// </summary>
        /// <param name="gTurn">游戏回合</param>
        public void setRulesTurn(int gTurn)
        {
            BoardStateRulesTurn.Clear();
            foreach (int rNum in BoardStateRulesGame.Keys)
            {
                bool gonext = false;
                bool noturnrule = true;
                foreach (Condition cond in heapOfRules[rNum].conditions)
                {
                    if (gonext) break;
                    if (cond.orCondNum < 0)
                    {
                        switch (cond.parameter)
                        {
                            case param.turn_equal:
                                noturnrule = false;
                                if (gTurn == cond.num)
                                {
                                    BoardStateRulesTurn.Add(rNum, 0);
                                    gonext = true;
                                }
                                continue;
                            case param.turn_notequal:
                                noturnrule = false;
                                if (gTurn == cond.num)
                                {
                                    if (BoardStateRulesTurn.ContainsKey(gTurn)) BoardStateRulesTurn.Remove(gTurn);
                                    gonext = true;
                                }
                                continue;
                            case param.turn_greater:
                                noturnrule = false;
                                if (gTurn > cond.num) BoardStateRulesTurn.Add(rNum, 0);
                                continue;
                            case param.turn_less:
                                noturnrule = false;
                                if (gTurn < cond.num) BoardStateRulesTurn.Add(rNum, 0);
                                continue;
                        }
                    }
                    else
                    {
                        foreach (Condition orCond in cond.orConditions)
                        {
                            switch (cond.parameter)
                            {
                                case param.turn_equal:
                                    noturnrule = false;
                                    if (gTurn == cond.num)
                                    {
                                        BoardStateRulesTurn.Add(rNum, 0);
                                        gonext = true;
                                    }
                                    continue;
                                case param.turn_notequal:
                                    noturnrule = false;
                                    if (gTurn == cond.num)
                                    {
                                        if (BoardStateRulesTurn.ContainsKey(gTurn)) BoardStateRulesTurn.Remove(gTurn);
                                        gonext = true;
                                    }
                                    continue;
                                case param.turn_greater:
                                    noturnrule = false;
                                    if (gTurn > cond.num) BoardStateRulesTurn.Add(rNum, 0);
                                    continue;
                                case param.turn_less:
                                    noturnrule = false;
                                    if (gTurn < cond.num) BoardStateRulesTurn.Add(rNum, 0);
                                    continue;
                            }
                        }
                    }
                }
                if (noturnrule) BoardStateRulesTurn.Add(rNum, 0);
            }
        }

        /// <summary>
        /// 读取规则
        /// </summary>
        /// <param name="behavName">行为名称或路径</param>
        /// <param name="nameIsPath">名称是否为路径</param>
        public void readRules(string behavName, bool nameIsPath = false)
        {
            pathToRules = behavName;
            if (!nameIsPath)
            {
                if (MulliganRulesDB.ContainsKey(behavName))
                {
                    MulliganRules = MulliganRulesDB[behavName];
                    mulliganRulesLoaded = true;
                    return;
                }

                if (!Silverfish.Instance.BehaviorPath.ContainsKey(behavName))
                {
                    Helpfunctions.Instance.ErrorLog(behavName + ": no special rules.");
                    return;
                }
                pathToRules = Path.Combine(Silverfish.Instance.BehaviorPath[behavName], "_rules.txt");
            }

            if (!System.IO.File.Exists(pathToRules))
            {
                Helpfunctions.Instance.ErrorLog(behavName + ": no special rules.");
                return;
            }
            try
            {
                string[] lines = System.IO.File.ReadAllLines(pathToRules);
                string tmps;
                bool getNextRule;
                MulliganRules.Clear();
                List<Rule> rulesList = new List<Rule>();
                foreach (string s in lines)
                {
                    getNextRule = false;
                    if (s == "" || s == null) continue;
                    if (s.StartsWith("//")) continue;
                    string[] preRule = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    Rule oneRule = new Rule();
                    List<Condition> conditions = new List<Condition>();
                    int orCondNum = 1;
                    foreach (string ss in preRule)
                    {
                        if (getNextRule) break;
                        String[] tmp = ss.Split('=');
                        string condition = tmp[0];
                        switch (condition)
                        {
                            case "ur":
                                oneRule.ultimateRule = true;
                                continue;
                            case "rn":
                                try { oneRule.ruleNumber = Convert.ToInt32(tmp[1]); }
                                catch
                                {
                                    Helpfunctions.Instance.ErrorLog("[RulesEngine] Wrong rule number (must be a number): " + ss);
                                    getNextRule = true;
                                }
                                continue;
                            case "rr":
                                try { oneRule.replacedRule = Convert.ToInt32(tmp[1]); }
                                catch
                                {
                                    Helpfunctions.Instance.ErrorLog("[RulesEngine] Wrong replaced rule number (must be a number): " + ss);
                                    getNextRule = true;
                                }
                                continue;
                            case "bonus":
                                try { oneRule.bonus = Convert.ToInt32(tmp[1]); }
                                catch
                                {
                                    Helpfunctions.Instance.ErrorLog("[RulesEngine] Wrong bonus (must be a number): " + ss);
                                    getNextRule = true;
                                }
                                continue;
                            default:
                                String[] condAnd = ss.Split('&');
                                foreach (string singlecondAnd in condAnd)
                                {
                                    if (getNextRule) break;
                                    if (singlecondAnd.Contains("|"))
                                    {
                                        tmps = singlecondAnd.Replace("||", "|");
                                        String[] condOr = tmps.Split('|');
                                        Condition orCondidion = new Condition(param.orcond, condOr.Count(), (this.printRules == 0) ? "" : s);
                                        orCondidion.orCondNum = orCondNum;
                                        foreach (string singlecondOr in condOr)
                                        {
                                            if (!validateCondition(singlecondOr, s))
                                            {
                                                Helpfunctions.Instance.ErrorLog("[RulesEngine] " + condErr + singlecondOr);
                                                getNextRule = true;
                                            }
                                            if (getNextRule) break;
                                            condTmp.orCondNum = orCondNum;
                                            orCondidion.orConditions.Add(condTmp);
                                        }
                                        conditions.Add(orCondidion);
                                        orCondNum++;
                                        continue;
                                    }
                                    if (validateCondition(singlecondAnd, s))
                                    {
                                        conditions.Add(condTmp);
                                        continue;
                                    }
                                    else
                                    {
                                        Helpfunctions.Instance.ErrorLog("[RulesEngine] " + condErr + singlecondAnd);
                                        getNextRule = true;
                                    }
                                }
                                continue;
                        }
                    }
                    if (getNextRule) continue;
                    oneRule.conditions = conditions;
                    rulesList.Add(oneRule);
                }

                heapOfRules.Clear();
                replacedRules.Clear();
                foreach (Rule r in rulesList) 
                {
                    if (r.ruleNumber == 0) continue;
                    if (heapOfRules.ContainsKey(r.ruleNumber))
                    {
                        Helpfunctions.Instance.ErrorLog("[RulesEngine] Rule rejected. Duplicate numbers: rn=" + r.ruleNumber);
                    }
                    else
                    {
                        heapOfRules.Add(r.ruleNumber, r);
                    }
                }

                Dictionary<int, Rule> tmpRules = new Dictionary<int, Rule>();
                int i = 1;
                foreach (Rule r in rulesList) 
                {
                    if (r.ruleNumber != 0) continue;
                    while (heapOfRules.ContainsKey(i)) i++;
                    r.ruleNumber = i;
                    tmpRules.Add(i, r);
                    i++;
                }
                foreach (Rule r in rulesList) 
                {
                    if (r.replacedRule == 0) continue;
                    if (heapOfRules.ContainsKey(r.replacedRule))
                    {
                        replacedRules.Add(r.ruleNumber, r.replacedRule);
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("[RulesEngine] No rule to replace: rr=" + r.replacedRule);
                        r.replacedRule = 0;
                    }
                }
                foreach (var r in tmpRules) 
                {
                    if (heapOfRules.ContainsKey(r.Key))
                    {
                        Helpfunctions.Instance.ErrorLog("[RulesEngine] Replaced rule rejected. Duplicate numbers: rr=" + r.Key);
                    }
                    else heapOfRules.Add(r.Key, r.Value);
                }

                Dictionary<TAG_CLASS, int> equalOwnHeroes = new Dictionary<TAG_CLASS, int>();
                Dictionary<TAG_CLASS, int> notequalOwnHeroes = new Dictionary<TAG_CLASS, int>();
                Dictionary<TAG_CLASS, int> equalEnHeroes = new Dictionary<TAG_CLASS, int>();
                Dictionary<TAG_CLASS, int> notequalEnHeroes = new Dictionary<TAG_CLASS, int>();

                foreach (var r in heapOfRules)
                {
                    equalOwnHeroes.Clear();
                    notequalOwnHeroes.Clear();
                    equalEnHeroes.Clear();
                    notequalEnHeroes.Clear();
                    foreach (Condition cond in getAllCondList(r.Value.conditions))
                    {
                        switch (cond.parameter)
                        {
                            case param.ownhero_equal:
                                if (equalOwnHeroes.ContainsKey(cond.hClass)) equalOwnHeroes[cond.hClass]++;
                                else equalOwnHeroes.Add(cond.hClass, 1);
                                continue;
                            case param.ownhero_notequal:
                                if (notequalOwnHeroes.ContainsKey(cond.hClass)) notequalOwnHeroes[cond.hClass]++;
                                else notequalOwnHeroes.Add(cond.hClass, 1);
                                continue;
                            case param.enhero_equal:
                                if (equalEnHeroes.ContainsKey(cond.hClass)) equalEnHeroes[cond.hClass]++;
                                else equalEnHeroes.Add(cond.hClass, 1);
                                continue;
                            case param.enhero_notequal:
                                if (notequalEnHeroes.ContainsKey(cond.hClass)) notequalEnHeroes[cond.hClass]++;
                                else notequalEnHeroes.Add(cond.hClass, 1);
                                continue;
                        }
                    }
                    if (equalOwnHeroes.Count > 0 || notequalOwnHeroes.Count > 0)
                    {
                        foreach (TAG_CLASS hClass in Enum.GetValues(typeof(TAG_CLASS)))
                        {
                            if (hClass == TAG_CLASS.INVALID || hClass == TAG_CLASS.WHIZBANG) continue;
                            if (equalOwnHeroes.ContainsKey(hClass))
                            {
                                if (equalOwnHeroes[hClass] > 1) Helpfunctions.Instance.ErrorLog("[RulesEngine] Double own Hero class (equal): " + hClass);
                                if (notequalOwnHeroes.ContainsKey(hClass)) Helpfunctions.Instance.ErrorLog("[RulesEngine] The same equal/notequal own Hero class: " + hClass);
                                if (RuleOwnClass.ContainsKey(hClass)) RuleOwnClass[hClass].Add(r.Key, 0); 
                                else RuleOwnClass.Add(hClass, new Dictionary<int, int>() { { r.Key, 0 } });
                            }
                            else if (equalOwnHeroes.Count < 1)
                            {
                                if (!notequalOwnHeroes.ContainsKey(hClass))
                                {
                                    if (notequalOwnHeroes[hClass] > 1) Helpfunctions.Instance.ErrorLog("[RulesEngine] Double own Hero class (notequal): " + hClass);
                                    if (RuleOwnClass.ContainsKey(hClass)) RuleOwnClass[hClass].Add(r.Key, 0); 
                                    else RuleOwnClass.Add(hClass, new Dictionary<int, int>() { { r.Key, 0 } });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (RuleOwnClass.ContainsKey(TAG_CLASS.WHIZBANG)) RuleOwnClass[TAG_CLASS.WHIZBANG].Add(r.Key, 0); 
                        else RuleOwnClass.Add(TAG_CLASS.WHIZBANG, new Dictionary<int, int>() { { r.Key, 0 } });
                    }
                    if (equalEnHeroes.Count > 0 || notequalEnHeroes.Count > 0)
                    {
                        foreach (TAG_CLASS hClass in Enum.GetValues(typeof(TAG_CLASS)))
                        {
                            if (hClass == TAG_CLASS.INVALID || hClass == TAG_CLASS.WHIZBANG) continue;
                            if (equalEnHeroes.ContainsKey(hClass))
                            {
                                if (equalEnHeroes[hClass] > 1) Helpfunctions.Instance.ErrorLog("[RulesEngine] Double enemy Hero class (equal): " + hClass);
                                if (notequalEnHeroes.ContainsKey(hClass)) Helpfunctions.Instance.ErrorLog("[RulesEngine] The same equal/notequal enemy Hero class: " + hClass);
                                if (RuleEnemyClass.ContainsKey(hClass)) RuleEnemyClass[hClass].Add(r.Key, 0); 
                                else RuleEnemyClass.Add(hClass, new Dictionary<int, int>() { { r.Key, 0 } });
                            }
                            else if (equalEnHeroes.Count < 1)
                            {
                                if (!notequalEnHeroes.ContainsKey(hClass))
                                {
                                    if (notequalEnHeroes[hClass] > 1) Helpfunctions.Instance.ErrorLog("[RulesEngine] Double enemy Hero class (notequal): " + hClass);
                                    if (RuleEnemyClass.ContainsKey(hClass)) RuleEnemyClass[hClass].Add(r.Key, 0); 
                                    else RuleEnemyClass.Add(hClass, new Dictionary<int, int>() { { r.Key, 0 } });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (RuleEnemyClass.ContainsKey(TAG_CLASS.WHIZBANG)) RuleEnemyClass[TAG_CLASS.WHIZBANG].Add(r.Key, 0); 
                        else RuleEnemyClass.Add(TAG_CLASS.WHIZBANG, new Dictionary<int, int>() { { r.Key, 0 } });
                    }
                }
            }
            catch (Exception e)
            {
                Helpfunctions.Instance.ErrorLog("[规则编辑器] _rules.txt - 文本读取错误. 我们将使用默认规则，放弃自定义规则. 异常:" + e.Message);
                return;
            }

            Helpfunctions.Instance.ErrorLog("[规则编辑器] " + heapOfRules.Count + " 规则名 " + behavName + " 读取成功");
            setRuleCardIds();
        }

        private List<Condition> getAllCondList(List<Condition> tmp)
        {
            List<Condition> allCondList = new List<Condition>();
            foreach (Condition cond in tmp)
            {
                if (cond.parameter == param.orcond) allCondList.AddRange(cond.orConditions);
                else allCondList.Add(cond);
            }
            return allCondList;
        }

        public void setRuleCardIds()
        {
            RuleCardIdsPlay.Clear();
            RuleCardIdsHand.Clear();
            RuleCardIdsOwnBoard.Clear();
            RuleCardIdsEnemyBoard.Clear();
            RuleCardIdsAttack.Clear();
            foreach (var oneRule in heapOfRules)
            {
                bool stateRule = false;
                bool playRule = false;
                List<CardDB.cardIDEnum> IDsListPlay = new List<CardDB.cardIDEnum>();
                List<CardDB.cardIDEnum> IDsListHand = new List<CardDB.cardIDEnum>();
                List<CardDB.cardIDEnum> IDsListOB = new List<CardDB.cardIDEnum>();
                List<CardDB.cardIDEnum> IDsListEB = new List<CardDB.cardIDEnum>();
                List<CardDB.cardIDEnum> IDsListAttack = new List<CardDB.cardIDEnum>();
                foreach (Condition cond in getAllCondList(oneRule.Value.conditions))
                {
                    switch (cond.parameter)
                    {
                        case param.play:
                            IDsListPlay.Add(cond.cardID);
                            playRule = true;
                            continue;
                        case param.play2:
                            IDsListPlay.Add(cond.cardID);
                            playRule = true;
                            continue;
                        case param.attacker:
                            IDsListAttack.Add(cond.cardID);
                            playRule = true;
                            continue;
                        case param.ownhero_equal:
                            continue;
                        case param.ownhero_notequal:
                            continue;
                        case param.enhero_equal:
                            continue;
                        case param.enhero_notequal:
                            continue;
                        case param.ownhand_contain:
                            IDsListHand.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        case param.ownhand_notcontain:
                            IDsListHand.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        case param.ownboard_contain:
                            IDsListOB.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        case param.ownboard_notcontain:
                            IDsListOB.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        case param.enboard_contain:
                            IDsListEB.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        case param.enboard_notcontain:
                            IDsListEB.Add(cond.cardID);
                            stateRule = true;
                            continue;
                        default:
                            continue;
                    }
                }
                if (IDsListPlay.Count > 0) RuleCardIdsPlay.Add(oneRule.Key, IDsListPlay.Distinct().ToList());
                if (IDsListHand.Count > 0) RuleCardIdsHand.Add(oneRule.Key, IDsListHand);
                if (IDsListOB.Count > 0) RuleCardIdsOwnBoard.Add(oneRule.Key, IDsListOB);
                if (IDsListEB.Count > 0) RuleCardIdsEnemyBoard.Add(oneRule.Key, IDsListEB);
                if (IDsListAttack.Count > 0) RuleCardIdsAttack.Add(oneRule.Key, IDsListAttack);
                if ((playRule && stateRule) || !playRule) BoardStateRules.Add(oneRule.Key, 0);
            }
        }


        private bool validateCondition(string singlecond, string ruleString)
        {
            switch (singlecond)
            {
                case "omc=emc": condTmp = new Condition(param.omc_equal_emc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "omc!=emc": condTmp = new Condition(param.omc_notequal_emc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "omc>emc": condTmp = new Condition(param.omc_greater_emc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "omc<emc": condTmp = new Condition(param.omc_less_emc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "ohc=ehc": condTmp = new Condition(param.ohc_equal_ehc, 0, (this.printRules == 0) ? "" : ruleString); return true;  
                case "ohc!=ehc": condTmp = new Condition(param.ohc_notequal_ehc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "ohc>ehc": condTmp = new Condition(param.ohc_greater_ehc, 0, (this.printRules == 0) ? "" : ruleString); return true;
                case "ohc<ehc": condTmp = new Condition(param.ohc_less_ehc, 0, (this.printRules == 0) ? "" : ruleString); return true;
            }

            condErr = "";
            String[] tmp;
            String[] extraParam = new string[0];
            string parameter = "";
            param condParam = param.None;
            string pval = "";
            int pvaltype = 0;


            if (singlecond.StartsWith("p="))
            {
                extraParam = singlecond.Split(':');
                singlecond = extraParam[0];
            }
            else if (singlecond.StartsWith("a="))
            {
                extraParam = singlecond.Split(':');
                singlecond = extraParam[0];
            }

            getSinglecond(singlecond, out tmp, out parameter);

            if (tmp.Length == 2)
            {
                switch (tmp[1])
                {
                    case "coin":
                        pval = "GAME_005";
                        break;
                    case "!coin":
                        pval = "GAME_005";
                        break;
                    default:
                        pval = tmp[1];
                        break;
                }
            }
            else if (tmp.Length == 1)
            {
                switch (tmp[0])
                {
                    case "noduplicates":
                        condTmp = new Condition(param.noduplicates, 0, (this.printRules == 0) ? "" : ruleString);
                        return true;
                    default:
                        condErr = "Wrong condition: ";
                        return false;
                }
            }
            else
            {
                condErr = "Wrong condition: ";
                return false;
            }

            parameter = (tmp[0] + parameter).ToLower();
            switch (parameter)
            {
                case "tm=": condParam = param.tm_equal; break; 
                case "tm!=": condParam = param.tm_notequal; break;
                case "tm>": condParam = param.tm_greater; break;
                case "tm<": condParam = param.tm_less; break;
                case "am=": condParam = param.am_equal; break; 
                case "am!=": condParam = param.am_notequal; break;
                case "am>": condParam = param.am_greater; break;
                case "am<": condParam = param.am_less; break;
                case "owa=": condParam = param.owa_equal; break; 
                case "owa!=": condParam = param.owa_notequal; break;
                case "owa>": condParam = param.owa_greater; break;
                case "owa<": condParam = param.owa_less; break;
                case "ewa=": condParam = param.ewa_equal; break; 
                case "ewa!=": condParam = param.ewa_notequal; break;
                case "ewa>": condParam = param.ewa_greater; break;
                case "ewa<": condParam = param.ewa_less; break;
                case "owd=": condParam = param.owd_equal; break; 
                case "owd!=": condParam = param.owd_notequal; break;
                case "owd>": condParam = param.owd_greater; break;
                case "owd<": condParam = param.owd_less; break;
                case "ewd=": condParam = param.ewd_equal; break; 
                case "ewd!=": condParam = param.ewd_notequal; break;
                case "ewd>": condParam = param.ewd_greater; break;
                case "ewd<": condParam = param.ewd_less; break;
                case "omc=": condParam = param.omc_equal; break; 
                case "omc!=": condParam = param.omc_notequal; break;
                case "omc>": condParam = param.omc_greater; break;
                case "omc<": condParam = param.omc_less; break;
                case "emc=": condParam = param.emc_equal; break; 
                case "emc!=": condParam = param.emc_notequal; break;
                case "emc>": condParam = param.emc_greater; break;
                case "emc<": condParam = param.emc_less; break;

                case "omc:murlocs=": condParam = param.omc_murlocs_equal; break;
                case "omc:murlocs!=": condParam = param.omc_murlocs_notequal; break;
                case "omc:murlocs>": condParam = param.omc_murlocs_greater; break;
                case "omc:murlocs<": condParam = param.omc_murlocs_less; break;
                case "emc:murlocs=": condParam = param.emc_murlocs_equal; break;
                case "emc:murlocs!=": condParam = param.emc_murlocs_notequal; break;
                case "emc:murlocs>": condParam = param.emc_murlocs_greater; break;
                case "emc:murlocs<": condParam = param.emc_murlocs_less; break;
                case "omc:demons=": condParam = param.omc_demons_equal; break;
                case "omc:demons!=": condParam = param.omc_demons_notequal; break;
                case "omc:demons>": condParam = param.omc_demons_greater; break;
                case "omc:demons<": condParam = param.omc_demons_less; break;
                case "emc:demons=": condParam = param.emc_demons_equal; break;
                case "emc:demons!=": condParam = param.emc_demons_notequal; break;
                case "emc:demons>": condParam = param.emc_demons_greater; break;
                case "emc:demons<": condParam = param.emc_demons_less; break;
                case "omc:mechs=": condParam = param.omc_mechs_equal; break;
                case "omc:mechs!=": condParam = param.omc_mechs_notequal; break;
                case "omc:mechs>": condParam = param.omc_mechs_greater; break;
                case "omc:mechs<": condParam = param.omc_mechs_less; break;
                case "emc:mechs=": condParam = param.emc_mechs_equal; break;
                case "emc:mechs!=": condParam = param.emc_mechs_notequal; break;
                case "emc:mechs>": condParam = param.emc_mechs_greater; break;
                case "emc:mechs<": condParam = param.emc_mechs_less; break;
                case "omc:beasts=": condParam = param.omc_beasts_equal; break;
                case "omc:beasts!=": condParam = param.omc_beasts_notequal; break;
                case "omc:beasts>": condParam = param.omc_beasts_greater; break;
                case "omc:beasts<": condParam = param.omc_beasts_less; break;
                case "emc:beasts=": condParam = param.emc_beasts_equal; break;
                case "emc:beasts!=": condParam = param.emc_beasts_notequal; break;
                case "emc:beasts>": condParam = param.emc_beasts_greater; break;
                case "emc:beasts<": condParam = param.emc_beasts_less; break;
                case "omc:totems=": condParam = param.omc_totems_equal; break;
                case "omc:totems!=": condParam = param.omc_totems_notequal; break;
                case "omc:totems>": condParam = param.omc_totems_greater; break;
                case "omc:totems<": condParam = param.omc_totems_less; break;
                case "emc:totems=": condParam = param.emc_totems_equal; break;
                case "emc:totems!=": condParam = param.emc_totems_notequal; break;
                case "emc:totems>": condParam = param.emc_totems_greater; break;
                case "emc:totems<": condParam = param.emc_totems_less; break;
                case "omc:pirates=": condParam = param.omc_pirates_equal; break;
                case "omc:pirates!=": condParam = param.omc_pirates_notequal; break;
                case "omc:pirates>": condParam = param.omc_pirates_greater; break;
                case "omc:pirates<": condParam = param.omc_pirates_less; break;
                case "emc:pirates=": condParam = param.emc_pirates_equal; break;
                case "emc:pirates!=": condParam = param.emc_pirates_notequal; break;
                case "emc:pirates>": condParam = param.emc_pirates_greater; break;
                case "emc:pirates<": condParam = param.emc_pirates_less; break;
                case "omc:Dragons=": condParam = param.omc_Dragons_equal; break;
                case "omc:Dragons!=": condParam = param.omc_Dragons_notequal; break;
                case "omc:Dragons>": condParam = param.omc_Dragons_greater; break;
                case "omc:Dragons<": condParam = param.omc_Dragons_less; break;
                case "emc:Dragons=": condParam = param.emc_Dragons_equal; break;
                case "emc:Dragons!=": condParam = param.emc_Dragons_notequal; break;
                case "emc:Dragons>": condParam = param.emc_Dragons_greater; break;
                case "emc:Dragons<": condParam = param.emc_Dragons_less; break;
                case "omc:elems=": condParam = param.omc_elems_equal; break;
                case "omc:elems!=": condParam = param.omc_elems_notequal; break;
                case "omc:elems>": condParam = param.omc_elems_greater; break;
                case "omc:elems<": condParam = param.omc_elems_less; break;
                case "emc:elems=": condParam = param.emc_elems_equal; break;
                case "emc:elems!=": condParam = param.emc_elems_notequal; break;
                case "emc:elems>": condParam = param.emc_elems_greater; break;
                case "emc:elems<": condParam = param.emc_elems_less; break;
                case "omc:shr=": condParam = param.omc_shr_equal; break;
                case "omc:shr!=": condParam = param.omc_shr_notequal; break;
                case "omc:shr>": condParam = param.omc_shr_greater; break;
                case "omc:shr<": condParam = param.omc_shr_less; break;
                case "emc:shr=": condParam = param.emc_shr_equal; break;
                case "emc:shr!=": condParam = param.emc_shr_notequal; break;
                case "emc:shr>": condParam = param.emc_shr_greater; break;
                case "emc:shr<": condParam = param.emc_shr_less; break;
                case "omc:undamaged=": condParam = param.omc_undamaged_equal; break;
                case "omc:undamaged!=": condParam = param.omc_undamaged_notequal; break;
                case "omc:undamaged>": condParam = param.omc_undamaged_greater; break;
                case "omc:undamaged<": condParam = param.omc_undamaged_less; break;
                case "emc:undamaged=": condParam = param.emc_undamaged_equal; break;
                case "emc:undamaged!=": condParam = param.emc_undamaged_notequal; break;
                case "emc:undamaged>": condParam = param.emc_undamaged_greater; break;
                case "emc:undamaged<": condParam = param.emc_undamaged_less; break;
                case "omc:damaged=": condParam = param.omc_damaged_equal; break;
                case "omc:damaged!=": condParam = param.omc_damaged_notequal; break;
                case "omc:damaged>": condParam = param.omc_damaged_greater; break;
                case "omc:damaged<": condParam = param.omc_damaged_less; break;
                case "emc:damaged=": condParam = param.emc_damaged_equal; break;
                case "emc:damaged!=": condParam = param.emc_damaged_notequal; break;
                case "emc:damaged>": condParam = param.emc_damaged_greater; break;
                case "emc:damaged<": condParam = param.emc_damaged_less; break;
                case "omc:shields=": condParam = param.omc_shields_equal; break;
                case "omc:shields!=": condParam = param.omc_shields_notequal; break;
                case "omc:shields>": condParam = param.omc_shields_greater; break;
                case "omc:shields<": condParam = param.omc_shields_less; break;
                case "emc:shields=": condParam = param.emc_shields_equal; break;
                case "emc:shields!=": condParam = param.emc_shields_notequal; break;
                case "emc:shields>": condParam = param.emc_shields_greater; break;
                case "emc:shields<": condParam = param.emc_shields_less; break;
                case "omc:taunts=": condParam = param.omc_taunts_equal; break;
                case "omc:taunts!=": condParam = param.omc_taunts_notequal; break;
                case "omc:taunts>": condParam = param.omc_taunts_greater; break;
                case "omc:taunts<": condParam = param.omc_taunts_less; break;
                case "emc:taunts=": condParam = param.emc_taunts_equal; break;
                case "emc:taunts!=": condParam = param.emc_taunts_notequal; break;
                case "emc:taunts>": condParam = param.emc_taunts_greater; break;
                case "emc:taunts<": condParam = param.emc_taunts_less; break;
                case "ohc=": condParam = param.ohc_equal; break; 
                case "ohc!=": condParam = param.ohc_notequal; break;
                case "ohc>": condParam = param.ohc_greater; break;
                case "ohc<": condParam = param.ohc_less; break;
                case "ehc=": condParam = param.ehc_equal; break; 
                case "ehc!=": condParam = param.ehc_notequal; break;
                case "ehc>": condParam = param.ehc_greater; break;
                case "ehc<": condParam = param.ehc_less; break;
                //+extensions:
                case "ohc:minions=": condParam = param.ohc_minions_equal; break;
                case "ohc:minions!=": condParam = param.ohc_minions_notequal; break;
                case "ohc:minions>": condParam = param.ohc_minions_greater; break;
                case "ohc:minions<": condParam = param.ohc_minions_less; break;
                case "ohc:spells=": condParam = param.ohc_spells_equal; break;
                case "ohc:spells!=": condParam = param.ohc_spells_notequal; break;
                case "ohc:spells>": condParam = param.ohc_spells_greater; break;
                case "ohc:spells<": condParam = param.ohc_spells_less; break;
                case "ohc:secrets=": condParam = param.ohc_secrets_equal; break;
                case "ohc:secrets!=": condParam = param.ohc_secrets_notequal; break;
                case "ohc:secrets>": condParam = param.ohc_secrets_greater; break;
                case "ohc:secrets<": condParam = param.ohc_secrets_less; break;
                case "ohc:weapons=": condParam = param.ohc_weapons_equal; break;
                case "ohc:weapons!=": condParam = param.ohc_weapons_notequal; break;
                case "ohc:weapons>": condParam = param.ohc_weapons_greater; break;
                case "ohc:weapons<": condParam = param.ohc_weapons_less; break;
                case "ohc:murlocs=": condParam = param.ohc_murlocs_equal; break;
                case "ohc:murlocs!=": condParam = param.ohc_murlocs_notequal; break;
                case "ohc:murlocs>": condParam = param.ohc_murlocs_greater; break;
                case "ohc:murlocs<": condParam = param.ohc_murlocs_less; break;
                case "ohc:demons=": condParam = param.ohc_demons_equal; break;
                case "ohc:demons!=": condParam = param.ohc_demons_notequal; break;
                case "ohc:demons>": condParam = param.ohc_demons_greater; break;
                case "ohc:demons<": condParam = param.ohc_demons_less; break;
                case "ohc:mechs=": condParam = param.ohc_mechs_equal; break;
                case "ohc:mechs!=": condParam = param.ohc_mechs_notequal; break;
                case "ohc:mechs>": condParam = param.ohc_mechs_greater; break;
                case "ohc:mechs<": condParam = param.ohc_mechs_less; break;
                case "ohc:beasts=": condParam = param.ohc_beasts_equal; break;
                case "ohc:beasts!=": condParam = param.ohc_beasts_notequal; break;
                case "ohc:beasts>": condParam = param.ohc_beasts_greater; break;
                case "ohc:beasts<": condParam = param.ohc_beasts_less; break;
                case "ohc:totems=": condParam = param.ohc_totems_equal; break;
                case "ohc:totems!=": condParam = param.ohc_totems_notequal; break;
                case "ohc:totems>": condParam = param.ohc_totems_greater; break;
                case "ohc:totems<": condParam = param.ohc_totems_less; break;
                case "ohc:pirates=": condParam = param.ohc_pirates_equal; break;
                case "ohc:pirates!=": condParam = param.ohc_pirates_notequal; break;
                case "ohc:pirates>": condParam = param.ohc_pirates_greater; break;
                case "ohc:pirates<": condParam = param.ohc_pirates_less; break;
                case "ohc:Dragons=": condParam = param.ohc_Dragons_equal; break;
                case "ohc:Dragons!=": condParam = param.ohc_Dragons_notequal; break;
                case "ohc:Dragons>": condParam = param.ohc_Dragons_greater; break;
                case "ohc:Dragons<": condParam = param.ohc_Dragons_less; break;
                case "ohc:elems=": condParam = param.ohc_elems_equal; break;
                case "ohc:elems!=": condParam = param.ohc_elems_notequal; break;
                case "ohc:elems>": condParam = param.ohc_elems_greater; break;
                case "ohc:elems<": condParam = param.ohc_elems_less; break;
                case "ohc:shields=": condParam = param.ohc_shields_equal; break;
                case "ohc:shields!=": condParam = param.ohc_shields_notequal; break;
                case "ohc:shields>": condParam = param.ohc_shields_greater; break;
                case "ohc:shields<": condParam = param.ohc_shields_less; break;
                case "ohc:taunts=": condParam = param.ohc_taunts_equal; break;
                case "ohc:taunts!=": condParam = param.ohc_taunts_notequal; break;
                case "ohc:taunts>": condParam = param.ohc_taunts_greater; break;
                case "ohc:taunts<": condParam = param.ohc_taunts_less; break;
                case "t=": condParam = param.turn_equal; break; 
                case "t!=": condParam = param.turn_notequal; break;
                case "t>": condParam = param.turn_greater; break;
                case "t<": condParam = param.turn_less; break;
                case "overload=": condParam = param.overload_equal; break; 
                case "overload!=": condParam = param.overload_notequal; break;
                case "overload>": condParam = param.overload_greater; break;
                case "overload<": condParam = param.overload_less; break;
                case "owncarddraw=": condParam = param.owncarddraw_equal; break;
                case "owncarddraw!=": condParam = param.owncarddraw_notequal; break;
                case "owncarddraw>": condParam = param.owncarddraw_greater; break;
                case "owncarddraw<": condParam = param.owncarddraw_less; break;
                case "ohhp=": condParam = param.ohhp_equal; break;
                case "ohhp!=": condParam = param.ohhp_notequal; break;
                case "ohhp>": condParam = param.ohhp_greater; break;
                case "ohhp<": condParam = param.ohhp_less; break;
                case "ehhp=": condParam = param.ehhp_equal; break; 
                case "ehhp!=": condParam = param.ehhp_notequal; break;
                case "ehhp>": condParam = param.ehhp_greater; break;
                case "ehhp<": condParam = param.ehhp_less; break;

                case "ob=": condParam = param.ownboard_contain; pvaltype = 1; break; 
                case "ob!=": condParam = param.ownboard_notcontain; pvaltype = 1; break;
                case "eb=": condParam = param.enboard_contain; pvaltype = 1; break; 
                case "eb!=": condParam = param.enboard_notcontain; pvaltype = 1; break;
                case "oh=": condParam = param.ownhand_contain; pvaltype = 1; break; 
                case "oh!=": condParam = param.ownhand_notcontain; pvaltype = 1; break;
                case "ow=": condParam = param.ownweapon_equal; pvaltype = 1; break; 
                case "ow!=": condParam = param.ownweapon_notequal; pvaltype = 1; break;
                case "ew=": condParam = param.enweapon_equal; pvaltype = 1; break; 
                case "ew!=": condParam = param.enweapon_notequal; pvaltype = 1; break;
                case "ohero=": condParam = param.ownhero_equal; pvaltype = 2; break; 
                case "ohero!=": condParam = param.ownhero_notequal; pvaltype = 2; break;
                case "ehero=": condParam = param.enhero_equal; pvaltype = 2; break; 
                case "ehero!=": condParam = param.enhero_notequal; pvaltype = 2; break;

                case "p=": condParam = param.play; pvaltype = 1; 
                    break;
                case "p2=": condParam = param.play2; pvaltype = 1; 
                    break;
                case "a=": condParam = param.attacker; pvaltype = 1; 
                    break;
                default:
                    condErr = "Wrong parameter: ";
                    return false;
            }

            bool returnRes = false;
            switch (pvaltype)
            {
                case 0:
                    try
                    {
                        condTmp = new Condition(condParam, Convert.ToInt32(pval), (this.printRules == 0) ? "" : ruleString);
                        returnRes = true;
                    }
                    catch
                    {
                        condErr = "Wrong parameter value (must be a number): ";
                        returnRes = false;
                    }
                    break;
                case 1:
                    CardDB.cardIDEnum cardId = CardDB.Instance.cardIdstringToEnum(pval);
                    if (cardId == CardDB.cardIDEnum.None)
                    {
                        condErr = "Wrong CardID: ";
                        returnRes = false;
                    }
                    else
                    {
                        condTmp = new Condition(condParam, cardId, (this.printRules == 0) ? "" : ruleString);
                        returnRes = true;
                    }
                    break;
                case 2:
                    TAG_CLASS hClass = prozis.heroEnumtoTagClass(prozis.heroNametoEnum(pval.ToLower()));
                    if (hClass == TAG_CLASS.INVALID)
                    {
                        condErr = "Wrong Hero Class: ";
                        returnRes = false;
                    }
                    else
                    {
                        condTmp = new Condition(condParam, hClass, (this.printRules == 0) ? "" : ruleString);
                        returnRes = true;
                    }
                    break;
            }
            if (extraParam.Count() > 1 && returnRes)
            {
                List<Condition> extraConds = new List<Condition>();
                int extraParamCount = extraParam.Count();
                for (int i = 1; i < extraParamCount; i++)
                {
                    getSinglecond(extraParam[i], out tmp, out parameter);
                    
                    int pvalInt = 0;
                    CardDB.cardIDEnum pvalCardId = CardDB.cardIDEnum.None;
                    try
                    {
                        switch (tmp[0])
                        {
                            case "tgt":
                                pvalCardId = CardDB.Instance.cardIdstringToEnum(tmp[1]);
                                if (pvalCardId == CardDB.cardIDEnum.None)
                                {
                                    condErr = "Wrong CardID: ";
                                    returnRes = false;
                                }
                                break;
                            default:
                                pvalInt = Convert.ToInt32(tmp[1]);
                                break;
                        }
                    }
                    catch
                    {
                        condErr = "Wrong extra parameter: ";
                        return false;
                    }

                    switch (tmp[0] + parameter)
                    {
                        case "pen=": condTmp.bonus = pvalInt; continue;
                        case "aAt=": condParam = param.aAt_equal; break; 
                        case "aAt!=": condParam = param.aAt_notequal; break;
                        case "aAt>": condParam = param.aAt_greater; break;
                        case "aAt<": condParam = param.aAt_less; break;
                        case "aHp=": condParam = param.aHp_equal; break; 
                        case "aHp!=": condParam = param.aHp_notequal; break;
                        case "aHp>": condParam = param.aHp_greater; break;
                        case "aHp<": condParam = param.aHp_less; break;
                        case "tAt=": condParam = param.tAt_equal; break; 
                        case "tAt!=": condParam = param.tAt_notequal; break;
                        case "tAt>": condParam = param.tAt_greater; break;
                        case "tAt<": condParam = param.tAt_less; break;
                        case "tHp=": condParam = param.tHp_equal; break; 
                        case "tHp!=": condParam = param.tHp_notequal; break;
                        case "tHp>": condParam = param.tHp_greater; break;
                        case "tHp<": condParam = param.tHp_less; break;
                        case "tgt=": condParam = param.tgt_equal; break;
                        case "tgt!=": condParam = param.tgt_notequal; break;
                        default:
                            condErr = "Wrong extra parameter: ";
                            return false;
                    }
                    if (tmp[0] == "tgt") extraConds.Add(new Condition(condParam, pvalCardId, (this.printRules == 0) ? "" : ruleString));
                    else extraConds.Add(new Condition(condParam, pvalInt, (this.printRules == 0) ? "" : ruleString));
                }
                condTmp.extraConditions.AddRange(extraConds);
            }
            return returnRes;
        }

        private void getSinglecond(string singlecond, out String[] tmp, out string parameter)
        {
            parameter = "";
            if (singlecond.Contains("!="))
            {
                tmp = singlecond.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries);
                parameter = "!=";
            }
            else if (singlecond.Contains("="))
            {
                tmp = singlecond.Split('=');
                parameter = "=";
            }
            else if (singlecond.Contains(">"))
            {
                tmp = singlecond.Split('>');
                parameter = ">";
            }
            else if (singlecond.Contains("<"))
            {
                tmp = singlecond.Split('<');
                parameter = "<";
            }
            else tmp = singlecond.Split('=');
        }

        private bool checkCondition(Condition cond, Playfield p, Action a = null)
        {
            condErr = "";
            switch (cond.parameter)
            {
                case param.tm_equal: 
                    if (p.ownMaxMana == cond.num) return true;
                    return false;
                case param.tm_notequal:
                    if (p.ownMaxMana != cond.num) return true;
                    return false;
                case param.tm_greater:
                    if (p.ownMaxMana > cond.num) return true;
                    return false;
                case param.tm_less:
                    if (p.ownMaxMana < cond.num) return true;
                    return false;
                case param.am_equal: 
                    if (p.mana == cond.num) return true;
                    return false;
                case param.am_notequal:
                    if (p.mana != cond.num) return true;
                    return false;
                case param.am_greater:
                    if (p.mana > cond.num) return true;
                    return false;
                case param.am_less:
                    if (p.mana < cond.num) return true;
                    return false;
                case param.owa_equal: 
                    if (p.ownWeapon.Angr== cond.num) return true;
                    return false;
                case param.owa_notequal:
                    if (p.ownWeapon.Angr!= cond.num) return true;
                    return false;
                case param.owa_greater:
                    if (p.ownWeapon.Angr> cond.num) return true;
                    return false;
                case param.owa_less:
                    if (p.ownWeapon.Angr< cond.num) return true;
                    return false;
                case param.ewa_equal: 
                    if (p.enemyWeapon.Angr == cond.num) return true;
                    return false;
                case param.ewa_notequal:
                    if (p.enemyWeapon.Angr != cond.num) return true;
                    return false;
                case param.ewa_greater:
                    if (p.enemyWeapon.Angr > cond.num) return true;
                    return false;
                case param.ewa_less:
                    if (p.enemyWeapon.Angr < cond.num) return true;
                    return false;
                case param.owd_equal: 
                    if (p.ownWeapon.Durability == cond.num) return true;
                    return false;
                case param.owd_notequal:
                    if (p.ownWeapon.Durability != cond.num) return true;
                    return false;
                case param.owd_greater:
                    if (p.ownWeapon.Durability > cond.num) return true;
                    return false;
                case param.owd_less:
                    if (p.ownWeapon.Durability < cond.num) return true;
                    return false;
                case param.ewd_equal: 
                    if (p.enemyWeapon.Durability == cond.num) return true;
                    return false;
                case param.ewd_notequal:
                    if (p.enemyWeapon.Durability != cond.num) return true;
                    return false;
                case param.ewd_greater:
                    if (p.enemyWeapon.Durability > cond.num) return true;
                    return false;
                case param.ewd_less:
                    if (p.enemyWeapon.Durability < cond.num) return true;
                    return false;
                case param.omc_equal: 
                    if (p.ownMinions.Count == cond.num) return true;
                    return false;
                case param.omc_notequal:
                    if (p.ownMinions.Count != cond.num) return true;
                    return false;
                case param.omc_greater:
                    if (p.ownMinions.Count > cond.num) return true;
                    return false;
                case param.omc_less:
                    if (p.ownMinions.Count < cond.num) return true;
                    return false;
                case param.emc_equal: 
                    if (p.enemyMinions.Count == cond.num) return true;
                    return false;
                case param.emc_notequal:
                    if (p.enemyMinions.Count != cond.num) return true;
                    return false;
                case param.emc_greater:
                    if (p.enemyMinions.Count > cond.num) return true;
                    return false;
                case param.emc_less:
                    if (p.enemyMinions.Count < cond.num) return true;
                    return false;
                case param.omc_equal_emc: 
                    if (p.ownMinions.Count == p.enemyMinions.Count) return true;
                    return false;
                case param.omc_notequal_emc:
                    if (p.ownMinions.Count != p.enemyMinions.Count) return true;
                    return false;
                case param.omc_greater_emc:
                    if (p.ownMinions.Count > p.enemyMinions.Count) return true;
                    return false;
                case param.omc_less_emc:
                    if (p.ownMinions.Count < p.enemyMinions.Count) return true;
                    return false;
                case param.ohc_equal: 
                    if (p.owncards.Count == cond.num) return true;
                    return false;
                case param.ohc_notequal:
                    if (p.owncards.Count != cond.num) return true;
                    return false;
                case param.ohc_greater:
                    if (p.owncards.Count > cond.num) return true;
                    return false;
                case param.ohc_less:
                    if (p.owncards.Count < cond.num) return true;
                    return false;
                case param.ehc_equal: 
                    if (p.enemyAnzCards == cond.num) return true;
                    return false;
                case param.ehc_notequal:
                    if (p.enemyAnzCards != cond.num) return true;
                    return false;
                case param.ehc_greater:
                    if (p.enemyAnzCards > cond.num) return true;
                    return false;
                case param.ehc_less:
                    if (p.enemyAnzCards < cond.num) return true;
                    return false;
                case param.ohc_equal_ehc:
	                if (p.owncards.Count == p.enemyAnzCards) return true;
	                return false;
                case param.ohc_notequal_ehc:
	                if (p.owncards.Count != p.enemyAnzCards) return true;
	                return false;
                case param.ohc_greater_ehc:
	                if (p.owncards.Count > p.enemyAnzCards) return true;
	                return false;
                case param.ohc_less_ehc:
	                if (p.owncards.Count < p.enemyAnzCards) return true;
	                return false;
                case param.ohc_minions_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.MOB) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_minions_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.MOB) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_minions_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.MOB) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_minions_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.MOB) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_spells_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.SPELL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_spells_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.SPELL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_spells_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.SPELL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_spells_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.SPELL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_secrets_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Secret) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_secrets_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Secret) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_secrets_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Secret) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_secrets_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Secret) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_weapons_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.WEAPON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_weapons_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.WEAPON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_weapons_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.WEAPON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_weapons_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.type == CardDB.cardtype.WEAPON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_murlocs_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_murlocs_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_murlocs_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_murlocs_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_demons_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_demons_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_demons_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_demons_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_mechs_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_mechs_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_mechs_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_mechs_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_beasts_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_beasts_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_beasts_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_beasts_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_totems_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_totems_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_totems_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                    /*tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;*/
                case param.ohc_pirates_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_pirates_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_pirates_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_pirates_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_Dragons_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_Dragons_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_Dragons_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_Dragons_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_elems_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_elems_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_elems_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_elems_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if ((TAG_RACE)hc.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_shields_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Shield) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_shields_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Shield) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_shields_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Shield) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_shields_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.Shield) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.ohc_taunts_equal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.tank) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.ohc_taunts_notequal:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.tank) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.ohc_taunts_greater:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.tank) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.ohc_taunts_less:
                    tmp_counter = 0;
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.tank) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.aAt_equal: 
                    if (a.own != null && a.own.Angr == cond.num) return true;
                    return false;
                case param.aAt_notequal:
                    if (a.own != null && a.own.Angr != cond.num) return true;
                    return false;
                case param.aAt_greater:
                    if (a.own != null && a.own.Angr > cond.num) return true;
                    return false;
                case param.aAt_less:
                    if (a.own != null && a.own.Angr < cond.num) return true;
                    return false;
                case param.aHp_equal: 
                    if (a.own != null && a.prevHpOwn == cond.num) return true;
                    return false;
                case param.aHp_notequal:
                    if (a.own != null && a.prevHpOwn != cond.num) return true;
                    return false;
                case param.aHp_greater:
                    if (a.own != null && a.prevHpOwn > cond.num) return true;
                    return false;
                case param.aHp_less:
                    if (a.own != null && a.prevHpOwn < cond.num) return true;
                    return false;
                case param.tAt_equal: 
                    if (a.target != null && a.target.Angr == cond.num) return true;
                    return false;
                case param.tAt_notequal:
                    if (a.target != null && a.target.Angr != cond.num) return true;
                    return false;
                case param.tAt_greater:
                    if (a.target != null && a.target.Angr > cond.num) return true;
                    return false;
                case param.tAt_less:
                    if (a.target != null && a.target.Angr < cond.num) return true;
                    return false;
                case param.tHp_equal: 
                    if (a.target != null && a.prevHpTarget == cond.num) return true;
                    return false;
                case param.tHp_notequal:
                    if (a.target != null && a.prevHpTarget != cond.num) return true;
                    return false;
                case param.tHp_greater:
                    if (a.target != null && a.prevHpTarget > cond.num) return true;
                    return false;
                case param.tHp_less:
                    if (a.target != null && a.prevHpTarget < cond.num) return true;
                    return false;
                case param.omc_murlocs_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_murlocs_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_murlocs_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_murlocs_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_murlocs_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_murlocs_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_murlocs_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_murlocs_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_demons_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_demons_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_demons_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_demons_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_demons_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_demons_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_demons_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_demons_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_mechs_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_mechs_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_mechs_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_mechs_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_mechs_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_mechs_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_mechs_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_mechs_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_beasts_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_beasts_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_beasts_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_beasts_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_beasts_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_beasts_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_beasts_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_beasts_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PET) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_totems_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_totems_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_totems_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_totems_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_totems_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_totems_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_totems_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_totems_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_pirates_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_pirates_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_pirates_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_pirates_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_pirates_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_pirates_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_pirates_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_pirates_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.PIRATE) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_Dragons_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_Dragons_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_Dragons_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_Dragons_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_Dragons_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_Dragons_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_Dragons_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_Dragons_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_elems_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_elems_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_elems_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_elems_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_elems_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_elems_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_elems_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_elems_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if ((TAG_RACE)m.handcard.card.race == TAG_RACE.ELEMENTAL) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_shr_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_shr_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_shr_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_shr_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_shr_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_shr_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_shr_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_shr_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.name == CardDB.cardNameEN.silverhandrecruit) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_undamaged_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_undamaged_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_undamaged_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_undamaged_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_undamaged_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_undamaged_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_undamaged_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_undamaged_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (!m.wounded) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_damaged_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_damaged_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_damaged_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_damaged_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_damaged_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_damaged_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_damaged_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_damaged_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.wounded) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_shields_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_shields_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_shields_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_shields_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_shields_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_shields_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_shields_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_shields_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.divineshild) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.omc_taunts_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.omc_taunts_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.omc_taunts_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.omc_taunts_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.ownMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;
                case param.emc_taunts_equal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter == cond.num) return true;
                    return false;
                case param.emc_taunts_notequal:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter != cond.num) return true;
                    return false;
                case param.emc_taunts_greater:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter > cond.num) return true;
                    return false;
                case param.emc_taunts_less:
                    tmp_counter = 0;
                    foreach (Minion m in p.enemyMinions) if (m.taunt) tmp_counter++;
                    if (tmp_counter < cond.num) return true;
                    return false;

                case param.turn_equal: 
                    if (p.gTurn == cond.num) return true;
                    return false;
                case param.turn_notequal:
                    if (p.gTurn != cond.num) return true;
                    return false;
                case param.turn_greater:
                    if (p.gTurn > cond.num) return true;
                    return false;
                case param.turn_less:
                    if (p.gTurn < cond.num) return true;
                    return false;
                case param.overload_equal: 
                    if (p.ueberladung == cond.num) return true;
                    return false;
                case param.overload_notequal:
                    if (p.ueberladung != cond.num) return true;
                    return false;
                case param.overload_greater:
                    if (p.ueberladung > cond.num) return true;
                    return false;
                case param.overload_less:
                    if (p.ueberladung < cond.num) return true;
                    return false;
                case param.owncarddraw_equal: 
                    if (p.owncarddraw == cond.num) return true;
                    return false;
                case param.owncarddraw_notequal:
                    if (p.owncarddraw != cond.num) return true;
                    return false;
                case param.owncarddraw_greater:
                    if (p.owncarddraw > cond.num) return true;
                    return false;
                case param.owncarddraw_less:
                    if (p.owncarddraw < cond.num) return true;
                    return false;
                case param.ohhp_equal: 
                    if (p.ownHero.Hp == cond.num) return true;
                    return false;
                case param.ohhp_notequal:
                    if (p.ownHero.Hp != cond.num) return true;
                    return false;
                case param.ohhp_greater:
                    if (p.ownHero.Hp > cond.num) return true;
                    return false;
                case param.ohhp_less:
                    if (p.ownHero.Hp < cond.num) return true;
                    return false;
                case param.ehhp_equal: 
                    if (p.enemyHero.Hp == cond.num) return true;
                    return false;
                case param.ehhp_notequal:
                    if (p.enemyHero.Hp != cond.num) return true;
                    return false;
                case param.ehhp_greater:
                    if (p.enemyHero.Hp > cond.num) return true;
                    return false;
                case param.ehhp_less:
                    if (p.enemyHero.Hp < cond.num) return true;
                    return false;

                case param.ownboard_contain: 
                    foreach (Minion m in p.ownMinions) if (m.handcard.card.cardIDenum == cond.cardID) return true;
                    return false;
                case param.ownboard_notcontain:
                    foreach (Minion m in p.ownMinions) if (m.handcard.card.cardIDenum == cond.cardID) return false;
                    return true;
                case param.enboard_contain: 
                    foreach (Minion m in p.enemyMinions) if (m.handcard.card.cardIDenum == cond.cardID) return true;
                    return false;
                case param.enboard_notcontain:
                    foreach (Minion m in p.enemyMinions) if (m.handcard.card.cardIDenum == cond.cardID) return false;
                    return true;
                case param.ownhand_contain: 
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.cardIDenum == cond.cardID) return true;
                    return false;
                case param.ownhand_notcontain:
                    foreach (Handmanager.Handcard hc in p.owncards) if (hc.card.cardIDenum == cond.cardID) return false;
                    return true;
                case param.ownweapon_equal: 
                    if (p.ownWeapon.card.cardIDenum == cond.cardID) return true;
                    return false;
                case param.ownweapon_notequal:
                    if (p.ownWeapon.card.cardIDenum != cond.cardID) return true;
                    return false;
                case param.enweapon_equal: 
                    if (p.enemyWeapon.card.cardIDenum == cond.cardID) return true;
                    return false;
                case param.enweapon_notequal:
                    if (p.enemyWeapon.card.cardIDenum != cond.cardID) return true;
                    return false;
                case param.ownhero_equal: 
                    if (cond.hClass == TAG_CLASS.WHIZBANG) return true;
                    if (p.ownHeroStartClass == cond.hClass) return true;
                    return false;
                case param.ownhero_notequal:
                    if (p.ownHeroStartClass != cond.hClass) return true;
                    return false;
                case param.enhero_equal: 
                    if (cond.hClass == TAG_CLASS.WHIZBANG) return true;
                    if (p.enemyHeroStartClass == cond.hClass) return true;
                    return false;
                case param.enhero_notequal:
                    if (p.enemyHeroStartClass != cond.hClass) return true;
                    return false;
                case param.tgt_equal:
                    if (a.target!= null && (a.target.handcard.card.cardIDenum == cond.cardID || (a.target.isHero && cond.cardID == CardDB.cardIDEnum.hero))) return true;
                    return false;
                case param.tgt_notequal:
                    if (a.target != null)
                    {
                        if (a.target.isHero)
                        {
                            if (cond.cardID != CardDB.cardIDEnum.hero) return true;
                            else return false;
                        }
                        else if (a.target.handcard.card.cardIDenum != cond.cardID) return true;
                    }
                    return false;

                
                case param.noduplicates:
                    return p.prozis.noDuplicates;
                default:
                    condErr = "Wrong parameter: ";
                    return false;
            }
        }

    }

}