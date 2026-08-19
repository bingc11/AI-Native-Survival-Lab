using P04.Core.Clock;
using P04.Core.Event;
using P04.Core.Systems;
using P04.Core.State;
using Xunit;

namespace P04.Tests;

public class WorldStateTests
{
    [Fact]
    public void TimeState_Advance_IncrementsHour()
    {
        var time = new TimeState();
        Assert.Equal(8, time.Hour);

        time.Advance();
        Assert.Equal(9, time.Hour);
    }

    [Fact]
    public void TimeState_Advance_RolloverToNextDay()
    {
        var time = new TimeState { };
        // 手动设置到 23 点
        for (int i = 0; i < 15; i++) time.Advance(); // 8→23
        Assert.Equal(23, time.Hour);
        Assert.Equal(1, time.Day);

        time.Advance(); // 23→0，Day+1
        Assert.Equal(0, time.Hour);
        Assert.Equal(2, time.Day);
    }

    [Fact]
    public void PlayerState_Eat_ReducesHunger()
    {
        var player = new PlayerState();
        player.IncreaseHunger(50f);
        Assert.Equal(50f, player.Hunger);

        player.Eat(30f);
        Assert.Equal(20f, player.Hunger);
    }

    [Fact]
    public void PlayerState_TakeDamage_ReducesHealth()
    {
        var player = new PlayerState();
        player.TakeDamage(30f);
        Assert.Equal(70f, player.Health);
        Assert.True(player.IsAlive);

        player.TakeDamage(80f);
        Assert.Equal(0f, player.Health);
        Assert.False(player.IsAlive);
    }
}

public class SimulationClockTests
{
    [Fact]
    public void Clock_TicksSystemsByInterval()
    {
        var world = new WorldState();
        var eventBus = new EventBus();
        var clock = new SimulationClock();
        var system = new SurvivalSystem(eventBus);

        clock.Register(system);

        // 跑 3 tick
        clock.Run(world, 3);

        // SurvivalSystem Interval=1，应该跑 3 次
        // 每次饥饿 +2，所以总饥饿 = 6
        Assert.Equal(6f, world.Player.Hunger);
        Assert.Equal(3, world.TickCount);
    }
}

public class SurvivalSystemTests
{
    [Fact]
    public void SurvivalSystem_IncreasesHunger()
    {
        var world = new WorldState();
        var eventBus = new EventBus();
        var system = new SurvivalSystem(eventBus);

        system.Update(world, 0);
        Assert.Equal(2f, world.Player.Hunger);
    }

    [Fact]
    public void SurvivalSystem_EmitsHypothermiaEvent()
    {
        var world = new WorldState();
        var eventBus = new EventBus();
        var system = new SurvivalSystem(eventBus);

        WorldEvent? capturedEvent = null;
        eventBus.Subscribe(evt => capturedEvent = evt);

        // 手动设置低温
        world.Player.ChangeTemperature(-5f); // 37→32
        Assert.True(world.Player.Temperature < 35f);

        system.Update(world, 10);

        Assert.NotNull(capturedEvent);
        Assert.Equal("HypothermiaStarted", capturedEvent!.Type);
    }
}
