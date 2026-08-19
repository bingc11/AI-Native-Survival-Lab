using System;
using P04.Core.Clock;
using P04.Core.Commands;
using P04.Core.Event;
using P04.Core.Simulation;
using P04.Core.State;
using P04.Core.Systems;

namespace P04.Simulation
{
    /// <summary>
    /// 无头运行器：跑 N tick，提交命令，打印状态/事件/指标。
    /// </summary>
    public static class HeadlessRunner
    {
        public static void Run(int tickCount)
        {
            var world = new WorldState();
            var eventBus = new EventBus();
            var clock = new SimulationClock();
            var metrics = new SimulationMetrics();
            var replay = new ReplayRecorder(seed: 42);

            // 注册系统
            var actions = new ActionSystem(eventBus, metrics, replay);
            clock.Register(actions);
            clock.Register(new SurvivalSystem(eventBus));

            // 事件 → 指标 + 打印
            eventBus.Subscribe(evt =>
            {
                metrics.RecordEvent();
                Console.WriteLine($"  [Event] {evt}");
            });

            Console.WriteLine($"=== Headless Survival Simulation ({tickCount} ticks) ===\n");

            // 每 tick：提交命令（模拟一个简单的"求生策略"）+ 推进
            for (int tick = 0; tick < tickCount; tick++)
            {
                Console.WriteLine($"Tick {tick}: Day {world.Time.Day}, {world.Time.Hour}:00, " +
                    $"Location={world.Player.Location}, Health={world.Player.Health:F0}, " +
                    $"Hunger={world.Player.Hunger:F0}, Temp={world.Player.Temperature:F1}");

                // 求生策略：每 8 小时吃一次（Forest 有 10 食物，够撑）；每 12 小时换个地点
                if (tick % 8 == 0)
                {
                    actions.Submit(new Command("Eat", "Food", 1f));
                }
                if (tick == 12)
                {
                    actions.Submit(new Command("Move", "River"));
                }
                // 故意提交一条非法命令：移动到不存在的地点
                if (tick == 16)
                {
                    actions.Submit(new Command("Move", "Moon"));
                }

                clock.Tick(world);

                if (!world.Player.IsAlive)
                {
                    Console.WriteLine($"\n[Game Over] Player died at tick {tick}");
                    break;
                }
            }

            // 结果指标
            metrics.SurvivedTicks = world.TickCount;
            Console.WriteLine($"\n=== Simulation Complete ===");
            Console.WriteLine($"Metrics: {metrics}");
            Console.WriteLine("\n=== Replay 记录（前 6 条）===");
            int shown = 0;
            foreach (var r in replay.Commands)
            {
                if (shown++ >= 6) break;
                Console.WriteLine($"  tick {r.Tick} {r.Action}({r.Target},{r.Amount}) => {(r.Accepted ? "accepted" : "rejected")}");
            }
        }
    }
}
