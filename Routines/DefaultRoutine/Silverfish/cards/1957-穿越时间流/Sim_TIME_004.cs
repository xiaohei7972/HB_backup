using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：7 攻击力：7 生命值：7
	//Conflux Crasher
	//时光流汇扫荡者
	//<b>Rewind</b><b>Battlecry:</b> Deal 7 damageto a random enemy.
	//<b>回溯</b>。<b>战吼：</b>随机对一个敌人造成7点伤害。
	class Sim_TIME_004 : SimTemplate
	{
		// public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		// {
		// 	// 获取敌方随从列表的副本，避免在遍历时修改原始列表
		// 	List<Minion> minions = new List<Minion>();
		// 	if (own.own)
		// 	{
		// 		minions.AddRange(p.enemyHero);
		// 		minions.Add(p.enemyHero);
		// 	}
		// 	else
		// 	{
		// 		minions.AddRange(p.ownMinions);
		// 		minions.Add(p.ownMinions);
		// 	}
		// 	// 遍历每个敌方随从
		// 	foreach (Minion minion in minions)
		// 	{
		// 		if (minion.untouchable) continue;
		// 		p.minionGetDamageOrHeal(minion, 7);
		// 		break;
		// 	}
		// }

	}
}
