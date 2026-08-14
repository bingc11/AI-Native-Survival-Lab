using System;
using System.Collections.Generic;
using System.Linq;

namespace P02.Core
{
    /// <summary>
    /// 记忆检索（Retrieval）：做决定时，从记忆流里捞出"相关的"记忆。
    /// 分数 = 相关性 × 时间衰减 × 重要性（Generative Agents 的三因子简化版）：
    ///   相关性 —— 查询的关键词命中几条内容
    ///   时间衰减 —— 越久远越小（1 / (1 + 天数差)）
    ///   重要性 —— 越重要越容易被想起
    /// 按分数降序取 TopK。
    /// </summary>
    public sealed class MemoryRetrieval
    {
        private readonly int _topK;

        public MemoryRetrieval(int topK = 5)
        {
            _topK = Math.Max(1, topK);
        }

        public IReadOnlyList<MemoryRecord> Retrieve(MemoryBank stream, string query, int currentDay)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrWhiteSpace(query)) return Array.Empty<MemoryRecord>();

            return stream.All
                .Select(m => new { Memory = m, Score = Score(m, query, currentDay) })
                .Where(x => x.Score > 0f)
                .OrderByDescending(x => x.Score)
                .Take(_topK)
                .Select(x => x.Memory)
                .ToList();
        }

        /// <summary>单个记忆的检索分数。</summary>
        public float Score(MemoryRecord m, string query, int currentDay)
        {
            float relevance = Relevance(m.Content, query);      // 0..N：命中关键词数
            float recency = 1f / (1f + Math.Max(0, currentDay - m.Day)); // (0,1]：越久越小
            return relevance * recency * m.Importance;
        }

        private static float Relevance(string content, string query)
        {
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int hits = words.Count(w => content.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            return hits;
        }
    }
}
