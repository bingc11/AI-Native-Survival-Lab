using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace P01.Core
{
    /// <summary>
    /// 规则导演：第一个 IEventDirector 实现（策略模式三兄弟之一）。
    /// 非 AI 基线——完全由程序员写死的规则表决定事件，100% 可预测。
    /// 游戏状态文本里出现哪个关键词，就选对应事件；都没出现 → 返回 null（无事发生）。
    /// </summary>
    public sealed class RuleBasedDirector : IEventDirector
    {
        /// <summary>一条规则：状态文本里包含 Keyword，就选择 EventId。</summary>
        public sealed class Rule
        {
            public string Keyword { get; }
            public string EventId { get; }

            public Rule(string keyword, string eventId)
            {
                Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
                EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
            }
        }

        private readonly IReadOnlyList<Rule> _rules;

        public RuleBasedDirector(IEnumerable<Rule> rules)
        {
            _rules = rules is null
                ? throw new ArgumentNullException(nameof(rules))
                : new List<Rule>(rules);
        }

        public Task<string?> ChooseEventAsync(string gameState, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (gameState != null)
            {
                foreach (Rule rule in _rules)
                {
                    if (gameState.Contains(rule.Keyword, StringComparison.Ordinal))
                    {
                        return Task.FromResult<string?>(rule.EventId);
                    }
                }
            }

            return Task.FromResult<string?>(null); // 无事发生
        }
    }
}
