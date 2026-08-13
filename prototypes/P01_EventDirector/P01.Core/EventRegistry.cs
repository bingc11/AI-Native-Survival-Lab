using System;
using System.Collections.Generic;
using System.Linq;

namespace P01.Core
{
    /// <summary>
    /// 合法事件的唯一来源（"围墙"）。
    /// 游戏启动时用写死的事件列表构造它；之后导演给的任何事件 ID，
    /// 都要用它来验收（Contains / Get），不在墙内的 ID 一律拒绝。
    /// </summary>
    public sealed class EventRegistry
    {
        private readonly Dictionary<string, GameEvent> _byId;

        public EventRegistry(IEnumerable<GameEvent> events)
        {
            _byId = (events ?? throw new ArgumentNullException(nameof(events)))
                .ToDictionary(e => e.Id, e => e); // 重复 ID 会抛异常，正好防呆
        }

        /// <summary>全部已注册事件。</summary>
        public IReadOnlyList<GameEvent> All => _byId.Values.ToList();

        /// <summary>这个 ID 在墙内吗？（验收的闸门）</summary>
        public bool Contains(string id) => id != null && _byId.ContainsKey(id);

        /// <summary>按 ID 取事件；不在墙内返回 null。</summary>
        public GameEvent? Get(string id)
            => id != null && _byId.TryGetValue(id, out GameEvent e) ? e : null;
    }
}
