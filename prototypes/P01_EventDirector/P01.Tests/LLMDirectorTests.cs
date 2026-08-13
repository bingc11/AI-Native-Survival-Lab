using System;
using System.Threading;
using System.Threading.Tasks;
using P00.Core;
using P01.Core;
using Xunit;

namespace P01.Tests;

/// <summary>
/// LLM 导演测试：用 P00 的 StubProvider（假模型）扮演模型，
/// 让它"说"各种话，验证导演怎么处理。
/// </summary>
public class LLMDirectorTests
{
    private static readonly GameEvent[] Events =
    {
        new("wolf_pack", "狼群来袭", "玩家在森林区域"),
        new("food_rot", "食物腐烂", "玩家有库存食物"),
        new("abandoned_camp", "发现废弃营地", "玩家在野外"),
    };

    private static LLMDirector Create(IGameAIProvider provider)
        => new(provider, Events);

    [Fact]
    public async Task ModelReturnsValidJson_ReturnsEventId()
    {
        var stub = new StubProvider("{\"event\":\"wolf_pack\"}");
        var director = Create(stub);

        string? id = await director.ChooseEventAsync("玩家在森林里", CancellationToken.None);

        Assert.Equal("wolf_pack", id);
    }

    [Fact]
    public async Task ModelReturnsUnknownId_PassesItThrough()
    {
        // 模型乱编了一个墙外 ID —— 导演不拦，原样给出（验收是调用方的事）
        var stub = new StubProvider("{\"event\":\"ufo_landing\"}");
        var director = Create(stub);

        string? id = await director.ChooseEventAsync("玩家在森林里", CancellationToken.None);

        Assert.Equal("ufo_landing", id);
    }

    [Fact]
    public async Task ModelReturnsNoise_ReturnsNull()
    {
        // 模型回了废话，不是 JSON → 解析失败 → 无事发生
        var stub = new StubProvider("好啊那今天就先这样吧");
        var director = Create(stub);

        string? id = await director.ChooseEventAsync("玩家在森林里", CancellationToken.None);

        Assert.Null(id);
    }

    [Fact]
    public async Task ModelReturnsJsonWrappedInText_StillParsed()
    {
        // 模型废话+JSON 混在一起，容错解析
        var stub = new StubProvider("我选这个：{\"event\":\"food_rot\"} 结束。");
        var director = Create(stub);

        string? id = await director.ChooseEventAsync("玩家带着食物", CancellationToken.None);

        Assert.Equal("food_rot", id);
    }

    [Fact]
    public async Task CancelledToken_Propagates()
    {
        var stub = new StubProvider("{\"event\":\"wolf_pack\"}");
        var director = Create(stub);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => director.ChooseEventAsync("玩家在森林里", cts.Token));
    }
}
