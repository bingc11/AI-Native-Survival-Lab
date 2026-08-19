using P04.Core.State;

namespace P04.Core.Clock
{
    /// <summary>
    /// 系统接口：每个系统声明自己的 Interval（多久跑一次）和 Phase（在哪个阶段跑）。
    /// </summary>
    public interface ISystem
    {
        /// <summary>每隔几个 tick 运行一次（1=每 tick，6=每 6 tick）。</summary>
        int Interval { get; }

        /// <summary>在哪个 Phase 运行。</summary>
        Phase RunPhase { get; }

        /// <summary>运行系统（读/改 WorldState）。</summary>
        void Update(WorldState world, int currentTick);
    }
}
