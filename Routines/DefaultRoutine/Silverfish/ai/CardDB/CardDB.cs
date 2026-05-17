using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using Logger = Triton.Common.LogUtilities.Logger;
using log4net;

namespace HREngine.Bots
{
    public partial class CardDB
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
       
        // 存储 Sim_* 类的类型字典
        private static readonly Dictionary<string, Type> SimTypesDict;

        // 静态构造函数：扫描所有程序集填充卡牌类型字典
        static CardDB()
        {
            // 加载 SilverfishCards.dll（卡牌模拟类独立程序集）
            try
            {
                // 先尝试从执行目录加载，再尝试从 CompiledAssemblies 加载
                string exeDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string cardsPath = System.IO.Path.Combine(exeDir, "SilverfishCards.dll");
                if (!System.IO.File.Exists(cardsPath))
                    cardsPath = System.IO.Path.Combine(exeDir, "..", "CompiledAssemblies", "SilverfishCards.dll");
                if (!System.IO.File.Exists(cardsPath))
                    cardsPath = System.IO.Path.Combine(Environment.CurrentDirectory, "CompiledAssemblies", "SilverfishCards.dll");
                if (System.IO.File.Exists(cardsPath))
                    Assembly.LoadFrom(cardsPath);
            }
            catch { /* SilverfishCards.dll 非必需 */ }

            Type baseType = typeof(SimTemplate);
            var dict = new Dictionary<string, Type>();
            // 扫描当前 AppDomain 中所有程序集，查找 SimTemplate 子类
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var t in asm.GetTypes())
                    {
                        if (t.Namespace == "HREngine.Bots" && t.BaseType == baseType)
                            dict[t.Name] = t;
                    }
                }
                catch { /* 跳过无法反射的程序集 */ }
            }
            SimTypesDict = dict;
        }
        public class Card
        {
            public cardIDEnum cardIDenum = cardIDEnum.None;//cardId
            public string dbfId = "";
            public cardNameEN nameEN = cardNameEN.unknown;//cardNameEN英文名称
            public cardNameCN nameCN = cardNameCN.未知;//cardNameCN中文名称

            public Race race = Race.INVALID;//种族
            public int rarity = 0;//稀有度
            public int cost = 0;//费用
            public int Attack = 0; //攻击力
            public int Health = 0;//血量
            public int Class = 0;//职业
            public cardtype type = CardDB.cardtype.NONE;//类别
            public int Durability = 0;//for weapons//耐久值
            public bool tank = false;//嘲讽
            public bool Shield = false;//圣盾
            public bool Charge = false;//冲锋
            public bool Rush = false;//突袭
            public bool Stealth = false;//潜行
            public bool Elusive = false;//扰魔
            public bool windfury = false;//风怒
            public bool megaWindfury = false;//超级风怒
            public bool poisonous = false;//剧毒
            public bool lifesteal = false;//吸血
            public int dormant = 0;//休眠 0表示非休眠生物或者已醒，还有多少回合醒来
            public bool reborn = false;//复生
            /// <summary> 荣誉击杀 </summary>
            public bool HonorableKill { get; set; }
            /// <summary> 超杀 </summary>
            public bool Overkill { get; set; }
            /// <summary>法术迸发 </summary>
            public bool Spellburst { get; set; }

            /// <summary> 暴怒 </summary>
            public bool Frenzy { get; set; }
            public bool battlecry = false;//战吼
            public bool choice = false;//抉择
            public bool deathrattle = false;//亡语
            public bool Silence = false;//沉默

            public bool discover = false;//发现
            public bool oneTurnEffect = false;
            public bool Enrage = false;//愤怒 激怒
            public bool Aura = false;//光环
            public bool Elite = false;//精华??
            public bool Combo = false;//连击
            public int overload = 0;//超载
            public bool immuneWhileAttacking = false;//攻击时免疫
            public bool untouchable = false;//不可被攻击
            public bool Freeze = false;//冰冻
            public bool AdjacentBuff = false;//相邻buff 恐狼?

            public bool Secret = false;//奥秘
            public bool Nature = false;//自然
            public bool Quest = false;//任务
            public bool Questline = false;//任务线
            public bool Morph = false;//变形
            public bool Spellpower = false;//法强
            public bool Inspire = false;//激励
            public bool Outcast = false;//流放
            public bool Corrupted = false;//已腐蚀
            public bool Corrupt = false;//可腐蚀
            public bool CantAttack = false; //不可攻击
            public bool Collectable = false; //可收藏
            public SpellSchool SpellSchool = SpellSchool.NONE;//法术派系
            public bool Tradeable = false;//可交易
            public int TradeCost = 0;//交易消耗法力值

            public bool isToken = false;
            public int isCarddraw = 0;
            public bool damagesTarget = false;
            public bool damagesTargetWithSpecial = false;
            public int targetPriority = 0;
            /// <summary>
            /// 是特殊随从
            /// </summary>
            public bool isSpecialMinion = false;
            public int spellpowervalue = 0;
            public List<cardtrigers> trigers;
            private SimTemplate _simCard;
            public SimTemplate sim_card
            {
                get
                {
                    if (_simCard == null)
                    {
                        _simCard = CardDB.GetCardSimulation(this.cardIDenum);
                    }
                    return _simCard;
                }
                set { _simCard = value; }
            }
            /// <summary>
            /// 将其视为同一张卡。储存defid,根据卡defid获取同一张卡的sim
            /// </summary>
            public string TreatItAsTheSameCard = "";
            public int TAG_SCRIPT_DATA_NUM_1 = 0;//标签脚本数据编号1，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
            public int TAG_SCRIPT_DATA_NUM_2 = 0;//标签脚本数据编号2，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
            public int TAG_SCRIPT_DATA_NUM_3 = 0;//标签脚本数据编号3，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
            public int TAG_SCRIPT_DATA_NUM_4 = 0;//标签脚本数据编号4，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求

            public int DECK_ACTION_COST = 0;//卡组操作消耗法力值

            public bool Dredge = false;//探底
            public int CooldownTurn = 0;//地标冷却回合
            public bool Infuse = false;//注能
            public bool Infused = false;//已注能
            public int InfuseNum = 0;//注能数量
            public int Manathirst = 0;//法力渴求
            public bool Finale = false;//压轴
            public bool Overheal = false;//过量治疗
            public bool Titan = false;//泰坦
            public bool TitanAbilityUsed1 = false;//泰坦第一技能
            public bool TitanAbilityUsed2 = false;//泰坦第二技能
            public bool TitanAbilityUsed3 = false;//泰坦第三技能
            public List<Card> TitanAbility = new List<Card>();//泰坦技能列表
            public bool Forge = false;//锻造
            public int ForgeCost = 0;//锻造消耗法力值
            public bool Forged = false;//已锻造
            public bool Quickdraw = false;//快枪
            public bool Excavate = false;//发掘
            public bool Echo = false; // 回响
            public bool nonKeywordEcho = false; // 非关键词回响，设计师左右脑互搏的结果。就是个在本回合可以重复使用
            public bool Twinspell = false; // 双生法术
            public bool Temporary = false; // 临时
            public int armor = 0; //英雄牌的护甲值
            public int heroPower = 0; //英雄牌的技能dbfid
            public int upgradedHeroPower = 0;//升级的英雄技能defid
            public int KeepHeroClass = 0;//打出英雄保持原职业
            public int Miniaturize = 0;
            public int Gigantity = 0;
            public int CollectionRelatedCardDataBaseId = 0;
            public int CardAlternateCost = 0;
            public int Objective = 0; // 光环 如救生光环
            public int ObjectiveAura = 0; // 会影响场面的光环 如征战平原
            public int Sigil = 0; // 咒符

            public int costBlood = 0;   // 鲜血符文
            public int costFrost = 0;    // 冰霜符文
            public int costUnholy = 0; // 邪恶符文
            public int CastsWhenDrawn = 0; // 抽到时触发效果

            public bool HideCost = false;//隐藏费用
            public bool ShiftingSpell = false;//
            public int InteractableObjectCost = 0;
            public bool CanTargetCardsInHand = false;
            public bool InteractableObject = false;
            public int UsesCharges = 0;
            public int TriggerVisual = 0;//有触发效果
            public int MODULAR_ENTITY_PART_1 = 0;//自定义模块1
            public int MODULAR_ENTITY_PART_2 = 0;//自定义模块2
            /// <summary>磁力允许的其他随从类型</summary>
            public int magneticToRace = 0;
            #region 卡牌分类 
            public bool SilverHandRecruit = false; //白银之手新兵
            public bool SI_7 = false;//军情七处
            /// <summary>跟班</summary>
            public bool markOfEvil = false;
            /// <summary>树人</summary>
            public bool Treant = false;
            /// <summary>古树</summary>
            public bool Ancient = false;
            /// <summary>小鬼</summary>
            public bool IMP = false;
            /// <summary>雏龙</summary>
            public bool Whelp = false;
            /// <summary>星舰组件</summary>
            public bool StarshipPiece = false;
            /// <summary>星舰</summary>
            public bool Starship = false;
            /// <summary>乘务员</summary>
            public bool Crewmate = false;
            public int MultipleClasses = 0; // 194: 玉莲帮 296: 暗金教 532: 玉莲帮
            public bool Zerg = false;   // 异虫
            public bool Terran = false; // 人族
            public bool Protoss = false;    // 星灵
            public bool Wipe = false; //小精灵
            public bool Rafaam = false; //拉法姆
            public bool Ysera = false; //伊瑟拉

            #endregion
            #region 要求目标满足的条件
            public int needEmptyPlacesForPlaying = 0;
            public int needWithMinAttackValueOf = 0;
            public int needWithMaxAttackValueOf = 0;
            public int needWithExactAttackValueOf = 0;
            public int needWithMinimumCorpeses = 0;
            public int needRaceForPlaying = 0;
            public int needRaceInHand = 0;
            public CardDB.Specialtags needTagForPlaying = 0;

            public int needMinNumberOfEnemy = 0;
            public int needMinTotalMinions = 0;
            public int needMinOwnMinions = 0;
            public int needMinionsCapIfAvailable = 0;
            public int needControlaSecret = 0;
            #endregion
            public void getModularCard(out CardDB.Card part1, out CardDB.Card part2)
            {

                part1 = CardDB.Instance.getCardDataFromDbfID(MODULAR_ENTITY_PART_1.ToString());
                part2 = CardDB.Instance.getCardDataFromDbfID(MODULAR_ENTITY_PART_2.ToString());
            }
            public void updateDIYCard()
            {
                if (MODULAR_ENTITY_PART_1 != 0 && MODULAR_ENTITY_PART_2 != 0)
                {
                    CardDB.Card part1 = Instance.getCardDataFromDbfID(MODULAR_ENTITY_PART_1.ToString());
                    CardDB.Card part2 = Instance.getCardDataFromDbfID(MODULAR_ENTITY_PART_2.ToString());

                    if (part1 != null && part2 != null)
                    {
                        cost = part1.cost + part2.cost;
                        HideCost = false;
                        if (this.type == CardDB.cardtype.MOB)
                        {

                            Attack = part1.Attack + part2.Attack;
                            Health = part1.Health + part2.Health;
                            textCN = part1.textCN + part2.textCN;
                            tank = (part1.tank || part2.tank);                                    //嘲讽
                            Shield = (part1.Shield || part2.Shield);                              //圣盾
                            Charge = (part1.Charge || part2.Charge);                              //冲锋
                            Rush = (part1.Rush || part2.Rush);                                    //突袭
                            Stealth = (part1.Stealth || part2.Stealth);                           //潜行
                            Elusive = (part1.Elusive || part2.Elusive);                           //扰魔
                            windfury = (part1.windfury || part2.windfury);                        //风怒
                            poisonous = (part1.poisonous || part2.poisonous);                     //剧毒
                            lifesteal = (part1.lifesteal || part2.lifesteal);                     //吸血
                            reborn = (part1.reborn || part2.reborn);                              //复生
                        }
                    }
                }
            }

            public List<Race> races = new List<Race>(); //TODO:种族集合

            //TODO:种族数
            public int GetRaceCount()
            {
                return this.GetRaces().Count;
            }
            /// <summary>
            /// 获取种族集合
            /// </summary>
            /// <returns>种族集合</returns>
            public List<Race> GetRaces()
            {
                if (this.races == null)
                {
                    this.races = new List<Race>();
                }
                if (this.races.Contains(Race.ALL))
                {
                    return this.races;
                }
                this.races = Enumerable.ToList<Race>(Enumerable.Distinct<Race>(this.races));
                if (this.races.Count > 1)
                {
                    Race[] order = new Race[]
                    {
                        Race.UNDEAD,
                        Race.ELEMENTAL,
                        Race.MECHANICAL,
                        Race.DEMON,
                        Race.MURLOC,
                        Race.QUILBOAR,
                        Race.NAGA,
                        Race.PET,
                        Race.DRAGON,
                        Race.DRAENEI,
                        Race.TOTEM,
                        Race.PIRATE
                    };
                    this.races.Sort((Race r1, Race r2) => Array.IndexOf<Race>(order, r1).CompareTo(Array.IndexOf<Race>(order, r2)));
                }

                return this.races;
            }

            public string textCN = "";
            public int count = 1;

            // private bool _honorableKill = false;
            // private bool _overkill = false;
            // private bool _spellburst = false;
            // private bool _frenzy = false;
            public string OnlineCardImage
            {
                get { return "https://art.hearthstonejson.com/v1/render/latest/zhCN/256x/" + cardIDenum.ToString() + ".png"; }
            }

            public string OnlineCardTile
            {
                get { return "https://art.hearthstonejson.com/v1/tiles/" + cardIDenum.ToString() + ".png"; }
            }

            public string CardInfo
            {
                get
                {
                    return "[" + cost + "] \t\t" + Attack.ToString() + "/" + Health.ToString() + "\n" + nameCN.ToString() + "\n" + textCN + "\n" + type.ToString() + " \t\t" + race.ToString();
                }
            }

            public string Status
            {
                get
                {
                    return "(" + cost + ") " + nameCN.ToString() + " [" + count + "] ";
                }
            }

            public string Color
            {
                get
                {
                    switch (rarity)
                    {
                        case 3:
                            return "DodgerBlue";
                        case 4:
                            return "BlueViolet";
                        case 5:
                            return "DarkOrange";
                        case 1:
                        default:
                            return "Gray";
                    }
                }
            }

            /// <summary>
            /// 存在错误类型
            /// </summary>
            /// <param name="errorType"></param>
            /// <returns></returns>
            public bool ExistErrorType(CardDB.ErrorType2 errorType)
            {
                foreach (var pr in this.sim_card.GetPlayReqs())
                {
                    if (pr.errorType == errorType) return true;
                }
                return false;
            }
            /// <summary>
            /// 获取目标
            /// </summary>
            /// <param name="p">场面</param>
            /// <param name="isLethalCheck"></param>
            /// <param name="own"></param>
            /// <param name="result"></param>
            /// <param name="playReqs"></param>
            /// <returns></returns>
            public List<Minion> getTargets(Playfield p, bool isLethalCheck, bool own, List<Minion> result, PlayReq[] playReqs)
            {
                if (playReqs.Length == 0)
                {
                    result.Add(null);
                    return result;

                }
                Minion ownHero = p.ownHero;
                Minion enemyHero = p.enemyHero;
                List<Minion> targets = new List<Minion>();
                bool targetAll = false;
                bool targetAllEnemy = false;
                bool targetAllFriendly = false;
                bool targetEnemyHero = false;
                bool targetOwnHero = false;
                bool targetOnlyMinion = false;
                bool extraParam = false;
                bool wereTargets = false;
                bool REQ_UNDAMAGED_TARGET = false;
                bool REQ_TARGET_WITH_DEATHRATTLE = false;
                bool REQ_TARGET_WITH_RACE = false;
                bool REQ_TARGET_MIN_ATTACK = false;
                bool REQ_TARGET_MAX_ATTACK = false;
                bool REQ_TARGET_EXACT_ATTACK = false;
                bool REQ_MUST_TARGET_TAUNTER = false;
                bool REQ_STEADY_SHOT = false;
                bool REQ_FROZEN_TARGET = false;
                bool REQ_HERO_TARGET = false;
                bool REQ_DAMAGED_TARGET = false;
                bool REQ_LEGENDARY_TARGET = false;
                bool REQ_TARGET_IF_AVAILABLE = false;
                bool REQ_STEALTHED_TARGET = false;
                bool REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN = false;
                bool REQ_TARGET_NO_NATURE = false;
                bool REQ_TARGET_IS_NON_TITAN = false;
                bool REQ_TARGET_SILVER_HAND_RECRUIT = false;
                bool REQ_LOCATION_TARGET = false;
                bool REQ_TARGET_MUST_HAVE_TAG = false;
                bool REQ_FRIENDLY_MINION_OF_RACE_IN_HAND = false;
                bool REQ_DAMAGED_TARGET_UNLESS_COMBO = false;
                bool REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN = false;
                foreach (PlayReq pr in playReqs)
                {
                    switch (pr.errorType)
                    {
                        case ErrorType2.REQ_TARGET_TO_PLAY:
                        case ErrorType2.REQ_TARGET_TO_PLAY2:
                            targetAll = true;
                            continue;
                        case ErrorType2.REQ_MINION_TARGET:
                            targetOnlyMinion = true;
                            continue;
                        case ErrorType2.REQ_LOCATION_TARGET:
                            REQ_LOCATION_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE:
                            REQ_TARGET_IF_AVAILABLE = true;
                            targetAll = true;
                            continue;
                        case ErrorType2.REQ_FRIENDLY_TARGET:
                            if (own) targetAllFriendly = true;
                            else targetAllEnemy = true;
                            continue;
                        case ErrorType2.REQ_NUM_MINION_SLOTS:
                            if ((own ? p.ownMinions.Count : p.enemyMinions.Count) > 7 - this.needEmptyPlacesForPlaying) return result;
                            continue;
                        case ErrorType2.REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT:
                            if (own) { if (p.ownMinions.Count > 6 & p.ownMaxMana > 9) return result; }
                            else if (p.enemyMinions.Count > 6 & p.enemyMaxMana > 9) return result;
                            continue;
                        case ErrorType2.REQ_ENEMY_TARGET:
                            if (own) targetAllEnemy = true;
                            else targetAllFriendly = true;
                            continue;
                        case ErrorType2.REQ_HERO_TARGET:
                            REQ_HERO_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_MINIMUM_ENEMY_MINIONS:
                            if ((own ? p.enemyMinions.Count : p.ownMinions.Count) < this.needMinNumberOfEnemy) return result;
                            continue;
                        case ErrorType2.REQ_NONSELF_TARGET:
                            targetAll = true;
                            continue;
                        case ErrorType2.REQ_TARGET_WITH_RACE:
                            REQ_TARGET_WITH_RACE = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_DAMAGED_TARGET:
                            REQ_DAMAGED_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_MAX_ATTACK:
                            REQ_TARGET_MAX_ATTACK = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_EXACT_ATTACK:
                            REQ_TARGET_EXACT_ATTACK = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_WEAPON_EQUIPPED:
                            if ((own ? p.ownWeapon.Durability : p.enemyWeapon.Durability) == 0) return result;
                            continue;
                        case ErrorType2.REQ_TARGET_FOR_COMBO:
                            if (p.cardsPlayedThisTurn >= 1) targetAll = true;
                            continue;
                        case ErrorType2.REQ_TARGET_MIN_ATTACK:
                            REQ_TARGET_MIN_ATTACK = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_MINIMUM_TOTAL_MINIONS:
                            if (this.needMinTotalMinions > p.ownMinions.Count + p.enemyMinions.Count) return result;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK:
                            if (ownHero.Angr >= 1) return result;
                            continue;
                        case ErrorType2.REQ_MINIMUM_CORPSES:
                            if (p.getCorpseCount() < this.needWithMinimumCorpeses) return result;
                            continue;
                        case ErrorType2.REQ_MINION_CAP_IF_TARGET_AVAILABLE:
                            if ((own ? p.ownMinions.Count : p.enemyMinions.Count) > 7 - this.needMinionsCapIfAvailable) return result;
                            continue;
                        case ErrorType2.REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY://不能召唤更多图腾
                            bool searingtotem = false;
                            bool wrathofairtotem = false;
                            bool stoneclawtotem = false;
                            bool healingtotem = false;
                            foreach (Minion m in (own ? p.ownMinions : p.enemyMinions))
                            {
                                if (m.name == CardDB.cardNameEN.healingtotem)
                                {//治疗图腾
                                    healingtotem = true;
                                    continue;
                                }
                                // 力量或者法强图腾
                                if (m.name == CardDB.cardNameEN.wrathofairtotem || m.name == CardDB.cardNameEN.strengthtotem)
                                {
                                    wrathofairtotem = true;
                                    continue;
                                }
                                if (m.name == CardDB.cardNameEN.searingtotem)
                                {
                                    searingtotem = true;
                                    continue;
                                }
                                if (m.name == CardDB.cardNameEN.stoneclawtotem)
                                {
                                    stoneclawtotem = true;
                                    continue;
                                }
                            }
                            if (healingtotem && wrathofairtotem && searingtotem && stoneclawtotem) return result;
                            continue;
                        case ErrorType2.REQ_MUST_TARGET_TAUNTER:
                            REQ_MUST_TARGET_TAUNTER = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND:
                            if (own)
                            {
                                targetAll = p.anyRaceCardInHand(CardDB.Race.DRAGON);
                            }
                            else targetAll = true; // apriori the enemy have a dragon
                            continue;
                        case ErrorType2.REQ_FRIENDLY_MINION_OF_RACE_IN_HAND:
                            {
                                REQ_FRIENDLY_MINION_OF_RACE_IN_HAND = true;
                                extraParam = true;
                                continue;
                            }
                        case ErrorType2.REQ_LEGENDARY_TARGET:
                            REQ_LEGENDARY_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_UNDAMAGED_TARGET:
                            REQ_UNDAMAGED_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_WITH_DEATHRATTLE:
                            REQ_TARGET_WITH_DEATHRATTLE = true;
                            targetOnlyMinion = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_NO_NATURE:
                            REQ_TARGET_NO_NATURE = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN:
                            REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_STEADY_SHOT:
                            REQ_STEADY_SHOT = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_FROZEN_TARGET:
                            REQ_FROZEN_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_MINION_OR_ENEMY_HERO:
                            REQ_STEADY_SHOT = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_STEALTHED_TARGET:
                            REQ_STEALTHED_TARGET = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_ENEMY_WEAPON_EQUIPPED:
                            if (own)
                            {
                                if (p.enemyWeapon.Durability > 0) targetEnemyHero = true;
                                else return result;
                            }
                            else
                            {
                                if (p.ownWeapon.Durability > 0) targetOwnHero = true;
                                else return result;
                            }
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS:
                            int tmp = (own) ? p.ownMinions.Count : p.enemyMinions.Count;
                            if (tmp >= needMinOwnMinions) targetAll = true;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS:
                            if (p.ownSecretsIDList.Count >= needControlaSecret) targetAll = true;
                            continue;
                        case ErrorType2.REQ_MUST_PLAY_OTHER_CARD_FIRST:
                            if (p.cardsPlayedThisTurn == 0) return result;
                            continue;
                        case ErrorType2.REQ_HAND_NOT_FULL:
                            if (p.owncards.Count == 10) return result;
                            continue;
                        case ErrorType2.REQ_TARGET_IS_NON_TITAN:
                            REQ_TARGET_IS_NON_TITAN = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_SILVER_HAND_RECRUIT:
                            REQ_TARGET_SILVER_HAND_RECRUIT = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_MUST_HAVE_TAG:
                            REQ_TARGET_MUST_HAVE_TAG = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_DAMAGED_TARGET_UNLESS_COMBO:
                            REQ_DAMAGED_TARGET_UNLESS_COMBO = true;
                            extraParam = true;
                            continue;
                        case ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN:
                            REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN = true;
                            extraParam = true;
                            continue;
                    }
                }

                if (targetAll)
                {

                    wereTargets = true;


                    if (targetAllFriendly != targetAllEnemy)
                    {
                        if (targetAllFriendly)
                        {
                            foreach (Minion m in p.ownMinions) if (!m.untouchable) targets.Add(m);
                        }
                        else
                        {
                            foreach (Minion m in p.enemyMinions) if (!m.untouchable) targets.Add(m);
                        }
                    }
                    else
                    {
                        foreach (Minion m in p.ownMinions) if (!m.untouchable) targets.Add(m);
                        foreach (Minion m in p.enemyMinions) if (!m.untouchable) targets.Add(m);
                    }
                    if (targetOnlyMinion)
                    {
                        targetEnemyHero = false;
                        targetOwnHero = false;
                    }
                    else
                    {
                        if (!enemyHero.immune) targetEnemyHero = true;
                        if (!ownHero.immune) targetOwnHero = true;
                        if (targetAllEnemy) targetOwnHero = false;
                        if (targetAllFriendly) targetEnemyHero = false;
                    }


                }

                if (extraParam)
                {
                    wereTargets = true;
                    if (REQ_TARGET_MUST_HAVE_TAG)
                    {
                        foreach (Minion m in targets)
                        {
                            switch (this.needTagForPlaying)
                            {
                                case Specialtags.CardRace:
                                    m.extraParam = !(m.handcard.card.GetRaceCount() > 0);
                                    break;
                                case Specialtags.IMP:
                                    m.extraParam = !m.handcard.card.IMP;
                                    break;
                                case Specialtags.markOfEvil:
                                    m.extraParam = !m.handcard.card.markOfEvil;
                                    break;
                                case Specialtags.Treant:
                                    m.extraParam = !m.handcard.card.Treant;
                                    break;
                                case Specialtags.Whelp:
                                    m.extraParam = !m.handcard.card.Whelp;
                                    break;
                                case Specialtags.StarshipPiece:
                                case Specialtags.Starship:
                                    m.extraParam = !m.handcard.card.Starship && !m.handcard.card.StarshipPiece;
                                    break;
                                case Specialtags.Protoss:
                                    m.extraParam = !m.handcard.card.Protoss;
                                    break;


                            }
                        }

                    }
                    if (REQ_TARGET_IS_NON_TITAN)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.handcard.card.Titan;
                        }
                    }
                    if (REQ_TARGET_SILVER_HAND_RECRUIT)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.handcard.card.SilverHandRecruit;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }

                    if (REQ_TARGET_WITH_RACE)
                    {
                        foreach (Minion m in targets)
                        {
                            // 不满足使用条件（或者是融合怪）
                            // if (m.handcard.card.race != (Race)this.needRaceForPlaying && m.handcard.card.race != Race.ALL) m.extraParam = true;
                            // if (m.handcard.card.race != (Race)this.needRaceForPlaying && m.handcard.card.race != Race.ALL) m.extraParam = true;
                            m.extraParam = !RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), (CardDB.Race)this.needRaceForPlaying);
                        }
                        // targetOwnHero = (p.ownHeroName == HeroEnum.lordjaraxxus && (TAG_RACE)this.needRaceForPlaying == TAG_RACE.DEMON);
                        // targetEnemyHero = (p.enemyHeroName == HeroEnum.lordjaraxxus && (TAG_RACE)this.needRaceForPlaying == TAG_RACE.DEMON);
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }

                    if (REQ_FRIENDLY_MINION_OF_RACE_IN_HAND)
                    {
                        if (own)
                        {
                            foreach (Handmanager.Handcard hc in p.owncards)
                            {
                                if (RaceUtils.MinionBelongsToRace(hc.card.GetRaces(), (CardDB.Race)this.needRaceInHand)) { targetAll = true; break; }
                            }
                        }
                        else targetAll = true; // apriori the enemy have a dragon
                    }
                    if (REQ_HERO_TARGET)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = true;
                        }
                        targetOwnHero = true;
                        targetEnemyHero = true;
                    }



                    if (REQ_DAMAGED_TARGET)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.wounded;
                        }
                        targetOwnHero = ownHero.wounded;
                        targetEnemyHero = enemyHero.wounded;
                    }
                    if (REQ_DAMAGED_TARGET_UNLESS_COMBO)
                    {
                        if (p.cardsPlayedThisTurn >= 1){
                            foreach (Minion m in targets)
                            {
                                m.extraParam = false;
                            }
                            targetOwnHero = ownHero.wounded;
                            targetEnemyHero = enemyHero.wounded;
                        }
                        else
                        {

                            foreach (Minion m in targets)
                            {
                                m.extraParam = !m.wounded;
                            }
                            targetOwnHero = ownHero.wounded;
                            targetEnemyHero = enemyHero.wounded;
                        }
                    }
                    //本回合英雄血量变化
                    if (REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN)
                    {

                    }

                    if (REQ_TARGET_MAX_ATTACK)
                    {
                        foreach (Minion m in targets)
                        {
                            if (m.Angr > this.needWithMaxAttackValueOf)
                            {
                                m.extraParam = true;
                            }
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_TARGET_MIN_ATTACK)
                    {
                        foreach (Minion m in targets)
                        {
                            if (m.Angr < this.needWithMinAttackValueOf)
                            {
                                m.extraParam = true;
                            }
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_TARGET_EXACT_ATTACK)
                    {
                        foreach (Minion m in targets)
                        {
                            if (m.Angr == this.needWithExactAttackValueOf)
                            {
                                m.extraParam = true;
                            }
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_MUST_TARGET_TAUNTER)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.taunt;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_UNDAMAGED_TARGET)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = m.wounded;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_STEALTHED_TARGET)
                    {
                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.stealth;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_TARGET_WITH_DEATHRATTLE)
                    {
                        foreach (Minion m in targets)
                        {
                            if (!m.silenced && (m.handcard.card.deathrattle || m.deathrattle2 != null ||
                            m.ancestralspirit + m.desperatestand + m.souloftheforest + m.stegodon + m.livingspores + m.explorershat + m.returnToHand + m.infest + m.itsnecrolit + m.sheepmask > 0)) continue;
                            else m.extraParam = true;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_TARGET_NO_NATURE)
                    {
                        if (p.useNature < 1)
                        {
                            foreach (Minion m in targets) m.extraParam = true;
                            targetOwnHero = false;
                            targetEnemyHero = false;
                        }
                    }
                    if (REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN)
                    {
                        if (p.anzOwnElementalsLastTurn < 1)
                        {
                            foreach (Minion m in targets) m.extraParam = true;
                            targetOwnHero = false;
                            targetEnemyHero = false;
                        }
                    }
                    if (REQ_TARGET_NO_NATURE)
                    {
                        if (p.useNature < 1)
                        {
                            foreach (Minion m in targets) m.extraParam = true;
                            targetOwnHero = false;
                            targetEnemyHero = false;
                        }
                    }
                    if (REQ_LEGENDARY_TARGET)
                    {
                        wereTargets = false;
                        foreach (Minion m in targets)
                        {
                            if (m.handcard.card.rarity != 5) m.extraParam = true;
                        }
                        targetOwnHero = false;
                        targetEnemyHero = false;
                    }
                    if (REQ_STEADY_SHOT)
                    {
                        if ((p.weHaveSteamwheedleSniper && own) || (p.enemyHaveSteamwheedleSniper && !own))
                        {
                            foreach (Minion m in targets)
                            {
                                // if (m.cantBeTargetedBySpellsOrHeroPowers && (this.type == cardtype.HEROPWR || this.type == cardtype.SPELL))
                                if (m.Elusive && (this.type == cardtype.HEROPWR || this.type == cardtype.SPELL))
                                {
                                    m.extraParam = true;
                                    if (m.stealth && !m.own) m.extraParam = true;
                                }
                            }
                            if (own) targetEnemyHero = true;
                            else targetOwnHero = true;
                        }
                        else wereTargets = false;
                    }
                    if (REQ_FROZEN_TARGET)
                    {

                        foreach (Minion m in targets)
                        {
                            m.extraParam = !m.frozen;
                        }
                    }
                    if (REQ_LOCATION_TARGET)
                    {

                        foreach (Minion m in targets)
                        {
                            if (m.handcard.card.type != cardtype.LOCATION) m.extraParam = true;
                            else result.Add(m);
                        }
                        targetEnemyHero = false;
                        targetOwnHero = false;
                    }
                }

                if (targetEnemyHero && own && enemyHero.stealth) targetEnemyHero = false;
                if (targetOwnHero && !own && ownHero.stealth) targetOwnHero = false;

                //斩杀
                if (isLethalCheck)
                {
                    if (targetEnemyHero && own) result.Add(enemyHero);
                    else if (targetOwnHero && !own) result.Add(ownHero);

                    switch (this.type)
                    {
                        case cardtype.SPELL:
                            if (p.prozis.penman.attackBuffDatabase.ContainsKey(this.nameEN))
                            {
                                if (targetOwnHero && own) result.Add(ownHero);
                                foreach (Minion m in targets)
                                {
                                    if (m.extraParam != true && !m.Elusive)
                                    {
                                        if (m.own)
                                        {
                                            if (m.Ready) result.Add(m);
                                        }
                                        else if (m.taunt) result.Add(m);
                                    }
                                    m.extraParam = false;
                                }
                            }
                            else
                            {
                                switch (this.nameEN)
                                {
                                    case cardNameEN.polymorphboar://变形术：野猪
                                        foreach (Minion m in targets)
                                        {
                                            m.extraParam = false;
                                            if (m.Elusive) continue;
                                            if (m.own) result.Add(m);
                                            else if (m.taunt) result.Add(m);
                                        }
                                        break;
                                    // case cardNameEN.hex: goto case cardNameEN.polymorph;//妖术
                                    case cardNameEN.hex://妖术
                                    case cardNameEN.polymorph://变形术
                                        foreach (Minion m in targets)
                                        {
                                            m.extraParam = false;
                                            if (!m.own && m.taunt && !m.cantBeTargetedBySpellsOrHeroPowers) result.Add(m);
                                        }
                                        break;
                                }
                            }
                            break;
                        case cardtype.MOB:
                            foreach (Minion m in targets)
                            {
                                if (!m.extraParam)
                                {
                                    if (m.stealth && !m.own) continue;
                                    result.Add(m);
                                }
                                m.extraParam = false;
                            }
                            break;
                        case cardtype.HEROPWR:
                            if (p.prozis.penman.attackBuffDatabase.ContainsKey(this.nameEN))
                            {
                                foreach (Minion m in targets)
                                {
                                    if (!m.extraParam && !m.cantBeTargetedBySpellsOrHeroPowers)
                                    {
                                        if (m.own)
                                        {
                                            if (m.Ready) result.Add(m);
                                        }
                                        else if (m.taunt) result.Add(m);
                                    }
                                    m.extraParam = false;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (targetEnemyHero) result.Add(enemyHero);
                    if (targetOwnHero) result.Add(ownHero);

                    foreach (Minion m in targets)
                    {
                        if (!m.extraParam)
                        {
                            if (m.stealth && !m.own) continue;
                            if (m.Elusive && (this.type == cardtype.SPELL || this.type == cardtype.HEROPWR)) continue;
                            result.Add(m);
                        }
                        m.extraParam = false;
                    }
                }

                //非地标目标指向，移除地标
                if (!REQ_LOCATION_TARGET)
                {


                    result.RemoveAll(minion =>
                          minion?.handcard?.card.type == CardDB.cardtype.LOCATION);
                }
                else
                {

                    result.RemoveAll(minion =>
                             (minion?.handcard?.card.type == CardDB.cardtype.MOB || minion?.handcard?.card.type == CardDB.cardtype.HERO));
                }

                //移除不可接触的随从
                result.RemoveAll(minion => minion?.handcard?.card.untouchable == true);

                if (result.Count == 0 && (!wereTargets || REQ_TARGET_IF_AVAILABLE)) result.Add(null);

                return result;

            }
            /// <summary>
            /// 获得卡牌的目标
            /// </summary>
            /// <param name="p"></param>
            /// <param name="isLethalCheck"></param>
            /// <param name="own"></param>
            /// <returns></returns>
            public List<Minion> getTargetsForCard(Playfield p, bool isLethalCheck, bool own)
            {
                //if wereTargets=true and 0 targets at end -> then can not play this card
                PlayReq[] playReqs = this.sim_card.GetPlayReqs();
                List<Minion> result = new List<Minion>();
                if ((this.type == CardDB.cardtype.MOB || this.type == CardDB.cardtype.LOCATION) && ((own && p.ownMinions.Count >= 7) || (!own && p.enemyMinions.Count >= 7))) return result; // cant play mob, if we have allready 7 mininos
                if (this.Secret && ((own && (p.ownSecretsIDList.Contains(this.cardIDenum) || p.ownSecretsIDList.Count >= 5)) || (!own && p.enemySecretCount >= 5))) return result;
                if (!string.IsNullOrWhiteSpace(this.TreatItAsTheSameCard))
                {
                    Card card = Instance.getCardDataFromDbfID(this.TreatItAsTheSameCard);
                    this.sim_card = card.sim_card;
                    playReqs = this.sim_card.GetPlayReqs();
                }
                if (playReqs.Length == 0) { result.Add(null); return result; }

                result = this.getTargets(p, p.isLethalCheck, own, result, playReqs);
                //如果是法术或英雄技能，移除扰魔目标
                if (this.type == CardDB.cardtype.SPELL || this.type == CardDB.cardtype.HEROPWR)
                {
                    result.RemoveAll(minion =>
                          minion?.handcard?.card.Elusive == true);
                }
                return result;
            }

            /// <summary>
            /// 获取英雄技能的目标
            /// </summary>
            /// <param name="p">场面</param>
            /// <param name="own">是否自己打出</param>
            /// <returns></returns>
            public List<Minion> getTargetsForHeroPower(Playfield p, bool own)
            {

                var targetsForHeroPower = getTargetsForCard(p, p.isLethalCheck, own);
                var abName = own ? p.ownHeroAblility.card.nameEN : p.enemyHeroAblility.card.nameEN;
                var abType = 0; //0 none, 1 damage, 2 heal, 3 buff
                switch (abName)
                {
                    case cardNameEN.heal:
                    case cardNameEN.lesserheal:
                        if (p.anzOwnAuchenaiSoulpriest > 0 || p.embracetheshadow > 0) abType = 1;
                        else abType = 2;
                        break;
                    case cardNameEN.ballistashot: abType = 1; break;
                    case cardNameEN.steadyshot: abType = 1; break;
                    case cardNameEN.fireblast: abType = 1; break;
                    case cardNameEN.fireblastrank2: abType = 1; break;
                    case cardNameEN.lightningjolt: abType = 1; break;
                    case cardNameEN.mindspike: abType = 1; break;
                    case cardNameEN.mindshatter: abType = 1; break;
                    case cardNameEN.powerofthefirelord: abType = 1; break;
                    case cardNameEN.shotgunblast: abType = 1; break;
                    case cardNameEN.unbalancingstrike: abType = 1; break;
                    case cardNameEN.dinomancy: abType = 3; break;
                }

                switch (abType)
                {
                    case 2:
                        List<Minion> minions = own ? p.ownMinions : p.enemyMinions;
                        int tCount = minions.Count;
                        bool needCut = true;
                        for (int i = 0; i < tCount; i++)
                        {
                            switch (minions[i].name)
                            {
                                case cardNameEN.shadowboxer:
                                    if (own && p.enemyHero.Hp == 1 && p.enemyMinions.Count > 0) needCut = false;
                                    break;
                                case cardNameEN.holychampion: needCut = false; break;
                                case cardNameEN.lightwarden: needCut = false; break;
                                case cardNameEN.northshirecleric: needCut = false; break;


                            }
                        }

                        tCount = targetsForHeroPower.Count;
                        if (tCount > 0)
                        {
                            if (targetsForHeroPower[0] != null)
                            {
                                List<Minion> tmp = new List<Minion>();
                                for (int i = 0; i < tCount; i++)
                                {
                                    Minion m = targetsForHeroPower[i];
                                    if (m.Hp < m.maxHp)
                                    {
                                        if (needCut)
                                        {
                                            if (m.own == own) tmp.Add(m);
                                        }
                                        else tmp.Add(m);
                                    }
                                }
                                return tmp;
                            }
                        }
                        break;
                }

                return targetsForHeroPower;
            }

            /// <summary>
            /// 获得地标卡牌的目标
            /// </summary>
            /// <param name="p">当前的游戏状态</param>
            /// <param name="isLethalCheck">是否进行斩杀检测</param>
            /// <param="own">是否为自己的回合</param>
            /// <returns>返回可以选择的随从目标列表</returns>
            public List<Minion> getTargetsForLocation(Playfield p, bool isLethalCheck, bool own)
            {
                List<Minion> result = new List<Minion>();
                PlayReq[] playReq = this.sim_card.GetUseAbilityReqs();

                if (this.MODULAR_ENTITY_PART_1 != 0 & this.MODULAR_ENTITY_PART_2 != 0)
                {
                    CardDB.Card modular1 = CardDB.Instance.getCardDataFromDbfID(this.MODULAR_ENTITY_PART_1.ToString());
                    CardDB.Card modular2 = CardDB.Instance.getCardDataFromDbfID(this.MODULAR_ENTITY_PART_2.ToString());
                    PlayReq[] modular1PlayReq = modular1.sim_card.GetPlayReqs();
                    PlayReq[] modular2PlayReq = modular2.sim_card.GetPlayReqs();
                    PlayReq[] tempPlayReq = new PlayReq[playReq.Length + modular1PlayReq.Length + modular2PlayReq.Length];
                    Array.Copy(modular1PlayReq, 0, tempPlayReq, 0, modular1PlayReq.Length);
                    Array.Copy(modular2PlayReq, 0, tempPlayReq, modular1PlayReq.Length, modular2PlayReq.Length);
                    playReq = tempPlayReq;
                }
                if (playReq.Length == 0)
                {
                    result.Add(null);
                    return result;
                }
                result = this.getTargets(p, isLethalCheck, own, result, playReq);

                if (playReq.Length == 0)
                {
                    result.Add(null);
                    return result;
                }
                return result;
            }

            /// <summary>
            /// 计算费用 会减费的牌需要在里面写
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public int calculateManaCost(Playfield p)//calculates the mana from orginal mana, needed for back-to hand effects and new draw
            {
                int retval = this.cost;//卡牌本身的费用
                int offset = 0;//每个随从的减费

                if (p.anzOwnShadowfiend > 0) offset -= p.anzOwnShadowfiend;//暗影狂乱 需要费用减去抓的怪费用

                switch (this.type)
                {
                    case cardtype.MOB:
                        if (p.anzOwnAviana > 0) retval = 1;//av娜

                        if (p.anzOwnScargil > 0 && (this.race == Race.MURLOC || this.race == Race.ALL)) retval = 1;//斯卡基尔

                        if (p.ownDemonCostLessOnce > 0 && (this.race == Race.DEMON || this.race == Race.ALL))
                            offset -= p.ownDemonCostLessOnce;

                        offset += p.ownMinionsCostMore;//随从消耗更多

                        if (this.deathrattle) offset += p.ownDRcardsCostMore;

                        offset += p.managespenst;

                        int temp = -(p.startedWithbeschwoerungsportal) * 2;
                        if (retval + temp <= 0) temp = -retval + 1;//传送门 负数
                        offset = offset + temp;

                        if (p.mobsplayedThisTurn == 0)
                        {//消耗血
                            offset -= p.winzigebeschwoererin;
                        }

                        if (this.battlecry)
                        {
                            offset += p.nerubarweblord * 2;//尼鲁巴蛛网领主
                        }

                        if ((TAG_RACE)this.race == TAG_RACE.MECHANICAL)
                        { //if the number of zauberlehrlings change
                            offset -= p.anzOwnMechwarper;//Mechwarper机械跃迁
                        }
                        break;
                    case cardtype.SPELL:
                        if (p.nextSpellThisTurnCost0) return 0;//这个标志位在sim卡里
                        offset += p.ownSpelsCostMore;
                        if (p.playedPreparation)//伺机待发
                        { //if the number of zauberlehrlings change
                            offset -= 2;
                        }
                        break;
                    case cardtype.WEAPON:
                        offset -= p.blackwaterpirate * 2;//黑水海盗
                        if (this.deathrattle) offset += p.ownDRcardsCostMore;
                        break;
                }

                offset -= p.myCardsCostLess;

                switch (this.nameCN)
                {
                    case CardDB.cardNameCN.希望圣契:
                    case CardDB.cardNameCN.正义圣契:
                    case CardDB.cardNameCN.智慧圣契:
                    case CardDB.cardNameCN.审判圣契:
                        retval = retval + offset - p.libram;
                        break;
                }
                retval = retval + offset;
                if (this.Secret)
                {
                    if (p.anzOwnCloakedHuntress > 0 || p.nextSecretThisTurnCost0) retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            /// <summary>
            /// 获取卡牌费用
            /// </summary>
            /// <param name="p"></param>
            /// <param name="currentcost"></param>
            /// <returns></returns>
            public int getManaCost(Playfield p, int currentcost)
            {
                int retval = currentcost;

                // offset < 0费用降低,offset > 0费用增加
                int offset = 0; // if offset < 0 costs become lower, if >0 costs are higher at the end

                // CARDS that increase/decrease the manacosts of others ##############################
                //卡片增加减少卡片法力消耗
                switch (this.type)
                {
                    case cardtype.HEROPWR:
                        retval += p.ownHeroPowerCostLessOnce;
                        if (retval < 0) retval = 0;
                        return retval;
                    case cardtype.MOB:

                        if (p.ownMinionsCostMore != p.ownMinionsCostMoreAtStart)
                        {
                            offset += (p.ownMinionsCostMore - p.ownMinionsCostMoreAtStart);
                        }//


                        if (this.deathrattle && p.ownDRcardsCostMore != p.ownDRcardsCostMoreAtStart)
                        {
                            offset += (p.ownDRcardsCostMore - p.ownDRcardsCostMoreAtStart);
                        }


                        if (p.managespenst != p.startedWithManagespenst)
                        {
                            offset += (p.managespenst - p.startedWithManagespenst);
                        }


                        if (this.battlecry && p.nerubarweblord != p.startedWithnerubarweblord)
                        {
                            offset += (p.nerubarweblord - p.startedWithnerubarweblord) * 2;
                        }


                        if (p.anzOwnAviana > 0)
                        {
                            retval = 1;
                        }

                        if (p.anzOwnScargil > 0 && (this.race == Race.MURLOC || this.race == Race.ALL))
                        {
                            retval = 1;
                        }

                        if (p.anzOwnMechwarper != p.anzOwnMechwarperStarted && (TAG_RACE)this.race == TAG_RACE.MECHANICAL)
                        {
                            offset += (p.anzOwnMechwarperStarted - p.anzOwnMechwarper);
                        }


                        if (p.startedWithbeschwoerungsportal != p.beschwoerungsportal)
                        {
                            offset += (p.startedWithbeschwoerungsportal - p.beschwoerungsportal) * 2;
                        }


                        if (p.winzigebeschwoererin != p.startedWithWinzigebeschwoererin && ((p.turnCounter == 0 && p.startedWithMobsPlayedThisTurn == 0) || (p.turnCounter > 0 && p.mobsplayedThisTurn == 0)))
                        {
                            offset += (p.startedWithWinzigebeschwoererin - p.winzigebeschwoererin);
                        }

                        //卫兵
                        if (p.winRazormaneBattleguard != p.startedRazormaneBattleguard && this.tank)
                        {
                            offset -= p.winRazormaneBattleguard * 2;
                        }

                        if (p.anzOwnDragonConsort != p.anzOwnDragonConsortStarted && (TAG_RACE)this.race == TAG_RACE.DRAGON)
                        {
                            offset += (p.anzOwnDragonConsortStarted - p.anzOwnDragonConsort) * 2;
                        }

                        //火光元素
                        if (p.ownElementCost != p.ownElementCostStarted && (TAG_RACE)this.race == TAG_RACE.ELEMENTAL)
                        {
                            offset += p.ownElementCostStarted - p.ownElementCost;
                        }

                        //雷矛军用山羊
                        if (p.ownBeastCostLessOnce != p.ownBeastCostLessOnceStarted && (TAG_RACE)this.race == TAG_RACE.PET)
                        {
                            offset += (p.ownBeastCostLessOnceStarted - p.ownBeastCostLessOnce) * 2;
                        }

                        // 下一张元素随从牌的法力值消耗减少量
                        if (p.nextElementalReduction > 0 && (TAG_RACE)this.race == TAG_RACE.ELEMENTAL)
                        {
                            offset -= p.nextElementalReduction;
                        }

                        // 本回合下一张元素随从牌的法力值消耗减少量
                        if (p.thisTurnNextElementalReduction > 0 && (TAG_RACE)this.race == TAG_RACE.ELEMENTAL)
                        {
                            offset -= p.thisTurnNextElementalReduction;
                        }

                        break;
                    case cardtype.SPELL:

                        if (p.nextSpellThisTurnCost0) return 0;


                        if (p.ownSpelsCostMoreAtStart != p.ownSpelsCostMore)
                        {
                            offset += p.ownSpelsCostMore - p.ownSpelsCostMoreAtStart;
                        }

                        if (p.anzOwnPopGarThePutrid > 0)
                        {
                            offset -= p.anzOwnPopGarThePutrid * 2;
                        }

                        if (p.playedPreparation)
                        {
                            offset -= 2;
                        }
                        break;
                    case cardtype.WEAPON:

                        if (p.blackwaterpirateStarted != p.blackwaterpirate)
                        {
                            offset += (p.blackwaterpirateStarted - p.blackwaterpirate) * 2;
                        }

                        if (this.deathrattle && p.ownDRcardsCostMore != p.ownDRcardsCostMoreAtStart)
                        {
                            offset += (p.ownDRcardsCostMore - p.ownDRcardsCostMoreAtStart);
                        }
                        break;
                }


                if (p.startedWithmyCardsCostLess != p.myCardsCostLess)
                {
                    offset += p.startedWithmyCardsCostLess - p.myCardsCostLess;
                }
                switch (this.nameEN)
                {
                    case CardDB.cardNameEN.frenziedfelwing://狂暴邪翼蝠
                        if (p.enemyHero.Hp < p.enemyHeroTurnStartedHp)
                        {
                            retval = retval + offset - (p.enemyHeroTurnStartedHp - p.enemyHero.Hp);
                        }
                        break;
                    case CardDB.cardNameEN.libramofwisdom: //智慧圣契
                    case CardDB.cardNameEN.libramofjustice: //正义圣契
                    case CardDB.cardNameEN.libramofhope: //希望圣契
                    case CardDB.cardNameEN.libramofjudgment: //审判圣契
                    case CardDB.cardNameEN.libramofjudgment_YOP_011t: //审判圣契已腐蚀
                    case CardDB.cardNameEN.libramofdivinity: //信仰圣契
                    case CardDB.cardNameEN.libramoffaith: //神性圣契
                        retval = retval + offset - p.libram;
                        break;
                    case CardDB.cardNameEN.volcaniclumberer: //火山邪木
                    case CardDB.cardNameEN.dragonsbreath: //龙息术
                    case CardDB.cardNameEN.solemnvigil: //严正警戒
                    case CardDB.cardNameEN.volcanicdrake: //火山幼龙
                    case CardDB.cardNameEN.volcanicdrake_Story_09_VolcanicDrake: //火山幼龙
                        retval = retval + offset - p.ownMinionsDiedTurn - p.enemyMinionsDiedTurn;
                        break;
                    case CardDB.cardNameEN.bookofthedead: //亡者之书
                    case CardDB.cardNameEN.bookofthedead_PVPDR_SCH_Active54: //亡者之书
                    case CardDB.cardNameEN.bookofthedead_ULDA_006: //亡者之书
                    case CardDB.cardNameEN.bookofthedead_VAC_464t24: //亡者之书
                    case CardDB.cardNameEN.reskathepitboss: //矿坑老板雷斯卡
                        retval = retval + offset - p.tempTrigger.ownMinionsDied - p.tempTrigger.enemyMinionsDied;
                        break;
                    case CardDB.cardNameEN.urzulgiant: //乌祖尔巨人
                        retval = retval + offset - p.tempTrigger.ownMinionsDied;
                        break;
                    case CardDB.cardNameEN.mulchmuncher: //植被破碎机
                        retval = retval + offset - p.tempTrigger.ownTreantDied;
                        break;
                    case CardDB.cardNameEN.knightofthewild: //荒野骑士
                    case CardDB.cardNameEN.knightofthewild_WON_003: //荒野骑士
                    case CardDB.cardNameEN.frostsabermatriarch: //霜刃豹头领
                        retval = retval + offset - p.tempTrigger.ownBeastSummoned;
                        break;
                    case CardDB.cardNameEN.fyethesettingsun: //落日灵龙菲伊
                        retval = retval + offset - p.tempTrigger.ownDragonSummoned;
                        break;
                    case CardDB.cardNameEN.cultivation: //栽培
                        retval = retval + offset - p.tempTrigger.ownTreantDied;
                        break;
                    case CardDB.cardNameEN.dreadcorsair: //恐怖海盗
                    case CardDB.cardNameEN.dreadcorsair_NEW1_022: //恐怖海盗
                    case CardDB.cardNameEN.dreadcorsair_VAN_NEW1_022: //恐怖海盗
                    case CardDB.cardNameEN.cuttingclass: //劈砍课程
                    case CardDB.cardNameEN.ravenousdrake: //贪食幼龙
                    case CardDB.cardNameEN.battlehawkstrider: //战斗陆行鸟
                        retval = retval + offset - p.ownWeapon.Angr + p.ownWeaponAttackStarted; // if weapon attack change we change manacost
                        break;
                    case CardDB.cardNameEN.spreadtheword_ETC_384: //散布消息
                        retval = retval + offset - p.ownHero.Angr;
                        break;
                    case CardDB.cardNameEN.thingfrombelow://深渊魔物
                    case CardDB.cardNameEN.gigantotem://图腾巨像
                        if (p.playactions.Count > 0)
                        {
                            foreach (Action a in p.playactions)
                            {
                                if (a.actionType == actionEnum.playcard)
                                {
                                    switch (a.hc.card.nameEN)
                                    {
                                        case cardNameEN.tuskarrtotemic: retval -= p.ownBrannBronzebeard + 1; break;
                                        case cardNameEN.splittingaxe://分裂战斧
                                            int ownTotemsCount = 0;
                                            foreach (Minion m in p.ownMinions)
                                            {
                                                if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM) ownTotemsCount++;
                                            }
                                            retval -= ownTotemsCount;
                                            break;
                                        default:
                                            if ((TAG_RACE)a.hc.card.race == TAG_RACE.TOTEM) retval--;
                                            break;
                                    }
                                }
                                else if (a.actionType == actionEnum.useHeroPower)
                                {
                                    switch (a.hc.card.nameEN)
                                    {
                                        case cardNameEN.totemiccall: retval--; break;
                                        case cardNameEN.totemicslam: retval--; break;
                                    }
                                }
                            }
                        }
                        retval = retval + offset;
                        break;
                    case CardDB.cardNameEN.mountaingiant: //山岭巨人
                    case CardDB.cardNameEN.mountaingiant_EX1_105: //山岭巨人
                    case CardDB.cardNameEN.mountaingiant_VAN_EX1_105: //山岭巨人
                    case CardDB.cardNameEN.thesunwell: //太阳之井
                    case CardDB.cardNameEN.goldshiregnoll: //闪金镇豺狼人
                    case CardDB.cardNameEN.tableflip: //掀桌子
                    case CardDB.cardNameEN.livinghorizon: //活体天光
                        retval = retval + offset - (p.owncards.Count - 1) + p.ownCardsCountStarted;
                        break;
                    case CardDB.cardNameEN.clockworkgiant: //发条巨人
                        retval = retval + offset - p.enemyAnzCards + p.enemyCardsCountStarted;
                        break;
                    case CardDB.cardNameEN.moltengiant: //熔核巨人
                    case CardDB.cardNameEN.moltengiant_EX1_620: //熔核巨人
                    case CardDB.cardNameEN.moltengiant_LETLT_081_01: //熔核巨人
                    case CardDB.cardNameEN.moltengiant_Story_11_MoltenGiantPuzzle: //熔核巨人
                    case CardDB.cardNameEN.moltengiant_VAN_EX1_620: //熔核巨人
                        retval = retval + offset - p.ownHeroHpStarted + p.ownHero.Hp;
                        break;
                    case CardDB.cardNameEN.frostgiant:
                        retval = retval + offset - p.anzUsedOwnHeroPower;
                        break;
                    case CardDB.cardNameEN.arcanegiant: //奥术巨人
                    case CardDB.cardNameEN.gravehorror: //墓园恐魔
                    case CardDB.cardNameEN.umbralowl: //幽影猫头鹰
                    case CardDB.cardNameEN.umbralowl_DMF_060: //幽影猫头鹰
                        retval = retval + offset - p.spellsplayedSinceRecalc;
                        break;
                    case CardDB.cardNameEN.snowfurygiant: //雪怒巨人
                    case CardDB.cardNameEN.snowfurygiant_ICC_090: //雪怒巨人
                        retval = retval + offset - p.ueberladung;
                        break;
                    case CardDB.cardNameEN.kabalcrystalrunner: //暗金教水晶侍女
                    case CardDB.cardNameEN.kabalcrystalrunner_WON_308: //暗金教水晶侍女
                        retval = retval + offset - 2 * p.secretsplayedSinceRecalc;
                        break;
                    case CardDB.cardNameEN.seagiant: //海巨人
                    case CardDB.cardNameEN.mogufleshshaper: //魔古血肉塑型者
                        {
                            int MinionsCount = 0;

                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.type == cardtype.MOB)
                                    MinionsCount++;
                            }
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.type == cardtype.MOB)
                                    MinionsCount++;
                            }

                            retval = retval + offset - MinionsCount + p.ownMinionStartCount + p.enemyMinionStartCount;
                        }
                        // retval = retval + offset - p.ownMinions.Count - p.enemyMinions.Count + p.ownMobsCountStarted + p.enemyMobsCountStarted;
                        break;
                    case CardDB.cardNameEN.bloodboilbrute: //沸血蛮兵
                        {
                            int woundedMinionsCount = 0;

                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.type == cardtype.MOB && m.wounded)
                                    woundedMinionsCount++;
                            }
                            foreach (Minion m in p.enemyMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.type == cardtype.MOB && m.wounded)
                                    woundedMinionsCount++;
                            }
                            retval = retval + offset - woundedMinionsCount + p.ownMinionStartCount + p.enemyMinionStartCount;
                        }
                        break;
                    case CardDB.cardNameEN.demonbolt: //恶魔之箭
                        {
                            int ownMinionsCount = 0;
                            if (p.ownMinions.Count > 0)
                            {
                                foreach (Minion m in p.ownMinions)
                                {
                                    if (m.untouchable || m.dormant > 0) continue;
                                    if (m.handcard.card.type == cardtype.MOB)
                                        ownMinionsCount++;
                                }
                            }
                            retval = retval + offset - ownMinionsCount + p.ownMinionStartCount;
                        }
                        break;
                    case CardDB.cardNameEN.rabblebouncer: //场馆保镖
                    case CardDB.cardNameEN.prismaticbeam: //棱彩光束
                    case CardDB.cardNameEN.eredarbrute: //艾瑞达蛮兵
                        {
                            int enemyMinionsCount = 0;
                            if (p.enemyMinions.Count > 0)
                            {
                                foreach (Minion m in p.enemyMinions)
                                {
                                    if (m.untouchable || m.dormant > 0) continue;
                                    if (m.handcard.card.type == cardtype.MOB)
                                        enemyMinionsCount++;
                                }
                            }
                            retval = retval + offset - enemyMinionsCount + p.enemyMinionStartCount;
                        }
                        break;
                    case CardDB.cardNameEN.secondratebruiser: //二流打手
                        retval = retval + offset - ((p.enemyMinions.Count < 3) ? 0 : 2) + ((p.enemyMobsCountStarted < 3) ? 0 : 2);
                        break;
                    case CardDB.cardNameEN.golemagg: //古雷曼格
                        retval = retval + offset - p.ownHeroHpStarted + p.ownHero.Hp;
                        break;
                    case CardDB.cardNameEN.skycapnkragg: //天空上尉库拉格
                        {
                            int costBonusPirate = 0;
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.handcard.card.race == CardDB.Race.PIRATE || m.handcard.card.race == CardDB.Race.ALL) costBonusPirate++;
                            }
                            retval = retval + offset - costBonusPirate + p.anzOwnPiratesStarted;
                        }
                        break;
                    case CardDB.cardNameEN.everyfinisawesome: //鱼人恩典
                        {
                            int costBonusMurloc = 0;
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.race == Race.MURLOC || m.handcard.card.race == Race.ALL) costBonusMurloc++;
                            }
                            retval = retval + offset - costBonusMurloc + p.anzOwnMurlocStarted;
                        }
                        break;
                    case CardDB.cardNameEN.solarflare: //阳炎耀斑
                        {
                            int costBonusElemental = 0;
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.race == Race.ELEMENTAL || m.handcard.card.race == Race.ALL) costBonusElemental++;
                            }
                            retval = retval + offset - costBonusElemental + p.anzOwnElementStarted;
                        }
                        break;
                    case CardDB.cardNameEN.captainslog: //舰长日志
                        {
                            int costBonusDraenei = 0;
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.untouchable || m.dormant > 0) continue;
                                if (m.handcard.card.race == Race.DRAENEI || m.handcard.card.race == Race.ALL) costBonusDraenei++;
                            }
                            retval = retval + offset - costBonusDraenei + p.anzOwnDraeneiStarted;
                        }
                        break;
                    case CardDB.cardNameEN.aeroponics: //空气栽培
                        {
                            int costBonusTreant = 0;
                            if (p.ownMinions.Count > 0)
                            {
                                foreach (Minion m in p.ownMinions)
                                {
                                    if (m.untouchable || m.dormant > 0) continue;
                                    if (m.handcard.card.Terran)
                                        costBonusTreant++;
                                }
                            }
                            retval = retval + offset - costBonusTreant + p.anzOwnTreantStarted;
                        }
                        break;
                    case CardDB.cardNameEN.fleshgiant: // 血肉巨人
                        retval = retval + offset - p.healOrDamageTimes;
                        break;
                    case CardDB.cardNameEN.crush: //重碾
                        // cost 4 less if we have a dmged minion
                        {
                            bool dmgedminions = false;
                            foreach (Minion m in p.ownMinions)
                            {
                                if (m.wounded) dmgedminions = true;
                            }
                            if (dmgedminions != p.startedWithDamagedMinions)
                            {
                                if (dmgedminions) retval = retval + offset - 4;
                                else retval = retval + offset + 4;
                            }
                        }
                        break;
                    case CardDB.cardNameEN.happyghoul: //开心的食尸鬼
                    case CardDB.cardNameEN.happyghoul_ICC_700: //开心的食尸鬼
                        if (p.healTimes > 0)
                            retval = 0 + offset;
                        break;
                    case CardDB.cardNameEN.wildmagic: //狂野魔法
                        retval = 0;
                        break;
                    case CardDB.cardNameEN.shieldshatter: //裂盾一击
                    case CardDB.cardNameEN.cryptkeeper: //地穴看守者
                        retval = retval + offset - p.ownHero.armor;
                        break;
                    case CardDB.cardNameEN.safetygoggles: //安全护目镜'
                        if (p.ownHero.armor == 0)
                            retval = 0;
                        break;
                    case CardDB.cardNameEN.frostwolfwarmaster: //霜狼将领
                    case CardDB.cardNameEN.scribblingstenographer: //潦草的书记员
                    case CardDB.cardNameEN.scribblingstenographer_MAW_020: //潦草的书记员
                    case CardDB.cardNameEN.everburningphoenix: //永燃火凤
                        retval = retval + offset - p.cardsPlayedThisTurn;
                        break;
                    case CardDB.cardNameEN.ireboundbrute: //怒缚蛮兵
                    case CardDB.cardNameEN.everythingmustgo: //一件不留
                        retval = retval + offset - p.owncarddraw;
                        break;
                    default:
                        retval = retval + offset;
                        break;
                }

                if (this.Secret && (p.anzOwnCloakedHuntress > 0 || p.nextSecretThisTurnCost0))
                {
                    retval = 0;
                }

                retval = Math.Max(0, retval);

                return retval;
            }

            /// <summary>
            /// 能否打出牌
            /// </summary>
            /// <param name="p"></param>
            /// <param name="manacost"></param>
            /// <param name="own"></param>
            /// <returns></returns>
            public bool canplayCard(Playfield p, int manacost, bool own)
            {
                if (p.mana < this.getManaCost(p, manacost)) return false;
                if (this.getTargetsForCard(p, false, own).Count == 0) return false;
                return true;
            }

            /// <summary>
            /// 打印中文信息用，中文名 + 身材，方便辨识
            /// </summary>
            /// <returns></returns>
            public string chnInfo()
            {
                if (type == cardtype.MOB) //随从
                    return nameCN.ToString() + "(" + Attack + "," + Health + ")";
                else
                    return nameCN.ToString();
            }

            /// <summary>
            /// 获取泰坦卡牌的技能列表
            /// </summary>
            public List<CardDB.Card> GetTitanAbility()
            {
                var titanSkills = new List<CardDB.Card>();

                switch (this.cardIDenum.ToString())
                {
                    case "TTN_092":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_092t1)); // 守护秩序
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_092t2)); // 统帅风范
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_092t3)); // 迅疾挥剑
                        break;
                    case "TTN_075":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_075t)); // 元尊之力
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_075t2)); // 远古知识
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_075t3)); // 无限潜能
                        break;
                    case "TTN_800":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_800t2)); // 天空之王
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_800t)); // 狂海怒涛
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_800t3)); // 沙加恩之怒
                        break;
                    case "TTN_415":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_415t)); // 泰坦锻造
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_415t2)); // 升温回火
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_415t3)); // 烈焰之心
                        break;
                    case "TTN_858":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_858t1)); // 增援
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_858t2)); // 强化
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_858t3)); // 平静
                        break;
                    case "TTN_429":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_429t)); // 塑造星辰
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_429t2)); // 扫出历史
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_429t3)); // 英雄视界
                        break;
                    case "TTN_862":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_862t1)); // 雕琢晶石
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_862t2)); // 力量展现
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_862t3)); // 阿古尼特大军
                        break;
                    case "TTN_737":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_737t)); // 鲜血符文
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_737t3)); // 冰霜符文
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_737t1)); // 邪恶符文
                        break;
                    case "TTN_960":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_960t2)); // 打入虚空！
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_960t3)); // 地狱火！
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_960t4)); // 军团进攻！
                        break;
                    case "YOG_516":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.YOG_516t)); // 混沌统治
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.YOG_516t2)); // 诱引狂乱
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.YOG_516t3)); // 触须攒聚
                        break;
                    case "TTN_903":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_903t)); // 自行生长
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_903t2)); // 丰饶收获
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_903t3)); // 繁茂似锦
                        break;
                    case "TTN_721":
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_721t)); // 加装火炮！
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_721t1)); // 动力全开！
                        titanSkills.Add(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TTN_721t2)); // 防御拉满！
                        break;
                }

                return titanSkills;
            }
        }

        public List<Card> cardlist = new List<Card>();
        Dictionary<cardIDEnum, Card> cardidToCardList = new Dictionary<cardIDEnum, Card>();
        Dictionary<string, Card> carddbfidToCardList = new Dictionary<string, Card>();
        Dictionary<cardNameCN, Card> cardNameCNToCardList = new Dictionary<cardNameCN, Card>();
        Dictionary<cardNameEN, Card> cardNameENToCardList = new Dictionary<cardNameEN, Card>();


        public Card unknownCard;
        public bool installedWrong = false;

        public Card burlyrockjaw;
        private static CardDB instance;

        //定义三个稀有度的宝藏卡牌池
        public Dictionary<string, List<cardIDEnum>> treasurePools;

        public static CardDB Instance
        {
            get
            {
                if (instance == null)
                {
                    Helpfunctions.Instance.ErrorLog("开始加载CardDB");
                    var dt = DateTime.Now;
                    instance = new CardDB();
                    //instance.enumCreator();// only call this to get latest cardids
                    // 处理「套牌规则视为同一卡牌」的替换逻辑
                    foreach (Card c in instance.cardlist)
                    {
                        if (!string.IsNullOrWhiteSpace(c.TreatItAsTheSameCard))
                        {
                            Card targetCard = instance.getCardDataFromDbfID(c.TreatItAsTheSameCard);
                            if (targetCard != instance.unknownCard)
                                c.sim_card = targetCard.sim_card;   // 替换为关联卡牌的模拟逻辑
                        }
                    }  

                    // 统一所有幸运币和技能卡牌的模拟模板
                    var coinSim = GetCardSimulation(CardDB.cardIDEnum.GAME_005);    // 幸运币
                    var HERO_01bp = GetCardSimulation(CardDB.cardIDEnum.HERO_01bp); // 战士
                    var HERO_02bp = GetCardSimulation(CardDB.cardIDEnum.HERO_02bp); // 萨满
                    var HERO_03bp = GetCardSimulation(CardDB.cardIDEnum.HERO_03bp); // 潜行者
                    var HERO_04bp = GetCardSimulation(CardDB.cardIDEnum.HERO_04bp); // 圣骑士
                    var HERO_05bp = GetCardSimulation(CardDB.cardIDEnum.HERO_05bp); // 猎人
                    var HERO_06bp = GetCardSimulation(CardDB.cardIDEnum.HERO_06bp); // 德鲁伊
                    var HERO_07bp = GetCardSimulation(CardDB.cardIDEnum.HERO_07bp); // 术士
                    var HERO_08bp = GetCardSimulation(CardDB.cardIDEnum.HERO_08bp); // 法师
                    var HERO_09bp = GetCardSimulation(CardDB.cardIDEnum.HERO_09bp); // 牧师
                    var HERO_10bp = GetCardSimulation(CardDB.cardIDEnum.HERO_10bp); // 恶魔猎手
                    var HERO_11bp = GetCardSimulation(CardDB.cardIDEnum.HERO_11bp); // 死亡骑士
                    // 升级后的技能
                    var HERO_01bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_01bp2); // 战士
                    var HERO_02bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_02bp2); // 萨满
                    var HERO_03bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_03bp2); // 潜行者
                    var HERO_04bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_04bp2); // 圣骑士
                    var HERO_05bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_05bp2); // 猎人
                    var HERO_06bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_06bp2); // 德鲁伊
                    var HERO_07bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_07bp2); // 术士
                    var HERO_08bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_08bp2); // 法师
                    var HERO_09bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_09bp2); // 牧师
                    var HERO_10bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_10bp2); // 恶魔猎手
                    var HERO_11bp2 = GetCardSimulation(CardDB.cardIDEnum.HERO_11bp2); // 死亡骑士
                    //foreach (Card c in instance.cardlist)
                    //{
                    //    // 幸运币
                    //    if (c.coincard && c.cardIDenum != CardDB.cardIDEnum.GAME_005)
                    //    {
                    //        c.sim_card = coinSim;
                    //    }
                    //    // 战士技能 - 全副武装
                    //    if (c.nameCN == CardDB.cardNameCN.全副武装 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_01bp)
                    //    {
                    //        c.sim_card = HERO_01bp;
                    //    }
                    //    // 萨满技能 - 图腾召唤
                    //    if (c.nameCN == CardDB.cardNameCN.图腾召唤 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_02bp)
                    //    {
                    //        c.sim_card = HERO_02bp;
                    //    }
                    //    // 潜行者技能 - 匕首精通
                    //    if (c.nameCN == CardDB.cardNameCN.匕首精通 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_03bp)
                    //    {
                    //        c.sim_card = HERO_03bp;
                    //    }
                    //    // 圣骑士技能 - 援军
                    //    if (c.nameCN == CardDB.cardNameCN.援军 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_04bp)
                    //    {
                    //        c.sim_card = HERO_04bp;
                    //    }
                    //    // 猎人技能 - 稳固射击
                    //    if (c.nameCN == CardDB.cardNameCN.稳固射击 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_05bp)
                    //    {
                    //        c.sim_card = HERO_05bp;
                    //    }
                    //    // 德鲁伊技能 - 变形
                    //    if (c.nameCN == CardDB.cardNameCN.变形 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_06bp)
                    //    {
                    //        c.sim_card = HERO_06bp;
                    //    }
                    //    // 术士技能 - 生命分流
                    //    if (c.nameCN == CardDB.cardNameCN.生命分流 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_07bp)
                    //    {
                    //        c.sim_card = HERO_07bp;
                    //    }
                    //    // 法师技能 - 火焰冲击
                    //    if (c.nameCN == CardDB.cardNameCN.火焰冲击 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_08bp)
                    //    {
                    //        c.sim_card = HERO_08bp;
                    //    }
                    //    // 牧师技能 - 次级治疗术
                    //    if (c.nameCN == CardDB.cardNameCN.次级治疗术 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_09bp)
                    //    {
                    //        c.sim_card = HERO_09bp;
                    //    }
                    //    // 恶魔猎手技能 - 恶魔之爪
                    //    if (c.nameCN == CardDB.cardNameCN.恶魔之爪 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_10bp)
                    //    {
                    //        c.sim_card = HERO_10bp;
                    //    }
                    //    // 死亡骑士技能 - 食尸鬼冲锋
                    //    if (c.nameCN == CardDB.cardNameCN.食尸鬼冲锋 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_11bp)
                    //    {
                    //        c.sim_card = HERO_11bp;
                    //    }

                    //    // 战士技能 - 坚壁
                    //    if (c.nameCN == CardDB.cardNameCN.坚壁 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_01bp2)
                    //    {
                    //        c.sim_card = HERO_01bp2;
                    //    }
                    //    // 萨满技能 - 图腾崇拜
                    //    if (c.nameCN == CardDB.cardNameCN.图腾崇拜 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_02bp2)
                    //    {
                    //        c.sim_card = HERO_02bp2;
                    //    }
                    //    // 潜行者技能 - 浸毒匕首
                    //    if (c.nameCN == CardDB.cardNameCN.浸毒匕首 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_03bp2)
                    //    {
                    //        c.sim_card = HERO_03bp2;
                    //    }
                    //    // 圣骑士技能 - 白银之手
                    //    if (c.nameCN == CardDB.cardNameCN.白银之手 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_04bp2)
                    //    {
                    //        c.sim_card = HERO_04bp2;
                    //    }
                    //    // 猎人技能 - 弩炮射击
                    //    if (c.nameCN == CardDB.cardNameCN.弩炮射击 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_05bp2)
                    //    {
                    //        c.sim_card = HERO_05bp2;
                    //    }
                    //    // 德鲁伊技能 - 恐怖变形
                    //    if (c.nameCN == CardDB.cardNameCN.恐怖变形 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_06bp2)
                    //    {
                    //        c.sim_card = HERO_06bp2;
                    //    }
                    //    // 术士技能 - 灵魂分流
                    //    if (c.nameCN == CardDB.cardNameCN.灵魂分流 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_07bp2)
                    //    {
                    //        c.sim_card = HERO_07bp2;
                    //    }
                    //    // 法师技能 - 二级火焰冲击
                    //    if (c.nameCN == CardDB.cardNameCN.二级火焰冲击 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_08bp2)
                    //    {
                    //        c.sim_card = HERO_08bp2;
                    //    }
                    //    // 牧师技能 - 治疗术
                    //    if (c.nameCN == CardDB.cardNameCN.治疗术 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_09bp2)
                    //    {
                    //        c.sim_card = HERO_09bp2;
                    //    }
                    //    // 恶魔猎手技能 - 恶魔之咬
                    //    if (c.nameCN == CardDB.cardNameCN.恶魔之咬 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_10bp2)
                    //    {
                    //        c.sim_card = HERO_10bp2;
                    //    }
                    //    // 死亡骑士技能 - 食尸鬼狂暴
                    //    if (c.nameCN == CardDB.cardNameCN.食尸鬼狂暴 && c.type == CardDB.cardtype.HEROPWR && c.cardIDenum != CardDB.cardIDEnum.HERO_11bp2)
                    //    {
                    //        c.sim_card = HERO_11bp2;
                    //    }
                    //}


                    Helpfunctions.Instance.ErrorLog("加载完毕,总计用时: " + (DateTime.Now - dt).TotalSeconds + " s");
                    Log.Debug($"加载完毕,总计用时: {(DateTime.Now - dt).TotalSeconds} s");
                    instance.setAdditionalData();
                }
                return instance;
            }
        }
        //解析 carddefs.xml的函数
        CardDB()
        {
            this.cardlist.Clear();
            this.cardidToCardList.Clear();
            this.carddbfidToCardList.Clear();
            this.cardNameCNToCardList.Clear();
            this.cardNameENToCardList.Clear();

            treasurePools = new Dictionary<string, List<cardIDEnum>>
            {
                { "common", new List<cardIDEnum> {
                    cardIDEnum.WW_001t4,
                    cardIDEnum.WW_001t,
                    cardIDEnum.WW_001t2,
                    cardIDEnum.WW_001t18,
                    cardIDEnum.WW_001t3,
                    cardIDEnum.DEEP_999t1,
                }},
                { "rare", new List<cardIDEnum> {
                    cardIDEnum.WW_001t16,
                    cardIDEnum.WW_001t7,
                    cardIDEnum.WW_001t8,
                    cardIDEnum.WW_001t5,
                    cardIDEnum.WW_001t9,
                    cardIDEnum.DEEP_999t2,
                }},
                { "epic", new List<cardIDEnum> {
                    cardIDEnum.WW_001t11,
                    cardIDEnum.WW_001t17,
                    cardIDEnum.WW_001t13,
                    cardIDEnum.WW_001t12,
                    cardIDEnum.WW_001t14,
                    cardIDEnum.DEEP_999t3,
                }},
            };

            //placeholdercard
            this.cardlist.Add(new Card { nameEN = cardNameEN.unknown, cost = 10 });
            this.unknownCard = cardlist[0];

            //尝试加载CardDefs.bin二进制缓存提速(避免每次启动解析35MB XML)
            string xmlPath = Path.Combine(Settings.Instance.path, "CardDefs.xml");
            string binPath = Path.Combine(Settings.Instance.path, "CardDefs.bin");
            bool loadedFromCache = false;

            if (File.Exists(binPath))
            {
                try
                {
                    DateTime binTime = File.GetLastWriteTimeUtc(binPath);
                    DateTime xmlTime = File.Exists(xmlPath) ? File.GetLastWriteTimeUtc(xmlPath) : DateTime.MinValue;
                    if (binTime >= xmlTime)
                    {
                        List<CardDB.Card> loadedCards;
                        Dictionary<CardDB.cardIDEnum, CardDB.Card> loadedCardIdMap;
                        Dictionary<string, CardDB.Card> loadedDbfIdMap;
                        Dictionary<CardDB.cardNameCN, CardDB.Card> loadedNameCNMap;
                        Dictionary<CardDB.cardNameEN, CardDB.Card> loadedNameENMap;

                        if (CardDefsCache.TryLoad(binPath, out loadedCards, out loadedCardIdMap, out loadedDbfIdMap, out loadedNameCNMap, out loadedNameENMap))
                        {
                            this.cardlist = loadedCards;
                            this.cardidToCardList = loadedCardIdMap;
                            this.carddbfidToCardList = loadedDbfIdMap;
                            this.cardNameCNToCardList = loadedNameCNMap;
                            this.cardNameENToCardList = loadedNameENMap;
                            this.unknownCard = this.cardlist[0];
                            loadedFromCache = true;
                        }
                    }
                }
                catch { }
            }

            if (!loadedFromCache)
            {
                var filePath = Path.Combine(Settings.Instance.path, "CardDefs.xml");
                if (!File.Exists(filePath))
                {
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("在" + Settings.Instance.path + "下找不到 CardDefs.xml!");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    return;
                }
                var reNameEN = new Regex("[a-zA-Z0-9]");
                var reNameCN = new Regex("[a-zA-Z0-9]|[\\u4e00-\\u9fa5]");

                //优化: 纯 XmlReader 流式解析，零中间对象分配
                //Tag 属性直接从 Reader 读取，仅 184/185 使用 ReadSubtree
            using (var reader = System.Xml.XmlReader.Create(filePath))
            {
                while (reader.ReadToFollowing("Entity"))
                {
                    var cardId = reader.GetAttribute("CardID");
                    var card = new Card();
                    card.dbfId = reader.GetAttribute("ID");
                    card.cardIDenum = this.cardIdstringToEnum(cardId);
                    if (cardId.EndsWith("t") ||
                        cardId.Equals("ds1_whelptoken") ||
                        cardId.Equals("CS2_mirror") ||
                        cardId.Equals("CS2_050") ||
                        cardId.Equals("CS2_052") ||
                        cardId.Equals("CS2_051") ||
                        cardId.Equals("NEW1_009") ||
                        cardId.Equals("CS2_152") ||
                        cardId.Equals("CS2_boar") ||
                        cardId.Equals("EX1_tk11") ||
                        cardId.Equals("EX1_506a") ||
                        cardId.Equals("skele21") ||
                        cardId.Equals("EX1_tk9") ||
                        cardId.Equals("EX1_finkle") ||
                        cardId.Equals("EX1_598") ||
                        cardId.Equals("EX1_tk34"))
                    {
                        card.isToken = true;
                    }

                    using (var subReader = reader.ReadSubtree())
                    {
                        while (subReader.Read() && !(subReader.NodeType == XmlNodeType.EndElement && subReader.LocalName == "Entity"))
                        {
                            if (subReader.NodeType != XmlNodeType.Element) continue;
                            if (subReader.LocalName == "Entity") continue;

                            if (subReader.LocalName == "ReferencedTag")
                            {
                                var refId = subReader.GetAttribute("enumID");
                                if (refId == "190") card.tank = true;
                                else if (refId == "1518") card.dormant = 1;
                                continue;
                            }
                            if (subReader.LocalName != "Tag") continue;

                            var enumId = subReader.GetAttribute("enumID");
                            if (enumId == null) continue;

                            //复杂标签(含子元素): 184=textCN, 185=nameEN/nameCN
                            if (enumId == "184")
                            {
                                using (var tagReader = subReader.ReadSubtree())
                                {
                                    while (tagReader.Read())
                                        if (tagReader.NodeType == XmlNodeType.Element && tagReader.LocalName == "zhCN")
                                            card.textCN = tagReader.ReadElementContentAsString();
                                }
                                continue;
                            }
                            if (enumId == "185")
                            {
                                using (var tagReader = subReader.ReadSubtree())
                                {
                                    while (tagReader.Read())
                                    {
                                        if (tagReader.NodeType != XmlNodeType.Element) continue;
                                        if (tagReader.LocalName == "enUS")
                                        {
                                            var enText = tagReader.ReadElementContentAsString();
                                            var nameEN = "";
                                            foreach (Match m in reNameEN.Matches(enText))
                                                if (m.Success) nameEN += m.Value;
                                            if (nameEN == "continue" || nameEN == "protected") nameEN = "@" + nameEN;
                                            if (nameEN.Length > 0 && char.IsDigit(nameEN[0])) nameEN = "_" + nameEN;
                                            nameEN = nameEN.ToLower();
                                            card.nameEN = this.cardNameENstringToEnum(nameEN);
                                        }
                                        else if (tagReader.LocalName == "zhCN")
                                        {
                                            var cnText = tagReader.ReadElementContentAsString();
                                            var nameCN = "";
                                            foreach (Match m in reNameCN.Matches(cnText))
                                                if (m.Success) nameCN += m.Value;
                                            if (nameCN == "continue" || nameCN == "protected") nameCN = "@" + nameCN;
                                            if (nameCN.Length > 0 && char.IsDigit(nameCN[0])) nameCN = "_" + nameCN;
                                            card.nameCN = this.cardNameCNstringToEnum(nameCN);
                                        }
                                    }
                                }
                                continue;
                            }

                            //简单标签: 直接从 Reader 读属性值
                            switch (enumId)
                            {
                                case "32": card.TriggerVisual = int.Parse(subReader.GetAttribute("value")); break;
                                case "643": card.Nature = true; break;
                                case "321": card.Collectable = true; break;
                                case "227": card.CantAttack = true; break;
                                case "45": card.Health = int.Parse(subReader.GetAttribute("value")); break;
                                case "199": card.Class = int.Parse(subReader.GetAttribute("value")); break;
                                case "47": card.Attack = int.Parse(subReader.GetAttribute("value")); break;
                                case "200":
                                    card.race = (Race)int.Parse(subReader.GetAttribute("value"));
                                    card.races.Add((Race)int.Parse(subReader.GetAttribute("value")));
                                    break;
                                case "203": card.rarity = int.Parse(subReader.GetAttribute("value")); break;
                                case "48": card.cost = int.Parse(subReader.GetAttribute("value")); break;
                                case "202": card.type = (cardtype)int.Parse(subReader.GetAttribute("value")); break;
                                case "443": card.choice = true; break;
                                case "363": card.poisonous = true; break;
                                case "212": card.Enrage = true; break;
                                case "338": card.oneTurnEffect = true; break;
                                case "362": card.Aura = true; break;
                                case "190": break; // tank handled via ReferencedTag above
                                case "218": card.battlecry = true; break;
                                case "415": card.discover = true; break;
                                case "189": card.windfury = true; break;
                                case "217": card.deathrattle = true; break;
                                case "403": card.Inspire = true; break;
                                case "187": card.Durability = int.Parse(subReader.GetAttribute("value")); break;
                                case "114": card.Elite = true; break;
                                case "220": card.Combo = true; break;
                                case "1637": card.Frenzy = true; break;
                                case "1920": card.HonorableKill = true; break;
                                case "684": card.HideCost = true; break;
                                case "936": card.ShiftingSpell = true; break;
                                case "923": card.Overkill = true; break;
                                case "215": card.overload = int.Parse(subReader.GetAttribute("value")); break;
                                case "685": card.lifesteal = true; break;
                                case "448": card.untouchable = true; break;
                                case "191": card.Stealth = true; break;
                                case "219": card.Secret = true; break;
                                case "462": card.Quest = true; break;
                                case "1725": card.Questline = true; break;
                                case "208": card.Freeze = true; break;
                                case "350": card.AdjacentBuff = true; break;
                                case "194": card.Shield = true; break;
                                case "197": card.Charge = true; break;
                                case "339": card.Silence = true; break;
                                case "293": card.Morph = true; break;
                                case "192":
                                    card.Spellpower = true;
                                    card.spellpowervalue = int.Parse(subReader.GetAttribute("value"));
                                    break;
                                case "791": card.Rush = true; break;
                                case "1085": card.reborn = true; break;
                                case "1427": card.Spellburst = true; break;
                                case "1551": card.Corrupted = true; break;
                                case "1524": card.Corrupt = true; break;
                                case "1635": card.SpellSchool = (SpellSchool)int.Parse(subReader.GetAttribute("value")); break;
                                case "1518": break; // dormant handled via ReferencedTag above
                                case "1333": card.Outcast = true; break;
                                case "1720": card.Tradeable = true; break;
                                case "1743": card.DECK_ACTION_COST = int.Parse(subReader.GetAttribute("value")); break;
                                case "2": card.TAG_SCRIPT_DATA_NUM_1 = int.Parse(subReader.GetAttribute("value")); break;
                                case "3": card.TAG_SCRIPT_DATA_NUM_2 = int.Parse(subReader.GetAttribute("value")); break;
                                case "2889": card.TAG_SCRIPT_DATA_NUM_3 = int.Parse(subReader.GetAttribute("value")); break;
                                case "2919": card.TAG_SCRIPT_DATA_NUM_4 = int.Parse(subReader.GetAttribute("value")); break;
                                case "2332": card.Dredge = true; break;
                                case "2456":
                                    if (cardId != "MAW_031") card.Infuse = true;
                                    break;
                                case "2457": card.Infused = true; break;
                                case "2498": card.Manathirst = int.Parse(subReader.GetAttribute("value")); break;
                                case "2820": card.Finale = true; break;
                                case "2821": card.Overheal = true; break; // ReferencedTag filtered above
                                case "2772": card.Titan = true; break;    // ReferencedTag filtered above
                                case "2785": card.Forge = true; break;
                                case "3011": card.Forged = true; break;
                                case "2905": card.Quickdraw = true; break;
                                case "3114": card.Excavate = true; break;
                                case "1211": card.Elusive = true; break;
                                case "846": card.Echo = true; break;
                                case "1114": card.nonKeywordEcho = true; break;
                                case "1193": card.Twinspell = true; break;
                                case "3630": card.Temporary = true; break;
                                case "373": card.immuneWhileAttacking = true; break;
                                case "380": card.heroPower = int.Parse(subReader.GetAttribute("value")); break;
                                case "292": card.armor = int.Parse(subReader.GetAttribute("value")); break;
                                case "3382": card.KeepHeroClass = int.Parse(subReader.GetAttribute("value")); break;
                                case "1452": card.CollectionRelatedCardDataBaseId = int.Parse(subReader.GetAttribute("value")); break;
                                case "858": card.TreatItAsTheSameCard = subReader.GetAttribute("name"); break;
                                case "2837": card.CollectionRelatedCardDataBaseId = int.Parse(subReader.GetAttribute("value")); break;
                                case "3318": card.Miniaturize = int.Parse(subReader.GetAttribute("value")); break;
                                case "3399": card.Gigantity = int.Parse(subReader.GetAttribute("value")); break;
                                case "1077": card.CastsWhenDrawn = int.Parse(subReader.GetAttribute("value")); break;
                                case "1749": card.Objective = int.Parse(subReader.GetAttribute("value")); break;
                                case "2311": card.ObjectiveAura = int.Parse(subReader.GetAttribute("value")); break;
                                case "2329": card.Sigil = int.Parse(subReader.GetAttribute("value")); break;
                                case "2196": card.costBlood = int.Parse(subReader.GetAttribute("value")); break;
                                case "2197": card.costFrost = int.Parse(subReader.GetAttribute("value")); break;
                                case "2198": card.costUnholy = int.Parse(subReader.GetAttribute("value")); break;
                                case "1086": card.upgradedHeroPower = int.Parse(subReader.GetAttribute("value")); break;
                                case "994": card.markOfEvil = true; break;
                                case "2831": card.Treant = true; break;
                                case "2871": card.Ancient = true; break;
                                case "1965": card.IMP = true; break;
                                case "2355": card.Whelp = true; break;
                                case "3555": card.Starship = true; break;
                                case "3568": card.StarshipPiece = true; break;
                                case "3631": card.Crewmate = true; break;
                                case "1678": card.SI_7 = true; break;
                                case "3444": card.SilverHandRecruit = true; break;
                                case "3928": card.Rafaam = true; break;
                                case "4392": card.Ysera = true; break;
                                case "476": card.MultipleClasses = int.Parse(subReader.GetAttribute("value")); break;
                                case "3457": card.Zerg = true; break;
                                case "3458": card.Terran = true; break;
                                case "3469": card.Protoss = true; break;
                                case "3881": card.Wipe = true; break;
                                case "2525": card.races.Add(Race.DRAENEI); break;
                                case "2534": card.races.Add(Race.UNDEAD); break;
                                case "2536": card.races.Add(Race.MURLOC); break;
                                case "2537": card.races.Add(Race.DEMON); break;
                                case "2539": card.races.Add(Race.MECHANICAL); break;
                                case "2540": card.races.Add(Race.ELEMENTAL); break;
                                case "2542": card.races.Add(Race.PET); break;
                                case "2543": card.races.Add(Race.TOTEM); break;
                                case "2522": card.races.Add(Race.PIRATE); break;
                                case "2523": card.races.Add(Race.DRAGON); break;
                                case "2546": card.races.Add(Race.QUILBOAR); break;
                                case "2553": card.races.Add(Race.NAGA); break;
                                case "4090": card.InteractableObjectCost = int.Parse(subReader.GetAttribute("value")); break;
                                case "1508": card.CanTargetCardsInHand = true; break;
                                case "4089": card.InteractableObject = true; break;
                                case "4257": card.UsesCharges = int.Parse(subReader.GetAttribute("value")); break;
                                case "2859": card.magneticToRace = int.Parse(subReader.GetAttribute("value")); break;
                            }
                        }
                    }
                cardlist.Add(card);
                if (!string.IsNullOrEmpty(card.dbfId))
                    carddbfidToCardList[card.dbfId] = card;
                if (card.cardIDenum != cardIDEnum.None)
                    cardidToCardList[card.cardIDenum] = card;
                if (card.nameCN != cardNameCN.未知)
                {
                    cardNameCNToCardList[card.nameCN] = card;
                }
                if (card.nameEN != cardNameEN.unknown)
                {
                    cardNameENToCardList[card.nameEN] = card;
                }
                } // end while
            } // end using (XmlReader)
            } // if (!loadedFromCache)

            //处理DECK_ACTION_COST、TAG_SCRIPT_DATA_NUM_1、TAG_SCRIPT_DATA_NUM_2等属性
            foreach (Card item in cardlist)
            {
                if (item.Infuse)
                {
                    item.InfuseNum = item.TAG_SCRIPT_DATA_NUM_1;
                }
                if (item.Tradeable)
                {
                    item.TradeCost = item.DECK_ACTION_COST;
                }
                if (item.Forge)
                {
                    item.ForgeCost = item.DECK_ACTION_COST;
                }
                if (!string.IsNullOrWhiteSpace(item.TreatItAsTheSameCard))
                {
                    Card OriginCard = this.getCardDataFromDbfID(item.TreatItAsTheSameCard);
                    if (OriginCard != this.unknownCard)
                        item.sim_card = OriginCard.sim_card;
                }
            }

            //在POST-PROCESSING之后保存二进制缓存，确保InfuseNum/TradeCost/ForgeCost等字段已正确设置
            Log.Info("CardDB: loadedFromCache=" + loadedFromCache + " cardlist.Count=" + cardlist.Count + " binPath=" + binPath);
            if (!loadedFromCache)
            {
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    CardDefsCache.Save(binPath, this.cardlist, this.cardidToCardList, this.carddbfidToCardList, this.cardNameCNToCardList, this.cardNameENToCardList);
                    sw.Stop();
                    Log.Info("CardDefsCache.Save DONE (" + sw.Elapsed.TotalSeconds.ToString("0.0") + "s) -> " + binPath);
                }
                catch (Exception ex)
                {
                    Log.Warn("CardDefsCache.Save FAILED: " + ex.Message);
                }
            }
            else
            {
                Log.Info("CardDB: loaded from cache, skipping save");
            }
        }

        /// <summary>
        /// 根据CardDB.cardNameEN卡名获取卡
        /// </summary>
        /// <param name="cardname"></param>
        /// <returns>CardDB.Card类对象</returns>
        public CardDB.Card getCardData(CardDB.cardNameEN cardname)
        {
            CardDB.Card c;
            if (this.cardNameENToCardList.TryGetValue(cardname, out c))
                return c;
            return this.unknownCard;
        }
        /// <summary>
        /// 根据CardDB.cardNameCN卡名获取卡
        /// </summary>
        /// <param name="cardname"></param>
        /// <returns>CardDB.Card类对象</returns>
        public CardDB.Card getCardData(CardDB.cardNameCN cardname)
        {
            CardDB.Card c;
            if (this.cardNameCNToCardList.TryGetValue(cardname, out c))
                return c;
            return this.unknownCard;
        }


        /// <summary>
        /// CardDB.cardIDEnum id获取卡
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns>CardDB.Card类对象</returns>
        public CardDB.Card getCardDataFromID(CardDB.cardIDEnum cardId)
        {
            CardDB.Card c;
            if (this.cardidToCardList.TryGetValue(cardId, out c))
                return c;
            return this.unknownCard;
        }

        /// <summary>
        /// 根据dbfID获取卡
        /// </summary>
        /// <param name="dbfID"></param>
        /// <returns>CardDB.Card类对象</returns>
        public CardDB.Card getCardDataFromDbfID(String dbfID)
        {
            CardDB.Card c;
            if (this.carddbfidToCardList.TryGetValue(dbfID, out c))
                return c;
            return this.unknownCard;
        }

        private void setAdditionalData()
        {
            PenalityManager pen = PenalityManager.Instance;

            foreach (Card c in this.cardlist)
            {
                if (c.cardIDenum == cardIDEnum.None)
                    continue;                             //Todo: 为了确保Test能跑通

                if (pen.cardDrawBattleCryDatabase.ContainsKey(c.nameEN))
                {
                    c.isCarddraw = pen.cardDrawBattleCryDatabase[c.nameEN];
                }

                if (pen.DamageTargetSpecialDatabase.ContainsKey(c.nameEN))
                {
                    c.damagesTargetWithSpecial = true;
                }

                if (pen.DamageTargetDatabase.ContainsKey(c.nameEN))
                {
                    c.damagesTarget = true;
                }

                if (pen.priorityTargets.ContainsKey(c.nameEN))
                {
                    c.targetPriority = pen.priorityTargets[c.nameEN];
                }

                if (pen.specialMinions.ContainsKey(c.nameEN))
                {
                    c.isSpecialMinion = true;
                }


                c.trigers = new List<cardtrigers>();
                // 只有拥有自定义 Sim_XXX 类的卡牌才需要检查触发器类型
                // 默认 SimTemplate 有超过 20 个虚方法，其 trigers 会在后续被清空
                if (SimTypesDict.ContainsKey("Sim_" + c.cardIDenum.ToString()))
                {
                    Type trigerType = c.sim_card.GetType();
                    foreach (string trigerName in Enum.GetNames(typeof(cardtrigers)))
                    {
                        try
                        {
                            foreach (var m in trigerType.GetMethods().Where(e => e.Name.Equals(trigerName, StringComparison.Ordinal)))
                            {
                                if (m.DeclaringType == trigerType)
                                    c.trigers.Add((cardtrigers)Enum.Parse(typeof(cardtrigers), trigerName));
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (c.trigers.Count > 20) c.trigers.Clear();
                }
            }
        }
    }

    /// <summary>
    /// 目标
    /// </summary>
    public struct targett
    {
        public int target;//目标
        public int targetEntity;//目标实体

        public targett(int targ, int ent)
        {
            this.target = targ;
            this.targetEntity = ent;
        }
    }

    /// <summary>
    /// 打出卡牌要求
    /// </summary>
    public struct PlayReq
    {
        public CardDB.ErrorType2 errorType;
        public int param;

        public PlayReq(CardDB.ErrorType2 errorType, int param)
        {
            this.errorType = errorType;
            this.param = param;
        }
        public PlayReq(CardDB.ErrorType2 errorType, CardDB.Race race)
        {
            this.errorType = errorType;
            this.param = (int)race;
        }
        public PlayReq(CardDB.ErrorType2 errorType, CardDB.Specialtags specialtags)
        {
            this.errorType = errorType;
            this.param = (int)specialtags;
        }

        public PlayReq(CardDB.ErrorType2 errorType)
        {
            this.errorType = errorType;
            this.param = -1;
        }
        public void UpdateCardAttr(CardDB.Card card)
        {
            switch (errorType)
            {
                case CardDB.ErrorType2.REQ_TARGET_MAX_ATTACK:
                    card.needWithMaxAttackValueOf = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_MIN_ATTACK:
                    card.needWithMinAttackValueOf = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_EXACT_ATTACK:
                    card.needWithExactAttackValueOf = param;
                    break;
                case CardDB.ErrorType2.REQ_MINIMUM_CORPSES:
                    card.needWithMinimumCorpeses = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_WITH_RACE:
                    card.needRaceForPlaying = param;
                    break;
                case CardDB.ErrorType2.REQ_FRIENDLY_MINION_OF_RACE_IN_HAND:
                    card.needRaceInHand = param;
                    break;
                case CardDB.ErrorType2.REQ_NUM_MINION_SLOTS:
                    card.needEmptyPlacesForPlaying = param;
                    break;
                case CardDB.ErrorType2.REQ_MINION_CAP_IF_TARGET_AVAILABLE:
                    card.needMinionsCapIfAvailable = param;
                    break;
                case CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS:
                    card.needMinNumberOfEnemy = param;
                    break;
                case CardDB.ErrorType2.REQ_MINIMUM_TOTAL_MINIONS:
                    card.needMinTotalMinions = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS:
                    card.needMinOwnMinions = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS:
                    card.needControlaSecret = param;
                    break;
                case CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG:
                    card.needTagForPlaying = (CardDB.Specialtags)param;
                    break;
            }
        }
    }
}