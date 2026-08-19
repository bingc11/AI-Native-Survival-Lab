using System.Collections.Generic;
using P04.Core.Clock;
using P04.Core.Commands;
using P04.Core.Event;
using P04.Core.Simulation;
using P04.Core.State;

namespace P04.Core.Systems
{
    /// <summary>
    /// 动作系统：命令的执行者（Command 闭环的落点）。
    /// AI/玩家 Submit 命令 → 下一个 tick 的 Action 阶段校验 + 执行 → 状态变化 + 事件广播。
    /// 校验失败的命令被拒绝（记 invalid，供 Evaluation）。
    /// </summary>
    public sealed class ActionSystem : ISystem
    {
        private readonly Queue<Command> _queue = new Queue<Command>();
        private readonly CommandValidator _validator = new CommandValidator();
        private readonly EventBus _eventBus;
        private readonly SimulationMetrics _metrics;
        private readonly ReplayRecorder _replay;

        public int Interval => 1;
        public Phase RunPhase => Phase.Action;

        public ActionSystem(EventBus eventBus, SimulationMetrics metrics = null, ReplayRecorder replay = null)
        {
            _eventBus = eventBus;
            _metrics = metrics;
            _replay = replay;
        }

        /// <summary>提交一条命令（Intent）。在下一个 tick 的 Action 阶段执行。</summary>
        public void Submit(Command command)
        {
            _queue.Enqueue(command ?? new Command("Noop"));
        }

        public void Update(WorldState world, int currentTick)
        {
            while (_queue.Count > 0)
            {
                var cmd = _queue.Dequeue();

                if (_validator.Validate(cmd, world))
                {
                    Execute(cmd, world, currentTick);
                    _metrics?.RecordAction(cmd.Action);
                    _replay?.Record(currentTick, cmd, accepted: true);
                }
                else
                {
                    _metrics?.RecordInvalidAction();
                    _replay?.Record(currentTick, cmd, accepted: false);
                    _eventBus.Publish(new WorldEvent("InvalidCommand", $"命令被拒绝: {cmd}", currentTick));
                }
            }
        }

        private void Execute(Command cmd, WorldState world, int tick)
        {
            switch (cmd.Action)
            {
                case "Eat":
                    // 消耗食物，恢复饥饿（1 食物 = 30 饱食度）
                    world.Resources.Consume(world.Player.Location, "Food", cmd.Amount);
                    world.Player.Eat(cmd.Amount * 30f);
                    break;

                case "GatherWood":
                    // 采集木材（存在即可采集，无需库存）
                    world.Resources.Consume(world.Player.Location, "Wood", cmd.Amount);
                    break;

                case "Move":
                    world.Player.Move(cmd.Target);
                    break;
            }

            world.EventLog.Record($"Tick {tick}: {cmd} executed");
        }
    }
}
