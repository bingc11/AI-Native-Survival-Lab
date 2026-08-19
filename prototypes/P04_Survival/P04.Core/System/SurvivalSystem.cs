using P04.Core.Clock;
using P04.Core.Event;
using P04.Core.State;

namespace P04.Core.System
{
    /// <summary>
    /// 生存系统：每 tick 推进饥饿/体温。
    /// </summary>
    public sealed class SurvivalSystem : ISystem
    {
        private readonly EventBus _eventBus;

        public int Interval => 1; // 每 tick 跑
        public Phase RunPhase => Phase.Survival;

        public SurvivalSystem(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Update(WorldState world, int currentTick)
        {
            // 饥饿自然上升
            world.Player.IncreaseHunger(2f);

            // 体温受环境影响
            float tempDelta = (world.Environment.AmbientTemperature - world.Player.Temperature) * 0.1f;
            world.Player.ChangeTemperature(tempDelta);

            // 检查低温状态
            if (world.Player.Temperature < 35f)
            {
                _eventBus.Publish(new WorldEvent("HypothermiaStarted", "玩家体温过低", currentTick));
                world.EventLog.Record($"Tick {currentTick}: Hypothermia started");
            }

            // 检查饿死
            if (world.Player.Hunger >= 100f)
            {
                world.Player.TakeDamage(10f);
                _eventBus.Publish(new WorldEvent("StarvationDamage", "玩家因饥饿受伤", currentTick));
            }
        }
    }
}
