using System.Collections.Concurrent;

namespace HREngine.Bots
{
    /// <summary>
    /// 对象池，用于复用 Playfield 实例，减少 AI 深度复制中的 GC 压力。
    /// 每次 AI 模拟搜索都会创建大量 Playfield 副本（maxwide=3000, maxdeep=12），
    /// 此池避免了重复分配 Playfield 对象及其内部的 List/Dictionary。
    /// </summary>
    public static class PlayfieldPool
    {
        private static readonly ConcurrentBag<Playfield> _pool = new ConcurrentBag<Playfield>();

        /// <summary>
        /// 从池中获取一个 Playfield 实例，并用 source 的数据填充。
        /// 如果池为空，则回退到 new Playfield(source)。
        /// </summary>
        public static Playfield Rent(Playfield source)
        {
            if (_pool.TryTake(out var pf))
            {
                pf.CopyFrom(source);
                return pf;
            }
            return new Playfield(source);
        }

        /// <summary>
        /// 将使用完毕的 Playfield 实例归还池中。
        /// 归还前会自动调用 ClearForPool() 释放对象引用，避免内存泄漏。
        /// </summary>
        public static void Return(Playfield pf)
        {
            if (pf == null) return;
            pf.ClearForPool();
            _pool.Add(pf);
        }

        /// <summary>
        /// 预热对象池，创建指定数量的 Playfield 实例，避免 AI 搜索初期触发大量 new 分配。
        /// </summary>
        /// <param name="count">预热实例数量</param>
        public static void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var pf = new Playfield();
                pf.ClearForPool();
                _pool.Add(pf);
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
