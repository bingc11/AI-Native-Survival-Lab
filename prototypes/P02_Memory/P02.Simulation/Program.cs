using System.Threading;
using P00.Core;
using P01.Core;
using P02.Core;

namespace P02.Simulation
{
    public static class Program
    {
        public static async Task<int> Main()
        {
            // ---------- 记忆流：幸存者 5 天的经历（经历 → 记忆） ----------
            var memory = new MemoryBank();
            memory.Add("森林里的狼群袭击了我", day: 1, importance: 8);
            memory.Add("我捡到一把斧头", day: 2, importance: 6);
            memory.Add("村子居民很友善，给了我食物", day: 3, importance: 4);
            memory.Add("河边鱼很多，但水位上涨", day: 4, importance: 5);
            memory.Add("森林边缘又听到狼嚎", day: 5, importance: 7);

            var retrieval = new MemoryRetrieval(topK: 3);

            // ---------- 第 6 天，玩家问："森林安全吗？" ----------
            Console.WriteLine("=== 第 6 天，玩家问：'森林安全吗？' ===\n");

            string baseState = "玩家在森林边缘，血量70，时间是白天";

            Console.WriteLine("[无记忆] 喂给导演的状态：");
            Console.WriteLine("  " + baseState);
            Console.WriteLine();

            // 检索相关记忆，拼进状态（记忆 → 决策）
            var related = retrieval.Retrieve(memory, "森林 狼群 安全", currentDay: 6);
            string enriched = baseState
                + "\n相关记忆：" + string.Join("；", related.Select(r => $"Day{r.Day}: {r.Content}"));

            Console.WriteLine("[有记忆] 喂给导演的状态：");
            Console.WriteLine("  " + enriched);
            Console.WriteLine();

            // ---------- 反思 ----------
            Console.WriteLine("=== 反思（经历 → 认知） ===");
            Console.WriteLine("  " + Reflection.Reflect(related, "森林/狼群"));
            Console.WriteLine();

            // ---------- 交给 P01 导演决策（记忆版状态） ----------
            Console.WriteLine("=== 交给 P01 导演（LLM + Stub 假模型） ===\n");
            var events = new[]
            {
                new GameEvent("flee", "撤离森林", "玩家在森林"),
                new GameEvent("hunt", "猎杀狼群", "玩家有斧头"),
            };
            var registry = new EventRegistry(events);
            var llm = new LLMDirector(new StubProvider("{\"event\":\"flee\"}"), events);
            var runner = new EventDirectorRunner(llm, registry, TimeSpan.FromSeconds(3));

            var result = await runner.RunAsync(enriched, CancellationToken.None);
            Console.WriteLine($"导演决定：{Format(result)}");
            Console.WriteLine();

            // ---------- 高频主题 ----------
            Console.WriteLine("=== 这个 NPC 最近在想什么（高频词） ===");
            foreach (string topic in Reflection.TopTopics(memory, 3))
            {
                Console.WriteLine("  " + topic);
            }

            return 0;
        }

        private static string Format(DirectorResult r) => r.Outcome switch
        {
            DirectorOutcome.EventSelected => $"[{r.SelectedEvent!.Id}] {r.SelectedEvent.Title}",
            DirectorOutcome.NothingHappened => "无事发生",
            DirectorOutcome.InvalidId => $"非法事件 '{r.ProposedId}'",
            DirectorOutcome.TimeoutOrCancelled => "超时/取消",
            _ => $"错误 {r.ErrorMessage}",
        };
    }
}
