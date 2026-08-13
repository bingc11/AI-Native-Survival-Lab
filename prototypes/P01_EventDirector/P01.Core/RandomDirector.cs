using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace P01.Core
{
    /// <summary>
    /// 随机导演：策略三兄弟之二。从候选事件里随机抽一个。
    /// 与规则导演一样是"对照基线"——用来观察"无脑随机"和"规则/AI"的差别。
    /// 同样不管合法性：只给候选，验收是调用方的事。
    /// </summary>
    public sealed class RandomDirector : IEventDirector
    {
        private readonly IReadOnlyList<string> _candidateIds;
        private readonly Random _random;

        public RandomDirector(IEnumerable<string> candidateIds, Random? random = null)
        {
            _candidateIds = candidateIds is null
                ? throw new ArgumentNullException(nameof(candidateIds))
                : new List<string>(candidateIds);
            _random = random ?? new Random();
        }

        public Task<string?> ChooseEventAsync(string gameState, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_candidateIds.Count == 0) return Task.FromResult<string?>(null); // 没候选 → 无事发生

            int index = _random.Next(_candidateIds.Count);                        // 随机抽一个下标
            return Task.FromResult<string?>(_candidateIds[index]);
        }
    }
}
