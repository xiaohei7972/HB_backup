using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//* 战路 Warpath
    //<b>Echo</b>Deal 1 damage to all_minions.
    //<b>回响</b>对所有随从造成1点伤害。
	class Sim_GIL_654 : SimTemplate //战路
	{

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
			p.allMinionsGetDamage(dmg);

		}


        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                // new PlayReq(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS,1),
            };
        }
	}
}