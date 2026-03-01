using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：2 攻击力：2 生命值：1
	//Ancient Raptor
	//远古迅猛龙
	//[x]<b>Battlecry:</b> Choose to gain+3 Attack, <b>Divine Shield</b>,or "<b>Deathrattle:</b> Summontwo 1/1 Plants."
	//<b>战吼：</b>从+3攻击力，<b>圣盾</b>或“<b>亡语：</b>召唤两个1/1的植物”中选择一项并获得。
	class Sim_TLC_245 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (choice == 1)
			{
				p.minionGetBuffed(own, 3, 0);
			}
			if (choice == 2)
			{
				own.divineshild = true;
			}
			if (choice == 3)
			{
				own.livingspores++;
			}
		}
		
	}
}
