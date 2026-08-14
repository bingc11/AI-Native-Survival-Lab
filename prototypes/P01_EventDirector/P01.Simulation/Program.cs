using P00.Core;
using P01.Core;

// ============================================================================
// P01 组装实战：14 天生存世界模拟
// 三种导演（规则/随机/LLM+假模型）换着用，游戏层代码一模一样。
// 最后打印收口人日志的分类统计 —— 看"确定性执行边界"的落地效果。
// ============================================================================

namespace P01.Simulation
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            // ---------- 事件表（围墙的"名单"） ----------
            var events = new[]
            {
                new GameEvent("wolf_pack", "狼群来袭", "玩家在森林区域"),
                new GameEvent("food_rot", "食物腐烂", "玩家有库存食物"),
                new GameEvent("abandoned_camp", "发现废弃营地", "玩家在野外"),
                new GameEvent("rainstorm", "暴雨", "玩家在露天"),
            };
            var registry = new EventRegistry(events);

            // ---------- 三种导演，组装方式各不相同 ----------
            var ruleBased = new RuleBasedDirector(new[]
            {
                new RuleBasedDirector.Rule("森林", "wolf_pack"),
                new RuleBasedDirector.Rule("野外", "abandoned_camp"),
            });
            var random = new RandomDirector(events.Select(e => e.Id), new Random(123));
            var llm = new LLMDirector(new StubProvider("{\"event\":\"wolf_pack\"}"), events);

            Console.WriteLine("========== P01 Event Director Simulation ==========\n");

            await RunSimulationAsync("RuleBased 规则导演", ruleBased, registry);
            await RunSimulationAsync("Random 随机导演", random, registry);
            await RunSimulationAsync("LLM 模型导演 (Stub 假模型)", llm, registry);

            Console.WriteLine("========== Done ==========");
            return 0;
        }

        /// <summary>用同一个游戏层代码，跑 14 天：换导演，调度逻辑一行不改。</summary>
        private static async Task RunSimulationAsync(string name, IEventDirector director, EventRegistry registry)
        {
            var scheduler = new EventScheduler(director, registry, TimeSpan.FromSeconds(3));

            // 简单世界状态
            string area = "森林";
            int health = 100;

            Console.WriteLine($"--- {name} ---");
            for (int day = 1; day <= 14; day++)
            {
                string time = day % 2 == 0 ? "夜晚" : "白天";
                var result = await scheduler.DecideAsync(area, health, time, CancellationToken.None);
                Console.WriteLine(FormatDay(day, result));

                if (result.Outcome == DirectorOutcome.EventSelected && result.SelectedEvent != null)
                {
                    ApplyEvent(result.SelectedEvent.Id, ref area, ref health);
                }
            }

            PrintStats(scheduler.Log);
            Console.WriteLine();
        }

        private static string FormatDay(int day, DirectorResult r) => r.Outcome switch
        {
            DirectorOutcome.EventSelected => $"Day {day,2}: EVENT   [{r.SelectedEvent!.Id}] {r.SelectedEvent.Title}",
            DirectorOutcome.NothingHappened => $"Day {day,2}: nothing happened",
            DirectorOutcome.InvalidId => $"Day {day,2}: INVALID id '{r.ProposedId}' (rejected)",
            DirectorOutcome.TimeoutOrCancelled => $"Day {day,2}: timeout/cancelled",
            _ => $"Day {day,2}: ERROR {r.ErrorMessage}",
        };

        /// <summary>事件对世界的（简化）影响。</summary>
        private static void ApplyEvent(string id, ref string area, ref int health)
        {
            switch (id)
            {
                case "wolf_pack": health -= 20; area = "森林"; break;
                case "food_rot": health -= 10; break;
                case "abandoned_camp": health += 10; area = "野外"; break;
                case "rainstorm": health -= 5; break;
            }
            if (health < 0) health = 0;
        }

        /// <summary>按 DirectorOutcome 分类统计日志（确定性执行边界的"证据"）。</summary>
        private static void PrintStats(IReadOnlyList<DirectorResult> log)
        {
            Console.WriteLine($"  [stats] total={log.Count}");
            foreach (var outcome in Enum.GetValues<DirectorOutcome>())
            {
                int count = log.Count(r => r.Outcome == outcome);
                Console.WriteLine($"    {outcome,-18}: {count}");
            }
        }
    }
}
