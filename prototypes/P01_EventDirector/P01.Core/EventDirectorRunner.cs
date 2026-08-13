using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace P01.Core
{
    /// <summary>
    /// 收口人（P01 收官角色）：把 导演(生产) + 围墙(验收) + 日志(留档) 串成一条线。
    ///
    /// 流程：
    ///   1. 调注入的导演拿候选 ID（三兄弟换着用，这里不用改）
    ///   2. 验收：null → 无事发生；ID 不在围墙里 → 非法；在 → 执行
    ///   3. 超时/取消/异常 → 分类记录日志，一律不执行
    ///
    /// 这就是"确定性执行边界"的收口：不管导演是谁、模型多疯，
    /// 游戏层只会收到一个归好类的 DirectorResult，绝无意外。
    /// </summary>
    public sealed class EventDirectorRunner
    {
        private readonly IEventDirector _director;
        private readonly EventRegistry _registry;
        private readonly TimeSpan _timeout;
        private readonly List<DirectorResult> _log = new List<DirectorResult>();

        public EventDirectorRunner(IEventDirector director, EventRegistry registry, TimeSpan? timeout = null)
        {
            _director = director ?? throw new ArgumentNullException(nameof(director));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _timeout = timeout ?? TimeSpan.FromSeconds(5);
        }

        /// <summary>运行历史（每次 RunAsync 追加一条）。</summary>
        public IReadOnlyList<DirectorResult> Log => _log;

        public async Task<DirectorResult> RunAsync(string gameState, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                // 外部取消 + 超时，二合一的令牌
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_timeout);

                string? proposed = await _director.ChooseEventAsync(gameState, cts.Token);
                sw.Stop();

                if (proposed == null)
                {
                    return LogResult(new DirectorResult(DirectorOutcome.NothingHappened, elapsedMs: sw.ElapsedMilliseconds));
                }

                GameEvent? selected = _registry.Get(proposed); // 验收闸门
                if (selected == null)
                {
                    return LogResult(new DirectorResult(DirectorOutcome.InvalidId,
                        proposedId: proposed, elapsedMs: sw.ElapsedMilliseconds));
                }

                return LogResult(new DirectorResult(DirectorOutcome.EventSelected,
                    selectedEvent: selected, proposedId: proposed, elapsedMs: sw.ElapsedMilliseconds));
            }
            catch (OperationCanceledException)
            {
                sw.Stop();
                return LogResult(new DirectorResult(DirectorOutcome.TimeoutOrCancelled, elapsedMs: sw.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                sw.Stop();
                return LogResult(new DirectorResult(DirectorOutcome.Error,
                    errorMessage: ex.Message, elapsedMs: sw.ElapsedMilliseconds));
            }
        }

        private DirectorResult LogResult(DirectorResult result)
        {
            _log.Add(result);
            return result;
        }
    }
}
