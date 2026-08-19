namespace P04.Simulation
{
    public static class Program
    {
        public static void Main()
        {
            // Baseline 对照实验：无策略 vs 有策略，跑 100 tick
            HeadlessRunner.Run(100, withStrategy: false, compact: true);
            HeadlessRunner.Run(100, withStrategy: true, compact: true);
        }
    }
}