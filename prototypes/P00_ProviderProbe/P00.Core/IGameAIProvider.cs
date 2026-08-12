using System.Threading;
using System.Threading.Tasks;

namespace P00.Core
{
    /// <summary>
    /// 游戏层与模型层的唯一边界（TASK-0002 核心接口）。
    /// 输入：查询文本；输出：补全后的纯文本。
    /// 结构化约束（grammar / JSON schema）由具体 Provider 负责，此接口不感知。
    /// 实现必须尊重 cancellationToken：超时/取消时抛 OperationCanceledException，
    /// 不得静默吞掉取消。
    /// </summary>
    public interface IGameAIProvider
    {
        Task<string> CompleteAsync(string query, CancellationToken cancellationToken);
    }
}
