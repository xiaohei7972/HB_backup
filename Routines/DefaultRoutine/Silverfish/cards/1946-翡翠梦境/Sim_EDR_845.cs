using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：5 攻击力：5 生命值：6
	//Hamuul Runetotem
	//哈缪尔·符文图腾
	//[x]<b>Start of Game:</b> If each spellin your deck is Nature, <b>Imbue</b>your Hero Power. Repeat this every 3 spells you cast.
	//<b>对战开始时：</b>如果你套牌中的每张法术牌均为自然法术，<b>灌注</b>你的英雄技能。你每施放3个法术，重复此效果。
	class Sim_EDR_845 : SimTemplate
	{
		// Start of Game: Imbue Hero Power. Repeat every 3 spells cast.
		// Imbue improves your Hero Power. We simulate by granting a permanent
		// mana cost reduction and a small hero power boost.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// Simulate Imbue: reduce Hero Power cost and provide a small buff
			if (own.own)
			{
				// Imbued Hero Power is typically cheaper and stronger
				// Represent this as mana gain and hero power improvement
				p.ownMaxMana += 1;
				p.mana += 1;

				// Also summon a 2/2 Treant or provide a minion buff
				// to represent the Imbued hero power effect
				foreach (Minion m in p.ownMinions)
				{
					p.minionGetBuffed(m, 1, 1);
				}
			}
		}
		
	}
}
