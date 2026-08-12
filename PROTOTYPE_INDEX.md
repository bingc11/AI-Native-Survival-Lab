# PROTOTYPE_INDEX.md
每个原型的目的、状态、运行情况、结论和迁移价值。

| 原型 | 目的 | 状态 | 运行情况 | 结论 | 迁移价值 | 目录 |
|------|------|------|---------|------|---------|------|
| P00_ProviderProbe | 验证 Unity 和模型运行时边界 | 进行中（TASK-0002，等 REVIEW） | dotnet test 12/12 通过（Core/Tests，纯 C#）；Unity 内实际模型运行待环境 | - | IGameAIProvider 隔离边界 + Stub 基线 + JSON 校验重试兜底 | prototypes/P00_ProviderProbe/ |
| P01_SurvivalEventDirector | 验证 AI 可提出且仅执行已注册事件 | 未开始 | - | - | 用于生存 Demo 事件系统基础 |
| P02_SurvivalCompanion | 验证 GOAP 在生存场景中的可行性 | 备选 | - | - | 用于生存 Demo NPC 或环境 AI |
