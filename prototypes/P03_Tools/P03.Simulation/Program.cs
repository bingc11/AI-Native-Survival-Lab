using P03.Core;

namespace P03.Simulation
{
    public static class Program
    {
        public static int Main()
        {
            Console.WriteLine("=== P03 动作层：工具调用（function calling） ===\n");

            // ---------- 工具白名单（AI 只能调这些） ----------
            var registry = new ToolRegistry(new[]
            {
                new AITool("GiveItem", "给玩家物品"),
                new AITool("GetPlayerHealth", "读取玩家血量"),
                new AITool("SpawnEnemy", "在玩家附近刷一个敌人"),
            });

            // ---------- 执行器 + 游戏侧注入真实能力 ----------
            var executor = new ToolExecutor(registry);
            int playerHealth = 70;
            executor.Register("GiveItem", args => $"背包 +{args}");
            executor.Register("GetPlayerHealth", _ => $"玩家血量 {playerHealth}");
            executor.Register("SpawnEnemy", args => $"刷出敌人: {args}");

            // ---------- 模拟"模型想做事"：输出工具调用请求 ----------
            Console.WriteLine("[模型] 我注意到玩家血低了，想给他回血药：");
            var call1 = new ToolCall("GiveItem", "{\"item\":\"health_potion\",\"count\":1}");
            Print(executor, call1);

            Console.WriteLine("\n[模型] 顺便想看看玩家血量：");
            var call2 = new ToolCall("GetPlayerHealth", "{}");
            Print(executor, call2);

            Console.WriteLine("\n[模型] 想直接删除所有存档（恶意的！）：");
            var call3 = new ToolCall("DeleteAllData", "{}");
            Print(executor, call3);

            return 0;
        }

        private static void Print(ToolExecutor executor, ToolCall call)
        {
            var r = executor.Execute(call);
            Console.WriteLine($"  {call}");
            Console.WriteLine($"  → [{r.Outcome}] {r.Output ?? r.Error}");
        }
    }
}
