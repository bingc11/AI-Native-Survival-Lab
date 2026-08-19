using System;

namespace P04.Core.State
{
    /// <summary>
    /// 玩家状态：位置/生命/饥饿/体温。
    /// </summary>
    public sealed class PlayerState
    {
        public string Location { get; set; } = "Forest";
        public float Health { get; set; } = 100f;
        public float Hunger { get; set; } = 0f;       // 0=饱，100=饿死
        public float Temperature { get; set; } = 37f; // 体温

        public bool IsAlive => Health > 0f;

        /// <summary>饥饿上升（每 tick 自然上升）。</summary>
        public void IncreaseHunger(float amount)
        {
            Hunger = Math.Min(100f, Hunger + amount);
        }

        /// <summary>吃东西恢复饥饿。</summary>
        public void Eat(float amount)
        {
            Hunger = Math.Max(0f, Hunger - amount);
        }

        /// <summary>体温变化（受环境影响）。</summary>
        public void ChangeTemperature(float delta)
        {
            Temperature += delta;
        }

        /// <summary>受伤。</summary>
        public void TakeDamage(float amount)
        {
            Health = Math.Max(0f, Health - amount);
        }
    }
}
