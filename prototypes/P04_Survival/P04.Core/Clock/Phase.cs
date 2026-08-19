namespace P04.Core.Clock
{
    /// <summary>
    /// 一个 tick 内的有序阶段（解决冲突：进食在生存结算之前）。
    /// </summary>
    public enum Phase
    {
        Intent,        // 收集玩家/AI 的 Command
        Action,        // 执行合法 Command（吃东西先在此发生）
        Survival,      // 生存结算（饥饿/体温）
        Environment,   // 天气/资源
        Event          // 广播本 tick 产生的事件
    }
}
