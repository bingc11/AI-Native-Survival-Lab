using System.Collections.Generic;
using System.Text;
using P04.Core.Commands;

namespace P04.Core.Simulation
{
    /// <summary>
    /// 重放记录器：记录"验证后的 Command 流"。
    /// 重放 = 用相同初始 WorldState + 相同 seed 重建，再按记录的命令流重跑。
    /// LLM 输出不进 replay（只在提交前消费，命令本身已是确定性输入）。
    /// </summary>
    public sealed class ReplayRecorder
    {
        /// <summary>单条命令记录。</summary>
        public sealed class CommandRecord
        {
            public int Tick { get; }
            public string Action { get; }
            public string Target { get; }
            public float Amount { get; }
            public bool Accepted { get; }

            public CommandRecord(int tick, string action, string target, float amount, bool accepted)
            {
                Tick = tick;
                Action = action;
                Target = target;
                Amount = amount;
                Accepted = accepted;
            }

            public Command ToCommand() => new Command(Action, Target, Amount);
        }

        private readonly List<CommandRecord> _commands = new List<CommandRecord>();

        /// <summary>本次模拟使用的随机种子。</summary>
        public int Seed { get; }

        public ReplayRecorder(int seed)
        {
            Seed = seed;
        }

        /// <summary>记录一条命令及是否被接受。</summary>
        public void Record(int tick, Command command, bool accepted)
        {
            _commands.Add(new CommandRecord(tick, command.Action, command.Target, command.Amount, accepted));
        }

        public IReadOnlyList<CommandRecord> Commands => _commands;

        /// <summary>导出重放文本（seed + 命令流），用于归档/重现。</summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# Replay Seed={Seed}");
            foreach (var c in _commands)
            {
                sb.AppendLine($"tick {c.Tick} {c.ToCommand()} => {(c.Accepted ? "accepted" : "rejected")}");
            }
            return sb.ToString();
        }
    }
}
