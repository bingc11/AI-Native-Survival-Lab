using System;

namespace P01.Core
{
    /// <summary>
    /// 一次导演决策的完整记录：结论 + 现场信息（结果本身可当日志用）。
    /// </summary>
    public sealed class DirectorResult
    {
        public DirectorOutcome Outcome { get; }

        /// <summary>验收通过的事件（仅 EventSelected 时有值）。</summary>
        public GameEvent? SelectedEvent { get; }

        /// <summary>导演给出的原始 ID（可能非法，留档用）。</summary>
        public string? ProposedId { get; }

        public string? ErrorMessage { get; }

        /// <summary>本次耗时（毫秒）。</summary>
        public long ElapsedMs { get; }

        public DirectorResult(
            DirectorOutcome outcome,
            GameEvent? selectedEvent = null,
            string? proposedId = null,
            string? errorMessage = null,
            long elapsedMs = 0)
        {
            Outcome = outcome;
            SelectedEvent = selectedEvent;
            ProposedId = proposedId;
            ErrorMessage = errorMessage;
            ElapsedMs = elapsedMs;
        }
    }
}
