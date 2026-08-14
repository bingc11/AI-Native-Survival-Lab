using System;
using System.Threading;
using System.Threading.Tasks;
using P01.Core;
using Xunit;

namespace P01.Tests;

/// <summary>
/// 游戏层调度器测试：组装 + 翻译 + 日志透传。
/// </summary>
public class EventSchedulerTests
{
    private static readonly GameEvent[] Events =
    {
        new("wolf_pack", "狼群来袭", "玩家在森林区域"),
        new("food_rot", "食物腐烂", "玩家有库存食物"),
    };

    private static EventRegistry Registry() => new(Events);

    private static RuleBasedDirector ForestWolfDirector()
        => new(new[] { new RuleBasedDirector.Rule("森林", "wolf_pack") });

    [Fact]
    public async Task DecideAsync_AreaMatchesRule_ReturnsEventSelected()
    {
        var scheduler = new EventScheduler(ForestWolfDirector(), Registry());

        var result = await scheduler.DecideAsync("森林", 100, "白天", CancellationToken.None);

        Assert.Equal(DirectorOutcome.EventSelected, result.Outcome);
        Assert.Equal("wolf_pack", result.SelectedEvent?.Id);
    }

    [Fact]
    public async Task DecideAsync_NoMatch_ReturnsNothingHappened()
    {
        var scheduler = new EventScheduler(ForestWolfDirector(), Registry());

        var result = await scheduler.DecideAsync("洞穴", 100, "白天", CancellationToken.None);

        Assert.Equal(DirectorOutcome.NothingHappened, result.Outcome);
    }

    [Fact]
    public async Task DecideAsync_AccumulatesInLog()
    {
        var scheduler = new EventScheduler(ForestWolfDirector(), Registry());

        await scheduler.DecideAsync("森林", 100, "白天", CancellationToken.None);
        await scheduler.DecideAsync("洞穴", 100, "夜晚", CancellationToken.None);

        Assert.Equal(2, scheduler.Log.Count);
        Assert.Equal(DirectorOutcome.EventSelected, scheduler.Log[0].Outcome);
        Assert.Equal(DirectorOutcome.NothingHappened, scheduler.Log[1].Outcome);
    }
}
