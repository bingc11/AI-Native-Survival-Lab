using System;
using P01.Core;
using Xunit;

namespace P01.Tests;

public class EventRegistryTests
{
    private static EventRegistry SampleRegistry()
        => new(new[]
        {
            new GameEvent("wolf_pack", "狼群来袭", "玩家在森林区域"),
            new GameEvent("food_rot", "食物腐烂", "玩家有库存食物"),
            new GameEvent("abandoned_camp", "发现废弃营地", "玩家在野外"),
        });

    [Fact]
    public void Contains_RegisteredEvent_ReturnsTrue()
    {
        var registry = SampleRegistry();

        Assert.True(registry.Contains("wolf_pack"));
        Assert.True(registry.Contains("food_rot"));
    }

    [Fact]
    public void Contains_UnknownEvent_ReturnsFalse()
    {
        var registry = SampleRegistry();

        // 模型可能编造墙外 ID（ufo_landing），必须被拒绝
        Assert.False(registry.Contains("ufo_landing"));
        Assert.False(registry.Contains(""));
        Assert.False(registry.Contains(null!));
    }

    [Fact]
    public void Get_RegisteredEvent_ReturnsEvent()
    {
        var registry = SampleRegistry();

        var found = registry.Get("wolf_pack");

        Assert.NotNull(found);
        Assert.Equal("狼群来袭", found.Title);
    }

    [Fact]
    public void Get_UnknownEvent_ReturnsNull()
    {
        var registry = SampleRegistry();

        Assert.Null(registry.Get("ufo_landing"));
    }

    [Fact]
    public void All_ReturnsEveryRegisteredEvent()
    {
        var registry = SampleRegistry();

        Assert.Equal(3, registry.All.Count);
        Assert.Equal(new[] { "wolf_pack", "food_rot", "abandoned_camp" },
            registry.All.Select(e => e.Id));
    }

    [Fact]
    public void DuplicateId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new EventRegistry(new[]
        {
            new GameEvent("wolf_pack", "狼群来袭"),
            new GameEvent("wolf_pack", "重复ID"),
        }));
    }
}
