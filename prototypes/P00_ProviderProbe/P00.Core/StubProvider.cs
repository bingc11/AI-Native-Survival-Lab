using System;
using System.Threading;
using System.Threading.Tasks;

namespace P00.Core
{
    /// <summary>
    /// 非 AI 基线（CLAUDE.md：每个 AI 方案保留确定性执行边界，每个实验必须有非 AI 基线）。
    /// 固定输出、无随机性、无网络。用于对照真实 Provider 的行为，也是自动测试的锚点。
    /// </summary>
    public sealed class StubProvider : IGameAIProvider
    {
        private readonly string _response;

        public StubProvider(string response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public Task<string> CompleteAsync(string query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_response);
        }
    }
}
