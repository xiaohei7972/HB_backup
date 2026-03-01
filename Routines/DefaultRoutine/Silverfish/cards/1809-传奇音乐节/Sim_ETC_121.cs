using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：3 攻击力：4 生命值：3
	//Rock Master Voone
	//摇滚教父沃恩
	//<b>Battlecry:</b> Copy each minion of a different type in your hand.
	//<b>战吼：</b>复制你手牌中每个不同类型的随从牌各一张。
	class Sim_ETC_121 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{

			if (own.own)
			{
				List<CardDB.Race> races = new List<CardDB.Race>();
				foreach (Handmanager.Handcard hc in p.owncards)
				{
					foreach (CardDB.Race race in hc.card.GetRaces())
					{
						if (race == CardDB.Race.ALL)
						{
							p.drawACard(hc.card.cardIDenum, true, true);
							goto end;
						}
						else if (!races.Contains(race))
						{
							races.Add(race);
							p.drawACard(hc.card.cardIDenum, true, true);
							goto end;
						}
					}
					end:;

				}

			}
		}

	}
}
