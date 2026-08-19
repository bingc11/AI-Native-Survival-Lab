using P04.Core.State;

namespace P04.Core.Commands
{
    /// <summary>
    /// 命令验证器：检查 Command 是否合法（条件满足）。
    /// AI proposes, game disposes 的"disposes"部分。
    /// </summary>
    public sealed class CommandValidator
    {
        /// <summary>验证命令是否可执行。</summary>
        public bool Validate(Command command, WorldState world)
        {
            switch (command.Action)
            {
                case "Eat":
                    // 必须有食物
                    return world.Resources.Get(world.Player.Location).Food >= command.Amount;

                case "GatherWood":
                    // 必须有木材
                    return world.Resources.Get(world.Player.Location).Wood >= command.Amount;

                case "Move":
                    // 目标地点必须存在
                    return !string.IsNullOrEmpty(command.Target) && world.Resources.Contains(command.Target);

                default:
                    return true; // 未知命令默认允许（可扩展）
            }
        }
    }
}
