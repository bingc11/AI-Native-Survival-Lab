# HANDOFF.md
用于新主控会话接续。最新更新：2026-08-13

## 当前目标
P04 World Simulation Core（Headless 生存模拟）——已落第一版骨架（WorldState/Clock/Command/Event/SurvivalSystem，7 测试绿，commit 925bd0c），正在按新思路检查修改，然后带用户逐文件学习。

## 已完成（截至当前）
- P00 通信层（13 测试绿）/ P01 决策层（28 绿）/ P02 记忆层（10 绿）/ P03 动作层（8 绿）
- 四原型笔记已进 Notion：Long term Goals → 1.LLMUnity → 8.13-P00接口 / 8.13-P01事件导演 / 8.13-P02记忆 / 8.13-P03动作层
- P04 第一版骨架：prototypes/P04_Survival/（WorldState 5 子对象 + SimulationClock(Interval/Phase) + ISystem + Command/CommandValidator + EventBus/WorldEvent + SurvivalSystem + HeadlessRunner，7 测试绿，commit 925bd0c）

## 项目总目标（2026-08-13 调整，详见 ROADMAP.md）
以一个未来能迁入 Unity 3D 生存游戏的 Headless Gameplay Simulation 为主干，在真实游戏问题中融合传统 Game AI、LLM Agent 与 AI-native gameplay。
- **不再横向补 LLM Agent 技能**（RAG/Context/Prompt 逐个补），改为以游戏模拟为核心
- 路线：P04 World Sim → P05 GOAP+Multi-Agent → P06 Perception/Belief+FSM/BT/GOAP → P07 Semantic Interaction → P08 Social Memory → P09 RAG（按需）

## Unity 节奏（重要）
- 不是 P04-P09 全做完才进 Unity
- 顺序：纯 C# 做到 P04-P06 基本闭环 → **先学 Oddssey 补 Unity 引擎基础** → **单开 Unity 项目搭基本生存游戏** → 再把 Core 接进 Unity Vertical Slice
- 用户当前实习无法稳定用 Unity，先纯 C# 主线；Oddssey 与纯 C# 可穿插

## 教学方式（用户明确约定）
- **Track A 代码**：用户前期对代码无思路，**由 Agent 带跑大段 + 讲解**；用户有感觉后，关键设计点由用户先提结构、Agent 批评。Agent 管 boilerplate/重复实现/测试辅助/重构/解释；用户必须真懂 Domain Model/系统边界/invariant/算法/为什么用/不用某架构
- **Track B 理论**：工程问题→基础理论→论文→返回工程，不读脱离工程的论文；每个理论必答 8 问（见 ROADMAP）
- Evaluation 贯穿：机制都有 Baseline + 指标（Success Rate/Survival Time/Invalid Action Rate/Replan Count 等），禁止"看起来更聪明了"
- 用户是"手记型"：学完一块手记发我，整理进 Notion（手记底稿 + 补缺漏，不要概括）
- 讲解规范：直白、逐行、先结构后内容、少比喻；用户说看不懂就换更直白的说法重讲

## 留档纪律（新，写入了 CLAUDE.md）
每完成一个小阶段（原型/重大设计决策/教学里程碑）立即更新 HANDOFF/STATUS/PROTOTYPE_INDEX/DECISIONS，确保换 Agent 无缝。

## 工作区和环境
- E:\Unity-AIProjects，main 分支，与 origin/main 同步（925bd0c）
- git 直连 GitHub 不稳：push/clone 用 `git -c http.proxy=http://127.0.0.1:7897 ...`
- NuGet：dotnet restore/test 前设 `$env:HTTPS_PROXY`；dotnet 10.0.200；无 Unity 编辑器
- Notion：MCP 常断，用 API 直连（token 在 C:\Users\35706\.config\opencode\notion-token.txt；走代理 127.0.0.1:7897）
- 勿用 PowerShell Set-Content 改写带中文的源文件（破坏 UTF-8），用编辑器工具

## 已知问题
- Git Credential Manager 曾报"页文件太小"崩溃；走代理后正常
- LLMUnity 原生库预编译，JSON schema→GBNF 在原生层不可见

## 不应重复的调查
- 不再重做 P00-P03 的架构讨论（除非明确需要）
- 不再把路线理解为"横向补 Agent 技能"——以游戏模拟为主干
- 不再讨论"要不要先学 Unity"——已定：Oddssey 后单开项目（见 ROADMAP）

## 下一步第一项动作
1. 检查/修改 P04 第一版（已知问题：SurvivalSystem 低温事件每 tick 重复触发；CommandValidator 的 Move 检查恒 true；缺 Deterministic RNG / Replay / Evaluation metrics / ActionSystem）
2. 改好后带用户逐文件学 P04
