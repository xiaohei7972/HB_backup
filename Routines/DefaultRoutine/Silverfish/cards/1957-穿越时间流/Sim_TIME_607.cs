using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：8 攻击力：8 生命值：8
	//Frostmourne Remnant
	//霜之哀伤残片
	//<b>Battlecry:</b> Summon a 6/6 Frostwyrm for each Corpses spent this game. (max 3)
	//<b>战吼：</b>在本局对战中每消耗过一份残骸，召唤一个6/6的冰霜巨龙。（最多3个）
	class Sim_TIME_607 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Count total corpses spent this game from ownGraveyard
			// Corpses are tracked via CS2_122 key in ownGraveyard
			// The total spent = initial corpses - remaining corpses
			// We approximate by checking total minions that died (corpse generation events)
			int corpsesSpent = 0;
			if (p.ownGraveyard.ContainsKey(CardDB.cardIDEnum.CS2_122))
			{
				corpsesSpent = p.ownGraveyard[CardDB.cardIDEnum.CS2_122];
			}

			// Also count from the corpse consumption tracking
			int maxFrostwyrms = System.Math.Min(corpsesSpent, 3);
			maxFrostwyrms = System.Math.Min(maxFrostwyrms, 7 - (own.own ? p.ownMinions.Count : p.enemyMinions.Count));

			for (int i = 0; i < maxFrostwyrms; i++)
			{
				int pos = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
				p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.None), pos, own.own);
				if (pos < (own.own ? p.ownMinions.Count : p.enemyMinions.Count))
				{
					Minion wyrm = own.own ? p.ownMinions[pos] : p.enemyMinions[pos];
					p.minionGetBuffed(wyrm, 6, 6);
				}
			}
		}
	}
}
