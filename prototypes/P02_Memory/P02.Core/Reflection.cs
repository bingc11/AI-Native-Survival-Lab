using System.Collections.Generic;
using System.Linq;

namespace P02.Core
{
    /// <summary>
    /// 反思（Reflection）：把零散记忆提炼成高层洞察。
    /// Generative Agents 里是用 LLM 做总结；这里用规则做简化版：
    ///   给定一个主题，数一数相关记忆有多少条、最近一次是什么，
    ///   合成一句"洞察"。体现"经历 → 认知"的提炼过程。
    /// </summary>
    public static class Reflection
    {
        /// <summary>提炼主题相关的记忆为一句洞察。</summary>
        public static string Reflect(IReadOnlyList<MemoryRecord> related, string topic)
        {
            if (related is null || related.Count == 0)
            {
                return $"关于「{topic}」：暂无相关记忆";
            }

            int count = related.Count;
            string latest = related[0].Content; // 已按分数排序，第一条 = 最相关
            return $"关于「{topic}」：发生过 {count} 件相关的事（最相关：{latest}）";
        }

        /// <summary>统计所有记忆里的高频词（top n），供"看这个 NPC 最近在想什么"用。</summary>
        public static IReadOnlyList<string> TopTopics(MemoryBank stream, int topN = 3)
        {
            var words = stream.All
                .SelectMany(m => m.Content.Split(' '))
                .Where(w => w.Length > 1)
                .GroupBy(w => w, System.StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(topN)
                .ToList();
            return words;
        }
    }
}
