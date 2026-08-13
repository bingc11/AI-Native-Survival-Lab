using System;

namespace P01.Core
{
    /// <summary>
    /// 一个"合法事件"的定义（一份数据，不含行为）。
    /// 事件 = 游戏中可以发生的一件事，由游戏层负责执行。
    /// 导演（Director）只能从事件表里挑事件，不能自己发明。
    /// </summary>
    public sealed class GameEvent
    {
        /// <summary>唯一标识，如 "wolf_pack"。查表时用这个。</summary>
        public string Id { get; }

        /// <summary>给人看的标题，如 "狼群来袭"。</summary>
        public string Title { get; }

        /// <summary>前提描述（给 LLM 参考用），可空。</summary>
        public string? Requirement { get; }

        public GameEvent(string id, string title, string? requirement = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Requirement = requirement;
        }
    }
}
