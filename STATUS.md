# STATUS.md

## 当前阶段
P04 World Simulation Core（Headless 生存模拟），v2 完成（Command 闭环），带用户学习中。

## 当前唯一主线
P04：带用户逐文件学习已落地的 World Simulation Core（WorldState → Clock → Commands → Event → System → Simulation）。

## 当前活动任务
P04 v2 已提交（bf45fa3，15 测试绿）→ TEACHING（带用户学 P04，已讲到 SimulationClock/ISystem）。

## 当前任务模式
TEACHING（带用户逐文件学 P04）

## 最近通过验证的 Commit
本仓库：bf45fa3（P04 v2，dotnet test 15/15 绿）
历史：P00 13 绿 / P01 28 绿 / P02 10 绿 / P03 8 绿

## 当前阻塞
- 本机无 Unity 编辑器（P04 只做纯 C# 形态，Unity 接入延后）
- GitHub/NuGet 直连不稳，需走本机代理 127.0.0.1:7897

## 下一动作
1. 带用户学完 P04 剩余部分：Command 闭环 / ActionSystem / v2 新文件（SeededRandom / Replay / Metrics）
2. 用户理解后：P04 阶段收尾（用户手记 → 补进 Notion）→ 更新留档四件套

## 当前明确不做的事项
- 不实现 P05 GOAP / Multi-Agent（留边界不写代码）
- 不实现 RAG / Semantic Interaction / Social Memory
- 不现在建 Unity 工程（学完 Oddssey 后单开项目）
- 不为 P04-P09 预造抽象框架
