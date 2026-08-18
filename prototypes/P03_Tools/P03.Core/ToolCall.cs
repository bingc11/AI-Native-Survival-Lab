using System;

namespace P03.Core
{
    /// <summary>
    /// 一次"工具调用请求"（模型想做的事）。
    /// 模型不直接执行，而是发一个请求：要调哪个工具 + 参数。
    /// 是否执行、怎么执行，由 ToolExecutor 决定。
    /// </summary>
    public sealed class ToolCall
    {
        public string ToolName { get; }
        public string ArgumentsJson { get; }

        public ToolCall(string toolName, string argumentsJson = "{}")
        {
            ToolName = toolName ?? throw new ArgumentNullException(nameof(toolName));
            ArgumentsJson = argumentsJson ?? throw new ArgumentNullException(nameof(argumentsJson));
        }

        public override string ToString() => $"ToolCall({ToolName}, args={ArgumentsJson})";
    }
}
