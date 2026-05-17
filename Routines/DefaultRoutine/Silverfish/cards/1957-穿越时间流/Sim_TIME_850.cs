using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 战士 费用：7 攻击力：7 生命值：7
	//Lo'Gosh, Blood Fighter
	//血斗士洛戈什
	//[x]<b>Fabled</b>, <b>Rush</b>. <b>Deathrattle:</b>Summon a Blood Fighter fromyour hand. It gains +5/+5 andattacks a random enemy.
	//<b>奇闻</b><b>突袭</b>。<b>亡语：</b>从你的手牌中召唤一位血斗士，使其获得+5/+5并随机攻击一个敌人。
	class Sim_TIME_850 : SimTemplate
	{
		public override void onDeathrattle(Playfield p, Minion m)
		{
			if (!m.own) return;

			Handmanager.Handcard targetHc = null;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.cardIDenum == CardDB.cardIDEnum.TIME_850)
				{
					targetHc = hc;
					break;
				}
			}
			if (targetHc != null)
			{
				p.removeCard(targetHc);
				int pos = p.ownMinions.Count;
				p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TIME_850t), pos, true);
				if (pos < p.ownMinions.Count)
				{
					p.minionGetBuffed(p.ownMinions[pos], 5, 5);
				}
			}
		}
	}
}
