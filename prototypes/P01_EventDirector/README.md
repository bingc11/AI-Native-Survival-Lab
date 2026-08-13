# P01 Event Director

AI 事件导演（纯 C#，无 Unity 依赖）。AI 决定"接下来发生什么事件"，但只能从已注册事件表里挑。

## 结构（第一步落盘）

```
P01.Core/
  GameEvent.cs        事件数据（Id / Title / Requirement）
  EventRegistry.cs    合法事件唯一来源（围墙）；Contains/Get 是验收闸门
  IEventDirector.cs   导演合同：ChooseEventAsync(游戏状态) → 事件ID
P01.Tests/            EventRegistryTests（6 个测试）
```

## 后续将加入

- RuleBasedDirector / RandomDirector / LLMDirector（三个实现，策略模式）
- 导演调用方：查表验收 + 日志分类（非法/超时/错误）+ 兜底

## 设计要点（与 P00 一致）

- 接口只管"给状态 → 给 ID"，不保证合法；合法性由调用方查表验收
- 三个导演走同一入口，游戏层不感知背后是规则/随机/LLM
- 确定性执行边界：AI 可以自由发言，但只有过了验收的东西才允许影响游戏
