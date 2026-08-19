using System;
using System.Collections.Generic;
using System.Linq;
using P04.Core.State;

namespace P04.Core.Clock
{
    /// <summary>
    /// 模拟时钟：固定步长推进，按 Interval/Phase 调度系统。
    /// </summary>
    public sealed class SimulationClock
    {
        private readonly List<ISystem> _systems = new List<ISystem>();

        /// <summary>注册系统。</summary>
        public void Register(ISystem system)
        {
            _systems.Add(system ?? throw new ArgumentNullException(nameof(system)));
        }

        /// <summary>推进 1 tick。</summary>
        public void Tick(WorldState world)
        {
            // 按 Phase 顺序执行（同一 Phase 内按注册顺序）
            foreach (Phase phase in Enum.GetValues(typeof(Phase)))
            {
                foreach (var system in _systems.Where(s => s.RunPhase == phase))
                {
                    // 检查 Interval：每 N tick 跑一次
                    if (world.TickCount % system.Interval == 0)
                    {
                        system.Update(world, world.TickCount);
                    }
                }
            }

            world.AdvanceTick();
        }

        /// <summary>跑 N tick。</summary>
        public void Run(WorldState world, int tickCount)
        {
            for (int i = 0; i < tickCount; i++)
            {
                Tick(world);
            }
        }
    }
}
