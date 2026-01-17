using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HREngine.Bots
{

    //每个策略的 Penality{策略名}文件里面放 三个函数：打牌评分，随从进攻评分；英雄进攻评分 以及他们需要用到的private函数
    //这三个函数用于单动作评估，如果返回值超过500，则被剪枝，不列入候选动作
    /// <summary>
    /// 便宜策略使用需谨慎
    /// </summary>
    public partial class Behavior丨狂野丨宇宙兽王猎 : Behavior
    {
        public override int getPlayCardPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.一串香蕉_ETC_201:
                case CardDB.cardNameCN.一串香蕉_ETC_201t:
                case CardDB.cardNameCN.一串香蕉_ETC_201t2:
                case CardDB.cardNameCN.梦魇:
                case CardDB.cardNameCN.新鲜气息_YOD_005:
                case CardDB.cardNameCN.新鲜气息_YOD_005ts:
                case CardDB.cardNameCN.探险帽:
                case CardDB.cardNameCN.凶猛狂暴:
                case CardDB.cardNameCN.两栖之灵:
                case CardDB.cardNameCN.虫外有虫:
                case CardDB.cardNameCN.迷彩坐骑:
                case CardDB.cardNameCN.狗狗饼干:
                case CardDB.cardNameCN.狗狗饼干_DED_009:
                case CardDB.cardNameCN.山羊坐骑:
                case CardDB.cardNameCN.野兽之心:
                case CardDB.cardNameCN.地精的把戏:
                case CardDB.cardNameCN.狂野怒火:
                case CardDB.cardNameCN.魔暴龙面具:
                case CardDB.cardNameCN.香蕉:
                case CardDB.cardNameCN.无穷香蕉:
                case CardDB.cardNameCN.萌物来袭:

                    {
                        if (target != null && !target.own) return 1000;

                    }
                    break;

                case CardDB.cardNameCN.瞄准射击:
                    if (target != null && target.own) return 1000;
                    break;
                case CardDB.cardNameCN.恐惧畏缩:
                case CardDB.cardNameCN.激光弹幕:
                case CardDB.cardNameCN.眼镜蛇射击:
                case CardDB.cardNameCN.剧毒箭矢:
                case CardDB.cardNameCN.标记射击:
                case CardDB.cardNameCN.咒术之箭:
                case CardDB.cardNameCN.灭龙射击:
                case CardDB.cardNameCN.布置陷阱:
                case CardDB.cardNameCN.腐蚀吐息:
                case CardDB.cardNameCN.数量压制:
                case CardDB.cardNameCN.套索射击:
                case CardDB.cardNameCN.穿刺射击:
                case CardDB.cardNameCN.飞翼冲击:
                case CardDB.cardNameCN.侧翼打击:
                case CardDB.cardNameCN.强风射击:
                case CardDB.cardNameCN.爆炸射击:
                    {
                        if (target != null && !target.isHero)
                        {
                            if (target.own) return 1000;
                        }
                        else
                        {
                            return 1000;
                        }

                    }
                    break;

                case CardDB.cardNameCN.梦境:
                    if (target != null && target.own && target.charge == 0) return 1000;
                    break;

                case CardDB.cardNameCN.黑铁矮人:
                case CardDB.cardNameCN.叫嚣的中士:
                    if (p.ownMinions.Count > 0 && target != null && !target.own) return 1000;
                    break;
                case CardDB.cardNameCN.杀戮命令:
                case CardDB.cardNameCN.奥术射击:
                case CardDB.cardNameCN.猎人印记:
                    if (target.own) return 1000;
                    break;
                case CardDB.cardNameCN.铁喙猫头鹰:
                    if (p.enemyMinions.Count > 0 && target != null && target.own && !target.frozen && !(target.Angr < target.handcard.card.Attack))
                    {
                        return 1000;
                    }
                    break;
                case CardDB.cardNameCN.防护改装师:
                    if (target != null && target.own && target.Hp == 1) return 1000;
                    break;
                case CardDB.cardNameCN.暴掠龙女王:
                    if (target != null && target.own && target.Hp == 1) return 1000;
                    break;
            }
            return getComboPenality(card, target, p, nowHandcard);
        }

        public override int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            if (!m.silenced && m.handcard.card.CantAttack || target.untouchable)
                return 1000;
            int pen = -10;
            if (target.isHero && p.calTotalAngr() >= p.enemyHero.Hp)
            {
                return -1000;
            }
            return pen;
        }

        //英雄攻击惩罚
        public override int getAttackWithHeroPenality(Minion target, Playfield p)
        {
            if (target.untouchable)
                return 1000;
            int pen = -10;
            if (target.isHero && p.calTotalAngr() >= p.enemyHero.Hp) return -1000;
            return pen;
        }

        private int getSecretPenality(Playfield p)  // 牌序和防奥秘的影响
        {
            return 0;
        }
    }
}
