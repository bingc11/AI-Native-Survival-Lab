using System;
using System.Threading;
using System.Threading.Tasks;
using P00.Core;
using P01.Core;
using Xunit;

namespace P01.Tests;

/// <summary>
/// 收口人测试：验证"导演 → 验收 → 日志"整条线的五种结论。
/// </summary>
public class EventDirectorRunnerTests
{
    private static readonly GameEvent[] Events =
    {
        new("wolf_pack", "狼群来袭", "玩家在森林区域"),
        new("food_rot", "食物腐烂", "玩家有库存食物"),
    };

    private static EventRegistry Registry() => new(Events);

    private static RuleBasedDirector WolfDirector()
        => new(new[] { new RuleBasedDirector.Rule("森林", "wolf_pack") });

    /// <summary>慢模型：10 秒后才回（配合超时测试）。</summary>
    private sealed class SlowProvider : IGameAIProvider
    {
        public async Task<string> CompleteAsync(string query, CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
            return "{\"event\":\"wolf_pack\"}";
        }
    }

    /// <summary>一调用就炸的模型（配合 Error 测试）。</summary>
    private sealed class ThrowingProvider : IGameAIProvider
    {
        public Task<string> CompleteAsync(string query, CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    [Fact]
    public async Task ValidEvent_IsSelectedAndLogged()
    {
        var runner = new EventDirectorRunner(WolfDirector(), Registry());

        var result = await runner.RunAsync("玩家在森林里", CancellationToken.None);

        Assert.Equal(DirectorOutcome.EventSelected, result.Outcome);
        Assert.Equal("wolf_pack", result.SelectedEvent?.Id);
        Assert.Single(runner.Log);
    }

    [Fact]
    public async Task LlmInventsUnknownId_RejectedAsInvalid()
    {
        // 模型乱编 ufo_landing → 导演原样给 → 收口人查墙 → 拒绝
        var stub = new StubProvider("{\"event\":\"ufo_landing\"}");
        var llm = new LLMDirector(stub, Events);
        var runner = new EventDirectorRunner(llm, Registry());

        var result = await runner.RunAsync("玩家在森林里", CancellationToken.None);

        Assert.Equal(DirectorOutcome.InvalidId, result.Outcome);
        Assert.Equal("ufo_landing", result.ProposedId);
        Assert.Null(result.SelectedEvent);
    }

    [Fact]
    public async Task DirectorDeclines_NothingHappened()
    {
        var runner = new EventDirectorRunner(WolfDirector(), Registry());

        var result = await runner.RunAsync("玩家在洞穴睡觉", CancellationToken.None);

        Assert.Equal(DirectorOutcome.NothingHappened, result.Outcome);
    }

    [Fact]
    public async Task PreCancelled_TimeoutOrCancelled()
    {
        var llm = new LLMDirector(new SlowProvider(), Events);
        var runner = new EventDirectorRunner(llm, Registry(), TimeSpan.FromSeconds(30));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await runner.RunAsync("状态", cts.Token);

        Assert.Equal(DirectorOutcome.TimeoutOrCancelled, result.Outcome);
    }

    [Fact]
    public async Task SlowModel_TimeOut_TimeoutOrCancelled()
    {
        var llm = new LLMDirector(new SlowProvider(), Events);
        var runner = new EventDirectorRunner(llm, Registry(), TimeSpan.FromMilliseconds(100));

        var result = await runner.RunAsync("状态", CancellationToken.None);

        Assert.Equal(DirectorOutcome.TimeoutOrCancelled, result.Outcome);
    }

    [Fact]
    public async Task ProviderThrows_ErrorLogged()
    {
        var llm = new LLMDirector(new ThrowingProvider(), Events);
        var runner = new EventDirectorRunner(llm, Registry());

        var result = await runner.RunAsync("状态", CancellationToken.None);

        Assert.Equal(DirectorOutcome.Error, result.Outcome);
        Assert.Equal("boom", result.ErrorMessage);
    }

    [Fact]
    public async Task Log_AccumulatesPerRunWithOutcomes()
    {
        var runner = new EventDirectorRunner(WolfDirector(), Registry());

        await runner.RunAsync("玩家在森林里", CancellationToken.None);
        await runner.RunAsync("玩家在洞穴睡觉", CancellationToken.None);

        Assert.Equal(2, runner.Log.Count);
        Assert.Equal(DirectorOutcome.EventSelected, runner.Log[0].Outcome);
        Assert.Equal(DirectorOutcome.NothingHappened, runner.Log[1].Outcome);
    }
}
