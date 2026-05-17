using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Logger = Triton.Common.LogUtilities.Logger;
using log4net;

namespace HREngine.Bots
{   
    /// <summary>
    /// CardDB辅助方法，根据不同参数获取CardDB.Card
    /// </summary>
    partial class CardDB
    {
        /// <summary>
        /// 输入卡牌id，输出cardIDEnum枚举对象
        /// </summary>
        /// <param name="cardId">卡牌id字符串</param>
        /// <returns>cardIDEnum枚举对象</returns>
        public CardDB.cardIDEnum cardIdstringToEnum(string cardId)
        {
            CardDB.cardIDEnum CardEnum;
            if (Enum.TryParse<cardIDEnum>(cardId, false, out CardEnum)) return CardEnum;
            else
            {
                return CardDB.cardIDEnum.None;
            }
        }


        /// <summary>
        /// 输入卡牌中文名，输出CardDB.Card类对象，多个同名，返回第一个
        /// </summary>
        /// <param name="chnName">卡牌中文名</param>
        /// <returns>CardDB.Card类对象</returns>
        public CardDB.Card chnNameToCard(string chnName)
        {
            CardDB.Card c;
            CardDB.cardNameCN enumCn;
            if (Enum.TryParse<CardDB.cardNameCN>(chnName, out enumCn) && cardNameCNToCardList.TryGetValue(enumCn, out c))
            {
                return c;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 输入卡牌英文名，输出cardNameEN枚举对象
        /// </summary>
        /// <param name="cardNameEn">卡牌英文名</param>
        /// <returns>CardDB.cardNameEN枚举对象</returns>
        public CardDB.cardNameEN cardNameENstringToEnum(string cardNameEn)
        {
            CardDB.cardNameEN NameEnum;
            if (Enum.TryParse<CardDB.cardNameEN>(cardNameEn, false, out NameEnum)) return NameEnum;
            else return CardDB.cardNameEN.unknown;
        }

        /// <summary>
        /// 输入卡牌英文名，输出cardNameCN枚举对象
        /// </summary>
        /// <param name="cardNameEn">卡牌英文名</param>
        /// <returns>CardDB.cardNameCN枚举对象</returns>
        public CardDB.cardNameCN cardNameENstringToEnumCN(string cardNameEn)
        {
            CardDB.cardNameEN NameEnum;
            if (Enum.TryParse<CardDB.cardNameEN>(cardNameEn, false, out NameEnum))
            {
                CardDB.Card card = getCardData(NameEnum);
                return card.nameCN;
            }
            else
            {
                return CardDB.cardNameCN.未知;
            }
            ;
        }

        /// <summary>
        /// 输入卡牌中文名，输出cardNameCN枚举对象
        /// </summary>
        /// <param name="cardNameCn"></param>
        /// <returns>CardDB.cardNameCN枚举对象</returns>
        public cardNameCN cardNameCNstringToEnum(string cardNameCn)
        {
            CardDB.cardNameCN NameEnum;
            if (Enum.TryParse<CardDB.cardNameCN>(cardNameCn, false, out NameEnum)) return NameEnum;
            else return CardDB.cardNameCN.未知;
        }

        /// <summary>
        /// 输入种族英文名,输出CardDB.Race枚举对象
        /// </summary>
        /// <param name="raceNameEn">种族英文名</param>
        /// <returns>CardDB.Race枚举对象</returns>
        public CardDB.Race raceNameStringToEnum(string raceNameEn)
        {
            CardDB.Race RaceEnum;
            if (Enum.TryParse<CardDB.Race>(raceNameEn, false, out RaceEnum)) return RaceEnum;
            else return CardDB.Race.BLANK;
        }

        /// <summary>
        /// 输入法术派系英文名,输出CardDB.SpellSchool枚举对象
        /// </summary>
        /// <param name="spellSchoolNameEn">法术派系英文名</param>
        /// <returns>CardDB.SpellSchool枚举对象</returns>
        public CardDB.SpellSchool spellSchoolNameStringToEnum(string spellSchoolNameEn)
        {
            CardDB.SpellSchool SpellSchoolEnum;
            if (Enum.TryParse<CardDB.SpellSchool>(spellSchoolNameEn, false, out SpellSchoolEnum)) return SpellSchoolEnum;
            else return CardDB.SpellSchool.PHYSICAL_COMBAT;
        }

        /// <summary>
        /// 根据卡牌 ID 获取对应的 SimTemplate 实例
        /// 包含英雄皮肤和异画幸运币的映射逻辑
        /// </summary>
        public static SimTemplate GetCardSimulation(CardDB.cardIDEnum tempCardIdEnum)
        {
            // 英雄皮肤映射：将各种皮肤映射到基础英雄
            switch (tempCardIdEnum)
            {
                // 战士皮肤
                case CardDB.cardIDEnum.VAN_HERO_01bp:
                case CardDB.cardIDEnum.CS2_102_H1:
                case CardDB.cardIDEnum.CS2_102_H2:
                case CardDB.cardIDEnum.CS2_102_H3:
                case CardDB.cardIDEnum.CS2_102_H4:
                case CardDB.cardIDEnum.HERO_01dbp:
                case CardDB.cardIDEnum.HERO_01fbp:
                case CardDB.cardIDEnum.VAN_CS2_102_H3:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_01bp;
                    break;
                // 萨满皮肤
                case CardDB.cardIDEnum.VAN_HERO_02bp:
                case CardDB.cardIDEnum.CS2_049_H1:
                case CardDB.cardIDEnum.CS2_049_H2:
                case CardDB.cardIDEnum.CS2_049_H3:
                case CardDB.cardIDEnum.CS2_049_H4:
                case CardDB.cardIDEnum.CS2_049_H5:
                case CardDB.cardIDEnum.HERO_02fbp:
                case CardDB.cardIDEnum.HERO_02mbp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_02bp;
                    break;
                // 潜行者皮肤
                case CardDB.cardIDEnum.CS2_083b_H1:
                case CardDB.cardIDEnum.CS2_083b_H2:
                case CardDB.cardIDEnum.HERO_03dbp:
                case CardDB.cardIDEnum.VAN_HERO_03bp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_03bp;
                    break;
                // 圣骑士皮肤
                case CardDB.cardIDEnum.HERO_04lbp:
                case CardDB.cardIDEnum.CS2_101_H1:
                case CardDB.cardIDEnum.CS2_101_H2:
                case CardDB.cardIDEnum.CS2_101_H3:
                case CardDB.cardIDEnum.CS2_101_H4:
                case CardDB.cardIDEnum.HERO_04fbp:
                case CardDB.cardIDEnum.HERO_04ebp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_04bp;
                    break;
                // 猎人皮肤
                case CardDB.cardIDEnum.VAN_HERO_05bp:
                case CardDB.cardIDEnum.DS1h_292_H1:
                case CardDB.cardIDEnum.DS1h_292_H2:
                case CardDB.cardIDEnum.DS1h_292_H3:
                case CardDB.cardIDEnum.HERO_05dbp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_05bp;
                    break;
                // 德鲁伊皮肤
                case CardDB.cardIDEnum.VAN_HERO_06bp:
                case CardDB.cardIDEnum.CS2_017_HS1:
                case CardDB.cardIDEnum.CS2_017_HS2:
                case CardDB.cardIDEnum.CS2_017_HS3:
                case CardDB.cardIDEnum.CS2_017_HS4:
                case CardDB.cardIDEnum.HERO_06ebp:
                case CardDB.cardIDEnum.HERO_06fbp:
                case CardDB.cardIDEnum.HERO_06zbp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_06bp;
                    break;
                // 术士皮肤
                case CardDB.cardIDEnum.VAN_HERO_07bp:
                case CardDB.cardIDEnum.CS2_056_H1:
                case CardDB.cardIDEnum.CS2_056_H2:
                case CardDB.cardIDEnum.CS2_056_H3:
                case CardDB.cardIDEnum.HERO_07dbp:
                case CardDB.cardIDEnum.HERO_07ebp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_07bp;
                    break;
                // 法师皮肤
                case CardDB.cardIDEnum.CS2_034_H1:
                case CardDB.cardIDEnum.CS2_034_H2:
                case CardDB.cardIDEnum.CS2_034_H3:
                case CardDB.cardIDEnum.CS2_034_H4:
                case CardDB.cardIDEnum.HERO_08ebp:
                case CardDB.cardIDEnum.HERO_08fbp:
                case CardDB.cardIDEnum.TRLA_Mage_01:
                case CardDB.cardIDEnum.VAN_HERO_08bp:
                case CardDB.cardIDEnum.HERO_08lbp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_08bp;
                    break;
                // 牧师皮肤
                case CardDB.cardIDEnum.CS1h_001_H1:
                case CardDB.cardIDEnum.CS1h_001_H2:
                case CardDB.cardIDEnum.CS1h_001_H3:
                case CardDB.cardIDEnum.HERO_09dbp:
                case CardDB.cardIDEnum.VAN_HERO_09bp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_09bp;
                    break;
                // 恶魔猎手皮肤
                case CardDB.cardIDEnum.HERO_10bbp:
                case CardDB.cardIDEnum.HERO_10bpe:
                case CardDB.cardIDEnum.HERO_10cbp:
                case CardDB.cardIDEnum.TB_HunterPrince_04:
                case CardDB.cardIDEnum.VAN_HERO_10bp:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_10bp;
                    break;
                // 死亡骑士皮肤
                case CardDB.cardIDEnum.HERO_11bp:
                case CardDB.cardIDEnum.HERO_11gbp:
                case CardDB.cardIDEnum.HERO_11cbp:
                case CardDB.cardIDEnum.HERO_11hbp:
                case CardDB.cardIDEnum.HERO_11ibp:
                case CardDB.cardIDEnum.HERO_11lbp:
                case CardDB.cardIDEnum.HERO_11ohp:
                case CardDB.cardIDEnum.HERO_11uhp:
                case CardDB.cardIDEnum.HERO_11vhp:
                case CardDB.cardIDEnum.HERO_11zhp:
                case CardDB.cardIDEnum.HERO_11aehp:
                case CardDB.cardIDEnum.HERO_11ajhp:
                case CardDB.cardIDEnum.HERO_11aihp:
                case CardDB.cardIDEnum.HERO_11q_LichKing:
                case CardDB.cardIDEnum.HERO_11s_Scarlet_hls:
                case CardDB.cardIDEnum.HERO_11v:
                case CardDB.cardIDEnum.HERO_11ab:
                case CardDB.cardIDEnum.HERO_11o_ReskathePitBoss:
                    tempCardIdEnum = CardDB.cardIDEnum.HERO_11bp;
                    break;
                // 异画幸运币
                case CardDB.cardIDEnum.DMF_COIN1:
                case CardDB.cardIDEnum.DMF_COIN2:
                case CardDB.cardIDEnum.LOOTA_BOSS_45p:
                case CardDB.cardIDEnum.BAR_COIN1:
                case CardDB.cardIDEnum.BAR_COIN2:
                case CardDB.cardIDEnum.BAR_COIN3:
                case CardDB.cardIDEnum.SW_COIN1:
                case CardDB.cardIDEnum.SW_COIN2:
                case CardDB.cardIDEnum.REV_COIN1:
                case CardDB.cardIDEnum.REV_COIN2:
                    tempCardIdEnum = CardDB.cardIDEnum.GAME_005;
                    break;
            }

            string className = "Sim_" + tempCardIdEnum.ToString();
            Type simType = GetTypeByName(className);
            if (simType != null)
            {
                return Activator.CreateInstance(simType) as SimTemplate;
            }
            return new SimTemplate(); // 无对应类时返回空模板
        }

        // 根据类名查找类型
        private static Type GetTypeByName(string className)
        {
            Type t;
            if (SimTypesDict.TryGetValue(className, out t))
            {
                return t;
            }
            return null;
        }
    }
}
