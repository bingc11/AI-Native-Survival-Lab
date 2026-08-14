using System;

namespace P02.Core
{
    /// <summary>
    /// 一条记忆：NPC 的一次经历。
    /// 三个字段对应 Generative Agents 记忆流的三要素：
    ///   Content    —— 发生了什么
    ///   Day        —— 什么时候（时间戳，简化用"第几天"）
    ///   Importance —— 多重要（0-10）
    /// </summary>
    public sealed class MemoryRecord
    {
        public string Content { get; }
        public int Day { get; }
        public float Importance { get; }

        public MemoryRecord(string content, int day, float importance)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Day = day;
            Importance = Math.Clamp(importance, 0f, 10f); // 重要性夹在 0-10
        }

        public override string ToString() => $"Day{Day}({Importance:0.#}): {Content}";
    }
}
