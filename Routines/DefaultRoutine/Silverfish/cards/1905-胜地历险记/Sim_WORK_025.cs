using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 德鲁伊 费用：4 攻击力：2 生命值：1
	//Number Cruncher
	//数据分析狮
	//[x]<b><b>Rush</b>, Taunt</b><b>Choose Thrice -</b> Gain+2 Attack; or +2 Health.
	//<b><b>突袭</b>。嘲讽</b>。<b>选择三次：</b>获得+2攻击力；或者+2生命值。
	class Sim_WORK_025 : SimTemplate
	{
		public override void onCardPlay(Playfield p, Minion own, Minion target, int choice, Handmanager.Handcard hc)
        {
			for (int i = 0; i < 3; i++)
			{
				if (choice == 1)
				{
					p.minionGetBuffed(own, 2, 0);
				}

				if (choice == 2)
				{
					p.minionGetBuffed(own, 0, 2);
				}
			}
		}
	}
}
