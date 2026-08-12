# P00 Provider Probe

验证 Unity 与模型运行时边界的最小原型（TASK-0002）。纯 C#，无 Unity 依赖。

## 结构

```
P00.Core/           类库（netstandard2.1，可被 Unity 工程引用）
  IGameAIProvider.cs      游戏层与模型层的唯一边界：CompleteAsync(query, ct)
  StubProvider.cs         非 AI 基线：固定输出、确定性、无网络
  JsonOutputValidator.cs  确定性执行边界：JSON 解析 + 必需字段校验 + 有限重试 + 兜底
P00.Tests/          xunit 自动测试（net10.0）
  StubProviderTests.cs        确定性 / 取消
  JsonOutputValidatorTests.cs 校验 / 重试 / 兜底 / schema 自洽 / 异常传播
  CancellationTests.cs        超时取消不阻塞调用线程
UnityAdapter/       LLMUnityAdapter.cs（代码形态，不编译；放入 Unity 工程后使用）
```

## 构建与测试

```powershell
cd prototypes/P00_ProviderProbe
dotnet test          # 需要 NuGet 时若失败，先设代理：$env:HTTPS_PROXY="http://127.0.0.1:7897"
```

## 关键设计

- 游戏层只依赖 `IGameAIProvider`，换模型不换游戏代码。
- `StubProvider` 是非 AI 基线（协议要求每个 AI 方案有确定性对照）。
- `JsonOutputValidator` 保证模型输出失控时游戏层只会收到「合法 JSON 或兜底 JSON」。
- 取消/超时：取消令牌贯穿 validator → provider，测试断言不阻塞调用线程（退出条件）。
