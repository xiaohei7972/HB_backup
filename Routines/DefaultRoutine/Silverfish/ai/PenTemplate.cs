
namespace HREngine.Bots
{
    /// <summary>
    /// 惩罚模板类
    /// 为攻击和出牌决策提供惩罚值计算的基础模板
    /// </summary>
    internal class PenTemplate
    {
        /// <summary>
        /// 敌方随从攻击力因子
        /// </summary>
        private int enemyMinionAttackFactor = 2;

        /// <summary>
        /// 敌方随从基础价值
        /// </summary>
        private int enemyMinionBaseValue = 10;

        /// <summary>
        /// 敌方随从生命值因子
        /// </summary>
        private int enemyMinionHPFactor = 2;

        /// <summary>
        /// 获取攻击惩罚值
        /// </summary>
        /// <param name="p">游戏状态</param>
        /// <param name="target">攻击目标</param>
        /// <param name="isLethal">是否为斩杀</param>
        /// <returns>攻击惩罚值</returns>
        public virtual int getAttackPenalty(Playfield p, Minion target, bool isLethal)
        {
            return 0;
        }

        /// <summary>
        /// 获取出牌惩罚值
        /// </summary>
        /// <param name="p">游戏状态</param>
        /// <param name="m">要出的随从</param>
        /// <param name="target">目标</param>
        /// <param name="choice">选择</param>
        /// <param name="isLethal">是否为斩杀</param>
        /// <returns>出牌惩罚值</returns>
        public virtual int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            return 0;
        }

        /// <summary>
        /// 获取随从的价值
        /// </summary>
        /// <param name="Angr">攻击力</param>
        /// <param name="HP">生命值</param>
        /// <param name="isTaunt">是否为嘲讽</param>
        /// <returns>随从的价值</returns>
        public int getValueOfMinion(int Angr, int HP, bool isTaunt = false)
        {
            return this.enemyMinionBaseValue + this.enemyMinionAttackFactor * Angr + this.enemyMinionHPFactor * HP;
        }


    }
}