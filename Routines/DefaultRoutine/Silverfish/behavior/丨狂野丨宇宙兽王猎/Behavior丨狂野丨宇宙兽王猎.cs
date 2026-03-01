using System.Collections.Generic;
using System;
using System.Linq;

namespace HREngine.Bots
{
    /// <summary>
    /// 便宜策略使用需谨慎
    /// </summary>
    public partial class Behavior丨狂野丨宇宙兽王猎 : Behavior
    {
        /// <summary>
        /// 敌方奖励
        /// </summary>
        private int bonus_enemy = 4;
        /// <summary>
        /// 我方奖励
        /// </summary>
        private int bonus_mine = 4;
        // 危险血线
        private int hpboarder = 15;
        // 抢脸血线
        private int aggroboarder = 15;

        // private static readonly ILog ilog_0 = Common.LogUtilities.Logger.GetLoggerInstanceForType();
        public override string BehaviorName() { return "丨狂野丨宇宙兽王猎"; }
        PenalityManager penman = PenalityManager.Instance;

        public override int getComboPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {

            // 无法选中
            if (target != null && target.untouchable)
            {
                return 100000;
            }
            // 初始惩罚值
            int penalty = 0;
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.蛇油:
                    {
                        if (p.spellpower > 1)
                        {
                            penalty -= p.spellpower * 20;

                        }
                        else
                        {
                            penalty += 100;

                        }

                    }
                    break;
                case CardDB.cardNameCN.毒蛇花:
                    {
                        if (target != null)
                        {

                            if (target.handcard.card.nameCN == CardDB.cardNameCN.恐鳞 || target.handcard.card.nameCN == CardDB.cardNameCN.恐鳞_WON_025)
                            {
                                penalty -= 50;
                                foreach (Minion m in p.enemyMinions)
                                {
                                    penalty -= 20;
                                }
                            }
                            else
                            {
                                penalty += 10;
                            }
                        }
                    }
                    break;
                case CardDB.cardNameCN.奇迹推销员:
                case CardDB.cardNameCN.跳虫_SC_010:
                case CardDB.cardNameCN.潜踪群蛇:
                    {
                        penalty -= 30;
                    }
                    break;
                case CardDB.cardNameCN.击伤猎物:
                    {
                        if (target != null)
                        {

                            if (target.Hp == 1)
                            {
                                penalty -= 30;
                            }
                            else if (target.divineshild)
                            {
                                penalty -= 20;
                            }
                            //满场亏一个111
                            if (p.ownMinions.Count == 7)
                            {
                                penalty += 20;
                            }
                        }
                    }
                    break;
                case CardDB.cardNameCN.神话观测者:
                    {
                        penalty -= 10;
                        foreach (Minion m in p.ownMinions)
                        {
                            penalty -= 20;
                        }
                    }
                    break;
                case CardDB.cardNameCN.选种饲养员:
                    {
                        // if(p.prozis)

                    }
                    break;
                case CardDB.cardNameCN.异教低阶牧师:
                case CardDB.cardNameCN.音箱践踏者:
                case CardDB.cardNameCN.洛欧塞布:
                case CardDB.cardNameCN.锋鳞:

                    {
                        penalty -= 20;
                        if (getPlayfieldValue(p) > 30)
                        {
                            //场面好就封魔
                            penalty -= 40;
                        }

                    }
                    break;
                case CardDB.cardNameCN.灵体偷猎者:
                    {
                        penalty -= 40;

                    }
                    break;

                case CardDB.cardNameCN.野性之魂:
                    {
                        penalty -= 40;
                        if (p.ownMinions.Count > 6)
                        {
                            //满场不下
                            penalty += 100;
                        }
                        else if (p.ownMinions.Count == 6)
                        {
                            //6个格子，下了小亏
                            penalty += 20;
                        }

                    }
                    break;

                case CardDB.cardNameCN.艾拉隆:
                    {
                        if (p.ownMinions.Count == 6)
                        {
                            //满场下很亏
                            penalty += 66;
                        }
                        else if (p.ownMinions.Count == 5)
                        {
                            //6个格子，下了小亏
                            penalty += 44;
                        }
                        else if (p.ownMinions.Count == 4)
                        {
                            penalty += 22;

                        }
                        else
                        {
                            penalty -= 40;
                        }
                    }
                    break;

                case CardDB.cardNameCN.水晶养护工:
                    {
                        if (p.ownMaxMana <= p.enemyMaxMana)
                        {
                            penalty -= 40;
                        }
                        else
                        {
                            penalty += 20;
                        }
                    }
                    break;
                case CardDB.cardNameCN.迷失者塞尔杜林:
                    {
                        penalty -= 20;
                        foreach (Minion m in p.enemyMinions)
                        {
                            if (m.Hp <= 3 && !m.divineshild)
                            {
                                penalty -= 20;
                            }
                            else
                            {
                                penalty -= 10;
                            }
                        }
                    }
                    break;
                case CardDB.cardNameCN.酸喉:
                case CardDB.cardNameCN.酸喉_WON_024:
                    {
                        if (p.ownMinions.Any((m) => (m.handcard.card.nameCN == CardDB.cardNameCN.恐鳞 || m.handcard.card.nameCN == CardDB.cardNameCN.恐鳞_WON_025)))
                        {
                            penalty -= 60;
                        }
                        else
                        {
                            penalty -= 20;
                        }
                    }
                    break;
                case CardDB.cardNameCN.恐鳞:
                case CardDB.cardNameCN.恐鳞_WON_025:
                    {
                        if (p.ownMinions.Any((m) => (m.handcard.card.nameCN == CardDB.cardNameCN.酸喉 || m.handcard.card.nameCN == CardDB.cardNameCN.酸喉_WON_024)))
                        {
                            penalty -= 60;
                        }
                        else if (p.owncards.Any((hc) => hc.card.nameCN == CardDB.cardNameCN.毒蛇花))
                        {
                            penalty -= 60;
                        }
                        else
                        {
                            foreach (Minion m in p.enemyMinions)
                            {
                                if (m.Hp == 1 || m.divineshild)
                                {
                                    penalty -= 12;
                                }
                            }
                        }
                    }
                    break;
                case CardDB.cardNameCN.大主教奈丽:
                    {
                        penalty -= 23;
                    }
                    break;
                case CardDB.cardNameCN.导航员伊莉斯:
                    {
                        penalty -= 25;
                    }
                    break;
                case CardDB.cardNameCN.苔原犀牛:
                    {
                        penalty += 30;
                    }
                    break;
                case CardDB.cardNameCN.雷诺杰克逊:
                    {
                        if (p.ownHero.Hp <= 18)
                        {
                            penalty -= 60;
                        }
                        penalty -= 20;
                    }
                    break;
                case CardDB.cardNameCN.奎尔萨拉斯的希望:
                    {
                        if (p.ownWeapon == null)
                        {
                            penalty += 30;
                        }
                        else
                        {
                            penalty -= 30;

                        }
                        foreach (Minion m in p.ownMinions)
                        {
                            penalty -= 10;
                        }
                    }
                    break;
                case CardDB.cardNameCN.调酒师鲍勃:
                    {
                        if (p.enemyMinions.Count > 3)
                        {
                            penalty -= 37;
                        }
                    }
                    break;
                case CardDB.cardNameCN.海卓拉顿:
                    {
                        penalty -= 40;
                        if (p.ownMinions.Count > 5)
                        {
                            penalty += 20;
                        }
                    }
                    break;
                case CardDB.cardNameCN.兽王莱欧洛克斯:
                    {
                        int pets = 0;
                        foreach (Handmanager.Handcard handcard in p.owncards)
                        {
                            if (RaceUtils.MinionBelongsToRace(handcard.card.GetRaces(), CardDB.Race.PET))
                            {
                                pets++;
                            }
                        }
                        if (pets == 0)
                        {
                            //手上没野兽不出
                            penalty += 100;
                        }
                        else if (pets == 1)
                        {
                            //只有一个野兽有点小亏
                            penalty += 20;
                        }
                        else if (pets >= 2)
                        {
                            //两个或两个以上下了不亏
                            penalty -= 80;
                        }
                    }
                    break;
                case CardDB.cardNameCN.荆棘谷之心:
                    {
                        int diePet = 0;
                        foreach (GraveYardItem m in p.diedMinions)
                        {
                            CardDB.Card dieCard = CardDB.Instance.getCardDataFromID(m.cardid);
                            if (RaceUtils.MinionBelongsToRace(dieCard.GetRaces(), CardDB.Race.PET) && dieCard.cost >= 5)
                            {
                                diePet++;
                            }
                        }
                        if (diePet == 0)
                        {
                            penalty += 100;
                        }
                        else
                        {
                            penalty -= diePet * 30;
                        }
                    }
                    break;
                case CardDB.cardNameCN.戈德林:
                    {
                        foreach (Minion m in p.ownMinions)
                        {
                            if (RaceUtils.MinionBelongsToRace(m.handcard.card.GetRaces(), CardDB.Race.PET))
                            {
                                penalty -= m.Angr * 10;
                            }
                        }
                    }
                    break;
                default:
                    penalty = 0; // 如果卡牌名称不匹配，使用初始惩罚值
                    break;
            }
            return penalty;
        }

        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value > -200000) return p.value;
            float retval = 0;
            retval += getGeneralVal(p);
            retval += getHpValue(p, hpboarder, aggroboarder);
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;
            bool useAb = false;
            // 排序问题！！！！
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    case actionEnum.trade:
                        a.penalty += 200;
                        continue;
                    case actionEnum.useLocation:
                        a.penalty += 200;
                        continue;
                    case actionEnum.useTitanAbility:
                        a.penalty += 200;
                        continue;
                    case actionEnum.forge:
                        a.penalty += 200;
                        continue;
                    // 英雄攻击
                    case actionEnum.attackWithHero:
                        if (p.ownWeapon.card.nameCN == CardDB.cardNameCN.奎尔萨拉斯的希望)
                        {
                            a.penalty += 50;
                        }
                        continue;
                    case actionEnum.useHeroPower:
                        if (p.ownHeroAblility.card.nameCN == CardDB.cardNameCN.稳固射击)
                        {
                            if (p.enemyHero.Hp <= 10)
                            {
                                a.penalty += 50;

                            }
                        }
                        else if (p.ownHeroAblility.card.nameCN == CardDB.cardNameCN.追踪术_GDB_846hp)
                        {
                            if (p.owncards.Count < 7)
                            {
                                a.penalty += 20;

                            }
                            else
                            {
                                a.penalty -= 50;
                            }

                        }
                        useAb = true;
                        break;
                    case actionEnum.playcard:
                        break;
                    default:
                        continue;
                }
                switch (a.card.card.nameCN)
                {
                    case CardDB.cardNameCN.幸运币:
                        retval -= i;
                        break;
                }
            }
            // 对手基本随从交换模拟
            // retval += enemyTurnPen(p);
            retval -= p.lostDamage;
            retval += getSecretPenality(p); // 奥秘的影响
            retval -= p.enemyWeapon.Angr * 3 + p.enemyWeapon.Durability * 3;
            return retval;
        }


        // 敌方随从价值 主要等于 （HP + Angr） * 4  
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    dieNextTurn = true;
                    break;
                }
            }
            foreach (CardDB.cardIDEnum s in p.ownSecretsIDList)
            {
                if (s == CardDB.cardIDEnum.EX1_610 || s == CardDB.cardIDEnum.VAN_EX1_610)
                {
                    if (m.Hp <= 2)
                    {
                        dieNextTurn = true;
                        break;
                    }
                }
            }
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            if (m.Hp <= 0) return 0;
            int retval = 4;
            if (m.Angr > 0 || p.enemyHeroStartClass == TAG_CLASS.PRIEST)
                retval += m.Hp * bonus_enemy;
            retval += m.spellpower * bonus_enemy * 3 / 2;
            if (!m.frozen && !m.cantAttack)
            {
                retval += m.Angr * bonus_enemy;
                if (m.windfury) retval += m.Angr * bonus_enemy / 2;
            }
            if (m.silenced) return retval;

            //嘲讽
            if (m.taunt) retval += 2;
            //圣盾
            if (m.divineshild) retval += m.Angr * 2;
            //圣盾嘲讽
            if (m.divineshild && m.taunt) retval += 5;
            //潜行
            if (m.stealth) retval += 2;
            //扰魔
            if (m.Elusive) retval += 5;
            //复生
            if (m.reborn) retval += 5;
            //法术迸发
            if (m.Spellburst) retval += 5;
            //暴怒
            if (m.Frenzy) retval += 5;
            //荣誉消灭
            if (m.HonorableKill) retval += 5;
            //超杀
            if (m.Overkill) retval += 5;
            //吸血
            if (m.lifesteal) retval += m.Angr * bonus_enemy;
            // 剧毒
            if (m.poisonous)
            {
                retval += 10;
                if (p.ownMinions.Count < p.enemyMinions.Count) retval += 15;
            }
            //光环
            if (m.Aura) retval += 30;
            if (m.TriggerVisual) retval += 40;
            if (m.dormant > 0)
            {
                retval -= bonus_mine * m.dormant;
            }
            // 血量越低，解怪优先度越高
            if (p.ownHero.Hp <= 15)
            {
                retval += (16 - p.ownHero.Hp) * 3;
                if (p.ownHero.Hp <= 6) retval *= 2;
            }
            return retval;
        }
        /// <summary>
        /// 我方随从价值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public override int getMyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    dieNextTurn = true;
                    break;
                }
            }
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            int retval = 5;
            if (m.Hp <= 0) return 0;
            retval += m.Hp * bonus_mine;
            retval += m.Angr * bonus_mine;
            if (m.Hp <= 1 && !m.divineshild) retval -= (m.Angr - 1) * (bonus_mine - 1);
            // 高攻低血是垃圾
            if (m.Angr > m.Hp + 4) retval -= (m.Angr - m.Hp) * (bonus_mine - 1);
            // 风怒价值
            if ((!m.playedThisTurn || m.rush == 1 || m.charge == 1) && m.windfury) retval += m.Angr;
            // 圣盾价值
            if (m.divineshild) retval += m.Angr * 3;
            // 潜行价值
            if (m.stealth) retval += m.Angr / 2 + 1;
            // 吸血
            if (m.lifesteal) retval += m.Angr / 2 + 1;

            // 圣盾嘲讽
            if (m.divineshild && m.taunt) retval += 4;
            //扰魔
            if (m.Elusive) retval += 5;
            //复生
            if (m.reborn) retval += 5;
            //法术迸发
            if (m.Spellburst) retval += 5;
            //暴怒
            if (m.Frenzy) retval += 5;
            //荣誉消灭
            if (m.HonorableKill) retval += 5;
            //超杀
            if (m.Overkill) retval += 5;
            // 剧毒
            if (m.poisonous) retval += 10;
            //光环
            if (m.Aura) retval += 30;
            if (m.TriggerVisual) retval += 40;
            if (m.dormant > 0)
            {
                retval -= bonus_mine * m.dormant;
            }

            switch (m.handcard.card.nameCN)
            {

                case CardDB.cardNameCN.神话观测者:
                    retval += 3 * p.ownMinions.Count * 12;
                    break;
                case CardDB.cardNameCN.苔原犀牛:
                    retval += 44;
                    break;
                case CardDB.cardNameCN.戈德林:
                    retval += 70;
                    break;
                case CardDB.cardNameCN.海卓拉顿:
                    if (p.ownMinions.Any((minion) => (minion.handcard.card.nameCN == CardDB.cardNameCN.海卓拉顿之头 || minion.handcard.card.nameCN == CardDB.cardNameCN.海卓拉顿之头_TSC_950t2)))
                    {
                        retval += 50;
                    }

                    break;
            }
            return retval;
        }

        public override int getSirFinleyPriority(List<Handmanager.Handcard> discoverCards)
        {

            return -1; //comment out or remove this to set manual priority
            int sirFinleyChoice = -1;
            int tmp = int.MinValue;
            for (int i = 0; i < discoverCards.Count; i++)
            {
                CardDB.cardNameEN name = discoverCards[i].card.nameEN;
                if (SirFinleyPriorityList.ContainsKey(name) && SirFinleyPriorityList[name] > tmp)
                {
                    tmp = SirFinleyPriorityList[name];
                    sirFinleyChoice = i;
                }
            }
            return sirFinleyChoice;
        }
        public override int getSirFinleyPriority(CardDB.Card card)
        {
            return SirFinleyPriorityList[card.nameEN];
        }

        private Dictionary<CardDB.cardNameEN, int> SirFinleyPriorityList = new Dictionary<CardDB.cardNameEN, int>
        {
            //{HeroPowerName, Priority}, where 0-9 = manual priority
            { CardDB.cardNameEN.lesserheal, 0 },
            { CardDB.cardNameEN.shapeshift, 6 },
            { CardDB.cardNameEN.fireblast, 7 },
            { CardDB.cardNameEN.totemiccall, 1 },
            { CardDB.cardNameEN.lifetap, 9 },
            { CardDB.cardNameEN.daggermastery, 5 },
            { CardDB.cardNameEN.reinforce, 4 },
            { CardDB.cardNameEN.armorup, 2 },
            { CardDB.cardNameEN.steadyshot, 8 }
        };

        public override int getHpValue(Playfield p, int hpboarder, int aggroboarder)
        {
            int offset_enemy = 0;

            int retval = 0;
            // 血线安全
            if (p.ownHero.Hp + p.ownHero.armor > hpboarder)
            {
                retval += (5 + p.ownHero.Hp + p.ownHero.armor - hpboarder);
            }
            // 快死了
            else
            {
                //if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                retval -= 5 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
            }
            if (p.ownHero.Hp + p.ownHero.armor < 10 && p.ownHero.Hp + p.ownHero.armor > 0)
            {
                retval -= 200 / (p.ownHero.Hp + p.ownHero.armor);
            }
            // 对手血线安全
            if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy >= aggroboarder)
            {
                retval += 2 * (aggroboarder - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 开始打脸
            else
            {
                retval += 4 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 场攻+直伤大于对方生命，预计完成斩杀
            if (p.anzEnemyTaunt == 0 && p.calTotalAngr() + p.calDirectDmg(p.mana, false) >= p.enemyHero.Hp + p.enemyHero.armor)
            {
                retval += 2000;
            }
            return retval;
        }

        /// <summary>
        /// 获取使用地标的惩罚值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="target"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public override int getUseLocationPenality(Minion m, Minion target, Playfield p)
        {
            int penalty = 0; // 初始惩罚值为 0

            switch (m.handcard.card.nameCN.ToString())
            {
                case "树篱迷宫":
                    penalty = -47; // 优先级数值转为负值作为惩罚值
                    break;
                case "远足步道":
                    penalty = -49;
                    break;
                case "尤格萨隆的监狱":
                    penalty = -52;
                    break;
                case "惊险悬崖":
                    penalty = -68;
                    break;
                case "鹦鹉乐园":
                    penalty = -59;
                    break;
                case "大地之末号":
                    penalty = -59;
                    break;
                case "潮汐之地":
                    penalty = -50;
                    break;
                case "小玩物小屋":
                    penalty = -48;
                    break;
                default:
                    penalty = -200; // 如果卡牌名称不匹配，使用初始惩罚值
                    break;
            }
            return penalty;
        }

        /// <summary>
        /// 获取使用泰坦技能的惩罚值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="target"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public override int getUseTitanAbilityPenality(Minion m, Minion target, Playfield p)
        {
            int penalty = -100;
            return penalty;
        }

        /// <summary>
        /// 发现卡的价值
        /// </summary>
        /// <param name="card"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public override int getDiscoverVal(CardDB.Card card, Playfield p)
        {
            //初始化Hsreplay数据
            Hsreplay hs = Hsreplay.Instance;

            // 从对应职业的数据列表中找到匹配的卡牌数据
            CardStats cardStats = Hsreplay.AllCardStats.FirstOrDefault(c => c.DbfId == card.dbfId);

            if (cardStats != null)
            {
                Helpfunctions.Instance.logg("getDiscoverVal - 使用Hsreplay数据比对" + card.nameCN.ToString() + " => " + cardStats.WinrateWhenDrawn);
                // ilog_0.Info("getDiscoverVal - 使用Hsreplay数据比对" + card.nameCN.ToString() + " => " + cardStats.WinrateWhenDrawn);
                // 返回 WinrateWhenDrawn 的整数部分
                return (int)cardStats.WinrateWhenDrawn;
            }

            // 如果找不到对应的卡牌数据，或者职业数据不存在，则返回默认值
            return 0;
        }

    }
}