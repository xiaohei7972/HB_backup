using System.Collections.Concurrent;

namespace HREngine.Bots
{
    /// <summary>
    /// 对象池，用于复用 Minion 实例，减少 AI 深度复制中的 GC 压力。
    /// CopyFrom() 每次调用 addMinionsReal 都会 new Minion(m)，
    /// 通过此池可复用已分配的 Minion 对象。
    /// </summary>
    public static class MinionPool
    {
        private static readonly ConcurrentBag<Minion> _pool = new ConcurrentBag<Minion>();

        /// <summary>
        /// 从池中获取一个 Minion 实例，并用 source 的数据填充。
        /// 如果池为空，则回退到 new Minion(source)。
        /// </summary>
        public static Minion Rent(Minion source)
        {
            if (_pool.TryTake(out var m))
            {
                m.CopyFrom(source);
                return m;
            }
            return new Minion(source);
        }

        /// <summary>
        /// 将使用完毕的 Minion 实例归还池中。
        /// 归还前会自动调用 ClearForPool() 释放对象引用。
        /// </summary>
        public static void Return(Minion m)
        {
            if (m == null) return;
            m.ClearForPool();
            _pool.Add(m);
        }

        /// <summary>
        /// 预热对象池，创建指定数量的 Minion 实例。
        /// </summary>
        /// <param name="count">预热实例数量</param>
        public static void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var m = new Minion();
                m.ClearForPool();
                _pool.Add(m);
            }
        }

        /// <summary>
        /// 清空对象池（用于测试或重置）。
        /// </summary>
        public static void Clear()
        {
            while (_pool.TryTake(out _)) { }
        }
    }
}
