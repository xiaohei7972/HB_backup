using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Triton.Game.Data;
using log4net;
using Logger = Triton.Common.LogUtilities.Logger;

namespace HREngine.Bots
{
    /*
    留牌系统功能说明：
    (最低优先级)
    - 保留所有法力值消耗低于XXX的卡牌 (法力值规则)
    (中等优先级)
    - 保留指定的卡牌 (可以选择保留1或2张)
    - 弃掉指定的卡牌 (所有卡牌)
    (高优先级)
    - 创建规则，根据其他卡牌的存在来弃掉(所有)卡牌
    (最高优先级)
    - 创建规则，根据其他卡牌的存在来保留1或2张卡牌
    - 支持不同行为模式的不同规则集
     
    其他特性：
    - 可以创建类似：如果我有硬币，则...的规则
    - 可以为不同的己方英雄-敌方英雄组合创建规则 (任意或所有)
    - 允许针对同一卡牌、同一英雄组合同时存在不同优先级的规则
      (即可能同时存在3个规则)
     */

    /// <summary>
    /// 留牌系统类，负责处理炉石传说游戏中的开局留牌逻辑
    /// </summary>
    public class Mulligan
    {
        /// <summary>
        /// 留牌规则文件路径
        /// </summary>
        string pathToMulligan = "";
        
        /// <summary>
        /// 留牌规则是否已加载
        /// </summary>
        public bool mulliganRulesLoaded = false;
        
        /// <summary>
        /// 留牌规则字典，存储当前行为模式的规则
        /// </summary>
        Dictionary<string, string> MulliganRules = new Dictionary<string, string>();
        
        /// <summary>
        /// 留牌规则数据库，存储所有行为模式的规则
        /// </summary>
        Dictionary<string, Dictionary<string, string>> MulliganRulesDB = new Dictionary<string, Dictionary<string, string>>();
        
        /// <summary>
        /// 手动留牌规则字典
        /// </summary>
        Dictionary<CardDB.cardIDEnum, string> MulliganRulesManual = new Dictionary<CardDB.cardIDEnum, string>();
        
        /// <summary>
        /// 卡牌实体列表
        /// </summary>
        List<CardIDEntity> cards = new List<CardIDEntity>();
        
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        /// <summary>
        /// 卡牌实体类，用于存储卡牌信息和留牌状态
        /// </summary>
        public class CardIDEntity
        {
            /// <summary>
            /// 卡牌ID枚举
            /// </summary>
            public CardDB.cardIDEnum id = CardDB.cardIDEnum.None;
            
            /// <summary>
            /// 实体ID
            /// </summary>
            public int entitiy = 0;
            
            /// <summary>
            /// 留牌状态 (0:未决定, 正数:保留, 负数:弃掉)
            /// </summary>
            public int hold = 0;
            
            /// <summary>
            /// 规则留牌状态
            /// </summary>
            public int holdByRule = 0;
            
            /// <summary>
            /// 法力值规则留牌状态
            /// </summary>
            public int holdByManarule = 1;
            
            /// <summary>
            /// 留牌原因
            /// </summary>
            public string holdReason = "";
            
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="id_string">卡牌ID字符串</param>
            /// <param name="entt">实体ID</param>
            public CardIDEntity(string id_string, int entt)
            {
                this.id = CardDB.Instance.cardIdstringToEnum(id_string);
                this.entitiy = entt;
            }
        }

        /// <summary>
        /// 单例实例
        /// </summary>
        private static Mulligan instance;

        /// <summary>
        /// 单例访问属性
        /// </summary>
        public static Mulligan Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Mulligan();
                }
                return instance;
            }
        }

        /// <summary>
        /// 私有构造函数，实现单例模式
        /// </summary>
        private Mulligan()
        {
        }

        /// <summary>
        /// 读取留牌规则
        /// </summary>
        /// <param name="behavName">行为模式名称</param>
        private void readRules(string behavName)
        {
            // 如果规则数据库中已存在该行为模式的规则，直接使用
            if (MulliganRulesDB.ContainsKey(behavName))
            {
                MulliganRules = MulliganRulesDB[behavName];
                mulliganRulesLoaded = true;
                return;
            }
		
            // 检查行为路径是否存在
            if (!Silverfish.Instance.BehaviorPath.ContainsKey(behavName))
            {
                Helpfunctions.Instance.ErrorLog(behavName + ": no special mulligan.");
                return;
            }

            // 构建留牌规则文件路径
            pathToMulligan = Path.Combine(Silverfish.Instance.BehaviorPath[behavName], "_mulligan.txt");

            // 检查文件是否存在
            if (!System.IO.File.Exists(pathToMulligan))
            {
                Helpfunctions.Instance.ErrorLog(behavName + ": no special mulligan.");
                return;
            }
            
            try
            {
                // 读取文件所有行
                string[] lines = System.IO.File.ReadAllLines(pathToMulligan);
                MulliganRules.Clear();
                
                // 解析每一行规则
                foreach (string s in lines)
                {
                    // 跳过空行和注释行
                    if (s == "" || s == null) continue;
                    if (s.StartsWith("//")) continue;
                    
                    // 分割规则键值对
                    string[] oneRule = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                    // 分割键和值的各个部分
                    string[] tempKey = oneRule[0].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] tempValue = oneRule[1].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // 构建规则键和值
                    string MullRuleKey = joinSomeTxt(tempKey[0], ";", tempKey[1], ";", tempKey[2], ";", (tempValue[1] != "/") ? "1" : "0");
                    string MullRuleValue = joinSomeTxt(tempKey[3], ";", tempValue[0], ";", tempValue[1]);

                    // 添加或更新规则
                    if (MulliganRules.ContainsKey(MullRuleKey)) MulliganRules[MullRuleKey] = MullRuleValue;
                    else MulliganRules.Add(MullRuleKey, MullRuleValue);
                }
            }
            catch (Exception e)
            {
                Helpfunctions.Instance.ErrorLog("[开局留牌] 留牌文件_mulligan.txt读取错误. 只能应用默认配置. 异常:" + e.Message);
                return;
            }
            
            Helpfunctions.Instance.ErrorLog("[开局留牌] 读取规则—— " + behavName);
            // 验证规则
            validateRule(behavName);
        }

        /// <summary>
        /// 验证留牌规则的有效性
        /// </summary>
        /// <param name="behavName">行为模式名称</param>
        private void validateRule(string behavName)
        {
            // 存储被拒绝的规则
            List<string> rejectedRule = new List<string>();
            // 修复的规则数量
            int repairedRules = 0;
            // 临时规则字典
            Dictionary<string, string> MulliganRulesTmp = new Dictionary<string, string>();

            // 遍历所有规则
            foreach (KeyValuePair<string, string> oneRule in MulliganRules)
            {
                // 分割规则键和值
                string[] ruleKey = oneRule.Key.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                string[] ruleValue = oneRule.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                string ruleValueOne = oneRule.Value;

                // 检查规则格式是否正确
                if (ruleKey.Length != 4 || ruleValue.Length != 3) { rejectedRule.Add(getClearRule(oneRule.Key)); continue; }

                // 验证卡牌ID、英雄枚举值和规则类型
                if (ruleKey[0] != CardDB.Instance.cardIdstringToEnum(ruleKey[0]).ToString()) { rejectedRule.Add(getClearRule(oneRule.Key)); continue; }
                if (ruleKey[1] != Hrtprozis.Instance.heroNametoEnum(ruleKey[1]).ToString()) { rejectedRule.Add(getClearRule(oneRule.Key)); continue; }
                if (ruleKey[2] != Hrtprozis.Instance.heroNametoEnum(ruleKey[2]).ToString()) { rejectedRule.Add(getClearRule(oneRule.Key)); continue; }
                if (ruleValue[0] != "Hold" && ruleValue[0] != "Discard") { rejectedRule.Add(getClearRule(oneRule.Key)); continue; }

                // 验证数量值是否为整数
                try
                {
                    Convert.ToInt32(ruleValue[1]);
                }
                catch (Exception e) { rejectedRule.Add(getClearRule(oneRule.Key)); Helpfunctions.Instance.logg("异常 " + e.Message); continue; }

                // 处理规则值的第三部分
                if (ruleValue[2] != "/")
                {
                    // 如果长度小于4，视为法力值规则
                    if (ruleValue[2].Length < 4)
                    {
                        int manaRule = 4;
                        try
                        {
                            manaRule = Convert.ToInt32(ruleValue[2]);
                        }
                        catch { }
                        
                        // 限制法力值规则范围
                        if (manaRule < 0) manaRule = 0;
                        else if (manaRule > 100) manaRule = 100;

                        // 构建新的规则值
                        StringBuilder tmpSB = new StringBuilder(ruleValue[0], 500);
                        tmpSB.Append(";").Append(ruleValue[1]).Append(";").Append(manaRule);
                        ruleValueOne = tmpSB.ToString();
                    }
                    else
                    {
                        // 处理卡牌列表规则
                        bool wasBreak = false;
                        string[] addedCards = ruleValue[2].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        Dictionary<CardDB.cardIDEnum, string> MulliganRulesManualTmp = new Dictionary<CardDB.cardIDEnum, string>();
                        
                        foreach (string s in addedCards)
                        {
                            CardDB.cardIDEnum tempID = CardDB.Instance.cardIdstringToEnum(s);
                            if (s != tempID.ToString())
                            {
                                rejectedRule.Add(getClearRule(oneRule.Key));
                                wasBreak = true;
                                break;
                            }
                            else
                            {
                                // 处理重复卡牌
                                if (MulliganRulesManualTmp.ContainsKey(tempID)) { repairedRules++; continue; }
                                else MulliganRulesManualTmp.Add(tempID, "");
                            }
                        }
                        
                        if (wasBreak) continue;
                        
                        // 构建新的规则值
                        StringBuilder tmpSB = new StringBuilder(ruleValue[0], 500);
                        tmpSB.Append(";").Append(ruleValue[1]).Append(";" );
                        for (int i = 0; i < MulliganRulesManualTmp.Count; i++)
                        {
                            if (i + 1 == MulliganRulesManualTmp.Count) break;
                            tmpSB.Append(MulliganRulesManualTmp.ElementAt(i).Key.ToString()).Append("/");
                        }
                        tmpSB.Append(MulliganRulesManualTmp.ElementAt(MulliganRulesManualTmp.Count - 1).Key.ToString());
                        ruleValueOne = tmpSB.ToString();
                    }
                }

                // 添加到临时规则字典
                MulliganRulesTmp.Add(oneRule.Key, ruleValueOne);
            }

            // 处理被拒绝的规则
            if (rejectedRule.Count > 0)
            {
                Helpfunctions.Instance.ErrorLog("[开局留牌] 弃掉卡牌的规则列表:");
                foreach (string tmp in rejectedRule)
                {
                    Helpfunctions.Instance.ErrorLog(tmp);
                }
                Helpfunctions.Instance.ErrorLog("[开局留牌] 关闭规则列表.");
            }

            // 处理修复的规则
            if (repairedRules > 0) Helpfunctions.Instance.ErrorLog(repairedRules.ToString() + " repaired rules");
            
            // 清空原规则并添加验证后的规则
            MulliganRules.Clear();
            foreach (KeyValuePair<string, string> oneRule in MulliganRulesTmp)
            {
                MulliganRules.Add(oneRule.Key, oneRule.Value);
            }

            // 记录规则加载结果
            Helpfunctions.Instance.ErrorLog("[开局留牌] " + (MulliganRules.Count > 0 ? (MulliganRules.Count + " 读取留牌规则成功") : "并没有特殊的规则."));
            mulliganRulesLoaded = true;
            
            // 处理旧版本兼容性
            if (behavName == "")
            {
                MulliganRulesDB.Add("控场模式", new Dictionary<string, string>(MulliganRules));
                MulliganRulesDB.Add("怼脸模式", new Dictionary<string, string>(MulliganRules));
            }
            else
            {
                MulliganRulesDB.Add(behavName, new Dictionary<string, string>(MulliganRules));
            }
        }

        /// <summary>
        /// 获取清晰的规则字符串
        /// </summary>
        /// <param name="ruleKey">规则键</param>
        /// <returns>格式化后的规则字符串</returns>
        private string getClearRule(string ruleKey)
        {
            if (MulliganRules.ContainsKey(ruleKey))
            {
                StringBuilder clearRule = new StringBuilder("", 2000);
                string[] rKey = ruleKey.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                string[] rValue = MulliganRules[ruleKey].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                clearRule.Append(rKey[0]).Append(";").Append(rKey[1]).Append(";").Append(rKey[2]).Append(";");
                clearRule.Append(rValue[0]).Append(":").Append(rValue[1]).Append(";").Append(rValue[2]).Append("\r\n");
                return clearRule.ToString();
            }
            else return "noKey";
        }

        /// <summary>
        /// 获取留牌规则键
        /// </summary>
        /// <param name="cardIDM">卡牌ID</param>
        /// <param name="ownMHero">己方英雄</param>
        /// <param name="enemyMHero">敌方英雄</param>
        /// <param name="isExtraRule">是否为额外规则</param>
        /// <returns>规则键字符串</returns>
        private string getMullRuleKey(CardDB.cardIDEnum cardIDM = CardDB.cardIDEnum.None, HeroEnum ownMHero = HeroEnum.None, HeroEnum enemyMHero = HeroEnum.None, int isExtraRule = 0)
        {
            StringBuilder MullRuleKey = new StringBuilder("", 500);
            MullRuleKey.Append(cardIDM).Append(";").Append(ownMHero).Append(";").Append(enemyMHero).Append(";").Append(isExtraRule);
            return MullRuleKey.ToString();
        }
        
        /// <summary>
        /// 连接多个文本字符串
        /// </summary>
        /// <param name="v1">文本1</param>
        /// <param name="v2">文本2</param>
        /// <param name="v3">文本3</param>
        /// <param name="v4">文本4</param>
        /// <param name="v5">文本5</param>
        /// <param name="v6">文本6</param>
        /// <param name="v7">文本7</param>
        /// <returns>连接后的字符串</returns>
        private string joinSomeTxt(string v1 = "", string v2 = "", string v3 = "", string v4 = "", string v5 = "", string v6 = "", string v7 = "")
        {
            StringBuilder retValue = new StringBuilder("", 500);
            retValue.Append(v1).Append(v2).Append(v3).Append(v4).Append(v5).Append(v6).Append(v7);
            return retValue.ToString();
        }

        /// <summary>
        /// 获取留牌列表
        /// </summary>
        /// <param name="mulliganData">留牌数据</param>
        /// <param name="behave">行为模式</param>
        /// <returns>是否成功应用留牌规则</returns>
        public bool getHoldList(MulliganData mulliganData, Behavior behave)
        {
            // 清空卡牌列表
            cards.Clear();
            // 读取规则
            readRules(behave.BehaviorName());
            
            // 检查规则是否加载成功
            if (!mulliganRulesLoaded) return false;
            
            // 检查卡牌数量是否为3或4
            if (!(mulliganData.Cards.Count == 3 || mulliganData.Cards.Count == 4))
            {
                Helpfunctions.Instance.ErrorLog("[Mulligan] Mulligan is not used, since it got number of cards: " + cards.Count.ToString());
                return false;
            }

            Log.InfoFormat("[开局留牌] 应用这个 {0} 规则:", behave.BehaviorName());

            // 添加卡牌到列表
            for (var i = 0; i < mulliganData.Cards.Count; i++)
            {
                cards.Add(new CardIDEntity(mulliganData.Cards[i].Entity.Id, i));
            }
            
            // 获取英雄类型
            HeroEnum ownHeroClass = Hrtprozis.Instance.heroTAG_CLASSstringToEnum(mulliganData.UserClass.ToString());
            HeroEnum enemyHeroClass = Hrtprozis.Instance.heroTAG_CLASSstringToEnum(mulliganData.OpponentClass.ToString());

            // 初始化法力值规则为4
            int manaRule = 4;
            
            // 尝试获取法力值规则
            string MullRuleKey = getMullRuleKey(CardDB.cardIDEnum.None, ownHeroClass, enemyHeroClass, 1);
            if (MulliganRules.ContainsKey(MullRuleKey))
            {
                string[] temp = MulliganRules[MullRuleKey].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                manaRule = Convert.ToInt32(temp[2]);
            }
            else
            {
                // 尝试获取己方英雄的法力值规则
                MullRuleKey = getMullRuleKey(CardDB.cardIDEnum.None, ownHeroClass, HeroEnum.None, 1);
                if (MulliganRules.ContainsKey(MullRuleKey))
                {
                    string[] temp = MulliganRules[MullRuleKey].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    manaRule = Convert.ToInt32(temp[2]);
                }

                // 尝试获取通用法力值规则
                MullRuleKey = getMullRuleKey(CardDB.cardIDEnum.None, HeroEnum.None, HeroEnum.None, 1);
                if (MulliganRules.ContainsKey(MullRuleKey))
                {
                    string[] temp = MulliganRules[MullRuleKey].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    manaRule = Convert.ToInt32(temp[2]);
                }
            }

            // 处理硬币
            CardIDEntity Coin = new CardIDEntity("GAME_005", -888);
            if (cards.Count == 4) cards.Add(Coin); //we have a coin

            // 处理每张卡牌
            foreach (CardIDEntity CardIDEntityC in cards)
            {
                CardDB.Card c = CardDB.Instance.getCardDataFromID(CardIDEntityC.id);
                
                // 应用法力值规则
                if (CardIDEntityC.hold == 0 && CardIDEntityC.holdByRule == 0)
                {
                    if (c.cost < manaRule)
                    {
                        CardIDEntityC.holdByManarule = 2;
                        CardIDEntityC.holdReason = joinSomeTxt("保留这些卡牌因为法力值消耗:", c.cost.ToString(), " 小于预定值:", manaRule.ToString());
                    }
                    else
                    {
                        CardIDEntityC.holdByManarule = -2;
                        CardIDEntityC.holdReason = joinSomeTxt("弃掉这些卡牌因为法力值消耗:", c.cost.ToString(), " 没有小于预定值:", manaRule.ToString());
                    }
                }

                // 初始化规则变量
                int allowedQuantitySimple = 0;
                int allowedQuantityExtra = 0;
                bool hasRuleClassSimple = false;

                // 检查简单规则
                bool hasRule = false;
                string MullRuleKeySimple = getMullRuleKey(c.cardIDenum, ownHeroClass, enemyHeroClass, 0); //Simple key for Class enemy
                if (MulliganRules.ContainsKey(MullRuleKeySimple)) { hasRule = true; hasRuleClassSimple = true; }
                else
                {
                    MullRuleKeySimple = getMullRuleKey(c.cardIDenum, ownHeroClass, HeroEnum.None, 0); //Simple key for ALL enemy
                    if (MulliganRules.ContainsKey(MullRuleKeySimple)) hasRule = true;
                }
                if (hasRule)
                {
                    string[] val = MulliganRules[MullRuleKeySimple].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    allowedQuantitySimple = ((val[1] == "2") ? 2 : 1) * ((val[0] == "Hold") ? 1 : -1);
                }

                // 检查额外规则
                hasRule = false;
                string MullRuleKeyExtra = getMullRuleKey(c.cardIDenum, ownHeroClass, enemyHeroClass, 1); //Extra key for Class enemy
                if (MulliganRules.ContainsKey(MullRuleKeyExtra)) hasRule = true;
                else if (!hasRuleClassSimple)
                {
                    MullRuleKeyExtra = getMullRuleKey(c.cardIDenum, ownHeroClass, HeroEnum.None, 1); //Extra key for ALL enemy
                    if (MulliganRules.ContainsKey(MullRuleKeyExtra)) hasRule = true;
                }
                if (hasRule)
                {
                    string[] val = MulliganRules[MullRuleKeyExtra].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    allowedQuantityExtra = ((val[1] == "2") ? 2 : 1) * ((val[0] == "Hold") ? 1 : -1);
                }

                // 叠加规则
                bool useHold = false;
                bool useDiscard = false;
                bool useHoldRule = false;
                bool useDiscardRule = false;

                if (allowedQuantitySimple != 0 && allowedQuantitySimple != allowedQuantityExtra)
                {
                    if (allowedQuantitySimple > 0) useHold = true;
                    else useDiscard = true;
                }
                if (allowedQuantityExtra != 0)
                {
                    if (allowedQuantityExtra < 0) useDiscardRule = true;
                    else useHoldRule = true;
                }

                // 应用规则
                string[] MullRuleValueExtra = new string[3];
                if (allowedQuantityExtra != 0) MullRuleValueExtra = MulliganRules[MullRuleKeyExtra].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                
                // 应用弃牌规则
                if (useDiscardRule)
                {
                    if (MullRuleValueExtra[2] != "/")
                    {
                        string[] addedCards = MullRuleValueExtra[2].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        MulliganRulesManual.Clear();
                        foreach (string s in addedCards) { MulliganRulesManual.Add(CardDB.Instance.cardIdstringToEnum(s), ""); }

                        foreach (CardIDEntity tmp in cards)
                        {
                            if (CardIDEntityC.entitiy == tmp.entitiy) continue;
                            if (MulliganRulesManual.ContainsKey(tmp.id))
                            {
                                CardIDEntityC.holdByRule = -2;
                                CardIDEntityC.holdReason = joinSomeTxt("符合规则而弃掉: ", getClearRule(MullRuleKeyExtra));
                                break;
                            }
                        }
                    }
                }
                else if (useDiscard)
                {
                    CardIDEntityC.hold = -2;
                    CardIDEntityC.holdReason = joinSomeTxt("符合规则而弃掉: ", getClearRule(MullRuleKeySimple));
                }

                // 应用留牌规则
                if (useHoldRule)
                {
                    if (CardIDEntityC.holdByRule == 0)
                    {
                        string[] addedCards = MullRuleValueExtra[2].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        MulliganRulesManual.Clear();
                        foreach (string s in addedCards) { MulliganRulesManual.Add(CardDB.Instance.cardIdstringToEnum(s), ""); }

                        bool foundFreeCard = false;
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (CardIDEntityC.entitiy == cards[i].entitiy) continue;
                            if (MulliganRulesManual.ContainsKey(cards[i].id))
                            {
                                CardIDEntityC.holdByRule = 2;
                                CardIDEntityC.holdReason = joinSomeTxt("符合规则而保留: ", getClearRule(MullRuleKeyExtra));
                                
                                // 处理被标记为弃牌的卡牌
                                if (cards[i].holdByRule < 0)
                                {
                                    for (int j = i; j < cards.Count; j++)
                                    {
                                        if (CardIDEntityC.entitiy == cards[j].entitiy) continue;
                                        if (MulliganRulesManual.ContainsKey(cards[j].id))
                                        {
                                            if (cards[j].holdByRule < 0) continue;
                                            foundFreeCard = true;
                                            cards[j].holdByRule = 2;
                                            cards[j].holdReason = joinSomeTxt("符合规则而保留: ", getClearRule(MullRuleKeyExtra));
                                            break;
                                        }
                                    }
                                    if (!foundFreeCard)
                                    {
                                        foundFreeCard = true;
                                        cards[i].holdByRule = 2;
                                        cards[i].holdReason = joinSomeTxt("符合规则而保留: ", getClearRule(MullRuleKeyExtra));
                                        break;
                                    }
                                }
                                else
                                {
                                    foundFreeCard = true;
                                    cards[i].holdByRule = 2;
                                    cards[i].holdReason = joinSomeTxt("符合规则而保留: ", getClearRule(MullRuleKeyExtra));
                                }

                                // 处理只允许保留一张的情况
                                if (allowedQuantityExtra == 1)
                                {
                                    foreach (CardIDEntity tmp in cards)
                                    {
                                        if (tmp.entitiy == CardIDEntityC.entitiy) continue;
                                        if (tmp.id == CardIDEntityC.id)
                                        {
                                            tmp.holdByRule = -2;
                                            tmp.holdReason = joinSomeTxt("符合规则而弃掉: ", getClearRule(MullRuleKeyExtra));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                // 应用简单留牌规则
                if (useHold && CardIDEntityC.holdByRule != -2)
                {
                    if (CardIDEntityC.hold == 0)
                    {
                        CardIDEntityC.hold = 2;
                        CardIDEntityC.holdReason = joinSomeTxt("符合规则而保留: ", getClearRule(MullRuleKeySimple));
                        
                        // 处理只允许保留一张的情况
                        if (allowedQuantitySimple == 1)
                        {
                            CardIDEntityC.hold = 1;
                            foreach (CardIDEntity tmp in cards)
                            {
                                if (tmp.entitiy == CardIDEntityC.entitiy) continue;
                                if (tmp.id == CardIDEntityC.id)
                                {
                                    tmp.hold = -2;
                                    tmp.holdReason = joinSomeTxt("discard Second card by rule: ", getClearRule(MullRuleKeySimple));
                                }
                            }
                        }
                    }
                }
            }

            // 移除硬币
            if (cards.Count == 5) cards.Remove(Coin);

            // 确定最终留牌状态
            foreach (CardIDEntity c in cards)
            {
                if (c.holdByRule == 0)
                {
                    if (c.hold == 0)
                    {
                        c.holdByRule = c.holdByManarule;
                    }
                    else
                    {
                        c.holdByRule = c.hold;
                    }
                }
            }

            // 应用特殊留牌规则
            behave.specialMulligan(cards);

            // 设置留牌结果
            for (var i = 0; i < mulliganData.Cards.Count; i++)
            {
                // holdByRule > 0 表示保留，false表示不弃牌
                mulliganData.Mulligans[i] = (cards[i].holdByRule > 0) ? false : true;
                Log.InfoFormat("[开局留牌] {0} {1}.", mulliganData.Cards[i].Entity.Name, cards[i].holdReason);
            }
            return true;
        }

    }

}