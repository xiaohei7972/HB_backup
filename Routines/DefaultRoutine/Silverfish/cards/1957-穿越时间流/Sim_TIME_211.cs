using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：5 攻击力：5 生命值：5
	//Lady Azshara
	//艾萨拉女士
	//[x]<b>Fabled</b>. <b>Choose One -</b>Empower Zin-Azshari; orThe Well of Eternity. <i>(The__other gets destroyed!)</i>
	//<b>奇闻</b><b>抉择：</b>强化辛艾萨莉；或者永恒之井。<i>（未选择的选项会被摧毁！）</i>
	class Sim_TIME_211 : SimTemplate
	{
		CardDB.Card TheWellofEternity = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_211t1t);
		CardDB.Card ZinAzshari = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_211t2t);
		public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
        {
			//强化辛艾萨莉
			if (choice == 1 || p.ownFandralStaghelm >= 1)
			{
				foreach (Minion m in own.own ? p.ownMinions.ToArray() : p.enemyMinions.ToArray())
				{
					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1 || m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1t)
					{
						p.minionGetDestroyed(m);
						continue;
					}

					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2)
					{
						p.minionTransform(m, ZinAzshari);
						continue;
					}
					
				}
                if (own.own)
                {
                    foreach (Handmanager.Handcard hd in p.owncards.ToArray())
                    {
                        if (hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1 || hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1t)
                        {
                            p.removeCard(hd);
                            continue;

                        }

                        if (hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2)
                        {
                            p.removeCard(hd);
                            p.drawACard(CardDB.cardIDEnum.TIME_211t2t, own.own, true);
                            continue;

                        }
                    }
                }
            }
			//强化永恒之井
			if (choice == 2 || p.ownFandralStaghelm >= 1)
			{
				foreach (Minion m in own.own ? p.ownMinions.ToArray() : p.enemyMinions.ToArray())
				{
					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2 || m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2t)
					{
						p.minionGetDestroyed(m);
						continue;
					}

					if (m.handcard.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1t)
					{
						p.minionTransform(m, TheWellofEternity);
						continue;
					}


				}
                if (own.own)
                {
                    foreach (Handmanager.Handcard hd in p.owncards.ToArray())
                    {
                        if (hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2 || hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t2t)
                        {
                            p.removeCard(hd);
                            continue;

                        }

                        if (hd.card.cardIDenum == CardDB.cardIDEnum.TIME_211t1)
                        {
                            p.removeCard(hd);
                            p.drawACard(CardDB.cardIDEnum.TIME_211t1t, own.own, true);
                            continue;

                        }
                    }
                }
            }
		}

	}
}
