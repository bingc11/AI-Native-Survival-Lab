# HANDOFF.md
用于新主控会话接续。最新更新：2026-08-07

## 当前目标
- TASK-0001（LLMUnity SCOUT）已完成，等待用户阅读确认。

## 已完成
- 工作台同步：commit 63afeb3 已 push 至 origin/main。
- LLMUnity 已 clone 到 E:\references\LLMUnity（undreamai/LLMUnity，commit 2c30b44，v3.0.3，Apache-2.0）。
  - 注意：简报中的 ilciro/LLMUnity URL 已 404，官方仓库迁移为 undreamai/LLMUnity。
- 已产出 docs/source-studies/llmunity/：
  - READING_GUIDE.md（3 条调用链、6 个扩展点、可跳过目录、P00 接口契约建议）
  - SOURCE_CARD.md（commit/license/运行环境/复用判断）
  - MY_NOTES.md（空白占位，留给用户）
- SOURCE_INDEX.md / STATUS.md 已同步更新。

## 未完成
- 用户尚未阅读 READING_GUIDE 并确认（阶段暂停点）。
- P00 实现未开始（按任务规则不得提前进入实现阶段）。

## 工作区和分支状态
- E:\Unity-AIProjects，main 分支，与 origin/main 同步。
- git 直连 GitHub 不可达：push/clone 需 `git -c http.proxy=http://127.0.0.1:7897 ...`（未写入持久配置）。

## 测试状态
- 无（本阶段只读侦察）。

## 已知问题
- Git Credential Manager 曾报"页文件太小"崩溃；走代理后 push 正常，可能与页文件配置有关，如再遇可留意。
- native llama 库为预编译 DLL（Resources/），JSON schema→GBNF 转换在原生层，仓库内不可见。

## 不应重复的调查
- 不再重新 clone LLMUnity（已存在）。
- 不重复读整个 LLMUnity 仓库，只按 READING_GUIDE 走关键调用链。

## 下一步第一项动作
1. 请用户阅读 docs/source-studies/llmunity/READING_GUIDE.md（重点：第 6 节 7 个问题）。
2. 用户确认后由主控台决定：TASK-0001 关闭 + 是否启动 P00 BUILDER。
