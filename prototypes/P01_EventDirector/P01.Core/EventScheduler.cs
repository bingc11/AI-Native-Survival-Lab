using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace P01.Core
{
    /// <summary>
    /// 游戏层事件调度器（P01 组装实战的"组装点"）。
    /// 职责：
    ///   1. 组装：收下导演 + 围墙，内部造好收口人 Runner（换导演 = 换构造参数，游戏层其余代码不动）
    ///   2. 翻译：游戏层每 Tick 调用 DecideAsync，把"世界数据"翻译成给导演/模型的文本
    ///   3. 收口：把结果（DirectorResult）交还游戏层，游戏层只认五类结论
    /// </summary>
    public sealed class EventScheduler
    {
        private readonly EventDirectorRunner _runner;

        public EventScheduler(IEventDirector director, EventRegistry registry, TimeSpan? timeout = null)
        {
            _runner = new EventDirectorRunner(director, registry, timeout); // 组装收口人
        }

        /// <summary>运行历史（透传收口人的日志）。</summary>
        public IReadOnlyList<DirectorResult> Log => _runner.Log;

        /// <summary>
        /// 每 Tick 调度一次：把世界数据翻译成文本，交给收口人。
        /// </summary>
        public async Task<DirectorResult> DecideAsync(
            string area, int health, string timeOfDay, CancellationToken cancellationToken)
        {
            string gameState = $"玩家在{area}，血量{health}，时间是{timeOfDay}"; // 游戏层翻译世界
            return await _runner.RunAsync(gameState, cancellationToken);
        }
    }
}
