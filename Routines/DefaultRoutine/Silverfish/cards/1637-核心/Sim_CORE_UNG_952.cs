using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 圣骑士 费用：5
	//Spikeridged Steed
	//剑龙骑术
	//Give a minion +2/+6 and <b>Taunt</b>. When it dies, summon a Stegodon.
	//使一个随从获得+2/+6和<b>嘲讽</b>。当该随从死亡时，召唤一只剑龙。
	class Sim_CORE_UNG_952 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
            if (target != null)
            {

                p.minionGetBuffed(target, 2, 6);
                target.stegodon++;
            }
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {
                new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
                new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
            };
        }
		
	}
}
