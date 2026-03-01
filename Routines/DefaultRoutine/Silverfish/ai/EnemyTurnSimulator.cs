namespace HREngine.Bots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 敌方回合模拟器类，用于模拟敌方回合的可能行动
    /// 帮助AI评估不同决策的后果，选择最优策略
    /// </summary>
    public class EnemyTurnSimulator
    {

        /// <summary>
        /// 线程ID
        /// </summary>
        public int thread = 0;

        /// <summary>
        /// 可能的移动列表，用于存储模拟的游戏状态
        /// </summary>
        private List<Playfield> posmoves = new List<Playfield>(7000);
        //public int maxwide = 20;
        /// <summary>
        /// 移动生成器实例
        /// </summary>
        Movegenerator movegen = Movegenerator.Instance;
        /// <summary>
        /// 最大搜索宽度
        /// </summary>
        public int maxwide = 20;
        /// <summary>
        /// 两回合模拟数量
        /// </summary>
        private int twotsamount = 0;
        /// <summary>
        /// 如果能在下一回合结束游戏则狂暴
        /// </summary>
        private int berserkIfCanFinishNextTour = 0;


        /// <summary>
        /// 设置最大搜索宽度
        /// </summary>
        /// <param name="firstStep">是否为第一步</param>
        public void setMaxwide(bool firstStep)
        {
            twotsamount = Settings.Instance.secondTurnAmount;
            if (firstStep) maxwide = Settings.Instance.enemyTurnMaxWide;
            else maxwide = Settings.Instance.enemyTurnMaxWideSecondStep;
            berserkIfCanFinishNextTour = Settings.Instance.berserkIfCanFinishNextTour;
        }

        /// <summary>
        /// 模拟敌方回合的可能行动
        /// </summary>
        /// <param name="rootfield">根游戏场状态</param>
        /// <param name="simulateTwoTurns">是否模拟两回合</param>
        /// <param name="playaround">是否防奥秘</param>
        /// <param name="print">是否打印输出</param>
        /// <param name="pprob">防奥秘概率1</param>
        /// <param name="pprob2">防奥秘概率2</param>
        public void simulateEnemysTurn(Playfield rootfield, bool simulateTwoTurns, bool playaround, bool print, int pprob, int pprob2)
        {
            if (rootfield.bestEnemyPlay == null)
            {
                bool havedonesomething = true;
                posmoves.Clear();

                posmoves.Add(new Playfield(rootfield));
                posmoves[0].isLethalCheck = false; 
                posmoves[0].startTurn();
                rootfield.guessingHeroHP = posmoves[0].guessingHeroHP;
                List<Playfield> temp = new List<Playfield>();
                int deep = 0;
                int enemMana = Math.Min(rootfield.enemyMaxMana + 1, 10);

                if (playaround && !rootfield.loatheb)
                {
                    float oldval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                    posmoves[0].value = int.MinValue;
                    enemMana = posmoves[0].EnemyCardPlaying(rootfield.enemyHeroStartClass, enemMana, rootfield.enemyAnzCards, pprob, pprob2);
                    if (posmoves[0].wehaveCounterspell > 1)
                    {
                        posmoves[0].ownSecretsIDList.Remove(CardDB.cardIDEnum.EX1_287);
                        //posmoves[0].evaluatePenality -= 7; Todo:这个值只用于sim里触发，使得打分代码集中在behavior以及sim，其他地方不涉及
                    }
                    float newval = Ai.Instance.botBase.getPlayfieldValue(posmoves[0]);
                    posmoves[0].value = int.MinValue;
                    posmoves[0].enemyAnzCards--;
                    posmoves[0].triggerCardsChanged(false);
                    if (oldval < newval)
                    {
                        posmoves.Clear();
                        posmoves.Add(new Playfield(rootfield));
                        posmoves[0].startTurn();
                    }
                }
                
                if (posmoves[0].ownHeroHasDirectLethal())
                {
                    if (posmoves[0].value >= -2000000) rootfield.value -= 2000000;
                    else rootfield.value = -2000000;

                    return;
                }

                doSomeBasicEnemyAi(posmoves[0]);

                //play ability!
                if (rootfield.enemyHeroPowerCostLessOnce <= 0 && posmoves[0].enemyAbilityReady && enemMana >= rootfield.enemyHeroAblility.manacost && posmoves[0].enemyHeroAblility.card.canplayCard(posmoves[0], 0, false) && !rootfield.loatheb)
                {
                    int abilityPenality = 0;
                    List<Minion> trgts = posmoves[0].enemyHeroAblility.card.getTargetsForHeroPower(posmoves[0], false);
                    foreach (Minion trgt in trgts)
                    {
                        Action a = new Action(actionEnum.useHeroPower, posmoves[0].enemyHeroAblility, null, 0, trgt, abilityPenality, 0);
                        Playfield pf = new Playfield(posmoves[0]);
                        pf.doAction(a);
                        posmoves.Add(pf);
                    }
                }

                int boardcount = 0;
                //movegen...

                int i = 0;
                int count = 0;
                Playfield p = null;

                Playfield bestold = null;
                havedonesomething = true;
                float bestoldval = int.MaxValue;
                while (havedonesomething)
                {

                    temp.Clear();
                    temp.AddRange(posmoves);
                    havedonesomething = false;
                    count = temp.Count;
                    for (i = 0; i < count; i++)
                    {
                        p = temp[i];
                        if (p.complete)
                        {
                            continue;
                        }
                        List<Action> actions = movegen.getMoveList(p, false, true, false); 

                        foreach (Action a in actions)
                        {
                            havedonesomething = true;
                            Playfield pf = new Playfield(p);
                            pf.doAction(a);
                            posmoves.Add(pf);
                            boardcount++;
                        }

                        p.endTurn();
                        p.complete = true;
                        p.guessingHeroHP = rootfield.guessingHeroHP;
                        if (Ai.Instance.botBase.getPlayfieldValue(p) < bestoldval) // want the best enemy-play-> worst for us
                        {
                            bestoldval = Ai.Instance.botBase.getPlayfieldValue(p);
                            bestold = p;
                        }
                        posmoves.Remove(p);
                        if (boardcount >= maxwide) break;
                    }


                    cuttingposibilitiesET();

                    deep++;
                    if (boardcount >= maxwide) break;
                }

                posmoves.Add(bestold);
                float bestval = int.MaxValue;
                Playfield bestplay = bestold;
                count = posmoves.Count;
                for (i = 0; i < count; i++)
                {
                    p = posmoves[i];
                    if (!p.complete)
                    {
                        p.endTurn();
                        p.complete = true;
                    }
                    p.guessingHeroHP = rootfield.guessingHeroHP;
                    float val = Ai.Instance.botBase.getPlayfieldValue(p);
                    if (p.enemyMinions.Count > 6) val += 12;
                    if (bestval > val)// we search the worst value
                    {
                        bestplay = p;
                        bestval = val;
                    }
                }
                if (bestplay.enemyMinions.Count < 7)
                {
                    if (bestplay.enemyAnzCards > 0)
                    {
                        if (bestplay.enemyMaxMana > 5) bestplay.callKid(this.spellbreaker43, bestplay.enemyMinions.Count, false, false);
                        else bestplay.callKid(this.flame, bestplay.enemyMinions.Count, false, false);
                        int tmp = bestplay.enemyMinions.Count;
                        bestplay.simulateTrapsEndEnemyTurn();

                        if (tmp == bestplay.enemyMinions.Count)
                        {
                            int bval = 1;
                            if (bestplay.enemyMaxMana > 4) bval = 2;
                            if (bestplay.enemyMaxMana > 7) bval = 3;
                            if (bestplay.enemyMinions.Count >= 1) bestplay.minionGetBuffed(bestplay.enemyMinions[bestplay.enemyMinions.Count - 1], bval - 1, bval);
                        }
                    }
                }
                bestplay.startTurn();      
                bestplay.ownAbilityReady = false;
                bestplay.owncarddraw = rootfield.owncarddraw;
                bestplay.complete = true;
			bestplay.isLethalCheck = rootfield.isLethalCheck;
                Ai.Instance.botBase.getPlayfieldValue(bestplay);
                bestval = bestplay.value;
                rootfield.value = bestplay.value;

                if (twotsamount > 0 || (rootfield.isLethalCheck && berserkIfCanFinishNextTour > 0))
                {
                    rootfield.bestEnemyPlay = new Playfield(bestplay);
                    rootfield.bestEnemyPlay.value = bestval;
                }
            }




            if ((simulateTwoTurns || (rootfield.isLethalCheck && berserkIfCanFinishNextTour > 0)) && rootfield.bestEnemyPlay != null && rootfield.bestEnemyPlay.value > -1000)
            {


                float bestval = rootfield.bestEnemyPlay.value;
                rootfield.bestEnemyPlay.complete = false;
                rootfield.bestEnemyPlay.value = int.MinValue;
                rootfield.value = Settings.Instance.firstweight * bestval + Settings.Instance.secondweight * Ai.Instance.nextTurnSimulator[this.thread].doallmoves(rootfield.bestEnemyPlay, print);




            }
        }
        
        /// <summary>
        /// 裁剪可能的行动，通过哈希值去重，减少搜索空间
        /// </summary>
        public void cuttingposibilitiesET()
        {
            Dictionary<Int64, Playfield> tempDict = new Dictionary<Int64, Playfield>();
            Playfield p = null;
            int max = posmoves.Count;
            for (int i = 0; i < max; i++)
            {
                p = posmoves[i];
                Int64 hash = p.GetPHash();
                if (!tempDict.ContainsKey(hash)) tempDict.Add(hash, p);

            }
            posmoves.Clear();
            foreach (KeyValuePair<Int64, Playfield> d in tempDict)
            {
                posmoves.Add(d.Value);
            }
            tempDict.Clear();
        }

        /// <summary>
        /// 火焰卡牌对象
        /// </summary>
        CardDB.Card flame = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_614t);
        /// <summary>
        /// 法术破坏者卡牌对象
        /// </summary>
        CardDB.Card spellbreaker43 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_048);

        /// <summary>
        /// 执行基本的敌方AI行为
        /// 处理敌方随从的各种特殊能力和触发效果
        /// </summary>
        /// <param name="p">游戏场状态</param>
        private void doSomeBasicEnemyAi(Playfield p)
        {
            if (p.enemyHeroName == HeroEnum.mage)
            {
                if (Probabilitymaker.Instance.enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_561)) p.ownHero.Hp = Math.Max(5, p.ownHero.Hp - 7);
            }

            //play some cards (to not overdraw)
            if (p.enemyAnzCards >= 8)
            {
                p.enemyAnzCards--;
                p.triggerCardsChanged(false);
            }
            if (p.enemyAnzCards >= 4)
            {
                p.enemyAnzCards--;
                p.triggerCardsChanged(false);
            }
            if (p.enemyAnzCards >= 2)
            {
                p.enemyAnzCards--;
                p.triggerCardsChanged(false);
            }
            
            foreach (Minion m in p.enemyMinions.ToArray())
            {
                if (m.silenced) continue;
                if (p.enemyAnzCards >= 2 && (m.name == CardDB.cardNameEN.gadgetzanauctioneer || m.name == CardDB.cardNameEN.starvingbuzzard))
                {
                    if (p.enemyDeckSize >= 1)
                    {
                        p.drawACard(CardDB.cardNameEN.unknown, false);
                    }
                }
                int anz = 0;
                switch (m.name)
                {
                    //****************************************heal
                    case CardDB.cardNameEN.northshirecleric:
                        anz = 0;
                        foreach (Minion mnn in p.enemyMinions)
                        {
                            if (mnn.wounded) anz++;
                        }
                        anz = Math.Min(anz, 3);
                        for (int i = 0; i < anz; i++)
                        {
                            if (p.enemyDeckSize >= 1) p.drawACard(CardDB.cardNameEN.unknown, false);
                        }
                        continue;
                    case CardDB.cardNameEN.shadowboxer:
                        anz = 0;
                        foreach (Minion mnn in p.enemyMinions)
                        {
                            if (mnn.wounded) anz++;
                        }
                        if (anz > 0)
                        {
                            anz = Math.Min(anz, 3);
                            Minion target = p.ownHero;
                            for (; anz > 0; anz--)
                            {
                                if (p.ownMinions.Count > 0) target = p.searchRandomMinion(p.ownMinions, searchmode.searchLowestHP);
                                if (target == null) target = p.ownHero;
                                p.minionGetDamageOrHeal(target, 1);
                            }
                        }
                        continue;
                    case CardDB.cardNameEN.lightwarden:
                        anz = 0;
                        foreach (Minion mnn in p.enemyMinions)
                        {
                            if (mnn.wounded) anz++;
                        }
                        if (p.enemyHero.wounded) anz++;
                        if (anz >= 2) p.minionGetBuffed(m, 2, 0);
                        continue;
                        //****************************************
                    //****************************************spell
                    case CardDB.cardNameEN.manaaddict:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.minionGetTempBuff(m, 2, 0);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.minionGetTempBuff(m, 2, 0);
                        }
                        continue;
                    case CardDB.cardNameEN.manawyrm:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.minionGetBuffed(m, 1, 0);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.minionGetBuffed(m, 1, 0);
                        }
                        continue;
                    case CardDB.cardNameEN.dragonkinsorcerer:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.minionGetTempBuff(m, 1, 1);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.minionGetBuffed(m, 1, 1);
                        }
                        continue;
                    case CardDB.cardNameEN.violetteacher:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_026t), p.enemyMinions.Count, false);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_026t), p.enemyMinions.Count, false);
                        }
                        continue;
                    case CardDB.cardNameEN.warsongcommander:
                        p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_165t1), p.enemyMinions.Count, false, false);
                        continue;
                    case CardDB.cardNameEN.gadgetzanauctioneer:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.drawACard(CardDB.cardNameEN.unknown, false);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.drawACard(CardDB.cardNameEN.unknown, false);
                        }
                        continue;
                    case CardDB.cardNameEN.archmageantonidas:
                        if (p.ownMinions.Count < 1) p.minionGetDamageOrHeal(p.ownHero, 6);
                        else
                        {
                            Minion target = new Minion();
                            foreach (Minion mnn in p.ownMinions)
                            {
                                if (mnn.Hp <= 6 && (mnn.Hp + mnn.Angr) > (target.Hp + target.Angr)) target = mnn;
                            }
                            p.minionGetDamageOrHeal(target, 6);
                        }
                        continue;
                    case CardDB.cardNameEN.gazlowe:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.drawACard(CardDB.cardNameEN.unknown, false);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.drawACard(CardDB.cardNameEN.unknown, false);
                        }
                        continue;
                    case CardDB.cardNameEN.flamewaker:
                        anz = 0;
                        if (p.enemyAnzCards >= 1) anz++;
                        if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) anz++;
                        if (anz > 0)
                        {
                            Minion target = p.ownHero;
                            anz = anz * 2 - 1;
                            p.minionGetDamageOrHeal(target, 1);
                            for (; anz > 0; anz--)
                            {
                                if (p.ownMinions.Count > 0) target = p.searchRandomMinion(p.ownMinions, searchmode.searchLowestHP);
                                if (target == null) target = p.ownHero;
                                p.minionGetDamageOrHeal(target, 1);
                            }
                        }
                        continue;
                        //****************************************
                    //****************************************secret
                    case CardDB.cardNameEN.secretkeeper:
                        if (p.enemyAnzCards >= 3) p.minionGetBuffed(m, 1, 1);
                        continue;
                    case CardDB.cardNameEN.etherealarcanist:
                        if (p.enemyAnzCards >= 3 || p.enemySecretCount > 0) p.minionGetBuffed(m, 2, 2);
                        continue;
                        //****************************************
                    //****************************************play
                    case CardDB.cardNameEN.illidanstormrage:
                        if (p.enemyAnzCards >= 1) p.callKid(flame, p.enemyMinions.Count, false);
                        continue;
                    case CardDB.cardNameEN.questingadventurer:
                        if (p.enemyAnzCards >= 1)
                        {
                            p.minionGetBuffed(m, 1, 1);
                            if (p.enemyAnzCards >= 3 && p.enemyMaxMana >= 5) p.minionGetBuffed(m, 1, 1);
                        }
                        continue;
                    case CardDB.cardNameEN.unboundelemental:
                        if (p.enemyAnzCards >= 2)p.minionGetBuffed(m, 1, 1);
                        continue;
                        //****************************************
                    //****************************************turn
                    //****************************************armor
                    case CardDB.cardNameEN.siegeengine:
                        anz = 0;
                        foreach (Minion mnn in p.enemyMinions)
                        {
                            if (mnn.name == CardDB.cardNameEN.armorsmith) anz++;
                        }
                        if (p.enemyAnzCards >= 3) anz++;
                        if (anz > 0) p.minionGetBuffed(m, anz, 0);
                        continue;
                    //****************************************summon
                    case CardDB.cardNameEN.murloctidecaller:
                        if (p.enemyAnzCards >= 2) p.minionGetBuffed(m, 1, 0);
                        continue;
                    case CardDB.cardNameEN.undertaker:
                        if (p.enemyAnzCards >= 2) p.minionGetBuffed(m, 1, 0);
                        continue;
                    case CardDB.cardNameEN.starvingbuzzard:
                        if (p.enemyAnzCards >= 2) p.drawACard(CardDB.cardNameEN.unknown, false);
                        continue;
                    case CardDB.cardNameEN.cobaltguardian:
                        if (p.enemyAnzCards >= 2) m.divineshild = true;
                        continue;
                    case CardDB.cardNameEN.knifejuggler:
                        anz = Math.Min(p.enemyAnzCards, (int)p.enemyMaxMana/2);
                        if (anz > 0)
                        {
                            Minion target = p.ownHero;
                            for (; anz > 0; anz--)
                            {
                                if (p.ownMinions.Count > 0) target = p.searchRandomMinion(p.ownMinions, searchmode.searchLowestHP);
                                if (target == null) target = p.ownHero;
                                p.minionGetDamageOrHeal(target, 1);
                            }
                        }
                        continue;
                    case CardDB.cardNameEN.shipscannon:
                        if (p.enemyAnzCards >= 2)
                        {
                            Minion target = p.ownHero;
                            if (p.ownMinions.Count > 0) target = p.searchRandomMinion(p.ownMinions, searchmode.searchLowestHP);
                            if (target == null) target = p.ownHero;
                            p.minionGetDamageOrHeal(target, 1);
                        }
                        continue;
                    case CardDB.cardNameEN.tundrarhino:
                        p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_125), p.enemyMinions.Count, false, true, true);
                        continue;
                        //****************************************
                    //****************************************damage
                    case CardDB.cardNameEN.frothingberserker:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3) p.minionGetBuffed(m, 1, 0);
                        continue;
                    case CardDB.cardNameEN.gurubashiberserker:
                        if (m.Hp >= 4 && p.enemyAnzCards >= 3) p.minionGetBuffed(m, 3, 0);
                        continue;
                    case CardDB.cardNameEN.floatingwatcher:
                        if (p.enemyMaxMana >= p.enemyAnzCards * 2) p.minionGetBuffed(m, 2, 2);
                        continue;
                    case CardDB.cardNameEN.armorsmith:
                        if (p.enemyMinions.Count >= 3) p.minionGetArmor(p.enemyHero, 1);
                        continue;
                    case CardDB.cardNameEN.gahzrilla:
                        if (m.Hp >= 4 && p.enemyAnzCards >= 3) p.minionGetBuffed(m, m.Angr * 2, 0);
                        continue;
                    case CardDB.cardNameEN.acolyteofpain:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3) p.drawACard(CardDB.cardNameEN.unknown, false);
                        continue;
                    case CardDB.cardNameEN.mechbearcat:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3) p.drawACard(CardDB.cardNameEN.unknown, false);
                        continue;
                    case CardDB.cardNameEN.grimpatron:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3)  p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRM_019), p.enemyMinions.Count, false);
                        continue;
                    case CardDB.cardNameEN.dragonegg:
                        if (p.enemyAnzCards >= 3) p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRM_022t), p.enemyMinions.Count, false);
                        continue;
                    case CardDB.cardNameEN.impgangboss:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3) p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRM_006t), p.enemyMinions.Count, false);
                        continue;
                    case CardDB.cardNameEN.axeflinger:
                        if (m.Hp >= 3 && p.enemyAnzCards >= 3) p.minionGetDamageOrHeal(p.ownHero, 2);
                        continue;
                    case CardDB.cardNameEN.brannbronzebeard:
                        p.minionGetBuffed(m, 0, 6);
                        continue;
                    case CardDB.cardNameEN.obsidiandestroyer:
                        if (p.enemyMinions.Count < 6) p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOE_009t), p.enemyMinions.Count, false);
                        continue;
                    case CardDB.cardNameEN.tunneltrogg:                        
                        p.minionGetBuffed(m, 1, 0);
                        continue;
                    case CardDB.cardNameEN.summoningstone:
                        if (p.enemyMinions.Count < 6) p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOE_017), p.enemyMinions.Count, false);
                        continue;
                        //****************************************
                    //****************************************dies (rough approximation)
                    //****************************************
                }
            }
            p.doDmgTriggers();
        }


    }

}