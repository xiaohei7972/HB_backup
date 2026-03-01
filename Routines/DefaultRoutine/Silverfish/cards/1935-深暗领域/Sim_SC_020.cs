using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 术士 费用：1
	//Consume
	//吞噬
	//[x]Remove 1 Durabilityfrom a friendly locationto restore #8 Health toyour hero.
	//移除一个友方地标的1点耐久度，为你的英雄恢复#8点生命值。
	class Sim_SC_020 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				int heal = ownplay ? p.getSpellHeal(-8) : p.getEnemySpellHeal(-8);
				target.CooldownTurn--;
				p.minionGetDamageOrHeal(ownplay ? p.ownHero : p.enemyHero, heal);
			}
		}

        public override PlayReq[] GetPlayReqs()
        {
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_LOCATION_TARGET),
			};
        }
		
	}
}
