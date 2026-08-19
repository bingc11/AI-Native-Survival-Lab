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
    /// withStrategy=true 时提交"求生策略"命令；false 时什么都不做（Baseline 对照组）。
    /// </summary>
    public static class HeadlessRunner
    {
        public static void Run(int tickCount, bool withStrategy = true, bool compact = false)
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

            // 事件 → 指标 + （非 compact 时）打印
            eventBus.Subscribe(evt =>
            {
                metrics.RecordEvent();
                if (!compact) Console.WriteLine($"  [Event] {evt}");
            });

            string title = withStrategy ? "=== 有策略（定时吃 + 换地点）===" : "=== 无策略（Baseline：什么都不做）===";
            Console.WriteLine($"\n{title} ({tickCount} ticks, seed=42)\n");

            // 每 tick：按策略提交命令 + 推进
            for (int tick = 0; tick < tickCount; tick++)
            {
                // 精简模式每 5 tick 打一行，完整模式每 tick 打一行
                if (!compact || tick % 5 == 0)
                {
                    Console.WriteLine($"Tick {tick}: Day {world.Time.Day}, {world.Time.Hour}:00, " +
                        $"Location={world.Player.Location}, Health={world.Player.Health:F0}, " +
                        $"Hunger={world.Player.Hunger:F0}, Temp={world.Player.Temperature:F1}");
                }

                if (withStrategy)
                {
                    // 求生策略：每 8 小时吃一次；第 12 tick 换到河边
                    if (tick % 8 == 0)
                    {
                        actions.Submit(new Command("Eat", "Food", 1f));
                    }
                    if (tick == 12)
                    {
                        actions.Submit(new Command("Move", "River"));
                    }
                }

                clock.Tick(world);

                if (!world.Player.IsAlive)
                {
                    Console.WriteLine($"\n[Game Over] Player died at tick {tick} (Hunger={world.Player.Hunger:F0})");
                    break;
                }
            }

            // 结果指标
            metrics.SurvivedTicks = world.TickCount;
            Console.WriteLine($"Metrics: {metrics}");
        }
    }
}