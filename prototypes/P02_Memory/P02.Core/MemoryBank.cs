using System;
using System.Collections.Generic;

namespace P02.Core
{
    /// <summary>
    /// 记忆流（Memory Stream）：所有经历按时间顺序存放的"记忆库"。
    /// 只负责"存"和"读"，不负责"选"（选是 MemoryRetrieval 的事）。
    /// 封装：写走 Add（私有 List），读走 All（只读接口）。
    /// </summary>
    public sealed class MemoryBank
    {
        private readonly List<MemoryRecord> _records = new List<MemoryRecord>();

        /// <summary>全部记忆（按添加顺序 = 时间顺序）。</summary>
        public IReadOnlyList<MemoryRecord> All => _records;

        public int Count => _records.Count;

        /// <summary>经历一件事，记进记忆流。</summary>
        public void Add(string content, int day, float importance)
        {
            Add(new MemoryRecord(content, day, importance));
        }

        public void Add(MemoryRecord record)
        {
            _records.Add(record ?? throw new ArgumentNullException(nameof(record)));
        }
    }
}
