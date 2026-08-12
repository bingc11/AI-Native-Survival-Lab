using System.Threading;
using System.Threading.Tasks;
using P00.Core;
using Xunit;

namespace P00.Tests;

public class StubProviderTests
{
    [Fact]
    public async Task CompleteAsync_ReturnsFixedResponse_Deterministically()
    {
        var stub = new StubProvider("{\"intent\":\"gather_wood\"}");

        string first = await stub.CompleteAsync("任意输入", CancellationToken.None);
        string second = await stub.CompleteAsync("任意输入", CancellationToken.None);

        Assert.Equal("{\"intent\":\"gather_wood\"}", first);
        Assert.Equal(first, second); // 非 AI 基线：确定性
    }

    [Fact]
    public async Task CompleteAsync_CancelledToken_ThrowsImmediately()
    {
        var stub = new StubProvider("{}");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => stub.CompleteAsync("query", cts.Token));
    }

    [Fact]
    public async Task CompleteAsync_IgnoresQueryContent()
    {
        var stub = new StubProvider("fixed");

        string a = await stub.CompleteAsync("", CancellationToken.None);
        string b = await stub.CompleteAsync(null!, CancellationToken.None);

        Assert.Equal("fixed", a);
        Assert.Equal("fixed", b);
    }
}
