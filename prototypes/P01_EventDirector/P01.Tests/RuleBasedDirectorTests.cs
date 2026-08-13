using System;
using System.Threading;
using System.Threading.Tasks;
using P01.Core;
using Xunit;

namespace P01.Tests;

public class RuleBasedDirectorTests
{
    private static RuleBasedDirector WolfOrCampDirector()
        => new(new[]
        {
            new RuleBasedDirector.Rule("森林", "wolf_pack"),
            new RuleBasedDirector.Rule("野外", "abandoned_camp"),
        });

    [Fact]
    public async Task MatchingKeyword_ReturnsThatRuleEventId()
    {
        var director = WolfOrCampDirector();

        string? id = await director.ChooseEventAsync("玩家正在森林里砍树", CancellationToken.None);

        Assert.Equal("wolf_pack", id);
    }

    [Fact]
    public async Task LaterRuleAlsoMatches_ReturnsFirstRule()
    {
        var director = WolfOrCampDirector();

        string? id = await director.ChooseEventAsync("玩家在森林里的野外迷路", CancellationToken.None);

        // 规则按顺序匹配，先命中先得
        Assert.Equal("wolf_pack", id);
    }

    [Fact]
    public async Task NoMatchingKeyword_ReturnsNull_AsNothingHappens()
    {
        var director = WolfOrCampDirector();

        string? id = await director.ChooseEventAsync("玩家在洞穴里睡觉", CancellationToken.None);

        Assert.Null(id); // null = 无事发生
    }

    [Fact]
    public async Task NullGameState_ReturnsNull()
    {
        var director = WolfOrCampDirector();

        string? id = await director.ChooseEventAsync(null!, CancellationToken.None);

        Assert.Null(id);
    }

    [Fact]
    public async Task CancelledToken_ThrowsImmediately()
    {
        var director = WolfOrCampDirector();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => director.ChooseEventAsync("森林", cts.Token));
    }
}
