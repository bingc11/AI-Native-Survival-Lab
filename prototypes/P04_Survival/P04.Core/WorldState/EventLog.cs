using System;
using System.Collections.Generic;

namespace P04.Core.State
{
    /// <summary>
    /// 事件日志：记录已发生的世界事件（供 replay/debug）。
    /// </summary>
    public sealed class EventLog
    {
        private readonly List<string> _events = new List<string>();

        public IReadOnlyList<string> All => _events;

        public void Record(string eventDescription)
        {
            _events.Add(eventDescription);
        }

        public void Clear()
        {
            _events.Clear();
        }
    }
}
