using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//随从 中立 费用：5 攻击力：4 生命值：5
	//Star Vulpera
	//星际狐人
	//[x]<b>Tradeable</b>_<b>Battlecry:</b> Destroy an enemy_<b>Starship</b> or <b>Starship Piece</b>.
	//<b>可交易</b><b>战吼：</b>消灭一个敌方<b>星舰</b>或<b>星舰组件</b>。
	class Sim_GDB_340 : SimTemplate
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				p.minionGetDestroyed(target);
			}
		}


		public override PlayReq[] GetPlayReqs()
		{
			return new PlayReq[] {
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE),
				new PlayReq(CardDB.ErrorType2.REQ_ENEMY_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_MINION_TARGET),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG,CardDB.Specialtags.StarshipPiece),
				new PlayReq(CardDB.ErrorType2.REQ_TARGET_MUST_HAVE_TAG,CardDB.Specialtags.Starship),
			};
		}

	}
}
