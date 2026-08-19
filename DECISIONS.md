# DECISIONS.md
技术决策索引。

| 决策编号 | 标题 | 决策摘要 | 日期 | 状态 |
|----------|------|----------|------|------|
| ADR-0001 | 最小工作台与 Git 提交 | 阶段 A 不做完整 GUI 和大模板，使用最小文件结构 | 2026-08-04 | 已批准 |
| ADR-0002 | 学习路线重定位 | 从"横向补 LLM Agent 技能"改为"以 Headless 生存模拟为主干，融合传统 Game AI + LLM + AI-native"；路线 P04→P09（详见 ROADMAP.md） | 2026-08-13 | 已批准 |
| ADR-0003 | P04 时间模型 | 单个 SimulationClock（固定步长，1 tick=1 游戏小时）+ 每个 System 声明 Interval 与 Phase（Intent→Action→Survival→Environment→Event），不做多时钟对象 | 2026-08-13 | 已批准 |
| ADR-0004 | System 是唯一 State 修改者 | Command/AI 只能 propose，System 直接改 WorldState；Event 是系统改完后广播的事实；不用 Redux 式 Reducer | 2026-08-13 | 已批准 |
| ADR-0005 | Replay 语义 | 记录"验证后的 Command 流"（+ 初始 WorldState + seed），LLM 输出不进 replay；确定性系统用 seeded RNG | 2026-08-13 | 已批准 |
| ADR-0006 | WorldState 拆分 | 拆 5 子对象：Time/Player/Environment/Resource/EventLog（AgentState/ThreatState 留到 P05/P06） | 2026-08-13 | 已批准 |
| ADR-0007 | Unity 接入节奏 | 纯 C# 做到 P04-P06 闭环 → 学 Oddssey 补引擎基础 → 单开 Unity 项目搭基本生存架构 → 再接入 Core，并行演化 | 2026-08-13 | 已批准 |
| ADR-0008 | 留档纪律 | 每完成小阶段立即更新留档四件套（HANDOFF/STATUS/PROTOTYPE_INDEX/DECISIONS） | 2026-08-13 | 已批准 |
