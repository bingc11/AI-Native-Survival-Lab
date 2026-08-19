using System.Collections.Generic;
using P04.Core.Clock;
using P04.Core.Commands;
using P04.Core.Event;
using P04.Core.Simulation;
using P04.Core.State;
using P04.Core.Systems;
using Xunit;

namespace P04.Tests;

public class ActionSystemTests
{
    [Fact]
    public void Eat_ReducesHungerAndConsumesFood()
    {
        var world = new WorldState();
        world.Player.IncreaseHunger(80f); // 很饿
        var eventBus = new EventBus();
        var metrics = new SimulationMetrics();
        var actions = new ActionSystem(eventBus, metrics);

        actions.Submit(new Command("Eat", "Food", 1f));
        actions.Update(world, 0);

        Assert.Equal(80f - 30f, world.Player.Hunger);     // 30 饱食度
        Assert.Equal(10f - 1f, world.Resources.Get("Forest").Food); // 消耗 1 食物
        Assert.Equal(1, metrics.TotalActions);
    }

    [Fact]
    public void InvalidCommand_IsRejectedAndLogged()
    {
        var world = new WorldState();
        var eventBus = new EventBus();
        var metrics = new SimulationMetrics();
        var actions = new ActionSystem(eventBus, metrics);

        WorldEvent captured = null;
        eventBus.Subscribe(evt => captured = evt);

        actions.Submit(new Command("Move", "Moon")); // 不存在的地点
        actions.Update(world, 5);

        Assert.Equal("InvalidCommand", captured.Type);
        Assert.Equal(1, metrics.InvalidActions);
        Assert.Equal(0, metrics.TotalActions);
    }

    [Fact]
    public void Move_ChangesPlayerLocation()
    {
        var world = new WorldState();
        var actions = new ActionSystem(new EventBus());

        actions.Submit(new Command("Move", "River"));
        actions.Update(world, 0);

        Assert.Equal("River", world.Player.Location);
    }

    [Fact]
    public void CommandLoop_ThroughClock_ExecutesAtActionPhase()
    {
        var world = new WorldState();
        var eventBus = new EventBus();
        var clock = new SimulationClock();
        var actions = new ActionSystem(eventBus);
        clock.Register(actions);
        clock.Register(new SurvivalSystem(eventBus));

        world.Player.IncreaseHunger(90f);
        actions.Submit(new Command("Eat", "Food", 2f));

        clock.Tick(world); // Action 阶段应执行 Eat

        // 2 食物 = 60 饱食度：90 → 30，再被 Survival 的 +2 抵消一点
        Assert.Equal(90f - 60f + 2f, world.Player.Hunger);
        Assert.Equal(1, world.TickCount);
    }
}

public class SeededRandomTests
{
    [Fact]
    public void SameSeed_SameSequence()
    {
        var a = new SeededRandom(123);
        var b = new SeededRandom(123);

        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(a.Next(0, 1000), b.Next(0, 1000));
        }
        Assert.Equal(a.NextFloat(), b.NextFloat());
    }

    [Fact]
    public void DifferentSeed_DifferentSequence()
    {
        var a = new SeededRandom(1);
        var b = new SeededRandom(2);

        Assert.NotEqual(a.Next(0, 1000), b.Next(0, 1000));
    }
}

public class ReplayRecorderTests
{
    [Fact]
    public void RecordsAcceptedAndRejectedCommands()
    {
        var replay = new ReplayRecorder(seed: 7);

        replay.Record(0, new Command("Eat", "Food", 1f), accepted: true);
        replay.Record(1, new Command("Move", "Moon"), accepted: false);

        Assert.Equal(2, replay.Commands.Count);
        Assert.True(replay.Commands[0].Accepted);
        Assert.False(replay.Commands[1].Accepted);
        Assert.Equal("Eat", replay.Commands[0].Action);
        Assert.Equal(7, replay.Seed);
    }
}

public class MetricsTests
{
    [Fact]
    public void InvalidActionRate_IsComputed()
    {
        var metrics = new SimulationMetrics();
        metrics.RecordAction("Eat");
        metrics.RecordAction("Move");
        metrics.RecordInvalidAction();
        metrics.RecordInvalidAction();

        Assert.Equal(2, metrics.TotalActions);
        Assert.Equal(2, metrics.InvalidActions);
        Assert.Equal(0.5f, metrics.InvalidActionRate);
    }
}
