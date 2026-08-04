# ROADMAP.md

## Phase A：Source Audit and Disposable Probes
- 审计 LLMUnity 源码调用链
- 制作阅读路线，填充 READING_GUIDE.md
- 为 P00 建立模型接口边界
- 退出条件：LLMUnity 新增引用模块列表已固定，P00 接口契约可描述，源代码入口已明确定位。

## Phase B：P00 Provider Probe（计划中）
- 建立 IGameAIProvider 接口
- 实现 Stub Provider
- 实现至少一个真实 Adapter
- 实现结构化输出、JSON 解析与验证
- 退出条件：Stub 稳定运行，超时/取消不阻塞主线程，存在最小自动测试。

## Phase C：P01 Survival Event Director（计划中）
- 实现 RuleBasedDirector、RandomDirector、LLMDirector
- 保留非法/超时/错误分类日志
- 退出条件：P01 可在隔离场景中批量模拟，仅日志输出，无游戏状态污染。

## Phase D：Evaluate & Handle Off（计划中）
- 根据 P01 结果评估通往生存 Demo 的迁移价值。
- 若条件满足，创建下一阶段任务并移交。
