// ============================================================================
// LLMUnityAdapter.cs —— 代码形态（PROOF-OF-CONCEPT）
// ============================================================================
// 本机无 Unity 编辑器，此文件【不加入 dotnet 编译】（TASK-0002 允许范围）。
// 用途：演示如何把 P00 的 IGameAIProvider 边界接到 LLMUnity.LLMAgent。
// 放入 Unity 工程方法：把此文件加入引用了 LLMUnity 包 + P00.Core（netstandard2.1）的 asmdef。
// 依据：docs/source-studies/llmunity/READING_GUIDE.md 第 5 节接口契约。
//   - 隔离边界：游戏层只见 P00.Core.IGameAIProvider，不依赖 LLMUnity 类型。
//   - 结构化输出：构造时设置 grammar（GBNF / JSON schema），输出为文本 JSON；
//     外层 P00.Core.JsonOutputValidator 负责解析 / 校验 / 有限重试 / 兜底。
// ============================================================================

using System;
using System.Threading;
using System.Threading.Tasks;
using LLMUnity;

namespace P00.UnityAdapter
{
    /// <summary>
    /// 把 LLMUnity.LLMAgent 包装成 IGameAIProvider。
    /// 取消语义：等待模型输出期间若取消，调用 LLMAgent.CancelRequests() 终止在途请求，
    /// 并抛 OperationCanceledException，保证不阻塞调用线程。
    /// </summary>
    public sealed class LLMUnityAdapter : P00.Core.IGameAIProvider
    {
        private readonly LLMAgent _agent;

        public LLMUnityAdapter(LLMAgent agent, string grammar = null, string systemPrompt = null)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            if (systemPrompt != null) _agent.systemPrompt = systemPrompt;
            if (grammar != null) _agent.grammar = grammar; // 采样期约束，见 READING_GUIDE 第 2 节
        }

        public async Task<string> CompleteAsync(string query, CancellationToken cancellationToken)
        {
            Task<string> chatTask = _agent.Chat(query, addToHistory: false);

            var cancelTcs = new TaskCompletionSource<object>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            using (cancellationToken.Register(() =>
            {
                _agent.CancelRequests(); // 终止原生在途请求
                cancelTcs.TrySetResult(null);
            }))
            {
                Task winner = await Task.WhenAny(chatTask, cancelTcs.Task);
                if (winner == chatTask)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await chatTask; // 正常完成；若 Chat 抛异常则在此传播
                }
            }

            throw new OperationCanceledException(cancellationToken);
        }
    }
}
