using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 圣骑士 费用：8 攻击力：8 生命值：8
	//Chromatus
	//克洛玛图斯
	//<b>Colossal +4</b><b>Taunt</b>, <b>Lifesteal</b>, <b>Elusive</b>, <b>Divine Shield</b>
	//<b>巨型+4</b><b>嘲讽</b>。<b>吸血</b>。<b>扰魔</b><b>圣盾</b>
	class Sim_CATA_432 : SimTemplate
	{
		// Keywords (Taunt, Lifesteal, Elusive, Divine Shield) handled by HREngine
		// Colossal +4 handled by HREngine (summons 4 heads: 432t1-432t4)
		// Those heads have Deathrattle that removes keywords, also handled by HREngine

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Chromatus is a premium keyword bundle with premium stats
			// Taunt protects face, Lifesteal provides healing, Elusive prevents targeting, Divine Shield absorbs damage
			p.evaluatePenality -= 12; // very strong defensive keyword bundle on an 8/8 body
		}
	}
}
