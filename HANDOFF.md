# HANDOFF.md
用于新主控会话接续。最新更新：2026-08-07

## 当前目标
- TASK-0002：实现 P00 Provider Probe（IGameAIProvider + Stub + JSON 验证器 + 最小测试）。

## 已完成
- TASK-0001（LLMUnity SCOUT）完成并归档 tasks/completed/，用户已阅读确认。
- 交付物：docs/source-studies/llmunity/{READING_GUIDE,SOURCE_CARD,MY_NOTES}.md（commit 8278b9a）
- 关键结论：LLMUnity（undreamai，v3.0.3，Apache-2.0）提供 grammar 采样期约束，无输出校验/重试工具；扩展点在 LLMAgent.Chat override 与 SetupCallerObject。
- TASK-0002 任务文件已创建：tasks/active/TASK-0002-p00-provider-probe.md

## 未完成
- TASK-0002 实现（prototypes/P00_ProviderProbe/）尚未开始。
- Unity 工程未创建（本机无 Unity 编辑器）。

## 工作区和分支状态
- E:\Unity-AIProjects，main 分支。
- git 直连 GitHub 不可达：push/clone 需 `git -c http.proxy=http://127.0.0.1:7897 ...`（未写入持久配置）。
- 环境：dotnet 10.0.200 可用。

## 测试状态
- 无（P00 测试待 TASK-0002 实现）。

## 已知问题
- Git Credential Manager 曾报"页文件太小"崩溃；走代理后 push 正常。
- native llama 库为预编译 DLL，JSON schema→GBNF 转换在原生层，仓库内不可见。
- 无 Unity 编辑器：Adapter 只能写代码形态，实际加载模型需 Unity 环境。

## 不应重复的调查
- 不再重新 clone LLMUnity、不重读整个仓库。
- 不再做 SCOUT 级侦察，除非主控台指定新第三方源码。

## 下一步第一项动作
1. 实现 TASK-0002：prototypes/P00_ProviderProbe/（.sln + Core + Tests，dotnet test 全绿）。
2. 提交后暂停，等待 REVIEW。
