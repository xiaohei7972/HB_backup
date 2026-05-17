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
    // 搜索与评估：目标搜索、最佳位置、场面评估
    public partial class Playfield
    {
        /// <summary>
        /// 获取并返回下一个实体编号，同时将实体编号计数器递增。
        /// </summary>
        /// <returns>当前的实体编号。</returns>
        public int getNextEntity()
        {
            int retval = this.nextEntity;  // 保存当前的实体编号
            this.nextEntity++;  // 将实体编号计数器递增
            return retval;  // 返回当前的实体编号
        }

        /// <summary>
        /// 获得所有可攻击的目标随从
        /// </summary>
        /// <param name="own"></param>
        /// <param name="isLethalCheck"></param>
        /// <returns></returns>
        public List<Minion> GetAttackTargets(bool own, bool isLethalCheck)
        {
            List<Minion> tauntTarget = new List<Minion>();
            List<Minion> trgts2 = new List<Minion>();

            List<Minion> temp = (own) ? this.enemyMinions : this.ownMinions;
            bool hasTaunts = false;
            foreach (Minion m in temp)
            {
                //不可攻击 潜行 免疫
                if (m.untouchable || m.stealth || m.immune) continue;//不可攻击
                if (m.taunt)
                {
                    hasTaunts = true;
                    tauntTarget.Add(m);
                }
                else
                {
                    trgts2.Add(m);
                }
            }
            //移除地标
            trgts2.RemoveAll(minion => minion?.handcard?.card?.type == CardDB.cardtype.LOCATION);
            if (hasTaunts) return tauntTarget;

            if (isLethalCheck) trgts2.Clear(); // only target enemy hero during Lethal check!

            if (own && !(this.enemyHero.immune || this.enemyHero.stealth)) trgts2.Add(this.enemyHero);//免疫 潜行
            else if (!own && !(this.ownHero.immune || this.ownHero.stealth)) trgts2.Add(this.ownHero);


            return trgts2;
        }

        /// <summary>
        /// 获取最好的位置
        /// </summary>
        /// <param name="card"></param>
        /// <param name="lethal"></param>
        /// <returns></returns>
        public int getBestPlace(CardDB.Card card, bool lethal)
        {
            //we return the zonepos!
            if (card.type != CardDB.cardtype.MOB) return 1;
            if (this.ownMinions.Count == 0) return 1;
            if (this.ownMinions.Count == 1)
            {
                if (this.ownMinions[0].Angr < card.Attack) return 1;
                else if (this.ownMinions[0].handcard.card.nameEN == CardDB.cardNameEN.flametonguetotem || this.ownMinions[0].handcard.card.nameEN == CardDB.cardNameEN.direwolfalpha) return 1;
                else if (card.nameEN == CardDB.cardNameEN.tuskarrtotemic) return 1;
                else if (this.ownMinions[0].handcard.card.nameCN == CardDB.cardNameCN.战场军官
                    || this.ownMinions[0].handcard.card.nameCN == CardDB.cardNameCN.恐狼前锋
                    || this.ownMinions[0].handcard.card.nameCN == CardDB.cardNameCN.火舌图腾) return 1;
                else return 2;
            }

            // 战场军官特别判定,随从至少两只
            if (card.nameCN == CardDB.cardNameCN.战场军官)
            {
                int place = 1;
                int maxAngr = 0;
                for (int ii = 1; ii < this.ownMinions.Count; ii++)
                {
                    int angr = (this.ownMinions[ii - 1].windfury || this.ownMinions[ii - 1].frozen || this.ownMinions[ii - 1].cantAttack ? 0 : this.ownMinions[ii - 1].Angr) + (this.ownMinions[ii].windfury || this.ownMinions[ii].frozen || this.ownMinions[ii].cantAttack ? 0 : this.ownMinions[ii].Angr);
                    if (angr > maxAngr)
                    {
                        place = ii + 1;
                        maxAngr = angr;
                    }
                }
                return place;
            }

            for (int ii = 0; ii < this.ownMinions.Count; ii++)
            {
                if (this.ownMinions[ii].handcard.card.nameCN == CardDB.cardNameCN.战场军官 || this.ownMinions[ii].handcard.card.nameCN == CardDB.cardNameCN.恐狼前锋 || this.ownMinions[ii].handcard.card.nameCN == CardDB.cardNameCN.战场军官)
                {
                    // 冲锋怪放在战场军官左边
                    if (card.Charge || card.Rush)
                    {
                        return ii + 1;
                    }
                    else
                    {
                        // 右边没有怪或者左边有两只怪
                        if (this.ownMinions.Count - ii <= 1 || ii >= 2) return this.ownMinions.Count + 1;
                        else return 1;
                    }
                }
            }

            // buff 怪
            if (card.nameCN == CardDB.cardNameCN.暴风城卫兵 || card.nameCN == CardDB.cardNameCN.阿古斯防御者 || card.nameCN == CardDB.cardNameCN.年迈的法师
                || card.nameCN == CardDB.cardNameCN.阳焰瓦格里 || card.nameCN == CardDB.cardNameCN.菌菇术士 || card.nameCN == CardDB.cardNameCN.蠕动的恐魔 || card.nameCN == CardDB.cardNameCN.污手街守护者)
            {
                int place = 1;
                int maxVal = 0;
                for (int ii = 1; ii < this.ownMinions.Count; ii++)
                {
                    int val = 0;
                    if (this.ownMinions[ii - 1].Ready)
                    {
                        val++;
                        if (this.ownMinions[ii - 1].windfury) val++;
                    }
                    if (this.ownMinions[ii].Ready)
                    {
                        val++;
                        if (this.ownMinions[ii].windfury) val++;
                    }
                    if (val > maxVal)
                    {
                        place = ii + 1;
                        maxVal = val;
                    }
                }
                return place;
            }

            // 为军官准备
            /* if (Ai.Instance.botBase.BehaviorName() == "骑士" || Ai.Instance.botBase.BehaviorName() == "黑眼任务术")
            {
                int nowAngr = card.Attack;
                for (int ii = 0; ii < this.ownMinions.Count; ii++)
                {
                    if (this.ownMinions[ii].Angr < nowAngr)
                    {
                        return ii + 1;
                    }
                }
                return this.ownMinions.Count + 1;
            } */


            int omCount = this.ownMinions.Count;
            int[] places = new int[omCount];
            int[] buffplaces = new int[omCount];
            int i = 0;
            int tempval = 0;
            if (lethal)
            {
                bool givesBuff = false;
                switch (card.nameEN)
                {
                    case CardDB.cardNameEN.grimestreetprotector: givesBuff = true; break;
                    case CardDB.cardNameEN.defenderofargus: givesBuff = true; break;
                    case CardDB.cardNameEN.flametonguetotem: givesBuff = true; break;
                    case CardDB.cardNameEN.direwolfalpha: givesBuff = true; break;
                    case CardDB.cardNameEN.ancientmage: givesBuff = true; break;
                    case CardDB.cardNameEN.tuskarrtotemic: givesBuff = true; break;
                    case CardDB.cardNameEN.battlegroundbattlemaster: givesBuff = true; break;
                }
                if (givesBuff)
                {
                    if (this.ownMinions.Count == 2) return 2;
                    i = 0;
                    foreach (Minion m in this.ownMinions)
                    {

                        places[i] = 0;
                        tempval = 0;
                        if (m.Ready)
                        {
                            tempval -= m.Angr - 1;
                            if (m.windfury) tempval -= m.Angr - 1;
                        }
                        else tempval = 1000;
                        places[i] = tempval;

                        i++;
                    }

                    i = 0;
                    int bestpl = 7;
                    int bestval = 10000;
                    foreach (Minion m in this.ownMinions)
                    {
                        int prev = 0;
                        int next = 0;
                        if (i >= 1) prev = places[i - 1];
                        next = places[i];
                        if (bestval >= prev + next)
                        {
                            bestval = prev + next;
                            bestpl = i;
                        }
                        i++;
                    }
                    return bestpl + 1;
                }
                else return this.ownMinions.Count + 1;
            }
            //日怒、阿古斯
            if (card.nameEN == CardDB.cardNameEN.sunfuryprotector || card.nameEN == CardDB.cardNameEN.defenderofargus) // bestplace, if right and left minions have no taunt + lots of hp, dont make priority-minions to taunt
            {
                if (lethal) return 1;
                if (this.ownMinions.Count == 2)
                {
                    int val1 = prozis.penman.getValueOfUsefulNeedKeepPriority(this.ownMinions[0].handcard.card.nameEN);
                    int val2 = prozis.penman.getValueOfUsefulNeedKeepPriority(this.ownMinions[1].handcard.card.nameEN);
                    if (val1 == 0 && val2 == 0) return 2;
                    else if (val1 > val2) return 3;
                    else return 1;
                }

                i = 0;
                foreach (Minion m in this.ownMinions)
                {

                    places[i] = 0;
                    tempval = 0;
                    if (!m.taunt)
                    {
                        tempval -= m.Hp;
                    }
                    else
                    {
                        tempval -= m.Hp - 2;
                    }

                    if (m.windfury)
                    {
                        tempval += 2;
                    }

                    tempval += prozis.penman.getValueOfUsefulNeedKeepPriority(m.handcard.card.nameEN);
                    places[i] = tempval;
                    i++;
                }


                i = 0;
                int bestpl = 7;
                int bestval = 10000;
                foreach (Minion m in this.ownMinions)
                {
                    int prev = 0;
                    int next = 0;
                    if (i >= 1) prev = places[i - 1];
                    next = places[i];
                    if (bestval > prev + next)
                    {
                        bestval = prev + next;
                        bestpl = i;
                    }
                    i++;
                }
                return bestpl + 1;
            }

            int cardIsBuffer = 0;
            bool placebuff = false;
            if (card.nameEN == CardDB.cardNameEN.flametonguetotem || card.nameEN == CardDB.cardNameEN.direwolfalpha || card.nameEN == CardDB.cardNameEN.tuskarrtotemic || card.nameEN == CardDB.cardNameEN.battlegroundbattlemaster)
            {
                placebuff = true;
                if (card.nameEN == CardDB.cardNameEN.flametonguetotem || card.nameEN == CardDB.cardNameEN.tuskarrtotemic) cardIsBuffer = 2;
                if (card.nameEN == CardDB.cardNameEN.direwolfalpha) cardIsBuffer = 1;
            }
            bool tundrarhino = false;
            foreach (Minion m in this.ownMinions)
            {
                if (m.handcard.card.nameEN == CardDB.cardNameEN.tundrarhino) tundrarhino = true;
                if (m.handcard.card.nameEN == CardDB.cardNameEN.flametonguetotem || m.handcard.card.nameEN == CardDB.cardNameEN.direwolfalpha || card.nameEN == CardDB.cardNameEN.battlegroundbattlemaster) placebuff = true;
            }
            //max attack this turn
            if (placebuff)
            {


                int cval = 0;
                if (card.Charge || (card.race == CardDB.Race.PET && tundrarhino))
                {
                    cval = card.Attack;
                    if (card.windfury) cval = card.Attack;
                }
                if (card.nameEN == CardDB.cardNameEN.nerubianegg)
                {
                    cval += 1;
                }
                i = 0;
                int[] whirlwindplaces = new int[this.ownMinions.Count];
                int gesval = 0;
                int minionsBefore = -1;
                int minionsAfter = -1;
                foreach (Minion m in this.ownMinions)
                {
                    buffplaces[i] = 0;
                    whirlwindplaces[i] = 1;
                    places[i] = 0;
                    tempval = -1;

                    if (m.Ready)
                    {
                        tempval = m.Angr;
                        if (m.windfury && m.numAttacksThisTurn == 0)
                        {
                            tempval += m.Angr;
                            whirlwindplaces[i] = 2;
                        }
                    }
                    else whirlwindplaces[i] = 0;

                    switch (m.handcard.card.nameEN)
                    {
                        case CardDB.cardNameEN.flametonguetotem:
                            buffplaces[i] = 2;
                            goto case CardDB.cardNameEN.aiextra1;
                        case CardDB.cardNameEN.direwolfalpha:
                            buffplaces[i] = 1;
                            goto case CardDB.cardNameEN.aiextra1;
                        case CardDB.cardNameEN.aiextra1:
                            if (minionsBefore == -1) minionsBefore = i;
                            minionsAfter = omCount - i - 1;
                            break;
                    }
                    tempval++;
                    places[i] = tempval;
                    gesval += tempval;
                    i++;
                }
                //gesval = whole possible damage
                int bplace = 0;
                int bvale = 0;
                bool needbefore = false;
                int middle = (omCount + 1) / 2;
                int middleProximity = 1000;
                int tmp = 0;
                if (minionsBefore > -1 && minionsBefore <= minionsAfter) needbefore = true;
                tempval = 0;
                for (i = 0; i <= omCount; i++)
                {
                    tempval = gesval;
                    int current = cval;
                    int prev = 0;
                    int next = 0;
                    if (i >= 1)
                    {
                        tempval -= places[i - 1];
                        prev = places[i - 1];
                        if (prev >= 0) prev += whirlwindplaces[i - 1] * cardIsBuffer;
                        if (i < omCount)
                        {
                            prev -= whirlwindplaces[i - 1] * buffplaces[i];
                        }
                        if (current > 0) current += buffplaces[i - 1];
                    }
                    if (i < omCount)
                    {
                        tempval -= places[i];
                        next = places[i];
                        if (next >= 0) next += whirlwindplaces[i] * cardIsBuffer;
                        if (i >= 1)
                        {
                            next -= whirlwindplaces[i] * buffplaces[i - 1];
                        }
                        if (current > 0) current += buffplaces[i];
                    }
                    tempval += current + prev + next;

                    bool setVal = false;
                    if (tempval > bvale) setVal = true;
                    else if (tempval == bvale)
                    {
                        if (needbefore)
                        {
                            if (i <= minionsBefore) setVal = true;
                        }
                        else
                        {
                            if (minionsBefore > -1)
                            {
                                if (i >= omCount - minionsAfter) setVal = true;
                            }
                            else
                            {
                                tmp = middle - i;
                                if (tmp < 0) tmp *= -1;
                                if (tmp <= middleProximity)
                                {
                                    middleProximity = tmp;
                                    setVal = true;
                                }
                            }
                        }
                    }
                    if (setVal)
                    {
                        bplace = i;
                        bvale = tempval;
                    }
                }
                return bplace + 1;
            }

            // normal placement
            int bestplace = 0;
            int bestvale = 0;
            if (prozis.settings.placement == 1)
            {
                int cardvalue = card.Health * 2 + card.Attack;
                if (card.Shield) cardvalue = cardvalue * 3 / 2;
                cardvalue += prozis.penman.getValueOfUsefulNeedKeepPriority(card.nameEN);

                i = 0;
                foreach (Minion m in this.ownMinions)
                {
                    places[i] = 0;
                    tempval = m.maxHp * 2 + m.Angr;
                    if (m.divineShield) tempval = tempval * 3 / 2;
                    if (!m.silenced) tempval += prozis.penman.getValueOfUsefulNeedKeepPriority(m.handcard.card.nameEN);
                    places[i] = tempval;
                    i++;
                }

                tempval = 0;
                for (i = 0; i <= omCount; i++)
                {
                    if (i >= omCount - i)
                    {
                        bestplace = i;
                        break;
                    }
                    if (cardvalue >= places[i])
                    {
                        if (cardvalue < places[omCount - i - 1])
                        {
                            bestplace = i;
                            break;
                        }
                        else
                        {
                            if (places[i] > places[omCount - i - 1]) bestplace = omCount - i;
                            else bestplace = i;
                            break;
                        }
                    }
                    else
                    {
                        if (cardvalue >= places[omCount - i - 1])
                        {
                            bestplace = omCount - i;
                            break;
                        }
                    }
                }
            }
            else
            {
                int cardvalue = card.Attack * 2 + card.Health;
                if (card.tank)
                {
                    cardvalue += 5;
                    cardvalue += card.Health;
                }

                cardvalue += prozis.penman.getValueOfUsefulNeedKeepPriority(card.nameEN);
                cardvalue += 1;

                i = 0;
                foreach (Minion m in this.ownMinions)
                {
                    places[i] = 0;
                    tempval = m.Angr * 2 + m.maxHp;
                    if (m.taunt)
                    {
                        tempval += 6;
                        tempval += m.maxHp;
                    }
                    if (!m.silenced)
                    {
                        tempval += prozis.penman.getValueOfUsefulNeedKeepPriority(m.handcard.card.nameEN);
                        if (m.stealth) tempval += 40;
                    }
                    places[i] = tempval;

                    i++;
                }

                //bigminion if >=10
                tempval = 0;
                for (i = 0; i <= omCount; i++)
                {
                    int prev = cardvalue;
                    int next = cardvalue;
                    if (i >= 1) prev = places[i - 1];
                    if (i < omCount) next = places[i];


                    if (cardvalue >= prev && cardvalue >= next)
                    {
                        tempval = 2 * cardvalue - prev - next;
                        if (tempval > bestvale)
                        {
                            bestplace = i;
                            bestvale = tempval;
                        }
                    }
                    if (cardvalue <= prev && cardvalue <= next)
                    {
                        tempval = -2 * cardvalue + prev + next;
                        if (tempval > bestvale)
                        {
                            bestplace = i;
                            bestvale = tempval;
                        }
                    }
                }

            }

            return bestplace + 1;
        }

        /// <summary>
        /// 获取随从的最佳进化选择，并根据当前情况对随从进行增强或赋予新的特性。
        /// 进化选项：1-+1/+1，2-+3攻击力，3-+3生命值，4-嘲讽，5-圣盾，6-剧毒。
        /// </summary>
        /// <param name="m">需要进化的随从。</param>
        /// <returns>选择的进化效果对应的数字编号。</returns>
        public int getBestAdapt(Minion m)
        {
            // 判断己方英雄是否具有直接斩杀对手的能力
            bool ownLethal = this.ownHeroHasDirectLethal();
            bool needTaunt = false;

            // 如果己方具有斩杀能力，优先考虑为随从赋予嘲讽
            if (ownLethal)
            {
                needTaunt = true;
            }
            else if (m.Ready) // 如果随从已经准备好攻击
            {
                // 判断敌方英雄距离被斩杀还差多少伤害，如果小于等于3，则增加随从的攻击力
                if (guessEnemyHeroLethalMissing() <= 3)
                {
                    this.minionGetBuffed(m, 3, 0);
                    return 2; // 返回+3攻击力的选择
                }
                else if (ownLethal)
                {
                    needTaunt = true;
                }
                else
                {
                    // 如果随从的生命值大于3，则增加生命值
                    if (m.Hp > 3)
                    {
                        this.minionGetBuffed(m, 0, 3);
                        return 3; // 返回+3生命值的选择
                    }
                    else
                    {
                        m.poisonous = true;
                        return 6; // 如果生命值不高，则赋予剧毒效果
                    }
                }
            }
            else
            {
                this.minionGetBuffed(m, 1, 1);
                return 1; // 随从未准备好攻击时，增加+1/+1
            }

            // 如果需要嘲讽效果
            if (needTaunt)
            {
                // 如果随从没有嘲讽，赋予嘲讽效果
                if (!m.taunt)
                {
                    m.taunt = true;
                    if (m.own) this.anzOwnTaunt++;
                    else this.anzEnemyTaunt++;
                    return 4; // 返回嘲讽效果
                }
                // 如果随从已经有嘲讽，但没有圣盾，赋予圣盾效果
                else if (!m.divineShield)
                {
                    m.divineShield = true;
                    return 5; // 返回圣盾效果
                }
                // 如果随从已经有嘲讽和圣盾，但没有剧毒，赋予剧毒效果
                else if (!m.poisonous)
                {
                    m.poisonous = true;
                    return 6; // 返回剧毒效果
                }
                // 否则，增加生命值
                else
                {
                    this.minionGetBuffed(m, 0, 3);
                    return 3; // 返回+3生命值的选择
                }
            }

            return 0; // 返回0表示未进行任何进化
        }

        /// <summary>
        /// 根据卡牌类型或属性筛选手牌，并标记符合条件的卡牌。
        /// </summary>
        /// <param name="cards">需要筛选的手牌列表</param>
        /// <param name="tag">要筛选的卡牌属性或类型</param>
        /// <param name="race">当筛选条件为种族时的具体种族，默认为无效种族</param>
        private void getHandcardsByType(List<Handmanager.Handcard> cards, GAME_TAGs tag, TAG_RACE race = TAG_RACE.INVALID)
        {
            switch (tag)
            {
                case GAME_TAGs.None:
                    // 无条件筛选，标记所有卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        hc.extraParam3 = true;
                    }
                    break;
                case GAME_TAGs.Spell:
                    // 筛选法术牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.type == CardDB.cardtype.SPELL)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.SECRET:
                    // 筛选奥秘牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.Secret)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.Mob:
                    // 筛选随从牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.type == CardDB.cardtype.MOB)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.CARDRACE:
                    // 筛选特定种族的随从牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.type == CardDB.cardtype.MOB)
                        {
                            if (race == TAG_RACE.INVALID)
                            {
                                hc.extraParam3 = true;
                            }
                            else if (hc.card.race == (CardDB.Race)race)
                            {
                                hc.extraParam3 = true;
                            }
                        }
                    }
                    break;
                case GAME_TAGs.TAUNT:
                    // 筛选具有嘲讽属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.tank)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.COMBO:
                    // 筛选具有连击属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.Combo)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.DIVINE_SHIELD:
                    // 筛选具有圣盾属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.Shield)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.ENRAGED:
                    // 筛选具有激怒属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.Enrage)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.LIFESTEAL:
                    // 筛选具有吸血属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.lifesteal)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.OVERLOAD:
                    // 筛选具有过载属性的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.overload > 0)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.CLASS:
                    // 筛选当前职业的卡牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.Class == (int)ownHeroStartClass)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
                case GAME_TAGs.Weapon:
                    // 筛选武器牌
                    foreach (Handmanager.Handcard hc in cards)
                    {
                        if (hc.card.type == CardDB.cardtype.WEAPON)
                        {
                            hc.extraParam3 = true;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 计算己方所有可以攻击英雄的角色的总攻击力，包括随从和英雄自身。
        /// </summary>
        /// <returns>返回计算后的总攻击力</returns>
        public int calTotalAngr()
        {
            if (this.totalAngr == -1)
            {
                this.totalAngr = 0;

                // 计算己方随从的攻击力
                for (int i = 0; i < this.ownMinions.Count; i++)
                {
                    // 确保随从可以攻击并且可以攻击英雄
                    if (this.ownMinions[i].Ready && !this.ownMinions[i].cantAttackHeroes)
                    {
                        this.totalAngr += this.ownMinions[i].Angr;

                        // 判断随从是否具有风怒或受到战场军官的影响而获得额外攻击机会
                        if (this.ownMinions[i].windfury ||
                            (i > 0 && this.ownMinions[i - 1].handcard.card.nameCN == CardDB.cardNameCN.战场军官 && !this.ownMinions[i - 1].silenced) ||
                            (i < this.ownMinions.Count - 1 && this.ownMinions[i + 1].handcard.card.nameCN == CardDB.cardNameCN.战场军官 && !this.ownMinions[i + 1].silenced))
                        {
                            if (this.ownMinions[i].numAttacksThisTurn == 0)
                            {
                                this.totalAngr += this.ownMinions[i].Angr;
                            }
                        }
                        //判断虚触侍从对场攻的提升
                    }
                }

                // 计算己方英雄的攻击力
                if (this.ownHero.Ready)
                {
                    this.totalAngr += this.ownHero.Angr;
                }
            }
            return this.totalAngr;
        }

        /// <summary>
        /// 计算敌方所有可以攻击的角色（包括武器和随从）的总攻击力。
        /// 同时考虑敌方的军官风怒效果，并减去己方嘲讽随从的生命值。
        /// </summary>
        /// <returns>返回计算后的敌方总攻击力</returns>
        public int calEnemyTotalAngr()
        {
            if (this.enemyTotalAngr == -1)
            {
                this.enemyTotalAngr = this.enemyWeapon.Angr; // 从敌方武器开始计算攻击力
                for (int i = 0; i < this.enemyMinions.Count; i++)
                {
                    // 跳过无法攻击或被冻结的随从
                    if (this.enemyMinions[i].cantAttack || this.enemyMinions[i].frozen) continue;

                    // 增加当前随从的攻击力
                    this.enemyTotalAngr += this.enemyMinions[i].Angr;

                    // 检查是否有军官风怒效果
                    if (this.enemyMinions[i].windfury)
                    {
                        this.enemyTotalAngr += this.enemyMinions[i].Angr; // 如果满足风怒条件，额外计算一次攻击力
                    }
                    // if ((i > 0 && this.enemyMinions[i - 1].handcard.card.nameCN == CardDB.cardNameCN.战场军官 && !this.enemyMinions[i - 1].silenced) || (i < this.enemyMinions.Count - 1 && this.enemyMinions[i + 1].handcard.card.nameCN == CardDB.cardNameCN.战场军官 && !this.enemyMinions[i + 1].silenced))
                    // {
                    //     this.enemyTotalAngr += this.enemyMinions[i].Angr; // 如果满足风怒条件，额外计算一次攻击力
                    // }
                }

                // 减去己方嘲讽随从的生命值
                for (int i = 0; i < this.ownMinions.Count; i++)
                {
                    if (this.ownMinions[i].taunt)
                    {
                        this.enemyTotalAngr -= this.ownMinions[i].Hp;
                    }
                }

                // 确保最终的敌方总攻击力不为负数
                this.enemyTotalAngr = this.enemyTotalAngr < 0 ? 0 : this.enemyTotalAngr;
            }
            return this.enemyTotalAngr;
        }

        /// <summary>
        /// 根据指定的搜索模式和条件，从手牌中随机选择一个随从卡牌。
        /// </summary>
        /// <param name="cards">需要筛选的手牌列表</param>
        /// <param name="mode">搜索模式，如最低生命值、最高攻击力等</param>
        /// <param name="tag">要筛选的卡牌属性或类型（可选）</param>
        /// <param name="race">当筛选条件为种族时的具体种族，默认为无效种族（可选）</param>
        /// <returns>返回符合条件的手牌中的随从卡牌</returns>
        public Handmanager.Handcard searchRandomMinionInHand(List<Handmanager.Handcard> cards, searchmode mode, GAME_TAGs tag = GAME_TAGs.None, TAG_RACE race = TAG_RACE.INVALID)
        {
            Handmanager.Handcard ret = null;
            double value = 0;

            // 根据搜索模式设置初始值
            switch (mode)
            {
                case searchmode.searchLowestHP: value = 1000; break;
                case searchmode.searchHighestHP: value = -1; break;
                case searchmode.searchLowestAttack: value = 1000; break;
                case searchmode.searchHighestAttack: value = -1; break;
                case searchmode.searchHighAttackLowHP: value = -1; break;
                case searchmode.searchHighHPLowAttack: value = -1; break;
                case searchmode.searchLowestCost: value = 1000; break;
                case searchmode.searchHighesCost: value = -1; break;
            }

            // 筛选符合条件的卡牌
            getHandcardsByType(cards, tag, race);

            // 遍历筛选后的卡牌并根据搜索模式寻找最佳匹配
            foreach (Handmanager.Handcard hc in cards)
            {
                if (!hc.extraParam3) continue;
                hc.extraParam3 = false;

                switch (mode)
                {
                    case searchmode.searchLowestHP:
                        if ((hc.card.Health + hc.addHp) < value)
                        {
                            ret = hc;
                            value = (hc.card.Health + hc.addHp);
                        }
                        break;
                    case searchmode.searchHighestHP:
                        if ((hc.card.Health + hc.addHp) > value)
                        {
                            ret = hc;
                            value = (hc.card.Health + hc.addHp);
                        }
                        break;
                    case searchmode.searchLowestAttack:
                        if ((hc.card.Attack + hc.addattack) < value)
                        {
                            ret = hc;
                            value = (hc.card.Attack + hc.addattack);
                        }
                        break;
                    case searchmode.searchHighestAttack:
                        if ((hc.card.Attack + hc.addattack) > value)
                        {
                            ret = hc;
                            value = (hc.card.Attack + hc.addattack);
                        }
                        break;
                    case searchmode.searchHighAttackLowHP:
                        if ((hc.card.Attack + hc.addattack) / (hc.card.Health + hc.addHp) > value)
                        {
                            ret = hc;
                            value = (hc.card.Attack + hc.addattack) / (hc.card.Health + hc.addHp);
                        }
                        break;
                    case searchmode.searchHighHPLowAttack:
                        if ((hc.card.Health + hc.addHp) / (hc.card.Attack + hc.addattack) > value)
                        {
                            ret = hc;
                            value = (hc.card.Health + hc.addHp) / (hc.card.Attack + hc.addattack);
                        }
                        break;
                    case searchmode.searchLowestCost:
                        if (hc.manacost < value)
                        {
                            ret = hc;
                            value = hc.manacost;
                        }
                        break;
                    case searchmode.searchHighesCost:
                        if (hc.manacost > value)
                        {
                            ret = hc;
                            value = hc.manacost;
                        }
                        break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 根据指定的搜索模式，从随从列表中随机选择一个符合条件的随从。
        /// </summary>
        /// <param name="minions">随从列表</param>
        /// <param name="mode">搜索模式，如最低生命值、最高攻击力等</param>
        /// <returns>返回符合条件的随从对象，如果列表为空则返回 null</returns>
        public Minion searchRandomMinion(List<Minion> minions, searchmode mode)
        {
            if (minions.Count == 0) return null;

            Minion ret = null; // 用于存储最终返回的随从
            double value;

            // 根据搜索模式初始化 value
            switch (mode)
            {
                case searchmode.searchLowestHP:
                case searchmode.searchLowestAttack:
                case searchmode.searchLowestCost:
                    value = 1000; // 初始化为较大值，用于寻找最小值
                    break;
                case searchmode.searchHighestHP:
                case searchmode.searchHighestAttack:
                case searchmode.searchHighesCost:
                case searchmode.searchHighAttackLowHP:
                case searchmode.searchHighHPLowAttack:
                    value = -1;  // 初始化为较小值，用于寻找最大值
                    break;
                default:
                    value = 0;  // 其他情况初始化为0（根据需要调整）
                    break;
            }

            // 遍历随从列表，根据不同的搜索模式更新目标随从
            foreach (Minion m in minions)
            {
                if (m.Hp <= 0) continue; // 忽略生命值小于等于0的随从

                switch (mode)
                {
                    case searchmode.searchLowestHP:
                        if (m.Hp < value) // 更新目标为生命值最低的随从
                        {
                            ret = m;
                            value = m.Hp;
                        }
                        break;
                    case searchmode.searchHighestHP:
                        if (m.Hp > value) // 更新目标为生命值最高的随从
                        {
                            ret = m;
                            value = m.Hp;
                        }
                        break;
                    case searchmode.searchLowestAttack:
                        if (m.Angr < value) // 更新目标为攻击力最低的随从
                        {
                            ret = m;
                            value = m.Angr;
                        }
                        break;
                    case searchmode.searchHighestAttack:
                        if (m.Angr > value) // 更新目标为攻击力最高的随从
                        {
                            ret = m;
                            value = m.Angr;
                        }
                        break;
                    case searchmode.searchHighAttackLowHP:
                        if ((double)m.Angr / m.Hp > value) // 更新目标为高攻低血比值最大的随从
                        {
                            ret = m;
                            value = (double)m.Angr / m.Hp;
                        }
                        break;
                    case searchmode.searchHighHPLowAttack:
                        if (m.Angr == 0) // 如果攻击力为0，直接选中这个随从
                        {
                            if (ret == null) ret = m;
                            continue;
                        }
                        if ((double)m.Hp / m.Angr > value) // 更新目标为高血低攻比值最大的随从
                        {
                            ret = m;
                            value = (double)m.Hp / m.Angr;
                        }
                        break;
                    case searchmode.searchLowestCost:
                        if (m.handcard.card.cost < value) // 更新目标为费用最低的随从
                        {
                            ret = m;
                            value = m.handcard.card.cost;
                        }
                        break;
                    case searchmode.searchHighesCost:
                        if (m.handcard.card.cost > value) // 更新目标为费用最高的随从
                        {
                            ret = m;
                            value = m.handcard.card.cost;
                        }
                        break;
                }
            }
            return ret; // 返回符合条件的随从
        }

        /// <summary>
        /// 在指定的随从列表中，根据最大生命值和搜索模式查找符合条件的随从。
        /// </summary>
        /// <param name="minions">随从列表</param>
        /// <param name="mode">搜索模式，如最高生命值、最低攻击力等</param>
        /// <param name="maxHP">随从的最大生命值限制</param>
        /// <returns>返回符合条件的随从对象，如果没有符合条件的随从则返回 null</returns>
        public Minion searchRandomMinionByMaxHP(List<Minion> minions, searchmode mode, int maxHP)
        {
            if (maxHP < 1 || minions.Count == 0) return null;

            Minion ret = null; // 用于存储最终返回的随从
            double value = 0;  // 用于存储当前比较值
            int retVal = 0;    // 用于存储次要比较值，如攻击力或生命值

            // 遍历随从列表，根据模式进行搜索
            foreach (Minion m in minions)
            {
                if (m.Hp > maxHP) continue; // 忽略生命值大于maxHP的随从

                switch (mode)
                {
                    case searchmode.searchHighestHP:
                        if (m.Hp > value)
                        {
                            ret = m;
                            value = m.Hp;
                            retVal = m.Angr;
                        }
                        else if (m.Hp == value && retVal < m.Angr)
                        {
                            ret = m;
                        }
                        break;

                    case searchmode.searchLowestAttack:
                        if (m.Angr < value)
                        {
                            ret = m;
                            value = m.Angr;
                            retVal = m.Hp;
                        }
                        else if (m.Angr == value && retVal < m.Hp)
                        {
                            ret = m;
                        }
                        break;

                    case searchmode.searchHighestAttack:
                        if (m.Angr > value)
                        {
                            ret = m;
                            value = m.Angr;
                            retVal = m.Hp;
                        }
                        else if (m.Angr == value && retVal < m.Hp)
                        {
                            ret = m;
                        }
                        break;

                    case searchmode.searchHighAttackLowHP:
                        if ((double)m.Angr / m.Hp > value)
                        {
                            ret = m;
                            value = (double)m.Angr / m.Hp;
                            retVal = m.Hp;
                        }
                        else if ((double)m.Angr / m.Hp == value && retVal < m.Hp)
                        {
                            ret = m;
                        }
                        break;

                    case searchmode.searchHighHPLowAttack:
                        if (m.Angr == 0)
                        {
                            if (ret == null) ret = m;
                            break;
                        }
                        if ((double)m.Hp / m.Angr > value)
                        {
                            ret = m;
                            value = (double)m.Hp / m.Angr;
                            retVal = m.Hp;
                        }
                        else if ((double)m.Angr / m.Hp == value && retVal < m.Hp)
                        {
                            ret = m;
                        }
                        break;

                    default: // searchHighestHP为默认情况
                        if (m.Hp > value)
                        {
                            ret = m;
                            value = m.Hp;
                            retVal = m.Angr;
                        }
                        else if (m.Hp == value && retVal < m.Angr)
                        {
                            ret = m;
                        }
                        break;
                }
            }

            // 如果找到的随从生命值小于等于0，则返回null
            return ret != null && ret.Hp > 0 ? ret : null;
        }

        /// <summary>
        /// 随机选择一个敌方角色作为单次伤害的目标，优先选择合适的随从。
        /// </summary>
        /// <param name="damage">伤害值。</param>
        /// <param name="onlyMinions">是否只选择随从作为目标。</param>
        /// <returns>选择的目标随从。</returns>
        public Minion getEnemyCharTargetForRandomSingleDamage(int damage, bool onlyMinions = false)
        {
            Minion target = null;
            int tmp = this.guessingHeroHP; // 记录猜测的英雄血量
            this.guessHeroDamage(); // 重新计算可能的英雄伤害值

            if (this.guessingHeroHP < 0) // 如果猜测的英雄血量为负数，可能是受到严重威胁
            {
                target = this.searchRandomMinionByMaxHP(this.enemyMinions, searchmode.searchHighestAttack, damage); // 尝试寻找高攻击力的随从作为目标
                if ((target == null || this.enemyHero.Hp <= damage) && !onlyMinions)
                {
                    target = this.enemyHero; // 如果没有找到合适的随从且敌方英雄生命值低，则选择敌方英雄
                }
            }
            else
            {
                // 根据伤害值选择目标，伤害值高优先选择低血量目标，伤害值低优先选择高血量目标
                target = this.searchRandomMinion(this.enemyMinions, damage > 3 ? searchmode.searchLowestHP : searchmode.searchHighestHP);
                if (target == null && !onlyMinions)
                {
                    target = this.enemyHero; // 如果没有找到合适的随从则选择敌方英雄
                }
            }
            this.guessingHeroHP = tmp; // 恢复原先猜测的英雄血量
            return target;
        }

        public CardDB.Card getRandomCardForManaMinion(int mana)
        {
            List<CardDB.Card> cards = new List<CardDB.Card>();
            foreach (CardDB.Card c in CardDB.Instance.cardlist)
            {
                if (c.type == CardDB.cardtype.MOB && c.cost == mana && c.cardIDenum != CardDB.cardIDEnum.None)
                {
                    cards.Add(c);
                }
            }
            if (cards.Count == 0)
            {
                foreach (CardDB.Card c in CardDB.Instance.cardlist)
                {
                    if (c.type == CardDB.cardtype.MOB && c.cost <= mana && c.cardIDenum != CardDB.cardIDEnum.None)
                    {
                        cards.Add(c);
                    }
                }
            }
            if (cards.Count == 0) return CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.None);
            return cards[this.getRandomNumber(0, cards.Count - 1)];
        }

        public List<CardDB.cardIDEnum> CheckTurnDeckForType(CardDB.cardtype type, int count)
        {
            List<CardDB.cardIDEnum> retval = new List<CardDB.cardIDEnum>();
            if (prozis.turnDeck != null)
            {
                foreach (KeyValuePair<CardDB.cardIDEnum, int> entry in prozis.turnDeck)
                {
                    CardDB.Card card = CardDB.Instance.getCardDataFromID(entry.Key);
                    if (card.type == type)
                    {
                        retval.Add(entry.Key);
                        if (retval.Count >= count) break;
                    }
                }
            }
            return retval;
        }

        public CardDB.cardIDEnum CheckTurnDeckExists(TAG_RACE race)
        {
            if (prozis.turnDeck != null)
            {
                foreach (KeyValuePair<CardDB.cardIDEnum, int> entry in prozis.turnDeck)
                {
                    CardDB.Card card = CardDB.Instance.getCardDataFromID(entry.Key);
                    if ((TAG_RACE)card.race == race)
                    {
                        return entry.Key;
                    }
                }
            }
            return CardDB.cardIDEnum.None;
        }
    }
}
