using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Buddy.Coroutines;
using Triton.Bot;
using Triton.Common;
using Triton.Game;
using Triton.Game.Mapping;
// using Logger = Triton.Common.LogUtilities.Logger;
// using log4net;
namespace HREngine.Bots
{
    public class Silverfish
    {
        // private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        public string versionnumber = "2025.08.08";
        private bool singleLog = false;
        private StringBuilder botbehave = new StringBuilder("noname", 20);
        private bool needSleep = false;

        public Playfield lastpf;
        private Settings sttngs = Settings.Instance;

        private List<Minion> ownMinions = new List<Minion>();
        private List<Minion> enemyMinions = new List<Minion>();
        private List<Handmanager.Handcard> handCards = new List<Handmanager.Handcard>();
        private List<string> ownSecretList = new List<string>();
        private Dictionary<int, TAG_CLASS> enemySecretList = new Dictionary<int, TAG_CLASS>();
        private Dictionary<int, IDEnumOwner> LurkersDB = new Dictionary<int, IDEnumOwner>();
        public Dictionary<string, Behavior> BehaviorDB = new Dictionary<string, Behavior>();
        public Dictionary<string, string> BehaviorPath = new Dictionary<string, string>();
        Dictionary<CardDB.cardIDEnum, int> startDeck = new Dictionary<CardDB.cardIDEnum, int>();
        Dictionary<CardDB.cardIDEnum, int> tmpDeck = new Dictionary<CardDB.cardIDEnum, int>();
        Dictionary<CardDB.cardIDEnum, int> turnDeck = new Dictionary<CardDB.cardIDEnum, int>();
        List<GraveYardItem> graveYard = new List<GraveYardItem>();
        private delegate List<HSCard> getAllCardsDele();
        static getAllCardsDele getAllCards;
        int ownController = 1;
        int enemyController = 2;
        private int currentMana = 0;
        private int ownMaxMana = 0;
        private int numOptionPlayedThisTurn = 0;
        private int numMinionsPlayedThisTurn = 0;
        private int cardsPlayedThisTurn = 0;
        private int ueberladung = 0;//过载
        private int lockedMana = 0;//锁上
        private int maxResources = 10;//法力水晶上限
        private int maxHandSize = 10;//手牌上限

        private int enemyMaxMana = 0;

        private string heroname = "";
        private string enemyHeroname = "";

        private CardDB.Card heroAbility = new CardDB.Card();
        private int ownHeroPowerCost = 2;//?
        private bool ownAbilityisReady = false;
        private CardDB.Card enemyAbility = new CardDB.Card();
        private int enemyHeroPowerCost = 2;

        private Weapon ownWeapon;
        private Weapon enemyWeapon;

        private int anzcards = 0;//卡牌数量
        private int enemyAnzCards = 0;

        private int ownHeroFatigue = 0;//疲劳
        private int enemyHeroFatigue = 0;
        private int ownDecksize = 0;//牌库数量
        private int enemyDecksize = 0;

        private Minion ownHero;
        private Minion enemyHero;

        private int gTurn = 0;
        private int gTurnStep = 0;
        private int anzOgOwnCThunHpBonus = 0;//克苏恩血量
        private int anzOgOwnCThunAngrBonus = 0;//克苏恩攻击
        private int anzOgOwnCThunTaunt = 0;//克苏恩嘲讽
        private static Silverfish instance;

        public static Silverfish Instance
        {
            get { return instance ?? (instance = new Silverfish()); }
        }

        private Silverfish()
        {
            //设置文件路径
            this.singleLog = Settings.Instance.writeToSingleFile;
            Helpfunctions.Instance.ErrorLog("开始启动...");
            string p = "." + System.IO.Path.DirectorySeparatorChar + "Routines" + System.IO.Path.DirectorySeparatorChar + "DefaultRoutine" +
                       System.IO.Path.DirectorySeparatorChar + "Silverfish" + System.IO.Path.DirectorySeparatorChar;
            string path = p + "UltimateLogs" + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(path);
            sttngs.setFilePath(p + "Data" + Path.DirectorySeparatorChar);
            if (!singleLog)
            {
                sttngs.setLoggPath(path);
            }
            else
            {
                sttngs.setLoggPath(p);
                sttngs.setLoggFile("UILogg.txt");
                Helpfunctions.Instance.createNewLoggfile();
            }
            setBehavior();
            getAllCards = Extensions.GetAllCards;

        }

        /// <summary>
        /// 设置行为
        /// </summary>
        /// <returns></returns>
        private bool setBehavior()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(Behavior)).ToArray();
            foreach (Type t in types)
            {
                string s = t.Name;
                if (s == "BehaviorMana") continue;
                if (s.Length > 8 && s.Substring(0, 8) == "Behavior")
                {
                    Behavior b = (Behavior)Activator.CreateInstance(t);
                    BehaviorDB.Add(b.BehaviorName(), b);
                }
            }

            string p = Path.Combine("Routines", "DefaultRoutine", "Silverfish", "behavior");//路径
            string[] files = Directory.GetFiles(p, "Behavior*.cs", SearchOption.AllDirectories);//获取文件
            int bCount = 0;
            foreach (string file in files)
            {
                bCount++;
                string bPath = Path.GetDirectoryName(file);//获取文件夹名字
                String fullPath = Path.GetFullPath(file);//获取完整路径
                String bNane = Path.GetFileNameWithoutExtension(file).Remove(0, 8);//获取除了Behavior之后的名字
                if (BehaviorDB.ContainsKey(bNane))
                {
                    if (!BehaviorPath.ContainsKey(bNane)) BehaviorPath.Add(bNane, bPath);//加入db
                }
            }

            if (BehaviorDB.Count != BehaviorPath.Count || BehaviorDB.Count != bCount)
            {
                Helpfunctions.Instance.ErrorLog("Behavior: 登记过 - " + BehaviorDB.Count + ", 文件夹 - " + bCount + ", 有路径 - "
                    + BehaviorPath.Count + ". 这些值应该相同. 或许你有其他文件在  'custom_behavior' 文件夹.");
            }

            if (BehaviorDB.ContainsKey("丨通用丨不设惩罚"))
            {//必须
                Ai.Instance.botBase = BehaviorDB["丨通用丨不设惩罚"];
                return true;
            }
            else
            {
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog("没有找到战场策略!");
                Helpfunctions.Instance.ErrorLog("请把战场策略放到指定的文件中.");
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                return false;
            }
        }
        public Behavior getBehaviorByName(string bName)
        {
            if (BehaviorDB.ContainsKey(bName))
            {
                sttngs.setSettings(bName);
                ComboBreaker.Instance.readCombos(bName);
                RulesEngine.Instance.readRules(bName);
                return BehaviorDB[bName];
            }
            else
            {
                if (BehaviorDB.ContainsKey("控场模式")) return BehaviorDB["控场模式"];//默认控场模式
                else
                {
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("没有找到战场策略!");
                    Helpfunctions.Instance.ErrorLog("请把战场策略放到指定的文件中.");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                    Helpfunctions.Instance.ErrorLog("ERROR#################################################");
                }
            }
            return null;
        }


        public void setnewLoggFile()
        {
            gTurn = 0;
            gTurnStep = 0;
            anzOgOwnCThunHpBonus = 0;
            anzOgOwnCThunAngrBonus = 0;
            anzOgOwnCThunTaunt = 0;
            Questmanager.Instance.Reset();
            if (!singleLog)
            {
                sttngs.setLoggFile("UILogg" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt");
                Helpfunctions.Instance.createNewLoggfile();
                Helpfunctions.Instance.ErrorLog("#######################################################");
                Helpfunctions.Instance.ErrorLog("对局日志保持在: " + sttngs.logpath + sttngs.logfile);
                Helpfunctions.Instance.ErrorLog("#######################################################");
            }
            else
            {
                sttngs.setLoggFile("UILogg.txt");
            }
        }

        internal void updateStartDeck()
        {
            //读取信息加入卡组
            startDeck.Clear();
            long DeckId = Triton.Bot.Logic.Bots.DefaultBot.DefaultBotSettings.Instance.LastDeckId;
            foreach (var deck in Triton.Bot.Settings.MainSettings.Instance.CustomDecks)
            {
                if (deck.DeckId == DeckId)
                {
                    foreach (string s in deck.CardIds)
                    {
                        CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(s);
                        if (startDeck.ContainsKey(id))
                        {
                            startDeck[id]++;
                        }
                        else startDeck.Add(id, 1);
                    }
                    break;
                }
            }
            //TODO:在这里进行套牌宇宙的判断
            /*             foreach (var kv in startDeck)
                        {

                            if (kv.Value > 0)
                            {
                                // 不是宇宙套牌
                                // value = false;
                                break;
                            }


                        } */
        }

        /// <summary>
        /// 更新所有信息
        /// </summary>
        /// <param name="botbase"></param>
        /// <param name="numcal"></param>
        /// <param name="sleepRetry"></param>
        /// <returns></returns>
        public bool updateEverything(Behavior botbase, int numcal, out bool sleepRetry)
        {
            this.needSleep = false;
            this.updateBehaveString(botbase);

            Hrtprozis.Instance.clearAllRecalc();
            Handmanager.Instance.clearAllRecalc();
            updateRealTimeInfo();//获取实时场面信息 

            Hrtprozis.Instance.updateTurnDeck(turnDeck);//对局信息更新卡组
            Hrtprozis.Instance.setOwnPlayer(ownController);//对局信息设置我方玩家                
            Handmanager.Instance.setOwnPlayer(ownController);//手牌管理 设置我方玩家

            this.numOptionPlayedThisTurn = 0;//本回合选项数量
            this.numOptionPlayedThisTurn += this.cardsPlayedThisTurn + this.ownHero.numAttacksThisTurn;//打出的卡片数量+攻击数量
            foreach (Minion m in this.ownMinions)
            {
                if (m.Hp >= 1) this.numOptionPlayedThisTurn += m.numAttacksThisTurn;//随从血量大于1 
            }

            Hrtprozis.Instance.updatePlayer(this.ownMaxMana, this.currentMana, this.cardsPlayedThisTurn,
                this.numMinionsPlayedThisTurn, this.numOptionPlayedThisTurn, this.ueberladung, this.lockedMana, TritonHs.OurHero.EntityId,
                TritonHs.EnemyHero.EntityId);//更新玩家
            Hrtprozis.Instance.updateSecretStuff(this.ownSecretList, this.enemySecretList.Count);//更新奥秘

            Hrtprozis.Instance.updateHero(this.ownWeapon, this.heroname, this.heroAbility, this.ownAbilityisReady, this.ownHeroPowerCost, this.ownHero);//更新我方英雄
            Hrtprozis.Instance.updateHero(this.enemyWeapon, this.enemyHeroname, this.enemyAbility, false, this.enemyHeroPowerCost, this.enemyHero, this.enemyMaxMana);//更新敌方英雄

            Questmanager.Instance.updatePlayedMobs(this.gTurnStep);//任务管理
            Hrtprozis.Instance.updateMinions(this.ownMinions, this.enemyMinions);//更新随从
            Hrtprozis.Instance.updateLurkersDB(this.LurkersDB);//潜伏者?
            Handmanager.Instance.setHandcards(this.handCards, this.anzcards, this.enemyAnzCards);//手牌
            Hrtprozis.Instance.updateFatigueStats(this.ownDecksize, this.ownHeroFatigue, this.enemyDecksize, this.enemyHeroFatigue);//疲劳
            Hrtprozis.Instance.updateJadeGolemsInfo(GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.JADE_GOLEM), GameState.Get().GetOpposingSidePlayer().GetTag(GAME_TAG.JADE_GOLEM));//青玉

            Hrtprozis.Instance.updateTurnInfo(this.gTurn, this.gTurnStep);//回合
            updateCThunInfobyCThun();//克苏恩
            Hrtprozis.Instance.updateCThunInfo(this.anzOgOwnCThunAngrBonus, this.anzOgOwnCThunHpBonus, this.anzOgOwnCThunTaunt);
            Probabilitymaker.Instance.setEnemySecretGuesses(this.enemySecretList);//猜测对手奥秘

            bool isTurnStart = false;
            if (Ai.Instance.nextMoveGuess.mana == -100)
            {
                isTurnStart = true;
                Ai.Instance.updateTwoTurnSim();
            }
            Probabilitymaker.Instance.setGraveYard(graveYard, isTurnStart);

            List<HSCard> list = TritonHs.GetCards(CardZone.Graveyard, true);//本回合选项数量增加
            foreach (GraveYardItem gi in Probabilitymaker.Instance.turngraveyard)//墓地
            {
                if (gi.own)
                {
                    foreach (HSCard entiti in list)
                    {
                        if (gi.entityId == entiti.EntityId)
                        {
                            this.numOptionPlayedThisTurn += entiti.NumAttackThisTurn;
                            break;
                        }
                    }
                }
            }

            sleepRetry = this.needSleep;
            if (sleepRetry && numcal == 0) return false;

            //不设置游戏规则
            if (!Hrtprozis.Instance.setGameRule)
            {
                RulesEngine.Instance.setCardIdRulesGame(this.ownHero.cardClass, this.enemyHero.cardClass);
                Hrtprozis.Instance.setGameRule = true;
            }

            Playfield p = new Playfield();
            p.enemyCardsOut = new Dictionary<CardDB.cardIDEnum, int>(Probabilitymaker.Instance.enemyGraveyard);//敌方出牌

            if (lastpf != null)
            {

                //TODO:最后的棋局和当前棋局相同
                if (lastpf.isEqualf(p))
                {
                    return false;
                }
                else
                {
                    if (p.gTurnStep > 0 && Ai.Instance.nextMoveGuess.ownMinions.Count == p.ownMinions.Count)
                    {
                        if (Ai.Instance.nextMoveGuess.ownHero.Ready != p.ownHero.Ready && !p.ownHero.Ready)
                        {
                            sleepRetry = true;
                            Helpfunctions.Instance.ErrorLog("[AI] 英雄的准备状态 = " + p.ownHero.Ready + ". 再次尝试....");
                            Ai.Instance.nextMoveGuess = new Playfield { mana = -100 };
                            return false;
                        }
                        for (int i = 0; i < p.ownMinions.Count; i++)
                        {
                            if (Ai.Instance.nextMoveGuess.ownMinions[i].Ready != p.ownMinions[i].Ready && !p.ownMinions[i].Ready)
                            {
                                sleepRetry = true;
                                Helpfunctions.Instance.ErrorLog("[AI] 随从的准备状态 = " + p.ownMinions[i].Ready + " (" + p.ownMinions[i].entitiyID + " " + p.ownMinions[i].handcard.card.cardIDenum + " " + p.ownMinions[i].name + "). 再次尝试....");
                                Ai.Instance.nextMoveGuess = new Playfield { mana = -100 };
                                return false;
                            }
                        }
                    }
                }

                Probabilitymaker.Instance.updateSecretList(p, lastpf);
                lastpf = p;//存储上一次场面信息
            }
            else
            {
                lastpf = p;
            }

            printUtils.printNowVal();

            Helpfunctions.Instance.ErrorLog("AI计算中，请稍候... " + DateTime.Now.ToString("HH:mm:ss.ffff"));


            using (TritonHs.Memory.ReleaseFrame(true))
            {
                printstuff();  //打印牌面细节到logg
                Ai.Instance.dosomethingclever(botbase);    //核心Ai计算
            }

            Helpfunctions.Instance.ErrorLog("计算完成! " + DateTime.Now.ToString("HH:mm:ss.ffff"));
            if (sttngs.printRules > 0)
            {
                String[] rulesStr = Ai.Instance.bestplay.rulesUsed.Split('@');
                if (rulesStr.Count() > 0 && rulesStr[0] != "")
                {
                    Helpfunctions.Instance.ErrorLog("规则权重 " + Ai.Instance.bestplay.ruleWeight * -1);
                    foreach (string rs in rulesStr)
                    {
                        if (rs == "") continue;
                        Helpfunctions.Instance.ErrorLog("规则: " + rs);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 实时更新游戏数据
        /// </summary>
        private void updateRealTimeInfo()
        {
            // List<HSCard> allcards = getAllCards();

            //turn
            int currTurn = TritonHs.GameState.GetTurn();
            if (gTurn == currTurn)
                gTurnStep++;
            else
                gTurnStep = 0;
            gTurn = currTurn;
            //hero
            // this.ownController = TritonHs.OurHero.ControllerId;
            // this.enemyController = TritonHs.EnemyHero.ControllerId;
            ownController = GameState.Get().GetFriendlyPlayerId();
            enemyController = GameState.Get().GetOpposingPlayerId();
            this.ownHero = new Minion();
            this.enemyHero = new Minion();
            this.currentMana = TritonHs.CurrentMana;
            this.ownMaxMana = TritonHs.Resources;
            // 后手
            if (gTurn % 2 == 0)
            {
                this.enemyMaxMana = ownMaxMana;
            }
            else
            {
                this.enemyMaxMana = ownMaxMana - 1;
            }
            //deck
            this.ownDecksize = 0;
            this.enemyDecksize = 0;
            this.turnDeck = new Dictionary<CardDB.cardIDEnum, int>();
            this.tmpDeck = new Dictionary<CardDB.cardIDEnum, int>(startDeck);
            this.graveYard.Clear();
            //secret
            ownSecretList.Clear();
            enemySecretList.Clear();
            //minion
            ownMinions.Clear();
            enemyMinions.Clear();
            LurkersDB.Clear();
            //hand
            handCards.Clear();
            anzcards = 0;
            enemyAnzCards = 0;

            // 初始化任务
            Questmanager.Instance.updateQuestStuff("None", 0, 1000, true);
            Questmanager.Instance.updateQuestStuff("None", 0, 1000, false);

            updateGame();
            foreach (var c in tmpDeck)
            {
                if (c.Value < 1) continue;
                if (c.Key == CardDB.cardIDEnum.None) continue;
                if (turnDeck.ContainsKey(c.Key))
                    turnDeck[c.Key] += c.Value;
                else
                    turnDeck.Add(c.Key, c.Value);
            }

            numMinionsPlayedThisTurn = TritonHs.NumMinionsPlayedThisTurn;
            cardsPlayedThisTurn = TritonHs.NumCardsPlayedThisTurn;
            ueberladung = TritonHs.OverloadOwed;
            lockedMana = TritonHs.OverloadLocked;
        }
        private void updateBehaveString(Behavior botbase)
        {
            botbehave = new StringBuilder(botbase.BehaviorName(), 100);
            botbehave.AppendFormat(" {0} maxCal {1} maxWide {2}", Ai.Instance.maxwide, Ai.Instance.maxCal, Ai.Instance.maxwide);
            botbehave.AppendFormat(" face {0}", ComboBreaker.Instance.attackFaceHP);
            if (Settings.Instance.berserkIfCanFinishNextTour > 0) botbehave.AppendFormat(" berserk:{0}", Settings.Instance.berserkIfCanFinishNextTour);
            if (Settings.Instance.weaponOnlyAttackMobsUntilEnfacehp > 0) botbehave.AppendFormat(" womob:{0}", Settings.Instance.weaponOnlyAttackMobsUntilEnfacehp);
            if (Settings.Instance.secondTurnAmount > 0)
            {
                if (Ai.Instance.nextMoveGuess.mana == -100)
                {
                    Ai.Instance.updateTwoTurnSim();
                }
                botbehave.AppendFormat(" twoturnsim {0} ntss {1} {2} {3}", Settings.Instance.secondTurnAmount, Settings.Instance.nextTurnDeep, Settings.Instance.nextTurnMaxWide, Settings.Instance.nextTurnTotalBoards);

            }

            if (Settings.Instance.playaround)
            {
                botbehave.AppendFormat(" playaround {0} {1}", Settings.Instance.playaroundprob, Settings.Instance.playaroundprob2);
            }
            botbehave.AppendFormat(" ets {0}", Settings.Instance.enemyTurnMaxWide);

            if (Settings.Instance.twotsamount > 0)
            {
                botbehave.AppendFormat(" ets2 {0}", Settings.Instance.enemyTurnMaxWideSecondStep);
            }

            if (Settings.Instance.useSecretsPlayAround)
            {
                botbehave.Append(" secret");
            }

            if (Settings.Instance.secondweight != 0.5f)
            {
                botbehave.AppendFormat(" weight {0}", (int)(Settings.Instance.secondweight * 100f));
            }

            if (Settings.Instance.placement != 0)
            {
                botbehave.AppendFormat(" plcmnt {0}", Settings.Instance.placement);
            }
            botbehave.AppendFormat(" aA {0}", Settings.Instance.adjustActions);

            if (Settings.Instance.concedeMode != 0) botbehave.AppendFormat(" cede:{0}", Settings.Instance.concedeMode);

        }

        public void updateCThunInfo(int OgOwnCThunAngrBonus, int OgOwnCThunHpBonus, int OgOwnCThunTaunt)
        {
            this.anzOgOwnCThunAngrBonus = OgOwnCThunAngrBonus;
            this.anzOgOwnCThunHpBonus = OgOwnCThunHpBonus;
            this.anzOgOwnCThunTaunt = OgOwnCThunTaunt;
            Hrtprozis.Instance.updateCThunInfo(this.anzOgOwnCThunAngrBonus, this.anzOgOwnCThunHpBonus, this.anzOgOwnCThunTaunt);
        }

        public void updateCThunInfobyCThun()
        {
            bool found = false;
            foreach (Handmanager.Handcard hc in this.handCards)
            {
                if (hc.card.nameEN == CardDB.cardNameEN.cthun)
                {
                    this.anzOgOwnCThunAngrBonus = hc.addattack;
                    this.anzOgOwnCThunHpBonus = hc.addHp;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                foreach (Minion m in this.ownMinions)
                {
                    if (m.name == CardDB.cardNameEN.cthun)
                    {
                        if (this.anzOgOwnCThunAngrBonus < m.Angr - 6) this.anzOgOwnCThunAngrBonus = m.Angr - 6;
                        if (this.anzOgOwnCThunHpBonus < m.Hp - 6) this.anzOgOwnCThunHpBonus = m.Angr - 6;
                        if (m.taunt && this.anzOgOwnCThunTaunt < 1) this.anzOgOwnCThunTaunt++;
                        found = true;
                        break;
                    }
                }
            }
        }

        //public static int getLastAffected(int entityid)
        //{

        //    List<HSCard> allEntitys = getAllCards();

        //    foreach (HSCard ent in allEntitys)
        //    {
        //        if (ent.GetTag(GAME_TAG.LAST_AFFECTED_BY) == entityid) return ent.GetTag(GAME_TAG.ENTITY_ID);
        //    }

        //    return 0;
        //}

        //public static int getCardTarget(int entityid)
        //{

        //    List<HSCard> allEntitys = getAllCards();

        //    foreach (HSCard ent in allEntitys)
        //    {
        //        if (ent.GetTag(GAME_TAG.ENTITY_ID) == entityid) return ent.GetTag(GAME_TAG.CARD_TARGET);
        //    }

        //    return 0;

        //}


        private void printstuff()  //会输出到文件夹UltimateLogs里面
        {
            Playfield p = this.lastpf;
            string dtimes = DateTime.Now.ToString("HH:mm:ss:ffff");
            // 记录对局信息
            printUtils.printRecord = true;
            printUtils.recordRounds = string.Format("{0}法力水晶 第{1}回合 第{2}步操作.txt", p.ownMaxMana, gTurn, gTurnStep);
            string enemysecretIds = "";
            enemysecretIds = Probabilitymaker.Instance.getEnemySecretData();
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("开始计算, 已花费时间: " + DateTime.Now.ToString("HH:mm:ss") + " V" +
                                        this.versionnumber + " " + this.botbehave.ToString());
            if (!Helpfunctions.Instance.writelogg) return;
            // 输出当前场面价值判定
            StringBuilder normalInfo = new StringBuilder("", 100);
            StringBuilder enemyVal = new StringBuilder("[敌方场面] ", 20);
            StringBuilder myVal = new StringBuilder("[我方场面] ", 20);
            StringBuilder handCard = new StringBuilder("[我方手牌] ", 20);
            StringBuilder enemyGuessHandCard = new StringBuilder("[敌方剩余卡牌预测] ", 20);

            normalInfo.AppendFormat("水晶： {0} / {1}", p.mana, p.ownMaxMana);
            StringBuilder ownWeapon = new StringBuilder("");
            if (p.ownWeapon != null)
                ownWeapon.AppendFormat("武器:  {0} ( {1} / {2} ) ", p.ownWeapon.card.nameCN, p.ownWeapon.Angr, p.ownWeapon.Durability);

            normalInfo.AppendFormat(" [我方英雄] {0}（生命: {1} + {2} {3}奥秘数: {4} )", p.ownHeroName, p.ownHero.Hp, p.ownHero.armor, ownWeapon, p.ownSecretsIDList.Count);
            StringBuilder enemyWeapon = new StringBuilder("");
            if (p.enemyWeapon != null)
                enemyWeapon.AppendFormat("武器:  {0} ( {1} / {2} ) ", p.enemyWeapon.card.nameCN, p.enemyWeapon.Angr, p.enemyWeapon.Durability);

            normalInfo.AppendFormat(" [敌方英雄] {0}（生命: {1} + {2} {3}奥秘数: {4}{5})", p.enemyHeroName, p.enemyHero.Hp, p.enemyHero.armor, enemyWeapon, p.enemySecretCount, (p.enemyHero.immune ? " 免疫" : ""));
            normalInfo.AppendFormat(" [任务] quests: {0} {1} {2}", p.ownQuest.Id, p.ownQuest.questProgress, p.ownQuest.maxProgress);
            normalInfo.AppendFormat(" {0} {1} {2}", p.enemyQuest.Id, p.enemyQuest.questProgress, p.enemyQuest.maxProgress);

            foreach (Minion m in p.ownMinions)
            {
                myVal.AppendFormat("{0} ( {1} / {2} ) ", m.handcard.card.nameCN, m.Angr, m.Hp);
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
                if (m.handcard.card.Titan)
                {
                    myVal.Append("[泰坦]");
                    myVal.Append(m.handcard.card.TitanAbilityUsed1 ? " 技能1可以使用" : " 技能1冷却");
                    myVal.Append(m.handcard.card.TitanAbilityUsed2 ? " 技能2可以使用" : " 技能2冷却");
                    myVal.Append(m.handcard.card.TitanAbilityUsed3 ? " 技能3可以使用" : " 技能3冷却");
                }
            }
            foreach (Minion m in p.enemyMinions)
            {
                enemyVal.AppendFormat("{0} ( {1} / {2} ) ", m.handcard.card.nameCN, m.Angr, m.Hp);
                enemyVal.Append(m.frozen ? "[冻结]" : "");
                enemyVal.Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
                enemyVal.Append(m.handcard.card.nameCN).Append(" ( " + m.Angr + " / " + m.Hp + " ) ").Append(m.frozen ? "[冻结]" : "").Append(!m.Ready || m.cantAttack ? "[无法攻击]" : "");
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
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                handCard.AppendFormat("{0}(费用： {1} ； {2} / {3} ) ", hc.card.nameCN, hc.manacost, (hc.addattack + hc.card.Attack), (hc.addHp + hc.card.Health));

            }
            foreach (var item in Hrtprozis.Instance.guessEnemyDeck)
            {
                CardDB.Card card = CardDB.Instance.getCardDataFromDbfID(item.Key);
                enemyGuessHandCard.AppendFormat("{0}{1};", card.nameCN, (item.Value > 1 ? "(" + item.Value + ")" : ""));
            }

            // Helpfunctions.Instance.logg(playedRaceThisTurn);
            // Helpfunctions.Instance.logg(playedRacesLastTurn);
            // Helpfunctions.Instance.logg(playedSpellSchoolThisTurn);
            // Helpfunctions.Instance.logg(playedSpellSchoolLastTurn);
            Helpfunctions.Instance.logg(normalInfo.ToString());
            Helpfunctions.Instance.logg("(猜测对手构筑为:" + p.enemyGuessDeck + " 套牌代码：" + Hrtprozis.Instance.enemyDeckCode);
            Helpfunctions.Instance.logg("预计直伤： " + Hrtprozis.Instance.enemyDirectDmg + "， 加上场攻一共 " + (Hrtprozis.Instance.enemyDirectDmg + p.calEnemyTotalAngr()) + " )");
            Helpfunctions.Instance.logg(enemyVal.ToString());
            Helpfunctions.Instance.logg(myVal.ToString());
            Helpfunctions.Instance.logg(handCard.ToString());
            Helpfunctions.Instance.logg(enemyGuessHandCard.ToString());

            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("#######################################################################");
            Helpfunctions.Instance.logg("turn " + gTurn + "/" + gTurnStep);// gTurn：如果先手，则1357... 如果后手 2468
            //gTurnStep是该回合，多次出牌/动作的每一步
            Helpfunctions.Instance.logg("mana " + currentMana + "/" + ownMaxMana);
            Helpfunctions.Instance.logg("emana " + enemyMaxMana);
            if (p.anzTamsin)
            {
                Helpfunctions.Instance.logg("anzTamsin " + p.anzTamsin);
            }
            Helpfunctions.Instance.logg("own secretsCount: " + ownSecretList.Count);
            Helpfunctions.Instance.logg("enemy secretsCount: " + enemySecretList.Count + " ;" + enemysecretIds);

            Ai.Instance.currentCalculatedBoard = dtimes;

            Hrtprozis.Instance.printHero();
            Hrtprozis.Instance.printOwnMinions();
            Hrtprozis.Instance.printEnemyMinions();
            Handmanager.Instance.printcards();
            Probabilitymaker.Instance.printTurnGraveYard();
            Probabilitymaker.Instance.printGraveyards();
            p.prozis.printOwnDeck();
            printUtils.printRecord = false;
        }
        /// <summary>
        /// 更新游戏数据
        /// </summary>
        public void updateGame()
        {

            Player FriendlyPlayer = GameState.Get().GetFriendlySidePlayer();
            Player OpposingPlayer = GameState.Get().GetOpposingSidePlayer();

            update(FriendlyPlayer, FriendlyPlayer.GetControllerId(), FriendlyPlayer.IsControlledByFriendlySidePlayer());
            update(OpposingPlayer, OpposingPlayer.GetControllerId(), OpposingPlayer.IsControlledByFriendlySidePlayer());
        }

        public void update(Player player, int controllerId, bool ControlledByFriendly)
        {
            List<Card> minions = player.GetBattlefieldZone().GetCards();
            List<Card> Handcards = player.GetHandZone().GetCards();
            List<Card> Deckards = player.GetDeckZone().GetCards();
            List<Card> Graveyards = player.GetGraveyardZone().GetCards();

            updateHero(player, controllerId, ControlledByFriendly);
            updateHeroPower(player, controllerId, ControlledByFriendly);
            updateWeapon(player, controllerId, ControlledByFriendly);

            //更新随从
            foreach (Card card in minions)
            {
                updateMinion(card.GetEntity(), controllerId, ControlledByFriendly);
            }
            //更新手牌
            foreach (Card card in Handcards)
            {
                updateHandcard(card.GetEntity(), controllerId, ControlledByFriendly);
            }
            //更新任务奥秘
            updateSecret(player.GetSecretZone(), controllerId, ControlledByFriendly);
            foreach (Card card in Deckards)
            {
                updateDeck(card.GetEntity(), controllerId, ControlledByFriendly);
            }
            foreach (Card card in Graveyards)
            {
                updateGraveyard(card, card.GetEntity(), controllerId, ControlledByFriendly);
            }
        }
        /// <summary>
        /// 更新英雄
        /// </summary>
        /// <param name="player"></param>
        /// <param name="controllerId"></param>
        /// <param name="ControlledByFriendly"></param>
        public void updateHero(Player player, int controllerId, bool ControlledByFriendly)
        {
            Entity hero = player.GetHero();
            if (ControlledByFriendly)
            {
                this.ownHero.entitiyID = hero.GetEntityId();
                this.ownHero.own = true;
                this.ownHero.isHero = true;
                this.ownHero.Ready = !hero.IsExhausted();
                this.ownHero.cardClass = (TAG_CLASS)hero.GetClass();
                this.ownHeroFatigue = player.GetFatigue();
                this.heroname = hero.GetClass().ToString().ToLower();
                this.ownHero.Angr = hero.GetATK();
                this.ownHero.maxHp = hero.GetHealth();
                this.ownHero.Hp = hero.GetCurrentHealth();
                this.ownHero.armor = hero.GetArmor();
                this.ownHero.frozen = hero.IsFrozen();
                this.ownHero.stealth = hero.IsStealthed();
                this.ownHero.windfury = hero.HasWindfury();
                ownHero.megaWindfury = hero.HasReferencedTag(GAME_TAG.MEGA_WINDFURY);//超级风怒
                this.ownHero.immune = hero.IsImmune();
                this.ownHero.Elusive = hero.HasTag(GAME_TAG.ELUSIVE);
                this.ownHero.numAttacksThisTurn = hero.GetNumAttacksThisTurn();
                this.ownHero.extraAttacksThisTurn = hero.GetExtraAttacksThisTurn();
                this.ownHero.immuneWhileAttacking = hero.GetTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) != 0;
                this.ownHero.nameCN = CardDB.Instance.cardNameCNstringToEnum(hero.GetName());
            }
            else
            {
                this.enemyHero.entitiyID = hero.GetEntityId();
                this.enemyHero.own = false;
                this.enemyHero.isHero = true;
                this.enemyHero.Ready = !hero.IsExhausted();
                this.enemyHero.cardClass = (TAG_CLASS)hero.GetClass();
                this.enemyHeroFatigue = player.GetFatigue();
                this.enemyHeroname = hero.GetClass().ToString().ToLower();
                this.enemyHero.Angr = hero.GetATK();
                this.enemyHero.maxHp = hero.GetHealth();
                this.enemyHero.Hp = hero.GetCurrentHealth();
                this.enemyHero.armor = hero.GetArmor();
                this.enemyHero.frozen = hero.IsFrozen();
                this.enemyHero.stealth = hero.IsStealthed();
                this.enemyHero.windfury = hero.HasWindfury();
                this.enemyHero.immune = hero.IsImmune();
                this.enemyHero.Elusive = hero.HasTag(GAME_TAG.ELUSIVE);
                this.enemyHero.numAttacksThisTurn = hero.GetNumAttacksThisTurn();
                this.enemyHero.extraAttacksThisTurn = hero.GetExtraAttacksThisTurn();
                this.enemyHero.immuneWhileAttacking = hero.GetTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) != 0;
                this.enemyHero.nameCN = CardDB.Instance.cardNameCNstringToEnum(hero.GetName());
            }

            List<Entity> attaches = hero.GetAttachments();//附魔
            if (attaches != null && attaches.Count > 0)
            {
                List<miniEnch> enchs = new List<miniEnch>();
                foreach (Entity attEnt in attaches)
                {
                    var cid = attEnt.GetCardId();
                    if (string.IsNullOrEmpty(cid))
                    {
                        cid = attEnt.GetEntityDef().GetCardId();
                    }
                    int creator = attEnt.GetCreatorId();
                    int cpyDeath = attEnt.GetTag(GAME_TAG.COPY_DEATHRATTLE);
                    int ctrlId = attEnt.GetControllerId();
                    enchs.Add(new miniEnch(CardDB.Instance.cardIdstringToEnum(cid), creator, ctrlId, cpyDeath, attEnt));
                }
                if (ControlledByFriendly)
                {
                    this.ownHero.loadEnchantments(enchs, ownController);
                }
                else
                {
                    this.enemyHero.loadEnchantments(enchs, enemyController);
                }
            }
        }
        /// <summary>
        /// 更新英雄技能
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateHeroPower(Player player, int controllerId, bool ControlledByFriendly)
        {
            Entity heroPower = player.GetHeroPower();
            if (ControlledByFriendly)
            {
                this.heroAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(heroPower.GetCardId()));
                this.ownAbilityisReady = !heroPower.IsExhausted();
                this.ownHeroPowerCost = heroPower.GetRealTimeCost();
            }
            else if (controllerId == enemyController)
            {
                this.enemyAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(heroPower.GetCardId()));
                this.enemyHeroPowerCost = heroPower.GetRealTimeCost();
            }
        }
        /// <summary>
        /// 更新武器
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateWeapon(Player player, int controllerId, bool ControlledByFriendly)
        {

            if (!player.HasWeapon())
            {
                if (ControlledByFriendly)
                {
                    ownWeapon = new Weapon();
                }
                else
                {
                    enemyWeapon = new Weapon();
                }
                return;
            }
            Entity weapon = player.GetWeaponCard().GetEntity();
            if (ControlledByFriendly)
            {
                ownWeapon = new Weapon();
                if (weapon != null)
                {
                    ownWeapon.equip(CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(weapon.GetCardId())));
                    ownWeapon.Angr = weapon.GetATK();
                    ownWeapon.Durability = weapon.GetHealth();
                    ownWeapon.poisonous = weapon.IsPoisonous();
                    ownWeapon.lifesteal = weapon.HasLifesteal();
                    ownWeapon.windfury = weapon.HasWindfury();
                    //武器法术迸发
                    ownWeapon.Spellburst = weapon.HasSpellburst();
                    // 武器计数器
                    int scriptNum1 = weapon.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
                    ownWeapon.scriptNum1 = scriptNum1;
                    // Helpfunctions.Instance.ErrorLog("武器计数器" + scriptNum1);
                    if (!this.ownHero.windfury) this.ownHero.windfury = ownWeapon.windfury;

                }
            }
            else
            {
                enemyWeapon = new Weapon();
                if (weapon != null)
                {
                    enemyWeapon.equip(CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(weapon.GetCardId())));
                    enemyWeapon.Angr = weapon.GetATK();
                    enemyWeapon.Durability = weapon.GetHealth();
                    enemyWeapon.poisonous = weapon.IsPoisonous();
                    enemyWeapon.lifesteal = weapon.HasLifesteal();
                    enemyWeapon.windfury = weapon.HasWindfury();
                    //武器法术迸发
                    enemyWeapon.Spellburst = weapon.HasSpellburst();
                    // 武器计数器
                    int scriptNum1 = weapon.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
                    enemyWeapon.scriptNum1 = scriptNum1;
                    // Helpfunctions.Instance.ErrorLog("武器计数器" + scriptNum1);
                    if (!this.enemyHero.windfury) this.enemyHero.windfury = enemyWeapon.windfury;

                }
            }
        }
        /// <summary>
        /// 更新随从
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateMinion(Entity entity, int controllerId, bool ControlledByFriendly)
        {
            int zp = entity.GetZonePosition();

            if (zp >= 1)
            {
                CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entity.GetCardId()));
                Minion m = new Minion();
                m.name = c.nameEN;
                m.nameCN = c.nameCN;
                m.handcard.card = c;

                m.zonepos = zp;//棋盘上的位置
                m.entitiyID = entity.GetEntityId();//实体id
                m.Angr = entity.GetATK();
                m.maxHp = entity.GetHealth();
                m.Hp = entity.GetCurrentHealth();
                if (m.Hp <= 0)
                    return;
                m.wounded = m.maxHp > m.Hp;
                m.own = entity.IsControlledByFriendlySidePlayer();

                m.ShowSleepZZZOverride = entity.IsAsleep();
                m.playedThisTurn = entity.GetNumTurnsInPlay() == 0 ? true : false;
                m.numAttacksThisTurn = entity.GetNumAttacksThisTurn();//本回合攻击次数
                m.extraAttacksThisTurn = entity.GetExtraAttacksThisTurn();//本回合额外的攻击次数
                m.exhausted = entity.IsExhausted();//枯竭的
                m.frozen = entity.IsFrozen();//冰冻
                m.Spellburst = entity.HasSpellburst();//法力迸发

                m.CooldownTurn = entity.GetLocationCooldown();//获取地标冷却

                m.taunt = entity.HasTaunt();//嘲讽
                m.windfury = entity.HasWindfury();//风怒
                m.megaWindfury = entity.HasReferencedTag(GAME_TAG.MEGA_WINDFURY);//超级风怒
                m.divineshild = entity.HasDivineShield();//圣盾
                m.stealth = entity.IsStealthed();//潜行
                m.poisonous = entity.IsPoisonous();//剧毒
                m.lifesteal = entity.HasLifesteal();//吸血
                m.rush = entity.HasRush() ? 1 : 0;//突袭
                m.reborn = entity.HasReborn();//复生
                m.Elusive = entity.HasTag(GAME_TAG.ELUSIVE);//扰魔
                m.charge = entity.HasCharge() ? 1 : 0;//冲锋
                m.nonKeywordCharge = entity.GetTag(GAME_TAG.NON_KEYWORD_CHARGE);//非关键词冲锋
                m.immune = entity.IsImmune();//免疫
                m.immuneWhileAttacking = entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING);//攻击时免疫
                m.untouchable = entity.HasTag(GAME_TAG.UNTOUCHABLE);//无法选择
                m.silenced = entity.HasTag(GAME_TAG.SILENCED);//沉默的
                m.cantAttackHeroes = entity.HasTag(GAME_TAG.CANNOT_ATTACK_HEROES);//无法攻击英雄
                m.cantAttack = entity.HasTag(GAME_TAG.CANT_ATTACK);//无法攻击
                // m.cantBeTargetedBySpellsOrHeroPowers = entity.CanBeTargetedByHeroPowers() || entity.CanBeTargetedByHeroPowers();//无法被法术和英雄技能选中
                m.hChoice = entity.GetTag(GAME_TAG.HIDDEN_CHOICE);
                m.dormant = entity.GetTag(GAME_TAG.DORMANT);//休眠
                m.untouchable = m.untouchable || m.dormant > 0;

                List<Entity> attaches = entity.GetAttachments();//附魔
                if (attaches != null && attaches.Count > 0)
                {
                    List<miniEnch> enchs = new List<miniEnch>();
                    foreach (var attEnt in attaches)
                    {
                        String cid = attEnt.GetCardId();
                        if (string.IsNullOrEmpty(cid))
                        {
                            cid = attEnt.GetEntityDef().GetCardId();
                        }
                        int creator = attEnt.GetCreatorId();
                        int cpyDeath = attEnt.GetTag(GAME_TAG.COPY_DEATHRATTLE);
                        int ctrlId = attEnt.GetControllerId();
                        enchs.Add(new miniEnch(CardDB.Instance.cardIdstringToEnum(cid), creator, ctrlId, cpyDeath, attEnt));
                    }
                    m.loadEnchantments(enchs, entity.GetCreatorId());
                }

                m.updateReadyness();

                // if (m.rush > 0 && !m.untouchable && m.charge == 0 && (m.numAttacksThisTurn == 0 || (m.numAttacksThisTurn == 1 && m.windfury)))
                // {
                //     m.y = true;
                //     if (m.playedThisTurn) m.cantAttackHeroes = true;
                //     else m.cantAttackHeroes = false;

                // }

                m.handcard.card.TAG_SCRIPT_DATA_NUM_1 = c.TAG_SCRIPT_DATA_NUM_1;//标签脚本数据编号1，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
                m.handcard.card.TAG_SCRIPT_DATA_NUM_2 = c.TAG_SCRIPT_DATA_NUM_2;//标签脚本数据编号2，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
                m.handcard.card.TAG_SCRIPT_DATA_NUM_3 = c.TAG_SCRIPT_DATA_NUM_3;//标签脚本数据编号3，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
                m.handcard.card.TAG_SCRIPT_DATA_NUM_4 = c.TAG_SCRIPT_DATA_NUM_4;//标签脚本数据编号4，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
                m.handcard.card.DECK_ACTION_COST = c.DECK_ACTION_COST;//卡组操作消耗法力值
                m.handcard.card.Dredge = c.Dredge;//探底
                // m.CooldownTurn = c.CooldownTurn;//地标冷却回合
                m.handcard.card.Infuse = c.Infuse;//注能
                m.handcard.card.Infused = c.Infused;//已注能
                m.handcard.card.InfuseNum = c.InfuseNum;//注能数量
                m.handcard.card.Manathirst = c.Manathirst;//法力渴求
                m.handcard.card.Finale = c.Finale;//压轴
                m.handcard.card.Overheal = c.Overheal;//过量治疗
                m.handcard.card.Titan = c.Titan;//泰坦
                m.handcard.card.TitanAbilityUsed1 = c.TitanAbilityUsed1;//泰坦第一技能
                m.handcard.card.TitanAbilityUsed2 = c.TitanAbilityUsed2;//泰坦第二技能
                m.handcard.card.TitanAbilityUsed3 = c.TitanAbilityUsed3;//泰坦第三技能
                m.handcard.card.TitanAbility = c.TitanAbility;//泰坦技能列表
                m.handcard.card.Forge = c.Forge;//锻造
                m.handcard.card.ForgeCost = c.ForgeCost;//锻造消耗法力值
                m.handcard.card.Forged = c.Forged;//已锻造
                m.handcard.card.Quickdraw = c.Quickdraw;//快枪
                m.handcard.card.Excavate = c.Excavate;//发掘
                m.handcard.card.Elusive = c.Elusive;//扰魔

                if (m.charge > 0 && m.playedThisTurn && !m.exhausted && m.numAttacksThisTurn == 0)
                {
                    needSleep = true;
                    Helpfunctions.Instance.ErrorLog("[AI] 冲锋的随从还没有准备好");
                }

                if (ControlledByFriendly)
                {
                    m.synergy = PenalityManager.Instance.getClassRacePriorityPenality(this.ownHero.cardClass, (TAG_RACE)c.race);
                    this.ownMinions.Add(m);
                }
                else
                {
                    m.synergy = PenalityManager.Instance.getClassRacePriorityPenality(this.enemyHero.cardClass, (TAG_RACE)c.race);
                    this.enemyMinions.Add(m);
                }
            }
        }
        /// <summary>
        /// 更新手牌
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateHandcard(Entity entity, int controllerId, bool ControlledByFriendly)
        {
            string cardId = entity.GetCardId();
            int zp = entity.GetZonePosition();
            if (zp >= 1)
            {
                if (ControlledByFriendly)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(cardId));
                    Handmanager.Handcard hc = new Handmanager.Handcard(c);
                    hc.MODULAR_ENTITY_PART_1 = entity.GetTag(GAME_TAG.MODULAR_ENTITY_PART_1);
                    hc.MODULAR_ENTITY_PART_2 = entity.GetTag(GAME_TAG.MODULAR_ENTITY_PART_2);
                    //自定义卡牌的模块1和模块2
                    if (hc.MODULAR_ENTITY_PART_1 != 0 && hc.MODULAR_ENTITY_PART_2 != 0)
                    {
                        hc.card.MODULAR_ENTITY_PART_1 = hc.MODULAR_ENTITY_PART_1;
                        hc.card.MODULAR_ENTITY_PART_2 = hc.MODULAR_ENTITY_PART_2;
                        if (hc.card.type == CardDB.cardtype.MOB)
                            c.updateDIYCard();
                    }
                    hc.position = zp;
                    hc.entity = entity.GetEntityId();
                    hc.manacost = entity.GetCost();
                    hc.poweredUp = entity.GetTag(GAME_TAG.POWERED_UP);//手牌高亮
                    hc.SCRIPT_DATA_NUM_1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
                    hc.SCRIPT_DATA_NUM_2 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
                    hc.temporary = entity.GetTag(3630) > 0 ? true : false;
                    hc.valeeraShadow = entity.GetTag(779) > 0 ? true : false;
                    // entity.GetTag(GAME_TAG.LITERALLY_UNPLAYABLE);//无法使用
                    // entity.GetTag(GAME_TAG.UNPLAYABLE_VISUALS);
                    hc.addattack = entity.GetATK() - entity.GetDefATK();
                    hc.addHp = entity.GetHealth() - entity.GetDefHealth();

                    List<Entity> attaches = entity.GetAttachments();//附魔
                    if (attaches != null && attaches.Count > 0)
                    {
                        hc.enchs = new List<CardDB.cardIDEnum>();
                        foreach (Entity attEnt in attaches)
                        {
                            String cid = attEnt.GetCardId();
                            if (string.IsNullOrEmpty(cid))
                            {
                                cid = attEnt.GetEntityDef().GetCardId();
                            }
                            hc.enchs.Add(CardDB.Instance.cardIdstringToEnum(cid));
                        }
                    }
                    handCards.Add(hc);
                    anzcards++;
                }
                else
                {
                    enemyAnzCards++;
                }
            }

        }
        /// <summary>
        /// 更新奥秘区域。包括了奥秘、任务、支线任务、光环、咒符
        /// </summary>
        /// <param name="zoneSecret">奥秘区域</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateSecret(ZoneSecret zoneSecret, int controllerId, bool ControlledByFriendly)
        {
            int cardCount = zoneSecret.GetCardCount();
            if (cardCount < 1)
            {
                return;
            }
            else
            {
                //奥秘
                List<Card> Secrets = zoneSecret.GetSecretCards();
                //任务
                List<Card> Quests = zoneSecret.GetQuestCards();
                //支线任务
                // List<Card> SideQuests = zoneSecret.GetSideQuestCards();
                //咒符
                // List<Card> Sigils = zoneSecret.GetSigilCards();
                //光环
                // List<Card> Objectives = zoneSecret.GetObjectiveCards();
                foreach (Card quest in Quests)
                {
                    Entity entity = quest.GetEntity();
                    string cardId = quest.GetCardId();
                    int currentQuestProgress = entity.GetTag(GAME_TAG.QUEST_PROGRESS);
                    int questProgressTotal = entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL);
                    if (ControlledByFriendly)
                    {

                        Questmanager.Instance.updateQuestStuff(cardId, currentQuestProgress, questProgressTotal, true);
                    }
                    else
                    {
                        Questmanager.Instance.updateQuestStuff(cardId, currentQuestProgress, questProgressTotal, true);

                    }

                }
                foreach (Card secret in Secrets)
                {
                    Entity entity = secret.GetEntity();
                    string cardId = secret.GetCardId();
                    int currentQuestProgress = entity.GetTag(GAME_TAG.QUEST_PROGRESS);
                    int questProgressTotal = entity.GetTag(GAME_TAG.QUEST_PROGRESS_TOTAL);
                    Questmanager.Instance.updateQuestStuff(cardId, currentQuestProgress, questProgressTotal, true);
                    if (ControlledByFriendly)
                        ownSecretList.Add(cardId);
                    else
                        enemySecretList.Add(entity.GetEntityId(), (TAG_CLASS)entity.GetClass());

                }
            }

        }
        /// <summary>
        /// 更新牌库
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        void updateDeck(Entity entity, int controllerId, bool ControlledByFriendly)
        {
            string cardId = entity.GetEntityDef().GetCardId();

            if (ControlledByFriendly)
                ownDecksize++;
            else
                enemyDecksize++;
            if (string.IsNullOrEmpty(cardId))
                return;
            int entityId = entity.GetEntityId();
            TAG_ZONE zone = (TAG_ZONE)entity.GetZone();
            CardDB.cardIDEnum idEnum = CardDB.Instance.cardIdstringToEnum(cardId);

            if (ownController == 1 && entityId < 34 ||
                ownController == 2 && entityId >= 34 && entityId < 64) //4~33为player1初始卡组  34~63为player2初始卡组
            {
                if (zone != TAG_ZONE.DECK && tmpDeck.ContainsKey(idEnum))
                    tmpDeck[idEnum]--;
            }
            else if (entityId >= 64) //TODO:63之后是双方衍生卡
            {
                if (ControlledByFriendly && zone == TAG_ZONE.DECK)
                {
                    if (turnDeck.ContainsKey(idEnum))
                        turnDeck[idEnum]++;
                    else turnDeck.Add(idEnum, 1);
                }
            }
        }
        /// <summary>
        /// 更新坟地
        /// </summary>
        /// <param name="card">卡牌</param>
        /// <param name="entity">实体</param>
        /// <param name="controllerId">玩家id</param>
        /// <param name="ControlledByFriendly">受友方控制</param>
        private void updateGraveyard(Card card, Entity entity, int controllerId, bool ControlledByFriendly)
        {
            Triton.Game.Mapping.TAG_CARDTYPE cardType = entity.GetCardType();
            if (cardType == Triton.Game.Mapping.TAG_CARDTYPE.ENCHANTMENT)//附魔牌不加入坟场数据库，是否合理？
                return;
            CardDB.cardIDEnum idEnum = CardDB.Instance.cardIdstringToEnum(entity.GetCardId());
            GraveYardItem gyi = new GraveYardItem(idEnum, entity.GetEntityId(), ControlledByFriendly, GraveYardItem.EnumGraveyardStatus.Unknown);
            Zone prevZone = card.GetPrevZone();
            if (prevZone != null)
            {
                var prevZoneName = prevZone.name;
                if (prevZoneName.Contains("Deck"))//牌库->坟场, 爆牌
                {
                    gyi.status = GraveYardItem.EnumGraveyardStatus.Burnt;
                }
                else if (prevZoneName.Contains("Play") || prevZoneName.Contains("Weapon") || prevZoneName.Contains("Secret"))//场面->坟场, 随从:死亡,武器任务奥秘:使用过
                {
                    if (cardType == Triton.Game.Mapping.TAG_CARDTYPE.MINION)
                    {
                        gyi.status = GraveYardItem.EnumGraveyardStatus.Died;
                    }
                    else
                    {
                        gyi.status = GraveYardItem.EnumGraveyardStatus.Used;
                    }
                }
                else if (prevZoneName.Contains("Hand"))//手牌->坟场，弃牌
                {
                    gyi.status = GraveYardItem.EnumGraveyardStatus.Discard;
                }
            }
            else
            {
                if (cardType == Triton.Game.Mapping.TAG_CARDTYPE.SPELL)//使用法术prevZone为空
                {
                    gyi.status = GraveYardItem.EnumGraveyardStatus.Used;
                }
            }
            graveYard.Add(gyi);
        }

    }

}
