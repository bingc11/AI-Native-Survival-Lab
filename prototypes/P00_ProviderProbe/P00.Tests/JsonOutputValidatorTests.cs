using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using P00.Core;
using Xunit;

namespace P00.Tests;

/// <summary>
/// 按给定输出序列依次响应的 provider（耗尽后重复最后一个）。
/// 用于精确控制重试路径上每次调用的返回值。
/// </summary>
public sealed class ScriptedProvider : IGameAIProvider
{
    private readonly Queue<Func<string, CancellationToken, Task<string>>> _steps;
    public int CallCount { get; private set; }

    public ScriptedProvider(params string[] outputs)
    {
        _steps = new Queue<Func<string, CancellationToken, Task<string>>>();
        foreach (string output in outputs)
        {
            string captured = output;
            _steps.Enqueue((_, _) => Task.FromResult(captured));
        }
    }

    public Task<string> CompleteAsync(string query, CancellationToken cancellationToken)
    {
        CallCount++;
        Func<string, CancellationToken, Task<string>> step =
            _steps.Count > 0 ? _steps.Dequeue() : (_, _) => Task.FromResult("{}");
        return step(query, cancellationToken);
    }

    public void QueueThrow(Exception exception)
    {
        _steps.Enqueue((_, _) => Task.FromException<string>(exception));
    }
}

public class JsonOutputValidatorTests
{
    private static readonly string[] Fields = { "intent", "target" };
    private const string Good = "{\"intent\":\"gather_wood\",\"target\":\"forest\"}";
    private const string Bad = "not json at all";
    private const string Missing = "{\"intent\":\"gather_wood\"}";

    private static JsonOutputValidator Create(IGameAIProvider inner, int maxRetries = 2)
        => new(inner, Fields, maxRetries);

    [Fact]
    public async Task ValidJson_PassesThroughUnmodified()
    {
        var inner = new ScriptedProvider(Good);
        var validator = Create(inner);

        string result = await validator.CompleteAsync("query", CancellationToken.None);

        Assert.Equal(Good, result);
        Assert.Equal(1, inner.CallCount); // 成功路径不重试
    }

    [Fact]
    public async Task InvalidJson_ExhaustsRetries_ThenFallsBack()
    {
        var inner = new ScriptedProvider(Bad);
        var validator = Create(inner, maxRetries: 2);

        string result = await validator.CompleteAsync("query", CancellationToken.None);

        Assert.Equal(3, inner.CallCount); // 1 次初始 + 2 次重试
        Assert.True(validator.TryValidate(result, out string? _), "兜底响应自身必须满足 schema");
    }

    [Fact]
    public async Task MissingRequiredField_FallsBack()
    {
        var inner = new ScriptedProvider(Missing);
        var validator = Create(inner, maxRetries: 0);

        string result = await validator.CompleteAsync("query", CancellationToken.None);

        Assert.NotEqual(Missing, result);
        Assert.True(validator.TryValidate(result, out string? _));
    }

    [Fact]
    public async Task FailsTwice_ThenSucceeds_ReturnsThirdResult()
    {
        var inner = new ScriptedProvider(Bad, Missing, Good);
        var validator = Create(inner, maxRetries: 3);

        string result = await validator.CompleteAsync("query", CancellationToken.None);

        Assert.Equal(Good, result);
        Assert.Equal(3, inner.CallCount);
    }

    [Fact]
    public async Task FallbackOutput_SatisfiesRequiredFields()
    {
        var inner = new ScriptedProvider(Bad, Bad, Bad);
        var validator = Create(inner, maxRetries: 0);

        string result = await validator.CompleteAsync("query", CancellationToken.None);

        Assert.True(validator.TryValidate(result, out string? error), error);
        Assert.Contains("intent", result);
        Assert.Contains("target", result);
    }

    [Fact]
    public async Task InnerCancellation_PropagatesToCaller()
    {
        var inner = new ScriptedProvider();
        var validator = Create(inner);
        using var cts = new CancellationTokenSource();

        inner.QueueThrow(new OperationCanceledException(cts.Token));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => validator.CompleteAsync("query", cts.Token));
    }
}
