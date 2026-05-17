using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 萨满祭司 费用：6 攻击力：4 生命值：4
	//Merithra
	//麦琳瑟拉
	//[x]<b>Battlecry:</b> Resurrect alldifferent friendly minionsthat cost (8) or more.
	//<b>战吼：</b>复活所有法力值消耗大于或等于（8）点的不同的友方随从。
	class Sim_EDR_238 : SimTemplate
	{
		// Resurrect all different friendly minions that cost (8) or more.
		// Search the graveyard and summon high-cost minions.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			int pos = p.ownMinions.Count;

			// Iterate through own graveyard to find 8+ cost minions
			HashSet<CardDB.cardIDEnum> resurrected = new HashSet<CardDB.cardIDEnum>();
			foreach (KeyValuePair<CardDB.cardIDEnum, int> kvp in p.ownGraveyard)
			{
				CardDB.Card graveCard = CardDB.Instance.getCardDataFromID(kvp.Key);
				if (graveCard.type == CardDB.cardtype.MOB && graveCard.cost >= 8)
				{
					if (!resurrected.Contains(kvp.Key))
					{
						resurrected.Add(kvp.Key);
						// Summon each qualifying minion
						p.callKid(graveCard, pos, own.own);
						pos++;
					}
				}
			}
		}
		
	}
}
