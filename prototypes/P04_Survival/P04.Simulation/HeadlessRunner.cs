using System;
using P04.Core.Clock;
using P04.Core.Event;
using P04.Core.System;
using P04.Core.State;

namespace P04.Simulation
{
    /// <summary>
    /// 无头运行器：跑 N tick，打印日志。
    /// </summary>
    public static class HeadlessRunner
    {
        public static void Run(int tickCount)
        {
            var world = new WorldState();
            var eventBus = new EventBus();
            var clock = new SimulationClock();

            // 注册系统
            clock.Register(new SurvivalSystem(eventBus));

            // 订阅事件（打印到控制台）
            eventBus.Subscribe(evt => Console.WriteLine($"  [Event] {evt}"));

            Console.WriteLine($"=== Headless Survival Simulation ({tickCount} ticks) ===\n");

            // 跑 N tick
            for (int i = 0; i < tickCount; i++)
            {
                Console.WriteLine($"Tick {i}: Day {world.Time.Day}, {world.Time.Hour}:00, " +
                    $"Health={world.Player.Health:F0}, Hunger={world.Player.Hunger:F0}, " +
                    $"Temp={world.Player.Temperature:F1}, Weather={world.Environment.Weather}");

                clock.Tick(world);

                if (!world.Player.IsAlive)
                {
                    Console.WriteLine($"\n[Game Over] Player died at tick {i}");
                    break;
                }
            }

            Console.WriteLine($"\n=== Simulation Complete ===");
            Console.WriteLine($"Total ticks: {world.TickCount}");
            Console.WriteLine($"Events logged: {world.EventLog.All.Count}");
        }
    }
}
