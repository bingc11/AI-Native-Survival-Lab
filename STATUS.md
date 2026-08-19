# STATUS.md

## 当前阶段
P04 World Simulation Core（Headless 生存模拟），v1 骨架已落盘，检查修改中。

## 当前唯一主线
P04：检查并补全 World Simulation Core（Deterministic RNG / Replay / Evaluation / ActionSystem），然后带用户逐文件学习。

## 当前活动任务
P04 v1 已提交（925bd0c，7 测试绿）→ 已知问题修改 + 带学。

## 当前任务模式
BUILDER（v1 骨架）→ 修改 + TEACHING（带用户学 P04）

## 最近通过验证的 Commit
本仓库：925bd0c（P04 v1，dotnet test 7/7 绿）
历史：P00 13 绿 / P01 28 绿 / P02 10 绿 / P03 8 绿

## 当前阻塞
- 本机无 Unity 编辑器（P04 只做纯 C# 形态，Unity 接入延后）
- GitHub/NuGet 直连不稳，需走本机代理 127.0.0.1:7897

## 下一动作
1. 修 P04 已知问题（低温事件重复触发、Move 校验恒 true）+ 补缺失件（SeededRandom / Replay / Evaluation metrics / ActionSystem）
2. dotnet test 跑绿 → 提交
3. 带用户逐文件学 P04（WorldState → Clock → Command → Event → System → Runner）

## 当前明确不做的事项
- 不实现 P05 GOAP / Multi-Agent（留边界不写代码）
- 不实现 RAG / Semantic Interaction / Social Memory
- 不现在建 Unity 工程（学完 Oddssey 后单开项目）
- 不为 P04-P09 预造抽象框架
