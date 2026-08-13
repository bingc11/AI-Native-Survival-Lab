using System;
using System.Threading;
using System.Threading.Tasks;
using P01.Core;
using Xunit;

namespace P01.Tests;

public class RandomDirectorTests
{
    private static readonly string[] Candidates = { "wolf_pack", "food_rot", "abandoned_camp" };

    [Fact]
    public async Task HasCandidates_ReturnsOnlyFromCandidateSet()
    {
        var director = new RandomDirector(Candidates);

        for (int i = 0; i < 30; i++)
        {
            string? id = await director.ChooseEventAsync("任意状态", CancellationToken.None);
            Assert.Contains(id, Candidates); // 永远只从候选里出
        }
    }

    [Fact]
    public async Task NoCandidates_ReturnsNull()
    {
        var director = new RandomDirector(Array.Empty<string>());

        string? id = await director.ChooseEventAsync("任意状态", CancellationToken.None);

        Assert.Null(id); // 无事发生
    }

    [Fact]
    public async Task SameSeed_ProducesSameSequence()
    {
        var a = new RandomDirector(Candidates, new Random(42));
        var b = new RandomDirector(Candidates, new Random(42));

        for (int i = 0; i < 5; i++)
        {
            string? ia = await a.ChooseEventAsync("", CancellationToken.None);
            string? ib = await b.ChooseEventAsync("", CancellationToken.None);
            Assert.Equal(ia, ib); // 相同种子 → 相同结果（可复现）
        }
    }

    [Fact]
    public async Task DifferentSeed_GenerallyDiffers()
    {
        var a = new RandomDirector(Candidates, new Random(1));
        var b = new RandomDirector(Candidates, new Random(999));

        bool anyDifference = false;
        for (int i = 0; i < 20 && !anyDifference; i++)
        {
            string? ia = await a.ChooseEventAsync("", CancellationToken.None);
            string? ib = await b.ChooseEventAsync("", CancellationToken.None);
            anyDifference = ia != ib;
        }

        Assert.True(anyDifference, "不同种子应有不同序列（概率上几乎必然）");
    }

    [Fact]
    public async Task CancelledToken_ThrowsImmediately()
    {
        var director = new RandomDirector(Candidates);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => director.ChooseEventAsync("状态", cts.Token));
    }
}
