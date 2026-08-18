using System;
using System.Collections.Generic;

namespace P03.Core
{
    /// <summary>
    /// 工具执行器（收口人）：把"工具调用请求"变成"真的做事"。
    /// 流程：
    ///   ① 校验：工具在不在白名单（ToolRegistry）？不在 → 拒绝（AI 乱调）
    ///   ② 校验：有没有对应的执行实现？没有 → 失败
    ///   ③ 执行：调游戏侧注册的 handler，返回结果
    ///
    /// 关键：AI 只能调白名单里的工具（确定性边界，和 P01 事件表同款）。
    /// 真正"做事"的代码由游戏侧通过 Register 注入，本类不关心具体逻辑。
    /// </summary>
    public sealed class ToolExecutor
    {
        private readonly ToolRegistry _registry;
        private readonly Dictionary<string, Func<string, string>> _handlers
            = new Dictionary<string, Func<string, string>>();

        public ToolExecutor(ToolRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <summary>把工具名绑定到一个实际执行函数（游戏侧注入真实能力）。</summary>
        public void Register(string toolName, Func<string, string> handler)
        {
            if (handler is null) throw new ArgumentNullException(nameof(handler));
            if (!_registry.Contains(toolName))
                throw new ArgumentException($"工具 '{toolName}' 不在白名单，不能注册执行器");
            _handlers[toolName] = handler;
        }

        public ToolResult Execute(ToolCall call)
        {
            if (call is null) throw new ArgumentNullException(nameof(call));

            // ① 白名单校验
            if (!_registry.Contains(call.ToolName))
            {
                return ToolResult.Rejected($"工具 '{call.ToolName}' 不在白名单，已拒绝");
            }

            // ② 有没有实现
            if (!_handlers.TryGetValue(call.ToolName, out Func<string, string> handler))
            {
                return ToolResult.NoImplementation($"工具 '{call.ToolName}' 已注册但未实现");
            }

            // ③ 执行
            try
            {
                string output = handler(call.ArgumentsJson);
                return ToolResult.Success(output);
            }
            catch (Exception ex)
            {
                return ToolResult.NoImplementation($"工具 '{call.ToolName}' 执行出错: {ex.Message}");
            }
        }
    }
}
