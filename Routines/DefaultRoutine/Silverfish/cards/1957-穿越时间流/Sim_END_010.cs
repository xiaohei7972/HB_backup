using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//中立 死亡骑士 费用：6 攻击力：5 生命值：5
	//Twilight Timereaver
	//暮光时光撕裂者
	//Choose One - Set the Attack of all other minions to 1; or Health to 1.
	//抉择——将所有其他随从的攻击力变为1；或者将所有其他随从的生命值变为1。
	class Sim_END_010 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			// choice: 1 = set attack to 1, 2 = set health to 1
			bool setAttack = (choice == 1);

			// Apply to all other minions (both sides)
			foreach (Minion m in p.ownMinions)
			{
				if (m.entityID == own.entityID) continue;
				if (setAttack)
					m.Angr = 1;
				else
					m.Hp = 1;
			}
			foreach (Minion m in p.enemyMinions)
			{
				if (setAttack)
					m.Angr = 1;
				else
					m.Hp = 1;
			}

			// Huge board impact
			p.evaluatePenality -= 8;
		}
	}
}
