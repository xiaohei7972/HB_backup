using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 巫妖王 费用：3
	//Death's Embrace
	//死亡之拥
	//Give a friendly minion "<b>Deathrattle:</b> Raise 3 Corpses."
	//使一个友方随从获得"<b>亡语：</b>获得3份残骸。"
	class Sim_TIME_608 : SimTemplate
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice, Handmanager.Handcard hc)
		{
			if (target != null)
			{
				// Raise 3 Corpses immediately as a simplified implementation,
				// since fully dynamic deathrattle attachment requires a dedicated token card.
				p.addCorpses(3);
			}
		}

		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[]
			{
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_TO_PLAY),
				new PlayReq(CardDB.ErrorType2.REQ_FRIENDLY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
			};
		}
	}
}
