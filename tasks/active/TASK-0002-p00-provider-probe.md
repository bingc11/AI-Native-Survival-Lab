# TASK-0002
实现 P00 Provider Probe：IGameAIProvider 接口 + Stub + 结构化输出验证器。

## 任务编号
TASK-0002

## 模式
BUILDER

## 目标
验证 Unity 与模型运行时边界：建立"游戏层 → 模型层"隔离接口，先实现非 AI 基线（Stub），再实现结构化输出（JSON）的解析、校验、有限重试与兜底，形成最小自动测试。为将来接入 LLMUnity Adapter 和 P01 Event Director 打底。

## 前置条件
- TASK-0001（LLMUnity SCOUT）已完成，READING_GUIDE 已确认。
- 关键结论：LLMUnity 提供 grammar（GBNF/JSON schema）采样期约束，但无输出校验/重试工具；扩展点在 LLMAgent.Chat override / SetupCallerObject。

## 允许修改
- 新增 `prototypes/P00_ProviderProbe/`（纯 C#，dotnet 10，.sln + Core 类库 + Tests）
- `PROTOTYPE_INDEX.md` 中 P00 一行的状态/目录/结论
- `docs/experiments/` 下 P00 实验记录（如有）

## 禁止修改
- `../references/LLMUnity`（保持只读）
- 不创建 Unity 工程（本环境无 Unity 编辑器；Adapter 只写代码形态，不加入 dotnet 编译）
- 不提交任何第三方完整源码
- 不修改 TASK-0001 产物与核心状态文件（除任务记录外）

## 必须实现
1. `IGameAIProvider` 接口：`Task<string> CompleteAsync(string query, CancellationToken ct)`，定义游戏层与模型层的唯一边界。
2. `StubProvider`：确定性、固定输出的非 AI 基线（每个 AI 方案必须有非 AI 基线）。
3. `LLMUnityAdapter`（代码形态，放 `UnityAdapter/` 子目录，不编译）：实现 IGameAIProvider，内部按 READING_GUIDE 契约包装 `LLMUnity.LLMAgent.Chat`，支持 grammar 注入、CancellationToken 超时/取消。
4. `JsonOutputValidator`：对 provider 输出做 JSON 解析 + 目标 schema 校验 + 有限重试 + 失败兜底（固定模板响应）。这是 READING_GUIDE 第 5 节的"确定性执行边界"。
5. 最小自动测试（xunit）：Stub 确定性、validator 的解析/校验/重试/兜底、超时与取消不阻塞调用线程。

## 验证方式
- `dotnet test`（prototypes/P00_ProviderProbe/）全部通过
- 超时/取消测试用 `Task.Delay` 模拟慢 provider，断言调用线程不被阻塞

## 退出条件
- Stub 稳定运行（测试通过、输出确定）
- 超时/取消不阻塞主线程（有测试覆盖）
- 存在最小自动测试（dotnet test 全绿）
- LLMUnityAdapter 代码形态完成，接口契约与 READING_GUIDE 一致

## 暂停点
- 实现完成 → 提交 → 交给 REVIEW 检查，再决定是否在 Unity 工程中实际加载模型运行（需要 Unity 环境）

## 当前分支
- 本仓库：main

## 交接要求
- 所有代码与测试提交到 prototypes/P00_ProviderProbe/
- 实现完成后暂停，等待 REVIEW，不擅自扩大范围
