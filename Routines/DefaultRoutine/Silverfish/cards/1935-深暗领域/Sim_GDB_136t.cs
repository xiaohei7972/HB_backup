using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Utils;

namespace HREngine.Bots
{
	//地标 萨满祭司 费用：5
	//The Galaxy's Lens
	//星系投影
	//<b><b>Spellburst</b>:</b> Absorb the spell's power!@Cast {0}.
	//<b><b>法术迸发</b>：</b>吸收法术的能量！@施放{0}。
	class Sim_GDB_136t : SimTemplate
	{
		CardDB.Card card = null;
		PlayReq[] playReqs = new PlayReq[] { };
		public override void useLocation(Playfield p, Minion triggerMinion, Minion target)
		{
			if (triggerMinion.handcard.enchs.Count > 0)
			{
				card.sim_card.onCardPlay(p, triggerMinion.own, target, 1);
			}
		}

		public override void OnSpellburst(Playfield p, Minion m, Handmanager.Handcard hc)
		{
			card = hc.card;
			playReqs = hc.card.sim_card.GetPlayReqs();
			m.handcard.enchs.Add(hc.card.cardIDenum);
		}

		public override PlayReq[] GetUseAbilityReqs()
		{
			return playReqs;
			// return new PlayReq[]{
			// 	new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
			// };
		}

	}
}
