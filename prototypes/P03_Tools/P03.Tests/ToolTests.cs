using System;
using P03.Core;
using Xunit;

namespace P03.Tests;

public class ToolRegistryTests
{
    [Fact]
    public void Contains_Registered_ReturnsTrue()
    {
        var registry = new ToolRegistry(new[] { new AITool("GiveItem", "给玩家物品") });

        Assert.True(registry.Contains("GiveItem"));
        Assert.False(registry.Contains("DeleteAllData")); // 白名单外
    }

    [Fact]
    public void Get_ReturnsTool_OrNull()
    {
        var registry = new ToolRegistry(new[] { new AITool("GiveItem", "给玩家物品") });

        Assert.NotNull(registry.Get("GiveItem"));
        Assert.Null(registry.Get("DeleteAllData"));
    }

    [Fact]
    public void DuplicateName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ToolRegistry(new[]
        {
            new AITool("GiveItem", "a"),
            new AITool("GiveItem", "b"),
        }));
    }
}

public class ToolExecutorTests
{
    private static (ToolExecutor Executor, ToolRegistry Registry) Setup()
    {
        var registry = new ToolRegistry(new[]
        {
            new AITool("GiveItem", "给玩家物品"),
            new AITool("SpawnEnemy", "刷一个敌人"),
        });
        var executor = new ToolExecutor(registry);
        executor.Register("GiveItem", args => $"给了物品: {args}");
        return (executor, registry);
    }

    [Fact]
    public void Execute_WhitelistedTool_Succeeds()
    {
        var (executor, _) = Setup();

        var result = executor.Execute(new ToolCall("GiveItem", "{\"item\":\"wood\",\"count\":5}"));

        Assert.Equal(ToolOutcome.Success, result.Outcome);
        Assert.Contains("给了物品", result.Output);
    }

    [Fact]
    public void Execute_UnknownTool_Rejected()
    {
        var (executor, _) = Setup();

        // AI 想调白名单外的工具 → 拒绝
        var result = executor.Execute(new ToolCall("DeleteAllData"));

        Assert.Equal(ToolOutcome.Rejected, result.Outcome);
        Assert.Contains("不在白名单", result.Error);
    }

    [Fact]
    public void Execute_RegisteredButNoImpl_Fails()
    {
        var (executor, _) = Setup();

        // SpawnEnemy 在白名单但没注册 handler
        var result = executor.Execute(new ToolCall("SpawnEnemy"));

        Assert.Equal(ToolOutcome.NoImplementation, result.Outcome);
    }

    [Fact]
    public void Register_NonWhitelisted_Throws()
    {
        var (executor, _) = Setup();

        Assert.Throws<ArgumentException>(
            () => executor.Register("DeleteAllData", _ => "boom"));
    }

    [Fact]
    public void Execute_HandlerThrows_ReturnsFailure()
    {
        var registry = new ToolRegistry(new[] { new AITool("Boom", "炸") });
        var executor = new ToolExecutor(registry);
        executor.Register("Boom", _ => throw new InvalidOperationException("炸了"));

        var result = executor.Execute(new ToolCall("Boom"));

        Assert.Equal(ToolOutcome.NoImplementation, result.Outcome);
        Assert.Contains("炸了", result.Error);
    }
}
