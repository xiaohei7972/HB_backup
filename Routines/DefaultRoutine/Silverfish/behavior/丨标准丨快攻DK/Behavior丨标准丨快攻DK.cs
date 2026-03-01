using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public partial class Behavior丨标准丨快攻DK : Behavior
    {
        private int bonus_enemy = 4;
        private int bonus_mine = 4;

        public override string BehaviorName() { return "丨标准丨快攻DK"; }
        PenalityManager penman = PenalityManager.Instance;





        /// <summary>
        /// 标准快攻DK的留牌策略
        /// </summary>
        /// <param name="cards">起手卡牌列表</param>
        public override void specialMulligan(List<Mulligan.CardIDEntity> cards)
        {
            int 怪异魔蚊 = 0;
            int 暴行祭礼 = 0;
            int 火羽精灵 = 0;
            int 病变虫群 = 0;
            int 鱼人木乃伊 = 0;
            int 恐惧猎犬训练师 = 0;
            int 感染吐息 = 0;
            int 疯狂生物 = 0;
            int 堕寒男爵 = 0;
            int 秘迹观测者 = 0;
            

            foreach (Mulligan.CardIDEntity card in cards)
            {
                CardDB.Card cardCN = CardDB.Instance.getCardDataFromID(card.id);
                if (cardCN.nameCN == CardDB.cardNameCN.怪异魔蚊)
                {
                    怪异魔蚊 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.暴行祭礼)
                {
                    暴行祭礼 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.火羽精灵)
                {
                    火羽精灵 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.病变虫群)
                {
                    病变虫群 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.鱼人木乃伊)
                {
                    鱼人木乃伊 += 1;
                }
                 if (cardCN.nameCN == CardDB.cardNameCN.恐惧猎犬训练师)
                {
                    恐惧猎犬训练师 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.感染吐息)
                {
                    感染吐息 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.疯狂生物)
                {
                    疯狂生物 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.堕寒男爵)
                {
                    堕寒男爵 += 1;
                }
                if (cardCN.nameCN == CardDB.cardNameCN.堕寒男爵)
                {
                    秘迹观测者 += 1;
                }
            }

            foreach (Mulligan.CardIDEntity card in cards)
            {
                CardDB.Card cardCN = CardDB.Instance.getCardDataFromID(card.id);

                if (cardCN.nameCN == CardDB.cardNameCN.怪异魔蚊)
                {
                    if (cards.Count >= 3)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "留一张怪异魔蚊";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = 2;
                                tmp.holdReason = "按规则留第二张怪异魔蚊，对面没解牌就后手压死对手";
                            }
                        }
                    }
                }

                if (cardCN.nameCN == CardDB.cardNameCN.暴行祭礼)
                {
                    if (cards.Count >= 3)
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不留暴行祭礼，没尸体还亏节奏";
                        
                    }
                }

                if (cardCN.nameCN == CardDB.cardNameCN.火羽精灵)
                {
                     if (cards.Count >= 3)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "留一张火羽精灵";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张火羽精灵，留一张配合533就好";
                            }
                        }
                    }                  
                }

                if (cardCN.nameCN == CardDB.cardNameCN.病变虫群)
                {
                    if (cards.Count >= 3)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "留一张病变虫群";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = 2;
                                tmp.holdReason = "按规则留第二张病变虫群超展开";
                            }
                        }
                    }
                }

                if (cardCN.nameCN == CardDB.cardNameCN.鱼人木乃伊)
                {
                    if (cards.Count >= 3)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "留一张鱼人木乃伊";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张鱼人木乃伊，两张太呆了";
                            }
                        }
                    }

                    if (cards.Count >= 3 && 火羽精灵 + 病变虫群 >= 1)
                    {
                        card.holdByRule = -2;
                        card.holdReason = "按特殊规则丢弃鱼人木乃伊，因为有更优质的展开";
                    }              
                }


                if (cardCN.nameCN == CardDB.cardNameCN.恐惧猎犬训练师)
                {
                    if (cards.Count >= 3 && 怪异魔蚊 + 火羽精灵 + 病变虫群 + 鱼人木乃伊 >= 1)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "先后手有能用的1费牌留1张恐惧猎犬训练师";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张卡恐惧猎犬训练师";
                            }
                        }
                    }
                    else
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不符合特殊规则不留恐惧猎犬训练师";
                    }                  
                }

                if (cardCN.nameCN == CardDB.cardNameCN.感染吐息)
                {
                    if (cards.Count >= 3 && 怪异魔蚊 + 火羽精灵 + 病变虫群 + 鱼人木乃伊 >= 1)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "先后手有能用的1费牌留1张感染吐息";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张感染吐息";
                            }
                        }
                    }
                    else
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不符合特殊规则不留感染吐息";
                    }                  
                }
                


                if (cardCN.nameCN == CardDB.cardNameCN.疯狂生物)
                {
                    if (cards.Count >= 3 && 怪异魔蚊 + 火羽精灵 + 病变虫群 + 鱼人木乃伊 >= 1)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "先后手有能用的1费牌留1张疯狂生物";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张疯狂生物";
                            }
                        }
                    }
                    else
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不符合特殊规则不留疯狂生物";
                    }                  
                }

                if (cardCN.nameCN == CardDB.cardNameCN.堕寒男爵)
                {
                    if (cards.Count >= 3 && 怪异魔蚊 + 火羽精灵 + 病变虫群 + 鱼人木乃伊 + 恐惧猎犬训练师 + 感染吐息 + 感染吐息 + 疯狂生物>= 2)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "能按费拍怪留1张堕寒男爵过牌";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张堕寒男爵";
                            }
                        }
                    }
                    else
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不符合特殊规则不留堕寒男爵";
                    }                  
                }

                if (cardCN.nameCN == CardDB.cardNameCN.秘迹观测者)
                {
                    if (cards.Count >= 3 && 怪异魔蚊 + 火羽精灵 + 病变虫群 + 鱼人木乃伊 + 恐惧猎犬训练师 + 感染吐息 + 感染吐息 + 疯狂生物>= 2)
                    {
                        card.holdByRule = 2;
                        card.holdReason = "能按费拍怪留1张秘迹观测者控场";
                        foreach (Mulligan.CardIDEntity tmp in cards)
                        {
                            if (tmp.entitiy == card.entitiy) continue;
                            if (tmp.id == card.id)
                            {
                                tmp.holdByRule = -2;
                                tmp.holdReason = "按规则丢弃第二张秘迹观测者";
                            }
                        }
                    }
                    else
                    {
                        card.holdByRule = -2;
                        card.holdReason = "不符合特殊规则不留秘迹观测者";
                    }                  
                }

                //剩下大于等于4费的卡hb是默认不留的
            }       
        }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public override int getComboPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            // 无法选中(值越大越不会打出)
            if (target != null && target.untouchable)
            {
                return 100000;
            }
            // 初始惩罚值（负为优先打出该牌，正为低优先打出该牌）
            int pen = 0;
            
            //此处为单卡描述
            switch (card.nameCN)		
            {   

                case CardDB.cardNameCN.幸运币:
                foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if ( hc.card.nameCN == CardDB.cardNameCN.怪异魔蚊 && p.ownMinions.Count >= 1 )
                            {

                            pen -= 100;

                            }
                        }
                break;
                case CardDB.cardNameCN.怪异魔蚊:
                if (p.ownMinions.Count >= 2 )  pen = -60;
                pen -= 6;
                break;

                case CardDB.cardNameCN.暴行祭礼:
                if (p.getCorpseCount() < 2) // 如果墓地里的尸体小于5 不打出此牌。 
                    {
                        pen += 500;
                    }
                    if (p.getCorpseCount() >= 2 && p.ownMaxMana >= 4)   
                    {
                        pen -= 25;
                    }
                break;

                case CardDB.cardNameCN.火羽精灵:
                break;

                case CardDB.cardNameCN.病变虫群:
                foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if ( hc.card.nameCN == CardDB.cardNameCN.怪异魔蚊 || hc.card.nameCN == CardDB.cardNameCN.火羽精灵 || hc.card.nameCN == CardDB.cardNameCN.鱼人木乃伊)
                            {

                            pen -= 10;

                            }
                        }
                        pen -= 7;  
                break;

                case CardDB.cardNameCN.鱼人木乃伊:
                break;
                
                case CardDB.cardNameCN.恐惧猎犬训练师:
				break;
                
                case CardDB.cardNameCN.感染吐息:
                if (p.ownMaxMana == 2 && target.Hp == 3) pen -= 20;
                if (p.ownMaxMana == 2 && target.Hp == 2) pen -= 20;
                int kill_num = 0;
                if (p.enemyMinions.Count > 0)
                    foreach (Minion mi in p.enemyMinions)
                    {
                        if (mi.Hp <= 1)                   
                        {
                        kill_num++;
                        pen -= mi.Angr * mi.Angr;
                        }
                    }
                pen -= kill_num * 10; 
                break;

                case CardDB.cardNameCN.疯狂生物:
                foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if ( hc.card.nameCN == CardDB.cardNameCN.怪异魔蚊 || hc.card.nameCN == CardDB.cardNameCN.火羽精灵 || hc.card.nameCN == CardDB.cardNameCN.鱼人木乃伊 && p.enemyMinions.Count == 0)
                            {

                            pen -= 10;

                            }
                        }
                break;

                case CardDB.cardNameCN.堕寒男爵:
                if (p.ownMaxMana == 3) pen -= 20;
                if (p.owncards.Count <= 3) pen -= 20; //手牌低于3时尽快打出
                if (p.owncards.Count >= 7) pen += 100;
                break;

                case CardDB.cardNameCN.秘迹观测者:
                if (p.ownMinions.Count >= 2 && p.ownMaxMana == 3)  pen = -50;
                if (p.ownMinions.Count >= p.enemyMinions.Count)  pen = -50;
                break;

                case CardDB.cardNameCN.恶毒恐魔:               
                break;
                
                case CardDB.cardNameCN.血虫感染:
                if (p.ownMaxMana == 4) pen -= 30;
                if (p.owncards.Count <= 3) pen -= 20; //手牌低于3时尽快打出
                // 统计敌方场上生命值小于等于1的随从数量，同时减去这些随从攻击力的平方值。
                int kill_num1 = 0;
                if (p.enemyMinions.Count > 0)
                    foreach (Minion mi in p.enemyMinions)
                    {
                        if (mi.Hp <= 1)                   
                        {
                        kill_num1++;
                        pen -= mi.Angr * mi.Angr;
                        }
                    }
                pen -= kill_num1 * 10;               
                break;

                case CardDB.cardNameCN.展馆茶壶:
                if (p.ownMinions.Count >= 3 && p.ownMaxMana == 5)  pen = -150;
                if (p.ownMinions.Count >= 3 )  pen = -80;
                break;

                case CardDB.cardNameCN.丑恶的残躯:
                if (p.ownMinions.Count <= 5 && p.ownMaxMana == 6)  pen = -100;
                if (p.ownMinions.Count <= 5 )  pen = -70;
                int kill3_num = 0;
                if (p.enemyMinions.Count > 0)
                    foreach (Minion mi in p.enemyMinions)
                    {
                        if (mi.Hp <= 3)                   
                        {
                        kill3_num++;
                        pen -= mi.Angr * mi.Angr;
                        }
                    }
                pen -= kill3_num * 10;
                break;

                case CardDB.cardNameCN.气闸破损:
                if (p.getCorpseCount() < 5 || p.ownMinions.Count >= 6 ) // 如果墓地里的尸体小于5 不打出此牌。 
                    {
                        pen += 500;
                    }
                if ( p.getCorpseCount() >= 5 && p.ownMinions.Count <= 5 )   // 撑起龟壳保护随从
                    {
                        pen -= 60;
                    }
                if ( p.getCorpseCount() >= 5 && p.ownMinions.Count <= 5 &&p.enemyMinions.Count >= p.ownMinions.Count)   // 撑起龟壳保护随从
                    {
                        pen -= 90;
                    }
                break;

                case CardDB.cardNameCN.死亡使者萨鲁法尔:
                pen -= 5;
                break;

                case CardDB.cardNameCN.食尸鬼冲锋:
                if (p.ownMinions.Count == 7 )  pen = +700;
                pen += 1;
                break;
        
                }
            return pen;
        }

        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value > -200000) return p.value;
            float retval = 0;
            retval += getGeneralVal(p);
            retval += p.owncarddraw * 5;
            // 危险血线
            int hpboarder = 3;
            // 不考虑法强了
            if (p.enemyHeroName == HeroEnum.mage) retval += 2 * p.enemyspellpower;
            // 抢脸血线
            int aggroboarder = 20;
            retval += getHpValue(p, hpboarder, aggroboarder);
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;
            bool useAb = false;

            bool canBe_flameward = false;

            if (p.anzOldWoman > 0)
            {
                foreach (SecretItem si in p.enemySecretList)  //Todo: 是否要判断己方回合还是敌方回合？？？
                {
                    if (si.canBe_flameward) { canBe_flameward = true; break; }
                }
            }
            bool attacted = false;
            // 排序问题！！！！
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    
                    // 英雄攻击
                    case actionEnum.attackWithMinion:
                    case actionEnum.attackWithHero:
                        if (a.target != null && a.target.isHero)
                        {
                            attacted = true;
                        }
                        continue;
                    case actionEnum.useHeroPower:
                        useAb = true;
                        break;
                    case actionEnum.playcard:
                        break;
                    default:
                        continue;
                }
                if (a.card.card.race == CardDB.Race.PIRATE || a.card.card.nameCN == CardDB.cardNameCN.虚触侍从)
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.handcard.card.nameCN == CardDB.cardNameCN.船载火炮)
                        {
                            retval += 10 - i * 3;
                            break;
                        }
                    }
                //// 出牌排序优先
                switch (a.card.card.nameCN)
                {
                    
                }
            }
            // 对手基本随从交换模拟
            retval -= p.lostDamage;
            //retval += getSecretPenality(p); // 奥秘的影响
            retval -= p.enemyWeapon.Angr * 3 + p.enemyWeapon.Durability * 3;

            // 留着技能下回合出的情况
            if (p.ownMaxMana < 2 && p.ownHeroPowerCostLessOnce <= -99)
            {
                if (!useAb && p.enemyMinions.Count == 0)
                    retval += 20;
            }

            // 特殊：优势防亵渎
            if (retval > 50 && p.enemyHeroStartClass == TAG_CLASS.WARLOCK && p.enemyMinions.Count == 0 && p.ownMinions.Count > 2)
            {
                bool found = false;
                // 从 2 开始防，术士自带一堆 1
                for (int i = 1; i <= 10; i++)
                {
                    found = false;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Hp == i)
                        {
                            retval -= 2 * (i - 1);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        if (i == 1) retval += 10;
                        if (i == 2) retval += 10;
                        break;
                    }
                }
            }
            // 震爆闲着没事可以出
            if (p.owncards.Count <= 4)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.nameCN == CardDB.cardNameCN.心灵震爆 && hc.getManaCost(p) <= p.mana)
                    {
                        retval -= 50;
                    }
                }
            }
             // 如果不攻击就能击杀还有额外奖励哦
            if (!attacted && p.enemyHero.Hp <= 0) retval += 10000;
            //p.value = retval;
            return retval;
        }


        // 发现卡的价值
        public override int getDiscoverVal(CardDB.Card card, Playfield p)
        {
			// Helpfunctions.Instance.logg("发现卡：" + card.nameCN);
			// Helpfunctions.Instance.logg("发现卡类型：" + card.race);
			// Helpfunctions.Instance.logg("发现卡费用：" + card.cost);
            switch (card.nameCN)
            {
    
            //随从

            case CardDB.cardNameCN.萨萨里安:
            case CardDB.cardNameCN.丑恶的残躯:
            case CardDB.cardNameCN.侏儒嚼嚼怪:
            case CardDB.cardNameCN.髓骨使御者:
            return 25;

            case CardDB.cardNameCN.堕寒男爵:
            case CardDB.cardNameCN.尸魔花:
            case CardDB.cardNameCN.喷灯破坏者:
            case CardDB.cardNameCN.团队领袖:
            case CardDB.cardNameCN.大作战狂热玩家:
            case CardDB.cardNameCN.展馆茶杯:
            case CardDB.cardNameCN.秘迹观测者:
            case CardDB.cardNameCN.法瑞克:
            case CardDB.cardNameCN.玩具盗窃恶鬼:
            case CardDB.cardNameCN.泼漆彩鳍鱼人:
            case CardDB.cardNameCN.笨拙的搬运工:
            case CardDB.cardNameCN.血蓟幻术师:
            case CardDB.cardNameCN.躁动的愤怒卫士:
            case CardDB.cardNameCN.达卡莱附魔师:
            case CardDB.cardNameCN.迅猛龙先锋:
            case CardDB.cardNameCN.逃生舱:
            case CardDB.cardNameCN.缝合巨人:
            return 20;

            case CardDB.cardNameCN.死亡金属骑士:
            case CardDB.cardNameCN.毛绒暴暴狗:
            case CardDB.cardNameCN.彩虹裁缝:
            case CardDB.cardNameCN.A3型机械金刚:
            case CardDB.cardNameCN.乌祖尔暴怒者:
            case CardDB.cardNameCN.南海船长:
            case CardDB.cardNameCN.巢群虫后:
            case CardDB.cardNameCN.死亡侍僧:
            case CardDB.cardNameCN.卡牌评级师:
            case CardDB.cardNameCN.可怕的主厨:
            case CardDB.cardNameCN.扮装选手:
            case CardDB.cardNameCN.法夜欺诈者:
            case CardDB.cardNameCN.混蒙畸体:
            case CardDB.cardNameCN.焦油爬行者:
            case CardDB.cardNameCN.爆破工程师:
            case CardDB.cardNameCN.甜蜜雪灵:
            case CardDB.cardNameCN.穆克拉:
            case CardDB.cardNameCN.粗暴的猢狲:
            case CardDB.cardNameCN.红衣指挥官:
            case CardDB.cardNameCN.荆棘帮暴徒:
            case CardDB.cardNameCN.软软多头蛇:
            case CardDB.cardNameCN.锈烂蝰蛇:
            case CardDB.cardNameCN.鱼人领军:           
            return 10;

            case CardDB.cardNameCN.骷髅帮手:
            case CardDB.cardNameCN.恐惧猎犬训练师:
            case CardDB.cardNameCN.蹒跚的僵尸坦克:
            case CardDB.cardNameCN.恶毒恐魔:
            case CardDB.cardNameCN.蛛魔护群守卫:
            case CardDB.cardNameCN.黑棘针线师:
            case CardDB.cardNameCN.死亡使者萨鲁法尔:
            case CardDB.cardNameCN.行程保安:
            case CardDB.cardNameCN.变异生命体:
            case CardDB.cardNameCN.套娃傀儡:
            return 15;

            case CardDB.cardNameCN.扛包收尸人:
            case CardDB.cardNameCN.寒冬先锋:
            case CardDB.cardNameCN.脆骨海盗:
            case CardDB.cardNameCN.滑雪高手:
            case CardDB.cardNameCN.业余傀儡师:
            case CardDB.cardNameCN.小精灵:
            case CardDB.cardNameCN.鱼人木乃伊:
            case CardDB.cardNameCN.狂野炎术师:
            case CardDB.cardNameCN.疯狂的炼金师:
            case CardDB.cardNameCN.血法师萨尔诺斯:
            case CardDB.cardNameCN.黑骑士:
            case CardDB.cardNameCN.冰喉:
            return 5;

            }
            return 0;
        }
        // 敌方随从价值 主要等于（HP + Angr） * 4
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
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            if (m.Hp <= 0) return 0;
            int retval = 0;
            if (m.Angr > 0 || m.taunt || m.handcard.card.race == CardDB.Race.TOTEM || p.enemyHeroStartClass == TAG_CLASS.PALADIN || p.enemyHeroStartClass == TAG_CLASS.PRIEST)
                retval += m.Hp * 4;
            retval += m.spellpower * 2;
            retval += m.Hp * m.Angr / 2;
            if (!m.frozen && (!m.cantAttack || m.handcard.card.nameCN == CardDB.cardNameCN.邪刃豹))
            {
                retval += m.Angr * 4;
                if (m.Angr > 5) retval += 10;
                if (m.windfury) retval += m.Angr * 2;
            }
            if (m.silenced) return retval;

            if (m.taunt) retval += 2;
            if (m.divineshild) retval += m.Angr * 2;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 2;

            // 鱼人
            if (m.handcard.card.race == CardDB.Race.MURLOC) retval += bonus_enemy * 4;

            // 剧毒价值两点属性
            if (m.poisonous)
            {
                retval += 8;
            }
            if (m.lifesteal) retval += m.Angr * bonus_enemy * 4;

            int bonus = 4;
            switch (m.handcard.card.nameCN)
            {
                case CardDB.cardNameCN.巫师学徒:
                case CardDB.cardNameCN.肢体商贩:
                case CardDB.cardNameCN.巨型图腾埃索尔:
                case CardDB.cardNameCN.驻锚图腾:
                case CardDB.cardNameCN.刺豚拳手:
                case CardDB.cardNameCN.空中飞爪:
                case CardDB.cardNameCN.金翼巨龙:
                case CardDB.cardNameCN.安保自动机:
                case CardDB.cardNameCN.机械跃迁者:
                case CardDB.cardNameCN.火焰术士弗洛格尔:
                case CardDB.cardNameCN.对空奥术法师:
                case CardDB.cardNameCN.前沿哨所:
                case CardDB.cardNameCN.战场军官:
                case CardDB.cardNameCN.伯尔纳锤喙:
                case CardDB.cardNameCN.甜水鱼人斥候:
                case CardDB.cardNameCN.塔姆辛罗姆:
                case CardDB.cardNameCN.暗影珠宝师汉纳尔:
                case CardDB.cardNameCN.伦萨克大王:
                case CardDB.cardNameCN.布莱恩铜须:
                case CardDB.cardNameCN.观星者露娜:
                case CardDB.cardNameCN.大法师瓦格斯:
                case CardDB.cardNameCN.火妖:
                case CardDB.cardNameCN.下水道渔人:
                case CardDB.cardNameCN.空中炮艇:
                case CardDB.cardNameCN.船载火炮:
                case CardDB.cardNameCN.火舌图腾:
                case CardDB.cardNameCN.末日预言者:
                case CardDB.cardNameCN.莫尔杉哨所:
                case CardDB.cardNameCN.鱼人领军:
                case CardDB.cardNameCN.南海船长:
                case CardDB.cardNameCN.灭龙弩炮:
                case CardDB.cardNameCN.战马训练师:
                case CardDB.cardNameCN.加基森拍卖师:
                case CardDB.cardNameCN.健谈的调酒师:
                case CardDB.cardNameCN.豪宅管家俄里翁:
                case CardDB.cardNameCN.小鬼骑士:
                case CardDB.cardNameCN.针岩图腾:
                case CardDB.cardNameCN.伴唱机:
                case CardDB.cardNameCN.空气之怒图腾:
                case CardDB.cardNameCN.战场通灵师:
                case CardDB.cardNameCN.纸艺天使:
                case CardDB.cardNameCN.纳亚克海克森:
                case CardDB.cardNameCN.烈焰亡魂:
                case CardDB.cardNameCN.饥饿食客哈姆:
                case CardDB.cardNameCN.箭矢工匠:
                case CardDB.cardNameCN.傀儡大师多里安:
                case CardDB.cardNameCN.脆骨海盗:
                case CardDB.cardNameCN.暗影升腾者:
                case CardDB.cardNameCN.赤红教士:
                case CardDB.cardNameCN.受伤的搬运工:
                case CardDB.cardNameCN.阿米图斯的信徒:
                case CardDB.cardNameCN.隐藏宝石:
                case CardDB.cardNameCN.落难的大法师:
                case CardDB.cardNameCN.虚空协奏者:
                case CardDB.cardNameCN.神话观测者:
                case CardDB.cardNameCN.奥术工匠:
                case CardDB.cardNameCN.食肉魔块:
                case CardDB.cardNameCN.小动物看护者:
                case CardDB.cardNameCN.扭曲的织网蛛:
                    retval += 150;
                    break;
              
                // 不解巨大劣势
                case CardDB.cardNameCN.安娜科德拉:
                case CardDB.cardNameCN.农夫:
                case CardDB.cardNameCN.旗标骷髅:
                case CardDB.cardNameCN.尼鲁巴蛛网领主:
                case CardDB.cardNameCN.凯瑞尔罗姆:
                case CardDB.cardNameCN.暗鳞先知:
                case CardDB.cardNameCN.鲨鳍后援:
                case CardDB.cardNameCN.相位追猎者:
                case CardDB.cardNameCN.鱼人宝宝车队:
                case CardDB.cardNameCN.饥饿的秃鹫:
                case CardDB.cardNameCN.锈水海盗:
                case CardDB.cardNameCN.盛装歌手:
                case CardDB.cardNameCN.玛克扎尔的小鬼:
                case CardDB.cardNameCN.发明机器人:
                case CardDB.cardNameCN.宝藏经销商:
                case CardDB.cardNameCN.随船外科医师:
                case CardDB.cardNameCN.玩具船:
                case CardDB.cardNameCN.淘金客:
                case CardDB.cardNameCN.雏龙牧人:
                    retval += 50;
                    break;

                // 算有点用
                case CardDB.cardNameCN.贪婪的书虫:
                case CardDB.cardNameCN.治疗图腾:
                case CardDB.cardNameCN.力量图腾:
                case CardDB.cardNameCN.神秘女猎手:
                case CardDB.cardNameCN.矮人神射手:
                case CardDB.cardNameCN.低阶侍从:
                case CardDB.cardNameCN.战斗邪犬:
                case CardDB.cardNameCN.法力浮龙:
                case CardDB.cardNameCN.飞刀杂耍者:
                    retval += 15;
                    break;

                case CardDB.cardNameCN.白银之手新兵:
                case CardDB.cardNameCN.脆弱的食尸鬼:
                case CardDB.cardNameCN.伊丽扎刺刃:
                case CardDB.cardNameCN.石心之王:
                case CardDB.cardNameCN.神秘的蛋:
                    retval -= 150;
                    break;
                
                case CardDB.cardNameCN.侏儒飞行员诺莉亚:
                if (p.anzEnemyTaunt == 0 && p.calTotalAngr()  >= p.enemyHero.Hp + p.enemyHero.armor)
                    retval -= 150;
                    break;
               
                   
                

                
                
            }
            return retval;
        }

        // 我方随从价值，大致等于主要等于 （HP + Angr） * 4 
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
            if (m.Hp <= 0) return 0;
            int retval = 5;
            if (m.Hp <= 0) return 0;
            retval += m.Hp * 4;
            retval += m.Angr * 4;
            retval += m.Hp * m.Angr / 2;
            // 高攻低血是垃圾
            if (m.Angr > m.Hp + 4) retval -= (m.Angr - m.Hp) * 3;

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

            int bonus = 4;
            switch (m.handcard.card.nameCN)
            {
                case CardDB.cardNameCN.虚触侍从:
                    retval += 2 * bonus_mine;
                    break;
                case CardDB.cardNameCN.随船外科医师:
                    retval += 2 * bonus_mine;
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
            int offset_mine = p.calEnemyTotalAngr() + Hrtprozis.Instance.enemyDirectDmg;

            int retval = 0;
            // 血线安全
            if (p.ownHero.Hp + p.ownHero.armor - offset_mine > hpboarder)
            {
                retval += (5 + p.ownHero.Hp + p.ownHero.armor - offset_mine - hpboarder) * 3 / 2;
            }
            // 快死了
            else if (p.ownHero.Hp + p.ownHero.armor - offset_mine > 0)
            {
                //if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                retval -= 4 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor + offset_mine) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor + offset_mine);
            }
            else
            {
                retval -= 3 * (hpboarder + 1) * (hpboarder + 1) + 100;
            }
            if (p.ownHero.Hp + p.ownHero.armor - offset_mine < 6 && p.ownHero.Hp + p.ownHero.armor - offset_mine > 0)
            {
                retval -= 80 / (p.ownHero.Hp + p.ownHero.armor - offset_mine);
            }
            // 对手血线安全
            if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy >= aggroboarder)
            {
                retval += 3 * (aggroboarder - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 开始打脸
            else
            {
                retval += 4 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 进入斩杀线
            // if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy <= 5 && p.enemyHero.Hp + p.enemyHero.armor + offset_enemy > 0)
            // {
            //     retval += 300 / (p.enemyHero.Hp + p.enemyHero.armor - offset_enemy);
            // }
            // 场攻+直伤大于对方生命，预计完成斩杀
            if (p.anzEnemyTaunt == 0 && p.calTotalAngr() + p.calDirectDmg(p.mana, false) >= p.enemyHero.Hp + p.enemyHero.armor)
            {
                retval += 2000;
            }
            // 下回合斩杀本回合打脸
            if (p.calDirectDmg(p.ownMaxMana + 1, false, true) >= p.enemyHero.Hp + p.enemyHero.armor)
            {
                retval += 100;
            }
            return retval;
        }


        /// <summary>
        /// 攻击触发的奥秘惩罚
        /// </summary>
        /// <param name="si"></param>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public override int getSecretPen_CharIsAttacked(Playfield p, SecretItem si, Minion attacker, Minion defender)
        {
            if (attacker.isHero) return 0;
            int pen = 0;
            // 攻击的基本惩罚
            if (si.canBe_explosive && !defender.isHero)
            {
                pen -= 20;
                //pen += (attacker.Hp + attacker.Angr);
                foreach (SecretItem sii in p.enemySecretList)
                {
                    sii.canBe_explosive = false;
                }
            }
            return pen;
        }

    }
}