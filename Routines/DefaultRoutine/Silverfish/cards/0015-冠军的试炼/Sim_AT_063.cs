using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//* 酸喉 Acidmaw
	//Whenever another minion takes damage, destroy it.
	//每当有敌方随从受到伤害时，将其消灭。
	class Sim_AT_063 : SimTemplate
	{


		public override void onAuraStarts(Playfield p, Minion own)
		{
			p.anzAcidmaw++;
		}

		public override void onAuraEnds(Playfield p, Minion m)
		{
			p.anzAcidmaw--;
		}
	}
}