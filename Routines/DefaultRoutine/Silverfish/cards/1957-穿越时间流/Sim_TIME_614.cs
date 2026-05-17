using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 巫妖王 费用：3 攻击力：4 生命值：3
	//San'layn Infiltrator
	//萨莱因渗透者
	//<b>Stealth</b>. <b>Battlecry:</b> If you spent 3 Corpses this turn, gain +2/+2 and <b>Divine Shield</b>.
	//<b>潜行</b>。<b>战吼：</b>如果你在本回合中消耗了3份残骸，获得+2/+2和<b>圣盾</b>。
	class Sim_TIME_614 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Check if 3+ Corpses have been spent this game
			// We check the total corpses consumed via CS2_122 tracking
			int corpseCount = p.getCorpseCount();
			// Simplification: if we have any corpse consumption activity, provide the buff
			// In a full implementation, this would track per-turn spending
			if (corpseCount >= 3 || p.ownGraveyard.ContainsKey(CardDB.cardIDEnum.CS2_122))
			{
				p.minionGetBuffed(own, 2, 2);
				own.divineShield = true;
			}
		}
	}
}
