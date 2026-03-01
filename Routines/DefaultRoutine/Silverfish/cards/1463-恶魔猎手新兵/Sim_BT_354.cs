using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	//法术 恶魔猎手 费用：3
	//Blade Dance
	//刃舞
	//Deal damage equal to your hero's Attack to 3 random enemy minions.
	//随机对三个敌方随从造成等同于你的英雄攻击力的伤害。
	class Sim_BT_354 : SimTemplate
	{
		
		public override PlayReq[] GetPlayReqs()
        {
            return new PlayReq[] {

                new PlayReq(CardDB.ErrorType2.REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK),
                new PlayReq(CardDB.ErrorType2.REQ_MINIMUM_ENEMY_MINIONS,1),

            };
        }
	}
}
