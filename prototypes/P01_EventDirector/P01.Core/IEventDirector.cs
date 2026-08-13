using System.Threading;
using System.Threading.Tasks;

namespace P01.Core
{
    /// <summary>
    /// 导演合同（P00 接口思想的第二次应用）。
    /// 输入：当前游戏状态的描述文本；输出：选中的事件 ID。
    ///
    /// 注意（确定性执行边界）：
    /// 接口只保证返回一个字符串，不保证它是合法事件 ID——
    /// 规则/随机导演天然合法，但 LLM 导演可能乱编。
    /// 合法性必须由调用方用 EventRegistry.Contains(id) 验收后再执行。
    /// </summary>
    public interface IEventDirector
    {
        /// <summary>
        /// 根据游戏状态选一个事件 ID（异步：LLM 导演需要等待模型）。
        /// </summary>
        Task<string> ChooseEventAsync(string gameState, CancellationToken cancellationToken);
    }
}
