# ROADMAP.md

## 总目标（2026-08-13 调整后）

以一个未来能迁入 Unity 3D 生存游戏的 **Headless Gameplay Simulation 为主干**，在真实游戏问题中逐步融合传统 Game AI、LLM Agent 与 AI-native gameplay。

不再是"横向补齐 LLM Agent 功能模块"，而是"以游戏模拟为核心，AI 是其中的一层"。

## 已完成：P00–P03 Agent 基础设施

| 原型 | 内容 | 状态 |
|------|------|------|
| P00 通信层 | IGameAIProvider + Stub + JSON 校验/重试/兜底 | ✅ 完成（13 测试绿） |
| P01 决策层 | 事件表白名单 + 三导演(策略) + 收口人 | ✅ 完成（28 测试绿） |
| P02 记忆层 | MemoryBank + 三因子检索 + 反思 | ✅ 完成（10 测试绿） |
| P03 动作层 | 工具白名单 + 执行器 + Register 注入 | ✅ 完成（8 测试绿） |

学习笔记已进 Notion（Long term Goals → 1.LLMUnity → 8.13-P00接口 / 8.13-P01事件导演 / 8.13-P02记忆 / 8.13-P03动作层）。

## 主干路线（P04 起，围绕 Headless Survival Simulation）

```
P04 World Simulation Core（当前）
    WorldState / Domain State / Time-Clock / Tick-Scheduled Update /
    Event-driven / Command-Intent / Rule-Validation / State Transition /
    Deterministic RNG / Replay / Headless Simulation / Evaluation Harness
    为后续留扩展边界：GOAP、Multi-Agent、Perception/Belief、
    Semantic Interaction、Unity Adapter、Headless Evaluation
    原则：当前问题驱动当前设计，未来需求只影响边界，不提前制造未来代码

P05 Planning + Multi-Agent
    Goal / Action Preconditions-Effects-Cost / State-space Search /
    GOAP / Replanning / Reservation / Job Assignment / Conflict

P06 Perception + Belief + 传统 Game AI
    Observation / Ground Truth vs Belief / Partial Observability /
    FSM / Behavior Tree / Utility AI / GOAP 对比

P07 Semantic Interaction（真正 AI-native mechanic）
    自然语言 → 语义表示 → Goal/Constraint/Rule/Intent → 验证 →
    传统 Planner/游戏系统 → 具体动作 → 持久世界后果
    研究问题："生成式 AI 提供了什么传统游戏 AI 无法提供的玩法能力？"

P08（按游戏需要）Persistent Social Memory
    Working/Episodic/Semantic Memory / Belief / Relationship / Trust /
    Knowledge Ownership / Provenance / Contradiction / Rumor

P09（只有玩法真正需要才进入）Knowledge / RAG
```

## Unity 接入节奏（重要）

**不要把 P04–P09 理解成必须全部纯 C# 完成才进 Unity。**

顺序：
1. 纯 C# 阶段：P04（→ P05 → P06）形成基本闭环
2. **先学 Oddssey 教程补 Unity 引擎基础**（MonoBehaviour/生命周期/协程/物理/输入/UI）
3. **单开 Unity 项目**搭建基本 3D 生存游戏架构（传统玩法打底）
4. 再把 P04 的 Core 接进 Unity Vertical Slice（AI 系统与 Unity Gameplay 并行演化）

当前（实习期无法稳定用 Unity）：继续纯 C# 主线。

## 学习方式（两个 Track）

### Track A：代码 / 代码架构
- C# 工程能力、Gameplay/Simulation/Game AI 架构、数据结构与算法、测试、可维护性、Unity 迁移边界
- **教学约定**：用户前期对代码无思路，由 Agent 带跑大段并讲解；用户有感觉后，关键设计点由用户先提结构、Agent 批评修正
- Agent 负责：boilerplate、重复实现、测试辅助、重构、解释
- 用户必须真懂：核心 Domain Model、系统边界、invariant、算法、为什么用/不用某架构

### Track B：理论 / 论文 / 深层知识
- 模式：**工程问题 → 基础理论 → 论文/工业系统 → 返回工程**（不做与工程脱节的随机阅读）
- 每个理论/论文必答 8 问：解决什么问题/为什么出现/核心思想/架构算法/怎么证明有效/局限/与当前实现的对应/是否应改变当前设计
- 重点论文：AI Native Games: A Survey and Roadmap、Generative Agents、Project Sid、AIvilization、GOAP/F.E.A.R.、Game AI Pro、SIMA/ACE（视野）

## Evaluation 思维（贯穿全程）

任何重要 AI/Game AI 机制保留 Baseline + 实验能力：
- 对比组：FSM vs GOAP；无 Memory vs 有 Memory；Rule Director vs LLM Director；GOAP vs GOAP+LLM；固定命令 vs Semantic Command
- 指标：Success Rate / Survival Time / Invalid Action Rate / Replan Count / Resource Efficiency / State Contradiction / Latency / LLM Calls / Token Cost
- 禁止用"看起来更聪明了"作为判断标准

## 每阶段学习循环

```
提出游戏问题 → 用户尝试分析/设计（有感觉后）→ Agent 批评设计 →
实现最小版本 → 遇到实际问题 → 补基础理论 → 读论文/工业案例 →
回头检查设计 → 修改实现 → Baseline/Ablation/Evaluation → 用户总结设计决策
```

终极目标：不只是"这个项目怎么写"，而是"为什么这样设计；背后的算法/AI/Simulation/软件工程原理；换一个游戏问题时能自己判断"。
