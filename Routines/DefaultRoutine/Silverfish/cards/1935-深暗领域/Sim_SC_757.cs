using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 潜行者 费用：1
	//Hallucination
	//幻像
	//Summon a copy of afriendly Protoss minion.It takes double damage.
	//召唤一个友方星灵随从的一个复制。该复制受到的伤害翻倍。
	class Sim_SC_757 : SimTemplate
	{
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
        {
			if (target != null)
			{
				Minion summoned = p.callKidAndReturn(target.handcard.card,target.zonepos,target.own);
				if (summoned != null)
				{
					summoned.setMinionToMinion(target);
				}
			}
        }

        public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),	
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),	
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),	
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG,CardDB.Specialtags.Protoss),	
			};
        }
		
	}
}
