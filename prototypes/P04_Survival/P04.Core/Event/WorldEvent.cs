using System;

namespace P04.Core.Event
{
    /// <summary>
    /// 世界事件：重要状态变化的通知（供 AI/UI/音效响应）。
    /// </summary>
    public sealed class WorldEvent
    {
        public string Type { get; }       // 事件类型（如 "HypothermiaStarted"）
        public string Description { get; } // 描述
        public int Tick { get; }           // 发生时的 tick

        public WorldEvent(string type, string description, int tick)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Description = description ?? "";
            Tick = tick;
        }

        public override string ToString() => $"[Tick {Tick}] {Type}: {Description}";
    }
}
