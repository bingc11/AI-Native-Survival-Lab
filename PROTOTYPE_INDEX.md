# PROTOTYPE_INDEX.md
每个原型的目的、状态、运行情况、结论和迁移价值。

| 原型 | 目的 | 状态 | 运行情况 | 结论 | 迁移价值 | 目录 |
|------|------|------|---------|------|---------|------|
| P00_ProviderProbe | 通信层：验证 Unity 与模型运行时边界 | ✅ 完成 | dotnet test 13/13 绿 | IGameAIProvider 隔离 + Stub 基线 + JSON 校验/重试/兜底 | 一切 AI 调用的统一边界 | prototypes/P00_ProviderProbe/ |
| P01_EventDirector | 决策层：AI 只提已注册事件，游戏收口 | ✅ 完成 | dotnet test 28/28 绿 | RuleBased/Random/LLM 三策略 + 收口人 | P04 v2 导演接入点 | prototypes/P01_EventDirector/ |
| P02_Memory | 记忆层：存储/检索/反思 | ✅ 完成 | dotnet test 10/10 绿 | 相关性×时间衰减×重要性检索 | P08 Social Memory 的种子 | prototypes/P02_Memory/ |
| P03_Tools | 动作层：工具白名单 + 执行器 | ✅ 完成 | dotnet test 8/8 绿 | Register 注入、白名单拒绝恶意调用 | P05/P07 动作出口 | prototypes/P03_Tools/ |
| P04_Survival | World Simulation Core：Headless 生存模拟内核 | 🔄 v1 完成（925bd0c，7 测试绿），修改带学中 | dotnet test 7/7 绿 | WorldState/Clock/Command/Event/SurvivalSystem 已验证 | 迁入 Unity 3D 生存 Demo 的主干 | prototypes/P04_Survival/ |
| P05_GOAP | 规划 + 多 Agent | 计划中 | - | - | NPC 目标驱动决策 | - |
| P06_Perception | 感知/信念 + FSM/BT/GOAP | 计划中 | - | - | 敌人/环境 AI | - |
