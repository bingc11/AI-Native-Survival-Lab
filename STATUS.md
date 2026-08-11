# STATUS.md

## 当前阶段
Phase A：Source Audit and Disposable Probes

## 当前唯一主线
TASK-0002：实现 P00 Provider Probe（IGameAIProvider + Stub + JSON 验证器 + 最小测试）。

## 当前活动任务
TASK-0002：BUILDER - P00 Provider Probe，验证 Unity 与模型运行时边界。

## 当前任务模式
BUILDER（实现）→ 完成后暂停，等待 REVIEW

## 最近通过验证的 Commit
本仓库：8278b9a（TASK-0001 SCOUT 交付物）
被审计源：LLMUnity 2c30b44 / v3.0.3（../references/LLMUnity）

## 当前阻塞
- GitHub 直连不可达，git 操作需走本机代理 127.0.0.1:7897（未写入 git 持久配置，单次命令 -c http.proxy 注入）。
- 本机无 Unity 编辑器：P00 以纯 C# 类库 + 自动测试落地；LLMUnityAdapter 只写代码形态不编译，实际加载模型需 Unity 环境。

## 下一动作
1. 实现 TASK-0002：prototypes/P00_ProviderProbe/（Core + Tests）。
2. `dotnet test` 全绿后提交，暂停等 REVIEW。

## 当前明确不做的事项
* 不立即实现 P00 原型代码。
* 不在本期创建 Unity 工程。
* 不克隆全部候选仓库。
* 不创建大型框架或空壳文件。
