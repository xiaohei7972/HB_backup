using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HREngine.Bots
{
    /// <summary>
    /// Thread-safe object pool for List&lt;Action&gt; used during AI move generation.
    /// Eliminates GC pressure from thousands of allocations per search depth level.
    /// </summary>
    public static class ActionListPool
    {
        private static readonly ConcurrentBag<List<Action>> _pool = new ConcurrentBag<List<Action>>();

        /// <summary>
        /// Rent a pooled list or create a new one with default capacity (50).
        /// </summary>
        public static List<Action> Rent()
        {
            if (_pool.TryTake(out var list))
                return list;
            return new List<Action>(50);
        }

        /// <summary>
        /// Return a list to the pool after clearing it.
        /// Call this after the foreach loop over the actions list completes.
        /// </summary>
        public static void Return(List<Action> list)
        {
            if (list == null) return;
            list.Clear();
            _pool.Add(list);
        }
    }
}
