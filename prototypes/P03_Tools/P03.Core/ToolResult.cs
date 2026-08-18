namespace P03.Core
{
    /// <summary>工具调用结果的状态分类（确定性边界的落地）。</summary>
    public enum ToolOutcome
    {
        /// <summary>成功执行，有输出。</summary>
        Success,
        /// <summary>工具不在白名单，被拒绝（AI 乱调）。</summary>
        Rejected,
        /// <summary>工具在白名单，但没有对应的执行实现。</summary>
        NoImplementation,
    }

    /// <summary>一次工具调用的结果。</summary>
    public sealed class ToolResult
    {
        public ToolOutcome Outcome { get; }
        public string Output { get; }
        public string? Error { get; }

        private ToolResult(ToolOutcome outcome, string output, string? error = null)
        {
            Outcome = outcome;
            Output = output;
            Error = error;
        }

        public static ToolResult Success(string output) => new ToolResult(ToolOutcome.Success, output);
        public static ToolResult Rejected(string error) => new ToolResult(ToolOutcome.Rejected, "", error);
        public static ToolResult NoImplementation(string error) => new ToolResult(ToolOutcome.NoImplementation, "", error);
    }
}
