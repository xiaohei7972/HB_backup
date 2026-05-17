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
    // 致命与敌方模拟：致命伤害计算、敌方出牌模拟、陷阱模拟
    public partial class Playfield
    {
        /// <summary>
        /// 模拟敌方使用AOE（范围攻击）技能的场景，并评估对局势的影响。
        /// </summary>
        /// <param name="pprob">敌方第一种AOE技能的概率。</param>
        /// <param name="pprob2">敌方第二种AOE技能的概率。</param>
        public void enemyPlaysAoe(int pprob, int pprob2)
        {
            if (!this.loatheb)  // 如果未受洛欧塞布的影响
            {
                Playfield p = new Playfield(this);  // 创建当前场景的副本
                float oldval = Ai.Instance.botBase.getPlayfieldValue(p);  // 计算当前场景的价值
                p.value = int.MinValue;  // 设置场景值为最小值，避免误用

                // 模拟敌方使用AOE技能的效果
                p.EnemyCardPlaying(p.enemyHeroStartClass, p.mana, p.enemyAnzCards, pprob, pprob2);

                float newval = Ai.Instance.botBase.getPlayfieldValue(p);  // 计算敌方使用AOE后的场景价值
                p.value = int.MinValue;  // 再次设置场景值为最小值

                // 如果模拟后的场景对敌方更有利（价值更低）
                if (oldval > newval)
                {
                    this.copyValuesFrom(p);  // 更新当前场景为模拟后的场景
                }
            }
        }


        /// <summary>
        /// 根据敌方英雄职业、当前法力值、手牌数量和概率，模拟敌方使用AOE（范围攻击）或其他卡牌的行为。
        /// </summary>
        /// <param name="enemyHeroStrtClass">敌方英雄的职业。</param>
        /// <param name="currmana">当前敌方的法力值。</param>
        /// <param name="cardcount">敌方手中的卡牌数量。</param>
        /// <param name="playAroundProb">敌方使用卡牌的概率。</param>
        /// <param name="pap2">另一个影响敌方使用卡牌的概率参数。</param>
        /// <returns>模拟敌方使用卡牌后剩余的法力值。</returns>
        public int EnemyCardPlaying(TAG_CLASS enemyHeroStrtClass, int currmana, int cardcount, int playAroundProb, int pap2)
        {
            int mana = currmana;
            if (cardcount == 0) return currmana;  // 如果敌方手中没有卡牌，直接返回当前法力值

            bool useAOE = false;  // 是否使用AOE技能的标志
            int mobscount = 0;  // 己方随从数量统计
            foreach (Minion min in this.ownMinions)
            {
                if (min.maxHp >= 2 && min.Angr >= 2) mobscount++;  // 统计攻击力和生命值都大于等于2的己方随从
            }

            if (mobscount >= 3) useAOE = true;  // 如果己方随从数量达到3个或以上，敌方可能会使用AOE技能

            if (enemyHeroStrtClass == TAG_CLASS.WARRIOR)  // 如果敌方是战士职业
            {
                bool usewhirlwind = true;  // 是否使用旋风斩的标志
                foreach (Minion m in this.enemyMinions)
                {
                    if (m.Hp == 1) usewhirlwind = false;  // 如果敌方随从生命值为1，则不使用旋风斩
                }
                if (this.ownMinions.Count <= 3) usewhirlwind = false;  // 如果己方随从数量小于等于3，则不使用旋风斩

                if (usewhirlwind)
                {
                    mana = EnemyPlaysACard(CardDB.cardNameEN.whirlwind, mana, playAroundProb, pap2);  // 模拟敌方使用旋风斩，并更新法力值
                }
            }

            if (!useAOE) return mana;  // 如果不使用AOE，直接返回当前法力值

            // 根据敌方英雄的职业，模拟使用相应的AOE技能，并更新法力值
            switch (enemyHeroStrtClass)
            {
                case TAG_CLASS.MAGE:  // 法师
                    mana = EnemyPlaysACard(CardDB.cardNameEN.flamestrike, mana, playAroundProb, pap2);//烈焰风暴
                    mana = EnemyPlaysACard(CardDB.cardNameEN.blizzard, mana, playAroundProb, pap2);//暴风雪
                    break;
                case TAG_CLASS.HUNTER:  // 猎人
                    mana = EnemyPlaysACard(CardDB.cardNameEN.unleashthehounds, mana, playAroundProb, pap2);//放狗
                    break;
                case TAG_CLASS.PRIEST:  // 牧师
                    mana = EnemyPlaysACard(CardDB.cardNameEN.holynova, mana, playAroundProb, pap2);//神圣新星
                    break;
                case TAG_CLASS.SHAMAN:  // 萨满
                    mana = EnemyPlaysACard(CardDB.cardNameEN.lightningstorm, mana, playAroundProb, pap2);//闪电风暴
                    mana = EnemyPlaysACard(CardDB.cardNameEN.maelstromportal, mana, playAroundProb, pap2);//漩涡传送门
                    break;
                case TAG_CLASS.PALADIN:  // 圣骑士
                    mana = EnemyPlaysACard(CardDB.cardNameEN.consecration, mana, playAroundProb, pap2);//奉献
                    break;
                case TAG_CLASS.DRUID:  // 德鲁伊
                    mana = EnemyPlaysACard(CardDB.cardNameEN.swipe, mana, playAroundProb, pap2);//横扫
                    break;
            }

            return mana;  // 返回敌方使用卡牌后的剩余法力值
        }


        /// <summary>
        /// 模拟敌方使用指定的卡牌，并根据卡牌效果对己方和敌方的随从、英雄进行相应的操作，
        /// 最后返回剩余的法力值。
        /// </summary>
        /// <param name="cardname">敌方使用的卡牌名称。</param>
        /// <param name="currmana">敌方当前的法力值。</param>
        /// <param name="playAroundProb">敌方使用卡牌的概率。</param>
        /// <param name="pap2">另一个影响敌方使用卡牌的概率参数。</param>
        /// <returns>使用卡牌后的剩余法力值。</returns>
        public int EnemyPlaysACard(CardDB.cardNameEN cardname, int currmana, int playAroundProb, int pap2)
        {
            switch (cardname)
            {
                case CardDB.cardNameEN.flamestrike:  // 烈焰风暴
                    if (currmana >= 7)
                    {
                        if (wehaveCounterspell == 0)  // 如果没有反制法术
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CS2_032, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(5);  // 烈焰风暴造成5点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;  // 如果设置了dontkill标志，则随从的HP不会降到1以下
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;  // 如果有反制法术，则递增反制计数
                        currmana -= 7;  // 消耗7点法力值
                    }
                    break;

                case CardDB.cardNameEN.blizzard:  // 暴风雪
                    if (currmana >= 6)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CS2_028, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 暴风雪造成2点伤害并使随从冻结
                            foreach (Minion enemy in temp.ToArray())
                            {
                                minionGetFrozen(enemy);  // 使随从冻结
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 6;
                    }
                    break;

                case CardDB.cardNameEN.unleashthehounds:  // 放狗
                    if (currmana >= 3)  // 消耗3点法力值
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.EX1_538, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob)
                            {
                                dontkill = true;
                            }

                            int anz = this.ownMinions.Count;  // 己方随从的数量
                            int posi = this.enemyMinions.Count - 1;
                            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_538t);  // 召唤的狗
                            for (int i = 0; i < anz; i++)
                            {
                                callKid(kid, posi, false);  // 根据己方随从数量召唤狗
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.holynova:  // 神圣新星
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CS1_112, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.enemyMinions;
                            int heal = getEnemySpellHeal(2);  // 神圣新星对敌方随从和英雄治疗2点
                            int damage = getEnemySpellDamageDamage(2);  // 对己方随从和英雄造成2点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                this.minionGetDamageOrHeal(enemy, -heal);
                            }
                            this.minionGetDamageOrHeal(this.enemyHero, -heal);
                            temp = this.ownMinions;
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                            this.minionGetDamageOrHeal(this.ownHero, damage);
                        }
                        else wehaveCounterspell++;
                        currmana -= 5;
                    }
                    break;

                case CardDB.cardNameEN.lightningstorm:  // 闪电风暴
                    if (currmana >= 3)  // 消耗3点法力值
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.EX1_259, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(3);  // 闪电风暴造成3点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.maelstromportal:  // 漩涡传送门
                    if (currmana >= 2)  // 消耗2点法力值
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.KAR_073, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(1);  // 漩涡传送门造成1点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 2;
                    }
                    break;

                case CardDB.cardNameEN.whirlwind:  // 旋风斩
                    if (currmana >= 1)  // 消耗1点法力值
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.EX1_400, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.enemyMinions;
                            int damage = getEnemySpellDamageDamage(1);  // 旋风斩造成1点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                this.minionGetDamageOrHeal(enemy, damage);
                            }
                            temp = this.ownMinions;
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 1;
                    }
                    break;

                case CardDB.cardNameEN.consecration:  // 奉献
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CS2_093, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 奉献造成2点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }

                            this.minionGetDamageOrHeal(this.ownHero, damage);
                        }
                        else wehaveCounterspell++;
                        currmana -= 4;
                    }
                    break;

                case CardDB.cardNameEN.swipe:  // 横扫
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CS2_012, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            int damage4 = getEnemySpellDamageDamage(4);  // 横扫对一个目标造成4点伤害
                            int damage1 = getEnemySpellDamageDamage(1);  // 对其他目标造成1点伤害

                            List<Minion> temp = this.ownMinions;
                            Minion target = null;
                            foreach (Minion mnn in temp.ToArray())
                            {
                                if (mnn.Hp <= damage4 || mnn.handcard.card.isSpecialMinion || target == null)
                                {
                                    target = mnn;  // 选择主目标
                                }
                            }
                            foreach (Minion mnn in temp.ToArray())
                            {
                                mnn.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(mnn, mnn.entityID == target.entityID ? damage4 : damage1);
                                mnn.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 4;
                    }
                    break;
                case CardDB.cardNameEN.remorselesswinter:  // 冷酷严冬
                    if (currmana >= 4)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.RLK_709, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 冷酷严冬造成2点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 4;
                    }
                    break;

                case CardDB.cardNameEN.starfall:  // 星辰坠落
                    if (currmana >= 5)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.VAN_NEW1_007, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 星辰坠落造成2点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 5;
                    }
                    break;

                case CardDB.cardNameEN.volcanicpotion:  // 火山药水
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CFM_065, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 火山药水造成2点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.firesale:  // 火热促销
                    if (currmana >= 4)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.SW_107, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(3);  // 火热促销造成3点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 4;
                    }
                    break;

                case CardDB.cardNameEN.risingwaves:  // 浪潮涌起
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.VAC_953, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(2);  // 浪潮涌起初始造成2点伤害
                            bool anyMinionDied = false;
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                if (enemy.Hp <= 0)
                                {
                                    anyMinionDied = true;
                                }
                                enemy.cantLowerHPbelowONE = false;
                            }
                            if (!anyMinionDied)
                            {
                                foreach (Minion enemy in temp.ToArray())
                                {
                                    this.minionGetDamageOrHeal(enemy, damage);  // 如果没有随从死亡，再造成2点伤害
                                }
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.spiritlash:  // 灵魂鞭笞
                    if (currmana >= 2)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.ICC_802, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(1);  // 灵魂鞭笞造成1点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                this.minionGetDamageOrHeal(this.enemyHero, -damage);  // 对敌方英雄造成治疗
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 2;
                    }
                    break;

                case CardDB.cardNameEN.dragonfirepotion:  // 龙息药水
                    if (currmana >= 6)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CFM_662, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(5);  // 龙息药水造成5点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                if (!enemy.handcard.card.race.Equals(TAG_RACE.DRAGON))  // 如果随从不是龙族，则造成伤害
                                {
                                    enemy.cantLowerHPbelowONE = dontkill;
                                    this.minionGetDamageOrHeal(enemy, damage);
                                    enemy.cantLowerHPbelowONE = false;
                                }
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 6;
                    }
                    break;

                case CardDB.cardNameEN.fanofknives:  // 刀扇
                    if (currmana >= 2)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.EX1_129, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(1);  // 刀扇造成1点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.elementaldestruction:  // 元素毁灭
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.AT_051, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            Random rng = new Random();
                            int damage = rng.Next(4, 6);  // 元素毁灭造成4到5点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;

                case CardDB.cardNameEN.hellfire:  // 地狱烈焰
                    if (currmana >= 3)
                    {
                        if (wehaveCounterspell == 0)
                        {
                            bool dontkill = false;
                            int prob = Probabilitymaker.Instance.getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum.CORE_CS2_062, this.enemyAnzCards, this.enemyDeckSize);
                            if (playAroundProb > prob) return currmana;
                            if (pap2 > prob) dontkill = true;

                            List<Minion> temp = this.ownMinions;
                            int damage = getEnemySpellDamageDamage(3);  // 地狱烈焰造成3点伤害
                            foreach (Minion enemy in temp.ToArray())
                            {
                                enemy.cantLowerHPbelowONE = dontkill;
                                this.minionGetDamageOrHeal(enemy, damage);
                                enemy.cantLowerHPbelowONE = false;
                            }
                            this.minionGetDamageOrHeal(this.ownHero, damage);
                            this.minionGetDamageOrHeal(this.enemyHero, damage);
                        }
                        else wehaveCounterspell++;
                        currmana -= 3;
                    }
                    break;
            }
            return currmana;  // 返回使用卡牌后的剩余法力值
        }

        /// <summary>
        /// 猜测敌方英雄血量丢失
        /// </summary>
        /// <returns></returns>
        public int guessEnemyHeroLethalMissing()
        {
            int lethalMissing = this.enemyHero.armor + this.enemyHero.Hp;
            if (this.anzEnemyTaunt == 0)
            {
                foreach (Minion m in this.ownMinions)
                {
                    if (m.Ready)
                    {
                        lethalMissing -= m.Angr;
                        if (m.windfury && m.numAttacksThisTurn == 0) lethalMissing -= m.Angr;
                    }
                }
            }
            return lethalMissing;
        }

        /// <summary>
        /// 随机伤害
        /// </summary>
        public void guessHeroDamage()
        {
            int ghd = 0;
            int ablilityDmg = 0;
            switch (this.enemyHeroAblility.card.cardIDenum)
            {
                //direct damage直伤
                case CardDB.cardIDEnum.HERO_05bp: ablilityDmg += 2; break;//猎人技能
                case CardDB.cardIDEnum.DS1h_292_H1: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.HERO_05bp2: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.DS1h_292_H1_AT_132: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.NAX15_02: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.NAX15_02H: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.NAX6_02: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.NAX6_02H: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.HERO_08bp: ablilityDmg += 1; break;
                case CardDB.cardIDEnum.CS2_034_H1: ablilityDmg += 1; break;
                case CardDB.cardIDEnum.CS2_034_H2: ablilityDmg += 1; break;
                case CardDB.cardIDEnum.AT_050t: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.HERO_08bp2: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.CS2_034_H1_AT_132: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.CS2_034_H2_AT_132: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.EX1_625t: ablilityDmg += 2; break;
                case CardDB.cardIDEnum.EX1_625t2: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.TU4d_003: ablilityDmg += 1; break;
                case CardDB.cardIDEnum.NAX7_03: ablilityDmg += 3; break;
                case CardDB.cardIDEnum.NAX7_03H: ablilityDmg += 4; break;
                case CardDB.cardIDEnum.ICC_830p: ablilityDmg += 2; break;//牧师dk技能
                case CardDB.cardIDEnum.ICC_831p: ablilityDmg += 3; break;//术士dk 技能
                case CardDB.cardIDEnum.ICC_833h: ablilityDmg += 1; break;//法师 dk技能
                //condition 有条件的
                case CardDB.cardIDEnum.BRMA05_2H: if (this.mana > 0) ablilityDmg += 10; break;
                case CardDB.cardIDEnum.BRMA05_2: if (this.mana > 0) ablilityDmg += 5; break;
                case CardDB.cardIDEnum.BRM_027p: if (this.ownMinions.Count < 1) ablilityDmg += 8; break;
                case CardDB.cardIDEnum.BRM_027pH: if (this.ownMinions.Count < 2) ablilityDmg += 8; break;
                case CardDB.cardIDEnum.TB_MechWar_Boss2_HeroPower: if (this.ownMinions.Count < 2) ablilityDmg += 1; break;
                //equip weapon 装备武器
                case CardDB.cardIDEnum.LOEA09_2: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) ghd += 2; break;
                case CardDB.cardIDEnum.LOEA09_2H: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) ghd += 5; break;
                case CardDB.cardIDEnum.HERO_03bp: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) ghd += 1; break;
                case CardDB.cardIDEnum.CS2_083b_H1: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) ghd += 1; break;
                case CardDB.cardIDEnum.HERO_03bp2: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) ghd += 2; break;
                case CardDB.cardIDEnum.HERO_06bp: if (!this.enemyHero.frozen) ghd += 1; break;
                case CardDB.cardIDEnum.HERO_06bp2: if (!this.enemyHero.frozen) ghd += 2; break;
                case CardDB.cardIDEnum.ICC_832p: if (!this.enemyHero.frozen) ghd += 3; break;
            }

            ghd += ablilityDmg;
            foreach (Minion m in this.enemyMinions)
            {
                if (m.frozen) continue;
                switch (m.name)
                {
                    case CardDB.cardNameEN.ancientwatcher: if (!m.silenced) continue; break;
                    case CardDB.cardNameEN.blackknight: if (!m.silenced) continue; break;
                    case CardDB.cardNameEN.whiteknight: if (!m.silenced) continue; break;
                    case CardDB.cardNameEN.humongousrazorleaf: if (!m.silenced) continue; break;
                }
                ghd += m.Angr;//总伤害
                if (m.windfury) ghd += m.Angr;
            }

            if (this.enemyWeapon.Durability > 0 && !this.enemyHero.frozen)
            {
                ghd += this.enemyWeapon.Angr;
                if (this.enemyHero.windfury && this.enemyWeapon.Durability > 1) ghd += this.enemyWeapon.Angr;
            }

            foreach (Minion m in this.ownMinions)
            {
                if (m.taunt) ghd -= m.Hp;
                if (m.taunt && m.divineShield) ghd -= 1;
            }

            int guessingHeroDamage = Math.Max(0, ghd);
            if (this.ownHero.immune) guessingHeroDamage = 0;
            this.guessingHeroHP = this.ownHero.Hp + this.ownHero.armor - guessingHeroDamage;

            bool haveImmune = false;
            if (this.guessingHeroHP < 1 && this.ownSecretsIDList.Count > 0)//场面伤害我死了的时候,有奥秘,计算能挡的伤害
            {
                foreach (CardDB.cardIDEnum secretID in this.ownSecretsIDList)
                {
                    switch (secretID)
                    {//能抗上伤害的奥秘
                        case CardDB.cardIDEnum.EX1_130: //Noble Sacrifice
                            if (this.enemyMinions.Count > 0)
                            {
                                int mAngr = 1000;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (!m.frozen && m.Angr < mAngr && m.Angr > 0) mAngr = m.Angr; //take the weakest
                                }
                                if (mAngr != 1000) this.guessingHeroHP += mAngr;
                            }
                            continue;
                        case CardDB.cardIDEnum.EX1_533: //Misdirection误导
                            if (this.enemyMinions.Count > 0)
                            {
                                int mAngr = 1000;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (!m.frozen && m.Angr < mAngr && m.Angr > 0) mAngr = m.Angr; //take the weakest
                                }
                                if (mAngr != 1000) this.guessingHeroHP += mAngr;
                            }
                            continue;
                        case CardDB.cardIDEnum.AT_060: //Bear Trap
                            if (this.enemyMinions.Count > 1) this.guessingHeroHP += 3;
                            continue;
                        case CardDB.cardIDEnum.EX1_611: //Freezing Trap
                            if (this.enemyMinions.Count > 0)
                            {
                                int mAngr = 1000;
                                int mCharge = 0;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (!m.frozen && m.Angr < mAngr && m.Angr > 0)
                                    {
                                        mAngr = m.Angr; //take the weakest
                                        mCharge = m.charge;
                                    }
                                }
                                if (mAngr < 1000 && mCharge < 1) this.guessingHeroHP += mAngr;
                            }
                            continue;
                        case CardDB.cardIDEnum.EX1_289: //Ice Barrier 寒冰护体
                            this.guessingHeroHP += 8;
                            continue;
                        case CardDB.cardIDEnum.EX1_295: //Ice Block
                            haveImmune = true;
                            break;
                        case CardDB.cardIDEnum.EX1_594: //Vaporize
                            if (this.enemyMinions.Count > 0)
                            {
                                int mAngr = 1000;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (!m.frozen && m.Angr < mAngr && m.Angr > 0) mAngr = m.Angr; //take the weakest
                                }
                                if (mAngr != 1000) this.guessingHeroHP += mAngr;
                            }
                            continue;
                        case CardDB.cardIDEnum.CORE_EX1_610: //Explosive Trap
                        case CardDB.cardIDEnum.VAN_EX1_610: //Explosive Trap
                        case CardDB.cardIDEnum.EX1_610: //Explosive Trap
                            if (this.enemyMinions.Count > 0)
                            {
                                int losshd = 0;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (m.frozen) continue;
                                    switch (m.name)
                                    {
                                        case CardDB.cardNameEN.ancientwatcher: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.blackknight: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.whiteknight: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.humongousrazorleaf: if (!m.silenced) continue; break;
                                    }
                                    if (m.Hp < 3)
                                    {
                                        losshd += m.Angr;
                                        if (m.windfury) losshd += m.Angr;
                                    }
                                }
                                this.guessingHeroHP += losshd;
                            }
                            continue;
                        //此处可加火焰陷阱
                        case CardDB.cardIDEnum.ULD_239: //Explosive Trap
                            if (this.enemyMinions.Count > 0)
                            {
                                int losshd = 0;
                                int maxAngr = 0;
                                foreach (Minion m in this.enemyMinions)
                                {
                                    if (m.frozen) continue;
                                    switch (m.name)
                                    {
                                        case CardDB.cardNameEN.ancientwatcher: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.blackknight: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.whiteknight: if (!m.silenced) continue; break;
                                        case CardDB.cardNameEN.humongousrazorleaf: if (!m.silenced) continue; break;
                                    }
                                    if (m.Hp < 4)
                                    {
                                        losshd += m.Angr;
                                        if (maxAngr < m.Angr) maxAngr = m.Angr;
                                        if (m.windfury) losshd += m.Angr;
                                    }
                                }
                                this.guessingHeroHP += losshd - maxAngr;
                            }
                            continue;
                    }
                }
                if (haveImmune && this.guessingHeroHP < 2) this.guessingHeroHP = 2;
            }
            if (this.ownHero.Hp + this.ownHero.armor <= ablilityDmg && !haveImmune) this.guessingHeroHP = this.ownHero.Hp + this.ownHero.armor - ablilityDmg;
        }

        /// <summary>
        /// 我方被斩杀的可能性
        /// </summary>
        /// <returns></returns>
        public bool ownHeroHasDirectLethal()
        {
            //fastLethalCheck
            if (this.anzOwnTaunt != 0) return false;
            if (this.ownHero.immune) return false;
            int totalEnemyDamage = 0;
            foreach (Minion m in this.enemyMinions)
            {
                if (!m.frozen && !m.cantAttack)
                {
                    switch (m.name)
                    {
                        case CardDB.cardNameEN.icehowl: if (!m.silenced) continue; break;
                    }
                    totalEnemyDamage += m.Angr;
                    if (m.windfury) totalEnemyDamage += m.Angr;
                }
            }

            if (this.enemyAbilityReady)
            {
                switch (this.enemyHeroAblility.card.cardIDenum)
                {
                    //direct damage
                    case CardDB.cardIDEnum.HERO_05bp: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.DS1h_292_H1: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.HERO_05bp2: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.DS1h_292_H1_AT_132: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.NAX15_02: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.NAX15_02H: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.NAX6_02: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.NAX6_02H: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.HERO_08bp: totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.CS2_034_H1: totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.CS2_034_H2: totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.AT_050t: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.HERO_08bp2: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.CS2_034_H1_AT_132: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.CS2_034_H2_AT_132: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.EX1_625t: totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.EX1_625t2: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.TU4d_003: totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.NAX7_03: totalEnemyDamage += 3; break;
                    case CardDB.cardIDEnum.NAX7_03H: totalEnemyDamage += 4; break;
                    //condition
                    case CardDB.cardIDEnum.BRMA05_2H: if (this.mana > 0) totalEnemyDamage += 10; break;
                    case CardDB.cardIDEnum.BRMA05_2: if (this.mana > 0) totalEnemyDamage += 5; break;
                    case CardDB.cardIDEnum.BRM_027p: if (this.ownMinions.Count < 1) totalEnemyDamage += 8; break;
                    case CardDB.cardIDEnum.BRM_027pH: if (this.ownMinions.Count < 2) totalEnemyDamage += 8; break;
                    case CardDB.cardIDEnum.TB_MechWar_Boss2_HeroPower: if (this.ownMinions.Count < 2) totalEnemyDamage += 1; break;
                    //equip weapon
                    case CardDB.cardIDEnum.LOEA09_2: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.LOEA09_2H: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 5; break;
                    case CardDB.cardIDEnum.HERO_06bp: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.HERO_03bp: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.CS2_083b_H1: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 1; break;
                    case CardDB.cardIDEnum.HERO_03bp2: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 2; break;
                    case CardDB.cardIDEnum.HERO_06bp2: if (this.enemyWeapon.Durability < 1 && !this.enemyHero.frozen) totalEnemyDamage += 2; break;
                }
            }
            if (this.enemyWeapon.Durability > 0 && this.enemyHero.Ready && !this.enemyHero.frozen)
            {
                totalEnemyDamage += this.enemyWeapon.Angr;
                if (this.enemyHero.windfury && this.enemyWeapon.Durability > 1) totalEnemyDamage += this.enemyWeapon.Angr;
            }

            if (totalEnemyDamage < this.ownHero.Hp + this.ownHero.armor) return false;
            if (this.ownSecretsIDList.Count > 0)
            {
                foreach (CardDB.cardIDEnum secretID in this.ownSecretsIDList)
                {
                    switch (secretID)
                    {
                        case CardDB.cardIDEnum.EX1_289: //Ice Barrier 寒冰护体
                            totalEnemyDamage -= 8;
                            continue;
                        case CardDB.cardIDEnum.EX1_295: //Ice Block
                            return false;
                        case CardDB.cardIDEnum.EX1_130: //Noble Sacrifice
                            return false;
                        case CardDB.cardIDEnum.EX1_533: //Misdirection
                            return false;
                        case CardDB.cardIDEnum.EX1_594: //Vaporize
                            return false;
                        case CardDB.cardIDEnum.EX1_611: //Freezing Trap
                            return false;
                        case CardDB.cardIDEnum.EX1_610: //Explosive Trap
                            return false;
                        case CardDB.cardIDEnum.AT_060: //Bear Trap
                            return false;
                        case CardDB.cardIDEnum.EX1_132: //Eye for an Eye
                            if ((this.enemyHero.Hp + this.enemyHero.armor) <= (this.ownHero.Hp + this.ownHero.armor) && !this.enemyHero.immune) return false;
                            continue;
                        case CardDB.cardIDEnum.LOE_021: //Dart Trap
                            if ((this.enemyHero.Hp + this.enemyHero.armor) < 6 && !this.enemyHero.immune) return false;
                            continue;
                    }
                }
            }
            if (totalEnemyDamage < this.ownHero.Hp + this.ownHero.armor) return false;
            return true;
        }

        /// <summary>
        /// 在敌方回合开始时模拟触发我方奥秘
        /// </summary>
        public void simulateTrapsStartEnemyTurn()
        {
            // DONT KILL ENEMY HERO (cause its only guessing)

            List<CardDB.cardIDEnum> tmpSecretsIDList = new List<CardDB.cardIDEnum>();
            List<Minion> temp;
            int pos;

            foreach (CardDB.cardIDEnum secretID in this.ownSecretsIDList)
            {
                switch (secretID)
                {
                    // 火焰结界：如果敌方怪兽属性值总和大于我方，则破坏我方场上所有怪兽，对敌方场上所有怪兽造成 3 点伤害

                    case CardDB.cardIDEnum.EX1_554: //snaketrap

                        pos = this.ownMinions.Count;
                        if (pos == 0) continue;
                        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_554t);//snake
                        callKid(kid, pos, true, false);
                        callKid(kid, pos, true);
                        callKid(kid, pos, true);
                        continue;
                    case CardDB.cardIDEnum.EX1_610: //explosive trap

                        temp = new List<Minion>(this.enemyMinions);
                        int damage = getSpellDamageDamage(2);
                        foreach (Minion m in temp)
                        {
                            minionGetDamageOrHeal(m, damage);
                        }
                        attackEnemyHeroWithoutKill(damage);
                        continue;
                    case CardDB.cardIDEnum.EX1_611: //freezing trap
                        {

                            int count = this.enemyMinions.Count;
                            if (count == 0) continue;
                            Minion mnn = this.enemyMinions[0];
                            for (int i = 1; i < count; i++)
                            {
                                if (this.enemyMinions[i].Angr < mnn.Angr) mnn = this.enemyMinions[i]; //take the weakest
                            }
                            minionReturnToHand(mnn, false, 0);
                            continue;
                        }
                    case CardDB.cardIDEnum.AT_060: //beartrap

                        if (this.enemyMinions.Count == 0 && ((this.enemyWeapon.Angr == 0 && !prozis.penman.HeroPowerEquipWeapon.ContainsKey(this.enemyHeroAblility.card.nameEN)) || this.enemyHero.frozen)) continue;
                        pos = this.ownMinions.Count;
                        callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_125), pos, true, false);
                        continue;
                    case CardDB.cardIDEnum.LOE_021: //Dart Trap

                        minionGetDamageOrHeal(this.enemyHero, getSpellDamageDamage(5), true);
                        continue;
                    case CardDB.cardIDEnum.EX1_533: // misdirection
                        {


                            int count = this.enemyMinions.Count;
                            if (count == 0 && ((this.enemyWeapon.Angr == 0 && !prozis.penman.HeroPowerEquipWeapon.ContainsKey(this.enemyHeroAblility.card.nameEN)) || this.enemyHero.frozen)) continue;
                            Minion mnn = this.enemyMinions[0];
                            for (int i = 1; i < count; i++)
                            {
                                if (this.enemyMinions[i].Angr > mnn.Angr) mnn = this.enemyMinions[i]; //take the strongest
                            }
                            mnn.Angr = 0;
                            //this.evaluatePenality -= this.enemyMinions.Count;// Todo: 不在这里引入打分 the more the enemy minions has on board, the more the posibility to destroy something other :D
                            continue;
                        }
                    case CardDB.cardIDEnum.KAR_004: //cattrick

                        pos = this.ownMinions.Count;
                        callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_017), pos, true, false);
                        continue;


                    case CardDB.cardIDEnum.EX1_287: //counterspell

                        wehaveCounterspell++;
                        continue;
                    case CardDB.cardIDEnum.EX1_289: //ice barrier  寒冰护体

                        if (this.enemyMinions.Count == 0 && ((this.enemyWeapon.Angr == 0 && !prozis.penman.HeroPowerEquipWeapon.ContainsKey(this.enemyHeroAblility.card.nameEN)) || this.enemyHero.frozen)) continue;
                        this.ownHero.armor += 8;
                        continue;
                    case CardDB.cardIDEnum.EX1_295: //ice block

                        guessHeroDamage();
                        if (guessingHeroHP < 11) this.ownHero.immune = true;
                        continue;
                    case CardDB.cardIDEnum.EX1_294: //mirror entity

                        if (this.ownMinions.Count < 7)
                        {
                            pos = this.ownMinions.Count - 1;
                            callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TU4f_007), pos, true, false);
                        }
                        else goto default;
                        continue;
                    case CardDB.cardIDEnum.AT_002: //effigy

                        if (this.ownMinions.Count == 0) continue;
                        pos = this.ownMinions.Count - 1;
                        callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TU4f_007), pos, true);
                        continue;
                    case CardDB.cardIDEnum.tt_010: //spellbender

                        //this.evaluatePenality -= 4;
                        continue;
                    case CardDB.cardIDEnum.EX1_594: // vaporize
                        {

                            int count = this.enemyMinions.Count;
                            if (count == 0) continue;
                            Minion mnn = this.enemyMinions[0];
                            for (int i = 1; i < count; i++)
                            {
                                if (this.enemyMinions[i].Angr < mnn.Angr) mnn = this.enemyMinions[i]; //take the weakest
                            }
                            minionGetDestroyed(mnn);
                            continue;
                        }
                    case CardDB.cardIDEnum.FP1_018: // duplicate
                        {

                            int count = this.ownMinions.Count;
                            if (count == 0) continue;
                            Minion mnn = this.ownMinions[0];
                            for (int i = 1; i < count; i++)
                            {
                                if (this.ownMinions[i].Angr < mnn.Angr) mnn = this.ownMinions[i]; //take the weakest
                            }
                            drawACard(mnn.name, true);
                            drawACard(mnn.name, true);
                            continue;
                        }




                    case CardDB.cardIDEnum.EX1_132: // eye for an eye
                        {

                            // todo for mage and hunter
                            if (this.enemyHero.frozen && this.enemyMinions.Count == 0) continue;
                            int dmg = 0;
                            int dmgW = 0;

                            int count = this.enemyMinions.Count;
                            if (count != 0)
                            {
                                Minion mnn = this.enemyMinions[0];
                                for (int i = 1; i < count; i++)
                                {
                                    if (this.enemyMinions[i].Angr < mnn.Angr) mnn = this.enemyMinions[i]; //take the weakest
                                }
                                dmg = mnn.Angr;
                            }
                            if (this.enemyWeapon.Angr != 0) dmgW = this.enemyWeapon.Angr;
                            else if (prozis.penman.HeroPowerEquipWeapon.ContainsKey(this.enemyHeroAblility.card.nameEN)) dmgW = prozis.penman.HeroPowerEquipWeapon[this.enemyHeroAblility.card.nameEN];
                            if (dmgW != 0)
                            {
                                if (dmg != 0)
                                {
                                    if (dmgW < dmg) dmg = dmgW;
                                }
                                else dmg = dmgW;
                            }
                            dmg = getSpellDamageDamage(dmg);
                            if (dmg != 0) attackEnemyHeroWithoutKill(dmg);
                            continue;
                        }
                    case CardDB.cardIDEnum.EX1_130: // noble sacrifice

                        if (this.enemyMinions.Count == 0 && ((this.enemyWeapon.Angr == 0 && !prozis.penman.HeroPowerEquipWeapon.ContainsKey(this.enemyHeroAblility.card.nameEN)) || this.enemyHero.frozen)) continue;
                        if (this.ownMinions.Count == 7) continue;
                        pos = this.ownMinions.Count - 1;
                        callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_097), pos, true, false);
                        continue;
                    case CardDB.cardIDEnum.EX1_136: // redemption


                        if (this.ownMinions.Count == 0) continue;
                        temp = new List<Minion>(this.ownMinions);
                        temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));
                        foreach (Minion m in temp)
                        {
                            if (m.divineShield) continue;
                            m.divineShield = true;
                            break;
                        }
                        continue;
                    case CardDB.cardIDEnum.FP1_020: // avenge


                        if (this.ownMinions.Count < 2 || (this.ownMinions.Count == 1 && !this.ownSecretsIDList.Contains(CardDB.cardIDEnum.EX1_130))) continue;
                        temp = new List<Minion>(this.ownMinions);
                        temp.Sort((a, b) => a.Hp.CompareTo(b.Hp));
                        minionGetBuffed(temp[0], 3, 2);
                        continue;
                    default:
                        tmpSecretsIDList.Add(secretID);
                        continue;
                }
            }
            this.ownSecretsIDList.Clear();
            this.ownSecretsIDList.AddRange(tmpSecretsIDList);

            this.doDmgTriggers();
        }

        /// <summary>
        /// 模拟敌方回合结束时触发己方的陷阱（奥秘）效果，并处理相应的随从和英雄状态。
        /// </summary>
        public void simulateTrapsEndEnemyTurn()
        {
            // 不杀死敌方英雄（因为这是一个推测性的模拟）
            List<CardDB.cardIDEnum> tmpSecretsIDList = new List<CardDB.cardIDEnum>(); // 用于存储未触发的奥秘
            List<Minion> temp;

            bool activate = false;
            foreach (CardDB.cardIDEnum secretID in this.ownSecretsIDList)
            {
                switch (secretID)
                {
                    case CardDB.cardIDEnum.EX1_609: // 狙击
                        activate = false;
                        if (this.enemyMinions.Count > 0)
                        {
                            temp = new List<Minion>(this.enemyMinions);
                            int damage = getSpellDamageDamage(4); // 狙击造成4点伤害
                            foreach (Minion m in temp)
                            {
                                if (m.playedThisTurn) // 如果随从在本回合中被召唤
                                {
                                    minionGetDamageOrHeal(m, damage); // 对该随从造成伤害
                                    activate = true;
                                    break;
                                }
                            }
                        }
                        if (!activate) tmpSecretsIDList.Add(secretID); // 如果未触发，将奥秘重新加入列表
                        continue;

                    case CardDB.cardIDEnum.EX1_379: // 忏悔
                        activate = false;
                        if (this.enemyMinions.Count > 0)
                        {
                            temp = new List<Minion>(this.enemyMinions);
                            foreach (Minion m in temp)
                            {
                                if (m.playedThisTurn) // 如果随从在本回合中被召唤
                                {
                                    m.Hp = 1; // 将该随从的生命值设为1
                                    m.maxHp = 1; // 将该随从的最大生命值设为1
                                    activate = true;
                                    break;
                                }
                            }
                        }
                        if (!activate) tmpSecretsIDList.Add(secretID); // 如果未触发，将奥秘重新加入列表
                        continue;

                    case CardDB.cardIDEnum.LOE_027: // 圣殿执行者的考验
                        activate = false;
                        if (this.enemyMinions.Count > 3) // 如果敌方随从数量超过3个
                        {
                            temp = new List<Minion>(this.enemyMinions);
                            foreach (Minion m in temp)
                            {
                                if (m.playedThisTurn) // 如果随从在本回合中被召唤
                                {
                                    this.minionGetDestroyed(m); // 摧毁该随从
                                    activate = true;
                                    break;
                                }
                            }
                        }
                        if (!activate) tmpSecretsIDList.Add(secretID); // 如果未触发，将奥秘重新加入列表
                        continue;

                    case CardDB.cardIDEnum.AT_073: // 竞争精神
                        if (this.enemyMinions.Count == 0) continue; // 如果没有敌方随从，则跳过
                        foreach (Minion m in this.ownMinions)
                        {
                            minionGetBuffed(m, 1, 1); // 增加己方所有随从的攻击力和生命值
                        }
                        continue;

                    default:
                        tmpSecretsIDList.Add(secretID); // 其他未触发的奥秘重新加入列表
                        continue;
                }
            }

            this.ownSecretsIDList.Clear(); // 清空当前的奥秘列表
            this.ownSecretsIDList.AddRange(tmpSecretsIDList); // 重新添加未触发的奥秘

            this.doDmgTriggers(); // 触发伤害相关的效果
        }

        /// <summary>
        /// 处理敌方打出一张卡牌的逻辑，包括触发相关效果、目标选择和奥秘触发等。
        /// </summary>
        /// <param name="c">敌方打出的卡牌。</param>
        /// <param name="target">卡牌的目标随从或英雄。</param>
        /// <param name="position">随从放置的位置。</param>
        /// <param name="choice">卡牌的选择参数（用于二选一或类似效果）。</param>
        /// <param name="penality">执行该操作的惩罚值。</param>
        public void EnemyplaysACard(CardDB.Card c, Minion target, int position, int choice, int penality)
        {
            // 创建一个新的手牌实例，表示敌方打出的卡牌
            Handmanager.Handcard hc = new Handmanager.Handcard(c);
            hc.entity = this.getNextEntity(); // 获取下一个实体ID
            if (logging) Helpfunctions.Instance.logg("enemy plays card " + c.nameEN + " target " + target);

            this.enemyAnzCards--; // 敌方手牌数量减少

            hc.target = target; // 设置目标
            this.triggerACardWillBePlayed(hc, false); // 触发卡牌即将被打出的事件
            this.triggerCardsChanged(false); // 触发手牌变化的事件

            // 检查并触发奥秘
            int newTarget = secretTrigger_SpellIsPlayed(target, c);
            target = EnemyUpdateTargetBasedOnSecret(newTarget, target);

            if (newTarget != -2) // 如果奥秘不是法术反制或扰咒术，执行卡牌效果
            {
                if (c.type == CardDB.cardtype.MOB) // 处理随从卡牌
                {
                    EnemyHandleEnemyMinionPlay(hc, target, position, choice);
                }
                else // 处理法术卡牌
                {
                    c.sim_card.onCardPlay(this, false, target, choice, hc); // 执行卡牌的主要效果
                    this.doDmgTriggers(); // 触发伤害效果
                }
            }
        }

        /// <summary>
        /// 敌方 - 处理敌方随从卡牌的打出逻辑。
        /// </summary>
        /// <param name="hc">手牌信息。</param>
        /// <param name="target">卡牌的目标。</param>
        /// <param name="position">随从放置的位置。</param>
        /// <param name="choice">卡牌的选择参数。</param>
        private void EnemyHandleEnemyMinionPlay(Handmanager.Handcard hc, Minion target, int position, int choice)
        {
            // TODO: 实现敌方随从的打出逻辑
            // 示例：this.placeAmobSomewhere(hc, target, choice, position);
        }

        /// <summary>
        /// 计算己方缺少多少伤害可以达成致命一击（即击败敌方英雄）。
        /// </summary>
        /// <returns>返回缺少的伤害值，如果为负数或零表示可以达成致命一击。</returns>
        public int lethalMissing()
        {
            // 如果之前已经计算过缺少的伤害并且该值小于1000，直接返回
            if (this.lethlMissing < 1000) return lethlMissing;

            // 从AI实例中获取当前致命一击缺少的伤害值
            lethlMissing = Ai.Instance.lethalMissing;

            // 如果缺少的伤害值大于5，直接返回
            if (lethlMissing > 5) return lethlMissing;

            int dmg = 0;

            // 如果敌方没有嘲讽随从，计算所有己方随从的伤害
            if (this.anzEnemyTaunt == 0)
            {
                foreach (Minion m in this.ownMinions)
                {
                    if (!m.Ready || m.frozen) continue; // 跳过无法攻击或被冻结的随从
                    dmg += m.Angr; // 加上随从的攻击力
                    if (m.windfury) dmg += m.Angr; // 如果随从有风怒，额外加一次攻击力
                }
            }
            else
            {
                // 如果敌方有嘲讽随从，逐个考虑如何突破嘲讽
                List<Minion> om = new List<Minion>(this.ownMinions); // 己方随从列表
                List<Minion> omn = new List<Minion>(); // 用于暂时存储不能突破嘲讽的随从
                Minion bm = null; // 用于存储能够突破嘲讽的最佳随从
                int tmp = 0;

                foreach (Minion d in this.enemyMinions)
                {
                    if (!d.taunt) continue; // 跳过非嘲讽随从
                    bm = null;

                    // 找出能够突破当前嘲讽随从的己方随从
                    foreach (Minion m in om)
                    {
                        if (!m.Ready || m.frozen) continue;

                        if (m.Angr < d.Hp)
                        {
                            omn.Add(m); // 如果随从的攻击力不足以击败嘲讽随从，暂时存入omn列表
                        }
                        else
                        {
                            if (bm == null)
                            {
                                bm = m; // 如果没有最佳随从，当前随从为最佳选择
                            }
                            else
                            {
                                if (m.Angr < bm.Angr)
                                {
                                    omn.Add(bm); // 如果当前随从的攻击力比最佳随从低，交换
                                    bm = m;
                                }
                                else
                                {
                                    omn.Add(m); // 否则将当前随从加入到暂存列表
                                }
                            }
                        }
                    }

                    // 如果没有找到突破当前嘲讽随从的随从，退出循环
                    if (bm == null)
                    {
                        dmg = 0;
                        tmp = 0;
                        break;
                    }

                    tmp++;
                    om.Clear();
                    om.AddRange(omn);
                    omn.Clear();
                }

                // 如果能够突破所有嘲讽随从，计算剩余随从的总伤害
                if (tmp >= this.anzEnemyTaunt)
                {
                    foreach (Minion m in om)
                    {
                        dmg += m.Angr;
                        if (m.windfury) dmg += m.Angr; // 如果随从有风怒，额外加一次攻击力
                    }
                }
            }

            // 计算缺少的伤害值，敌方英雄的当前血量减去己方的总伤害
            lethlMissing = this.enemyHero.Hp - dmg;

            return lethlMissing;
        }

        /// <summary>
        /// 判断下一回合是否能够获胜。此方法考虑了己方随从、法术以及敌方嘲讽等因素。
        /// </summary>
        /// <returns>如果能够在下一回合获胜，则返回true；否则返回false。</returns>
        public bool nextTurnWin()
        {
            // 首先检查是否有敌方嘲讽随从存在
            if (this.anzEnemyTaunt > 0)
            {
                // 如果存在敌方嘲讽，检查是否能通过其他手段如法术或特殊随从效果击败敌方英雄
                int potentialDmg = CalculatePotentialDamage();
                if (this.enemyHero.Hp > potentialDmg)
                {
                    return false;
                }
            }
            else
            {
                // 如果没有嘲讽随从，计算己方随从的总伤害
                int dmg = 0;
                foreach (Minion m in this.ownMinions)
                {
                    if (m.frozen) continue; // 跳过被冻结的随从
                    dmg += m.Angr; // 加上随从的攻击力
                    if (m.windfury) dmg += m.Angr; // 如果随从有风怒，额外加一次攻击力
                }
                // 如果己方随从的伤害不足以击败敌方英雄，返回false
                if (this.enemyHero.Hp > dmg)
                {
                    return false;
                }
            }

            // 如果可以通过随从或法术等手段击败敌方英雄，返回true
            return true;
        }

        /// <summary>
        /// 计算潜在的总伤害，包括随从攻击和可能的法术伤害。
        /// </summary>
        /// <returns>返回可以造成的潜在总伤害值。</returns>
        private int CalculatePotentialDamage()
        {
            int totalDamage = 0;

            // 计算己方随从的伤害
            foreach (Minion m in this.ownMinions)
            {
                if (!m.frozen)
                {
                    totalDamage += m.Angr;
                    if (m.windfury) totalDamage += m.Angr; // 风怒随从可以攻击两次
                }
            }

            // 考虑己方手中的法术伤害
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                if (hc.card.type == CardDB.cardtype.SPELL)
                {
                    // 根据法术ID判断其直接伤害值
                    int spellDamage = 0;
                    switch (hc.card.cardIDenum)
                    {
                        case CardDB.cardIDEnum.CS2_029: // 火球术（Fireball）
                            spellDamage = 6 + this.spellpower; // 基础伤害为6，加上法术强度
                            break;
                        case CardDB.cardIDEnum.EX1_238: // 闪电箭（Lightning Bolt）
                            spellDamage = 3 + this.spellpower; // 基础伤害为3，加上法术强度
                            break;
                        case CardDB.cardIDEnum.CS2_087: // 地狱烈焰（Hellfire）
                            spellDamage = 3 + this.spellpower; // 对所有角色造成3点伤害，加上法术强度
                            break;
                        case CardDB.cardIDEnum.CORE_CS2_024: // 寒冰箭（Frostbolt）
                            spellDamage = 3 + this.spellpower; // 基础伤害为3，加上法术强度
                            break;
                            // 在这里添加其他直接对敌方英雄造成伤害的法术
                    }

                    totalDamage += spellDamage;
                }
            }

            return totalDamage;
        }

        /// <summary>
        /// 计算直伤伤害
        /// </summary>
        /// <param name="mana"></param>
        /// <param name="force">计算当前回合不考虑对手有嘲讽的情况</param>
        /// <param name="calNextTurn">计算下回合斩杀</param>
        /// <param name="maxCal">出牌上限</param>
        /// <param name="calMax">最多考虑的可能的出牌数量</param>
        /// <returns></returns>
        public int calDirectDmg(int mana, bool force, bool calNextTurn = false, int maxCal = 15, int calMax = 15)
        {
            if (mana < 0) mana = 0;
            if (mana > 10) mana = 10;

            int flag = 0;
            // 手上的幸运币/激活
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                switch (hc.card.nameCN)
                {
                    case CardDB.cardNameCN.幸运币: mana++; break;
                    case CardDB.cardNameCN.激活: { mana++; if (hc.card.cardIDenum == CardDB.cardIDEnum.VAN_EX1_169) { mana++; flag |= 8; } break; }
                    case CardDB.cardNameCN.雷霆绽放: mana += 2; break;
                    case CardDB.cardNameCN.自然之力:
                        flag |= 1;
                        break;
                    case CardDB.cardNameCN.野蛮咆哮:
                        if ((flag & 2) == 1) flag |= 4;
                        flag |= 2;
                        break;
                }
            }

            // 01背包
            int[] cost = new int[100];
            int[] dmg = new int[100];
            int[][] dp = new int[100][];
            for (int i = 0; i < maxCal; i++)
            {
                dp[i] = new int[100];
            }
            int cnt = 1;
            if (this.ownAbilityReady || calNextTurn)
                switch (this.ownHeroAblility.card.nameCN)
                {
                    case CardDB.cardNameCN.恶魔之爪:
                    case CardDB.cardNameCN.变形:
                        if (this.anzEnemyTaunt == 0 && this.ownHero.numAttacksThisTurn == 0)
                        {
                            cost[cnt] = this.ownHeroAblility.getManaCost(this);
                            dmg[cnt] = 1;
                            cnt++;
                        }
                        break;
                    case CardDB.cardNameCN.恶魔之咬:
                    case CardDB.cardNameCN.恐怖变形:
                        if (this.anzEnemyTaunt == 0 && this.ownHero.numAttacksThisTurn == 0)
                        {
                            cost[cnt] = this.ownHeroAblility.getManaCost(this);
                            dmg[cnt] = 2;
                            cnt++;
                        }
                        break;
                    case CardDB.cardNameCN.火焰冲击:
                        cost[cnt] = this.ownHeroAblility.getManaCost(this);
                        dmg[cnt] = 1 + this.ownHeroPowerExtraDamage;
                        cnt++;
                        break;
                    case CardDB.cardNameCN.二级火焰冲击:
                    case CardDB.cardNameCN.心灵尖刺:
                    case CardDB.cardNameCN.稳固射击:
                        cost[cnt] = this.ownHeroAblility.getManaCost(this);
                        dmg[cnt] = 2 + this.ownHeroPowerExtraDamage;
                        cnt++;
                        break;
                    case CardDB.cardNameCN.弩炮射击:
                        cost[cnt] = this.ownHeroAblility.getManaCost(this);
                        dmg[cnt] = 3 + this.ownHeroPowerExtraDamage;
                        cnt++;
                        break;
                    case CardDB.cardNameCN.生命分流:
                        if (this.anzTamsin)
                        {
                            cost[cnt] = this.ownHeroAblility.getManaCost(this);
                            dmg[cnt] = 2 + this.ownHeroPowerExtraDamage;
                            cnt++;
                        }
                        break;
                }
            bool canAttack = false;
            int extra = 0;
            foreach (Minion m in this.ownMinions)
            {
                if (this.anzTamsin && m.handcard.card.nameCN == CardDB.cardNameCN.无证药剂师) extra++;
                if (m.Ready && !m.cantAttackHeroes) canAttack = true;
            }
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                // 冲锋
                if (((hc.card.Charge) || hc.card.Durability > 0 && this.ownWeapon.Durability == 0 || hc.card.nameCN == CardDB.cardNameCN.利爪德鲁伊) && (this.anzEnemyTaunt == 0 || force) && hc.addattack + hc.card.Attack > 0)
                {
                    cost[cnt] = hc.manacost;
                    dmg[cnt] = hc.addattack + hc.card.Attack;
                }
                // 法术直伤
                switch (hc.card.nameCN)
                {
                    case CardDB.cardNameCN.炎爆术:
                        dmg[cnt] += 10 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.火球术:
                        dmg[cnt] += 6 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.邪火药水:
                    case CardDB.cardNameCN.埃匹希斯冲击:
                    case CardDB.cardNameCN.心灵震爆:
                        dmg[cnt] += 5 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.标记射击:
                    case CardDB.cardNameCN.废铁射击:
                    case CardDB.cardNameCN.深水炸弹:
                    case CardDB.cardNameCN.虚空碎片:
                    case CardDB.cardNameCN.冰枪术:
                    case CardDB.cardNameCN.横扫:
                    case CardDB.cardNameCN.灵魂之火:
                        dmg[cnt] += 4 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.快速射击:
                    case CardDB.cardNameCN.影袭:
                    case CardDB.cardNameCN.诱饵射击:
                    case CardDB.cardNameCN.恶魔来袭:
                    case CardDB.cardNameCN.地狱烈焰:
                    case CardDB.cardNameCN.闪电箭:
                    case CardDB.cardNameCN.毒蛇神殿传送门:
                    case CardDB.cardNameCN.寒冰箭:
                    case CardDB.cardNameCN.杀戮命令:
                        dmg[cnt] += 3 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.奉献:
                    case CardDB.cardNameCN.图腾重击:
                    case CardDB.cardNameCN.符文宝珠:
                    case CardDB.cardNameCN.末日灾祸:
                    case CardDB.cardNameCN.点燃:
                    case CardDB.cardNameCN.奥术射击:
                        dmg[cnt] += 2 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.冰霜震击:
                    case CardDB.cardNameCN.火焰之雨:
                    case CardDB.cardNameCN.急速射击:
                    case CardDB.cardNameCN.击伤猎物:
                        dmg[cnt] += 1 + this.spellpower;
                        break;
                    case CardDB.cardNameCN.关门放狗:
                        dmg[cnt] += this.enemyMinions.Count;
                        break;
                    case CardDB.cardNameCN.瞄准射击:
                        dmg[cnt] += 3 + this.spellpower;
                        break;


                    // 战吼
                    case CardDB.cardNameCN.火眼莫德雷斯:
                        if (hc.poweredUp > 0)
                            dmg[cnt] += 10;
                        break;
                    case CardDB.cardNameCN.生命的缚誓者阿莱克丝塔萨:
                        dmg[cnt] += 8;
                        break;
                    case CardDB.cardNameCN.云雾王子:
                        if (hc.poweredUp > 0)
                            dmg[cnt] += 6;
                        break;
                    case CardDB.cardNameCN.遮天雨云:
                        if (hc.poweredUp > 0)
                            dmg[cnt] += 5;
                        break;
                    case CardDB.cardNameCN.小刀商贩:
                        if (this.ownHero.Hp > 4)
                            dmg[cnt] += 4;
                        break;
                    case CardDB.cardNameCN.马戏团医师:
                        if (hc.card.cardIDenum == CardDB.cardIDEnum.DMF_174t)
                            dmg[cnt] += 4;
                        break;
                    case CardDB.cardNameCN.旋岩虫:
                        if (hc.poweredUp > 0)
                            dmg[cnt] += 3;
                        break;
                    case CardDB.cardNameCN.渊狱惩击者:
                        dmg[cnt] += 3;
                        break;
                    case CardDB.cardNameCN.迦顿男爵:
                    case CardDB.cardNameCN.雾帆劫掠者:
                    case CardDB.cardNameCN.丛林守护者:
                        dmg[cnt] += 2;
                        break;
                    case CardDB.cardNameCN.南海岸酋长:
                        if (hc.poweredUp > 0)
                            dmg[cnt] += 2;
                        break;
                    case CardDB.cardNameCN.精灵弓箭手:
                        dmg[cnt] += 1;
                        break;
                    case CardDB.cardNameCN.暗影投弹手:
                        if (this.ownHero.Hp > 3)
                            dmg[cnt] += 3;
                        break;
                    case CardDB.cardNameCN.力量的代价:
                        if (canAttack)
                        {
                            dmg[cnt] += 4;
                            if (this.anzTamsinroame > 0 && hc.getManaCost(this) > 0)
                            {
                                dmg[cnt] += 4 * this.anzTamsinroame;
                            }
                        }
                        break;
                    case CardDB.cardNameCN.萌物来袭:
                        if (canAttack) dmg[cnt] += 1;
                        break;
                    case CardDB.cardNameCN.灵魂炸弹:
                        if (this.anzTamsin)
                        {
                            dmg[cnt] += 4;
                            if (this.anzTamsinroame > 0 && hc.getManaCost(this) > 0)
                            {
                                dmg[cnt] += 4 * this.anzTamsinroame;
                            }
                        }
                        break;
                    case CardDB.cardNameCN.亡者复生:
                        if (this.anzTamsin) dmg[cnt] += 3;
                        break;
                    case CardDB.cardNameCN.晶化师:
                        if (this.anzTamsin) dmg[cnt] += 5;
                        break;
                    case CardDB.cardNameCN.烈焰小鬼:
                        if (this.anzTamsin) dmg[cnt] += 3;
                        break;
                    case CardDB.cardNameCN.狗头人图书管理员:
                        if (this.anzTamsin) dmg[cnt] += 2;
                        break;
                    case CardDB.cardNameCN.粗俗的矮劣魔:
                        if (this.anzTamsin) dmg[cnt] += 2;
                        break;
                    default:
                        break;
                }
                // 走A模式
                if (this.ownHero.enchs.Contains(CardDB.cardIDEnum.CFM_020e))
                {
                    dmg[cnt] += 2;
                }
                int ReadyCount = 0, murlocCount = 0;
                bool foundWind = false;
                foreach (Minion m in this.ownMinions)
                {
                    if ((m.Ready || calNextTurn) && !m.cantAttackHeroes && !m.frozen)
                    {
                        ReadyCount++;
                        if (m.handcard.card.race == CardDB.Race.MURLOC || m.handcard.card.race == CardDB.Race.ALL) murlocCount++;
                        if (m.windfury) foundWind = true;
                    }
                }
                // 加攻法术
                if (this.anzEnemyTaunt == 0 || force)
                {
                    switch (hc.card.nameCN)
                    {
                        case CardDB.cardNameCN.自然之力:
                            if (hc.card.cardIDenum == CardDB.cardIDEnum.VAN_EX1_571)
                            {
                                dmg[cnt] += 6;
                            }
                            break;
                        case CardDB.cardNameCN.铁肤古夫:
                            dmg[cnt] += 8;
                            break;
                        case CardDB.cardNameCN.爪击:
                        case CardDB.cardNameCN.飞扑:
                            dmg[cnt] += 2;
                            break;
                        case CardDB.cardNameCN.风暴打击:
                        case CardDB.cardNameCN.铁齿铜牙:
                            dmg[cnt] += 3;
                            break;
                        case CardDB.cardNameCN.月触项链:
                        case CardDB.cardNameCN.野性之怒:
                        case CardDB.cardNameCN.撕咬:
                            dmg[cnt] += 4;
                            break;
                        default:
                            break;
                    }
                    if (ReadyCount > 0)
                    {
                        switch (hc.card.nameCN)
                        {
                            case CardDB.cardNameCN.王者祝福:
                                dmg[cnt] += 4;
                                if (foundWind) dmg[cnt] += 4;
                                break;
                            case CardDB.cardNameCN.暗鳞先知:
                                dmg[cnt] += murlocCount;
                                break;
                            case CardDB.cardNameCN.鱼人领军:
                            case CardDB.cardNameCN.鱼勇可贾:
                            case CardDB.cardNameCN.鱼人恩典:
                                dmg[cnt] += murlocCount * 2;
                                break;
                            case CardDB.cardNameCN.塞纳留斯:
                                dmg[cnt] += ReadyCount * 2;
                                if (foundWind) dmg[cnt] += 2;
                                break;
                            case CardDB.cardNameCN.野蛮咆哮:
                                dmg[cnt] += (ReadyCount + 1) * 2;
                                if (foundWind) dmg[cnt] += 2;
                                break;
                            case CardDB.cardNameCN.玉莲印记:
                            case CardDB.cardNameCN.野性之力:
                                dmg[cnt] += ReadyCount * 1;
                                if (foundWind) dmg[cnt] += 1;
                                break;
                        }
                    }
                }
                if (extra > 0 && hc.card.type == CardDB.cardtype.MOB)
                {
                    dmg[cnt] += 5 * extra;
                }
                if (dmg[cnt] > 0)
                {
                    cost[cnt] = hc.getManaCost(this);
                    cnt++;
                }
            }
            int nextTurnMana = mana;
            // 虚触侍从
            if (this.anzOldWoman > 0 && !calNextTurn)
            {
                for (int i = 0; i < maxCal; i++)
                {
                    if (dmg[i] > 0) dmg[i] += this.anzOldWoman;
                }
            }

            // 计算对手最多使用 calMax 张牌时的斩杀线，需要用多维背包计算
            if (calMax != 15)
            {
                if (maxCal > 40) maxCal = 40;
                if (maxCal < cnt) maxCal = cnt + 1;

                int[][][] muitiDp = new int[45][][];
                for (int i = 0; i < maxCal; i++)
                {
                    muitiDp[i] = new int[45][];
                    for (int j = 0; j <= nextTurnMana + 5; j++)
                    {
                        muitiDp[i][j] = new int[45];
                    }
                }
                // i 张牌
                for (int i = 1; i <= cnt; i++)
                    // 费用 j
                    for (int j = 0; j <= nextTurnMana; j++)
                        // 总手牌数
                        for (int k = 0; k <= calMax; k++)
                        {
                            muitiDp[i][j][k] = muitiDp[i - 1][j][k];
                            if (j >= cost[i] && k >= 1)
                            {
                                var tmp = dmg[i] + muitiDp[i - 1][j - cost[i]][k - 1];
                                if (tmp > muitiDp[i][j][k]) muitiDp[i][j][k] = tmp;
                            }
                        }
                int enemyMaxDmg = muitiDp[cnt][nextTurnMana][calMax];
                // 自然之力咆哮
                if ((this.anzEnemyTaunt == 0 || force) && mana >= 9 && flag == 3 && enemyMaxDmg < 14 + this.ownMinions.Count * 2) enemyMaxDmg = 14 + this.ownMinions.Count * 2;
                // 自然之力双咆哮
                if ((this.anzEnemyTaunt == 0 || force) && mana >= 12 && flag == 15 && enemyMaxDmg < 20 + this.ownMinions.Count * 4) enemyMaxDmg = 20 + this.ownMinions.Count * 4;
                return enemyMaxDmg;
            }

            // 第 i 张牌
            for (int i = 1; i <= cnt; i++)
            {
                // 剩余费用
                for (int j = 1; j <= nextTurnMana; j++)
                {
                    // 打不出去
                    if (cost[i] > j)
                    {
                        dp[i][j] = dp[i - 1][j];
                    }
                    else
                    {
                        // 能出牌的情况取其中最大伤害
                        dp[i][j] = Math.Max(dp[i - 1][j - cost[i]] + dmg[i], dp[i - 1][j]);
                    }
                }
            }
            int maxDmg = dp[cnt][nextTurnMana];
            // 自然之力咆哮
            if ((this.anzEnemyTaunt == 0 || force) && mana >= 9 && flag == 3 && maxDmg < 14 + this.ownMinions.Count * 2) maxDmg = 14 + this.ownMinions.Count * 2;
            // 自然之力双咆哮
            if ((this.anzEnemyTaunt == 0 || force) && mana >= 12 && flag == 15 && maxDmg < 20 + this.ownMinions.Count * 4) maxDmg = 20 + this.ownMinions.Count * 4;
            return maxDmg;
        }
    }
}
