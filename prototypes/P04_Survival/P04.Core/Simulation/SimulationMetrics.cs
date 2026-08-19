using System.Collections.Generic;

namespace P04.Core.Simulation
{
    /// <summary>
    /// 模拟指标：Headless Evaluation 的度量结果。
    /// 用于比较不同策略（Baseline / Ablation），不依赖"看起来更聪明"。
    /// </summary>
    public sealed class SimulationMetrics
    {
        private readonly Dictionary<string, int> _actionCounts = new Dictionary<string, int>();

        /// <summary>模拟存活到的 tick 数。</summary>
        public int SurvivedTicks { get; set; }

        /// <summary>被拒绝的非法命令数。</summary>
        public int InvalidActions { get; private set; }

        /// <summary>触发的世界事件数。</summary>
        public int EventsCount { get; private set; }

        public IReadOnlyDictionary<string, int> ActionCounts => _actionCounts;

        public void RecordAction(string action)
        {
            _actionCounts.TryGetValue(action, out int count);
            _actionCounts[action] = count + 1;
        }

        public void RecordInvalidAction()
        {
            InvalidActions++;
        }

        public void RecordEvent()
        {
            EventsCount++;
        }

        /// <summary>非法命令占比（0~1）。</summary>
        public float InvalidActionRate => _actionCounts.Count == 0 && InvalidActions == 0 ? 0f : InvalidActions / (float)(InvalidActions + TotalActions);

        /// <summary>总成功命令数。</summary>
        public int TotalActions
        {
            get
            {
                int total = 0;
                foreach (var kv in _actionCounts) total += kv.Value;
                return total;
            }
        }

        public override string ToString()
        {
            return $"SurvivedTicks={SurvivedTicks}, Actions={TotalActions}, Invalid={InvalidActions} (rate={InvalidActionRate:P0}), Events={EventsCount}";
        }
    }
}
