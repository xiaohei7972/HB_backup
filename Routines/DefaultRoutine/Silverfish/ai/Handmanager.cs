namespace HREngine.Bots
{
    using System.Collections.Generic;
    using System.Text;
    public class Handmanager  // 手牌管理
    {

        public class Handcard
        {
            public int position = 0; //手牌的位置
            public int entity = -1; //炉石传说内部entity编号
            public int manacost = 1000; //花费费用，但获取卡牌费用要用getManaCost(Playfield p)函数
            public int addattack = 0; //增加的攻击力，如风驰电掣的SIM中就使其 +1
            public int addHp = 0; //增加的血量
            public CardDB.Card card; //卡牌，指向CardDB.cs
            public Minion target; //目标
            public int poweredUp = 0; //手牌高亮
            public int darkmoon_num = 0; //暗月先知抽牌数：战斗中已触发的奥秘数
            public int extraParam2 = 0; //扩展参数2，可以用来记录一些此卡需要的特殊数据
            public bool extraParam3 = false; //扩展参数3
            public int SCRIPT_DATA_NUM_1 = 0;//标签脚本数据编号1，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
            public int SCRIPT_DATA_NUM_2 = 0;//标签脚本数据编号2，用于记录伤害、召唤数量、衍生物攻击力、衍生物血量、注能数量、法力渴求
            public int MODULAR_ENTITY_PART_1 = 0;//自定义模块1
            public int MODULAR_ENTITY_PART_2 = 0;//自定义模块2
            public int TAG_ONE_TURN_EFFECT = 0;
            public int LUNAHIGHLIGHTHINT = 0;
            public int scheme = 1;
            public List<CardDB.cardIDEnum> enchs = new List<CardDB.cardIDEnum>();
            public bool discovered = false;

            public bool temporary = false;//临时卡牌
            public bool valeeraShadow = false;//瓦莉拉的阴影，这才是临时牌用的tag
            //条件计数，例如施放过几张法术，英雄技能造成多少伤害等
            public int conditionalCount = 0;
            //条件卡牌，例如施放的法术牌
            public List<CardDB.cardIDEnum> conditionalList = new List<CardDB.cardIDEnum>();

            public void updateDIYCard(int hc_part_1, int hc_part_2)
            {
                if (hc_part_1 != 0 && hc_part_2 != 0)
                {
                    CardDB.Card part1 = CardDB.Instance.getCardDataFromDbfID(hc_part_1.ToString());
                    CardDB.Card part2 = CardDB.Instance.getCardDataFromDbfID(hc_part_2.ToString());
                    getDIYCard(part1, part2);
                }
            }
            public void getDIYCard(CardDB.Card part1, CardDB.Card part2)
            {
                this.card.cost = part1.cost + part2.cost;
                this.card.Attack = part1.Attack + part2.Attack;
                this.card.Health = part1.Health + part2.Health;
                this.card.textCN = part1.textCN + part2.textCN;
                this.card.tank = (part1.tank || part2.tank);                                    //嘲讽
                this.card.Shield = (part1.Shield || part2.Shield);                              //圣盾
                this.card.Charge = (part1.Charge || part2.Charge);                              //冲锋
                this.card.Rush = (part1.Rush || part2.Rush);                                    //突袭
                this.card.Stealth = (part1.Stealth || part2.Stealth);                           //潜行
                this.card.Elusive = (part1.Elusive || part2.Elusive);                           //扰魔
                this.card.windfury = (part1.windfury || part2.windfury);                        //风怒
                this.card.poisonous = (part1.poisonous || part2.poisonous);                     //剧毒
                this.card.lifesteal = (part1.lifesteal || part2.lifesteal);                     //吸血
                this.card.reborn = (part1.reborn || part2.reborn);                              //复生
            }
            public string Status
            {
                get
                {
                    return "{" + position + "} " + card.nameCN + " [费用: " + manacost + "] (" + (addattack + card.Attack) + "/" + (addHp + card.Health) + ")";
                }
            }

            public string OnlineCardImage
            {
                get
                {
                    return card.OnlineCardImage;
                }
            }

            public string OnlineCardTile
            {
                get
                {
                    return card.OnlineCardTile;
                }
            }

            public Handcard()
            {
                card = CardDB.Instance.unknownCard;
            }
            public Handcard(Handcard hc)
            {
                this.position = hc.position;
                this.entity = hc.entity;
                this.manacost = hc.manacost;
                this.card = hc.card;
                this.addattack = hc.addattack;
                this.addHp = hc.addHp;
                this.poweredUp = hc.poweredUp;
                this.SCRIPT_DATA_NUM_1 = hc.SCRIPT_DATA_NUM_1;
                this.SCRIPT_DATA_NUM_2 = hc.SCRIPT_DATA_NUM_2;
                this.MODULAR_ENTITY_PART_1 = hc.MODULAR_ENTITY_PART_1;
                this.MODULAR_ENTITY_PART_2 = hc.MODULAR_ENTITY_PART_2;
                this.discovered = hc.discovered;
                this.TAG_ONE_TURN_EFFECT = hc.TAG_ONE_TURN_EFFECT;
                this.LUNAHIGHLIGHTHINT = hc.LUNAHIGHLIGHTHINT;
                this.scheme = hc.scheme;
                this.enchs = hc.enchs;
                //临时卡牌
                this.temporary = hc.temporary;
                //条件计数
                this.conditionalCount = hc.conditionalCount;
                //条件卡牌
                this.conditionalList = hc.conditionalList;


            }
            public Handcard(CardDB.Card c)
            {
                this.position = 0;
                this.entity = -1;
                this.card = c;
                this.addattack = 0;
                this.addHp = 0;
                //临时卡牌
                this.temporary = c.Temporary;


            }
            public void setHCtoHC(Handcard hc)
            {
                this.manacost = hc.manacost;
                this.addattack = hc.addattack;
                this.addHp = hc.addHp;
                this.card = hc.card;
                this.poweredUp = hc.poweredUp;
                this.SCRIPT_DATA_NUM_1 = hc.SCRIPT_DATA_NUM_1;
                this.SCRIPT_DATA_NUM_2 = hc.SCRIPT_DATA_NUM_2;
                this.discovered = hc.discovered;
                this.TAG_ONE_TURN_EFFECT = hc.TAG_ONE_TURN_EFFECT;
                this.LUNAHIGHLIGHTHINT = hc.LUNAHIGHLIGHTHINT;
                this.enchs = hc.enchs;
                //临时卡牌
                this.temporary = hc.temporary;
                //条件计数
                this.conditionalCount = hc.conditionalCount;
                //条件卡牌
                this.conditionalList = hc.conditionalList;
            }

            //读取卡牌法力值
            public int getManaCost(Playfield p)
            {
                if (this.enchs.Count > 0)
                {
                    foreach (CardDB.cardIDEnum ench in this.enchs)
                    {
                        switch (ench)
                        {
                            case CardDB.cardIDEnum.SW_052t4e:// TODO 游戏内无法使用 声光干扰器
                            case CardDB.cardIDEnum.EDR_526e: // TODO 雷弗拉尔，恶念巨蛛
                            case CardDB.cardIDEnum.TTN_744e1:// TODO 严寒冰封
                            case CardDB.cardIDEnum.EDR_234e2:// TODO 翡翠厚赠
                                return 1000;
                            case CardDB.cardIDEnum.MAW_014e2:// TODO 公诉人梅尔特拉尼克斯
                                if (this.position != 1 && this.position != p.owncards.Count)
                                    return 1000;
                                break;
                            default:
                                continue;
                        }
                    }
                }

                return this.card.getManaCost(p, this.manacost);
            }

            //判定卡牌是否能够使用
            public bool canplayCard(Playfield p, bool own)
            {
                if (this.enchs.Count > 0)
                {
                    foreach (CardDB.cardIDEnum ench in this.enchs)
                    {
                        switch (ench)
                        {
                            case CardDB.cardIDEnum.SW_052t4e:// TODO 游戏内无法使用 声光干扰器
                            case CardDB.cardIDEnum.EDR_526e:// TODO 雷弗拉尔，恶念巨蛛
                            case CardDB.cardIDEnum.TTN_744e1:// TODO 严寒冰封
                            case CardDB.cardIDEnum.EDR_234e2:// TODO 翡翠厚赠
                                return false;
                            case CardDB.cardIDEnum.MAW_014e2:// TODO 公诉人梅尔特拉尼克斯
                                if (this.position != 1 && this.position != p.owncards.Count)
                                    return false;
                                break;
                            default:
                                continue;
                        }
                    }

                }
                return this.card.canplayCard(p, this.manacost, own);

            }
        }

        public List<Handcard> handCards = new List<Handcard>();

        public int anzcards = 0;

        public int enemyAnzCards = 0;

        private int ownPlayerController = 0;

        Helpfunctions help;
        CardDB cdb = CardDB.Instance;

        private static Handmanager instance;

        public static Handmanager Instance
        {
            get
            {
                return instance ?? (instance = new Handmanager());
            }
        }


        private Handmanager()
        {
            this.help = Helpfunctions.Instance;

        }

        public void clearAllRecalc()
        {
            this.handCards.Clear();
            this.anzcards = 0;
            this.enemyAnzCards = 0;
            this.ownPlayerController = 0;
        }

        public void setOwnPlayer(int player)
        {
            this.ownPlayerController = player;
        }




        public void setHandcards(List<Handcard> hc, int anzown, int anzenemy)
        {
            this.handCards.Clear();
            foreach (Handcard h in hc)
            {
                this.handCards.Add(new Handcard(h));
            }
            //this.handCards.AddRange(hc);
            this.handCards.Sort((a, b) => a.position.CompareTo(b.position));
            this.anzcards = anzown;
            this.enemyAnzCards = anzenemy;
        }

        public void printcards()
        {
            help.logg("Own Handcards: ");
            foreach (Handmanager.Handcard hc in this.handCards)
            {
                StringBuilder ownHandcard = new StringBuilder(60);
                ownHandcard.AppendFormat("pos {0} {1}({2}/{3}) {4} entity {5} {6}", hc.position, hc.card.nameCN, hc.card.Attack, hc.card.Health, hc.manacost, hc.entity, hc.card.cardIDenum);
                ownHandcard.AppendFormat(" {0} {1} {2}", hc.addattack, hc.addHp, hc.poweredUp);
                ownHandcard.AppendFormat(" {0} {1}", hc.MODULAR_ENTITY_PART_1, hc.MODULAR_ENTITY_PART_2);
                // string s = "pos " + hc.position + " " + hc.card.nameCN + "(" + hc.card.Attack + "/" + hc.card.Health + ")" + " " + hc.manacost + " entity " + hc.entity + " " + hc.card.cardIDenum + " " + hc.addattack + " " + hc.addHp + " " + hc.poweredUp;
                if (hc.enchs.Count > 0)
                {
                    foreach (CardDB.cardIDEnum cardIDEnum in hc.enchs)
                    {
                        ownHandcard.AppendFormat(" {0}", cardIDEnum.ToString());
                    }
                }
                help.logg(ownHandcard.ToString());
            }
            help.logg("Enemy cards: " + this.enemyAnzCards);
        }



    }

}