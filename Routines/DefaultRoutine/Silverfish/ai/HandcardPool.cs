using System.Collections.Concurrent;

namespace HREngine.Bots
{
    /// <summary>
    /// 对象池，用于复用 Handcard 实例，减少 AI 深度复制中的 GC 压力。
    /// addCardsReal() 每次创建新的 Handcard，通过此池可复用已分配的对象。
    /// </summary>
    public static class HandcardPool
    {
        private static readonly ConcurrentBag<Handmanager.Handcard> _pool = new ConcurrentBag<Handmanager.Handcard>();

        /// <summary>
        /// 从池中获取一个 Handcard 实例，并用 source 的数据填充。
        /// 如果池为空，则回退到 new Handcard(source)。
        /// </summary>
        public static Handmanager.Handcard Rent(Handmanager.Handcard source)
        {
            if (_pool.TryTake(out var hc))
            {
                hc.CopyFrom(source);
                return hc;
            }
            return new Handmanager.Handcard(source);
        }

        /// <summary>
        /// 将使用完毕的 Handcard 实例归还池中。
        /// 归还前会自动调用 ClearForPool() 释放对象引用。
        /// </summary>
        public static void Return(Handmanager.Handcard hc)
        {
            if (hc == null) return;
            hc.ClearForPool();
            _pool.Add(hc);
        }

        /// <summary>
        /// 预热对象池，创建指定数量的 Handcard 实例。
        /// </summary>
        /// <param name="count">预热实例数量</param>
        public static void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var hc = new Handmanager.Handcard();
                hc.ClearForPool();
                _pool.Add(hc);
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
