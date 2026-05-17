using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 无效的 费用：6 攻击力：6 生命值：7
	//Ultraxion
	//奥卓克希昂
	//[x]<b>Battlecry:</b> <b>Herald</b> {0}.Reduce Deathwing'sCost by ({1}). <i>(<b>Herald</b> to improve!)</i>
	//<b>战吼：</b><b>兆示</b>{0}。使死亡之翼的法力值消耗减少（{1}）点。<i>（随<b>兆示</b>的次数提升！）</i>
	class Sim_CATA_497 : SimTemplate
	{
		// Deathwing card IDs
		private static readonly CardDB.cardIDEnum Deathwing = CardDB.cardIDEnum.NEW1_030;
		private static readonly CardDB.cardIDEnum CataDeathwing = CardDB.cardIDEnum.CATA_190h;

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Track Herald count (each Herald played reduces upcoming Deathwing costs further)
			// Use script data to track cumulative Herald activations
			own.TAG_SCRIPT_DATA_NUM_1++;

			int heraldCount = own.TAG_SCRIPT_DATA_NUM_1;
			int costReduction = 2 + heraldCount; // Scales with Herald count

			// Reduce Deathwing cost in hand and deck
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if (hc.card.cardIDenum == Deathwing || hc.card.cardIDenum == CataDeathwing)
				{
					hc.manacost = Math.Max(0, hc.manacost - costReduction);
				}
			}

			// Also flag that Deathwing is discounted (for future draws through deck tracking)
			p.evaluatePenality -= 3 + heraldCount;
		}
	}
}
