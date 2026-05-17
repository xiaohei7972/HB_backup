using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Triton.Common.LogUtilities;

namespace HREngine.Bots
{
    // 伤害与治疗：伤害/治疗计算、群体伤害/治疗
    public partial class Playfield
    {
        /// <summary>
        /// 计算英雄技能造成的最终伤害值，考虑额外伤害和效果加成。
        /// </summary>
        /// <param name="dmg">基础伤害值。</param>
        /// <returns>最终计算后的英雄技能伤害值。</returns>
        public int getHeroPowerDamage(int dmg)
        {
            // 加上由于其他效果或增益而获得的额外伤害
            dmg += this.ownHeroPowerExtraDamage;

            // 如果存在双倍治疗效果（如奥金尼灵魂祭司），伤害值乘以双倍效果的数量
            if (this.doublepriest >= 1)
            {
                dmg *= (2 * this.doublepriest);
            }

            // 返回最终计算的伤害值
            return dmg;
        }

        /// <summary>
        /// 计算敌方英雄技能造成的最终伤害值，考虑额外伤害和效果加成。
        /// </summary>
        /// <param name="dmg">基础伤害值。</param>
        /// <returns>最终计算后的敌方英雄技能伤害值。</returns>
        public int getEnemyHeroPowerDamage(int dmg)
        {
            // 加上由于敌方增益效果或其他因素导致的额外伤害
            dmg += this.enemyHeroPowerExtraDamage;

            // 如果敌方存在双倍治疗效果（如奥金尼灵魂祭司），伤害值乘以双倍效果的数量
            if (this.enemydoublepriest >= 1)
            {
                dmg *= (2 * this.enemydoublepriest);
            }

            // 返回最终计算的伤害值
            return dmg;
        }

        /// <summary>
        /// 计算施法造成的最终伤害值，考虑法术伤害加成和效果倍增。
        /// </summary>
        /// <param name="dmg">基础伤害值。</param>
        /// <returns>最终计算后的法术伤害值。</returns>
        public int getSpellDamageDamage(int dmg)
        {
            int retval = dmg;  // 初始化返回值为基础伤害值

            // 加上法术伤害增益（如法术伤害+1效果）
            retval += this.spellpower;

            // 如果存在双倍治疗效果（如奥金尼灵魂祭司），伤害值乘以双倍效果的数量
            if (this.doublepriest >= 1)
            {
                retval *= (2 * this.doublepriest);
            }

            // 返回最终计算的法术伤害值
            return retval;
        }

        /// <summary>
        /// 计算法术治疗效果的最终数值，考虑到特殊效果如奥金尼灵魂祭司和阴影之握等。
        /// </summary>
        /// <param name="heal">基础治疗量。</param>
        /// <returns>最终计算后的治疗量或伤害量。</returns>
        public int getSpellHeal(int heal)
        {
            int retval = heal;  // 初始化返回值为基础治疗量

            // 检查是否有奥金尼灵魂祭司或阴影之握效果，这些效果会将治疗转化为伤害
            if (this.anzOwnAuchenaiSoulpriest > 0 || this.embracetheshadow > 0)
            {
                retval *= -1;  // 将治疗量转为负值，表示伤害
                retval -= this.spellpower;  // 减去法术伤害增益，使得转化后的伤害更高
            }

            // 如果存在双倍治疗效果（如双倍治疗牧师），对治疗量或伤害量乘以双倍效果的数量
            if (this.doublepriest >= 1)
            {
                retval *= (2 * this.doublepriest);
            }

            // 返回最终计算的治疗量或伤害量
            return retval;
        }

        /// <summary>
        /// 处理法术的吸血效果，根据法术治疗量为英雄恢复生命值或造成伤害。
        /// </summary>
        /// <param name="heal">法术治疗量。</param>
        /// <param name="own">是否是己方英雄（true 表示己方，false 表示敌方）。</param>
        public void applySpellLifesteal(int heal, bool own)
        {
            // 判断是否应将治疗转化为伤害
            bool minus = own ?
                (this.anzOwnAuchenaiSoulpriest > 0 || this.embracetheshadow > 0) :
                (this.anzEnemyAuchenaiSoulpriest > 0);

            // 计算吸血效果并应用于对应的英雄
            this.minionGetDamageOrHeal(own ? ownHero : enemyHero, -heal * (minus ? -1 : 1));
        }

        /// <summary>
        /// 计算随从治疗量，如果存在奥金尼灵魂祭司或阴影之握效果，将治疗转化为伤害。
        /// </summary>
        /// <param name="heal">基础治疗量。</param>
        /// <returns>最终的治疗量或伤害量。</returns>
        public int getMinionHeal(int heal)
        {
            // 如果存在奥金尼灵魂祭司或阴影之握效果，将治疗转化为伤害（即返回负的治疗量）
            return (this.anzOwnAuchenaiSoulpriest > 0 || this.embracetheshadow > 0) ? -heal : heal;
        }

        /// <summary>
        /// 计算敌方法术造成的最终伤害值，考虑法术伤害加成和效果倍增。
        /// </summary>
        /// <param name="dmg">基础伤害值。</param>
        /// <returns>最终计算后的敌方法术伤害值。</returns>
        public int getEnemySpellDamageDamage(int dmg)
        {
            int retval = dmg;  // 初始化返回值为基础伤害值

            // 加上敌方的法术伤害增益（如法术伤害+1效果）
            retval += this.enemyspellpower;

            // 如果敌方存在双倍治疗效果（如奥金尼灵魂祭司），伤害值乘以双倍效果的数量
            if (this.enemydoublepriest >= 1)
            {
                retval *= (2 * this.enemydoublepriest);
            }

            // 返回最终计算的法术伤害值
            return retval;
        }

        /// <summary>
        /// 计算敌方法术的治疗效果，根据敌方的奥金尼灵魂祭司和法术伤害加成决定是否将治疗转化为伤害。
        /// </summary>
        /// <param name="heal">基础治疗量。</param>
        /// <returns>最终计算后的治疗量或伤害量。</returns>
        public int getEnemySpellHeal(int heal)
        {
            int retval = heal;  // 初始化返回值为基础治疗量

            // 如果敌方有奥金尼灵魂祭司存在，将治疗转化为伤害，并且减去法术伤害加成
            if (this.anzEnemyAuchenaiSoulpriest >= 1)
            {
                retval *= -1;  // 将治疗转化为伤害
                retval -= this.enemyspellpower;  // 减去敌方的法术伤害加成，使得转化后的伤害更高
            }

            // 如果己方存在双倍治疗效果，对治疗量或伤害量进行倍增
            if (this.doublepriest >= 1)
            {
                retval *= (2 * this.doublepriest);
            }

            // 返回最终计算的治疗量或伤害量
            return retval;
        }

        /// <summary>
        /// 计算敌方随从的治疗效果，如果敌方有奥金尼灵魂祭司存在，将治疗转化为伤害。
        /// </summary>
        /// <param name="heal">基础治疗量。</param>
        /// <returns>最终的治疗量或伤害量。</returns>
        public int getEnemyMinionHeal(int heal)
        {
            // 如果敌方有奥金尼灵魂祭司存在，将治疗转化为伤害（即返回负的治疗量）
            return (this.anzEnemyAuchenaiSoulpriest >= 1) ? -heal : heal;
        }

        /// <summary>
        /// 对敌方英雄造成伤害，但确保其生命值不会低于1。
        /// </summary>
        /// <param name="dmg">要造成的伤害值。</param>
        public void attackEnemyHeroWithoutKill(int dmg)
        {
            this.enemyHero.cantLowerHPbelowONE = true; // 设置敌方英雄的HP不低于1的标志
            this.minionGetDamageOrHeal(this.enemyHero, dmg); // 对敌方英雄造成伤害
            this.enemyHero.cantLowerHPbelowONE = false; // 取消敌方英雄的HP保护标志
        }

        /// <summary>
        /// 对指定的随从造成伤害或进行治疗。
        /// </summary>
        /// <param name="m">目标随从</param>
        /// <param name="dmgOrHeal">正数表示伤害，负数表示治疗</param>
        /// <param name="dontDmgLoss">是否计算失去的伤害值（默认为false）</param>
        public void minionGetDamageOrHeal(Minion m, int dmgOrHeal, bool dontDmgLoss = false)
        {
            // 如果随从还活着，则调用随从自身的受伤或治疗方法
            if (m.Hp > 0 && !m.untouchable)
            {
                m.getDamageOrHeal(dmgOrHeal, this, false, dontDmgLoss);
            }
        }

        /// <summary>
        /// 对一方的所有随从造成伤害。
        /// </summary>
        /// <param name="own">是否为己方随从</param>
        /// <param name="damages">伤害值</param>
        public void allMinionOfASideGetDamage(bool own, int damages)
        {
            List<Minion> temp = (own) ? this.ownMinions : this.enemyMinions;
            foreach (Minion m in temp.ToArray())
            {
                minionGetDamageOrHeal(m, damages, true); // 然后对随从造成伤害
            }
            this.updateBoards(); // 更新游戏板状态
        }

        /// <summary>
        /// 对一方的所有角色（英雄和随从）造成伤害。
        /// </summary>
        /// <param name="own">是否为己方角色</param>
        /// <param name="damages">伤害值</param>
        public void allCharsOfASideGetDamage(bool own, int damages)
        {
            List<Minion> temp = (own) ? this.ownMinions : this.enemyMinions;
            foreach (Minion m in temp.ToArray())
            {
                minionGetDamageOrHeal(m, damages, true); // 对每个随从造成伤害
            }

            this.minionGetDamageOrHeal(own ? this.ownHero : this.enemyHero, damages); // 对英雄造成伤害
        }

        /// <summary>
        /// 对一方的所有角色（英雄和随从）随机造成多次伤害。
        /// </summary>
        /// <param name="ownSide">是否为己方角色</param>
        /// <param name="times">伤害次数</param>
        public void allCharsOfASideGetRandomDamage(bool ownSide, int times)
        {
            if ((!ownSide && this.enemyHero.Hp + this.enemyHero.armor <= times) || (ownSide && this.ownHero.Hp + this.ownHero.armor <= times))
            {
                if (!ownSide)
                {
                    int dmg = this.enemyHero.Hp + this.enemyHero.armor;  //假设情况下的伤害值
                    if (this.enemyMinions.Count > 2) dmg--;
                    times = times - dmg;
                    if (this.anzEnemyAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.enemyHero, 1); // 若敌方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.enemyHero, dmg); // 一次性造成所有伤害
                    }
                }
                else
                {
                    int dmg = this.ownHero.Hp + this.ownHero.armor - 1;
                    times = times - dmg;
                    if (this.anzOwnAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.ownHero, 1); // 若己方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.ownHero, dmg); // 一次性造成所有伤害
                    }
                }
            }

            List<Minion> temp = (ownSide) ? new List<Minion>(this.ownMinions) : new List<Minion>(this.enemyMinions);
            temp.Sort((a, b) => { int tmp = a.Hp.CompareTo(b.Hp); return tmp == 0 ? a.Angr - b.Angr : tmp; }); // 按生命值排序

            int border = 1;
            for (int pos = 0; pos < temp.Count; pos++)
            {
                Minion m = temp[pos];
                if (m.divineShield)
                {
                    m.divineShield = false; // 移除圣盾
                    times--;
                    if (times < 1) break;
                }
                if (m.Hp > border)
                {
                    int dmg = Math.Min(m.Hp - border, times); // 计算随从承受的伤害
                    times -= dmg;
                    this.minionGetDamageOrHeal(m, dmg);
                }
                if (times < 1) break;
            }
            if (times > 0)
            {
                int dmg = times;
                if (!ownSide)
                {
                    if (this.anzEnemyAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.enemyHero, 1); // 若敌方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.enemyHero, dmg); // 一次性造成所有伤害
                    }
                }
                else
                {
                    if (this.anzOwnAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.ownHero, 1); // 若己方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.ownHero, dmg); // 一次性造成所有伤害
                    }
                }
            }
        }
        /// <summary>
        /// 对一方的所有角色（英雄和随从）随机造成多次伤害。
        /// </summary>
        /// <param name="ownSide">是否为己方角色</param>
        /// <param name="times">伤害次数</param>
        public void allCharsOfASideMinionGetRandomDamage(bool ownSide, int times)
        {
            if ((!ownSide && this.enemyHero.Hp + this.enemyHero.armor <= times) || (ownSide && this.ownHero.Hp + this.ownHero.armor <= times))
            {
                /* if (!ownSide)
                {
                    int dmg = this.enemyHero.Hp + this.enemyHero.armor;  //假设情况下的伤害值
                    if (this.enemyMinions.Count > 2) dmg--;
                    times = times - dmg;
                    if (this.anzEnemyAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.enemyHero, 1); // 若敌方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.enemyHero, dmg); // 一次性造成所有伤害
                    }
                }
                else
                {
                    int dmg = this.ownHero.Hp + this.ownHero.armor - 1;
                    times = times - dmg;
                    if (this.anzOwnAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.ownHero, 1); // 若己方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.ownHero, dmg); // 一次性造成所有伤害
                    }
                } */
            }

            List<Minion> temp = (ownSide) ? new List<Minion>(this.ownMinions) : new List<Minion>(this.enemyMinions);
            temp.Sort((a, b) => { int tmp = a.Hp.CompareTo(b.Hp); return tmp == 0 ? a.Angr - b.Angr : tmp; }); // 按生命值排序

            int border = 1;
            for (int pos = 0; pos < temp.Count; pos++)
            {
                Minion m = temp[pos];
                if (m.divineShield)
                {
                    m.divineShield = false; // 移除圣盾
                    times--;
                    if (times < 1) break;
                }
                if (m.Hp > border)
                {
                    int dmg = Math.Min(m.Hp - border, times); // 计算随从承受的伤害
                    times -= dmg;
                    this.minionGetDamageOrHeal(m, dmg);
                }
                if (times < 1) break;
            }
            if (times > 0)
            {
                /* int dmg = times;
                if (!ownSide)
                {
                    if (this.anzEnemyAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.enemyHero, 1); // 若敌方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.enemyHero, dmg); // 一次性造成所有伤害
                    }
                }
                else
                {
                    if (this.anzOwnAnimatedArmor > 0)
                    {
                        for (; dmg > 0; dmg--)
                        {
                            this.minionGetDamageOrHeal(this.ownHero, 1); // 若己方有动画护甲，则逐点伤害
                        }
                    }
                    else
                    {
                        this.minionGetDamageOrHeal(this.ownHero, dmg); // 一次性造成所有伤害
                    }
                } */
            }
        }

        /// <summary>
        /// 对所有角色（包括英雄和随从）造成伤害，可选择排除某个特定实体。
        /// </summary>
        /// <param name="damages">要造成的伤害值</param>
        /// <param name="exceptID">排除的实体ID（默认为-1，即不排除任何实体）</param>
        public void allCharsGetDamage(int damages, int exceptID = -1)
        {
            // 对己方所有随从造成伤害，排除exceptID的实体
            foreach (Minion m in this.ownMinions)
            {
                if (m.entityID != exceptID)
                {
                    minionGetDamageOrHeal(m, damages, true);
                }
            }
            // 对敌方所有随从造成伤害，排除exceptID的实体
            foreach (Minion m in this.enemyMinions)
            {
                if (m.entityID != exceptID)
                {
                    minionGetDamageOrHeal(m, damages, true);
                }
            }
            // 对己方英雄造成伤害
            minionGetDamageOrHeal(this.ownHero, damages);
            // 对敌方英雄造成伤害
            minionGetDamageOrHeal(this.enemyHero, damages);
        }

        /// <summary>
        /// 对所有随从造成伤害，可选择排除某个特定实体。
        /// </summary>
        /// <param name="damages">要造成的伤害值</param>
        /// <param name="exceptID">排除的实体ID（默认为-1，即不排除任何实体）</param>
        public void allMinionsGetDamage(int damages, int exceptID = -1)
        {
            // 对己方所有随从造成伤害，排除exceptID的实体
            foreach (Minion m in this.ownMinions.ToArray())
            {
                if (m.entityID == exceptID) continue;
                minionGetDamageOrHeal(m, damages, true);

                // if (m.entityID != exceptID)
                // {
                //     minionGetDamageOrHeal(m, damages, true);
                // }
            }
            // 对敌方所有随从造成伤害，排除exceptID的实体
            foreach (Minion m in this.enemyMinions.ToArray())
            {
                if (m.entityID == exceptID) continue;
                minionGetDamageOrHeal(m, damages, true);

                // if (m.entityID != exceptID)
                // {
                //     minionGetDamageOrHeal(m, damages, true);
                // }
            }
        }

        /// <summary>
        /// 对一方的随机角色造成指定入参的伤害
        /// </summary>
        /// <param name="isEnemy">如果为 true，则对敌方角色造成伤害；否则对己方角色造成伤害。</param>
        /// <param name="damage">要造成的伤害量。</param>
        public void DealDamageToRandomCharacter(bool isEnemy, int damage)
        {
            List<Minion> possibleTargets = isEnemy ? this.enemyMinions : this.ownMinions;

            // 将英雄加入到可能的目标中
            if (isEnemy)
            {
                possibleTargets.Add(this.enemyHero);
            }
            else
            {
                possibleTargets.Add(this.ownHero);
            }

            // 从可能的目标中随机选择一个
            Minion target = possibleTargets[getRandomNumber(0, possibleTargets.Count - 1)];

            // 对选定的目标造成伤害
            this.minionGetDamageOrHeal(target, damage);
        }

        /// <summary>
        /// 所有随从受到随机伤害
        /// </summary>
        /// <param name="totalDamage"></param>
        public void allMinionsGetRandomDamage(int totalDamage)
        {
            Random rnd = new Random();  // 创建一个随机数生成器
            List<Minion> allMinions = new List<Minion>(this.ownMinions);
            allMinions.AddRange(this.enemyMinions);

            // 随机分配总伤害到所有随从身上
            while (totalDamage > 0 && allMinions.Count > 0)
            {
                // 随机选择一个随从
                Minion m = allMinions[rnd.Next(0, allMinions.Count)];

                // 计算本次伤害
                int dmg = rnd.Next(1, Math.Min(totalDamage, m.Hp) + 1);

                // 造成伤害
                this.minionGetDamageOrHeal(m, dmg);
                totalDamage -= dmg;

                // 如果随从死亡，从列表中移除
                if (m.Hp <= 0)
                {
                    allMinions.Remove(m);
                }
            }
        }
    }
}
