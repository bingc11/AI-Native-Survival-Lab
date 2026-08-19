using System;

namespace P04.Core.State
{
    /// <summary>
    /// 世界状态：所有领域状态的集合（Single Source of Truth）。
    /// </summary>
    public sealed class WorldState
    {
        public TimeState Time { get; } = new TimeState();
        public PlayerState Player { get; } = new PlayerState();
        public EnvironmentState Environment { get; } = new EnvironmentState();
        public ResourceState Resources { get; } = new ResourceState();
        public EventLog EventLog { get; } = new EventLog();

        /// <summary>当前 tick 数（用于 replay）。</summary>
        public int TickCount { get; private set; } = 0;

        /// <summary>推进 1 tick（时间 +1 小时）。</summary>
        public void AdvanceTick()
        {
            Time.Advance();
            TickCount++;
        }
    }
}
