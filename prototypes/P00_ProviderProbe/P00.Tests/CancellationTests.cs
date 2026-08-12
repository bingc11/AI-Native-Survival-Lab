using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using P00.Core;
using Xunit;

namespace P00.Tests;

/// <summary>
/// 验证「超时/取消不阻塞调用线程」这一退出条件。
/// </summary>
public class CancellationTests
{
    /// <summary>模拟慢模型：延迟可取消，内部无同步阻塞。</summary>
    private sealed class CancellableSlowProvider : IGameAIProvider
    {
        private readonly TimeSpan _delay;
        public int CallCount { get; private set; }

        public CancellableSlowProvider(TimeSpan delay) => _delay = delay;

        public async Task<string> CompleteAsync(string query, CancellationToken cancellationToken)
        {
            CallCount++;
            await Task.Delay(_delay, cancellationToken); // 尊重取消
            return "{\"intent\":\"slow\",\"target\":\"none\"}";
        }
    }

    [Fact]
    public async Task SlowProvider_CancelAfterTimeout_ReturnsPromptlyWithoutBlocking()
    {
        var inner = new CancellableSlowProvider(TimeSpan.FromSeconds(5));
        var validator = new JsonOutputValidator(inner, new[] { "intent", "target" });
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        var stopwatch = Stopwatch.StartNew();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => validator.CompleteAsync("query", cts.Token));
        stopwatch.Stop();

        // 若取消被吞或同步阻塞，会等到 5 秒才返回；要求远快于此。
        Assert.True(stopwatch.ElapsedMilliseconds < 2000,
            $"cancellation took {stopwatch.ElapsedMilliseconds}ms, expected < 2000ms");
        Assert.True(inner.CallCount >= 1, "slow provider should have been invoked");
    }

    [Fact]
    public async Task PreCancelledToken_ReturnsImmediately_WithoutInvokingInner()
    {
        var inner = new CancellableSlowProvider(TimeSpan.FromSeconds(5));
        var validator = new JsonOutputValidator(inner, new[] { "intent", "target" });
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var stopwatch = Stopwatch.StartNew();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => validator.CompleteAsync("query", cts.Token));
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 500,
            $"pre-cancelled path took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
        Assert.Equal(0, inner.CallCount); // 取消应在发起请求前生效
    }

    [Fact]
    public async Task Validator_PassesCancellationTokenToInner_SoSlowWorkCanBeAborted()
    {
        var inner = new CancellableSlowProvider(TimeSpan.FromSeconds(5));
        var validator = new JsonOutputValidator(inner, new[] { "intent", "target" });
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => validator.CompleteAsync("query", cts.Token));
    }
}
