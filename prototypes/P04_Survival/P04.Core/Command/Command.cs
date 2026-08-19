using System;
using System.Collections.Generic;

namespace P04.Core.Commands
{
    /// <summary>
    /// 玩家/AI 的意图（Command）。
    /// </summary>
    public sealed class Command
    {
        public string Action { get; }       // 动作名（如 "Eat", "GatherWood"）
        public string Target { get; }       // 目标（如 "Food", "Forest"）
        public float Amount { get; }        // 数量（可选）

        public Command(string action, string target = "", float amount = 1f)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Target = target ?? "";
            Amount = amount;
        }

        public override string ToString() => $"{Action}({Target}, {Amount})";
    }
}
