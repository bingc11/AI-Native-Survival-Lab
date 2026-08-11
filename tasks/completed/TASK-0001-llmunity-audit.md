# TASK-0001
审计 LLMUnity，为结构化输出调用链制作阅读路线。

## 状态
✅ 已完成（2026-08-07，用户已阅读确认）
- 产物：docs/source-studies/llmunity/{READING_GUIDE,SOURCE_CARD,MY_NOTES}.md
- 审计对象：undreamai/LLMUnity commit 2c30b44 / v3.0.3（Apache-2.0）
- 更正：原记录 URL ilciro/LLMUnity 已 404，官方仓库为 undreamai/LLMUnity

## 任务编号
TASK-0001

## 模式
SCOUT

## 背景
P00 Provider Probe（TASK-0001）需要理解第三方 Unity LLM 库 LLMUnity（仓库 ../references/LLMUnity/）的调用链、可扩展点和许可证限制，以便设计游戏层与模型层的隔离边界。

## 当前问题
LLMUnity 代码仓库是否已有本地副本？如果已有，主入口、Completion/LLM 接口、异步流程和可用回调路径分别是哪些？

## 输入资料
- 本地路径：../references/LLMUnity
- Git URL：https://github.com/ilciro/LLMUnity
- 许可证：未知，需要检查

## 允许修改范围
仅在 ../references/LLMUnity 上的独立分支上进行小型理解性修改。
不允许修改本仓库的核心代码。

## 禁止修改范围
- 不修改正式原型目录（本仓库下的 unity/）内容。
- 不提交任何第三方完整源码到本仓库。

## 必须回答的问题
1. 本地是否有干净的 checkout？如果没有，列出需要执行的命令来 clone（含 branch 或 tag）。
2. 本地浏览器/IDE 最容易进入的入口文件路径和类名。
3. 关键调用链（3-5 步）从游戏请求文本生成到最终收到结构化输出需要经过哪些节点。
4. 哪些类/方法构成官方扩展点（允许替换 Adapter、注入验证器或派生新 Guard）。
5. 在当前版本上，是否提供原生 JSON/Mode 结构化输出接口？如果需要，是如何实现的（解析/模式/示例数）。
6. 哪些目录/模块可以安全跳过而不影响 P00 设计？
7. 基于上述，给出 P00 最小修改建议（仅描述接口契约）。

## 预期产物
- docs/source-studies/llmunity/READING_GUIDE.md
- docs/source-studies/llmunity/SOURCE_CARD.md
- docs/source-studies/llmunity/MY_NOTES.md（留给用户）

## 验证方式
- Reading Guide 必须覆盖：入口 → 模型调用 → 响应解析 → 回传游戏层的节点。
- 可描述扩展点，格式：类 → 方法 → 用途。
- 可列举跳过目录，格式：目录 + 理由（与 P00 无关）。

## 阶段暂停点
一旦 READING_GUIDE.md 形成，本次任务暂停。不进入实现阶段。

## 最终退出条件
1. 本地仓库有 checkout/commit info。
2. 可列出 3 条关键调用链。
3. 可列举 3 个扩展点。
4. 可回答 P00 不应复用哪些模块。
5. 归档 SOURCE_CARD.md（包含本地路径、commit hash、license、运行环境）。

## 当前分支
- 第三方仓库：待确认（建议创建 audit/2026-08-04）
- 本仓库：main

## 交接要求
- 等待用户阅读 READING_GUIDE 并确认后再继续。
- 所有产物提交到 docs/source-studies/llmunity/。
- 不要在用户阅读前直接替用户填写完整 MY_NOTES。
