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
    // 调试与工具：调试打印、随机数、工具方法
    public partial class Playfield
    {
        /// <summary>
        /// 调试输出当前我方和敌方随从的状态信息，包括随从的名称、攻击力、当前生命值和最大生命值。
        /// </summary>
        public void debugMinions()
        {
            // 输出我方随从信息
            Helpfunctions.Instance.logg("我方随从################");
            foreach (Minion m in this.ownMinions)
            {
                // 打印随从的中文名称、攻击力、当前生命值和最大生命值
                Helpfunctions.Instance.logg(m.handcard.card.nameCN + " (" + m.Angr + "," + m.Hp + "/" + m.maxHp + ")");
            }

            // 输出敌方随从信息
            Helpfunctions.Instance.logg("敌方随从############");
            foreach (Minion m in this.enemyMinions)
            {
                // 打印随从的中文名称、攻击力、当前生命值和最大生命值
                Helpfunctions.Instance.logg(m.handcard.card.nameCN + " (" + m.Angr + "," + m.Hp + "/" + m.maxHp + ")");
            }
        }

        /// <summary>
        /// 打印当前场面上的状态信息
        /// </summary>
        public void printBoard()
        {
            StringBuilder head = new StringBuilder("", 100);
            head.AppendFormat("board/hash/turn: {0} / {1} / {2}  ++++++++++++++++++++++\n", value, this.hashcode, this.turnCounter);
            head.AppendFormat("惩罚 {0}\n", this.evaluatePenality);
            head.AppendFormat("法力水晶 {0}/{1}\n", this.mana, this.ownMaxMana);
            head.AppendFormat("已使用手牌数: {0} 剩余手牌: {1} 敌方手牌: {2}\n", this.cardsPlayedThisTurn, this.owncards.Count, this.enemyAnzCards);
            head.AppendFormat("我方英雄: \n");
            head.AppendFormat("血量: {0} + {1}\n", this.ownHero.Hp, this.ownHero.armor);
            head.AppendFormat("攻击力: {0}\n", this.ownHero.Angr);
            head.AppendFormat("武器: {0} {1} {2} {3} {4} {5}\n", this.ownWeapon.Angr, this.ownWeapon.Durability, this.ownWeapon.card.nameCN.ToString(), this.ownWeapon.card.cardIDenum, (this.ownWeapon.poisonous ? "剧毒" : "无剧毒buff"), (this.ownWeapon.lifesteal ? "吸血" : "无吸血buff"));
            head.AppendFormat("冻结状态: {0} \n", this.ownHero.frozen);
            head.AppendFormat("敌方英雄血量: {0} + {1}{2} \n", this.enemyHero.Hp, this.enemyHero.armor, ((this.enemyHero.immune) ? " immune" : ""));
            Helpfunctions.Instance.logg(head.ToString());

            // Helpfunctions.Instance.logg("board/hash/turn: " + value + "  /  " + this.hashcode + "  /  " + this.turnCounter + " ++++++++++++++++++++++");
            // Helpfunctions.Instance.logg("惩罚 " + this.evaluatePenality);
            // Helpfunctions.Instance.logg("法力水晶 " + this.mana + "/" + this.ownMaxMana);
            // Helpfunctions.Instance.logg("已使用手牌数: " + this.cardsPlayedThisTurn + " 剩余手牌: " + this.owncards.Count + " 敌方手牌: " + this.enemyAnzCards);
            // Helpfunctions.Instance.logg("我方英雄: ");
            // Helpfunctions.Instance.logg("血量: " + this.ownHero.Hp + " + " + this.ownHero.armor);
            // Helpfunctions.Instance.logg("攻击力: " + this.ownHero.Angr);
            // Helpfunctions.Instance.logg("武器: " + this.ownWeapon.Angr + " " + this.ownWeapon.Durability + " " + this.ownWeapon.card.nameCN.ToString() + " " + this.ownWeapon.card.cardIDenum + " " + (this.ownWeapon.poisonous ? "剧毒" : "无剧毒buff") + " " + (this.ownWeapon.lifesteal ? "吸血" : "无吸血buff"));
            // Helpfunctions.Instance.logg("冻结状态: " + this.ownHero.frozen + " ");
            // Helpfunctions.Instance.logg("敌方英雄血量: " + this.enemyHero.Hp + " + " + this.enemyHero.armor + ((this.enemyHero.immune) ? " immune" : ""));

            if (this.enemySecretCount >= 1) Helpfunctions.Instance.logg("enemySecrets: " + Probabilitymaker.Instance.getEnemySecretData(this.enemySecretList));
            /*             foreach (Action a in this.playactions)
                        {
                            // a.print();
                        } */
            StringBuilder ownMinionsStr = new StringBuilder("我方随从################\n", 100);

            // Helpfunctions.Instance.logg("我方随从################");

            foreach (Minion m in this.ownMinions)
            {
                ownMinionsStr.AppendFormat("{0}({1},{2}/{3}){4}号位 ID：{5}\n", m.handcard.card.nameCN.ToString(), m.Angr, m.Hp, m.maxHp, m.zonepos, m.entityID);
                // Helpfunctions.Instance.logg(m.handcard.card.nameCN.ToString() + "(" + m.Angr + "," + m.Hp + "/" + m.maxHp + ")" + m.zonepos + "号位 ID：" + m.entityID);
            }
            Helpfunctions.Instance.logg(ownMinionsStr.ToString());


            if (this.enemyMinions.Count > 0)
            {
                StringBuilder enemyMinionsStr = new StringBuilder("敌方随从################\n", 100);

                // Helpfunctions.Instance.logg("敌方随从################");

                foreach (Minion m in this.enemyMinions)
                {
                    enemyMinionsStr.AppendFormat("{0}({1},{2}/{3}){4}号位 ID：{5}\n", m.handcard.card.nameCN.ToString(), m.Angr, m.Hp, m.maxHp, m.zonepos, m.entityID);
                    // Helpfunctions.Instance.logg(m.handcard.card.nameCN.ToString() + "(" + m.Angr + "," + m.Hp + "/" + m.maxHp + ")" + m.zonepos + "号位 ID：" + m.entityID);
                }
                Helpfunctions.Instance.logg(enemyMinionsStr.ToString());
            }

            if (this.diedMinions.Count > 0)
            {
                StringBuilder diedMinionsStr = new StringBuilder("死亡随从############\n", 100);

                // Helpfunctions.Instance.logg("死亡随从############");
                foreach (GraveYardItem m in this.diedMinions)
                {
                    diedMinionsStr.AppendFormat("拥有者, entity, cardid: {0}, {1}, {2}\n", m.own, m.entityId, m.cardid);

                    // Helpfunctions.Instance.logg("拥有者, entity, cardid: " + m.own + ", " + m.entityId + ", " + m.cardid);
                }
                Helpfunctions.Instance.logg(diedMinionsStr.ToString());

            }

            StringBuilder owncardsStr = new StringBuilder("我方手牌: \n", 100);

            // Helpfunctions.Instance.logg("我方手牌: ");
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                owncardsStr.AppendFormat("pos {0} {1} (费用：{2}；{3}/{4}) elemPoweredUp {5} {6} {7} {8}\n", hc.position, hc.card.nameCN.ToString(), hc.manacost, (hc.addattack + hc.card.Attack), (hc.addHp + hc.card.Health), hc.poweredUp, hc.card.cardIDenum, hc.MODULAR_ENTITY_PART_1, hc.MODULAR_ENTITY_PART_2);
                // Helpfunctions.Instance.logg("pos " + hc.position + " " + hc.card.nameCN.ToString() + "(费用：" + hc.manacost + "；" + hc.addattack + hc.card.Attack + "/" + +hc.addHp + hc.card.Health + ") elemPoweredUp" + hc.poweredUp + " " + hc.card.cardIDenum + " " + (hc.MODULAR_ENTITY_PART_1 != 0 && hc.MODULAR_ENTITY_PART_2 != 0 ? ("MODULAR_ENTITY_PART_1: " + hc.MODULAR_ENTITY_PART_1 + " " + "MODULAR_ENTITY_PART_2: " + hc.MODULAR_ENTITY_PART_2) : ""));
            }
            Helpfunctions.Instance.logg(owncardsStr.ToString());

            Helpfunctions.Instance.logg("+++++++ printBoard end +++++++++");

            Helpfunctions.Instance.logg("");
        }

        /// <summary>
        /// 打印当前场面上的状态信息
        /// </summary>
        /// <returns></returns>
        public string printBoardString()
        {
            string retval = "Turn : board\t" + this.turnCounter + ":" + ((this.value < -2000000) ? "." : this.value.ToString());
            retval += "\r\n" + "pIdHistory\t";
            foreach (int pId in this.pIdHistory) retval += pId + " ";
            retval += "\r\n" + ("pen\t" + this.evaluatePenality);
            retval += "\r\n" + ("mana\t" + this.mana + "/" + this.ownMaxMana);
            retval += "\r\n" + ("cardsplayed : handsize : enemyhand\t" + this.cardsPlayedThisTurn + ":" + this.owncards.Count + ":" + this.enemyAnzCards);
            retval += "\r\n" + ("Hp : armor : Atk ownhero\t" + this.ownHero.Hp + ":" + this.ownHero.armor + ":" + this.ownHero.Angr + ((this.ownHero.immune) ? ":immune" : ""));
            retval += "\r\n" + ("Atk : Dur : Name : IDe : poison ownWeapon\t" + this.ownWeapon.Angr + " " + this.ownWeapon.Durability + " " + this.ownWeapon.name + " " + this.ownWeapon.card.cardIDenum + " " + (this.ownWeapon.poisonous ? 1 : 0) + " " + (this.ownWeapon.lifesteal ? 1 : 0));
            retval += "\r\n" + ("ownHero.frozen\t" + this.ownHero.frozen);
            retval += "\r\n" + ("Hp : armor enemyhero\t" + this.enemyHero.Hp + ":" + this.enemyHero.armor + ((this.enemyHero.immune) ? ":immune" : ""));
            retval += "\r\n" + ("Atk : Dur : Name : IDe : poison enemyWeapon\t" + this.enemyWeapon.Angr + " " + this.enemyWeapon.Durability + " " + this.enemyWeapon.name + " " + this.enemyWeapon.card.cardIDenum + " " + (this.enemyWeapon.poisonous ? 1 : 0) + " " + (this.enemyWeapon.lifesteal ? 1 : 0));
            retval += "\r\n" + ("carddraw own:enemy\t" + this.owncarddraw + ":" + this.enemycarddraw);

            if (this.enemySecretCount > 0) retval += "\r\n" + ("enemySecrets\t" + Probabilitymaker.Instance.getEnemySecretData(this.enemySecretList));
            if (this.ownSecretsIDList.Count > 0)
            {
                retval += "\r\n" + ("ownSecrets\t");
                foreach (CardDB.cardIDEnum s in this.ownSecretsIDList)
                {
                    retval += " " + CardDB.Instance.getCardDataFromID(s).nameEN;
                }
            }

            for (int i = 0; i < this.playactions.Count; i++) retval += "\r\n" + i + " action\t" + this.playactions[i].printString();
            retval += "\r\n" + ("OWN MINIONS################\t" + this.ownMinions.Count);

            for (int i = 0; i < this.ownMinions.Count; i++)
            {
                Minion m = this.ownMinions[i];
                retval += "\r\n" + (i + 1) + " OWN MINION\t" + m.zonepos + " " + m.entityID + ":" + m.name + " " + m.Angr + " " + m.Hp;
            }

            if (this.enemyMinions.Count > 0)
            {
                retval += "\r\n" + ("ENEMY MINIONS############\t" + this.enemyMinions.Count);
                for (int i = 0; i < this.enemyMinions.Count; i++)
                {
                    Minion m = this.enemyMinions[i];
                    retval += "\r\n" + (i + 1) + " ENEMY MINION\t" + m.zonepos + " " + m.entityID + ":" + m.name + " " + m.Angr + " " + m.Hp;
                }
            }

            if (this.diedMinions.Count > 0)
            {
                retval += "\r\n" + ("DIED MINIONS############\t");
                for (int i = 0; i < this.diedMinions.Count; i++)
                {
                    GraveYardItem m = this.diedMinions[i];
                    retval += "\r\n" + i + (" own : entity : cardid\t" + (m.own ? "own" : "en") + " " + m.entityId + " " + m.cardid);
                }
            }

            retval += "\r\nOwn Handcards\t\r\n";
            for (int i = 0; i < this.owncards.Count; i++)
            {
                Handmanager.Handcard hc = this.owncards[i];
                retval += "\r\n" + (i + 1) + " CARD\t" + (hc.position + " " + hc.entity + ":" + hc.card.nameEN + " " + hc.manacost + " " + hc.card.cardIDenum + " " + hc.addattack + " " + hc.addHp + " " + hc.poweredUp + "\r\n");
            }
            retval += ("Enemy Handcards count\t" + this.enemyAnzCards + "\r\n");

            return retval;
        }

        /// <summary>
        /// 打印当前场面上的状态信息
        /// </summary>
        public void printBoardDebug()
        {
            StringBuilder hero = new StringBuilder(20);
            StringBuilder ehero = new StringBuilder(20);
            hero.AppendFormat("hero {0} {1} {2}", this.ownHero.Hp, this.ownHero.armor, this.ownHero.entityID);
            ehero.AppendFormat("ehero {0} {1} {2}", this.enemyHero.Hp, this.enemyHero.armor, this.enemyHero.entityID);
            Helpfunctions.Instance.logg(hero.ToString());
            Helpfunctions.Instance.logg(ehero.ToString());
            foreach (Minion m in ownMinions)
            {
                Helpfunctions.Instance.logg(m.name + "m.entityID");
            }
            Helpfunctions.Instance.logg("-");
            foreach (Minion m in enemyMinions)
            {
                Helpfunctions.Instance.logg(m.name + " m.entityID");
            }
            Helpfunctions.Instance.logg("-");
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                Helpfunctions.Instance.logg(hc.position + " " + hc.card.nameEN + " " + hc.entity);
            }
        }

        /// <summary>
        /// 获取下一个动作
        /// </summary>
        /// <returns>返回下一个动作，如果没有动作则返回 null</returns>
        public Action getNextAction()
        {
            // 返回第一个动作或 null
            return this.playactions.Count > 0 ? this.playactions[0] : null;
        }

        /// <summary>
        /// 打印当前的动作列表
        /// </summary>
        /// <param name="toBuffer">是否将信息输出到缓冲区（当前实现未使用此参数）</param>
        public void printActions(bool toBuffer = false)
        {
            int index = 0;
            foreach (Action a in this.playactions)
            {
                index++;
                string tmp = index + ":  ";
                tmp += a.printString();
                Helpfunctions.Instance.logg(tmp);
            }
        }

        /// <summary>
        /// 打印行动信息，适用于调试或记录
        /// </summary>
        /// <param name="action">要打印的行动对象</param>
        public void printActionforDummies(Action a)
        {
            if (a.actionType == actionEnum.playcard)
            {
                Helpfunctions.Instance.ErrorLog("printActionforDummies - play " + a.hc.card.nameEN);
                if (a.druidchoice >= 1)
                {
                    string choose = (a.druidchoice == 1) ? "left card" : "right card";
                    Helpfunctions.Instance.ErrorLog("choose the " + choose);
                }
                if (a.place >= 1)
                {
                    Helpfunctions.Instance.ErrorLog("on position " + a.place);
                }
                if (a.target != null)
                {
                    if (!a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("and target to the enemy " + ename);
                    }

                    if (a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("and target to your own" + ename);
                    }

                    if (a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("and target your own hero");
                    }

                    if (!a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("and target to the enemy hero");
                    }
                }

            }
            if (a.actionType == actionEnum.attackWithMinion)
            {
                string name = "" + a.own.name;
                if (a.target.isHero)
                {
                    Helpfunctions.Instance.ErrorLog("printActionforDummies - attack with: " + name + " the enemy hero");
                }
                else
                {
                    string ename = "" + a.target.name;
                    Helpfunctions.Instance.ErrorLog("printActionforDummies - attack with: " + name + " the enemy: " + ename);
                }

            }

            if (a.actionType == actionEnum.attackWithHero)
            {
                if (a.target.isHero)
                {
                    Helpfunctions.Instance.ErrorLog("printActionforDummies - attack with your hero the enemy hero!");
                }
                else
                {
                    string ename = "" + a.target.name;
                    Helpfunctions.Instance.ErrorLog("printActionforDummies - attack with the hero, and choose the enemy: " + ename);
                }
            }
            if (a.actionType == actionEnum.useHeroPower)
            {
                Helpfunctions.Instance.ErrorLog("printActionforDummies - use your Heropower ");
                if (a.target != null)
                {
                    if (!a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("on enemy: " + ename);
                    }

                    if (a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("on your own: " + ename);
                    }

                    if (a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("on your own hero");
                    }

                    if (!a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("on your the enemy hero");
                    }

                }
            }
            if (a.actionType == actionEnum.useLocation)
            {
                Helpfunctions.Instance.ErrorLog("printActionforDummies - use your Location ");
                if (a.target != null)
                {
                    if (!a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("on enemy: " + ename);
                    }

                    if (a.target.own && !a.target.isHero)
                    {
                        string ename = "" + a.target.name;
                        Helpfunctions.Instance.ErrorLog("on your own: " + ename);
                    }

                    if (a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("on your own hero");
                    }

                    if (!a.target.own && a.target.isHero)
                    {
                        Helpfunctions.Instance.ErrorLog("on your the enemy hero");
                    }
                }
            }
            if (a.actionType == actionEnum.useTitanAbility)
            {
                StringBuilder retval = new StringBuilder();
                retval.Append("使用泰坦技能 ");
                string suffix = "";
                switch (a.own.handcard.card.cardIDenum.ToString())
                {
                    case "TTN_092":
                        if (a.titanAbilityNO == 1) suffix = "t1";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_858":
                        if (a.titanAbilityNO == 1) suffix = "t1";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_862":
                        if (a.titanAbilityNO == 1) suffix = "t1";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;

                    case "TTN_075":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_800":
                        if (a.titanAbilityNO == 1) suffix = "t2";
                        else if (a.titanAbilityNO == 2) suffix = "t3";
                        else if (a.titanAbilityNO == 3) suffix = "t";
                        break;
                    case "TTN_415":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_429":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "YOG_516":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_903":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                    case "TTN_721":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t1";
                        else if (a.titanAbilityNO == 3) suffix = "t2";
                        break;

                    case "TTN_737":
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t3";
                        else if (a.titanAbilityNO == 3) suffix = "t1";
                        break;

                    case "TTN_960":
                        if (a.titanAbilityNO == 1) suffix = "t2";
                        else if (a.titanAbilityNO == 2) suffix = "t3";
                        else if (a.titanAbilityNO == 3) suffix = "t4";
                        break;

                    default:
                        if (a.titanAbilityNO == 1) suffix = "t";
                        else if (a.titanAbilityNO == 2) suffix = "t2";
                        else if (a.titanAbilityNO == 3) suffix = "t3";
                        break;
                }
                CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(a.own.handcard.card.cardIDenum.ToString() + suffix));
                retval.Append(" 目标 " + (a.target != null && a.target.handcard != null ? a.target.handcard.card.nameCN.ToString() : "无"));
                Helpfunctions.Instance.ErrorLog("printActionforDummies - " + retval.ToString());
            }
            Helpfunctions.Instance.ErrorLog("");
        }

        private Random rng = new Random(); // 定义一个随机数生成器实例
        /// <summary>
        /// 生成指定范围内的随机数
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int getRandomNumber(int minValue, int maxValue)
        {
            return rng.Next(minValue, maxValue + 1);
        }

        /// <summary>
        /// 移除手牌中所有快枪状态
        /// </summary>
        /// <param name="handcards"></param>
        private void RemoveQuickDrawStatus(List<Handmanager.Handcard> handcards)
        {
            foreach (var card in handcards)
            {
                card.card.Quickdraw = false; // 假设 quickDraw 是一个标志，用于表示快枪状态
            }
        }

        /// <summary>
        /// 计算并返回释放了几个派系的法术
        /// </summary>
        /// <returns></returns>
        public int CountSpellSchoolsPlayed()
        {
            int count = 0;

            foreach (int value in this.ownSpellSchoolCounts.Values)
            {
                if (value > 0)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 检查牌库中是否有随从牌
        /// </summary>
        /// <returns></returns>
        public bool hasMinionsInDeck()
        {
            foreach (var cardEntry in prozis.turnDeck)
            {
                CardDB.Card card = CardDB.Instance.getCardDataFromID(cardEntry.Key);
                if (card.type == CardDB.cardtype.MOB)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 让每个敌方随从随机攻击一个敌方随从
        /// </summary>
        public void RandomEnemyMinionsAttackEachOther()
        {
            // 获取敌方随从列表的副本，避免在遍历时修改原始列表
            List<Minion> enemyMinions = new List<Minion>(this.enemyMinions);

            // 遍历每个敌方随从
            foreach (Minion attacker in enemyMinions)
            {
                // 检查当前随从是否还能攻击
                if (attacker.Ready && !attacker.frozen)
                {
                    // 从敌方随从列表中移除当前随从以避免其攻击自己
                    List<Minion> possibleTargets = new List<Minion>(enemyMinions);
                    possibleTargets.Remove(attacker);

                    // 如果没有其他可攻击的目标，跳过此随从
                    if (possibleTargets.Count == 0) continue;

                    // 随机选择一个敌方随从作为目标
                    Minion target = possibleTargets[this.getRandomNumber(0, possibleTargets.Count - 1)];

                    // 检查目标随从是否仍然存在
                    if (this.enemyMinions.Contains(target))
                    {
                        // 执行攻击
                        this.minionAttacksMinion(attacker, target);
                    }
                }
            }
        }

        public int getPosition(bool ownplay)
        {
            return ownplay ? this.ownMinions.Count : this.enemyMinions.Count;
        }
        /// <summary>
        /// 任意种族的卡牌在手牌
        /// </summary>
        /// <param name="race">种族类型</param>
        /// <returns></returns>
        public bool anyRaceCardInHand(CardDB.Race race)
        {
            bool raceCardInHand = false;
            foreach (Handmanager.Handcard hc in this.owncards)
            {
                if (RaceUtils.MinionBelongsToRace(hc.card.GetRaces(), race))
                {
                    raceCardInHand = true;
                    break;
                }
            }
            return raceCardInHand;
        }
    }
}
