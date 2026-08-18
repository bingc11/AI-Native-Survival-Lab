using System;

namespace P03.Core
{
    /// <summary>
    /// 一个"AI 能调用的工具"的定义（白名单条目）。
    /// 只描述"有什么工具"，不含执行逻辑（执行是 ToolExecutor 的事）。
    /// </summary>
    public sealed class AITool
    {
        public string Name { get; }
        public string Description { get; }

        public AITool(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public override string ToString() => $"{Name}: {Description}";
    }
}
